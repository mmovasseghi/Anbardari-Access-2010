using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    internal static class Navigation
    {
        public static void GoHome(DependencyObject from) =>
            (Window.GetWindow(from) as MainWindow)?.NavigateTo(new HomePage());

        public static void Go(DependencyObject from, Page page) =>
            (Window.GetWindow(from) as MainWindow)?.NavigateTo(page);
    }
}
