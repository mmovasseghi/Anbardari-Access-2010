using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                CboProduct.ItemsSource = AppServices.Products.Search(null, 300);
                CboSupplier.ItemsSource = AppServices.Suppliers.ListActive();
                CboDept.ItemsSource = AppServices.Departments.ListActive();
            };
        }

        private (DateTime?, DateTime?) ParseRange()
        {
            DateTime? f = null, t = null;
            if (JalaliCalendar.TryParse(TxtFromJ.Text, out var fd)) f = fd;
            if (JalaliCalendar.TryParse(TxtToJ.Text, out var td)) t = td;
            return (f, t);
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {
            try
            {
                var (f, t) = ParseRange();
                DataTable dt;
                switch (CboType.SelectedIndex)
                {
                    case 1: dt = AppServices.Reports.RunStock(false); break;
                    case 2: dt = AppServices.Reports.RunStock(true); break;
                    case 3:
                        if (CboSupplier.SelectedValue == null) { MessageBox.Show("فروشنده را انتخاب کنید.", "انباربان"); return; }
                        dt = AppServices.Reports.RunBySupplier(Convert.ToInt32(CboSupplier.SelectedValue), f, t);
                        break;
                    case 4:
                        if (CboDept.SelectedValue == null) { MessageBox.Show("بخش را انتخاب کنید.", "انباربان"); return; }
                        dt = AppServices.Reports.RunByDepartment(Convert.ToInt32(CboDept.SelectedValue), f, t);
                        break;
                    case 5:
                        if (string.IsNullOrWhiteSpace(TxtDelivery.Text)) { MessageBox.Show("شماره حواله را وارد کنید.", "انباربان"); return; }
                        dt = AppServices.Reports.RunByDelivery(TxtDelivery.Text.Trim());
                        break;
                    case 6:
                        if (CboProduct.SelectedValue == null) { MessageBox.Show("کالا را انتخاب کنید.", "انباربان"); return; }
                        dt = AppServices.Reports.RunMovement(Convert.ToInt32(CboProduct.SelectedValue), f, t);
                        break;
                    default:
                        dt = AppServices.Reports.RunCombined(f, t, null, null, null);
                        break;
                }
                Grid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "انباربان"); }
        }

        private void OnExcelHint(object sender, RoutedEventArgs e) =>
            MessageBox.Show("ردیف‌های جدول را انتخاب کنید → Ctrl+C → در Excel Paste کنید.", "انباربان");

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);
    }
}
