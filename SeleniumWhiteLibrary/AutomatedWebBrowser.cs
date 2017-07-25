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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using Newtonsoft.Json;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.UIItems.WPFUIItems;
using TestStack.White.WindowsAPI;
using WhiteSeleniumServer;
using Button = TestStack.White.UIItems.Button;
using Label = TestStack.White.UIItems.Label;
using Panel = TestStack.White.UIItems.Panel;
using TextBox = TestStack.White.UIItems.TextBox;
using Window = TestStack.White.UIItems.WindowItems.Window;

namespace SeleniumWhiteLibrary
{
    public class AutomatedWebBrowser
    {
        private const int WaitForExitTimeout = 60000;
        
        //TODO: Need to remove this but currently we have no way to wait for the window to be maxmized.
        private const int WaitForMaxmized = 2500;

        #region Variables

        private Dictionary<string, IUIItem> _cachedElements = new Dictionary<string, IUIItem>(); 
        private IUIItem _mainWindow = null;
        private Guid _sessionId;
        private const String ChangeResolution = "ChangeResolution";
        private const String IsProcessRunning = "IsProcessRunning";

        #endregion

        #region Methods
        public Dictionary<string, object> AddCookie(Dictionary<string, object> cookie)
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> Close()
        {
            var responseObject = new Dictionary<string, object>();
            responseObject["sessionId"] = this._sessionId.ToString();
            responseObject["status"] = JsonWire.ResponseCode.Sucess;
            responseObject["value"] = "Close Called";

            return responseObject;
        }

        public Dictionary<string, object> CloseWindow()
        {
            var apiName = "CloseWindow";
            var responseObject = new Dictionary<string, object>();
            responseObject["sessionId"] = this._sessionId.ToString();
            responseObject["status"] = JsonWire.ResponseCode.Sucess;
            responseObject["value"] = "Window Closed";

            LogData(apiName, "Closing Window...");
            
            try
            {
                LogData(apiName, "WARNING! WINDOW CLOSED IS SPINNING CLOSE CALL ONTO A SEPARATE THREAD FOR TESTING PURPOSES! IF STATE ISSUES WITH THIS ARE ENCOUNTERED FILE A BUG ON THE GITHUB PROJECT");
                var window = (Window) _mainWindow;

                //Start window close on separate thread due to winform issue where close is hanging for 2-5 minutes.
                new TaskFactory().StartNew(window.Close);
                
                LogData(apiName, "Window Closed");
                
                //Clear the cache after the window is closed.
                _cachedElements.Clear();

                LogData(apiName, "Cache Cleared");
            }
            catch (InvalidCastException)
            {
                return GenerateResponse(JsonWire.ResponseCode.UnknownError,
                    "Cannot cast the current root object to a window type, so the object cannot be closed.");
            }

            return responseObject;
        }

        public bool CheckSession(string sessionIdstring)
        {
            return sessionIdstring == this._sessionId.ToString();
        }

        public Dictionary<string, object> Click(string internalId)
        {
            IUIItem cachedObject;

            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {
                ListItem listItem = null;

                try
                {
                    listItem = (ListItem) cachedObject;
                    Console.WriteLine("Object cast to listitem");
                }
                catch (InvalidCastException)
                {

                }


                TextBox editBox = null;
                try
                {
                    editBox = (TextBox) cachedObject;

                }
                catch (InvalidCastException) { }

                if (listItem != null)
                {
                    listItem.Select();
                    listItem.KeyIn(KeyboardInput.SpecialKeys.RETURN);
                }
                else if (editBox != null)
                {
                    editBox.Click();
                }
                else
                {
                    cachedObject.Click();
                }
                
                return GenerateSuccessResponse("Element was clicked successfully.");
            }

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }

        public string CreateNewSession()
        {
            _mainWindow = Desktop.Instance.Windows().ToArray()[0];
            return CreateSessionGuidClearCache();
        }

        public String CreateSessionGuidClearCache()
        {
            this._sessionId = Guid.NewGuid();

            //Clear the cached elements when a new session comes in. This restricts a server from only running against one endpoint at at a time.            
            _cachedElements.Clear();

            return this._sessionId.ToString();
        }

        public string CreateNewSession(String windowHandle)
        {
            var windows = Desktop.Instance.Windows();
            windows.Reverse();

            //Find the main window
            foreach (var window in windows)
            {
                if (window.Title.StartsWith(windowHandle))
                {
                    _mainWindow = window;                    
                    window.Focus();
                    break;
                }
            }

            //If no window is found, return null.
            return _mainWindow == null ? null : CreateSessionGuidClearCache();
        }


