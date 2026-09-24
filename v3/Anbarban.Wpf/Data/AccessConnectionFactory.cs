using System;
using System.Data.OleDb;
using System.IO;
using Anbarban.Services;

namespace Anbarban.Data
{
    public sealed class AccessConnectionFactory
    {
        public string DatabasePath => AccessConfig.GetDatabasePath();

        public AccessConnectionFactory() => EnsureReady(DatabasePath);

        private static void EnsureReady(string path)
        {
            if (!AceProviderService.IsAceAvailable())
                throw new InvalidOperationException("ACE_NOT_INSTALLED");

            if (!File.Exists(path) || !DatabaseBootstrap.SchemaExists(path))
            {
                if (!DatabaseBootstrap.EnsureDatabase(path))
                    throw new FileNotFoundException(BuildHelpMessage(path));
            }
        }

        public static string BuildHelpMessage(string path) =>
            "فایل پایگاه داده پیدا نشد یا ساخته نشد." + Environment.NewLine + Environment.NewLine +
            "مسیر مورد انتظار:" + Environment.NewLine + path + Environment.NewLine + Environment.NewLine +
            "کارهایی که می‌توانید انجام دهید:" + Environment.NewLine +
            "۱) پوشه Data کنار Anbarban.exe بسازید و Inventory.accdb را آنجا کپی کنید" + Environment.NewLine +
            "۲) فایل «ساخت-پایگاه-داده.bat» را در همین پوشه پرتابل اجرا کنید" + Environment.NewLine +
            "۳) پیش‌نیازهای آفلاین: ZIP «پیش‌نیازها» را در پوشه prerequisites کنار exe بگذارید" + Environment.NewLine + Environment.NewLine +
            "نکته: اسکریپت قدیمی «ساخت-دیتابیس» فرم frmMain می‌سازد — برای v3 از «ساخت-پایگاه-داده» استفاده کنید.";

        public OleDbConnection Open()
        {
            var path = DatabasePath;
            EnsureReady(path);
            var c = new OleDbConnection(AccessConfig.BuildConnectionString(path));
            c.Open();
            return c;
        }
    }
}
