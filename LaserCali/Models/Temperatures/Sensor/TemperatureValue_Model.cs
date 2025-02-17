using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Temperatures.Sensor
{
    public class TemperatureValue_Model
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "temp")]
        public float Temp { get; set; }

        [JsonProperty(PropertyName = "isSensorConnected")]
        public bool IsSensorConnected { get; set; }

        [JsonProperty(PropertyName = "isEnable")]
        public bool IsEnable { get; set; }

        [JsonProperty(PropertyName = "tempId")]
        public int TempId { get; set; }

        [JsonProperty(PropertyName = "avgTemp")]
        public float AvgTemp { get; set; }

        [JsonProperty(PropertyName = "tempOffset")]
        public float TempOffset { get; set; }

        [JsonProperty(PropertyName = "slaveId")]
        public int SlaveId { get; set; }

    }
}
