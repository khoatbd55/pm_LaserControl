using DevExpress.Mvvm;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid.GroupRowLayout;
using LaserCali.Models.Temperatures.Sensor;
using LaserCali.Services.Api;
using LaserCali.Services.Environment;
using LaserCali.Services.Realtime;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace LaserCali.UIs.Windowns.HistoryChart
{
    /// <summary>
    /// Interaction logic for HistoryChartWindow.xaml
    /// </summary>
    public partial class HistoryChartWindow : Window
    {
        List<ItemTempUc> _list = new List<ItemTempUc>();
        bool _firstClose = true;
        CancellationTokenSource _backgroundCancellTokenSource = new CancellationTokenSource();
        MultiTempRealtimeService _multiTempRealtime = new MultiTempRealtimeService();
        Stopwatch stopWatch = new Stopwatch();
        ISplashScreenManagerService _waitForm;
        INotificationManager _notification = new NotificationManager();
        private void WaitForm_Init()
        {
            _waitForm = splashService;
            _waitForm.ViewModel = new DXSplashScreenViewModel();
            _waitForm.ViewModel.Subtitle = "Powered by DevExpress";
            _waitForm.ViewModel.Logo = new Uri("../../Images/Logo.png", UriKind.Relative);
        }

        public HistoryChartWindow()
        {
            InitializeComponent();
            
            this.Loaded += HistoryChartWindow_Loaded;
            this.Closing += HistoryChartWindow_Closing;
        }

        

        private async void HistoryChartWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_firstClose)
            {
                _firstClose = false;
                e.Cancel = true;
                _backgroundCancellTokenSource?.Cancel();
                await _multiTempRealtime.StopAsync();
                await Task.Delay(100);

                e.Cancel = true;
                stopWatch.Stop();
                this.Close();
            }
            else
            {

            }
        }

        private async void HistoryChartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WaitForm_Init();
            _list.Add(tempUc1);
            _list.Add(tempUc2);
            _list.Add(tempUc3);
            _list.Add(tempUc4);
            _list.Add(tempUc5);
            _list.Add(tempUc6);
            _list.Add(tempUc7);
            _list.Add(tempUc8);
            _list.Add(tempUc9);
            _list.Add(tempUc10);
            _list.Add(tempUc11);
            _list.Add(tempUc12);
            _list.Add(tempUc13);
            _list.Add(tempUc14);
            _list.Add(tempUc15);
            _list.Add(tempUc16);

            for(int i=0; i<_list.Count; i++)
            {
                if(i<10)
                {
                    _list[i].Channal = "CH0" + (i+1).ToString();
                    _list[i].Id = "ID:00" + (i+1).ToString();
                }
                else
                {
                    _list[i].Channal = "CH" + (i + 1).ToString();
                    _list[i].Id = "ID:0" + (i + 1).ToString();
                }
                _list[i].Value = "-.---";
            }

            Chart_Init();
            try
            {
                _waitForm.Show();
                using (var service = new HistoryMultiTempApiService())
                {
                    var history = await service.GetAllAsync();
                    ChartTemp_AddRange(history);
                }
                _waitForm.Close();
            }
            catch (Exception ex)
            {
                _waitForm.Close();
                _notification.Show("Error when load data", ex.Message, NotificationType.Error);
            }
            
            _multiTempRealtime.OnRecieveStatusMessage += _multiTempRealtime_OnRecieveStatusMessage; ;
            _multiTempRealtime.OnConnect += _multiTempRealtime_OnConnect; ;
            _multiTempRealtime.Run();
            stopWatch.Start();
        }

        private void Chart_Init()
        {
            chart.Diagram.Series.Clear();

            for (int i = 0; i < 16; i++)
            {
                chart.Diagram.Series.Add(new LineSeries2D()
                {
                    DisplayName = "CH:" + (i + 1),
                    Visible = true,
                    LastPoint = new SidePoint()
                    {
                        LabelDisplayMode = SidePointDisplayMode.SeriesPoint,
                        Label = new DevExpress.Xpf.Charts.SeriesLabel()
                        {
                            TextPattern = "CH:" + (i + 1).ToString() + "-{V}" + "°C",
                        },
                    },
                    CrosshairEnabled = true,
                    CrosshairLabelPattern = "CH:" + (i + 1).ToString() + "-{V}" + "°C"
                });

            }
        }

        private void diagram_Zoom(object sender, XYDiagram2DZoomEventArgs e)
        {
            try
            {
                XYDiagram2D diagram = (XYDiagram2D)sender;

                if (e.AxisX.ActualWholeRange.ActualMaxValueInternal == e.NewXRange.Max &&
                    e.AxisX.ActualWholeRange.ActualMinValueInternal == e.NewXRange.Min)
                    diagram.DefaultPane.AxisXScrollBarOptions.Visible = false;
                else
                    diagram.DefaultPane.AxisXScrollBarOptions.Visible = true;
            }
            catch (Exception)
            {

            }

        }

        public void ChartTemp_AddRange(List<MultiTempStatus_Model> log)
        {
            List< List<SeriesPoint>> listSerial = new List<List<SeriesPoint>>();
            for(int i=0;i<16;i++)
            {
                listSerial.Add(new List<SeriesPoint>());
            }    
            if(log.Count==0)
            {
                return;
            }    
            var lastCreated = log[0].CreatedAt;
            foreach (var item in log)
            {
                {
                    if (DateTime.Compare( item.CreatedAt.Subtract(new TimeSpan(0,1,0)), lastCreated)>0)
                    {
                        var lostTime = item.CreatedAt.Subtract(new TimeSpan(0, 0, 60));
                        for(int i=0;i< listSerial.Count;i++)
                        {
                            listSerial[i].Add(new SeriesPoint()
                            {
                                Argument = lostTime.ToString(),
                                Value = double.NaN,
                            });
                        }    
                        
                        
                    }
                }
                for (int i = 0; i < listSerial.Count; i++)
                {
                    listSerial[i].Add(new SeriesPoint()
                    {
                        Argument = item.CreatedAt.ToString(),
                        Value = item.Temps[i].Temp,
                    });
                }    
                
                
            }
            for(int i=0;i<listSerial.Count;i++)
            {
                chart.Diagram.Series[i].Points.AddRange(listSerial[i]);
            }    
            
        }

        public void ChartTemp_Add(MultiTempStatus_Model log)
        {
            if (log == null)
                return;
            for(int i=0;i<log.Temps.Count;i++) 
            {
                chart.Diagram.Series[i].Points.Add(new SeriesPoint()
                {
                    Argument = log.CreatedAt.ToString(),
                    Value = log.Temps[i].Temp
                });
                
            }
        }

        private void chart_BoundDataChanged(object sender, RoutedEventArgs e)
        {
            // Adjust the visual range.
            AxisX2D axisX = ((XYDiagram2D)chart.Diagram).ActualAxisX;
            DateTime maxRangeValue = (DateTime)axisX.ActualWholeRange.ActualMaxValue;
            axisX.ActualVisualRange.SetMinMaxValues(maxRangeValue.AddSeconds(-60), maxRangeValue);
        }

        private void _multiTempRealtime_OnConnect(object sender, bool isConnected)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                foreach(var item in _list)
                {
                    item.IsTempEnable = isConnected;
                } 
            }));
        }

        

        private void _multiTempRealtime_OnRecieveStatusMessage(object sender, Models.Temperatures.Sensor.MultiTempStatus_Model args)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (args.Temps.Count == _list.Count)
                    {
                        var temps = args.Temps;
                        for(int i = 0; i < _list.Count; i++)
                        {
                            var itemUc=_list[i];
                            if (temps[i].IsEnable)
                            {
                                itemUc.IsTempEnable = true;
                            }
                            else
                            {
                                itemUc.IsTempEnable = false;
                            }
                            if (temps[i].IsSensorConnected)
                            {
                                itemUc.Value = temps[i].AvgTemp.ToString("F3");
                            }
                            else
                            {
                                itemUc.Value = "-.---";
                            }
                        } 
                        if(stopWatch.ElapsedMilliseconds>=5000)
                        {
                            stopWatch.Restart();
                            ChartTemp_Add(args);


                        }    
                            
                    }
                }
                catch (Exception)
                {

                }
                
            }));
            
        }
    }
}
