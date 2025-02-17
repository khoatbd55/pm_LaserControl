using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Config
{
    public class CameraConfig_Model
    {
        public int RoiTop { get; set; }
        public int RoiBottom { get; set; }
        public int Threshold { get; set; }
        public int Rotation { get; set; }
        public int RectNoise { get; set; }
        public int DetectionDistance { get; set; }
        public double LenWidth { get; set; }
        public int Frame { get; set; }
        public int CycleDisplay { get; set; }
        

    }
}
