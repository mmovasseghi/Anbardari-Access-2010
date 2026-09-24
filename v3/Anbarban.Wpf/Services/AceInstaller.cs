using System;

namespace Anbarban.Services
{
    /// <summary>سازگاری با نام قبلی — همهٔ مسیرها آفلاین از OfflinePrerequisites.</summary>
    [Obsolete("Use OfflinePrerequisites")]
    public static class AceInstaller
    {
        public static bool HasBundledInstaller => OfflinePrerequisites.HasAceInstaller;

        public static (bool started, string message) TryRunBundledInstall() =>
            OfflinePrerequisites.TryRunAceInstall();

        public static void OpenRedistFolder() => OfflinePrerequisites.OpenPrerequisitesFolder();

        public static void OpenDownloadPage() => OfflinePrerequisites.OpenAceInstallerInExplorer();
    }
}
