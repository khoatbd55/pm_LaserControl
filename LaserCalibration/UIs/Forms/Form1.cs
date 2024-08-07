﻿using DevExpress.XtraWaitForm;
using LaserCalibration.Services;
using LaserCalibration.Services.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using LaserCalibration.Services.Environment;
using LaserCalibration.UIs.UCs;

namespace LaserCalibration
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        bool _firstClose = true;
        CancellationTokenSource _backgroundCancellTokenSource = new CancellationTokenSource();
        CameraService _camera = new CameraService();
        WaitForm_Service _waitForm;
        KEnvironmentSerial _environmentSerial = new KEnvironmentSerial();
        LaserSettingUc _settingUc = new LaserSettingUc();
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_firstClose)
            {
                _firstClose = false;
                e.Cancel = true;
                _backgroundCancellTokenSource?.Cancel();
                _waitForm.ShowProgressPanel();

                await _camera.StopAsync();
                await _environmentSerial.DisconnectAsync();
                await Task.Delay(100);
                _waitForm.CloseProgressPanel();
                e.Cancel = true;
                this.Close();
            }
            else
            {
                try
                {
                    Application.Exit();
                }
                catch (Exception)
                {

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _waitForm = new WaitForm_Service(this);
            xtraTabSetting.Controls.Clear();
            _settingUc.Dock= DockStyle.Fill;
            xtraTabSetting.Controls.Add(_settingUc);
            _camera.OnImage += _camera_OnImage;
            _camera.OnConnection += _camera_OnConnection;
            _camera.Run();
            _environmentSerial = new KEnvironmentSerial();
            _environmentSerial.OnExceptionAsync += _environmentSerial_OnExceptionAsync;
            _environmentSerial.OnConnectionAsync += _environmentSerial_OnConnectionAsync;
            _environmentSerial.OnRecievedMessageAsync += _environmentSerial_OnRecievedMessageAsync;
            _environmentSerial.Run(new Services.Environment.Models.ConfigOption.KNohmiSerialOptions()
            {
                PortName="COM4"
            });
        }

        private Task _environmentSerial_OnConnectionAsync(Services.Environment.Events.KNohmiConnection_EventArgs arg)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                try
                {
                    if (arg.IsConnected)
                    {
                        picEnviromentalStatus.Image= Properties.Resources.green_circle_32px;
                    }
                    else
                    {
                        picEnviromentalStatus.Image = Properties.Resources.black_circle_32px;
                    }
                }
                catch (Exception)
                {

                }
            }));
            return Task.CompletedTask;
        }

        private Task _environmentSerial_OnRecievedMessageAsync(Services.Environment.Events.KNohmiEnvironment_EventArg arg)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                try
                {
                    labelTemperature.Text = arg.Message.Temp.ToString();
                    labelPressure.Text = arg.Message.Pressure.ToString();
                    labelHumi.Text = arg.Message.Humi.ToString();
                }
                catch (Exception)
                {

                }
            }));
            return Task.CompletedTask;
        }

        private Task _environmentSerial_OnExceptionAsync(Services.Environment.Events.KNohmiException_EventArg arg)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                
            }));
            return Task.CompletedTask;
        }

        private void _camera_OnConnection(object sender, Models.Camera.CameraConnection_EventArg e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (e.IsConnected)
                {
                    picCameraStatus.Image = Properties.Resources.green_circle_32px;
                }
                else
                {
                    picCameraStatus.Image = Properties.Resources.black_circle_32px;
                }
            }));

        }

        int _countDelayImage = 0;
        private void _camera_OnImage(object sender, Models.Camera.CameraImage_EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if(++_countDelayImage>=2)
                {
                    _countDelayImage = 0;
                    //if (picCamera.Image != null)
                    //    picCamera.Image = null;
                    //picCamera.Image = e.Image;

                    var image = BitmapConverter.ToMat(e.Image);
                    // Convert the image to grayscale
                    Mat grayImage = new Mat();
                    Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

                    Mat blurred = new Mat();
                    Cv2.GaussianBlur(grayImage, blurred, new OpenCvSharp.Size(5, 5), 0);

                    // Threshold the image
                    Mat binaryImage = new Mat();
                    Cv2.Threshold(grayImage, binaryImage, 80, 255, ThresholdTypes.Binary);

                    //if (picDebug.Image != null)
                    //    picDebug.Image = null;
                    //picDebug.Image = BitmapConverter.ToBitmap(blurred);

                    //return;
                    // Find contours
                    OpenCvSharp.Point[][] contours;
                    HierarchyIndex[] hierarchy;
                    Cv2.FindContours(binaryImage, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                    List<Rect> _listPoint = new List<Rect>();
                        
                    //int idx = 0;
                    foreach (var contour in contours)
                    {
                        // Get the bounding rectangle of the contour
                        Rect boundingRect = Cv2.BoundingRect(contour);
                        if (boundingRect.Width > 10 && boundingRect.Height > 20)
                        {
                            // Calculate the center point of the bounding rectangle
                            //Point center = new Point(boundingRect.X + boundingRect.Width / 2, boundingRect.Y + boundingRect.Height / 2);
                            _listPoint.Add(boundingRect);
                        }

                    }
                    for (int i = 0; i < _listPoint.Count - 1; i++)
                    {
                        var current = _listPoint[i];
                        var next = _listPoint[i + 1];
                        var middleX = current.X - (current.X - next.X - next.Width) / 2;
                        Cv2.Line(image, new OpenCvSharp.Point(middleX, 0), new OpenCvSharp.Point(middleX, current.Y + image.Height), Scalar.Green, 10);
                    }

                    // Tính toán tâm của ảnh
                    OpenCvSharp.Point centerImage = new OpenCvSharp.Point(image.Width / 2, image.Height / 2);

                    // Vẽ một điểm tại tâm ảnh (có thể sử dụng hình tròn hoặc điểm)
                    //Cv2.Circle(image, centerImage, 5, Scalar.Red, -1); // Vẽ một điểm màu đỏ tại tâm ảnh
                    Cv2.Line(image, new OpenCvSharp.Point(centerImage.X, centerImage.Y - image.Height / 2),
                            new OpenCvSharp.Point(centerImage.X, centerImage.Y + image.Height / 2), Scalar.Orange, 10, LineTypes.AntiAlias);

                    if (picCamera.Image != null)
                        picCamera.Image = null;
                    picCamera.Image = BitmapConverter.ToBitmap(image);
                }    
                
            }));
        }

        private void xtraTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            //MessageBox.Show(xtraTabControl.SelectedTabPageIndex.ToString());
        }
    }
}
