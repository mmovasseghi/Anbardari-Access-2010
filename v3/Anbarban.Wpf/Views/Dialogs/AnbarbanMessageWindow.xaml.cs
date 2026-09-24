using System.Windows;
using System.Windows.Media.Imaging;

namespace Anbarban.Views.Dialogs
{
    public enum AnbarbanMessageKind { Info, Success, Warning, Error }

    public partial class AnbarbanMessageWindow : Window
    {
        public AnbarbanMessageWindow(AnbarbanMessageKind kind, string message, string? subtitle = null)
        {
            InitializeComponent();
            TxtMessage.Text = message;
            TxtTitle.Text = subtitle ?? TitleFor(kind);
            ImgIcon.Source = IconFor(kind);
            if (kind == AnbarbanMessageKind.Success)
                BtnOk.Content = "عالی، ممنون";
            else if (kind == AnbarbanMessageKind.Warning || kind == AnbarbanMessageKind.Error)
                BtnOk.Content = "باشه، اصلاح می‌کنم";
        }

        private static string TitleFor(AnbarbanMessageKind kind) => kind switch
        {
            AnbarbanMessageKind.Success => "انجام شد",
            AnbarbanMessageKind.Warning => "یک لحظه صبر کنید",
            AnbarbanMessageKind.Error => "انجام نشد",
            _ => "پیام انباربان"
        };

        private static BitmapImage IconFor(AnbarbanMessageKind kind)
        {
            var path = kind switch
            {
                AnbarbanMessageKind.Success => "/Assets/Icons/check-circle.png",
                AnbarbanMessageKind.Warning => "/Assets/Icons/warning.png",
                AnbarbanMessageKind.Error => "/Assets/Icons/error.png",
                _ => "/Assets/Icons/info.png"
            };
            return new BitmapImage(new System.Uri(path, System.UriKind.Relative));
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
