using OpenQA.Selenium;

namespace Parabank_Automation.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // Example page object members
        public IWebElement UsernameField => _driver.FindElement(By.Name("username"));
        public IWebElement PasswordField => _driver.FindElement(By.Name("password"));
        public IWebElement LoginButton => _driver.FindElement(By.CssSelector("form[name='login'] input[type='submit'], input.button[value='Log In'], button[type='submit']"));

        public void Login(string username, string password)
        {
            UsernameField.Clear();
            UsernameField.SendKeys(username);
            PasswordField.Clear();
            PasswordField.SendKeys(password);
            LoginButton.Click();
        }
    }
}