        public Dictionary<string, object> DescribeSession()
        {
            var sessionCapabilities = new Dictionary<string, object>();
            sessionCapabilities["browserAttachTimeout"] = 0;
            sessionCapabilities["browserName"] = "white";
            sessionCapabilities["cssSelectorsEnabled"] = true;
            sessionCapabilities["elementScrollBehavior"] = 0;
            sessionCapabilities["handlesAlerts"] = false;
            sessionCapabilities["javascriptEnabled"] = true;
            sessionCapabilities["nativeEvents"] = false;
            sessionCapabilities["platform"] = "WINDOWS";
            sessionCapabilities["takesScreenshot"] = true;
            sessionCapabilities["unexpectedAlertBehaviour"] = "ignore";
            sessionCapabilities["version"] = "1.0";

            var responseObject = new Dictionary<string, object>();
            responseObject["sessionId"] = this._sessionId.ToString();
            responseObject["status"] = JsonWire.ResponseCode.Sucess;
            responseObject["value"] = sessionCapabilities;

            return responseObject;
        }

        public Dictionary<string, object> ElementClear(string internalId)
        {

            IUIItem cachedObject;
            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {
                try
                {
                    ((TextBox)cachedObject).Text = String.Empty;
                    GenerateSuccessResponse("Cleared Text Box.");
                }
                catch (InvalidCastException) { }

                return GenerateNotImplementedResponse();
            }

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }

        public Dictionary<string, object> EndSession()
        {
            CleanSession();
            var responseObject = new Dictionary<string, object>();
            responseObject["sessionId"] = this._sessionId.ToString();
            responseObject["status"] = JsonWire.ResponseCode.Sucess;
            responseObject["value"] = null;

            return responseObject;
        }

        public Dictionary<string, object> Execute(string script, string args)
        {
            var exitCode = 0;

            //Get the Application Names
            var applicaiton = script.Split(' ').First();

            //Get the substring which should be the length of the word plus one.
            var arguments = script.Substring(applicaiton.Length + 1);

            //If someone passes in the Change Resolution argument, then execute the code for that.
            if (applicaiton.Equals(ChangeResolution))
            {
                var resolutionArguments = arguments.Split(' ');
                var width = Convert.ToInt32(resolutionArguments[0]);
                var height = Convert.ToInt32(resolutionArguments[1]);
                ScreenResolution.ChangeDisplaySettings(width, height);
            }
            
            else if (applicaiton.Equals(IsProcessRunning))
            {
                var processes = Process.GetProcessesByName(arguments);
                if (processes.Count().Equals(1))
                {
                    return GenerateSuccessResponse(String.Format("Process {0} Running", arguments));
                }
                else
                {
                    return GenerateResponse(JsonWire.ResponseCode.NoSuchWindow,
                        String.Format("Process {0} was not running", arguments));
                }
            }

            //Otherwise we are running an aribtrary command.
            else
            {
                var processStartInfo = new ProcessStartInfo(applicaiton, arguments);
                var process = Process.Start(processStartInfo);

                //Wait for exit, and if timeout is reached kill process. Otherwise an exception will be thrown parsing out the exit code.
                var waitForExit = process.WaitForExit(WaitForExitTimeout);

                if (!waitForExit)
                {
                    process.Kill();
                }

                exitCode = process.ExitCode;
            }

            return GenerateSuccessResponse(exitCode);
        }
        

        public Dictionary<string, object> FindElement(string strategy, string value, string parentElementId)
        {
            IUIItem item = null;

            try
            {

                switch (strategy)
                {
                    case "class name":
                        item = _mainWindow.Get(SearchCriteria.ByClassName(value));
                        break;
                    case "css selector":
                        item = GetElementByCssSelector(value);
                        break;
                    case "id":
                        item = _mainWindow.Get(SearchCriteria.ByAutomationId(value));
                        break;
                    case "text":
                        item = _mainWindow.Get(SearchCriteria.ByText(value));
                        break;
                    case "tag name":
                        item = GetElementByTagName(value);
                        break;
                    case "name":
                        item = _mainWindow.Get(SearchCriteria.ByNativeProperty(AutomationElement.NameProperty, value));
                        break;
                    default:
                        return GenerateNotImplementedResponse();
                }

            }
            catch (AutomationException)
            {
                //Exception thrown if the element is not found.
                //TODO: Not triggering the correct exception on the other end (NoSuchElement exception, instead it's just a webdriver exception...)
                return GenerateResponse(JsonWire.ResponseCode.NoSuchElement, "Element was not found on screen.");
            }

            //If the element is not found, return the not found response.
            if (item == null)
            {
                return GenerateResponse(JsonWire.ResponseCode.NoSuchElement, "Element was not found on screen.");
            }

            return GenerateResponseFoundElement(item);            
        }
        public Dictionary<string, object> FindElements(string strategy, string value, string parentElementId)
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> GetAttribute(string internalId, string name)
        {
            String attributeValue = String.Empty;

            return GenerateSuccessResponse(attributeValue);
        }


