using LaserCali.Models.Config;
using LaserCali.Models.Consts;
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

        public const string KeyLaser = "LaserConfig1";

        public const string KeyCommon = "CommonConfig";

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
            if (model.Camera.RoiBottom > CAMERA_ROI_MAX)
                model.Camera.RoiBottom = CAMERA_ROI_MAX;
            if (model.Camera.RoiBottom < CAMERA_ROI_MIN)
                model.Camera.RoiBottom = CAMERA_ROI_MIN;
            if (model.Camera.RoiTop > CAMERA_ROI_MAX)
                model.Camera.RoiTop = CAMERA_ROI_MAX;
            if (model.Camera.RoiTop < CAMERA_ROI_MIN)
                model.Camera.RoiTop = CAMERA_ROI_MIN;
            if (model.Camera.Rotation > CAMERA_ROTATION_MAX)
                model.Camera.Rotation = CAMERA_ROTATION_MAX;
            if (model.Camera.Rotation < CAMERA_ROTATION_MIN)
                model.Camera.Rotation = CAMERA_ROI_MIN;
            if (model.Camera.Threshold > CAMERA_THRESHOLD_MAX)
                model.Camera.Threshold = CAMERA_THRESHOLD_MAX;
            if (model.Camera.Threshold < CAMERA_THRESHOLD_MIN)
                model.Camera.Threshold = CAMERA_THRESHOLD_MIN;
            if (model.Camera.RectNoise > CAMERA_RECT_NOISE_MAX)
                model.Camera.RectNoise = CAMERA_RECT_NOISE_MAX;
            if (model.Camera.Threshold < CAMERA_RECT_NOISE_MIN)
                model.Camera.Threshold = CAMERA_RECT_NOISE_MIN;
            if(model.Camera.LenWidth>LEN_WIDTH_MAX)
                model.Camera.LenWidth = LEN_WIDTH_MAX;
            if(model.Camera.LenWidth<LEN_WIDTH_MIN)
                model.Camera.LenWidth=LEN_WIDTH_MIN;
            if (model.Camera.Frame > FRAME_MAX)
                model.Camera.Frame = FRAME_MAX;
            if (model.Camera.Frame < FRAME_MIN)
                model.Camera.Frame = FRAME_MIN;
            if(model.Camera.CycleDisplay>CYCLE_DISPLAY_MAX)
                model.Camera.CycleDisplay =CYCLE_DISPLAY_MAX;
            if(model.Camera.CycleDisplay<CYCLE_DISPLAY_MIN)
                model.Camera.CycleDisplay=CYCLE_DISPLAY_MIN;
        }

        public static LaserConfig_Model ReadConfig()
        {
            var str = GetData(KeyLaser);
            LaserConfig_Model model = new LaserConfig_Model()
            {
                Camera=new CameraConfig_Model()
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
                DisplayNameComport="COM1",
                EnviromentNameComport="COM2",
                TempNameComport="COM3",
                MqttHost="192.168.144.108",
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
