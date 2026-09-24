using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using Anbarban.Data;

namespace Anbarban.Services
{
    public static class AceProviderService
    {
        private static readonly string[] ProviderCandidates =
        {
            "Microsoft.ACE.OLEDB.16.0",
            "Microsoft.ACE.OLEDB.12.0",
            "Microsoft.ACE.OLEDB.15.0"
        };

        private static string? _cachedProvider;

        public static string? CachedProvider => _cachedProvider;

        public static IReadOnlyList<string> InstalledProviders()
        {
            var list = new List<string>();
            try
            {
                using var reader = OleDbEnumerator.GetRootEnumerator();
                while (reader.Read())
                {
                    var name = reader["SOURCES_NAME"]?.ToString();
                    if (!string.IsNullOrEmpty(name) && name.Contains("ACE"))
                        list.Add(name);
                }
            }
            catch { /* ignore */ }
            return list;
        }

        public static bool IsAceAvailable() => ResolveProvider() != null;

        public static string ResolveProvider()
        {
            if (!string.IsNullOrEmpty(_cachedProvider)) return _cachedProvider;

            var fromConfig = LoadSavedProvider();
            if (!string.IsNullOrEmpty(fromConfig) && ProviderWorks(fromConfig))
            {
                _cachedProvider = fromConfig;
                return _cachedProvider;
            }

            foreach (var p in ProviderCandidates)
            {
                if (ProviderWorks(p))
                {
                    _cachedProvider = p;
                    SaveProvider(p);
                    return p;
                }
            }

            var installed = InstalledProviders();
            foreach (var p in installed)
            {
                if (ProviderWorks(p))
                {
                    _cachedProvider = p;
                    SaveProvider(p);
                    return p;
                }
            }

            return null!;
        }

        public static void ClearCache() => _cachedProvider = null;

        private static bool ProviderWorks(string provider)
        {
            try
            {
                var temp = Path.Combine(Path.GetTempPath(), "anbarban_ace_probe.accdb");
                if (!File.Exists(temp) && !DatabaseBootstrap.TryCreateEmptyFile(temp, provider))
                    return false;
                using var conn = new OleDbConnection($"Provider={provider};Data Source={temp};");
                conn.Open();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string ConfigPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anbarban", "provider.txt");

        private static string? LoadSavedProvider()
        {
            try
            {
                var p = ConfigPath;
                return File.Exists(p) ? File.ReadAllText(p).Trim() : null;
            }
            catch { return null; }
        }

        private static void SaveProvider(string provider)
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(ConfigPath, provider);
            }
            catch { /* ignore */ }
        }

        public static string BuildConnectionString(string dbPath)
        {
            var provider = ResolveProvider();
            if (string.IsNullOrEmpty(provider))
                throw new InvalidOperationException("ACE_NOT_INSTALLED");
            return $"Provider={provider};Data Source={dbPath};Persist Security Info=False;";
        }
    }
}
