using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Camera
{
    public class CameraConnection_EventArg : EventArgs
    {
        public bool IsConnected { get; set; }
    }
}
