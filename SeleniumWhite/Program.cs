using SeleniumWhiteLibrary;
using WhiteSeleniumServer;

namespace SeleniumWhite
{
    class Program
    {
        static void Main(string[] args)
        {
            var BrowserState = new AutomatedWebBrowser();
            var server = new MiniHttpServer(SeleniumWhiteSettings.Default.Port);
            var handlers = new RequestHandlers(BrowserState);
            server.RegisterHandlers(handlers);
        }
    }
}
