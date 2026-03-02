using TechTalk.SpecFlow;
using OpenQA.Selenium;
using Parabank_Automation.Drivers;
using Parabank_Automation.Pages;
using NUnit.Framework;

namespace Parabank_Automation.StepDefinitions
{
    [Binding]
    public class LoginSteps
    {
        private readonly IWebDriver _driver;
        private readonly ScenarioContext _scenarioContext;

        public LoginSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _driver = Drivers.WebDriverManager.Instance;
        }

        [When("I login with registered credentials")]
        public void WhenILoginWithRegisteredCredentials()
        {
            var username = _scenarioContext.ContainsKey("RegisteredUsername") ? _scenarioContext["RegisteredUsername"] as string : null;
            var password = _scenarioContext.ContainsKey("RegisteredPassword") ? _scenarioContext["RegisteredPassword"] as string : null;

            if (string.IsNullOrWhiteSpace(username))
                username = Parabank_Automation.Config.TestSettings.DefaultUsername;
            if (string.IsNullOrWhiteSpace(password))
                password = Parabank_Automation.Config.TestSettings.DefaultPassword;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("No credentials available. Provide registered credentials from the scenario or configure Config/credentials.json with DefaultUsername and DefaultPassword.");
            }

            // if already logged in, perform logout first
            try
            {
                var logout = _driver.FindElements(OpenQA.Selenium.By.LinkText("Log Out")).FirstOrDefault();
                if (logout != null && logout.Displayed)
                {
                    logout.Click();
                    WaitForCondition(() => _driver.Url.Contains("index.htm") || _driver.FindElements(OpenQA.Selenium.By.Name("username")).Any(), 5000);
                }
            }
            catch { }

            // ensure we're on the home page with the login form
            if (!_driver.FindElements(OpenQA.Selenium.By.Name("username")).Any())
            {
                _driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
                WaitForCondition(() => _driver.FindElements(OpenQA.Selenium.By.Name("username")).Any(), 5000);
            }

            var loginPage = new LoginPage(_driver);
            loginPage.Login(username, password);

            var succeeded = WaitForCondition(() => _driver.Url.Contains("overview.htm") || _driver.PageSource.Contains("Log Out") || _driver.PageSource.Contains("Welcome"), 10000);
            Assert.IsTrue(succeeded, "Expected login to succeed and user to be signed in.");
        }

        [Then("the user is logged in")]
        public void ThenTheUserIsLoggedIn()
        {
            var loggedIn = _driver.PageSource.Contains("Log Out") || _driver.Url.Contains("overview.htm") || _driver.PageSource.Contains("Welcome");
            Assert.IsTrue(loggedIn, "Expected user to be logged in after performing login.");
        }

        private bool WaitForCondition(System.Func<bool> condition, int timeoutMs)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                try
                {
                    if (condition()) return true;
                }
                catch
                {
                }
                System.Threading.Thread.Sleep(250);
            }
            return false;
        }
    }
}
