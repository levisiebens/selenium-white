using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Remote;

namespace SeleniumWhiteAutomationLibrary
{
    public class TestSession
    {
        public TestSessionResponse GetSessionInfo(RemoteWebDriver remoteWebDriver, String hubUri = "")
        {
            var seleniumGridUri = Regex.Replace(hubUri, "/wd/hub", String.Empty);

            if (!seleniumGridUri.Equals(String.Empty))
            {
                var sessionIdProperty = typeof(RemoteWebDriver).GetProperty("SessionId", BindingFlags.Instance | BindingFlags.NonPublic);
                if (sessionIdProperty != null)
                {
                    var sessionId = sessionIdProperty.GetValue(remoteWebDriver, null) as SessionId;
                    if (sessionId != null)
                    {
                        var sessionRequest = HttpClientHelper.GetHttpClient(seleniumGridUri);
                        var result = sessionRequest.PostAsync(new Uri(String.Format("{0}/grid/api/testsession?session={1}", seleniumGridUri, sessionId)), null).Result;

                        return JsonConvert.DeserializeObject<TestSessionResponse>(result.Content.ReadAsStringAsync().Result);
                    }
                }
            }

            return new TestSessionResponse
            {
                InactivityTime = 0,
                Session = String.Format("localhost-{0}", Guid.NewGuid()),
                Success = true
            };
        }
    }
}
