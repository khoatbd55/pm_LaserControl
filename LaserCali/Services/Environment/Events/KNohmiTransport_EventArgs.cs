using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Events
{
    public class KNohmiTransport_EventArgs
    {
        public string Message { get; set; }
        public byte[] Raw { get; set; }
    }
}
