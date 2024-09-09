using DevExpress.Data.Extensions;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using LaserCali.Commands;
using LaserCali.Models.Views;
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

            // Tự động điều chỉnh kích thước cột
            tableView.UpdateLayout();
        }

        private void Edit(LaserValueModel item)
        {
            WindowLaserDataEdit dataEditWindow = new WindowLaserDataEdit(item.Id, item.EUT);
            dataEditWindow.OnSaveClick += DataEditWindow_OnSaveClick;
            dataEditWindow.ShowDialog();
        }

        private void DataEditWindow_OnSaveClick(object sender,int id, double eut)
        {
            var find = ListData.FindIndex(x => x.Id == id);
            if (find >= 0)
            {
                ListData[find].EUT = eut;
            }
        }


        private void Delete(LaserValueModel item)
        {
            var dia = DXMessageBox.Show("Are you sure you want to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dia == MessageBoxResult.Yes)
            {
                // Xử lý khi nhấn nút Delete
                var findIdx = ListData.FindIndex(x => x.Id == item.Id);
                if (findIdx >= 0)
                {
                    ListData.RemoveAt(findIdx);
                }
            }
            
        }

        public void AddValue(LaserValueModel item)
        {
            item.Id = ListData.Count + 1;
            ListData.Add(item);
        }


        private void dgvInfo_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {

        }
    }

    
}
