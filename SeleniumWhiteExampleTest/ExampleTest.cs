using NUnit.Framework;
using SeleniumWhiteAutomationLibrary;

namespace SeleniumWhiteExampleTest
{
    public class ExampleTest
    {
        [Test]
        public void TestMethod1()
        {
            var driver = SeleniumWhiteDriver.GenerateDriver();
            driver.SwitchTo().Window("Untitled - Notepad");
            driver.Quit();
        }
    }
}
