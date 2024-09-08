using DevExpress.Data.Extensions;
using DevExpress.Mvvm.DataAnnotations;
using LaserCali.Commands;
using LaserCali.UIs.Windowns;
using LaserCali.UIs.Windowns.LaserDataEdit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static LaserCali.UIs.UCs.MainDataTableUC;

namespace LaserCali.UIs.UCs
{
    /// <summary>
    /// Interaction logic for MainDataTableUC.xaml
    /// </summary>
    public partial class MainDataTableUC : UserControl
    {
        public ObservableCollection<LaserValueModel> ListData { get; set; } = new ObservableCollection<LaserValueModel>();
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public MainDataTableUC()
        {
            InitializeComponent();
            DataContext = this;
            EditCommand = new RelayCommand<LaserValueModel>(Edit);
            DeleteCommand=new RelayCommand<LaserValueModel>(Delete);

            //dgvInfo.ItemsSource = ListData;
            for (int i = 0; i < 6; i++)
            {
                ListData.Add(new LaserValueModel()
                {
                    Id=i,
                    EUT=0,
                    Laser=0,
                    Pressure=0,
                    RH=0,
                    TMaterial=0,
                    Tmt=0
                });
            }
            // Tự động điều chỉnh kích thước cột
            tableView.UpdateLayout();
        }

        public void Edit(LaserValueModel item)
        {
            WindowLaserDataEdit wd = new WindowLaserDataEdit();
            wd.ShowDialog();
        }

        public void Delete(LaserValueModel item)
        {
            // Xử lý khi nhấn nút Delete
            var findIdx = ListData.FindIndex(x => x.Id == item.Id);
            if (findIdx >= 0)
            {
                ListData.RemoveAt(findIdx);
            }
        }

        public class LaserValueModel: INotifyPropertyChanged
        {
            private int id;
            private double laser;
            private double eut;
            private double tMater;
            private double tmt;
            private double rh;
            private double pressure;
            public int Id
            { 
                get => id;
                set
                {
                    if(id != value) 
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
                    if(laser != value)
                    {
                        laser = value;
                        OnPropertyChanged(nameof(Laser));
                    }    
                }
            }
            public double EUT
            {
                get => eut;
                set
                {
                    if(eut != value)
                    {
                        eut = value;
                        OnPropertyChanged(nameof(EUT));
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
                        tmt=value;
                        OnPropertyChanged(nameof(Tmt));
                    }
                }
            }

            public double RH
            {
                get => rh;
                set
                {
                    if(rh != value)
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
                    if(pressure!=value)
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

        private void dgvInfo_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {

        }
    }

    
}
