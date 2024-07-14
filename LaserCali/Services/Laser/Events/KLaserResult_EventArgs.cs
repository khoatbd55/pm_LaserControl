using LaserCali.Laser.LaserWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Laser.Events
{
    public class KLaserResult_EventArgs:EventArgs
    {
        public double Pos { get; private set; }
        public float Beam { get;private set; }
        public uint Smpl { get;private set; }
        public TRIGSTAT Trig { get; private set; }

        public KLaserResult_EventArgs(double pos, float beam, uint smpl, TRIGSTAT trig)
        {
            Pos = pos;
            Beam = beam;
            Smpl = smpl;
            Trig = trig;
        }
    }
}
