//---------------------------------------------------------------------------------------------------------------------------------------------------
// Some code was originally pulled from project: http://winphonewebdriver.codeplex.com/. This code can be compared to that project to see which code was reused.
//
// Copyright Microsoft Corporation, Inc.
// All Rights Reserved
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
// INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 
// See the Apache 2 License for the specific language governing permissions and limitations under the License.
//---------------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace SeleniumWhiteLibrary
{
    public class Helpers
    {
        /// <summary>
        /// Function will build a javascript function call from given script.
        /// Function return value will be assinged to SeleniumWindowsPhoneDriver.LastCallResult
        /// check Js\Functions.js for more.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public static string WrapFunctionCallWithResult(string script, params object[] paramters)
        {
            return string.Format("window.top.document.__wd_fn_result = {0}", WrapFunctionCall(script, paramters));
        }

        public static string WrapFunctionCall(string script, params object[] paramters)
        {
            string wrapedParams = WrapParameters(paramters);

            return string.Format("({0})({1});", script, wrapedParams);
        }

        public static string WrapString(string input)
        {
            // Use the JSON converter here, so that we can make sure
            // we get the proper escaping of quotes and backslashes.
            return JsonConvert.SerializeObject(input);
        }

        private static string WrapParameters(params object[] paramters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object param in paramters)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                if (param == null)
                {
                    sb.Append("null");
                }
                else
                {
                    sb.Append(param.ToString());
                }
            }

            return sb.ToString();
        }

        public static string BuildJsonCookieJar(CookieCollection cookieJar)
        {
            StringBuilder result = new StringBuilder("[");
            StringBuilder partialCookie = new StringBuilder();
            string pairFormat = "\"{0}\":\"{1}\",";
            List<string> partials = new List<string>();

            foreach (Cookie cookie in cookieJar)
            {
                partialCookie.Clear();
                partialCookie.Append("{");
                partialCookie.AppendFormat(pairFormat, "name", cookie.Name);
                partialCookie.AppendFormat(pairFormat, "value", cookie.Value);
                partialCookie.AppendFormat(pairFormat, "expires", cookie.Expires);
                partialCookie.AppendFormat(pairFormat, "path", cookie.Path);

                if (cookie.Secure == true)
                {
                    partialCookie.AppendFormat(pairFormat, "secure", "true");
                }
                else
                {
                    partialCookie.AppendFormat(pairFormat, "secure", "false");
                }

                partialCookie.AppendFormat("\"{0}\":\"{1}\"", "domain", cookie.Domain);
                partialCookie.Append("}");
                partials.Add(partialCookie.ToString());
            }

            result.Append(string.Join(",", partials));

            result.Append(']');
            return result.ToString();
        }

        static public void Log(string data)
        {
#if DEBUG
            Console.WriteLine("LOG: " + data);
#endif
        }

        static public void Log(string format, params object[] data)
        {
#if DEBUG
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "LOG: " + format, data);
#endif
        }
    }
}
