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
            // ACE 12.0 — روی Win7 معمولاً ACE 2010 Redistributable (x86) لازم است
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }
    }
}
