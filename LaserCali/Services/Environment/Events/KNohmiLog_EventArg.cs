using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Events
{
    public class KNohmiLog_EventArg : EventArgs
    {
        public KNohmiLog_EventArg(object sender, string message)
        {
            Sender = sender;
            Message = message;
        }

        public object Sender { get; private set; }
        public string Message { get; private set; }
    }
}
