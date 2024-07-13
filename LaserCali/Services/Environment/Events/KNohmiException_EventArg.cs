using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Events
{
    public class KNohmiException_EventArg : EventArgs
    {
        public KNohmiException_EventArg(object sender, Exception ex)
        {
            Sender = sender;
            Ex = ex;
        }

        public object Sender { get; private set; }
        public Exception Ex { get; private set; }
    }
}
