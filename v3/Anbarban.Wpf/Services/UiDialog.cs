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
            var msg =
                "موتور پایگاه Microsoft Access (ACE) روی این ویندوز ثبت نشده است." + Environment.NewLine + Environment.NewLine +
                "انباربان نسخه 64-bit به «Access Database Engine 64-bit» نیاز دارد." + Environment.NewLine + Environment.NewLine +
                "پیشنهاد: دکمه «نصب خودکار» را بزنید (فایل در پوشه redist یا دانلود از مایکروسافت)." + Environment.NewLine +
                "اگر Office 32-bit دارید، ممکن است نصب 64-bit خطا بدهد — در راهنما توضیح داده شده.";

            var buttons = new List<AnbarMessageButton>
            {
                new AnbarMessageButton { Text = "نصب خودکار موتور", Result = AnbarMessageResult.InstallAce, IsPrimary = true },
                new AnbarMessageButton { Text = "دانلود از مایکروسافت", Result = AnbarMessageResult.DownloadAce },
                new AnbarMessageButton { Text = "باز کردن پوشه redist", Result = AnbarMessageResult.OpenFolder },
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