        public Dictionary<string, object> GetCurrentWindowHandle()
        {            
            var responseObject = new Dictionary<string, object>();
            responseObject["sessionId"] = this._sessionId.ToString();
            responseObject["status"] = JsonWire.ResponseCode.Sucess;
            responseObject["value"] = ((Window)_mainWindow).Title;

            return responseObject;
        }

        public Dictionary<string, object> GetAllWindowHandles()
        {
            const string apiName = "GetAllWindowHandles";
            var windowHandles = new List<string>();
            var windows = Desktop.Instance.Windows();

            LogData(apiName, "Enumerating Windows");

            foreach (var title in windows.Select(window => window.Title))
            {
                LogData(apiName, String.Format("Window Title {0} added", title));
                windowHandles.Add(title);
            }

            var responseObject = new Dictionary<string, object>();
            responseObject["sessionId"] = this._sessionId.ToString();
            responseObject["status"] = JsonWire.ResponseCode.Sucess;
            responseObject["value"] = windowHandles.ToArray();

            return responseObject;
        }

        public Dictionary<string, object> GetCookies()
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> GetCssProperty(string internalId, string propertyName)
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> GetLocation(string internalId)
        {
            IUIItem cachedObject;

            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {
                var responseObject = new Dictionary<string, object>();
                responseObject["sessionId"] = this._sessionId.ToString();
                responseObject["status"] = JsonWire.ResponseCode.Sucess;

                var locaitonDictionary = new Dictionary<string, object>();
                locaitonDictionary["x"] = cachedObject.Location.X;
                locaitonDictionary["y"] = cachedObject.Location.Y;
                responseObject["value"] = locaitonDictionary;
                
                return responseObject;
            }

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }

        public Dictionary<string, object> GetScreenshot()
        {            
            //Get screenshot of the entire desktop.
            var screenshot = new ScreenCapture().CaptureScreenShot();
            
            //Create memory stream and save image to memory stream as a png.
            var memoryStream = new MemoryStream();
            screenshot.Save(memoryStream, ImageFormat.Png);
            
            //Return image as base 64 encoded string png.
            var responseObject = new Dictionary<string, object>();
            responseObject["sessionId"] = _sessionId.ToString();
            responseObject["status"] = JsonWire.ResponseCode.Sucess;
            responseObject["value"] = Convert.ToBase64String(memoryStream.ToArray());

            return responseObject;
        }

        public Dictionary<string, object> GetSize(string internalId)
        {
            IUIItem cachedObject;

            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {
                var responseObject = new Dictionary<string, object>();
                responseObject["sessionId"] = this._sessionId.ToString();
                responseObject["status"] = JsonWire.ResponseCode.Sucess;
                
                var sizeDictionary = new Dictionary<string, object>();
                sizeDictionary["width"] = cachedObject.Bounds.Right - cachedObject.Bounds.Left;
                sizeDictionary["height"] = cachedObject.Bounds.Bottom - cachedObject.Bounds.Top;
                
                responseObject["value"] = sizeDictionary;

                return responseObject;
            }

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }

        public Dictionary<string, object> GetSource()
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> GetTagName(string internalId)
        {
            IUIItem cachedObject;
            _cachedElements.TryGetValue(internalId, out cachedObject);

            String tagName = String.Empty;

//            Type type = cachedObject.GetType();
//
//            //TODO: we're not returning the control type itself, also we're parsing instead of returning the type as it comes back as a proxy type. Should we just leave it as the proxy type?
//            if (type.Name.ToLower().Contains("combobox"))
//            {
//                //ControlType.ComboBox;
//                tagName = "select";
//            }

            return GenerateSuccessResponse(tagName);
        }

