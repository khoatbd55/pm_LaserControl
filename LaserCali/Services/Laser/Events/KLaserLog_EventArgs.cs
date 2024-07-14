
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Laser.Events
{
    public class KLaserLog_EventArgs:EventArgs
    {
        public string Message { get; private set; }
        public KLaserLog_EventArgs(string message)
        {
            Message = message;  
        }
    }
}
