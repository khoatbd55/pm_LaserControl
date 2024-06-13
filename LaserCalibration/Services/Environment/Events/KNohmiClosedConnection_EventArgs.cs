using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCalibration.Services.Environment.Events
{
    public class KNohmiClosedConnection_EventArgs : EventArgs
    {
        public KNohmiClosedConnection_EventArgs(object sender, EventArgs eventArgs)
        {
            Sender = sender;
            EventArgs = eventArgs;
        }

        public object Sender { get; private set; }
        public EventArgs EventArgs { get; private set; }
    }
}
