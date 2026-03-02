using System;
using System.IO;
using System.Text.Json;

namespace Parabank_Automation.Config
{
    public class UrlsModel { public string BaseUrl { get; set; } }
    public class CredentialsModel { public string DefaultUsername { get; set; } public string DefaultPassword { get; set; } }

    public static class TestSettings
    {
        public static string BaseUrl { get; private set; } = "https://parabank.parasoft.com/parabank/index.htm";
        public static string DefaultUsername { get; private set; } = "testuser";
        public static string DefaultPassword { get; private set; } = "SecureP@ss1";

        static TestSettings()
        {
            try
            {
                // Environment variable overrides (highest priority)
                var envBase = Environment.GetEnvironmentVariable("TEST_BASEURL");
                var envUser = Environment.GetEnvironmentVariable("TEST_USERNAME");
                var envPass = Environment.GetEnvironmentVariable("TEST_PASSWORD");
                if (!string.IsNullOrWhiteSpace(envBase)) BaseUrl = envBase;
                if (!string.IsNullOrWhiteSpace(envUser)) DefaultUsername = envUser;
                if (!string.IsNullOrWhiteSpace(envPass)) DefaultPassword = envPass;

                // Try to load from config files
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var urlsPath = Path.Combine(baseDir, "Config", "urls.json");
                var credsPath = Path.Combine(baseDir, "Config", "credentials.json");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (File.Exists(urlsPath))
                {
                    try
                    {
                        var ujson = File.ReadAllText(urlsPath);
                        var umodel = JsonSerializer.Deserialize<UrlsModel>(ujson, options);
                        if (umodel != null && !string.IsNullOrWhiteSpace(umodel.BaseUrl))
                            BaseUrl = umodel.BaseUrl;
                    }
                    catch { }
                }

                if (File.Exists(credsPath))
                {
                    try
                    {
                        var cjson = File.ReadAllText(credsPath);
                        var cmodel = JsonSerializer.Deserialize<CredentialsModel>(cjson, options);
                        if (cmodel != null)
                        {
                            if (!string.IsNullOrWhiteSpace(cmodel.DefaultUsername))
                                DefaultUsername = cmodel.DefaultUsername;
                            if (!string.IsNullOrWhiteSpace(cmodel.DefaultPassword))
                                DefaultPassword = cmodel.DefaultPassword;
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                // Keep defaults if anything fails
            }
        }
    }
}
