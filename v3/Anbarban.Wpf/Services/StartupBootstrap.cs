using System;
using System.Threading.Tasks;
using System.Windows;
using Anbarban.Data;
using Anbarban.Views;
using Anbarban.Views.Dialogs;

namespace Anbarban.Services
{
    public static class StartupBootstrap
    {
        public static bool Run()
        {
            while (true)
            {
                if (!AceProviderService.IsAceAvailable())
                {
                    var action = UiDialog.ShowAceMissing();
                    if (!HandleAceAction(action))
                        return false;
                    continue;
                }

                try
                {
                    var path = AccessConfig.GetDatabasePath();
                    if (!DatabaseBootstrap.EnsureDatabase(path))
                    {
                        UiDialog.Error("انجام نشد", AccessConnectionFactory.BuildHelpMessage(path));
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (IsAceRelated(ex))
                    {
                        AceProviderService.ClearCache();
                        UiDialog.ShowAceMissing();
                        continue;
                    }
                    UiDialog.Error("انجام نشد", ex.Message);
                    return false;
                }
            }
        }

        private static bool HandleAceAction(AnbarMessageResult action)
        {
            switch (action)
            {
                case AnbarMessageResult.Retry:
                    AceProviderService.ClearCache();
                    return true;

                case AnbarMessageResult.DownloadAce:
                    AceInstaller.OpenDownloadPage();
                    return true;

                case AnbarMessageResult.OpenFolder:
                    AceInstaller.OpenRedistFolder();
                    return true;

                case AnbarMessageResult.InstallAce:
                    return RunAceInstall();

                case AnbarMessageResult.Cancel:
                case AnbarMessageResult.None:
                default:
                    return false;
            }
        }

        private static bool RunAceInstall()
        {
            if (AceInstaller.HasBundledInstaller)
                return LaunchInstallerAndExit();

            AnbarbanDialog.Info("در حال آماده‌سازی فایل نصب موتور پایگاه… لطفاً چند لحظه صبر کنید.");
            try
            {
                var (ok, log) = Task.Run(() => AceInstaller.TryDownloadInstallerAsync()).GetAwaiter().GetResult();
                if (!ok)
                {
                    UiDialog.Error("انجام نشد",
                        "دانلود خودکار ممکن نشد." + Environment.NewLine + Environment.NewLine + log +
                        Environment.NewLine + Environment.NewLine +
                        "از دکمه «دانلود از مایکروسافت» استفاده کنید یا فایل AccessDatabaseEngine_X64.exe را در پوشه redist قرار دهید.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                UiDialog.Error("انجام نشد", ex.Message);
                return true;
            }

            if (AceInstaller.HasBundledInstaller)
                return LaunchInstallerAndExit();

            UiDialog.Error("انجام نشد", "فایل نصب در پوشه redist پیدا نشد.");
            return true;
        }

        private static bool LaunchInstallerAndExit()
        {
            var (started, message) = AceInstaller.TryRunBundledInstall();
            if (started)
                AnbarbanDialog.Info(message + Environment.NewLine + Environment.NewLine + "پس از پایان نصب، انباربان را دوباره اجرا کنید.");
            else
                UiDialog.Error("انجام نشد", message);
            if (started)
                Application.Current.Shutdown();
            return !started;
        }

        private static bool IsAceRelated(Exception ex)
        {
            var text = ex.ToString();
            return text.Contains("ACE", StringComparison.OrdinalIgnoreCase)
                   || text.Contains("OLEDB", StringComparison.OrdinalIgnoreCase)
                   || text.Contains("ACE_NOT_INSTALLED", StringComparison.OrdinalIgnoreCase)
                   || text.Contains("provider is not registered", StringComparison.OrdinalIgnoreCase);
        }

        public static void ShowMainWindow()
        {
            var w = new MainWindow();
            Application.Current.MainWindow = w;
            w.Show();
        }
    }
}
