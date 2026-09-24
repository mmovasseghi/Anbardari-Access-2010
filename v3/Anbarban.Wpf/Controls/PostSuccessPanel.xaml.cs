using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Controls
{
    public partial class PostSuccessPanel : UserControl
    {
        public event RoutedEventHandler? NewDocumentClicked;
        public event RoutedEventHandler? HomeClicked;
        public event RoutedEventHandler? UnlockClicked;

        public PostSuccessPanel()
        {
            InitializeComponent();
        }

        public void Configure(string newButtonLabel, string headline, string subtitle, string detail, bool showUnlock)
        {
            BtnNew.Content = newButtonLabel;
            TxtHeadline.Text = headline;
            TxtSubtitle.Text = subtitle;
            TxtDetail.Text = detail;
            BtnUnlock.Visibility = showUnlock ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnNew(object sender, RoutedEventArgs e) => NewDocumentClicked?.Invoke(this, e);

        private void OnHome(object sender, RoutedEventArgs e) => HomeClicked?.Invoke(this, e);

        private void OnUnlock(object sender, RoutedEventArgs e) => UnlockClicked?.Invoke(this, e);
    }
}
