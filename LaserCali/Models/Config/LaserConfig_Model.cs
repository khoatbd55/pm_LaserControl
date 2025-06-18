using LaserCali.Models.Enums;
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
            CameraShort=new CameraConfig_Model();
            CameraLong=new CameraConfig_Model();
        }
        public string EnviromentNameComport { get; set; }
        public string TempNameComport { get; set; }
        public string DisplayNameComport { get; set; }
        public string MqttHost { get; set; }
        public int LaserValueResolution { get; set; } = 3;
        public bool UseLaserFumula { get; set; } = false;
        public ETemperatureType TemperatureType { get; set; } = ETemperatureType.TwoPoint;
        public CameraConfig_Model CameraShort { get; set; }
        public CameraConfig_Model CameraLong { get; set; }
    }
}
