using LaserCali.Services.Environment.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaserCali.Services.Environment.Formatter
{
    public class KNohmiFormatter
    {
        public KNohmiEnvironmentMessage Decode(string str, byte[] frameRaw)
        {
            if (str != null)
            {
                if (str.Length >= 10)
                {
                    string pattern = @"[-+]?[0-9]*\.?[0-9]+";
                    MatchCollection matches = Regex.Matches(str, pattern);
                    // Kiểm tra số lượng kết quả
                    if (matches.Count >= 3 && str.Contains("P=") && str.Contains("T=") && str.Contains("RH="))
                    {
                        KNohmiEnvironmentMessage msg = new KNohmiEnvironmentMessage();
                        msg.Pressure = double.Parse(matches[0].Value);
                        msg.Temp = double.Parse(matches[1].Value);                        
                        msg.Humi = double.Parse(matches[2].Value);
                        msg.Humi *= 0.75;// quy đổi sang mmHg
                        return msg;
                    }
                }
            }
            return null;
        }
    }
}
