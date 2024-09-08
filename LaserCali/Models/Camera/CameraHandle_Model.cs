using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LaserCali.Models.Camera
{
    public class CameraHandle_Model
    {
        public ImageSource Image { get; set; }
        public bool IsCalculatorSuccess { get; set; }
        public int DistancePixcel { get; set; }
        public bool IsCenter { get; set; }

    }
}
