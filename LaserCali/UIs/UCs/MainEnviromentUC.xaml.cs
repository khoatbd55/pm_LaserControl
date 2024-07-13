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

        public MainEnviromentUC()
        {
            InitializeComponent();
        }
    }
}
