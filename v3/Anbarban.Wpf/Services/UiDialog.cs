using System;
using System.Collections.Generic;
using System.Windows;
using Anbarban.Views.Dialogs;

namespace Anbarban.Services
{
    public static class UiDialog
    {
        public static void Info(string message, string title = "انباربان", Window? owner = null) =>
            AnbarMessageWindow.Show(title, message, new List<AnbarMessageButton> { AnbarMessageButton.Ok() }, owner);

        public static void Error(string title, string message, Window? owner = null) =>
            AnbarMessageWindow.Show(title, message, new List<AnbarMessageButton>
            {
                AnbarMessageButton.Ok("باشه، اصلاح می‌کنم")
            }, owner);

        public static bool Confirm(string message, string title = "تأیید", Window? owner = null)
        {
            var r = AnbarMessageWindow.Show(title, message, new List<AnbarMessageButton>
            {
                AnbarMessageButton.Ok("بله"),
                AnbarMessageButton.Cancel("خیر")
            }, owner);
            return r == AnbarMessageResult.Ok;
        }

        public static AnbarMessageResult ShowAceMissing(Window? owner = null)
        {
            var expected = OfflinePrerequisites.PrerequisitesFolder;
            var hasFile = OfflinePrerequisites.HasAceInstaller;
            var msg =
                "موتور پایگاه Microsoft Access (ACE) روی این ویندوز نصب نیست." + Environment.NewLine + Environment.NewLine +
                OfflinePrerequisites.OfflineHelpText + Environment.NewLine + Environment.NewLine +
                "مسیر پیش‌نیاز روی این PC:" + Environment.NewLine + expected + Environment.NewLine +
                (hasFile
                    ? "✓ فایل نصب ACE در پوشه پیدا شد — «شروع نصب ACE» را بزنید."
                    : "✗ فایل AccessDatabaseEngine_X64.exe اینجا نیست — ZIP پیش‌نیازها را کپی کنید.");

            var buttons = new List<AnbarMessageButton>
            {
                new AnbarMessageButton
                {
                    Text = hasFile ? "شروع نصب ACE" : "باز کردن پوشه پیش‌نیازها",
                    Result = AnbarMessageResult.InstallAce,
                    IsPrimary = true
                },
                new AnbarMessageButton
                {
                    Text = "نمایش فایل نصب در Explorer",
                    Result = AnbarMessageResult.OpenInstaller
                },
                new AnbarMessageButton { Text = "باز کردن پوشه prerequisites", Result = AnbarMessageResult.OpenFolder },
                new AnbarMessageButton { Text = "دوباره امتحان کن", Result = AnbarMessageResult.Retry },
                AnbarMessageButton.Cancel("بستن")
            };
            return AnbarMessageWindow.Show("انجام نشد", msg, buttons, owner);
        }

        public static void ShowException(Exception ex, Window? owner = null)
        {
            var text = ex.Message;
            if (text.Contains("ACE") || text.Contains("OLEDB") || text.Contains("ACE_NOT_INSTALLED"))
            {
                ShowAceMissing(owner);
                return;
            }
            Error("انجام نشد", text, owner);
        }
    }
}
