using System;
using System.IO;
using System.Text;
using System.Windows;
using Anbarban.Services;

namespace Anbarban
{
    internal static class UiError
    {
        public static string LogFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error-log.txt");

        /// <summary>آخرین خطا به‌صورت یک‌خطی برای --ui-test و عیب‌یابی خودکار.</summary>
        public static string LastErrorFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last-error.txt");

        public static void Show(Exception ex, string userHint)
        {
            var logged = Log(ex, userHint);
            var msg = string.IsNullOrWhiteSpace(userHint)
                ? ex.Message
                : userHint + Environment.NewLine + Environment.NewLine + ex.Message;

            if (!string.IsNullOrEmpty(logged))
                msg += Environment.NewLine + Environment.NewLine + "جزئیات فنی در فایل:" + Environment.NewLine + logged;

            if (string.Equals(Environment.GetEnvironmentVariable("ANBARBAN_UI_TEST"), "1", StringComparison.Ordinal))
            {
                System.Diagnostics.Debug.WriteLine("UiError: " + msg);
                Console.Error.WriteLine(msg);
                return;
            }

            var blob = ex.ToString();
            if (blob.IndexOf("ACE_NOT_INSTALLED", StringComparison.OrdinalIgnoreCase) >= 0
                || blob.IndexOf("ACE.OLEDB", StringComparison.OrdinalIgnoreCase) >= 0
                || blob.IndexOf("provider is not registered", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                UiDialog.ShowAceMissing(Application.Current?.MainWindow);
                return;
            }

            AnbarbanDialog.Warn(msg, Application.Current?.MainWindow);
        }

        /// <summary>ثبت کامل استک برای عیب‌یابی خودکار (--ui-test و اجرای عادی).</summary>
        public static string Log(Exception ex, string context)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("==== " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ====");
                sb.AppendLine("Context: " + (context ?? ""));
                sb.AppendLine(ex.ToString());
                if (ex.InnerException != null)
                {
                    sb.AppendLine("--- Inner ---");
                    sb.AppendLine(ex.InnerException.ToString());
                }
                sb.AppendLine();
                var full = sb.ToString();
                File.AppendAllText(LogFilePath, full, Encoding.UTF8);
                try
                {
                    var oneLine = ex.GetType().FullName + " | " + (context ?? "") + " | " + ex.Message;
                    File.WriteAllText(LastErrorFilePath, oneLine + Environment.NewLine + ex.StackTrace, Encoding.UTF8);
                }
                catch { /* ignore */ }
                return LogFilePath;
            }
            catch
            {
                return "";
            }
        }

        public static bool Try(Action action, string userHint)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                Show(ex, userHint);
                return false;
            }
        }
    }
}
