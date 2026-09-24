using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    internal static class Navigation
    {
        public static void GoHome(DependencyObject from)
        {
            if (Window.GetWindow(from) is MainWindow mw)
                mw.NavigateTo(new HomePage(), "خانه", "کارهای روزانه");
        }

        public static void GoIncoming(DependencyObject from)
        {
            if (Window.GetWindow(from) is MainWindow mw)
                mw.NavigateTo(new IncomingPage(), "ثبت ورود کالا", "فاکتور خرید را اینجا می‌زنید");
        }

        public static void GoOutgoing(DependencyObject from)
        {
            if (Window.GetWindow(from) is MainWindow mw)
                mw.NavigateTo(new OutgoingPage(), "ثبت خروج / حواله", "کالا از انبار خارج می‌شود");
        }

        public static void Go(DependencyObject from, Page page)
        {
            if (Window.GetWindow(from) is MainWindow mw)
                mw.MainFrame.Navigate(page);
        }
    }
}
