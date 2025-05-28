using KUtilities.TaskExtentions;
using LaserCali.Services.Environment.Events;
using LaserCali.Services.Environment.Formatter;
using LaserCali.Services.Environment.Models.ConfigOption;
using LaserCali.Services.Environment.Models.Message;
using LaserCali.Services.Environment.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment
{
    public class KEnvironmentSerial
    {
        readonly KAsyncEvent<KNohmiEnvironment_EventArg> _recievedMessage = new KAsyncEvent<KNohmiEnvironment_EventArg>();
        readonly KAsyncEvent<KNohmiLog_EventArg> _logEvent = new KAsyncEvent<KNohmiLog_EventArg>();
        readonly KAsyncEvent<KNohmiException_EventArg> _exceptionEvent = new KAsyncEvent<KNohmiException_EventArg>();
        readonly KAsyncEvent<KNohmiConnection_EventArgs> _onConnectionEvent = new KAsyncEvent<KNohmiConnection_EventArgs>();



        public event Func<KNohmiEnvironment_EventArg, Task> OnRecievedMessageAsync
        {
            add => _recievedMessage.AddHandler(value);
            remove => _recievedMessage.RemoveHandler(value);
        }

        public event Func<KNohmiLog_EventArg, Task> OnLogAsync
        {
            add => _logEvent.AddHandler(value);
            remove => _logEvent.RemoveHandler(value);
        }

        public event Func<KNohmiException_EventArg, Task> OnExceptionAsync
        {
            add => _exceptionEvent.AddHandler(value);
            remove => _exceptionEvent.RemoveHandler(value);
        }

        public event Func<KNohmiConnection_EventArgs, Task> OnConnectionAsync
        {
            add => _onConnectionEvent.AddHandler(value);
            remove => _onConnectionEvent.RemoveHandler(value);
        }

        KNohmiTransport _transport;
        CancellationTokenSource _backgroundCancelTokenSource = new CancellationTokenSource();
        Task _taskEvent;
        Task _taskAutoReconnect;
        Task _taskStop;
        Task _taskDecode;
        KAsyncQueue<Exception> _stopQueue = new KAsyncQueue<Exception>();
        object _lockStop = new object();
        int _isComportOpened = 0;
        KNohmiFormatter _formatter = new KNohmiFormatter();
        public bool IsRunning { get; private set; } = false;
        long _isConnected = 0;

        KAsyncQueue<KNohmiTransport_EventArgs> _transportMsgQueue = new KAsyncQueue<KNohmiTransport_EventArgs>(50000);
        KAsyncQueue<KEnvironmentBaseMessage> _eventQueue = new KAsyncQueue<KEnvironmentBaseMessage>(50000);
        KNohmiSerialOptions _option;
        public void Run(KNohmiSerialOptions option)
        {
            this._option = option;

            _backgroundCancelTokenSource = new CancellationTokenSource();
            CancellationToken c = _backgroundCancelTokenSource.Token;
            _stopQueue = new KAsyncQueue<Exception>();
            _transportMsgQueue = new KAsyncQueue<KNohmiTransport_EventArgs>();
            _eventQueue = new KAsyncQueue<KEnvironmentBaseMessage>();
            _taskEvent = Task.Run(() => ProcessInflightEvent(c), c);
            _taskStop = Task.Run(() => ProcessStopAllTask(c), c);
            _taskAutoReconnect = Task.Run(() => ProcessAutoReconnect(c), c);
            _taskDecode = Task.Run(() => ProcessDecode(c), c);
            WriteLog("nohmi running...");
        }

        private async Task ProcessDecode(CancellationToken c)
        {
            while (!c.IsCancellationRequested)
            {
                try
                {
                    var msgQueue = await _transportMsgQueue.TryDequeueAsync(c).ConfigureAwait(false);
                    if (msgQueue.IsSuccess)
                    {
                        //WriteLog("rect frame:" + msgQueue.Item.Message);
                        var result = _formatter.Decode(msgQueue.Item.Message, msgQueue.Item.Raw);
                        if (result != null)
                        {

                            EnqueueMessage(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog("exception decode message nohmi", ex);
                }
            }
        }

        private async Task ProcessAutoReconnect(CancellationToken c)
        {
            while (!c.IsCancellationRequested)
            {
                try
                {
                    if (_isComportOpened == 0)
                    {
                        WriteLog("reconnect nohmi serial");
                        // cố gắng mở lại kết nối
                        SerialPortStart();
                        IsRunning = true;
                        Interlocked.Exchange(ref _isComportOpened, 1);// trạng thái báo hiệu comport đã mở
                        WriteLog("reconnect success, nohmi serial running...");
                    }
                }
                catch (Exception ex)
                {
                    IsRunning = false;
                    WriteLog("exception auto reconnect nohmi ", ex);
                }
                await Task.Delay(500, c).ConfigureAwait(false);
            }
        }


        private async Task ProcessInflightEvent(CancellationToken c)
        {
            while (!c.IsCancellationRequested)
            {
                try
                {
                    var msgQueue = await _eventQueue.TryDequeueAsync(c).ConfigureAwait(false);
                    if (msgQueue.IsSuccess)
                    {
                        var eventData = msgQueue.Item;
                        if (eventData is KNohmiEnvironmentMessage point)
                        {
                            if (_recievedMessage.HasHandlers)
                            {
                                await _recievedMessage.InvokeAsync(
                                    new KNohmiEnvironment_EventArg(this, point)).ConfigureAwait(false);
                            }
                        }
                        else if (eventData is KNohmiLogMessage log)
                        {
                            if (_logEvent.HasHandlers)
                            {
                                var isConnect = Interlocked.Read(ref _isConnected);
                                if (isConnect == 0)
                                {
                                    Interlocked.Exchange(ref _isConnected, 1);
                                    await _onConnectionEvent.InvokeAsync(new KNohmiConnection_EventArgs(this, true));
                                }
                                await _logEvent.InvokeAsync(new KNohmiLog_EventArg(this, log.Content)).ConfigureAwait(false);
                            }
                        }
                        else if (eventData is KNohmiExceptionMessage ex)
                        {
                            if (_exceptionEvent.HasHandlers)
                            {
                                var isConnect = Interlocked.Read(ref _isConnected);
                                if (isConnect != 0)
                                {
                                    Interlocked.Exchange(ref _isConnected, 0);
                                    await _onConnectionEvent.InvokeAsync(new KNohmiConnection_EventArgs(this, false));
                                }
                                await _exceptionEvent.InvokeAsync(new KNohmiException_EventArg(this, ex.Ex)).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void EnqueueMessage(KEnvironmentBaseMessage eventData)
        {
            _eventQueue.Enqueue(eventData);
        }

        private void WriteLog(string message, Exception ex)
        {
            EnqueueMessage(new KNohmiExceptionMessage(DateTime.Now, ex, message));
        }

        private void WriteLog(string message)
        {
            EnqueueMessage(new KNohmiLogMessage(DateTime.Now, message));
        }

        private void SerialPortStart()
        {
            _transport = new KNohmiTransport();
            _transport.Closed += _transport_Closed;
            _transport.OnExceptionOccur += _transport_OnExceptionOccur;
            _transport.OnMessageRecieved += _transport_OnMessageRecieved;
            _transport.Open(_option);
        }

        private void _transport_OnMessageRecieved(object sender, Events.KNohmiTransport_EventArgs e)
        {
            _transportMsgQueue.Enqueue(e);
        }

        private void _transport_OnExceptionOccur(object sender, Exception ex)
        {
            KNohmiExceptionMessage msgEvent = new KNohmiExceptionMessage(DateTime.Now, ex, "Exception serial port");
            EnqueueMessage(msgEvent);
        }

        private void _transport_Closed(object sender, EventArgs e)
        {
            Interlocked.Exchange(ref _isComportOpened, 0);
        }

        private async Task ProcessStopAllTask(CancellationToken c)
        {
            try
            {
                while (!c.IsCancellationRequested)
                {
                    var stop = await _stopQueue.TryDequeueAsync(c).ConfigureAwait(false);
                    if (stop.IsSuccess)
                    {
                        await CloseCoreAsync(stop.Item).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task CloseCoreAsync(Exception ex)
        {
            lock (_lockStop)
            {
                _backgroundCancelTokenSource?.Cancel();
            }
            await _transport.DisconnectAsync().ConfigureAwait(false);
            IsRunning = false;
            await WaitForTask(_taskAutoReconnect).ConfigureAwait(false);
            await WaitForTask(_taskEvent).ConfigureAwait(false);
            await WaitForTask(_taskDecode).ConfigureAwait(false);
            _transportMsgQueue.Clear();
            _eventQueue.Clear();
            if (_onConnectionEvent.HasHandlers)
            {
                await _onConnectionEvent.InvokeAsync(new KNohmiConnection_EventArgs(this, false)).ConfigureAwait(false);
            }
        }

        private void OnClosing(Exception e)
        {
            lock (_lockStop)
            {
                if (!_backgroundCancelTokenSource.Token.IsCancellationRequested)
                {
                    _stopQueue.Enqueue(e);
                }
            }

        }

        public async Task DisconnectAsync()
        {
            this.OnClosing(new Exception("disconnect by require"));
            await WaitForTask(_taskStop).ConfigureAwait(false);
        }

        private async Task WaitForTask(Task task)
        {
            try
            {
                if (task != null && !task.IsCompleted)
                    await task.ConfigureAwait(false);
            }
            catch (Exception)
            {

            }
        }
    }
}
