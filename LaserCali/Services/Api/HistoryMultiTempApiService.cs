using LaserCali.Models.Api;
using LaserCali.Models.Temperatures.Sensor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LaserCali.Services.Api
{
    public class HistoryMultiTempApiService:ApiBase_Service
    {
        public HistoryMultiTempApiService() : base()
        {

        }

        public async Task<List<MultiTempStatus_Model>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"api/history/search/lastest");
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<List<MultiTempStatus_Model>>(await response.Content.ReadAsStringAsync());
                return res;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    var res = JsonConvert.DeserializeObject<MyErrorResponse>(await response.Content.ReadAsStringAsync());
                    throw new Exception(res.Message);
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }
            }
        }
    }
}