        public Dictionary<string, object> GetText(string internalId)
        {
            IUIItem cachedObject;

            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {

                string returnValue = String.Empty;

                //Try unboxing the object, if we can't unbox it then we don't have an item of that type. This will be simpler then trying to switch on every possible control type.

                try
                {
                    returnValue = ((Button)cachedObject).Text;
                    GenerateSuccessResponse(returnValue);
                }
                catch (InvalidCastException) { }

                try
                {
                    returnValue = ((Label)cachedObject).Text;
                    GenerateSuccessResponse(returnValue);
                }
                catch (InvalidCastException) { }

                try
                {
                    returnValue = ((TextBox)cachedObject).Text;
                    GenerateSuccessResponse(returnValue);
                }
                catch (InvalidCastException) { }

                
                try
                {
                    returnValue = ((Panel)cachedObject).Text;
                    GenerateSuccessResponse(returnValue);
                }
                catch (InvalidCastException) { }

                //If we cannot unbox the element to the correct type, simply return an empty string.
                return GenerateSuccessResponse(returnValue);
            }

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }

        public Dictionary<string, object> GetTitle()
        {
            //TODO: Need to get the title from where?
            return GenerateSuccessResponse("Fake Title");
        }

        public Dictionary<string, object> GetUrl()
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> GoForward()
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> GoBack()
        {
            return GenerateNotImplementedResponse();
        }
               
        public Dictionary<string, object> IsDisplayed(string internalId)
        {
            IUIItem cachedObject;

            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {
                return GenerateSuccessResponse(cachedObject.Visible);
            }

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }

        public Dictionary<string, object> IsEnabled(string internalId)
        {
            IUIItem cachedObject;

            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {
                return GenerateSuccessResponse(cachedObject.Enabled);
            }

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }

        public Dictionary<string, object> IsSelected(string internalId)
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> Navigate(string uri)
        {
            return GenerateNotImplementedResponse();
            //            if (uri != null)
            //            {
            //                //TODO: This can be used to launch an application. OR switch to a window that is opened.
            //            }
            //            
            //            var result = new Dictionary<string, object>();
            //            result["sessionId"] = this._sessionId.ToString();
            //            result["status"] = JsonWire.ResponseCode.Sucess;
            //            result["value"] = null;
            //            return result;
        }

        public Dictionary<string, object> Refresh()
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> Submit(string internalId)
        {
            return GenerateNotImplementedResponse();
        }

        public Dictionary<string, object> SwitchToFrame(string frameIdentifier)
        {
            return GenerateNotImplementedResponse();
        }

        private SwitchWindowModel GetWindowSwitchModel(Window window)
        {
            var windowModel = new SwitchWindowModel
            {
                ChildWindows = new List<SwitchWindowModel>(),
                Title = window.Title,
                Window = window,
            };

            foreach (var childWindow in window.ModalWindows())
            {
                windowModel.ChildWindows.Add(new SwitchWindowModel
                {
                    Title = childWindow.Title,
                    Window = childWindow
                });
            }

            return windowModel;
        }

        public Dictionary<string, object> SwitchToWindow(string windowTitle)
        {
            var apiName = "SwitchToWindow";
            var windows = Desktop.Instance.Windows();

            LogData(apiName, "Getting Window Names / Handles Threaded");
            //Threading getting the windows since we were running into timeout issues with extra IE windows.
            var taskList = windows.Select(window => new TaskFactory().StartNew(() => GetWindowSwitchModel(window))).ToList();

            //Wait For tasks to complete
            Task.WaitAll(taskList.ToArray());
            LogData(apiName, "Getting Window Data Complete");

            var resultList = taskList.Select(task => task.Result).ToList();

            foreach (var result in resultList)
            {
                LogData(apiName, String.Format("Window {0}", result.Title));
                //Check for main window
                if (result.Title.Contains(windowTitle))
                {
                    LogData(apiName, String.Format("Found Matching Window"));
                    _cachedElements.Clear();
                    _mainWindow = result.Window;

                    if (!result.Window.DisplayState.Equals(DisplayState.Maximized))
                    {
                        LogData(apiName, String.Format("Maxmizing Window"));
                        new TaskFactory().StartNew(() => result.Window.Focus(DisplayState.Maximized));
                        Thread.Sleep(WaitForMaxmized);
                        LogData(apiName, String.Format("Completed Maximizing Window"));
                    }

                    return GenerateSuccessResponse("Window successfully switch to");
                }

                //Check first level of child windows
                foreach (var childWindow in result.ChildWindows.Where(childWindow => childWindow.Title.Contains(windowTitle)))
                {
                    _cachedElements.Clear();
                    _mainWindow = childWindow.Window;

                    if (!childWindow.Window.DisplayState.Equals(DisplayState.Maximized))
                    {
                        childWindow.Window.Focus(DisplayState.Maximized);
                        Thread.Sleep(WaitForMaxmized);
                    }

                    return GenerateSuccessResponse("Window successfully switch to");
                }
            }

            return GenerateResponse(JsonWire.ResponseCode.NoSuchElement, "Window passed in does not exist!");
        }

