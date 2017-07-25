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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumWhiteLibrary
{
    public class Listener
    {

        private Dictionary<ApiPattern, MiniHttpServer.RequestHandler> handlersTable;

        private UInt16 port;

        private TcpListener listener;
        private int LIMIT = 1;

        private bool listenerStarted = false;

        private IPAddress localAddr = IPAddress.Parse("0.0.0.0");

        public Listener(UInt16 port, Dictionary<ApiPattern, MiniHttpServer.RequestHandler> handlersTable)
        {            
            this.port = port;

            listener = new TcpListener(localAddr, port);
            listener.Start();
            listenerStarted = true;

            for (int i = 0; i < LIMIT; i++)
            {
                var t = new Thread(new ThreadStart(Service));
                t.Start();
            }

            this.handlersTable = handlersTable;

            Console.WriteLine("Listening On: {0}:{1}", localAddr, port);
        }

        public void Service()
        {
            Console.WriteLine("Waiting for a connection... ");

            while (true)
            {
                try
                {
                    if (!listenerStarted)
                    {
                        Console.WriteLine("{0:h:mm:ss tt zz}: Restarting Listener", DateTime.Now);
                        listener.Start();
                    }

                    var client = listener.AcceptTcpClient();
                    Console.WriteLine("{0:h:mm:ss tt zz}: Connection Established", DateTime.Now);

                    // Get a stream object for reading and writing
                    var stream = client.GetStream();

                    var socketIo = new SocketIO(stream);

                    //Read text from socket.
                    var data = String.Empty;

                    //Construct web request
                    var request = ReadRequest(data, stream, socketIo);

                    //Log Request                    
                    Console.WriteLine("{0:h:mm:ss tt zz}: Request {1}\r\n{2}", DateTime.Now, request.Method,
                        request.ResurcePath);

                    //Determine request actions
                    var dict = new Dictionary<string, string>();
                    var handlePair = handlersTable.FirstOrDefault(h => ApiPattern.Match(h.Key, request, out dict));

                    //If there is no match
                    if (handlePair.Key == ApiPattern.Empty)
                    {
                        Helpers.Log("Unimplemented method called");
                        ResponseWith(WebResponse.NotImplemented(), stream);
                    }
                    else
                    {
                        var success = true;
                        try
                        {
                            Console.WriteLine("{0:h:mm:ss tt zz}: Matching Action Found", DateTime.Now);
                            ResponseWith(handlePair.Value(request, dict), stream);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0:h:mm:ss tt zz}: Exception Occurred During Action\r\n{1}", DateTime.Now, e.Message);
                            Console.WriteLine("{0:h:mm:ss tt zz}: StackTrace:\r\n{1}", DateTime.Now, e.StackTrace);
                            success = false;
                        }

                        if (success == false)
                        {
                            ResponseWith(WebResponse.InternalServerError(), stream);
                        }
                    }

                    stream.Close();
                    client.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine("{0:h:mm:ss tt zz}: Exception: {1}", DateTime.Now, e);
                    Console.WriteLine("{0:h:mm:ss tt zz}: Message:\r\n{1}", DateTime.Now, e.Message);
                    Console.WriteLine("{0:h:mm:ss tt zz}: StackTrace:\r\n{1}", DateTime.Now, e.StackTrace);

                    //Stop the listener if we've hit an exception state
                    listener.Stop();
                    listenerStarted = false;
                    Console.WriteLine("{0:h:mm:ss tt zz}: Listener Stopped due to exception", DateTime.Now);
                }
            }
        }

        private void ResponseWith(WebResponse response, NetworkStream stream)
        {
            var responseString = response.ToString();
            var msg = System.Text.Encoding.ASCII.GetBytes(responseString);
            stream.Write(msg, 0, msg.Length);

            var logString = responseString;
            if (logString.Length > 500)
            {
                logString = String.Format("{0}...", logString.Substring(0, 500));
            }

            Console.WriteLine("Sent: {0}", logString);
        }

        private WebRequest ReadRequest(String data, NetworkStream stream, SocketIO socketIo)
        {            
            var result = new WebRequest();

            //Get the header and remove it from the list.
//            Console.WriteLine("Reading Input Line");

            var message = socketIo.ReadLine();
            
            var headerSplits = message.Split(new char[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

            if (headerSplits.Count() >= 3)
            {
                Console.WriteLine("Correct number of headers was found");

                result.Method = GetMethod(headerSplits[0]);
                Console.WriteLine("Method: {0}", result.Method);

                result.ResurcePath = headerSplits[1];
                Console.WriteLine("ResourcePath: {0}", result.ResurcePath);

                result.HttpVerison = headerSplits[2];
                Console.WriteLine("HttpVersion: {0}", result.HttpVerison);

                result.Headers = GetHeaders(socketIo);
            }

            if (result.ContentLength > 0)
            {
                result.Content = socketIo.ReadText(result.ContentLength);
                Console.WriteLine("Content: {0}", result.Content);
            }

            else
            {
                Console.WriteLine("Result was empty.");
                result.Content = string.Empty;
            }
            
            Console.WriteLine("Result returned.");
            return result;
        }

        private Dictionary<WebRequest.WebRequestHeader, string> GetHeaders(SocketIO socket)
        {

            var result = new Dictionary<WebRequest.WebRequestHeader, string>();

            while (true)
            {
                var line = socket.ReadLine();
                var splits = line.Split(new char[] { ':' }, 2);

                if (splits.Length < 2)
                {
                    return result;
                }

                Console.WriteLine("Data: {0} Value: {1}", splits[0], splits[1].Trim());

                switch (splits[0])
                {
                    case "Content-Length":
                        result[WebRequest.WebRequestHeader.ContentLength] = splits[1].Trim(); break;
                    case "Content-Type":
                        result[WebRequest.WebRequestHeader.ContentType] = splits[1].Trim(); break;
                    case "Cookie":
                        result[WebRequest.WebRequestHeader.Cookie] = splits[1].Trim(); break;
                    case "Accept":
                        result[WebRequest.WebRequestHeader.Accept] = splits[1].Trim(); break;
                }
            }
       }


        private WebRequest.WebRequestMethod GetMethod(string methodText)
        {
            methodText.ToUpper();

            switch (methodText)
            {
                case "GET":
                    return WebRequest.WebRequestMethod.Get;
                case "POST":
                    return WebRequest.WebRequestMethod.Post;
                case "DELETE":
                    return WebRequest.WebRequestMethod.Delete;
                case "PUT":
                    return WebRequest.WebRequestMethod.Put;
                default:
                    return WebRequest.WebRequestMethod.None;
            }
        }
    }
}
