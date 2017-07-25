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
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SeleniumWhiteLibrary
{
    class SocketIO
    {
        private NetworkStream networkStream;
        private int ReadCharacterTimeout = 5000;

        public void Die()
        {
        }

        public SocketIO(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        public string ReadString(uint size)
        {
            if (size == 0)
            {
                return string.Empty;
            }

            return string.Empty;            
        }

        public string ReadText(uint size)
        {
            var bytes = new Byte[256];
            var stringBuilder = new StringBuilder();
            
            while (!networkStream.DataAvailable) { }

            //Read number of bytes passed in.
            for (int i = 0; i < size; i++)
            {                
                stringBuilder.Append(ReadChar());
            }
//                var numberOfBytesRead = networkStream.Read(bytes, 0, bytes.Length);
//                stringBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead));
//            } while (networkStream.DataAvailable);

            return stringBuilder.ToString();
        }

        public char? ReadChar()
        {
            var timeout = DateTime.Now;
            timeout = timeout.AddMilliseconds(ReadCharacterTimeout);
//            Console.WriteLine("SocketIO - ReadChar: Reading Character");
            while (!networkStream.DataAvailable && DateTime.Now < timeout)
            {
//                Console.WriteLine("SocketIO - ReadChar: Waiting for data...");
                
                //Wait for data to become avalible.
                Thread.Sleep(100);
            }

            if (DateTime.Now > timeout)
            {
//                Console.WriteLine("SocketIO - ReadChar: Error! Timed out waiting for data to be avalible from the Network Stream.");
                return null;
            }

            var bytes = new Byte[1];
//            Console.WriteLine("SocketIO - ReadChar: Reading character");
            networkStream.Read(bytes, 0, bytes.Length);
//            Console.WriteLine("SocketIO - ReadChar: Reading character complete");
            return (char)bytes[0];
        }

        public string ReadLine()
        {
            var builder = new StringBuilder();
            var parsedLine = false; 
//            Console.WriteLine("SocketIO - ReadLine: Reading Line");

            while (!parsedLine)
            {
//                Console.WriteLine("SocketIO - ReadLine: Getting Character");
                var a = ReadChar();

                switch (a)
                {
                    case null:
                    case '\n':
                        //If we have a /n or we cannot parse any more characters, then break the loop.
                        parsedLine = true;
                        break;
                    case '\r':
                        var b = ReadChar();
                        if (b == '\n')
                        {
                            //If we have /r/n as our end line, then break.
                            parsedLine = true;
                            break;
                        }
                        builder.Append(a);
                        builder.Append(b);
                        break;
                    default:
                        builder.Append(a);
                        break;
                }
            }

            return builder.ToString();
        }

        public void WriteString(string text)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(text);
            networkStream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", text);            
        }
    }
}
