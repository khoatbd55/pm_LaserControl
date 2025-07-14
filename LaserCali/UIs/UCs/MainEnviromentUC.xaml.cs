using DevExpress.DirectX.Common.Direct2D;
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
    /// Interaction logic for MainEnviromentUC.xaml
    /// </summary>
    public partial class MainEnviromentUC : UserControl
    {
        public event Action OnBtnDeviceClick;

        double _tempEnv = 0;
        double _humiEnv = 0;
        double _pressureEnv = 0;
        double _tMaterial = 0;
        double _deltaT = 0;

        public double DeltaT
        {
            get => _deltaT;
            set
            {
                if (_deltaT != value)
                {
                    _deltaT = value;
                }
            }
        }

        public double TempMaterial
        {
            get => _tMaterial;
            set
            {
                if (_tMaterial != value)
                {
                    _tMaterial = value;
                    txtTemMaterial.Text = _tMaterial.ToString("F3");
                }
            }
        }

        public double TempEnv
        {
            get => _tempEnv;
            set
            {
                if (_tempEnv != value)
                {
                    _tempEnv = value;
                    txtTempEnv.Text = _tempEnv.ToString("F2");
                }
            }
        }

        public double HumiEnv
        {
            get => _humiEnv;
            set
            {
                if (_humiEnv != value)
                {
                    _humiEnv = value;
                    txtHumiEnv.Text = _humiEnv.ToString("F2");
                }
            }
        }

        public double PressureEnv
        {
            get => _pressureEnv;
            set
            {
                if (_pressureEnv != value)
                {
                    _pressureEnv = value;
                    txtPressureEnv.Text = _pressureEnv.ToString("F3");
                }
            }
        }

        public MainEnviromentUC()
        {
            InitializeComponent();
            //cboTemperatureType.Items.Clear();
            //for (int i = 0; i < 16; i++)
            //{
            //    cboTemperatureType.Items.Add($"CH{i + 1}");
            //}
            //cboTemperatureType.Items.Add("Average");
        }

        private void txtTemMaterial_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OnBtnDeviceClick != null)
            {
                OnBtnDeviceClick();
            }
        }
    }
}
