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
    /// Interaction logic for MainLaserUC.xaml
    /// </summary>
    public partial class MainLaserUC : UserControl
    {

        public event Action<RoutedEventArgs> OnResetClick;
        public event Action<RoutedEventArgs> OnDataClick;
        public event Action<RoutedEventArgs> OnExportClick;

        public double _laserValue = 0;
        public double _beam = 0;
        public double LaserValue
        {
            get => _laserValue;
            set
            {
                if(value != _laserValue) 
                {
                    _laserValue = value;
                    txtLaserValue.Text= _laserValue.ToString("F6");
                }
            }
        }

        public double Beam
        {
            get => _beam;
            set
            {
                if (value != _beam)
                {
                    _beam = value;
                    prbBeam.Value= _beam;
                }
            }
        }
        public MainLaserUC()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (OnResetClick != null)
            {
                OnResetClick(e);
            }
        }

        private void btnData_Click(object sender, RoutedEventArgs e)
        {
            if(OnDataClick!=null)
            {
                OnDataClick(e);
            }    
        }

        private void txtExport_Click(object sender, RoutedEventArgs e)
        {
            if (OnExportClick != null)
            {
                OnExportClick(e);
            }
        }
    }
}
