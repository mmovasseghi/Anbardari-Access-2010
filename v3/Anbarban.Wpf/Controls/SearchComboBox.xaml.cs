using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Anbarban.Models;

namespace Anbarban.Controls
{
    public partial class SearchComboBox : UserControl
    {
        private static readonly TimeSpan Debounce = TimeSpan.FromMilliseconds(260);
        private readonly DispatcherTimer _timer;
        private Func<string, IList<IdName>>? _loader;
        private bool _internalChange;
        private bool _browseMode;

        public static readonly DependencyProperty SelectedIdProperty =
            DependencyProperty.Register(nameof(SelectedId), typeof(int), typeof(SearchComboBox),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int SelectedId
        {
            get => (int)GetValue(SelectedIdProperty);
            set => SetValue(SelectedIdProperty, value);
        }

        public SearchComboBox()
        {
            InitializeComponent();
            _timer = new DispatcherTimer { Interval = Debounce };
            _timer.Tick += (_, __) =>
            {
                _timer.Stop();
                RunSearchSafe();
            };
            LstSuggest.PreviewMouseLeftButtonUp += OnListMouseUp;
            IsEnabledChanged += (_, __) => TxtQuery.IsEnabled = BtnDrop.IsEnabled = IsEnabled;
        }

        public void SetLoader(Func<string, IList<IdName>> loader) => _loader = loader;

        public void SetSelection(int id, string displayName)
        {
            using (InternalChange())
            {
                SelectedId = id;
                TxtQuery.Text = displayName ?? "";
                LstSuggest.ItemsSource = new List<IdName> { new IdName { Id = id, Name = displayName ?? "" } };
            }
        }

        public void ClearSelection()
        {
            using (InternalChange())
            {
                SelectedId = 0;
                TxtQuery.Text = "";
                LstSuggest.ItemsSource = null;
            }
        }

        public string QueryText => TxtQuery.Text ?? "";

        /// <summary>برای تست خودکار — اجرای فوری جستجو بدون debounce.</summary>
        internal void FlushPendingSearch()
        {
            _timer.Stop();
            RunSearchSafe();
        }

        private IDisposable InternalChange()
        {
            _internalChange = true;
            return new ActionDisposable(() => _internalChange = false);
        }

        private sealed class ActionDisposable : IDisposable
        {
            private readonly Action _onDispose;
            public ActionDisposable(Action onDispose) => _onDispose = onDispose;
            public void Dispose() => _onDispose();
        }

        private void OnQueryChanged(object sender, TextChangedEventArgs e)
        {
            if (_internalChange) return;
            SelectedId = 0;
            _timer.Stop();
            _timer.Start();
        }

        private void OnQueryGotFocus(object sender, RoutedEventArgs e)
        {
            if ((TxtQuery.Text ?? "").Trim().Length >= 1)
                RunSearchSafe();
        }

        private void OnQueryPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsEnabled) return;
            if (!TxtQuery.IsKeyboardFocusWithin)
                TxtQuery.Focus();
        }

        private void OnQueryKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && LstSuggest.Items.Count > 0)
            {
                SuggestPopup.IsOpen = true;
                LstSuggest.Focus();
                LstSuggest.SelectedIndex = 0;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
                SuggestPopup.IsOpen = false;
        }

        private void OnListMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_internalChange) return;
            var dep = e.OriginalSource as DependencyObject;
            while (dep != null && dep is not ListBoxItem)
                dep = VisualTreeHelper.GetParent(dep);
            if (dep is ListBoxItem { Content: IdName item })
                Commit(item);
        }

        private void OnListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && LstSuggest.SelectedItem is IdName item)
                Commit(item);
        }

        private void OnToggleDrop(object sender, RoutedEventArgs e)
        {
            if (SuggestPopup.IsOpen)
            {
                SuggestPopup.IsOpen = false;
                return;
            }
            _browseMode = true;
            RunSearchSafe();
            _browseMode = false;
            if (LstSuggest.Items.Count > 0)
            {
                SuggestPopup.IsOpen = true;
                LstSuggest.Focus();
            }
            else
                TxtQuery.Focus();
        }

        private void RunSearchSafe()
        {
            try
            {
                RunSearch();
            }
            catch (Exception ex)
            {
                UiError.Log(ex, "SearchComboBox.RunSearch");
                SuggestPopup.IsOpen = false;
                if (string.Equals(Environment.GetEnvironmentVariable("ANBARBAN_UI_TEST"), "1", StringComparison.Ordinal))
                    throw;
            }
        }

        private void RunSearch()
        {
            if (_loader == null) return;
            var term = (TxtQuery.Text ?? "").Trim();
            if (term.Length < 1 && !_browseMode)
            {
                LstSuggest.ItemsSource = null;
                SuggestPopup.IsOpen = false;
                return;
            }

            var items = _loader(term) ?? new List<IdName>();

            using (InternalChange())
            {
                LstSuggest.ItemsSource = items;
                SuggestPopup.IsOpen = items.Count > 0;
            }
        }

        private void Commit(IdName item)
        {
            if (item == null) return;
            using (InternalChange())
            {
                SelectedId = item.Id;
                TxtQuery.Text = item.Name ?? "";
                SuggestPopup.IsOpen = false;
            }
        }
    }
}
