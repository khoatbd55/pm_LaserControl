using DevExpress.Xpf.Editors.Helpers;
using LaserCali.Models.Camera;
using LaserCali.Services;
using LaserCali.Services.Environment;
using LaserCali.Services.Laser;
using LaserCali.UIs.Windowns.Setting;
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
        int _countDelayImage = 0;

        public MainWindow()
        {
            InitializeComponent();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            laserUc.OnResetClick += LaserUc_OnResetClick;
            laserUc.OnDataClick += LaserUc_OnDataClick;
            laserUc.OnExportClick += LaserUc_OnExportClick;
            _camera.OnImage += _camera_OnImage;
            _camera.OnConnection += _camera_OnConnection;
            _camera.Run();

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
                PortName = "COM4"
            });
        }

        private void LaserUc_OnResetClick(RoutedEventArgs obj)
        {
            if (_laser != null)
            {
                _laser.Reset();
            }
        }

        private void LaserUc_OnExportClick(RoutedEventArgs obj)
        {
            
        }

        private void LaserUc_OnDataClick(RoutedEventArgs obj)
        {
            
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
                if (++_countDelayImage >= 2)
                {
                    _countDelayImage = 0;

                    if (picCamera.Source != null)
                        picCamera.Source = null;
                    picCamera.Source = ImageLaserService.ImageHandle(e.Image);
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

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            WindowLaserSetting windowLaserSetting = new WindowLaserSetting();
            windowLaserSetting.ShowDialog();
        }
    }
}
