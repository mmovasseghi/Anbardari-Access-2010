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
            _docId = header.Id;
            TxtSummary.Text =
                $"شماره سند: {header.DocumentNumber}\nشماره فاکتور: {header.InvoiceNumber}\n" +
                $"تاریخ: {JalaliCalendar.Format(header.DocumentDate)}\nفروشنده: {header.SupplierName}";
            Grid.ItemsSource = lines;
        }

        private void OnBack() => Close();

        private void OnConfirm()
        {
            var err = AppServices.Post.PostIncoming(_docId);
            if (err != null)
            {
                MessageBox.Show(err, "انباربان", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Posted = true;
            MessageBox.Show("ورود کالا ثبت نهایی شد.", "انباربان", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
