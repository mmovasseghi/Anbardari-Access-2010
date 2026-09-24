using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Anbarban.Services;

namespace Anbarban.Controls
{
    public partial class JalaliDatePicker : UserControl
    {
        private int _viewYear;
        private int _viewMonth;
        private readonly Button[] _dayButtons = new Button[42];
        private bool _isLoaded;

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(JalaliDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public JalaliDatePicker()
        {
            JalaliCalendar.ToJalali(DateTime.Today, out _viewYear, out _viewMonth, out _);
            InitializeComponent();
            Loaded += (_, __) =>
            {
                _isLoaded = true;
                if (SelectedDate == null)
                    SelectedDate = DateTime.Today;
                SyncViewFromSelected();
                RefreshCalendar();
                SyncTextFromSelected();
            };
            IsEnabledChanged += (_, __) =>
            {
                if (TxtDate != null)
                    TxtDate.IsEnabled = BtnOpen.IsEnabled = IsEnabled;
            };
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not JalaliDatePicker p) return;
            if (!p._isLoaded || p.TxtMonthYear == null) return;
            p.SyncTextFromSelected();
            p.SyncViewFromSelected();
            p.RefreshCalendar();
        }

        public bool TryGetGregorian(out DateTime date) => JalaliCalendar.TryParse(TxtDate.Text, out date);

        public void SetGregorian(DateTime? g)
        {
            SelectedDate = g?.Date;
            SyncViewFromSelected();
            RefreshCalendar();
            SyncTextFromSelected();
        }

        private void SyncViewFromSelected()
        {
            var g = SelectedDate ?? DateTime.Today;
            JalaliCalendar.ToJalali(g, out _viewYear, out _viewMonth, out _);
        }

        private void SyncTextFromSelected()
        {
            if (TxtDate == null) return;
            TxtDate.Text = JalaliCalendar.Format(SelectedDate);
        }

        private void RefreshCalendar()
        {
            if (!_isLoaded || TxtMonthYear == null || DayGrid == null)
                return;
            if (_viewMonth < 1 || _viewMonth > 12)
                SyncViewFromSelected();

            var monthIndex = _viewMonth - 1;
            if (monthIndex < 0 || monthIndex >= JalaliCalendar.MonthNames.Length)
                return;

            TxtMonthYear.Text = $"{JalaliCalendar.MonthNames[monthIndex]} {_viewYear}";

            if (JalaliCalendar.TryJalaliToGregorian(_viewYear, _viewMonth, 1, out var first))
                TxtGregorianHint.Text = first.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            else
                TxtGregorianHint.Text = "";

            var daysInMonth = JalaliCalendar.GetDaysInMonth(_viewYear, _viewMonth);
            var startCol = JalaliCalendar.WeekColumnIndex(_viewYear, _viewMonth, 1);

            JalaliCalendar.ToJalali(DateTime.Today, out var ty, out var tm, out var td);

            for (var i = 0; i < 42; i++)
            {
                if (_dayButtons[i] == null)
                {
                    _dayButtons[i] = new Button();
                    _dayButtons[i].Click += OnDayClick;
                    if (DayGrid.Children.Count < 42)
                        DayGrid.Children.Add(_dayButtons[i]);
                }

                var btn = _dayButtons[i];
                var dayNum = i - startCol + 1;
                Style dayStyle;
                try
                {
                    dayStyle = (Style)FindResource("JalaliCalDayButton");
                }
                catch
                {
                    dayStyle = btn.Style;
                }

                if (dayNum < 1 || dayNum > daysInMonth)
                {
                    btn.Content = "";
                    btn.Tag = null;
                    btn.IsEnabled = false;
                    btn.Style = dayStyle;
                    continue;
                }

                btn.Content = dayNum.ToString();
                btn.Tag = dayNum;
                btn.IsEnabled = true;

                var isToday = _viewYear == ty && _viewMonth == tm && dayNum == td;
                var isSelected = false;
                if (SelectedDate.HasValue)
                {
                    JalaliCalendar.ToJalali(SelectedDate.Value, out var sy, out var sm, out var sd);
                    isSelected = sy == _viewYear && sm == _viewMonth && sd == dayNum;
                }

                btn.Style = isSelected
                    ? (Style)FindResource("JalaliCalDaySelected")
                    : isToday
                        ? (Style)FindResource("JalaliCalDayToday")
                        : dayStyle;
            }
        }

        private void OnDayClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b || b.Tag is not int day) return;
            if (!JalaliCalendar.TryJalaliToGregorian(_viewYear, _viewMonth, day, out var g)) return;
            SelectedDate = g;
            CalPopup.IsOpen = false;
        }

        private void OnPrevMonth(object sender, RoutedEventArgs e)
        {
            _viewMonth--;
            if (_viewMonth < 1) { _viewMonth = 12; _viewYear--; }
            RefreshCalendar();
        }

        private void OnNextMonth(object sender, RoutedEventArgs e)
        {
            _viewMonth++;
            if (_viewMonth > 12) { _viewMonth = 1; _viewYear++; }
            RefreshCalendar();
        }

        private void OnPickToday(object sender, RoutedEventArgs e)
        {
            SelectedDate = DateTime.Today;
            SyncViewFromSelected();
            RefreshCalendar();
        }

        private void OnTogglePopup(object sender, RoutedEventArgs e)
        {
            SyncViewFromSelected();
            RefreshCalendar();
            CalPopup.IsOpen = !CalPopup.IsOpen;
        }

        private void OnClosePopup(object sender, RoutedEventArgs e) => CalPopup.IsOpen = false;

        private void OnTextLostFocus(object sender, RoutedEventArgs e) => CommitText();

        private void OnTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitText();
                CalPopup.IsOpen = false;
            }
        }

        private void CommitText()
        {
            if (JalaliCalendar.TryParse(TxtDate.Text, out var g))
            {
                SelectedDate = g;
                SyncViewFromSelected();
                RefreshCalendar();
            }
            else if (!string.IsNullOrWhiteSpace(TxtDate.Text))
                SyncTextFromSelected();
        }
    }
}
