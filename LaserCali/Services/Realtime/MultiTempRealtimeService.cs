using KUtilities.TaskExtentions;
using LaserCali.Models.Consts;
using LaserCali.Models.Temperatures.Sensor;
using LaserCali.Services.Api;
using LaserCali.Services.Config;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace LaserCali.Services.Realtime
{
    public class MultiTempRealtimeService
    {
        public delegate void ConnectHandle(object sender, bool isConnected);
        public delegate void HistoryHandle(object sender, List<MultiTempStatus_Model> list);
        public delegate void RecieveStatusMessageHandle(object sender, MultiTempStatus_Model args);
        public delegate void LogMessageHandle(object sender, string args);
        public event HistoryHandle OnHistory;
        public event ConnectHandle OnConnect;
        public event RecieveStatusMessageHandle OnRecieveStatusMessage;
        public event LogMessageHandle OnLogMessage;

        CancellationTokenSource _backgroundCancelTokenSource = new CancellationTokenSource();
        KAsyncQueue<Exception> _stopQueue = new KAsyncQueue<Exception>();

        //ILogger _logger;

        IMqttClient _mqttClient;
        object _syncStop = new object();
        Task _taskStop;
        Task _taskKeepConnection;
        int _stepKeepConnection = 0;
        bool _isRunning = false;

        public MultiTempRealtimeService()
        {

        }

        
        public void Run()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _backgroundCancelTokenSource = new CancellationTokenSource();
                _stopQueue = new KAsyncQueue<Exception>();
                var c = _backgroundCancelTokenSource.Token;
                _taskKeepConnection = Task.Run(() => ProcessKeepConnectionTask(c), c);
                _taskStop = Task.Run(() => ProcessStopTask(c), c);

                WriteLog("{0} mqtt service - running", DateTime.Now);
            }

        }

        private async Task ProcessKeepConnectionTask(CancellationToken c)
        {
            bool isExceptionRised = false;
            while (!c.IsCancellationRequested)
            {
                try
                {
                    switch (_stepKeepConnection)
                    {
                        case 0:
                            {
                                try
                                {
                                    using(var service=new HistoryMultiTempApiService())
                                    {
                                        var res = await service.GetAllAsync();
                                        if (OnHistory != null)
                                        {
                                            OnHistory.Invoke(this, res);
                                        }

                                    }
                                    await MqttClient_InitAsync(c);
                                    WriteLog("mqtt service - mqtt client connected host");
                                    Interlocked.Increment(ref _stepKeepConnection);
                                    isExceptionRised = false;
                                }
                                catch (Exception e)
                                {
                                    if (!isExceptionRised)
                                    {
                                        WriteLog("{0}  mqtt service - error connect mqtt broker.detail :{1} -", DateTime.Now, e.Message);
                                        isExceptionRised = true;
                                    }

                                    await Task.Delay(1000, c);// chờ 1 xíu và kết nối lại
                                }
                            }
                            break;
                        case 1:
                            {
                                try
                                {
                                    WriteLog("{0} begin sub topic mqtt", DateTime.Now);
                                    await Client_SubTopicDevice();
                                    WriteLog("{0} sub topic mqtt complete", DateTime.Now);
                                    Interlocked.Increment(ref _stepKeepConnection);
                                    isExceptionRised = false;
                                }
                                catch (Exception e)
                                {
                                    if (!isExceptionRised)
                                    {
                                        WriteLog("{0} mqtt service - error subscribe mqtt client .detail :{1}", DateTime.Now, e.Message);
                                        isExceptionRised = true;
                                    }
                                    await Task.Delay(1000, c);// chờ 1 xíu và kết nối lại
                                }
                            }
                            break;
                        case 2:
                            await Task.Delay(1000, c);
                            break;
                        case 3:
                            {
                                // mất kết nối -> dừng task check và khởi động lại
                                Interlocked.Exchange(ref _stepKeepConnection, 0);// quay lại step 0
                                WriteLog("{0} mqtt service - reconnecting mqtt client", DateTime.Now);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    WriteLog("{1} mqtt service - unhanle exception in process keep connection task.detail:{0}",
                                DateTime.Now, e.Message);
                }
            }
        }

        private void WriteLog(string raw, params object[] args)
        {
            var message = string.Format(raw, args);
            if (OnLogMessage != null)
            {
                OnLogMessage(this, message);
            }
        }

        private async Task ProcessStopTask(CancellationToken cancelToken)
        {
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    var stop = await _stopQueue.TryDequeueAsync(cancelToken).ConfigureAwait(false);
                    if (stop.IsSuccess)
                    {
                        await CloseCoreAsync(stop.Item);
                    }
                }
                WriteLog("{0} mqtt service - stopped ", DateTime.Now);
            }
            catch (Exception e)
            {
                WriteLog("{0} mqtt service - fail in stop task send lss :{1}", DateTime.Now, e.Message);
            }
        }
        public async Task StopAsync()
        {
            _stopQueue.Enqueue(new Exception("cancel"));
            await WaitForTask(_taskStop);
        }
        private async Task CloseCoreAsync(Exception e)
        {
            lock (_syncStop)
            {
                _backgroundCancelTokenSource?.Cancel();
            }
            try
            {
                if (_mqttClient != null)
                {
                    if (_mqttClient.IsConnected)
                        await _mqttClient.DisconnectAsync();
                }

            }
            catch (Exception ex)
            {
                WriteLog("{0} mqtt service - error when stop all task.detail:{1}", DateTime.Now, ex.Message);
            }
            await WaitForTask(_taskKeepConnection);
            _stopQueue?.Clear();
            WriteLog("{0} mqtt service - stopped", DateTime.Now);
        }
        private async Task WaitForTask(Task task)
        {
            try
            {
                if (task != null)
                    await task;
            }
            catch (Exception)
            {

            }
        }

        private async Task Client_SubTopicDevice()
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                                    .WithTopic(AppConst.HEADER_MQTT_LO_NHIET_STATUS)
                                    .WithAtMostOnceQoS()
                                    .Build());
        }
        private async Task MqttClient_InitAsync(CancellationToken c)
        {
            var cfg = LaserConfigService.ReadConfig();
            var options = new MqttClientOptionsBuilder()
                    .WithClientId(Guid.NewGuid().ToString())
                    .WithTcpServer(cfg.MqttHost, AppConst.MqttPort)
                    .WithCredentials(AppConst.MqttUserName, AppConst.MqttPassword)
                    .WithCleanSession()
                    .WithKeepAlivePeriod(new TimeSpan(0, 0, 30))
                    .Build();
            _mqttClient = new MqttFactory().CreateMqttClient();
            await _mqttClient.ConnectAsync(options, c);
            _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync;
            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync;
            if (OnConnect != null)
            {
                OnConnect(this, true);
            }
        }

        private Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            if (OnConnect != null)
            {
                OnConnect(this, false);
            }
            Interlocked.Exchange(ref _stepKeepConnection, 3);// chuyển sang bước nhả kết nối và kết nối lại
            return Task.CompletedTask;
        }


        private Task _mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs obj)
        {
            try
            {
                string topic = obj.ApplicationMessage.Topic;
                byte[] payload = obj.ApplicationMessage.PayloadSegment.Array;
                var str = System.Text.Encoding.UTF8.GetString(payload);
                var status = JsonConvert.DeserializeObject<MultiTempStatus_Model>(str);
                if (OnRecieveStatusMessage != null)
                {
                    OnRecieveStatusMessage(this, status);
                }
            }
            catch (Exception e)
            {
                WriteLog("{0} fail to decode message :{1}", DateTime.Now, e.Message);
            }
            return Task.CompletedTask;
        }
    }
}
