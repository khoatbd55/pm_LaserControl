using DevExpress.DirectX.Common.Direct2D;
using DevExpress.Xpf.Grid.GroupRowLayout;
using LaserCali.Services;
using LaserCali.Services.Config;
using LaserCali.Services.Environment;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using LaserCali.Extention;
using DevExpress.Xpf.Core;
using LaserCali.Models.Config;
using LaserCali.Models.Camera;

namespace LaserCali.UIs.Windowns.Setting
{
    /// <summary>
    /// Interaction logic for WindowLaserSetting.xaml
    /// </summary>
    public partial class WindowLaserSetting : DevExpress.Xpf.Core.ThemedWindow
    {
        public event Action OnSaveSuccess;

        CancellationTokenSource _backgroundTokenSource = new CancellationTokenSource();
        bool _firstClose = true;
        KCameraService _camera = new KCameraService();
        long _roiTop = 0,_roiBottom=100,_threshold=0,_rotation=0,_rectNoise=10,_detecionDistance=1,_frame=1,_cycleDisplay=1;
        bool _isCenter = false;
        double _lenWidth = 0;
        object _syncLenWidth = new object();
        System.Windows.Media.Color COLOR_CONNECTED = System.Windows.Media.Color.FromRgb(31, 189, 0);
        System.Windows.Media.Color COLOR_DISCONNECTED = System.Windows.Media.Color.FromRgb(163, 163, 163);
        LaserConfig_Model _laserConfig = new LaserConfig_Model();

        int Frame
        {
            get => (int)Interlocked.Read(ref _frame);
            set
            {
                Interlocked.Exchange(ref _frame, value);
            }
        }

        int CycleDisplay
        {
            get => (int)Interlocked.Read(ref _cycleDisplay);
            set
            {
                Interlocked.Exchange(ref _cycleDisplay, value);
            }
        }

        double LenWidth
        {
            get
            {
                lock (_syncLenWidth)
                {
                    return _lenWidth;
                }
            }
            set
            {
                lock (_syncLenWidth)
                {
                    if (_lenWidth != value)
                    {
                        _lenWidth = value;
                    }
                }
                    
            }
        }
        bool IsCenter
        {
            get => _isCenter;
            set
            {
                if (_isCenter != value)
                {
                    _isCenter = value;
                    if (_isCenter)
                        iconCenter.Foreground = new SolidColorBrush(COLOR_CONNECTED);
                    else
                        iconCenter.Foreground = new SolidColorBrush(COLOR_DISCONNECTED);
                }
            }
        }

        int RoiTop
        {
            get => (int)Interlocked.Read(ref _roiTop);
            set
            {
                Interlocked.Exchange(ref _roiTop, value);
            }
        }

        int RoiBottom
        {
            get => (int)Interlocked.Read(ref _roiBottom);
            set
            {
                Interlocked.Exchange(ref _roiBottom, value);
            }
        }

        int Threshold
        {
            get => (int)Interlocked.Read(ref _threshold);
            set
            {
                Interlocked.Exchange(ref _threshold, value);
            }
        }

        int Rotation
        {
            get => (int)Interlocked.Read(ref _rotation);
            set
            {
                Interlocked.Exchange(ref _rotation, value);
            }
        }

        int RectNoise
        {
            get => (int)Interlocked.Read(ref _rectNoise);
            set
            {
                Interlocked.Exchange(ref _rectNoise, value);
            }
        }

        int DetectionDistance
        {
            get => (int)Interlocked.Read(ref _detecionDistance);
            set
            {
                Interlocked.Exchange(ref _detecionDistance, value);
            }
        }

        public WindowLaserSetting()
        {
            this.Loaded += WindowLaserSetting_Loaded;
            this.Closing += WindowLaserSetting_Closing;
            InitializeComponent();
        }

