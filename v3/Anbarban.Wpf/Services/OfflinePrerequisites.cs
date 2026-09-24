using System;
using System.Diagnostics;
using System.IO;

namespace Anbarban.Services
{
    /// <summary>پیش‌نیازهای آفلاین — پوشه prerequisites کنار Anbarban.exe (بدون اینترنت).</summary>
    public static class OfflinePrerequisites
    {
        public static string AppBase => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>پوشهٔ پیش‌نیازها (محتوای ZIP جدا را اینجا کپی کنید).</summary>
        public static string PrerequisitesFolder => Path.Combine(AppBase, "prerequisites");

        public const string AceFileName = "AccessDatabaseEngine_X64.exe";
        public const string DotNetFileName = "ndp48-x86-x64-allos-enu.exe";

        public static string? ResolveAceInstallerPath()
        {
            var candidates = new[]
            {
                Path.Combine(PrerequisitesFolder, AceFileName),
                Path.Combine(AppBase, "redist", AceFileName)
            };
            foreach (var p in candidates)
            {
                if (File.Exists(p)) return p;
            }
            return null;
        }

        public static string? ResolveDotNetInstallerPath()
        {
            var p = Path.Combine(PrerequisitesFolder, DotNetFileName);
            return File.Exists(p) ? p : null;
        }

        public static bool HasAceInstaller => ResolveAceInstallerPath() != null;

        public static string OfflineHelpText =>
            "این سیستم به اینترنت وصل نیست — پیش‌نیازها را از Release گیت‌هاب (فایل جدا) یک‌بار دانلود کنید." + Environment.NewLine + Environment.NewLine +
            "۱) دو فایل ZIP را بگیرید: Anbarban-v3-Portable.zip + Anbarban-v3-Prerequisites.zip" + Environment.NewLine +
            "۲) هر دو را در یک پوشه Extract کنید (مثلاً D:\\Anbarban)" + Environment.NewLine +
            "۳) باید پوشه prerequisites کنار Anbarban.exe باشد" + Environment.NewLine +
            "۴) اول پیش‌نیازها را نصب کنید، بعد «شروع انباربان.bat»";

        public static (bool started, string message) TryRunAceInstall()
        {
            var path = ResolveAceInstallerPath();
            if (path == null)
            {
                return (false,
                    "فایل " + AceFileName + " پیدا نشد." + Environment.NewLine + Environment.NewLine +
                    "پوشه مورد انتظار:" + Environment.NewLine + PrerequisitesFolder);
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = "/passive",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
                return (true, "نصب موتور پایگاه (ACE) شروع شد." + Environment.NewLine +
                              "پس از پایان، انباربان را دوباره باز کنید.");
            }
            catch (Exception ex)
            {
                return (false, "اجرای نصب ممکن نشد: " + ex.Message);
            }
        }

        /// <summary>باز کردن Explorer روی پوشه prerequisites (یا ساخت خالی برای کپی فایل).</summary>
        public static void OpenPrerequisitesFolder()
        {
            if (!Directory.Exists(PrerequisitesFolder))
                Directory.CreateDirectory(PrerequisitesFolder);
            Process.Start("explorer.exe", PrerequisitesFolder);
        }

        /// <summary>نمایش فایل نصب در Explorer تا کاربر دوبارکلیک کند (آفلاین).</summary>
        public static void OpenAceInstallerInExplorer()
        {
            var path = ResolveAceInstallerPath();
            if (path != null)
            {
                Process.Start("explorer.exe", $"/select,\"{path}\"");
                return;
            }
            OpenPrerequisitesFolder();
        }

        public static void OpenDotNetInstallerInExplorer()
        {
            var path = ResolveDotNetInstallerPath();
            if (path != null)
            {
                Process.Start("explorer.exe", $"/select,\"{path}\"");
                return;
            }
            OpenPrerequisitesFolder();
        }
    }
}
