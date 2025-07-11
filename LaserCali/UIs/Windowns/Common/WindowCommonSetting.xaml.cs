using LaserCali.Models.Config;
using LaserCali.Models.Consts;
using LaserCali.Models.Sensor;
using LaserCali.Models.Views;
using LaserCali.Services.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LaserCali.UIs.Windowns.Common
{
    /// <summary>
    /// Interaction logic for WindowCommonSetting.xaml
    /// </summary>
    public partial class WindowCommonSetting : DevExpress.Xpf.Core.ThemedWindow
    {
        public event Action OnSaveSuccess;
        CancellationTokenSource _backgroundTokenSource = new CancellationTokenSource();
        bool _firstClose = true;
        Task _task;
        LaserConfig_Model _laserConfig = new LaserConfig_Model();
        ObservableCollection<SensorPos_ViewModel> _sensorPosTable = new ObservableCollection<SensorPos_ViewModel>();
        public WindowCommonSetting()
        {
            this.Loaded += WindowCommonSetting_Loaded;
            this.Closing += WindowCommonSetting_Closing;
            InitializeComponent();
        }

        private void WindowCommonSetting_Loaded(object sender, RoutedEventArgs e)
        {
            var sensorPosCfg = LaserConfigService.ReadSensorPositionConfig();
            _sensorPosTable.Clear();
            foreach(var item  in sensorPosCfg)
            {
                _sensorPosTable.Add(new SensorPos_ViewModel()
                {
                    Index = item.Index,
                    Position=item.Position,
                });
            } 
            dgvSensorPos.ItemsSource = _sensorPosTable;
            cboTemperatureType.Items.Clear();
            cboTemperatureType.Items.Add("Average");
            cboTemperatureType.Items.Add("2 Points");
            var cfg = LaserConfigService.ReadConfig();
            _laserConfig = cfg;
            txtEnvHost.Text = cfg.EnvHost;
            nudEnvPort.Value = cfg.EnvPort;
            cboTemperature.Text = cfg.TempNameComport;
            txtMqttHost.EditValue = cfg.MqttHost;
            nudLaserValueResolution.Value = cfg.LaserValueResolution;
            cboTemperatureType.SelectedIndex = (int)cfg.TemperatureType;
            chbxUseLaserFormula.IsChecked = cfg.UseLaserFumula;
            Comport_Init();
        }

        private async void WindowCommonSetting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_firstClose)
            {
                _backgroundTokenSource?.Cancel();
                _firstClose = false;
                e.Cancel = true;
                try
                {
                   
                    if (_task != null)
                    {
                        await _task;
                    }
                    await Task.Delay(100);
                }
                catch (Exception)
                {

                }
                await Task.Delay(100);
                e.Cancel = true;
                this.Close();
            }
            else
            {

            }
        }

        #region Comport 


        private void Comport_Init()
        {
            _task = Task.Run(() => ProcessCheckComport(_backgroundTokenSource.Token));
        }

        private bool CheckComportExistCboComport(string item)
        {
            for (int i = 0; i < cboTemperature.Items.Count; i++)
            {
                string nameItemCbo = cboTemperature.Items[i].ToString();
                if (item == nameItemCbo)
                    return true;
            }
            return false;
        }


        private bool CheckItemsExistArrComport(string[] item, string nameComport)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (nameComport == item[i])
                    return true;
            }
            return false;
        }



        private async Task ProcessCheckComport(CancellationToken c)
        {
            try
            {
                while (!c.IsCancellationRequested)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        string[] arr_name_port = SerialPort.GetPortNames();
                        if (arr_name_port.Length > 0)
                        {
                            for (int i = 0; i < arr_name_port.Length; i++)
                            {
                                if (!CheckComportExistCboComport(arr_name_port[i]))
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        cboTemperature.Items.Add(arr_name_port[i]);
                                    });
                                }
                            }
                            for (int i = 0; i < cboTemperature.Items.Count; i++)
                            {
                                if (!CheckItemsExistArrComport(arr_name_port, cboTemperature.Items[i].ToString()))
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        cboTemperature.Items.RemoveAt(i);
                                    });
                                }
                            }
                        }
                        else
                        {
                            cboTemperature.Items.Clear();
                        }
                    }));

                    await Task.Delay(1000, c);
                }

            }
            catch (Exception)
            {

            }
        }


        #endregion

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _laserConfig.MqttHost = txtMqttHost.EditValue.ToString();
            _laserConfig.EnvHost = txtEnvHost.Text;
            _laserConfig.EnvPort= (int)nudEnvPort.Value;
            _laserConfig.TempNameComport = cboTemperature.Text;
            _laserConfig.LaserValueResolution = (int)nudLaserValueResolution.Value;
            _laserConfig.UseLaserFumula = (bool)chbxUseLaserFormula.IsChecked;
            LaserConfigService.SaveConfig(_laserConfig);
            List<SensorPos_Model> sensorPosList = new List<SensorPos_Model>();
            foreach(var item in _sensorPosTable)
            {
                sensorPosList.Add(new SensorPos_Model(item.Index, item.Position));
            }
            LaserConfigService.SaveSensorPositionConfig(sensorPosList);
            this.Close();
            if (OnSaveSuccess != null)
            {
                OnSaveSuccess();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
