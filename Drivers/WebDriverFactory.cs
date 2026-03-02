using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Parabank_Automation.Drivers
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateChrome()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            // Add other options as needed (headless, disable-gpu, etc.)
            return new ChromeDriver(options);
        }
    }
}
