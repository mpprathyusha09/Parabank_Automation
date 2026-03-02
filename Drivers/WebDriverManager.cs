using OpenQA.Selenium;

namespace Parabank_Automation.Drivers
{
    public static class WebDriverManager
    {
        public static IWebDriver Instance { get; private set; }

        public static void InitializeDriver()
        {
            if (Instance == null)
            {
                Instance = WebDriverFactory.CreateChrome();
            }
        }

        public static void QuitDriver()
        {
            Instance?.Quit();
            Instance = null;
        }
    }
}
