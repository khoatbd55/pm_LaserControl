using KUtilities.TaskExtentions;
using LaserCali.Services.Environment.Events;
using LaserCali.Services.Environment.Models.ConfigOption;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Transport
{
    public class KNohmiTransport
    {
        // Delegate for MessageModbus Event Handle
        public delegate void MessageRecieveEventHandle(object sender, KNohmiTransport_EventArgs e);
        // Delegate for connection close
        public delegate void ConnectionCloseEventHandle(object sender, EventArgs e);
        //
        public delegate void ExceptionOccurEventHandle(object sender, Exception ex);

        public event ExceptionOccurEventHandle OnExceptionOccur;
        // event for Message Modbus Recieved
        public event MessageRecieveEventHandle OnMessageRecieved;
        // event for Connection closed
        public event ConnectionCloseEventHandle Closed;

        protected SerialPort _serialPort;
        private CancellationTokenSource _backgroundCancellationSource = new CancellationTokenSource();
        KAsyncQueue<Exception> _stopQueue = new KAsyncQueue<Exception>();
        object _lockStop = new object();
        Task _taskStop;
        Task _taskRecv;
        KNohmiSerialOptions _options;

        public void Open(KNohmiSerialOptions options)
        {
            _options = options;
            try
            {
                _serialPort = new SerialPort();
                _serialPort.BaudRate = options.Baudrate;
                _serialPort.PortName = options.PortName;
                _serialPort.Parity = options.Parity;
                _serialPort.DataBits = options.DataBit;
                _serialPort.StopBits = options.StopBit;
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                _serialPort.Dispose();
                throw new Exception(ex.Message, ex.InnerException);
            }
            _backgroundCancellationSource = new CancellationTokenSource();
            var c = _backgroundCancellationSource.Token;
            _stopQueue = new KAsyncQueue<Exception>();
            _taskRecv = Task.Run(() => ProcessRecieved(c), c);
            _taskStop = Task.Run(() => ProcessStopAllTask(c), c);
        }

        private void OnMessageRecieve(KNohmiTransport_EventArgs msg)
        {
            if (this.OnMessageRecieved != null)
            {
                this.OnMessageRecieved(this, msg);
            }
        }

        private void EventExceptionOccur(Exception ex)
        {
            if (OnExceptionOccur != null)
            {
                this.OnExceptionOccur(this, ex);
            }
        }

        private void OnConnectionClosing(Exception e)
        {
            lock (_lockStop)
            {
                if (!_backgroundCancellationSource.Token.IsCancellationRequested)
                {
                    _stopQueue.Enqueue(e);
                }
            }
        }

        private async Task ProcessRecieved(CancellationToken c)
        {
            List<byte> listByte = new List<byte>();
            try
            {
                while (!c.IsCancellationRequested)
                {
                    try
                    {
                        var frame = await ReadAsync(1, c).ConfigureAwait(false);
                        listByte.AddRange(frame);
                        if (frame[0] == '\r')
                        {
                            var frame2 = await ReadAsync(1, c).ConfigureAwait(false);
                            listByte.AddRange(frame2);
                            if (frame2[0] == '\n')// kết thúc chuỗi
                            {
                                if (listByte.Count > 2)
                                {
                                    OnMessageRecieve(new KNohmiTransport_EventArgs()
                                    {
                                        Message = System.Text.Encoding.UTF8.GetString(listByte.ToArray()),
                                        Raw = listByte.ToArray()
                                    });
                                }
                                listByte.Clear();
                            }
                            else
                            {
                                if (listByte.Count > _options.BufferSize)
                                {
                                    listByte.Clear();
                                }
                            }

                        }
                        else
                        {
                            if (listByte.Count > _options.BufferSize)
                            {
                                listByte.Clear();
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                }
            }
            catch (Exception ex)
            {
                this.EventExceptionOccur(ex);
                this.OnConnectionClosing(ex);
            }
        }

        public async Task<byte[]> ReadAsync(int count, CancellationToken c)
        {
            byte[] frameBytes = new byte[count];
            int numBytesRead = 0;
            while (numBytesRead != count && !c.IsCancellationRequested)
            {
                var result = await _serialPort.BaseStream.ReadAsync(frameBytes, numBytesRead, count - numBytesRead, c).ConfigureAwait(false);
                if (result == 0)
                    break;
                else
                {
                    numBytesRead += result;
                }
            }
            return frameBytes;
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
                //_logger.LogError("{time} {0} cache service - fail in stop task {1}", DateTime.Now, this._name, e.Message);
            }
        }

        public async Task DisconnectAsync()
        {
            this.OnConnectionClosing(new Exception("disconnect by requires"));
            await WaitForTask(_taskStop);
        }
        public async Task CloseCoreAsync(Exception ex)
        {
            lock (_lockStop)
            {
                _backgroundCancellationSource?.Cancel();
            }
            _ = Task.Run(() =>
            {
                // fix deadlock 
                try
                {
                    _serialPort.DtrEnable = false;
                    _serialPort.RtsEnable = false;
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    this._serialPort.Close();
                }
                catch (Exception)
                {

                }
            });
            await Task.Delay(1200).ConfigureAwait(false);// đợi cho cổng đóng hẳn
            _ = Task.Run(() =>
            {
                // run in task avoid deadlook
                if (Closed != null)
                    Closed.Invoke(this, new EventArgs());
            });

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
