using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Models.Message
{
    public class KNohmiLogMessage : KEnvironmentBaseMessage
    {
        public KNohmiLogMessage(DateTime createdAt, string content)
        {
            CreatedAt = createdAt;
            Content = content;
        }
        public DateTime CreatedAt { get; private set; }
        public string Content { get; private set; }
    }
}
