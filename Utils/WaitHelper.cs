using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Parabank_Automation.Utils
{
    public static class WaitHelper
    {
        public static IWebElement WaitForElement(this IWebDriver driver, By by, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => d.FindElement(by));
        }
    }
}
