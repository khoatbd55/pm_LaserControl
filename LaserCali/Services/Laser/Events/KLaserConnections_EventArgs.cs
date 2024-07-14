using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Laser.Events
{
    public class KLaserConnections_EventArgs:EventArgs
    {
        public bool IsConnected { get;private set; }
        public KLaserConnections_EventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}
