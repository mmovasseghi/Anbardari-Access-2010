using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Anbarban.Views.Dialogs
{
    public partial class AnbarMessageWindow : Window
    {
        public AnbarMessageResult Result { get; private set; } = AnbarMessageResult.None;

        private AnbarMessageWindow(string title, string message, IList<AnbarMessageButton> buttons)
        {
            InitializeComponent();
            TxtTitle.Text = title;
            TxtMessage.Text = message;
            foreach (var b in buttons)
            {
                var btn = new Button
                {
                    Content = b.Text,
                    Margin = new Thickness(6, 0, 6, 0),
                    MinWidth = 140,
                    Padding = new Thickness(16, 10, 16, 10),
                    Style = b.IsPrimary
                        ? (Style)FindResource("PrimaryActionButton")
                        : (Style)FindResource("MenuTileButton")
                };
                btn.Click += (_, __) =>
                {
                    Result = b.Result;
                    DialogResult = true;
                    Close();
                };
                ButtonPanel.Children.Add(btn);
            }
        }

        private void OnDragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        public static AnbarMessageResult Show(
            string title,
            string message,
            IList<AnbarMessageButton> buttons,
            Window? owner = null)
        {
            var w = new AnbarMessageWindow(title, message, buttons);
            if (owner != null && owner.IsLoaded)
            {
                w.Owner = owner;
                w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            w.ShowDialog();
            return w.Result;
        }
    }

    public enum AnbarMessageResult
    {
        None,
        Ok,
        Cancel,
        InstallAce,
        DownloadAce,
        OpenFolder,
        Retry
    }

    public sealed class AnbarMessageButton
    {
        public string Text { get; set; } = "";
        public AnbarMessageResult Result { get; set; }
        public bool IsPrimary { get; set; }

        public static AnbarMessageButton Ok(string text = "باشه") =>
            new AnbarMessageButton { Text = text, Result = AnbarMessageResult.Ok, IsPrimary = true };

        public static AnbarMessageButton Cancel(string text = "انصراف") =>
            new AnbarMessageButton { Text = text, Result = AnbarMessageResult.Cancel };
    }
}
