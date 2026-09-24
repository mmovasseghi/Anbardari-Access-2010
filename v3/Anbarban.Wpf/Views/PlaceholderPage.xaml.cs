using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    public partial class PlaceholderPage : Page
    {
        public PlaceholderPage(string title, string body)
        {
            InitializeComponent();
            TxtTitle.Text = title;
            TxtBody.Text = body;
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow w)
                w.NavigateTo(new HomePage(), "خانه", "");
        }
    }
}
