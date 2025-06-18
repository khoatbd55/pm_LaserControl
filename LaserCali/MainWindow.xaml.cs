using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.Mvvm;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using LaserCali.Extention;
using LaserCali.Models.Camera;
using LaserCali.Models.Config;
using LaserCali.Models.Consts;
using LaserCali.Models.Enums;
using LaserCali.Services;
using LaserCali.Services.Config;
using LaserCali.Services.Environment;
using LaserCali.Services.Excels;
using LaserCali.Services.Laser;
using LaserCali.Services.Realtime;
using LaserCali.UIs.Windowns.Common;
using LaserCali.UIs.Windowns.DutInfo;
using LaserCali.UIs.Windowns.HistoryChart;
using LaserCali.UIs.Windowns.Info;
using LaserCali.UIs.Windowns.LaserDataAdd;
using LaserCali.UIs.Windowns.Setting;
using Newtonsoft.Json;
using Notification.Wpf;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace LaserCali
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DevExpress.Xpf.Core.ThemedWindow,INotifyPropertyChanged
    {
        bool _firstClose = true;
        CancellationTokenSource _backgroundCancellTokenSource = new CancellationTokenSource();
        KCameraService _camera = new KCameraService();
        KEnvironmentSerial _environmentSerial = new KEnvironmentSerial();
        KLaserService _laser = new KLaserService();
        MultiTempRealtimeService _multiTempRealtime = new MultiTempRealtimeService();
        int _countDelayImage = 0;
        LaserConfig_Model _laserCfg = new LaserConfig_Model();
        CameraConfig_Model _camLongCfg = new CameraConfig_Model();
        CameraConfig_Model _camShortCfg = new CameraConfig_Model();
        object _syncCamShortCfg=new object();
        object _syncCamLongCfg = new object();
        object _syncLaser = new object();
        Task _taskTimer;
        ETemperatureType _temperatureType = ETemperatureType.TwoPoint;

        ISplashScreenManagerService _waitForm;
        Notification.Wpf.NotificationManager _notification = new Notification.Wpf.NotificationManager();

        public MainWindow()
        {
            this.Loaded += Window_Loaded;
            this.Closing += Window_Closing;
            DataContext = this;
            InitializeComponent();
        }

        private void WaitForm_Init()
        {
            _waitForm = splashService;
            _waitForm.ViewModel = new DXSplashScreenViewModel();
            _waitForm.ViewModel.Subtitle = "Powered by DevExpress";
            _waitForm.ViewModel.Logo = new Uri("../../Images/Logo.png", UriKind.Relative);
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_firstClose)
            {
                _firstClose = false;
                e.Cancel = true;
                _backgroundCancellTokenSource?.Cancel();
               
                await _camera.StopAsync();
                await _environmentSerial.DisconnectAsync();
                await _laser.StopAsync();
                try
                {
                    if(_taskTimer!=null && _taskTimer.Status == TaskStatus.Running)
                    {
                        await _taskTimer;
                    }
                }
                catch (Exception)
                {

                }
                await _multiTempRealtime.StopAsync();
                await Task.Delay(100);
                e.Cancel = true;
                this.Close();
            }
            else
            {
                
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void LaserConfig_Set(LaserConfig_Model cfg)
        {
            lock (_syncLaser)
            {
                _laserCfg = cfg.Clone();
            }
        }

        private LaserConfig_Model LaserConfig_Get()
        {
            lock (_syncLaser)
            {
                return _laserCfg.Clone();
            }

        }

        private void CameraLongConfig_Set(CameraConfig_Model cfg)
        {
            lock (_syncCamLongCfg)
            {
                _camLongCfg = cfg.Clone();
            }
        }

        private CameraConfig_Model CameraLongConfig_Get()
        {
            lock (_syncCamLongCfg)
            {
                return _camLongCfg.Clone();
            }
        }

        private void CamerShortConfig_Set(CameraConfig_Model cfg)
        {
            lock (_syncCamShortCfg)
            {
                _camShortCfg = cfg.Clone();// JsonConvert.DeserializeObject<CameraConfig_Model>(JsonConvert.SerializeObject(cfg));
            }
        }

        private CameraConfig_Model CameraShortConfig_Get()
        {
            lock (_syncCamShortCfg)
            {
                return _camShortCfg.Clone();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WaitForm_Init();
            var cfg = LaserConfigService.ReadConfig();
            LaserConfig_Set(cfg);
            var commonCfg = LaserConfigService.ReadCommonConfig();
            _temperatureType =(ETemperatureType) commonCfg.TemperatureType;
            AppConst.HostApi = "http://" + cfg.MqttHost;
            CamerShortConfig_Set(cfg.CameraShort);
            CameraLongConfig_Set(cfg.CameraLong);
            _multiTempRealtime.OnRecieveStatusMessage += _multiTempRealtime_OnRecieveStatusMessage;
            _multiTempRealtime.OnConnect += _multiTempRealtime_OnConnect;
            _multiTempRealtime.Run();

            laserUc.ValueResolution = cfg.LaserValueResolution;
            _camera.OnImage += _camera_OnImage;
            _camera.OnConnection += _camera_OnConnection;
            _camera.Run(CameraShortConfig_Get());

            _laser.OnResult += _laser_OnResult;
            _laser.OnLog += _laser_OnLog;
            _laser.OnConnections += _laser_OnConnections;
            _laser.Run(new Services.Laser.Options.KLaserOptions() { MsDelayGetResult = 1000 });

            _environmentSerial = new KEnvironmentSerial();
            _environmentSerial.OnExceptionAsync += _environmentSerial_OnExceptionAsync; ;
            _environmentSerial.OnConnectionAsync += _environmentSerial_OnConnectionAsync; ;
            _environmentSerial.OnRecievedMessageAsync += _environmentSerial_OnRecievedMessageAsync; ;
            _environmentSerial.Run(new Services.Environment.Models.ConfigOption.KNohmiSerialOptions()
            {
                PortName = cfg.EnviromentNameComport,
            });
            _taskTimer=Task.Run(()=>ProcessTaskTimer(_backgroundCancellTokenSource.Token),_backgroundCancellTokenSource.Token);
        }
        private string _realtimeText = "11:55:00 06-06-2025";
        public string RealtimeText
        {
            get => _realtimeText;
            set
            {
                if (_realtimeText != value)
                {
                    _realtimeText = value;
                    OnPropertyChanged(nameof(RealtimeText));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private async Task ProcessTaskTimer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _ = Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            var now = DateTime.Now;
                            RealtimeText = now.ToString("HH:mm:ss dd/MM/yyyy");
                        }
                        catch (Exception)
                        {

                        }
                    }));
                    await Task.Delay(1000, token);
                    
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, exit the loop
                    break;
                }
            }
        }

        private void _multiTempRealtime_OnConnect(object sender, bool isConnected)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (isConnected)
                    iconConnections.IsTemperatureConnected = true;
                else
                    iconConnections.IsTemperatureConnected = false;
            }));
        }

        private void _multiTempRealtime_OnRecieveStatusMessage(object sender, Models.Temperatures.Sensor.MultiTempStatus_Model args)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    // hiển thị theo cài đặt
                    double result = 0;
                    double laserValue = laserUc.LaserValue;// lấy giá trị hiện tại laser
                    if (_temperatureType == ETemperatureType.TwoPoint)// hiển thị nhiệt độ trung bình
                    {
                        laserValue = Math.Abs(laserValue);
                        var posTempDouble = laserValue / 2;
                        int posTempInt = (int)posTempDouble;
                        int posTempNext= posTempInt + 1;
                        if(posTempInt>= args.Temps.Count-1)
                        {
                            posTempInt = args.Temps.Count - 1;
                        }
                        if (posTempNext >= args.Temps.Count-1)
                        {
                            posTempNext = args.Temps.Count - 1;
                        }
                        result = ((args.Temps[posTempInt].AvgTemp + args.Temps[posTempNext].AvgTemp)/2);
                    }
                    else// lấy nhiệt độ điểm gần nhất
                    {
                        laserValue=Math.Abs(laserValue);
                        var posTempDouble = laserValue / 2;
                        int posTempInt= (int)posTempDouble;
                        var phanDu=posTempDouble- posTempInt;
                        if (phanDu > 0.5)// làm tròn lên
                        {
                            posTempInt += 1;
                        }
                        if(posTempInt>=args.Temps.Count-1)
                            posTempInt= args.Temps.Count - 1;
                        result = (double)args.Temps[posTempInt].AvgTemp;

                    }
                    tempUc.TempMaterial= result;
                }
                catch (Exception)
                {

                }
            }));
        }

        private void Service_OnExportComplete(object sender, string e)
        {
            Dispatcher.Invoke(new MethodInvoker(delegate ()
            {
                var dialog = DXMessageBox.Show("File saved successfully. Do you want to open the file?", "Notification", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog == MessageBoxResult.Yes)
                {
                    FileInfo fi = new FileInfo(@e);
                    if (fi.Exists)
                    {
                        System.Diagnostics.Process.Start(@e);
                    }
                    else
                    {
                        DXMessageBox.Show("Excel file path does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }));
        }

        private void Service_OnExceptionOccur(object sender, Exception e)
        {
            _notification.Show("Error when export excel file", e.Message, Notification.Wpf.NotificationType.Error);
        }

        private void DataAddWindow_OnSaveClick(object sender, double eut)
        {
            // thêm vào bảng giá trị
            dataTableUc.AddValue(new Models.Views.LaserValueModel()
            {
                DUT = eut,
                Laser = laserUc.LaserValue,
                Pressure = tempUc.PressureEnv,
                Tmt = tempUc.TempEnv,
                RH = tempUc.HumiEnv,
                TMaterial = tempUc.TempMaterial,
            });
        }

        private void _laser_OnConnections(object sender, Services.Laser.Events.KLaserConnections_EventArgs arg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                iconConnections.IsLaserConnected = arg.IsConnected;
            }));
        }

        private void _laser_OnLog(object sender, Services.Laser.Events.KLaserLog_EventArgs arg)
        {
            WriteLog(arg.Message);
        }

        private void _laser_OnResult(object sender, Services.Laser.Events.KLaserResult_EventArgs arg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var cfg = LaserConfig_Get();
                if(cfg.UseLaserFumula)
                {
                    double T = tempUc.TempEnv;
                    double RH=tempUc.HumiEnv;
                    double P = tempUc.PressureEnv;
                    laserUc.LaserValue = LaserExtention.LaserFormular(T,RH,P, arg.Pos);
                }
                else
                {
                    laserUc.LaserValue = arg.Pos;

                }
                laserUc.Beam = arg.Beam;

            }));
        }

        private Task _environmentSerial_OnRecievedMessageAsync(Services.Environment.Events.KNohmiEnvironment_EventArg arg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                tempUc.TempEnv = arg.Message.Temp;
                tempUc.HumiEnv = arg.Message.Humi;
                tempUc.PressureEnv = arg.Message.Pressure;
            }));
            return Task.CompletedTask;
        }

        private Task _environmentSerial_OnConnectionAsync(Services.Environment.Events.KNohmiConnection_EventArgs arg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                iconConnections.IsEnviromentConnected = arg.IsConnected;
            }));
            return Task.CompletedTask;
        }

        private Task _environmentSerial_OnExceptionAsync(Services.Environment.Events.KNohmiException_EventArg arg)
        {
            return Task.CompletedTask;
        }

        private void _camera_OnConnection(object sender, CameraConnection_EventArg e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                iconConnections.IsCameraConnected = e.IsConnected;
            }));
        }


        private void _camera_OnImage(object sender, CameraImage_EventArgs e)
        {
            try
            {
                var cfg = CameraShortConfig_Get();
                if (++_countDelayImage >= cfg.CycleDisplay)
                {
                    _countDelayImage = 0;
                    var resultShort = ImageLaserService.ImageHandleResult(e.Image, CameraShortConfig_Get(),false);
                    resultShort.Image.Freeze();
                    var resultLong = ImageLaserService.ImageHandleResult(e.Image, CameraLongConfig_Get(),false);
                    resultLong.Image.Freeze();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (cameraUc1.picCamera.Source != null)
                                cameraUc1.picCamera.Source = null;
                            if (cameraUc2.picCamera.Source != null)
                                cameraUc2.picCamera.Source = null;
                            cameraUc1.picCamera.Source = resultShort.Image;
                            cameraUc2.picCamera.Source = resultLong.Image;
                            if (resultShort.IsCalculatorSuccess)
                            {
                                cameraUc1.TxtCenterDistance.Text = resultShort.DistanceMm.ToString("F4");
                                cameraUc1.IsCenter = resultShort.IsCenter;

                            }
                            else
                            {
                                cameraUc1.IsCenter = false;
                                cameraUc1.TxtCenterDistance.Text = "---";
                            }
                            if (resultLong.IsCalculatorSuccess)
                            {
                                cameraUc2.TxtCenterDistance.Text = resultLong.DistanceMm.ToString("F4");
                                cameraUc2.IsCenter = resultLong.IsCenter;
                            }
                            else
                            {
                                cameraUc2.IsCenter = false;
                                cameraUc2.TxtCenterDistance.Text = "---";
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }));

                }
            }
            catch (Exception)
            {

            }
        }

        private void WriteLog(string str)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                logUc.AddItem(str);
            }));
        }

        private async void WindowCommonSetting_OnSaveSuccess()
        {
            // khởi động lại các service serial
            try
            {
                _waitForm.Show();
                await _environmentSerial.DisconnectAsync();
                var cfg = LaserConfigService.ReadConfig();// LaserConfig_Get();
                LaserConfig_Set(cfg);
                _environmentSerial.Run(new Services.Environment.Models.ConfigOption.KNohmiSerialOptions()
                {
                    PortName = cfg.EnviromentNameComport,
                });
                laserUc.ValueResolution = cfg.LaserValueResolution;
                _waitForm.Close();
            }
            catch (Exception ex)
            {
                _notification.Show("Error restart serial", ex.Message, Notification.Wpf.NotificationType.Error);
                _waitForm.Close();
            }

        }

        private void WindowLaserSetting_OnSaveSuccess()
        {
            var cfg = LaserConfigService.ReadConfig();
            CamerShortConfig_Set(cfg.CameraShort);
            CameraLongConfig_Set(cfg.CameraLong);
        }

        private void WindowLaserSetting_Closed(object sender, EventArgs e)
        {
            _camera.Run(CameraShortConfig_Get());
        }

        public void tempUc_OnBtnDeviceClick()
        {
            HistoryChartWindow historyChartWindow = new HistoryChartWindow();
            historyChartWindow.Owner = this;
            historyChartWindow.ShowDialog();
        }

        private void btnTempChart_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            HistoryChartWindow historyChartWindow = new HistoryChartWindow();
            historyChartWindow.Owner = this;
            historyChartWindow.ShowDialog();
        }

        private void btnLaserReset_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_laser != null)
            {
                _laser.Reset();
            }
        }

        private void btnData_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            WindowLaserDataAdd dataAddWindow = new WindowLaserDataAdd();
            dataAddWindow.Owner = this;
            dataAddWindow.OnSaveClick += DataAddWindow_OnSaveClick;
            dataAddWindow.ShowDialog();
        }

        private async void btnExcelExport_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            SaveFileDialog _saveFileDialog = new SaveFileDialog();
            var now = DateTime.Now;
            _saveFileDialog.FileName += $"Laser_{now.Year}_{now.Month}_{now.Day}_{now.Hour}_{now.Minute}_{now.Second}";

            _saveFileDialog.Filter = "Excel files (*.xlsx) | *.xlsx"; ;
            _saveFileDialog.Title = "Select where to save the excel file";
            _saveFileDialog.DefaultExt = "xlsx";
            if (_saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _waitForm.Show();
                string path = _saveFileDialog.FileName;
                ExcelExportService service = new ExcelExportService();
                service.OnExceptionOccur += Service_OnExceptionOccur;
                service.OnExportComplete += Service_OnExportComplete;
                var dut= LaserConfigService.ReadDutConfig();
                await service.Export(dataTableUc.GetDataTable(), dut, path);
                _waitForm.Close();
            }
        }

        private async void btnCameraSetting_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            WindowLaserSetting windowLaserSetting = new WindowLaserSetting();
            windowLaserSetting.Closed += WindowLaserSetting_Closed;
            windowLaserSetting.OnSaveSuccess += WindowLaserSetting_OnSaveSuccess;
            await _camera.StopAsync();
            windowLaserSetting.ShowDialog();
        }

        private void btnCommonSetting_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            WindowCommonSetting windowCommonSetting = new WindowCommonSetting();
            windowCommonSetting.Owner = this;
            windowCommonSetting.OnSaveSuccess += WindowCommonSetting_OnSaveSuccess;
            windowCommonSetting.ShowDialog();
        }

        private void btnDutInfomation_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DutInfomationWindow dutInformationWindow = new DutInfomationWindow();
            dutInformationWindow.Owner = this;
            dutInformationWindow.ShowDialog();
        }

        private void btnInfo_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            InfomationWindow informationWindow = new InfomationWindow();
            informationWindow.Owner = this;
            informationWindow.ShowDialog();
        }

        

        
    }
}
