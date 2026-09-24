using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Anbarban.Views;
using Anbarban.Views.Dialogs;

namespace Anbarban.Services
{
    /// <summary>همان مسیر کاربر: دکمه‌ها و دیالوگ‌های مدال.</summary>
    public static class UiTestAutomation
    {
        public static void RunIncomingFullFlow(IUiTestHost host)
        {
            var sup = AppServices.Suppliers.ListActive();
            if (sup.Count == 0) throw new InvalidOperationException("no supplier");
            var prod = AppServices.Products.GetByCode("UITEST");
            if (prod == null) throw new InvalidOperationException("UITEST missing");

            var page = new IncomingPage();
            using var session = HostPage(host, page, "ثبت ورود — ذخیره فاکتور، قلم، بررسی نهایی");

            page.UiTest_FillHeader("UI-FULL-" + DateTime.Now.ToString("HHmmss"), sup[0].Id, sup[0].Name);
            page.UiTest_ClickSaveHeader();
            page.UpdateLayout();
            if (page.GetDocIdForTest() <= 0)
                throw new InvalidOperationException("header not saved");

            ScheduleModal<LineEditorWindow>(w =>
            {
                var label = prod.Name + (string.IsNullOrEmpty(prod.Code) ? "" : " (" + prod.Code + ")");
                LiveComboSearch.SetSelectedId(w.CboProduct, prod.Id, label);
                w.TxtQty.Text = "2";
                ClickButtonByContent(w, "تأیید");
            });
            page.UiTest_ClickAddLine();
            page.UpdateLayout();
            if (AppServices.Incoming.GetLines(page.GetDocIdForTest()).Count == 0)
                throw new InvalidOperationException("no lines after add");

            ScheduleModal<ConfirmIncomingWindow>(w => ClickButtonByContent(w, "تأیید و ثبت نهایی"));
            page.UiTest_ClickReview();
            page.UpdateLayout();

            var h = AppServices.Incoming.Get(page.GetDocIdForTest());
            if (h == null || !h.IsPosted)
                throw new InvalidOperationException("document not posted");
            var stock = AppServices.Stock.GetCurrentStock(prod.Id);
            if (stock < 2)
                throw new InvalidOperationException("stock=" + stock);
        }

        public static void RunOutgoingReviewFlow(IUiTestHost host, int productId)
        {
            var depts = AppServices.Departments.ListActive();
            if (depts.Count == 0)
                AppServices.Departments.Save(0, "بخش تست", true);
            depts = AppServices.Departments.ListActive();

            var h = new Models.OutgoingHeader
            {
                DeliveryNumber = "UI-OUT-" + DateTime.Now.ToString("HHmmss"),
                DocumentDate = DateTime.Today,
                Description = "ui flow"
            };
            var docId = AppServices.Outgoing.SaveHeader(h);
            AppServices.Outgoing.AddLine(docId, productId, 1, depts[0].Id);

            var page = new OutgoingPage(docId);
            using var session = HostPage(host, page, "ثبت خروج — بررسی نهایی");

            ScheduleModal<ConfirmOutgoingWindow>(w => ClickButtonByContent(w, "تأیید و ثبت نهایی"));
            var btn = FindButtonByContent(page, "بررسی و ثبت خروج");
            if (btn == null) throw new InvalidOperationException("review button missing");
            btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            page.UpdateLayout();

            var posted = AppServices.Outgoing.Get(docId);
            if (posted == null || !posted.IsPosted)
                throw new InvalidOperationException("outgoing not posted");
        }

        private static HostedPageSession HostPage(IUiTestHost host, Page page, string caption)
        {
            var window = new Window
            {
                Title = "انباربان — " + caption,
                Width = 1100,
                Height = 860,
                FlowDirection = FlowDirection.RightToLeft,
                FontFamily = page.FontFamily,
                Background = page.Background
            };
            var frame = new Frame { NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden };
            window.Content = frame;

            if (host.Live)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Topmost = true;
                window.Show();
                window.Activate();
            }
            else
            {
                window.ShowInTaskbar = false;
                window.WindowState = WindowState.Minimized;
                window.ShowActivated = false;
                window.Opacity = 0;
                window.Show();
            }

            frame.Navigate(page);
            PumpLoaded(page);
            page.UpdateLayout();
            return new HostedPageSession(window);
        }

        private static void PumpLoaded(FrameworkElement el)
        {
            el.Dispatcher.Invoke(() => { }, DispatcherPriority.Loaded);
            el.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        }

        private static void ScheduleModal<TWindow>(Action<TWindow> action) where TWindow : Window
        {
            var handled = false;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(80) };
            timer.Tick += (_, __) =>
            {
                if (handled) return;
                var w = Application.Current?.Windows.OfType<TWindow>().FirstOrDefault();
                if (w == null) return;
                handled = true;
                timer.Stop();
                action(w);
                w.UpdateLayout();
            };
            timer.Start();
        }

        private static void ClickButtonByContent(DependencyObject root, string contentPart)
        {
            var btn = FindButtonByContent(root, contentPart);
            if (btn == null)
                throw new InvalidOperationException("button not found: " + contentPart);
            btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private static Button? FindButtonByContent(DependencyObject root, string contentPart)
        {
            foreach (var btn in EnumerateVisuals<Button>(root))
            {
                var text = btn.Content?.ToString() ?? "";
                if (text.IndexOf(contentPart, StringComparison.Ordinal) >= 0)
                    return btn;
            }
            return null;
        }

        private static IEnumerable<T> EnumerateVisuals<T>(DependencyObject root) where T : DependencyObject
        {
            var count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i);
                if (child is T match)
                    yield return match;
                foreach (var nested in EnumerateVisuals<T>(child))
                    yield return nested;
            }
        }

        private sealed class HostedPageSession : IDisposable
        {
            private readonly Window _window;
            public HostedPageSession(Window window) => _window = window;
            public void Dispose() { try { _window.Close(); } catch { /* ignore */ } }
        }
    }
}
