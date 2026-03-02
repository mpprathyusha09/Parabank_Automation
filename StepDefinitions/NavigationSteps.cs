using TechTalk.SpecFlow;
using Parabank_Automation.Drivers;
using NUnit.Framework;

namespace Parabank_Automation.StepDefinitions
{
    [Binding]
    public class NavigationSteps
    {
        [Given("I open the application")]
        public void GivenIOpenTheApplication()
        {
            var driver = Drivers.WebDriverManager.Instance;
            var baseUrl = Config.TestSettings.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseUrl is not configured. Set Config/urls.json or Config/testsettings.json with BaseUrl.");

            driver.Navigate().GoToUrl(baseUrl);
        }

        [Given("I navigate to the registration page")]
        public void GivenINavigateToTheRegistrationPage()
        {
            var driver = Drivers.WebDriverManager.Instance;
            var registerLink = driver.FindElement(OpenQA.Selenium.By.LinkText("Register"));
            registerLink.Click();
        }

        [Then("the application home page is displayed")]
        public void ThenTheApplicationHomePageIsDisplayed()
        {
            var driver = Drivers.WebDriverManager.Instance;
            Assert.IsTrue(driver.PageSource.ToLower().Contains("customer login") || driver.Url.Contains("index.htm"));
        }
    }
}
