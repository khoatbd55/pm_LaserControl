using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Sensor
{
    public class SensorPos_Model
    {
        public SensorPos_Model(int index,int position)
        {
            Index = index;
            Position = position;
        }
        public int Index { get; set; }
        public int Position { get; set; }
    }
}
