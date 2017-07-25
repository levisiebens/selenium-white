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
using System.Threading.Tasks;
using WhiteSeleniumServer;

namespace SeleniumWhiteLibrary
{
    public class MiniHttpServer
    {
        public delegate WebResponse RequestHandler(WebRequest request, Dictionary<string, string> matchedApiValues);

        private Dictionary<ApiPattern, RequestHandler> handlersTable;
        private Listener listener;

        public MiniHttpServer(UInt16 port)
        {
            handlersTable = new Dictionary<ApiPattern, RequestHandler>();
            listener = new Listener(port, handlersTable);
        }

        public void RegisterHandle(ApiPattern pattern, RequestHandler handle)
        {
            handlersTable.Add(pattern, handle);
        }

        public void RegisterHandlers(RequestHandlers handlers)
        {
            this.RegisterHandle(
                  new ApiPattern(WebRequest.WebRequestMethod.Get, "/status"),
                  new MiniHttpServer.RequestHandler(handlers.GetStatus)
                  );

            this.RegisterHandle(
                  new ApiPattern(WebRequest.WebRequestMethod.Get, "/close"),
                  new MiniHttpServer.RequestHandler(handlers.Close)
                  );


            this.RegisterHandle(
                  new ApiPattern(WebRequest.WebRequestMethod.Post, "/session"),
                  new MiniHttpServer.RequestHandler(handlers.PostSession)
                  );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId"),
                new MiniHttpServer.RequestHandler(handlers.GetSession)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Delete, "/session/:sessionId"),
                new MiniHttpServer.RequestHandler(handlers.DeleteSession)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/url"),
                new MiniHttpServer.RequestHandler(handlers.PostUrl)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/forward"),
                new MiniHttpServer.RequestHandler(handlers.PostForward)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/back"),
                new MiniHttpServer.RequestHandler(handlers.PostBack)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/refresh"),
                new MiniHttpServer.RequestHandler(handlers.PostRefresh)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/screenshot"),
                new MiniHttpServer.RequestHandler(handlers.GetScreenshot)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/element"),
                new MiniHttpServer.RequestHandler(handlers.PostElement)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/element/:id/element"),
                new MiniHttpServer.RequestHandler(handlers.PostElementElement)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/element/:id/elements"),
                new MiniHttpServer.RequestHandler(handlers.PostElementElements)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/text"),
                new MiniHttpServer.RequestHandler(handlers.GetElementText)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/name"),
                new MiniHttpServer.RequestHandler(handlers.GetElementName)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/attribute/:name"),
                new MiniHttpServer.RequestHandler(handlers.GetElementAttribute)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/element/:id/click"),
                new MiniHttpServer.RequestHandler(handlers.PostClick)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/element/:id/value"),
                new MiniHttpServer.RequestHandler(handlers.PostElementValue)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/cookie"),
                new MiniHttpServer.RequestHandler(handlers.GetCookie)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/cookie"),
                new MiniHttpServer.RequestHandler(handlers.PostCookie)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Delete, "/session/:sessionId/cookie"),
                new MiniHttpServer.RequestHandler(handlers.DeleteCookie)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/window_handle"),
                new MiniHttpServer.RequestHandler(handlers.GetWindowHandle)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/window_handles"),
                new MiniHttpServer.RequestHandler(handlers.GetWindowHandles)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/window"),
                new MiniHttpServer.RequestHandler(handlers.SwitchToWindow)
                );            

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Delete, "/session/:sessionId/window"),
                new MiniHttpServer.RequestHandler(handlers.CloseWindow)
                );            

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/elements"),
                new MiniHttpServer.RequestHandler(handlers.PostElements)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/element/:id/submit"),
                new MiniHttpServer.RequestHandler(handlers.PostElementSubmit)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/source"),
                new MiniHttpServer.RequestHandler(handlers.GetSource)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/title"),
                new MiniHttpServer.RequestHandler(handlers.GetTitle)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/size"),
                new MiniHttpServer.RequestHandler(handlers.GetElementSize)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/selected"),
                new MiniHttpServer.RequestHandler(handlers.GetElementSelected)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Delete, "/session/:sessionId/"),
                new MiniHttpServer.RequestHandler(handlers.DeleteSession)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/location"),
                new MiniHttpServer.RequestHandler(handlers.GetElementLocation)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Delete, "/session/"),
                new MiniHttpServer.RequestHandler(handlers.DeleteSession)
             );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/element/:id/clear"),
                new MiniHttpServer.RequestHandler(handlers.PostElementClear)
             );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/displayed"),
                new MiniHttpServer.RequestHandler(handlers.GetElementDisplayed)
            );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/enabled"),
                new MiniHttpServer.RequestHandler(handlers.GetElementEnabled)
            );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Get, "/session/:sessionId/element/:id/css/:propertyName"),
                new MiniHttpServer.RequestHandler(handlers.GetElementCss)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/execute"),
                new MiniHttpServer.RequestHandler(handlers.PostExecute)
            );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/frame"),
                new MiniHttpServer.RequestHandler(handlers.PostFrame)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/buttondown"),
                new MiniHttpServer.RequestHandler(handlers.PostButtonDown)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/buttonup"),
                new MiniHttpServer.RequestHandler(handlers.PostButtonUp)
                );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/moveto"),
                new MiniHttpServer.RequestHandler(handlers.PostButtonUp)
            );

            this.RegisterHandle(
                new ApiPattern(WebRequest.WebRequestMethod.Post, "/session/:sessionId/doubleclick"),
                new MiniHttpServer.RequestHandler(handlers.PostDoubleClick)
                );


        }
    }
}
