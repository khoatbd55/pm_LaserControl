using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Models.Consts
{
    public class AppConst
    {
        public const string HEADER_MQTT_LO_NHIET_STATUS = "temp/status";
        // topic trạng thiết bị
        public const string HEADER_CACHE_LO_NHIET_STATUS = "temp:status:";
        // topic thông tin lịch sử
        public const string HEADER_CACHE_HISTORY_INFO = "temp:log:";

        public const string HEADER_MQTT_LO_NHIET_Log = "temp/log";

        public const int TotalSlave = 16;
    }
}
