using TechTalk.SpecFlow;
using Parabank_Automation.Drivers;
using OpenQA.Selenium;
using System;
using System.IO;

namespace Parabank_Automation.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;

        public TestHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Parabank_Automation.Drivers.WebDriverManager.InitializeDriver();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                // if the scenario failed, capture diagnostics
                if (_scenarioContext.TestError != null)
                {
                    var driver = Parabank_Automation.Drivers.WebDriverManager.Instance;
                    if (driver != null)
                    {
                        var resultsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
                        Directory.CreateDirectory(resultsDir);

                        var safeTitle = MakeSafeFileName(_scenarioContext.ScenarioInfo.Title);
                        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff");

                        // capture screenshot if supported
                        try
                        {
                            if (driver is ITakesScreenshot shooter)
                            {
                                var shot = shooter.GetScreenshot();
                                var file = Path.Combine(resultsDir, $"{safeTitle}_{timestamp}.png");
                                System.IO.File.WriteAllBytes(file, shot.AsByteArray);
                            }
                        }
                        catch { }

                        // save page source
                        try
                        {
                            var page = driver.PageSource;
                            var file = Path.Combine(resultsDir, $"{safeTitle}_{timestamp}.html");
                            File.WriteAllText(file, page ?? string.Empty);
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                Parabank_Automation.Drivers.WebDriverManager.QuitDriver();
            }
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name.Replace(' ', '_');
        }
    }
}