        public Dictionary<string, object> Type(string internalId, string keys)
        {
            IUIItem cachedObject;

            if (_cachedElements.TryGetValue(internalId, out cachedObject))
            {
                TextBox textBox = null;
                try
                {
                    textBox = (TextBox) cachedObject;
                }
                catch(InvalidCastException) {}

                if (textBox != null)
                {
                    textBox.Text = keys;
                }
                else
                {
                    cachedObject.Click();
                    cachedObject.Enter(keys);
                }
                
                return GenerateSuccessResponse("Element was clicked successfully.");
            }
            

            return GenerateResponse(JsonWire.ResponseCode.StaleElementReference, "Element did not exist in the cache.");
        }
        #endregion

        #region Helpers
        private void CleanSession()
        {
            _cachedElements.Clear();
            _mainWindow = null;
            _sessionId = Guid.Empty;
        }
        
        /// <summary>
        /// Returns the first element of the type, null if an invalid tag name or element does not exist.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        private IUIItem GetElementByTagName(String tagName)
        {
            IUIItem uiItem = null;

            //If we are searching for tag name of body, special case as the base framework uses that as a search each time, which is not valid when we switch from web to native.
            if (tagName.ToLower().Equals("body"))
            {
                uiItem = _mainWindow;
            }

            else
            {
                ControlType controlType = null;
                switch (tagName.ToLower())
                {
                    case "button":
                        controlType = ControlType.Button;
                        break;
                    default:
                        break;
                }

                if (controlType != null)
                {
                    uiItem = _mainWindow.Get(SearchCriteria.ByControlType(controlType));
                }
            }

            return uiItem;
        }

        /// <summary>
        /// Returns the first element of the type, null if an invalid tag name or element does not exist.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        private IUIItem GetElementByCssSelector(String cssSelector)
        {
            IUIItem uiItem = null;

            //If we are searching for css selector of body, special case as the base framework uses that as a search each time, which is not valid when we switch from web to native.
            if (cssSelector.ToLower().Equals("body"))
            {
                uiItem = _mainWindow;
            }

            else
            {
                //TODO: Simplistic implementation searching from the root.
                //Trim the first . off the string
                var cssSelectorName = cssSelector.TrimStart('.');
                uiItem = _mainWindow.Get(SearchCriteria.ByClassName(cssSelectorName));
            }

            return uiItem;
        }

        private Dictionary<string, object> GenerateResponse(JsonWire.ResponseCode responseCode, Object responseObject)
        {
            var result = new Dictionary<string, object>();
            result["status"] = responseCode;
            result["sessionId"] = this._sessionId.ToString();
            result["value"] = responseObject;

            return result;
        }

        private Dictionary<string, object> GenerateResponseFoundElement(IUIItem element)
        {
            var elementId = Guid.NewGuid().ToString();
            _cachedElements.Add(elementId, element);            

            var elementDictionary = new Dictionary<string, object>();
            elementDictionary["ELEMENT"] = elementId;

            var result = new Dictionary<string, object>();
            result["status"] = JsonWire.ResponseCode.Sucess;
            result["sessionId"] = this._sessionId.ToString();
            result["value"] = elementDictionary;          
            return result;
        }                       
        
        private Dictionary<string, object> GenerateSuccessResponse(Object responseObject)
        {
            return GenerateResponse(JsonWire.ResponseCode.Sucess, responseObject);
        }

        private Dictionary<string, object> GenerateNotImplementedResponse()
        {
            var result = new Dictionary<string, object>();
            result["status"] = 501;
            result["sessionId"] = this._sessionId.ToString();
            result["value"] = "Error! Not Implemented!";
            return result;
        }

        private void LogData(String api, String message)
        {
            Console.WriteLine("[{0:hh:mm:ss}] {1} - {2}", DateTime.Now, api, message);
        }

        #endregion
    }
}
