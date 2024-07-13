using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Models.Message
{
    public class KNohmiEnvironmentMessage : KEnvironmentBaseMessage
    {
        public int TempRaw { get; set; }
        public int HumiRaw { get; set; }
        public int PressureRaw { get; set; }
        public double Temp { get; set; }
        public double Humi { get; set; }
        public double Pressure { get; set; }
    }
}
