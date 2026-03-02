using TechTalk.SpecFlow;
using OpenQA.Selenium;
using Parabank_Automation.Drivers;
using Parabank_Automation.Pages;
using Parabank_Automation.Models;
using NUnit.Framework;

namespace Parabank_Automation.StepDefinitions
{
    [Binding]
    public class RegistrationSteps
    {
        private readonly IWebDriver _driver;
        private readonly ScenarioContext _scenarioContext;
        private RegistrationPage _registrationPage;

        public RegistrationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _driver = Parabank_Automation.Drivers.WebDriverManager.Instance;
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
                    // ignore transient exceptions while waiting
                }
                System.Threading.Thread.Sleep(250);
            }
            return false;
        }

        [When("I register with valid registration data")]
        public void WhenIRegisterWithValidRegistrationData()
        {
            _registrationPage = new RegistrationPage(_driver);

            var attempt = 0;
            const int maxAttempts = 5;
            RegistrationModel model = null;
            bool registered = false;

            while (attempt < maxAttempts && !registered)
            {
                attempt++;
                var defaultPassword = Parabank_Automation.Config.TestSettings.DefaultPassword;
                if (string.IsNullOrWhiteSpace(defaultPassword))
                    throw new InvalidOperationException("DefaultPassword is not configured. Set Config/credentials.json or Config/testsettings.json with DefaultPassword.");

                model = new RegistrationModel
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Address = "Address",
                    City = "City",
                    State = "State",
                    ZipCode = "00000",
                    Phone = "000-000-0000",
                    SSN = "000-00-0000",
                    Username = "user" + System.Guid.NewGuid().ToString("N").Substring(0, 8),
                    Password = defaultPassword,
                    ConfirmPassword = defaultPassword
                };

                _registrationPage.Populate(model);
                _registrationPage.Submit();

                // Wait for either success or validation indicating username collision or other errors
                var outcome = WaitForCondition(() =>
                    _driver.Url.Contains("overview.htm")
                    || _driver.PageSource.ToLower().Contains("your account was created")
                    || _driver.PageSource.ToLower().Contains("already exists")
                    || _driver.FindElements(OpenQA.Selenium.By.CssSelector("span.error")).Any(e => e.Displayed && !string.IsNullOrWhiteSpace(e.Text))
                , 5000);

                // If success detected, mark registered and store credentials
                if (outcome && ( _driver.Url.Contains("overview.htm") || _driver.PageSource.ToLower().Contains("your account was created") ))
                {
                    registered = true;
                    _scenarioContext["RegisteredUsername"] = model.Username;
                    _scenarioContext["RegisteredPassword"] = model.Password;
                    break;
                }

                // If page indicates username already exists, try again with a new username
                if (outcome && _driver.PageSource.ToLower().Contains("already exists"))
                {
                    // clear username and retry loop will create a new username
                    try
                    {
                        var usernameEl = _driver.FindElement(OpenQA.Selenium.By.Id("customer.username"));
                        usernameEl.Clear();
                    }
                    catch { }
                    continue;
                }

                // If validation errors other than username collision are present, stop and let the assertion handle it later
                break;
            }

            Assert.IsTrue(registered, $"Registration did not succeed after {maxAttempts} attempts. Last attempted username: {model?.Username}");
        }

        [When("I register with mismatched passwords")]
        public void WhenIRegisterWithMismatchedPasswords()
        {
            _registrationPage = new RegistrationPage(_driver);

            var defaultPassword = Parabank_Automation.Config.TestSettings.DefaultPassword;
            if (string.IsNullOrWhiteSpace(defaultPassword))
                throw new InvalidOperationException("DefaultPassword is not configured. Set Config/credentials.json or Config/testsettings.json with DefaultPassword.");

            var model = new RegistrationModel
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Username = "user" + System.DateTime.UtcNow.Ticks,
                Password = defaultPassword,
                ConfirmPassword = defaultPassword + "_mismatch"
            };

            _registrationPage.Populate(model);
            _registrationPage.Submit();
        }

        [When("I register without a username")]
        public void WhenIRegisterWithoutAUsername()
        {
            _registrationPage = new RegistrationPage(_driver);

            var defaultPassword = Parabank_Automation.Config.TestSettings.DefaultPassword;
            if (string.IsNullOrWhiteSpace(defaultPassword))
                throw new InvalidOperationException("DefaultPassword is not configured. Set Config/credentials.json or Config/testsettings.json with DefaultPassword.");

            var model = new RegistrationModel
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Password = defaultPassword,
                ConfirmPassword = defaultPassword,
                Username = null
            };

            _registrationPage.Populate(model);
            _registrationPage.Submit();
        }

        [Then("the registration is successful")]
        public void ThenTheRegistrationIsSuccessful()
        {
            // Wait for explicit success indicators rendered after registration:
            // - redirected to overview.htm
            // - presence of a visible "Log Out" link
            // - an <h1 class="title"> that starts with "Welcome"
            // - right panel paragraph containing "Your account was created"
            var success = WaitForCondition(() =>
            {
                try
                {
                    if (_driver.Url.Contains("overview.htm")) return true;

                    var logoutLink = _driver.FindElements(OpenQA.Selenium.By.LinkText("Log Out"));
                    if (logoutLink.Any(e => e.Displayed)) return true;

                    var headers = _driver.FindElements(OpenQA.Selenium.By.CssSelector("h1.title"));
                    if (headers.Any(h => h.Displayed && h.Text.Trim().StartsWith("Welcome"))) return true;

                    var rightPanelParagraphs = _driver.FindElements(OpenQA.Selenium.By.CssSelector("div#rightPanel p"));
                    if (rightPanelParagraphs.Any(p => p.Displayed && p.Text.ToLower().Contains("your account was created"))) return true;
                }
                catch
                {
                    // ignore transient DOM exceptions while waiting
                }
                return false;
            }, 10000);

            Assert.IsTrue(success, "Expected successful registration indicator (overview redirect, Log Out link, welcome header, or success message).");
        }

        [Then("the registration fails with a validation error")]
        public void ThenTheRegistrationFailsWithAValidationError()
        {
            var stayedOnRegister = _driver.Url.Contains("register.htm");
            var hasErrorText = _driver.PageSource.ToLower().Contains("error") || _driver.PageSource.ToLower().Contains("was not created") || _driver.PageSource.ToLower().Contains("passwords did not match");

            Assert.IsTrue(stayedOnRegister || hasErrorText, "Expected registration to fail and show validation error.");
        }
    }

}
