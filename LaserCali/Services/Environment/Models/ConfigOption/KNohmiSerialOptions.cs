using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Models.ConfigOption
{
    public class KNohmiSerialOptions
    {
        public string PortName { get; set; }
        public int Baudrate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.None;
        public int DataBit { get; set; } = 8;
        public StopBits StopBit { get; set; } = StopBits.One;
        public int BufferSize { get; set; } = 1000;
    }
}
