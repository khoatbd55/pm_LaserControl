using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Config
{
    public class LaserConfig_Model
    {
        public string EnviromentNameComport { get; set; }
        public string TempNameComport { get; set; }
        public string DisplayNameComport { get; set; }
        public int CameraTopPoint { get; set; }
        public int CameraBottomPoint { get; set; }
        public int CameraThreshold { get; set; }
    }
}
