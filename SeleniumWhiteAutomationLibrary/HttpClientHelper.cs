using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumWhiteAutomationLibrary
{
    public class HttpClientHelper
    {
        public static HttpClient GetHttpClient(String endpoint)
        {
            var webProxy = WebRequest.GetSystemWebProxy();

            var _cookies = new CookieContainer();

            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                Proxy = webProxy,
                UseProxy = true
            };
            var _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(endpoint)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _client;
        }
    }
}
