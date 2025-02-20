using LaserCali.Models.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LaserCali.Services.Api
{
    public class ApiBase_Service: IDisposable
    {
        protected HttpClient _httpClient;
        public ApiBase_Service()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.BaseAddress = new Uri(AppConst.HostApi+ $":{AppConst.HostPort}/");
        }

        protected void AddDefaultRequestReader()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + AppConst.TokenApi);
        }
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
