using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Anbarban.Data
{
    public static class AccessConfig
    {
        public static string GetDatabasePath()
        {
            var configured = ConfigurationManager.AppSettings["DatabasePath"] ?? @"Data\Inventory.accdb";
            if (Path.IsPathRooted(configured))
                return configured;

            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            return Path.Combine(baseDir, configured);
        }

        public static string BuildConnectionString(string dbPath)
        {
            // ACE 12.0 — نسخه v3: Redistributable 64-bit (هم‌تراز PlatformTarget x64)
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }
    }
}
