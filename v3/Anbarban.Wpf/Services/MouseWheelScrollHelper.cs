using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anbarban.Services
{
    /// <summary>اسکرول چرخ ماوس روی ناحیه‌هایی که خودشان اسکرول ندارند.</summary>
    public static class MouseWheelScrollHelper
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(MouseWheelScrollHelper),
                new PropertyMetadata(false, OnEnableChanged));

        public static void SetEnable(DependencyObject el, bool value) => el.SetValue(EnableProperty, value);
        public static bool GetEnable(DependencyObject el) => (bool)el.GetValue(EnableProperty);

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement ui) return;
            if ((bool)e.NewValue)
                ui.PreviewMouseWheel += OnPreviewMouseWheel;
            else
                ui.PreviewMouseWheel -= OnPreviewMouseWheel;
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled) return;
            var origin = e.OriginalSource as DependencyObject;
            if (origin != null && HasScrollableParent(origin, e.Delta))
                return;

            var target = FindScrollTarget(sender as DependencyObject, origin);
            if (target == null) return;

            if (target is ScrollViewer sv)
            {
                var next = sv.VerticalOffset - e.Delta / 3.0;
                if (next < 0) next = 0;
                if (next > sv.ScrollableHeight) next = sv.ScrollableHeight;
                sv.ScrollToVerticalOffset(next);
                e.Handled = true;
                return;
            }

            if (target is DataGrid dg)
            {
                var inner = FindVisualChild<ScrollViewer>(dg);
                if (inner != null && (inner.ScrollableHeight > 0 || inner.ScrollableWidth > 0))
                {
                    inner.ScrollToVerticalOffset(inner.VerticalOffset - e.Delta / 3.0);
                    e.Handled = true;
                }
            }
        }

        private static bool HasScrollableParent(DependencyObject start, int delta)
        {
            for (var p = start; p != null; p = VisualTreeHelper.GetParent(p))
            {
                if (p is ScrollViewer sv && sv.ScrollableHeight > 0)
                    return true;
                if (p is DataGrid)
                    return true;
            }
            return false;
        }

        private static DependencyObject FindScrollTarget(DependencyObject pageRoot, DependencyObject from)
        {
            for (var p = from ?? pageRoot; p != null; p = VisualTreeHelper.GetParent(p))
            {
                if (p is DataGrid dg) return dg;
                if (p is ScrollViewer sv && sv.ScrollableHeight > 0) return sv;
            }
            if (pageRoot != null)
            {
                var main = FindVisualChild<ScrollViewer>(pageRoot);
                if (main != null) return main;
            }
            return pageRoot;
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T match) return match;
                var nested = FindVisualChild<T>(child);
                if (nested != null) return nested;
            }
            return null!;
        }
    }
}
