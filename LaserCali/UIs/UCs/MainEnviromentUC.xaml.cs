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

        public double TempEnv
        {
            get => _tempEnv;
            set
            {
                if (_tempEnv != value)
                {
                    _tempEnv = value;
                    txtTempEnv.Text = _tempEnv.ToString("F1");
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
                    txtHumiEnv.Text = _humiEnv.ToString("F1");
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
                    txtPressureEnv.Text = _pressureEnv.ToString("F1");
                }
            }
        }

        public void TemperatureMaterial_Set(string value)
        {
            if (txtTemMaterial.Text != value)
            {
                txtTemMaterial.Text = value;
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

        public void TemperatureType_Set(int type)
        {
            //if (type < cboTemperatureType.Items.Count)
            //{
            //    cboTemperatureType.SelectedIndex = type;
            //}
        }

        private void btnDevice_Click(object sender, RoutedEventArgs e)
        {
            if (OnBtnDeviceClick != null)
            {
                OnBtnDeviceClick();
            }
        }

    }
}
