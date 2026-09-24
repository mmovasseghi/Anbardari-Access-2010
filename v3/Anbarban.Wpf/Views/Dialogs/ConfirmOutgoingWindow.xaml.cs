using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Anbarban.Models;
using Anbarban.Services;

namespace Anbarban.Views.Dialogs
{
    public partial class ConfirmOutgoingWindow : Window
    {
        private readonly int _docId;
        public bool Posted { get; private set; }

        public ConfirmOutgoingWindow(OutgoingHeader header, IList<OutgoingLine> lines)
        {
            InitializeComponent();
            _docId = header.Id;
            TxtSummary.Text =
                $"شماره حواله: {header.DeliveryNumber}\n" +
                $"تاریخ: {JalaliCalendar.Format(header.DocumentDate)}";
            Grid.ItemsSource = lines.Select(l => new
            {
                l.ProductName,
                l.Quantity,
                l.CurrentStock,
                AfterStock = l.CurrentStock - l.Quantity,
                l.DepartmentName
            }).ToList();
        }

        private void OnBack(object sender, RoutedEventArgs e) => Close();

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            var err = AppServices.Post.PostOutgoing(_docId);
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
            TxtSuccessDetail.Text = "حواله خروج در سیستم ثبت شد.";
        }

        private void OnSuccessDone(object sender, RoutedEventArgs e) => Close();
    }
}
