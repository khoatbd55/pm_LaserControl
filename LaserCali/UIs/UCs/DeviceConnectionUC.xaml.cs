using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaserCali.UIs.UCs
{
    /// <summary>
    /// Interaction logic for DeviceConnectionUC.xaml
    /// </summary>
    public partial class DeviceConnectionUC : UserControl
    {
        private bool _isCameraConnected = false;
        private bool _isLaserConnected = false;
        private bool _isTemperatureConnected = false;
        private bool _isEnviromentConnected = false;

        Color COLOR_CONNECTED = Color.FromRgb(31, 189, 0);
        Color COLOR_DISCONNECTED = Color.FromRgb(163, 163, 163);

        public bool IsEnviromentConnected
        {
            get => _isEnviromentConnected;
            set
            {
                if (value != _isEnviromentConnected)
                {
                    _isEnviromentConnected = value;
                    if (_isEnviromentConnected)
                    {
                        iconEnviroment.Foreground = new SolidColorBrush(COLOR_CONNECTED);
                    }
                    else
                    {
                        iconEnviroment.Foreground = new SolidColorBrush(COLOR_DISCONNECTED);
                    }
                }
            }
        }

        public bool IsTemperatureConnected
        {
            get => _isTemperatureConnected;
            set
            {
                if (value != _isTemperatureConnected)
                {
                    _isTemperatureConnected = value;
                    if (_isTemperatureConnected)
                    {
                        iconTemperature.Foreground = new SolidColorBrush(COLOR_CONNECTED);
                    }
                    else
                    {
                        iconTemperature.Foreground = new SolidColorBrush(COLOR_DISCONNECTED);
                    }
                }
            }
        }

        public bool IsLaserConnected
        {
            get => _isLaserConnected;
            set
            {
                if (value != _isLaserConnected)
                {
                    _isLaserConnected = value;
                    if (_isLaserConnected)
                    {
                        iconLaser.Foreground = new SolidColorBrush(COLOR_CONNECTED);
                    }
                    else
                    {
                        iconLaser.Foreground = new SolidColorBrush(COLOR_DISCONNECTED);
                    }
                }
            }
        }


        public bool IsCameraConnected
        {
            get => _isCameraConnected;
            set
            {
                if (value != _isCameraConnected)
                {
                    _isCameraConnected = value;
                    if (_isCameraConnected)
                    {
                        iconCamera.Foreground = new SolidColorBrush(COLOR_CONNECTED);
                    }
                    else
                    {
                        iconCamera.Foreground = new SolidColorBrush(COLOR_DISCONNECTED);
                    }
                }
            }
        }

        public DeviceConnectionUC()
        {
            InitializeComponent();
        }
    }
}
