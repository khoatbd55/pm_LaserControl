using LaserCali.Models.Config;
using LaserCali.Properties;
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

        public const string KeyLaser = "Laser";

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

        public static void SaveConfig(LaserConfig_Model model)
        {
            Limit(model);
            SaveData(KeyLaser, JsonConvert.SerializeObject(model));
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
                    RectNoise=20
                },
                DisplayNameComport="COM1",
                EnviromentNameComport="COM2",
                TempNameComport="COM3"
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
