using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using OpenCvSharp;
using LaserCali.Extention;
using LaserCali.Models.Camera;
using LaserCali.Models.Config;
using LaserCali.Services.Config;

namespace LaserCali.Services
{
    public class ImageLaserService
    {
        public static CameraHandle_Model ImageHandleResult(Bitmap bitmap, CameraConfig_Model cfg,bool isHorizontal)
        {
            CameraHandle_Model result = new CameraHandle_Model();
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

            var imageHight = imageRaw.Height;
            int top = cfg.RoiTop * imageHight / LaserConfigService.CAMERA_ROI_MAX;
            if (top > 0)
                top = top - 1;

            int bottom = cfg.RoiBottom * imageHight / LaserConfigService.CAMERA_ROI_MAX;
            if (bottom > 0)
                bottom = bottom - 1;
            // Xác định vùng ROI (tọa độ x, y, width, height)
            OpenCvSharp.Rect roi = new OpenCvSharp.Rect(0, top, imageRaw.Width, bottom - top);

            // Convert the image to grayscale
            Mat grayImage = new OpenCvSharp.Mat();
            // Cắt ảnh từ ROI
            Mat roiImage = new Mat(rotatedImage, roi);
            OpenCvSharp.Cv2.CvtColor(roiImage, grayImage, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            // Threshold the image
            OpenCvSharp.Mat binaryImage = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Threshold(grayImage, binaryImage, cfg.Threshold, 255, OpenCvSharp.ThresholdTypes.Binary);


            var image = roiImage;
            // Tính toán tâm của ảnh
            OpenCvSharp.Point centerPoint = new OpenCvSharp.Point(image.Width / 2, image.Height / 2);

            

            //return;
            // Find contours
            OpenCvSharp.Point[][] contours;
            OpenCvSharp.HierarchyIndex[] hierarchy;
            OpenCvSharp.Cv2.FindContours(binaryImage, out contours, out hierarchy, OpenCvSharp.RetrievalModes.External, OpenCvSharp.ContourApproximationModes.ApproxSimple);
            List<OpenCvSharp.Rect> _listPoint = new List<OpenCvSharp.Rect>();


            //int idx = 0;
            foreach (var contour in contours)
            {
                // Get the bounding rectangle of the contour
                OpenCvSharp.Rect boundingRect = OpenCvSharp.Cv2.BoundingRect(contour);
                if (boundingRect.Width > cfg.RectNoise && boundingRect.Height > cfg.RectNoise)
                {
                    // Calculate the center point of the bounding rectangle
                    //Point center = new Point(boundingRect.X + boundingRect.Width / 2, boundingRect.Y + boundingRect.Height / 2);
                    _listPoint.Add(boundingRect);
                }
            }

            int minDistance = -1;
            for (int i = 0; i < _listPoint.Count - 1; i++)
            {
                var current = _listPoint[i];
                var next = _listPoint[i + 1];
                var middleX = current.X - (current.X - next.X - next.Width) / 2;
                OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(middleX, 0),
                                new OpenCvSharp.Point(middleX, current.Y + image.Height), OpenCvSharp.Scalar.LightGreen, 10);
                int deltaDistance = Math.Abs(middleX - centerPoint.X);
                if (minDistance < 0)
                    minDistance = deltaDistance;
                else if (minDistance > deltaDistance)
                    minDistance = deltaDistance;
            }

            // Vẽ một điểm tại tâm ảnh (có thể sử dụng hình tròn hoặc điểm)
            //Cv2.Circle(image, centerImage, 5, Scalar.Red, -1); // Vẽ một điểm màu đỏ tại tâm ảnh
            OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(centerPoint.X, centerPoint.Y - image.Height / 2),
                                        new OpenCvSharp.Point(centerPoint.X, centerPoint.Y + image.Height / 2),
                                        OpenCvSharp.Scalar.Orange, 10, OpenCvSharp.LineTypes.AntiAlias);
            if(isHorizontal)
                OpenCvSharp.Cv2.Line(
                            image,
                            new OpenCvSharp.Point(centerPoint.X - image.Width / 2, centerPoint.Y),
                            new OpenCvSharp.Point(centerPoint.X + image.Width / 2, centerPoint.Y),
                            OpenCvSharp.Scalar.Orange, 10, OpenCvSharp.LineTypes.AntiAlias);


            if (minDistance >= 0)
            {
                result.IsCalculatorSuccess = true;
                result.DistancePixcel = minDistance;
                result.DistanceMm = ((double)minDistance) * cfg.LenWidth / image.Width;
                if (result.DistancePixcel <= cfg.DetectionDistance)
                {
                    result.IsCenter = true;
                }
                else
                {
                    result.IsCenter = false;
                }
            }
            result.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image).BitmapToImageSource();
            return result;
        }

    }
}
