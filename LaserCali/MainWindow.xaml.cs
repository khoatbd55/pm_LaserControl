using LaserCali.Models.Camera;
using LaserCali.Services;
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
                //await _environmentSerial.DisconnectAsync();
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
