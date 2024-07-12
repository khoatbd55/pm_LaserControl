using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LaserCalibration.Models.Camera
{
    public class CameraImage_EventArgs:EventArgs
    {
        public Bitmap Image { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
