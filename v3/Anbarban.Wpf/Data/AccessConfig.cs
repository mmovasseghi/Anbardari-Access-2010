using System.Configuration;
using System.IO;
using System.Reflection;
using Anbarban.Services;

namespace Anbarban.Data
{
    public static class AccessConfig
    {
        public static string GetDatabasePath()
        {
            var env = Environment.GetEnvironmentVariable("ANBARBAN_DATABASE_PATH");
            if (!string.IsNullOrWhiteSpace(env))
                return env.Trim();

            var configured = ConfigurationManager.AppSettings["DatabasePath"] ?? @"Data\Inventory.accdb";
            if (Path.IsPathRooted(configured))
                return configured;

            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            return Path.Combine(baseDir, configured);
        }

        public static string BuildConnectionString(string dbPath) =>
            AceProviderService.BuildConnectionString(dbPath);
    }
}
