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

namespace LaserCali.UIs.Windowns.LaserDataEdit
{
    /// <summary>
    /// Interaction logic for WindowLaserDataEdit.xaml
    /// </summary>
    public partial class WindowLaserDataEdit : Window
    {
        public delegate void SaveClickHandle(object sender,int id, double eut);
        public event SaveClickHandle OnSaveClick;
        int _id = 0;
        double _eut = 0;
        public WindowLaserDataEdit(int id,double eut)
        {
            _id = id;
            _eut = eut;
            this.Loaded += WindowLaserDataEdit_Loaded;
            InitializeComponent();
        }

        private void WindowLaserDataEdit_Loaded(object sender, RoutedEventArgs e)
        {
            nudEut.Value = (decimal)_eut;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (OnSaveClick != null)
            {
                OnSaveClick(this,this._id, _eut);
            }
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void nudEut_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            _eut = (double)nudEut.Value;
        }

        private void nudEut_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _eut = (double)nudEut.Value;
                e.Handled = true; // Ngăn không cho sự kiện tiếp tục lan truyền nếu cần
                if (OnSaveClick != null)
                {
                    OnSaveClick(this, this._id, _eut);
                }
                this.Close();
            }
        }
    }
}
