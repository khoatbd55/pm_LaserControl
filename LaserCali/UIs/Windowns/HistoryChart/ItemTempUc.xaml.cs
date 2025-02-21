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

namespace LaserCali.UIs.Windowns.HistoryChart
{
    /// <summary>
    /// Interaction logic for ItemTempUc.xaml
    /// </summary>
    public partial class ItemTempUc : UserControl
    {
        private string _channal = "CH01";
        private string _id = "ID:001";
        private string _value = "-.---";
        private bool _isTempEnable = false;

        public bool IsTempEnable
        {
            get => _isTempEnable;
            set
            {
                if (_isTempEnable != value)
                {
                    _isTempEnable = value;
                    if (_isTempEnable)
                    {
                        container.Opacity = 1;
                    }
                    else
                    {
                        container.Opacity = 0.6;
                    }
                } 
                    
            }
        }

        public string Channal
        {
            get => _channal;
            set
            {
                if(_channal != value)
                {
                    _channal = value;
                    txtChannel.Text = _channal;
                } 
                    
            }
        }

        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    txtId.Text = _id;
                }
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    txtValue.Text= _value;
                }
            }
        }
        public ItemTempUc()
        {
            InitializeComponent();
        }
    }
}
