using System;
using System.IO;
using System.Text.Json;

namespace Parabank_Automation.Config
{
    // small POCOs for separate config files
    public class UrlsModel { public string BaseUrl { get; set; } }
    public class CredentialsModel { public string DefaultUsername { get; set; } public string DefaultPassword { get; set; } }

    public static class TestSettings
    {
        public static string BaseUrl { get; private set; } = string.Empty;
        public static string DefaultUsername { get; private set; } = string.Empty;
        public static string DefaultPassword { get; private set; } = string.Empty;

        static TestSettings()
        {
            try
            {
                // Prefer separate files: Config/urls.json and Config/credentials.json
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var urlsPath = Path.Combine(baseDir, "Config", "urls.json");
                var credsPath = Path.Combine(baseDir, "Config", "credentials.json");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (File.Exists(urlsPath))
                {
                    var ujson = File.ReadAllText(urlsPath);
                    var umodel = JsonSerializer.Deserialize<UrlsModel>(ujson, options);
                    if (umodel != null && !string.IsNullOrWhiteSpace(umodel.BaseUrl))
                        BaseUrl = umodel.BaseUrl;
                }

                if (File.Exists(credsPath))
                {
                    var cjson = File.ReadAllText(credsPath);
                    var cmodel = JsonSerializer.Deserialize<CredentialsModel>(cjson, options);
                    if (cmodel != null)
                    {
                        DefaultUsername = cmodel.DefaultUsername ?? string.Empty;
                        DefaultPassword = cmodel.DefaultPassword ?? string.Empty;
                    }
                }

                // fallback to legacy single file testsettings.json if values are missing
                var legacyPath = Path.Combine(baseDir, "Config", "testsettings.json");
                if (!File.Exists(legacyPath)) legacyPath = Path.Combine(baseDir, "testsettings.json");
                if (File.Exists(legacyPath))
                {
                    var legacy = File.ReadAllText(legacyPath);
                    try
                    {
                        using var doc = JsonDocument.Parse(legacy);
                        var root = doc.RootElement;
                        if (string.IsNullOrWhiteSpace(BaseUrl) && root.TryGetProperty("BaseUrl", out var bu)) BaseUrl = bu.GetString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(DefaultUsername) && root.TryGetProperty("DefaultUsername", out var du)) DefaultUsername = du.GetString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(DefaultPassword) && root.TryGetProperty("DefaultPassword", out var dp)) DefaultPassword = dp.GetString() ?? string.Empty;
                    }
                    catch { }
                }
            }
            catch
            {
                // ignore and keep defaults
            }
        }
    }
}
