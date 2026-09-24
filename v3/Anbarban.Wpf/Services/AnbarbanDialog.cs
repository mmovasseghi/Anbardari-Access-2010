using System.Windows;
using Anbarban.Views.Dialogs;

namespace Anbarban.Services
{
    /// <summary>پیام‌های شیک انباربان — بدون MessageBox ویندوز.</summary>
    public static class AnbarbanDialog
    {
        public static void Info(string message, Window? owner = null, string? title = null) =>
            Show(AnbarbanMessageKind.Info, message, title, owner);

        public static void Success(string message, Window? owner = null, string? title = null)
        {
            if (UiTestMode.SuppressSuccessPopups) return;
            Show(AnbarbanMessageKind.Success, message, title ?? "درخواست شما انجام شد", owner);
        }

        public static void Warn(string message, Window? owner = null, string? title = null) =>
            Show(AnbarbanMessageKind.Warning, message, title, owner);

        public static void Error(string message, Window? owner = null, string? title = null) =>
            Show(AnbarbanMessageKind.Error, message, title, owner);

        private static void Show(AnbarbanMessageKind kind, string message, string? title, Window? owner)
        {
            if (UiTestMode.Active && kind == AnbarbanMessageKind.Info && UiTestMode.SuppressSuccessPopups)
                return;

            var win = new AnbarbanMessageWindow(kind, message, title)
            {
                Owner = ResolveOwner(owner)
            };
            win.ShowDialog();
        }

        private static Window? ResolveOwner(Window? owner)
        {
            if (owner != null) return owner;
            if (Application.Current?.MainWindow is { IsLoaded: true } mw) return mw;
            return null;
        }
    }
}
