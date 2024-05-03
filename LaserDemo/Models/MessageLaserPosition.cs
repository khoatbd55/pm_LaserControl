using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDemo.Models
{
    internal class MessageLaserPosition:MessageLaserBase
    {
        public double Pos { get; set; }
        public float Beam { get; set; }
        public uint Smpl { get; set; }
        public TRIGSTAT Trig { get; set; }
    }
}
