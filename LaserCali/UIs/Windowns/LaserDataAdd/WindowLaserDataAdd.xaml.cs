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

namespace LaserCali.UIs.Windowns.LaserDataAdd
{
    /// <summary>
    /// Interaction logic for WindowLaserDataAdd.xaml
    /// </summary>
    public partial class WindowLaserDataAdd : DevExpress.Xpf.Core.ThemedWindow
    {

        public delegate void SaveClickHandle(object sender, double eut);
        public event SaveClickHandle OnSaveClick;
        public WindowLaserDataAdd()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (OnSaveClick != null)
            {
                OnSaveClick(this, (double)nudEut.Value);
            }
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void nudEut_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {

        }
    }
}
;