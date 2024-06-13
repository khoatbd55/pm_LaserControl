using System.Text.RegularExpressions;

namespace TestRegx
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "P= 997.512 hPa T= 29.523 'C RH= 52.3232 %RH";

            // Sử dụng regex để trích xuất các số thực
            string pattern = @"[-+]?[0-9]*\.?[0-9]+";
            MatchCollection matches = Regex.Matches(input, pattern);

            // Kiểm tra số lượng kết quả
            if (matches.Count >= 3&& input.Contains("P=") && input.Contains("T=") && input.Contains("RH="))
            {
                string pressure = matches[0].Value;
                string temperature = matches[1].Value;
                string humidity = matches[2].Value;

                // In ra các giá trị đã trích xuất
                Console.WriteLine("Pressure: " + pressure);
                Console.WriteLine("Temperature: " + temperature);
                Console.WriteLine("Humidity: " + humidity);
            }
            else
            {
                Console.WriteLine("Không tìm thấy đủ số liệu trong chuỗi.");
            }
        }
    }
}