        private async void WindowLaserSetting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            if (_firstClose)
            {
                _backgroundTokenSource?.Cancel();
                _firstClose = false;
                e.Cancel = true;
                try
                {
                    await _camera.StopAsync();
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

        private void WindowLaserSetting_Loaded(object sender, RoutedEventArgs e)
        {
            var cfg = LaserConfigService.ReadConfig();
            _laserConfig = cfg;
            cboCameraFrameType.Items.Clear();
            cboCameraFrameType.Items.Add("Camera Frame 1#");
            cboCameraFrameType.Items.Add("Camera Frame 2#");
            scrollTop.Value = cfg.CameraShort.RoiTop;
            scrollBottom.Value = cfg.CameraShort.RoiBottom;
            trackThreshold.Value = cfg.CameraShort.Threshold;
            nudRectNoise.Value=cfg.CameraShort.RectNoise;
            nudDetationDistance.Value = cfg.CameraShort.DetectionDistance;
            nudLenWidth.Value = (decimal)cfg.CameraShort.LenWidth;
            nudFrame.Value = cfg.CameraShort.Frame;
            nudCycleDisplay.Value = cfg.CameraShort.CycleDisplay;
            nudRotate.Value = cfg.CameraShort.Rotation;
            cboCameraFrameType.SelectedIndex = 0;
            cboCameraFrameType.SelectedIndexChanged += cboCameraFrameType_SelectedIndexChanged;
            _camera.OnImage += _camera_OnImage;
            _camera.Run(cfg.CameraShort);
        }

        private void cboCameraFrameType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if(cboCameraFrameType.SelectedIndex==0)
            {
                // lưu lại giá trị trước
                _laserConfig.CameraLong = GetCamConfig();// lưu lại long rồi chuyển sang short

                scrollTop.Value = _laserConfig.CameraShort.RoiTop;
                scrollBottom.Value = _laserConfig.CameraShort.RoiBottom;
                trackThreshold.Value = _laserConfig.CameraShort.Threshold;
                nudRectNoise.Value = _laserConfig.CameraShort.RectNoise;
                nudDetationDistance.Value = _laserConfig.CameraShort.DetectionDistance;
                nudLenWidth.Value = (decimal)_laserConfig.CameraShort.LenWidth;
                nudFrame.Value = _laserConfig.CameraShort.Frame;
                nudCycleDisplay.Value = _laserConfig.CameraShort.CycleDisplay;
                nudRotate.Value = _laserConfig.CameraShort.Rotation;
            }
            else
            {
                _laserConfig.CameraShort = GetCamConfig();// lưu lại short rồi chuyển sang long

                scrollTop.Value = _laserConfig.CameraLong.RoiTop;
                scrollBottom.Value = _laserConfig.CameraLong.RoiBottom;
                trackThreshold.Value = _laserConfig.CameraLong.Threshold;
                nudRectNoise.Value = _laserConfig.CameraLong.RectNoise;
                nudDetationDistance.Value = _laserConfig.CameraLong.DetectionDistance;
                nudLenWidth.Value = (decimal)_laserConfig.CameraLong.LenWidth;
                nudFrame.Value = _laserConfig.CameraLong.Frame;
                nudCycleDisplay.Value = _laserConfig.CameraLong.CycleDisplay;
                nudRotate.Value = _laserConfig.CameraLong.Rotation;
            }
        }


        private CameraConfig_Model GetCamConfig()
        {
            return new CameraConfig_Model()
            {
                RoiTop=RoiTop,
                RoiBottom=RoiBottom,
                Threshold=Threshold,
                Rotation=Rotation,
                RectNoise = RectNoise,
                DetectionDistance=DetectionDistance,
                LenWidth=LenWidth,
                CycleDisplay=CycleDisplay,
                Frame=Frame,
            };
        }

        int _countDelayImage = 0;
        
        private void _camera_OnImage(object sender, Models.Camera.CameraImage_EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (++_countDelayImage >= CycleDisplay)
                {
                    _countDelayImage = 0;

                    if (picCamera.Source != null)
                        picCamera.Source = null;
                    picCamera.Source = ImageHandleDebug(e.Image,GetCamConfig());
                    if (picCameraResult.Source != null)
                        picCameraResult.Source = null;
                    var result = ImageLaserService.ImageHandleResult(e.Image, GetCamConfig());
                    picCameraResult.Source = result.Image;
                    if (result.IsCalculatorSuccess)
                    {
                        txtCenterDistancePixcel.Text = result.DistancePixcel.ToString();
                        txtCenterDistanceMm.Text = result.DistanceMm.ToString("F4");
                        IsCenter = result.IsCenter;
                    }
                    else
                    {
                        IsCenter = false;
                        txtCenterDistancePixcel.Text = "---";
                        txtCenterDistanceMm.Text = "---";
                    }
                }
            }));
        }

        

        private ImageSource ImageHandleDebug(Bitmap bitmap,CameraConfig_Model cfg)
        {
            var imageRaw = BitmapConverter.ToMat(bitmap);
            // Xác định tâm của hình ảnh (tọa độ x, y)
            Point2f center = new Point2f(imageRaw.Width / 2, imageRaw.Height / 2);
            // Tạo ma trận xoay (góc xoay 45 độ)
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, cfg.Rotation, 1.0); // Góc xoay 45 độ, scale 1.0
                                                                           // Áp dụng ma trận xoay lên hình ảnh
                                                                           // Kết quả hình ảnh sau khi xoay
            Mat rotatedImage = new Mat();
            Cv2.WarpAffine(imageRaw, rotatedImage, rotationMatrix, new OpenCvSharp.Size(imageRaw.Width, imageRaw.Height),
                            borderMode: BorderTypes.Constant, borderValue: new Scalar(255, 255, 255));

            // Convert the image to grayscale
            Mat grayImage = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(rotatedImage, grayImage, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            // Threshold the image
            OpenCvSharp.Mat binaryImage = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Threshold(grayImage, binaryImage, cfg.Threshold, 255, OpenCvSharp.ThresholdTypes.Binary);
            var image = binaryImage;


            var imageHight = image.Height;
            int top = cfg.RoiTop * imageHight / LaserConfigService.CAMERA_ROI_MAX;
            if (top > 0)
                top = top - 1;

            int bottom = cfg.RoiBottom * imageHight / LaserConfigService.CAMERA_ROI_MAX;
            if(bottom>0)
                bottom = bottom - 1;

            // vẽ đường trên cùng
            OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(0, top),
                                        new OpenCvSharp.Point(image.Width - 1, top),
                                        OpenCvSharp.Scalar.Orange, 10, OpenCvSharp.LineTypes.AntiAlias);
            // vẽ đường dưới dùng
            OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(0, bottom),
                                        new OpenCvSharp.Point(image.Width - 1, bottom),
                                        OpenCvSharp.Scalar.Orange, 10, OpenCvSharp.LineTypes.AntiAlias);

            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image).BitmapToImageSource();
        }

        private void trackThreshold_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            Threshold = (int)trackThreshold.Value;
            nudThreshold.Value=Threshold;
        }

        private void scrollTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RoiTop = (int)scrollTop.Value;
        }

        private void scrollBottom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RoiBottom = (int)scrollBottom.Value;
        }

        private void trackRotate_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            
            Rotation = (int)trackRotate.Value;
            nudRotate.Value = Rotation;
            
        }

        private void nudThreshold_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
           trackThreshold.Value=(int)nudThreshold.Value;
        }

        private void nudLenWith_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            LenWidth = (double)nudLenWidth.Value;
        }

       
        private void nudCycleDisplay_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            CycleDisplay = (int)nudCycleDisplay.Value;
        }

        private void nudFrame_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            Frame = (int)nudFrame.Value;
        }

        private void nudRotate_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            trackRotate.Value = (int)nudRotate.Value;
        }

        private void nudRectNoise_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            RectNoise = (int)nudRectNoise.Value;
        }

        private void nudDetationDistance_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            DetectionDistance = (int)nudDetationDistance.Value;
        }


        

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(RoiTop>=RoiBottom)
            {
                DXMessageBox.Show("Invalid range of values scroll top and scroll bottom","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // lưu cấu hình
            if(cboCameraFrameType.SelectedIndex == 0)
                _laserConfig.CameraShort = GetCamConfig();
            else
                _laserConfig.CameraLong = GetCamConfig();
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
