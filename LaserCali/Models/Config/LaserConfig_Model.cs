using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Config
{
    public class LaserConfig_Model
    {
        public LaserConfig_Model() 
        { 
            Camera=new CameraConfig_Model();
        }
        public string EnviromentNameComport { get; set; }
        public string TempNameComport { get; set; }
        public string DisplayNameComport { get; set; }
        public CameraConfig_Model Camera { get; set; }
    }
}
