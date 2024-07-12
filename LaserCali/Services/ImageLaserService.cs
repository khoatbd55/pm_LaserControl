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

namespace LaserCali.Services
{
    public class ImageLaserService
    {
        public static ImageSource ImageHandle(Bitmap bitmap)
        {
            var image = BitmapConverter.ToMat(bitmap);
            // Convert the image to grayscale
            Mat grayImage = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(image, grayImage, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

            OpenCvSharp.Mat blurred = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.GaussianBlur(grayImage, blurred, new OpenCvSharp.Size(5, 5), 0);

            // Threshold the image
            OpenCvSharp.Mat binaryImage = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Threshold(grayImage, binaryImage, 80, 255, OpenCvSharp.ThresholdTypes.Binary);

            //if (picDebug.Image != null)
            //    picDebug.Image = null;
            //picDebug.Image = BitmapConverter.ToBitmap(blurred);

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
                OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(middleX, 0), new OpenCvSharp.Point(middleX, current.Y + image.Height), OpenCvSharp.Scalar.Green, 10);
            }

            // Tính toán tâm của ảnh
            OpenCvSharp.Point centerImage = new OpenCvSharp.Point(image.Width / 2, image.Height / 2);

            // Vẽ một điểm tại tâm ảnh (có thể sử dụng hình tròn hoặc điểm)
            //Cv2.Circle(image, centerImage, 5, Scalar.Red, -1); // Vẽ một điểm màu đỏ tại tâm ảnh
            OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(centerImage.X, centerImage.Y - image.Height / 2),
                new OpenCvSharp.Point(centerImage.X, centerImage.Y + image.Height / 2), OpenCvSharp.Scalar.Orange, 10, OpenCvSharp.LineTypes.AntiAlias);
            return BitmapToImageSource(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image));
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

    }
}
