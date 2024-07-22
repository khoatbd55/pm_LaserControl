using DevExpress.Data.Extensions;
using LaserCali.Commands;
using LaserCali.UIs.Windowns;
using LaserCali.UIs.Windowns.LaserDataEdit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public MainDataTableUC()
        {
            InitializeComponent();
            DataContext = this;

            EditCommand = new RelayCommand<LaserValueModel>(OnEdit);
            DeleteCommand = new RelayCommand<LaserValueModel>(OnDelete);

            for (int i = 0; i < 6; i++)
            {
                ListData.Add(new LaserValueModel()
                {
                    ID=i,
                    EUT=0,
                    Laser=0,
                    Pressure=0,
                    RH=0,
                    TMaterial=0,
                    Tmt=0
                });
            }
        }

        private void OnEdit(LaserValueModel item)
        {
            WindowLaserDataEdit wd = new WindowLaserDataEdit();
            wd.ShowDialog();
        }

        private void OnDelete(LaserValueModel item)
        {
            // Xử lý khi nhấn nút Delete
            var findIdx = ListData.FindIndex(x => x.ID == item.ID);
            if (findIdx >= 0)
            {
                ListData.RemoveAt(findIdx);
            }
        }




        public class LaserValueModel
        {
            public int ID { get; set; }
            public double Laser { get; set; }
            public double EUT { get; set; }
            public double TMaterial { get; set; }
            public double Tmt { get; set; }
            public double RH { get; set; }
            public double Pressure { get; set; }
        }

    }

    
}
