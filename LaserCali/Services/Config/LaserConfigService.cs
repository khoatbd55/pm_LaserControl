using LaserCali.Models.Config;
using LaserCali.Models.Consts;
using LaserCali.Models.Enums;
using LaserCali.Models.Export;
using LaserCali.Models.Sensor;
using LaserCali.Properties;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Config
{
    public class LaserConfigService
    {

        public const int CAMERA_ROI_MIN = 0;
        public const int CAMERA_ROI_MAX = 100;

        public const int CAMERA_ROTATION_MIN = 0;
        public const int CAMERA_ROTATION_MAX = 365;

        public const int CAMERA_THRESHOLD_MIN = 0;
        public const int CAMERA_THRESHOLD_MAX = 255;

        public const int CAMERA_RECT_NOISE_MIN = 10;
        public const int CAMERA_RECT_NOISE_MAX = 500;

        public const double LEN_WIDTH_MIN = 1;
        public const double LEN_WIDTH_MAX = 500;

        public const int FRAME_MAX = 14;
        public const int FRAME_MIN = 1;

        public const int CYCLE_DISPLAY_MAX = 14;
        public const int CYCLE_DISPLAY_MIN = 1;

        public const string KeyLaser = "LaserConfig3";
        public const string KeyCommon = "CommonConfig";
        public const string KeyDut = "Dut";
        public const string KeySensorPos = "SensorPos";

        private static void SaveData(string key, string value)
        {
            Settings.Default[key] = value;
            Settings.Default.Save();
        }

        private static string GetData(string key)
        {
            string result = Settings.Default[key].ToString();
            return result;
        }

        public static List<SensorPos_Model> ReadSensorPositionConfig()
        {
            var str = GetData(KeySensorPos);
            if (str != null && str != "")
            {
                return JsonConvert.DeserializeObject<List<SensorPos_Model>>(str);
            }
            else
            {
                return new List<SensorPos_Model>()
                {
                    new SensorPos_Model(1,200),
                    new SensorPos_Model(2,2200),
                    new SensorPos_Model(3,4250),
                    new SensorPos_Model(4,6300),
                    new SensorPos_Model(5,8350),
                    new SensorPos_Model(6,10300),
                    new SensorPos_Model(7,12300),
                    new SensorPos_Model(8,14350),
                    new SensorPos_Model(9,16400),
                    new SensorPos_Model(10,18450),
                    new SensorPos_Model(11,20450),
                    new SensorPos_Model(12,22500),
                    new SensorPos_Model(13,24550),
                    new SensorPos_Model(14,26600),
                    new SensorPos_Model(15,28650),
                    new SensorPos_Model(16,30000)
                };
            }
        }

        public static void SaveSensorPositionConfig(List<SensorPos_Model> list)
        {
            SaveData(KeySensorPos, JsonConvert.SerializeObject(list));
        }

        public static DutInformation_Model ReadDutConfig()
        {
            var str = GetData(KeyDut);
            DutInformation_Model model = new DutInformation_Model();
            if (str != null && str != "")
            {
                model = JsonConvert.DeserializeObject<DutInformation_Model>(str);
            }
            else
            {
                model=new DutInformation_Model()
                {
                    Name = "DUT",
                    Model = "Model",
                    Serial = "Serial",
                    Range = "Range",
                    Resolution = "Resolution",
                    Grade = "Grade",
                    Manufacturer = "Manufacturer"
                };
            }    
            return model;
        }

        public static void SaveDutConfig(DutInformation_Model model)
        {
            SaveData(KeyDut, JsonConvert.SerializeObject(model));
        }

        public static CommonConfig_Model ReadCommonConfig()
        {
            var str = GetData(KeyCommon);
            CommonConfig_Model model = new CommonConfig_Model();
            if (str != null && str != "")
            {
                model = JsonConvert.DeserializeObject<CommonConfig_Model>(str);
            }
            return model;
        }

        public static void CommonConfigSave(CommonConfig_Model model)
        {
            SaveData(KeyCommon, JsonConvert.SerializeObject(model));
        }

        public static void SaveConfig(LaserConfig_Model model)
        {
            Limit(model);
            SaveData(KeyLaser, JsonConvert.SerializeObject(model));
            AppConst.HostApi = "http://" + model.MqttHost;
        }

        private static void Limit(LaserConfig_Model model)
        {
            if (model.CameraShort.RoiBottom > CAMERA_ROI_MAX)
                model.CameraShort.RoiBottom = CAMERA_ROI_MAX;
            if (model.CameraShort.RoiBottom < CAMERA_ROI_MIN)
                model.CameraShort.RoiBottom = CAMERA_ROI_MIN;
            if (model.CameraShort.RoiTop > CAMERA_ROI_MAX)
                model.CameraShort.RoiTop = CAMERA_ROI_MAX;
            if (model.CameraShort.RoiTop < CAMERA_ROI_MIN)
                model.CameraShort.RoiTop = CAMERA_ROI_MIN;
            if (model.CameraShort.Rotation > CAMERA_ROTATION_MAX)
                model.CameraShort.Rotation = CAMERA_ROTATION_MAX;
            if (model.CameraShort.Rotation < CAMERA_ROTATION_MIN)
                model.CameraShort.Rotation = CAMERA_ROI_MIN;
            if (model.CameraShort.Threshold > CAMERA_THRESHOLD_MAX)
                model.CameraShort.Threshold = CAMERA_THRESHOLD_MAX;
            if (model.CameraShort.Threshold < CAMERA_THRESHOLD_MIN)
                model.CameraShort.Threshold = CAMERA_THRESHOLD_MIN;
            if (model.CameraShort.RectNoise > CAMERA_RECT_NOISE_MAX)
                model.CameraShort.RectNoise = CAMERA_RECT_NOISE_MAX;
            if (model.CameraShort.Threshold < CAMERA_RECT_NOISE_MIN)
                model.CameraShort.Threshold = CAMERA_RECT_NOISE_MIN;
            if(model.CameraShort.LenWidth>LEN_WIDTH_MAX)
                model.CameraShort.LenWidth = LEN_WIDTH_MAX;
            if(model.CameraShort.LenWidth<LEN_WIDTH_MIN)
                model.CameraShort.LenWidth=LEN_WIDTH_MIN;
            if (model.CameraShort.Frame > FRAME_MAX)
                model.CameraShort.Frame = FRAME_MAX;
            if (model.CameraShort.Frame < FRAME_MIN)
                model.CameraShort.Frame = FRAME_MIN;
            if(model.CameraShort.CycleDisplay>CYCLE_DISPLAY_MAX)
                model.CameraShort.CycleDisplay =CYCLE_DISPLAY_MAX;
            if(model.CameraShort.CycleDisplay<CYCLE_DISPLAY_MIN)
                model.CameraShort.CycleDisplay=CYCLE_DISPLAY_MIN;
        }

        public static LaserConfig_Model ReadConfig()
        {
            var str = GetData(KeyLaser);
            LaserConfig_Model model = new LaserConfig_Model()
            {
                CameraShort=new CameraConfig_Model()
                {
                    RoiBottom = CAMERA_ROI_MAX,
                    Threshold = 80,
                    RoiTop = CAMERA_ROI_MIN,
                    Rotation = CAMERA_ROTATION_MIN,
                    RectNoise=20,
                    CycleDisplay=2,
                    Frame=10,
                    DetectionDistance=5,
                    LenWidth=5.7
                },
                CameraLong = new CameraConfig_Model()
                {
                    RoiBottom = CAMERA_ROI_MAX,
                    Threshold = 80,
                    RoiTop = CAMERA_ROI_MIN,
                    Rotation = CAMERA_ROTATION_MIN,
                    RectNoise=20,
                    CycleDisplay=2,
                    Frame=10,
                    DetectionDistance=5,
                    LenWidth=5.7
                },
                EnvHost ="192.168.144.201",
                EnvPort=502,
                TempNameComport="COM3",
                MqttHost="192.168.144.108",
                LaserValueResolution=3,
                TemperatureType = ETemperatureType.Avg,
                UseLaserFumula=false
            };
            try
            {
                if(str!=null &&  str != "")
                {
                    model=JsonConvert.DeserializeObject<LaserConfig_Model>(str);
                }
            }
            catch (Exception)
            {

            }
            Limit(model);
            return model;
        }
    }
}
