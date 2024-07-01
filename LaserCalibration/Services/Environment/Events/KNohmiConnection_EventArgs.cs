using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCalibration.Services.Environment.Events
{
    public class KNohmiConnection_EventArgs : EventArgs
    {
        public KNohmiConnection_EventArgs(object sender, bool isConnected)
        {
            Sender = sender;
            IsConnected = isConnected;
        }
        public bool IsConnected { get;private set; }

        public object Sender { get; private set; }
        public EventArgs EventArgs { get; private set; }
    }
}
