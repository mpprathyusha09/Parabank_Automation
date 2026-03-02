using NUnit.Framework;
using Parabank_Automation.Drivers;

namespace Parabank_Automation.Tests
{
    public class SmokeTests
    {
        [SetUp]
        public void SetUp()
        {
            Drivers.WebDriverManager.InitializeDriver();
        }

        [TearDown]
        public void TearDown()
        {
            Drivers.WebDriverManager.QuitDriver();
        }

        [Test]
        public void ApplicationHomePageLoads()
        {
            var driver = Drivers.WebDriverManager.Instance;
            var baseUrl = Config.TestSettings.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseUrl is not configured. Set Config/urls.json or Config/testsettings.json with BaseUrl.");

            driver.Navigate().GoToUrl(baseUrl);
            Assert.IsTrue(driver.PageSource.ToLower().Contains("customer login") || driver.Url.Contains("index.htm"));
        }
    }
}
