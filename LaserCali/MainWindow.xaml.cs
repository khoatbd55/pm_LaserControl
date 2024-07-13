using LaserCali.Models.Camera;
using LaserCali.Services;
using LaserCali.Services.Environment;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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
    public partial class MainWindow : Window
    {
        bool _firstClose = true;
        CancellationTokenSource _backgroundCancellTokenSource = new CancellationTokenSource();
        CameraService _camera = new CameraService();
        KEnvironmentSerial _environmentSerial = new KEnvironmentSerial();
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
            _camera.OnImage += _camera_OnImage;
            _camera.OnConnection += _camera_OnConnection;
            _camera.Run();
            _environmentSerial = new KEnvironmentSerial();
            _environmentSerial.OnExceptionAsync += _environmentSerial_OnExceptionAsync; ;
            _environmentSerial.OnConnectionAsync += _environmentSerial_OnConnectionAsync; ;
            _environmentSerial.OnRecievedMessageAsync += _environmentSerial_OnRecievedMessageAsync; ;
            _environmentSerial.Run(new Services.Environment.Models.ConfigOption.KNohmiSerialOptions()
            {
                PortName = "COM4"
            });
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

        

        int _countDelayImage = 0;
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
    }
}
