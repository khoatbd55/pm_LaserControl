using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Extention
{
    public class LaserExtention
    {
        public static double LaserFormular(double T,double RH,double P,double laserRaw)
        {
            double f = (RH / 100) * (4.07859739 + 0.44301857 * T + 0.00232093 * Math.Pow(T, 2)+ 0.00045785*Math.Pow(T,3));
            double n = 1 + (3.8369 * Math.Pow(10, -7) * P) *
                (
                    (1 + P * (0.817 - 0.0133 * T) * Math.Pow(10, -6)) /
                    (1 + 0.003661 * T)
                ) - 5.607943 * Math.Pow(10, -8) * f;
            double n0 = 1.000271332;
            return laserRaw*n/ n0;   
        }
    }
}
