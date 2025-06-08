using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Views
{
    public class LaserValueModel : INotifyPropertyChanged
    {
        private int id;
        private double laser;
        private double dut;
        private double tMater;
        private double tmt;
        private double rh;
        private double pressure;
        public int Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public double Laser
        {
            get => laser;
            set
            {
                if (laser != value)
                {
                    laser = value;
                    OnPropertyChanged(nameof(Laser));
                }
            }
        }
        public double DUT
        {
            get => dut;
            set
            {
                if (dut != value)
                {
                    dut = value;
                    OnPropertyChanged(nameof(DUT));
                }
            }
        }
        public double TMaterial
        {
            get => tMater;
            set
            {
                if (tMater != value)
                {
                    tMater = value;
                    OnPropertyChanged(nameof(TMaterial));
                }
            }
        }
        public double Tmt
        {
            get => tmt;
            set
            {
                if (tmt != value)
                {
                    tmt = value;
                    OnPropertyChanged(nameof(Tmt));
                }
            }
        }

        public double RH
        {
            get => rh;
            set
            {
                if (rh != value)
                {
                    rh = value;
                    OnPropertyChanged(nameof(RH));
                }
            }
        }
        public double Pressure
        {
            get => pressure;
            set
            {
                if (pressure != value)
                {
                    pressure = value;
                    OnPropertyChanged(nameof(Pressure));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
