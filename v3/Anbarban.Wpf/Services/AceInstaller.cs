using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Anbarban.Services
{
    public static class AceInstaller
    {
        public static string RedistFolder =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "redist");

        public static string BundledInstallerPath =>
            Path.Combine(RedistFolder, "AccessDatabaseEngine_X64.exe");

        public static bool HasBundledInstaller => File.Exists(BundledInstallerPath);

        public static string InstallScriptPath =>
            Path.Combine(RedistFolder, "install-ace.ps1");

        /// <summary>نصب خاموش ACE 64-bit اگر فایل در پوشه redist باشد.</summary>
        public static (bool started, string message) TryRunBundledInstall()
        {
            if (!HasBundledInstaller)
                return (false, "فایل نصب در پوشه redist نیست. از دکمه «دانلود و نصب» استفاده کنید.");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = BundledInstallerPath,
                    Arguments = "/passive",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
                return (true, "نصب موتور پایگاه شروع شد. پس از پایان، انباربان را دوباره باز کنید.");
            }
            catch (Exception ex)
            {
                return (false, "اجرای نصب ممکن نشد: " + ex.Message);
            }
        }

        public static void OpenDownloadPage()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.microsoft.com/en-us/download/details.aspx?id=54920",
                UseShellExecute = true
            });
        }

        public static void OpenRedistFolder()
        {
            if (!Directory.Exists(RedistFolder))
                Directory.CreateDirectory(RedistFolder);
            Process.Start("explorer.exe", RedistFolder);
        }

        public static async Task<(bool ok, string log)> TryDownloadInstallerAsync()
        {
            var script = InstallScriptPath;
            if (!File.Exists(script))
                return (false, "اسکریپت install-ace.ps1 پیدا نشد.");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{script}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi)!;
                var outText = await p.StandardOutput.ReadToEndAsync();
                var err = await p.StandardError.ReadToEndAsync();
                p.WaitForExit(120000);
                if (p.ExitCode == 0 && File.Exists(BundledInstallerPath))
                    return (true, outText);
                return (false, outText + "\n" + err);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
