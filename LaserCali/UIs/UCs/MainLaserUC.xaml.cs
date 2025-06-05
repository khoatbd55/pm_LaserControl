using LaserCali.Models.Views;
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

        public double _laserValue = 0;
        public double _beam = 0;

        public int _valueResolution = 3;
        public int ValueResolution
        {
            get => _valueResolution;
            set
            {
                if (value != _valueResolution)
                {
                    _valueResolution = value;
                }
            }
        }

        public double LaserValue
        {
            get => _laserValue;
            set
            {
                if(value != _laserValue) 
                {
                    _laserValue = value;
                    txtLaserValue.Text= _laserValue.ToString($"F{_valueResolution}");
                    
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
                    if(_beam>50)
                    {
                        prbBeam.Foreground = new SolidColorBrush(Colors.Green);
                        txtPrbBeam.Foreground = new SolidColorBrush(Colors.White);
                    }    
                    else if(_beam>0)
                    {
                        prbBeam.Foreground = new SolidColorBrush(Colors.Yellow);
                        txtPrbBeam.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        txtPrbBeam.Foreground= new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }
        public MainLaserUC()
        {
            InitializeComponent();
        }

    }
}
