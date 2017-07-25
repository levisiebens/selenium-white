using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Remote;

namespace SeleniumWhiteAutomationLibrary
{
    public class SeleniumWhiteDriver
    {
        private static int _port = 1234;
        

        public static RemoteWebDriver GenerateDriver(RemoteWebDriver remoteWebDriver = null, String hubUrl = "")
        {
            //Get Port for SeleniumWhite if it's set
            var port = ConfigurationManager.AppSettings.Get("SeleniumWhitePort");
            if (!string.IsNullOrEmpty(port))
            {
                _port = Convert.ToInt32(port);
            }

            var desiredCapabilities = new DesiredCapabilities();
            desiredCapabilities.SetCapability("browserName", "white");
            desiredCapabilities.SetCapability("version", "1.0");
            var seleniumGridUri = Regex.Replace(hubUrl, "/wd/hub", String.Empty);
            var seleniumWhiteServerIpAddress = String.Empty;

            //If we do not have a remote grid in the conneciton file, then we are running locally. So use the local address to run against.
            if (seleniumGridUri.Equals(String.Empty))
            {
                seleniumWhiteServerIpAddress = "http://localhost";
            }

            else
            {
                var sessionId = remoteWebDriver.Capabilities.GetCapability("webdriver.remote.sessionid").ToString();
                if (sessionId != null)
                {
                    var sessionRequest = HttpClientHelper.GetHttpClient(seleniumGridUri);
                    
                    var result = sessionRequest.PostAsync(new Uri(String.Format("{0}/grid/api/testsession?session={1}", seleniumGridUri, sessionId)), null).Result;

                    var testSessionResponse = JsonConvert.DeserializeObject<TestSessionResponse>(result.Content.ReadAsStringAsync().Result);

                    seleniumWhiteServerIpAddress = Regex.Replace(testSessionResponse.ProxyId, ":[0-9].*", String.Empty);
                }
            }

            return new RemoteWebDriver(new Uri(String.Format("{0}:{1}", seleniumWhiteServerIpAddress, _port)), desiredCapabilities);
        }
    }
}
