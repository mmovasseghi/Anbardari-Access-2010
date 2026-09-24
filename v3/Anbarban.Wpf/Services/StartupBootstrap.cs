using System;
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

                case AnbarMessageResult.OpenInstaller:
                    OfflinePrerequisites.OpenAceInstallerInExplorer();
                    return true;

                case AnbarMessageResult.OpenFolder:
                    OfflinePrerequisites.OpenPrerequisitesFolder();
                    return true;

                case AnbarMessageResult.InstallAce:
                    return RunAceInstallOffline();

                case AnbarMessageResult.DownloadAce:
                    OfflinePrerequisites.OpenAceInstallerInExplorer();
                    return true;

                case AnbarMessageResult.Cancel:
                case AnbarMessageResult.None:
                default:
                    return false;
            }
        }

        private static bool RunAceInstallOffline()
        {
            if (!OfflinePrerequisites.HasAceInstaller)
            {
                OfflinePrerequisites.OpenPrerequisitesFolder();
                AnbarbanDialog.Warn(
                    "فایل نصب ACE پیدا نشد." + Environment.NewLine + Environment.NewLine +
                    "از Release، ZIP «پیش‌نیازها» را گرفته و محتوا را در پوشه prerequisites کنار Anbarban.exe قرار دهید." + Environment.NewLine +
                    "سپس روی AccessDatabaseEngine_X64.exe دوبارکلیک کنید یا دوباره «شروع نصب ACE» را بزنید.",
                    null,
                    "پیش‌نیاز آفلاین");
                return true;
            }

            return LaunchInstallerAndExit();
        }

        private static bool LaunchInstallerAndExit()
        {
            var (started, message) = OfflinePrerequisites.TryRunAceInstall();
            if (started)
                AnbarbanDialog.Info(message + Environment.NewLine + Environment.NewLine + "پس از پایان نصب، انباربان را دوباره اجرا کنید.");
            else
            {
                OfflinePrerequisites.OpenAceInstallerInExplorer();
                UiDialog.Error("انجام نشد", message);
            }
            if (started)
                Application.Current.Shutdown();
            return !started;
        }

        private static bool IsAceRelated(Exception ex)
        {
            var text = ex.ToString();
            return text.IndexOf("ACE", StringComparison.OrdinalIgnoreCase) >= 0
                   || text.IndexOf("OLEDB", StringComparison.OrdinalIgnoreCase) >= 0
                   || text.IndexOf("ACE_NOT_INSTALLED", StringComparison.OrdinalIgnoreCase) >= 0
                   || text.IndexOf("provider is not registered", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static void ShowMainWindow()
        {
            var w = new MainWindow();
            Application.Current.MainWindow = w;
            w.Show();
        }
    }
}
