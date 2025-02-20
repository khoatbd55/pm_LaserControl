using LaserCali.Models.Config;
using LaserCali.Models.Consts;
using LaserCali.Services.Config;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public WindowCommonSetting()
        {
            this.Loaded += WindowCommonSetting_Loaded;
            this.Closing += WindowCommonSetting_Closing;
            InitializeComponent();
        }

        private void WindowCommonSetting_Loaded(object sender, RoutedEventArgs e)
        {
            var cfg = LaserConfigService.ReadConfig();
            _laserConfig = cfg;
            cboDisplay.Text = cfg.DisplayNameComport;
            cboEnviroment.Text = cfg.EnviromentNameComport;
            cboTemperature.Text = cfg.TempNameComport;
            txtMqttHost.EditValue = cfg.MqttHost;
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
            for (int i = 0; i < cboDisplay.Items.Count; i++)
            {
                string nameItemCbo = cboDisplay.Items[i].ToString();
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
                                        cboDisplay.Items.Add(arr_name_port[i]);
                                        cboEnviroment.Items.Add(arr_name_port[i]);
                                        cboTemperature.Items.Add(arr_name_port[i]);
                                    });
                                }
                            }
                            for (int i = 0; i < cboDisplay.Items.Count; i++)
                            {
                                if (!CheckItemsExistArrComport(arr_name_port, cboDisplay.Items[i].ToString()))
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        cboDisplay.Items.RemoveAt(i);
                                        cboEnviroment.Items.RemoveAt(i);
                                        cboTemperature.Items.RemoveAt(i);
                                    });
                                }
                            }
                        }
                        else
                        {
                            cboDisplay.Items.Clear();
                            cboEnviroment.Items.Clear();
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
            _laserConfig.DisplayNameComport = cboDisplay.Text;
            _laserConfig.EnviromentNameComport=cboEnviroment.Text;
            _laserConfig.TempNameComport = cboTemperature.Text;
            
            LaserConfigService.SaveConfig(_laserConfig);
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
