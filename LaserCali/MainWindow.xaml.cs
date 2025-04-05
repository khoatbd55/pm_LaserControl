using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using LaserCali.Extention;
using LaserCali.Models.Camera;
using LaserCali.Models.Config;
using LaserCali.Models.Consts;
using LaserCali.Services;
using LaserCali.Services.Config;
using LaserCali.Services.Environment;
using LaserCali.Services.Excels;
using LaserCali.Services.Laser;
using LaserCali.Services.Realtime;
using LaserCali.UIs.Windowns.Common;
using LaserCali.UIs.Windowns.HistoryChart;
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
    public partial class MainWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        bool _firstClose = true;
        CancellationTokenSource _backgroundCancellTokenSource = new CancellationTokenSource();
        KCameraService _camera = new KCameraService();
        KEnvironmentSerial _environmentSerial = new KEnvironmentSerial();
        KLaserService _laser = new KLaserService();
        MultiTempRealtimeService _multiTempRealtime = new MultiTempRealtimeService();
        int _countDelayImage = 0;
        LaserConfig_Model _laserCfg = new LaserConfig_Model();
        CameraConfig_Model _camCfg = new CameraConfig_Model();
        object _syncCamCfg=new object();
        object _syncLaser = new object();

        int _temperatureType = 0;

        bool _isCenter = false;
        ISplashScreenManagerService _waitForm;
        Notification.Wpf.NotificationManager _notification = new Notification.Wpf.NotificationManager();

        System.Windows.Media.Color COLOR_CONNECTED = System.Windows.Media.Color.FromRgb(31, 189, 0);
        System.Windows.Media.Color COLOR_DISCONNECTED = System.Windows.Media.Color.FromRgb(163, 163, 163);
        bool IsCenter
        {
            get => _isCenter;
            set
            {
                if (_isCenter != value)
                {
                    _isCenter = value;
                    if (_isCenter)
                    {
                        var color = new SolidColorBrush(COLOR_CONNECTED);
                        iconCenter.Foreground = color;
                        borderCamera.Background= color;
                        txtCenterDistance.Foreground= new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                        txtLabelCenter.Text = "Centering";
                        txtLabelCenter.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)); ;
                    }    
                    else
                    {
                        
                        iconCenter.Foreground = new SolidColorBrush(COLOR_DISCONNECTED);
                        borderCamera.Background=new SolidColorBrush(System.Windows.Media.Color.FromRgb(255,255,255));
                        txtCenterDistance.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30));
                        txtLabelCenter.Text = "";
                    }    
                        
                }
            }
        }

        public MainWindow()
        {
            this.Loaded += Window_Loaded;
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
                await _multiTempRealtime.StopAsync();
                await _camera.StopAsync();
                await _environmentSerial.DisconnectAsync();
                await _laser.StopAsync();
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
            lock(_syncLaser)
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

        private void CamerConfig_Set(CameraConfig_Model cfg)
        {
            lock (_syncCamCfg)
            {
                _camCfg = cfg.Clone();// JsonConvert.DeserializeObject<CameraConfig_Model>(JsonConvert.SerializeObject(cfg));
            }
        }

        private CameraConfig_Model CamerConfig_Get()
        {
            lock (_syncCamCfg)
            {
                return _camCfg.Clone();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WaitForm_Init();
            var cfg = LaserConfigService.ReadConfig();
            var commonCfg = LaserConfigService.ReadCommonConfig();
            tempUc.TemperatureType_Set(commonCfg.TemperatureType);
            _temperatureType = commonCfg.TemperatureType;
            AppConst.HostApi = "http://" + cfg.MqttHost;
            CamerConfig_Set(cfg.Camera);
            _multiTempRealtime.OnRecieveStatusMessage += _multiTempRealtime_OnRecieveStatusMessage;
            _multiTempRealtime.OnConnect += _multiTempRealtime_OnConnect;
            _multiTempRealtime.Run();

            laserUc.OnResetClick += LaserUc_OnResetClick;
            laserUc.OnDataClick += LaserUc_OnDataClick;
            laserUc.OnExportClick += LaserUc_OnExportClick;
            _camera.OnImage += _camera_OnImage;
            _camera.OnConnection += _camera_OnConnection;
            _camera.Run(CamerConfig_Get());

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
                    string result = "---";
                    if (_temperatureType == AppConst.TemperatureTypeAvr)// hiển thị nhiệt độ trung bình
                    {
                        double sum = 0;
                        int total = 0;
                        foreach (var item in args.Temps)
                        {
                            if (item.IsEnable && item.IsSensorConnected)
                            {
                                total += 1;
                                sum += item.AvgTemp;
                            }
                        }
                        if (total > 0)
                            result = (sum / total).ToString("F3");
                    }
                    else
                    {
                        if (args.Temps[_temperatureType].IsEnable && args.Temps[_temperatureType].IsSensorConnected)
                        {
                            result = args.Temps[_temperatureType].AvgTemp.ToString("F3");
                        } 
                            
                    }
                    tempUc.TemperatureMaterial_Set(result);
                }
                catch (Exception)
                {

                }
            }));
        }

        private void LaserUc_OnResetClick(RoutedEventArgs obj)
        {
            if (_laser != null)
            {
                _laser.Reset();
            }
        }

        private async void LaserUc_OnExportClick(RoutedEventArgs obj)
        {
            SaveFileDialog _saveFileDialog = new SaveFileDialog();
            var now=DateTime.Now;
            _saveFileDialog.FileName += $"Laser_{now.Year}_{now.Month}_{now.Day}_{now.Hour}_{now.Minute}_{now.Second}";

            _saveFileDialog.Filter = "Excel files (*.xlsx) | *.xlsx"; ;
            _saveFileDialog.Title = "Select where to save the excel file";
            _saveFileDialog.DefaultExt = "xlsx";
            if (_saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _waitForm.Show();
                string duongDan = _saveFileDialog.FileName;
                ExcelExportService service = new ExcelExportService();
                service.OnExceptionOccur += Service_OnExceptionOccur;
                service.OnExportComplete += Service_OnExportComplete;
                await service.Export(dataTableUc.GetDataTable(), duongDan);
                _waitForm.Close();
            }
        }

        private void Service_OnExportComplete(object sender, string e)
        {
            Dispatcher.Invoke(new MethodInvoker(delegate ()
            {
                var dialog = DXMessageBox.Show("File saved successfully. Do you want to open the file?", "Notification", MessageBoxButton.YesNo,MessageBoxImage.Question);
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

        private void LaserUc_OnDataClick(RoutedEventArgs obj)
        {
            WindowLaserDataAdd dataAddWindow = new WindowLaserDataAdd();
            dataAddWindow.OnSaveClick += DataAddWindow_OnSaveClick;
            dataAddWindow.ShowDialog();
        }

        private void DataAddWindow_OnSaveClick(object sender, double eut)
        {
            // thêm vào bảng giá trị
            dataTableUc.AddValue(new Models.Views.LaserValueModel()
            {
                EUT = eut,
                Laser=laserUc.LaserValue,
                Pressure= tempUc.PressureEnv,
                Tmt=tempUc.TempEnv,
                RH=tempUc.HumiEnv,
                TMaterial=0,
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
                laserUc.LaserValue = arg.Pos;
                laserUc.Beam=arg.Beam;

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
            
            Dispatcher.Invoke(new Action(() =>
            {
                var cfg = CamerConfig_Get();
                if (++_countDelayImage >= cfg.CycleDisplay)
                {
                    _countDelayImage = 0;

                    if (picCamera.Source != null)
                        picCamera.Source = null;
                    var result = ImageLaserService.ImageHandleResult(e.Image, CamerConfig_Get());
                    picCamera.Source = result.Image;
                    if (result.IsCalculatorSuccess)
                    {
                        txtCenterDistance.Text = result.DistanceMm.ToString("F4");
                        IsCenter = result.IsCenter;
                        
                    }
                    else
                    {
                        IsCenter = false;
                        txtCenterDistance.Text = "---";
                    }
                }
            }));
        }

        private void WriteLog(string str)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                logUc.AddItem(str);
            }));
        }

        private async void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            WindowLaserSetting windowLaserSetting = new WindowLaserSetting();
            windowLaserSetting.Closed += WindowLaserSetting_Closed;
            windowLaserSetting.OnSaveSuccess += WindowLaserSetting_OnSaveSuccess;
            await _camera.StopAsync();
            windowLaserSetting.ShowDialog();
        }

        private void btnCommonSeting_Click(object sender, RoutedEventArgs e)
        {
            WindowCommonSetting windowCommonSetting = new WindowCommonSetting();
            windowCommonSetting.OnSaveSuccess += WindowCommonSetting_OnSaveSuccess;
            windowCommonSetting.ShowDialog();
        }


        private async void WindowCommonSetting_OnSaveSuccess()
        {
            // khởi động lại các service serial
            try
            {
                _waitForm.Show();
                await _environmentSerial.DisconnectAsync();
                var cfg = LaserConfig_Get();
                _environmentSerial.Run(new Services.Environment.Models.ConfigOption.KNohmiSerialOptions()
                {
                    PortName = cfg.EnviromentNameComport,
                });
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
            CamerConfig_Set(cfg.Camera);
        }

        private void WindowLaserSetting_Closed(object sender, EventArgs e)
        {
            _camera.Run(CamerConfig_Get());
        }

        public void tempUc_OnBtnDeviceClick()
        {
            HistoryChartWindow historyChartWindow = new HistoryChartWindow();
            historyChartWindow.Owner = this;
            historyChartWindow.ShowDialog();
        }

        private void tempUc_OnTemperatureTypeChange(object sender, int type)
        {
            _temperatureType = type;
            var commonCfg = LaserConfigService.ReadCommonConfig();
            commonCfg.TemperatureType = type;
            LaserConfigService.CommonConfigSave(commonCfg);

        }
    }
}
