using LaserCali.Services.Environment.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Events
{
    public class KNohmiEnvironment_EventArg : EventArgs
    {
        public KNohmiEnvironment_EventArg(object sender, KNohmiEnvironmentMessage message)
        {
            Sender = sender;
            Message = message;
        }

        public object Sender { get; private set; }
        public KNohmiEnvironmentMessage Message { get; private set; }
    }
}
