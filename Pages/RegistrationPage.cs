using OpenQA.Selenium;
using Parabank_Automation.Models;

namespace Parabank_Automation.Pages
{
    public class RegistrationPage
    {
        private readonly IWebDriver _driver;

        public RegistrationPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement FirstName => _driver.FindElement(By.Id("customer.firstName"));
        public IWebElement LastName => _driver.FindElement(By.Id("customer.lastName"));
        public IWebElement Address => _driver.FindElement(By.Id("customer.address.street"));
        public IWebElement City => _driver.FindElement(By.Id("customer.address.city"));
        public IWebElement State => _driver.FindElement(By.Id("customer.address.state"));
        public IWebElement ZipCode => _driver.FindElement(By.Id("customer.address.zipCode"));
        public IWebElement Phone => _driver.FindElement(By.Id("customer.phoneNumber"));
        public IWebElement SSN => _driver.FindElement(By.Id("customer.ssn"));
        public IWebElement Username => _driver.FindElement(By.Id("customer.username"));
        public IWebElement Password => _driver.FindElement(By.Id("customer.password"));
        public IWebElement ConfirmPassword => _driver.FindElement(By.Id("repeatedPassword"));
        public IWebElement RegisterButton => _driver.FindElement(By.CssSelector("input.button[value='Register']"));

        public void Populate(RegistrationModel model)
        {
            if (model == null) return;

            if (model.FirstName != null) { FirstName.Clear(); FirstName.SendKeys(model.FirstName); }
            if (model.LastName != null) { LastName.Clear(); LastName.SendKeys(model.LastName); }
            if (model.Address != null) { Address.Clear(); Address.SendKeys(model.Address); }
            if (model.City != null) { City.Clear(); City.SendKeys(model.City); }
            if (model.State != null) { State.Clear(); State.SendKeys(model.State); }
            if (model.ZipCode != null) { ZipCode.Clear(); ZipCode.SendKeys(model.ZipCode); }
            if (model.Phone != null) { Phone.Clear(); Phone.SendKeys(model.Phone); }
            if (model.SSN != null) { SSN.Clear(); SSN.SendKeys(model.SSN); }
            if (model.Username != null) { Username.Clear(); Username.SendKeys(model.Username); }
            if (model.Password != null) { Password.Clear(); Password.SendKeys(model.Password); }
            if (model.ConfirmPassword != null) { ConfirmPassword.Clear(); ConfirmPassword.SendKeys(model.ConfirmPassword); }
        }

        public void Submit() => RegisterButton.Click();
    }
}
