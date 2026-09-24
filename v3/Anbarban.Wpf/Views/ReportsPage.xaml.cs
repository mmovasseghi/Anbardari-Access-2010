using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Anbarban.Converters;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class ReportsPage : Page
    {
        private static readonly JalaliDisplayConverter JalaliConverter = new();
        private DataView? _lastView;

        public ReportsPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LiveComboSearch.AttachProducts(CboProduct);
            LiveComboSearch.AttachSuppliers(CboSupplier);
            LiveComboSearch.AttachDepartments(CboDept);
            LstReportType.SelectedIndex = 0;
            OnLast30Days(this, new RoutedEventArgs());
            UpdateFilterPanels();
        }

        private int ReportTypeIndex =>
            LstReportType.SelectedItem is ListBoxItem item && item.Tag is string tag && int.TryParse(tag, out var i) ? i : 0;

        private void OnReportTypeChanged(object sender, SelectionChangedEventArgs e) => UpdateFilterPanels();

        private void UpdateFilterPanels()
        {
            var t = ReportTypeIndex;
            PanelDates.Visibility = t == 0 || t == 3 || t == 4 || t == 6 ? Visibility.Visible : Visibility.Collapsed;
            PanelProduct.Visibility = t == 6 ? Visibility.Visible : Visibility.Collapsed;
            PanelSupplier.Visibility = t == 3 ? Visibility.Visible : Visibility.Collapsed;
            PanelDept.Visibility = t == 4 ? Visibility.Visible : Visibility.Collapsed;
            PanelDelivery.Visibility = t == 5 ? Visibility.Visible : Visibility.Collapsed;
        }

        private (DateTime?, DateTime?) ParseRange()
        {
            DateTime? f = null, t = null;
            if (PickerFromJ.TryGetGregorian(out var fd)) f = fd;
            if (PickerToJ.TryGetGregorian(out var td)) t = td;
            return (f, t);
        }

        private void OnLast30Days(object sender, RoutedEventArgs e)
        {
            var to = DateTime.Today;
            var from = to.AddDays(-30);
            PickerFromJ.SelectedDate = from;
            PickerToJ.SelectedDate = to;
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {
            try
            {
                var (f, t) = ParseRange();
                DataTable dt;
                var reportName = "گزارش";
                switch (ReportTypeIndex)
                {
                    case 1:
                        reportName = "موجودی انبار";
                        dt = AppServices.Reports.RunStock(false);
                        break;
                    case 2:
                        reportName = "کم‌موجودی";
                        dt = AppServices.Reports.RunStock(true);
                        break;
                    case 3:
                        if (CboSupplier.SelectedId <= 0) { AnbarbanDialog.Warn("فروشنده را انتخاب کنید.", Window.GetWindow(this)); return; }
                        reportName = "ورود از فروشنده";
                        dt = AppServices.Reports.RunBySupplier(CboSupplier.SelectedId, f, t);
                        break;
                    case 4:
                        if (CboDept.SelectedId <= 0) { AnbarbanDialog.Warn("بخش را انتخاب کنید.", Window.GetWindow(this)); return; }
                        reportName = "خروج به بخش";
                        dt = AppServices.Reports.RunByDepartment(CboDept.SelectedId, f, t);
                        break;
                    case 5:
                        if (string.IsNullOrWhiteSpace(TxtDelivery.Text)) { AnbarbanDialog.Warn("شماره حواله را وارد کنید.", Window.GetWindow(this)); return; }
                        reportName = "حواله";
                        dt = AppServices.Reports.RunByDelivery(TxtDelivery.Text.Trim());
                        break;
                    case 6:
                        if (CboProduct.SelectedId <= 0) { AnbarbanDialog.Warn("کالا را انتخاب کنید.", Window.GetWindow(this)); return; }
                        reportName = "گردش کالا";
                        dt = AppServices.Reports.RunMovement(CboProduct.SelectedId, f, t);
                        break;
                    default:
                        reportName = "ورود و خروج";
                        dt = AppServices.Reports.RunCombined(f, t, null, null, null);
                        break;
                }
                BindGrid(dt);
                var range = "";
                if (f.HasValue || t.HasValue)
                    range = $" | بازه: {(f.HasValue ? JalaliCalendar.Format(f.Value) : "…")} تا {(t.HasValue ? JalaliCalendar.Format(t.Value) : "…")}";
                TxtSummary.Text = dt.Rows.Count == 0
                    ? $"{reportName}: چیزی در این بازه نیست.{range}"
                    : $"{reportName}: {dt.Rows.Count} ردیف{range}";
                TxtEmpty.Visibility = dt.Rows.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex) { AnbarbanDialog.Error(ex.Message, Window.GetWindow(this)); }
        }

        private void BindGrid(DataTable dt)
        {
            if (!dt.Columns.Contains("RowNum"))
            {
                dt.Columns.Add("RowNum", typeof(string));
                for (var i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["RowNum"] = (i + 1).ToString(CultureInfo.InvariantCulture);
            }

            Grid.Columns.Clear();
            Grid.Columns.Add(new DataGridTextColumn
            {
                Header = "ردیف",
                Binding = new Binding("RowNum") { Mode = BindingMode.OneWay },
                Width = new DataGridLength(56),
                IsReadOnly = true,
                ElementStyle = (Style)FindResource("ReportCellNumber")
            });

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == "RowNum") continue;

                var header = col.ColumnName;
                var isNumber = IsNumericColumn(header);
                var isDate = col.DataType == typeof(DateTime) || header.Contains("تاریخ");

                Binding binding;
                if (isDate)
                    binding = new Binding(col.ColumnName) { Mode = BindingMode.OneWay, Converter = JalaliConverter };
                else
                    binding = new Binding(col.ColumnName) { Mode = BindingMode.OneWay };

                var column = new DataGridTextColumn
                {
                    Header = header,
                    Binding = binding,
                    Width = isNumber ? new DataGridLength(100) : new DataGridLength(1, DataGridLengthUnitType.Star),
                    MinWidth = isNumber ? 80 : 120,
                    ElementStyle = isNumber
                        ? (Style)FindResource("ReportCellNumber")
                        : (Style)FindResource("ReportCellText")
                };
                Grid.Columns.Add(column);
            }

            _lastView = dt.DefaultView;
            Grid.ItemsSource = _lastView;
        }

        private static bool IsNumericColumn(string header) =>
            header.Contains("تعداد") || header.Contains("موجودی") || header.Contains("حداقل") || header == "کد";

        private void OnCopy(object sender, RoutedEventArgs e)
        {
            if (_lastView == null || _lastView.Table.Columns.Count == 0)
            {
                AnbarbanDialog.Info("ابتدا دکمه «گزارش‌گیری» را بزنید.", Window.GetWindow(this));
                return;
            }
            var table = _lastView.Table;
            var sb = new StringBuilder();
            var cols = new List<DataColumn>();
            foreach (DataColumn c in table.Columns)
                if (c.ColumnName != "RowNum") cols.Add(c);

            for (var c = 0; c < cols.Count; c++)
            {
                if (c > 0) sb.Append('\t');
                sb.Append(cols[c].ColumnName);
            }
            sb.AppendLine();
            foreach (DataRow row in table.Rows)
            {
                for (var c = 0; c < cols.Count; c++)
                {
                    if (c > 0) sb.Append('\t');
                    var val = row[cols[c].ColumnName];
                    if (val is DateTime d) sb.Append(JalaliCalendar.Format(d));
                    else sb.Append(val?.ToString() ?? "");
                }
                sb.AppendLine();
            }
            Clipboard.SetText(sb.ToString());
            AnbarbanDialog.Success("کپی شد.\nدر Excel با Ctrl+V بچسبانید.", Window.GetWindow(this));
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);

        internal bool UiTest_IsRunButtonVisible() =>
            BtnRunReport != null
            && BtnRunReport.Visibility == Visibility.Visible
            && BtnRunReport.IsEnabled;

        internal int UiTest_RunStockReport()
        {
            LstReportType.SelectedIndex = 1;
            UpdateFilterPanels();
            OnRun(this, new RoutedEventArgs());
            return Grid.Items.Count;
        }
    }
}
