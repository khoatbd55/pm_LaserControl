using DevExpress.Mvvm;
using DevExpress.Xpf.Core.FilteringUI.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for MainLogUC.xaml
    /// </summary>
    public partial class MainLogUC : UserControl
    {
        LogListViewModel vm=new LogListViewModel();
        public MainLogUC()
        {
            InitializeComponent();
            DataContext = vm;
        }

        public void AddItem(string item)
        {
            while(vm.ListPerson.Count>200)
            {
                vm.ListPerson.RemoveAt(0);
            }    
            vm.ListPerson.Add(new LogItem()
            {
                Value = Guid.NewGuid().ToString(),
                Detail = item,
            });
            listLog.SelectedIndex = vm.ListPerson.Count - 1;
            listLog.ScrollIntoView(vm.ListPerson.Last());
        }   
    }

    public class LogListViewModel
    {
        public LogListViewModel()
        {
            ListPerson = new ObservableCollection<LogItem>();
        }
        public ObservableCollection<LogItem> ListPerson { get; set; }
    }

    public class LogItem
    {
        public LogItem()
        {

        }
        public string Value { get; set; }
        public string Detail { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}
