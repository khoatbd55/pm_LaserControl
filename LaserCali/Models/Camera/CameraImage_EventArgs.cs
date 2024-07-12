using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media;

namespace LaserCali.Models.Camera
{
    public class CameraImage_EventArgs : EventArgs
    {
        public Bitmap Image { get; set; }
    }
}
