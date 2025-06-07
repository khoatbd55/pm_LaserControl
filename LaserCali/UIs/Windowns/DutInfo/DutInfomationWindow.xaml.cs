using DevExpress.Xpf.Utils.Native;
using LaserCali.Models.Export;
using LaserCali.Services.Config;
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
using System.Windows.Shapes;

namespace LaserCali.UIs.Windowns.DutInfo
{
    /// <summary>
    /// Interaction logic for DutInfomationWindow.xaml
    /// </summary>
    public partial class DutInfomationWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        public DutInfomationWindow()
        {
            this.Loaded += DutInformationWindow_Loaded;
            InitializeComponent();
        }

        private void DutInformationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var cfg= LaserConfigService.ReadDutConfig();
            if (cfg != null)
            {
                txtName.Text = cfg.Name;
                txtModel.Text = cfg.Model;
                txtSerial.Text = cfg.Serial;
                txtRange.Text = cfg.Range;
                txtResolution.Text = cfg.Resolution;
                txtGrade.Text = cfg.Grade;
                txtManufacturer.Text = cfg.Manufacturer;
            }
            else
            {
                MessageBox.Show("Failed to load DUT information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DutInformation_Model model = new DutInformation_Model
            {
                Name = txtName.Text,
                Model = txtModel.Text,
                Serial = txtSerial.Text,
                Range = txtRange.Text,
                Resolution = txtResolution.Text,
                Grade = txtGrade.Text,
                Manufacturer = txtManufacturer.Text
            };
            LaserConfigService.SaveDutConfig(model);
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
