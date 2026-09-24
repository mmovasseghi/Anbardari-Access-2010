using System;
using System.Collections.Generic;
using System.Windows;
using Anbarban.Models;
using Anbarban.Services;

namespace Anbarban.Views.Dialogs
{
    public partial class ConfirmIncomingWindow : Window
    {
        private readonly int _docId;
        public bool Posted { get; private set; }

        public ConfirmIncomingWindow(IncomingHeader header, IList<IncomingLine> lines)
        {
            InitializeComponent();
            if (header == null)
                throw new ArgumentNullException(nameof(header));
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));
            _docId = header.Id;
            var supplier = string.IsNullOrWhiteSpace(header.SupplierName) ? "—" : header.SupplierName;
            TxtSummary.Text =
                $"شماره سند: {header.DocumentNumber}\nشماره فاکتور: {header.InvoiceNumber}\n" +
                $"تاریخ: {JalaliCalendar.Format(header.DocumentDate)}\nفروشنده: {supplier}";
            LinesGrid.ItemsSource = lines;
        }

        private void OnBack(object sender, RoutedEventArgs e) => Close();

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            var err = AppServices.Post.PostIncoming(_docId);
            if (err != null)
            {
                AnbarbanDialog.Warn(err, this);
                return;
            }
            Posted = true;
            if (UiTestMode.SuppressSuccessPopups)
            {
                Close();
                return;
            }
            ReviewContent.Visibility = Visibility.Collapsed;
            PanelSuccess.Visibility = Visibility.Visible;
            TxtSuccessDetail.Text = "سند ورود در سیستم ثبت شد.";
        }

        private void OnSuccessDone(object sender, RoutedEventArgs e) => Close();
    }
}
