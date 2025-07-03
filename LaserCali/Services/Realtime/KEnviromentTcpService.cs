using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using KModbus.Data.Options;
using KModbus.Interfaces;
using KModbus.IO;
using KModbus.Message;
using KModbus.Service;
using KModbus.Service.Model;
using KUtilities.ConvertExtentions;
using KUtilities.TaskExtentions;
using LaserCali.Services.Environment.Events;
using LaserCali.Services.Environment.Formatter;
using LaserCali.Services.Environment.Models.ConfigOption;
using LaserCali.Services.Environment.Models.Message;
using LaserCali.Services.Environment.Transport;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserCali.Services.Realtime
{
    public class KEnviromentTcpService
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

        ModbusMasterRtu_Runtime _transport;
        CancellationTokenSource _backgroundCancelTokenSource = new CancellationTokenSource();
        Task _taskEvent;
        Task _taskStop;
        Task _taskDecode;
        Task _taskSendData;
        KAsyncQueue<Exception> _stopQueue = new KAsyncQueue<Exception>();
        object _lockStop = new object();
        KNohmiFormatter _formatter = new KNohmiFormatter();
        public bool IsRunning { get; private set; } = false;
        long _isConnected = 0;

        KAsyncQueue<KNohmiTransport_EventArgs> _transportMsgQueue = new KAsyncQueue<KNohmiTransport_EventArgs>(50000);
        KAsyncQueue<KEnvironmentBaseMessage> _eventQueue = new KAsyncQueue<KEnvironmentBaseMessage>(50000);
        public void Run(string host,int port)
        {
            _backgroundCancelTokenSource = new CancellationTokenSource();
            CancellationToken c = _backgroundCancelTokenSource.Token;
            _stopQueue = new KAsyncQueue<Exception>();
            _transportMsgQueue = new KAsyncQueue<KNohmiTransport_EventArgs>();
            _eventQueue = new KAsyncQueue<KEnvironmentBaseMessage>();
            SerialPortStart(host, port);
            _taskEvent = Task.Run(() => ProcessInflightEvent(c), c);
            _taskStop = Task.Run(() => ProcessStopAllTask(c), c);
            _taskDecode = Task.Run(() => ProcessDecode(c), c);
            _taskSendData=Task.Run(() => ProcessSendData(c), c);
            WriteLog("nohmi running...");
        }

        private async Task ProcessSendData(CancellationToken c)
        {
            while(!c.IsCancellationRequested)
            {
                try
                {
                    var res1 = await _transport.SendCommandNoRepeatAsync(new ReadHoldingRegisterRequest(241, 0, 4), new CancellationTokenSource().Token);
                    KNohmiEnvironmentMessage msg=null;
                    if (res1.Type == KModbus.Data.EModbusCmdResponseType.Success)
                    {
                        var request = (ReadHoldingRegisterRequest)res1.ResultObj.Request;
                        var response = (ReadHoldingRegisterResponse)res1.ResultObj.Response;
                        var f_reg = response.Register;
                        var b_reg = ConvertReg.ConvertArrayUint16ToByte(response.Register);
                        int idx = 0;
                        var RH = ConvertVariable.BytesToFloat(b_reg, ref idx);
                        var TEMP = ConvertVariable.BytesToFloat(b_reg, ref idx);
                        msg = new KNohmiEnvironmentMessage()
                        {
                            Humi = RH, // quy đổi sang mmHg
                            Temp = TEMP,
                        };
                    }
                    var res2 = await _transport.SendCommandNoRepeatAsync(new ReadHoldingRegisterRequest(240, 42, 2), new CancellationTokenSource().Token);
                    if (res2.Type == KModbus.Data.EModbusCmdResponseType.Success)
                    {
                        var request = (ReadHoldingRegisterRequest)res2.ResultObj.Request;
                        var response = (ReadHoldingRegisterResponse)res2.ResultObj.Response;
                        var f_reg = response.Register;
                        var b_reg = ConvertReg.ConvertArrayUint16ToByte(response.Register);
                        int idx = 0;
                        var pressure = ConvertVariable.BytesToFloat(b_reg, ref idx);
                        if (msg != null)
                        {
                            msg.Pressure= pressure * 0.75; // quy đổi sang mmHg
                            EnqueueMessage(msg);
                        }
                    }
                    await Task.Delay(200);
                }
                catch (Exception)
                {
                    //WriteLog("exception send message nohmi", ex);
                    await Task.Delay(500);
                }
            }
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
                                var isConnect = Interlocked.Read(ref _isConnected);
                                if (isConnect == 0)
                                {
                                    Interlocked.Exchange(ref _isConnected, 1);
                                    await _onConnectionEvent.InvokeAsync(new KNohmiConnection_EventArgs(this, true));
                                }
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

        private void SerialPortStart(string host,int port)
        {
            var tcpOption = new ModbusClientTcpChannelOptions();
            tcpOption.Server =host;
            tcpOption.Port = port;
            tcpOption.Timeout = TimeSpan.FromSeconds(10);
            var adapter = new ModbusTcpClientTransport(new ModbusClientTcpOptions()
            {
                PacketProtocal = EModbusPacketProtocal.TcpIp,
                TcpOption = tcpOption,
                TimeOutConnect = 10,
                TransactionId = 1
            });
            _transport = new ModbusMasterRtu_Runtime(adapter);
            _transport.OnRecievedMessageAsync += ModbusMaster_OnRecievedMessageAsync; ;
            _transport.OnNoRespondMessageAsync += ModbusMaster_OnNoRespondMessageAsync; ;
            _transport.OnExceptionAsync += ModbusMaster_OnExceptionAsync; ;
            _transport.OnClosedConnectionAsync += ModbusMaster_OnClosedConnectionAsync; ;
            List<CommandModbus_Service> listCmd = new List<CommandModbus_Service>();
            _transport.RunAutoConnectAsync(new KModbus.Config.KModbusMasterOption()
            {
                DelayResponse = 10,
                IsAutoReconnect = true,
                ListCmd = listCmd,
                MsSleep = 0,
                WaitResponse = 1500,
                Retry = 1
            });
        }

        private Task ModbusMaster_OnClosedConnectionAsync(KModbus.Service.Event.Child.MsgClosedConnectionEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task ModbusMaster_OnExceptionAsync(KModbus.Service.Event.Child.MsgExceptionEventArgs arg)
        {
            KNohmiExceptionMessage msgEvent = new KNohmiExceptionMessage(DateTime.Now, arg.Ex, "Exception serial port");
            EnqueueMessage(msgEvent);
            return Task.CompletedTask;
        }

        private Task ModbusMaster_OnNoRespondMessageAsync(KModbus.Service.Event.MsgNoResponseModbus_EventArg arg)
        {
            return Task.CompletedTask;
        }

        private Task ModbusMaster_OnRecievedMessageAsync(KModbus.Service.Event.MsgResponseModbus_EventArg arg)
        {
            return Task.CompletedTask;
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
            await WaitForTask(_taskEvent).ConfigureAwait(false);
            await WaitForTask(_taskDecode).ConfigureAwait(false);
            await WaitForTask(_taskSendData).ConfigureAwait(false);
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
