using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Temperatures.Sensor
{
    public class MultiTempStatus_Model
    {
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("temps")]
        public List<TemperatureValue_Model> Temps { get; set; }
    }
}
