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
    /// Interaction logic for CameraUC.xaml
    /// </summary>
    public partial class CameraUC : UserControl
    {
        public Image PicCamera
        {
            get => picCamera;
            set => picCamera = value;
        }
        public TextBlock TxtCenterDistance
        {
            get => txtCenterDistance;
            set => txtCenterDistance = value;
        }

        bool _isCenter = false;
        System.Windows.Media.Color COLOR_CONNECTED = System.Windows.Media.Color.FromRgb(31, 189, 0);
        System.Windows.Media.Color COLOR_DISCONNECTED = System.Windows.Media.Color.FromRgb(163, 163, 163);
        public bool IsCenter
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
                        borderCamera.Background = color;
                        txtCenterDistance.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                        txtLabelCenter.Text = "Centering";
                        txtLabelCenter.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)); ;
                    }
                    else
                    {

                        iconCenter.Foreground = new SolidColorBrush(COLOR_DISCONNECTED);
                        borderCamera.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                        txtCenterDistance.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30));
                        txtLabelCenter.Text = "";
                    }

                }
            }
        }

        public CameraUC()
        {
            InitializeComponent();
        }
    }
}
