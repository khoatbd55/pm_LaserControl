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
            SaveData("LaserSetting",JsonConvert.SerializeObject(model));
        }

        public static LaserConfig_Model LoadConfig()
        {
            var str = GetData("LaserSetting");
            LaserConfig_Model model = new LaserConfig_Model()
            {
                CameraBottomPoint = 0,
                CameraThreshold=80,
                CameraTopPoint=1943,
                DisplayNameComport="COM1",
                EnviromentNameComport="COM2",
                TempNameComport="COM3"
            };
            try
            {
                if(str!=null || str != "")
                {
                    model=JsonConvert.DeserializeObject<LaserConfig_Model>(str);
                }
            }
            catch (Exception)
            {

            }
            return model;
        }
    }
}
