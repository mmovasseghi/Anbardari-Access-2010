using System;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Models;
using Anbarban.Services;
using Anbarban.Views.Dialogs;
using Microsoft.VisualBasic;

namespace Anbarban.Views
{
    public partial class OutgoingPage : Page
    {
        private int _docId;
        private bool _posted;

        public OutgoingPage(int existingId = 0)
        {
            InitializeComponent();
            _docId = existingId;
            Loaded += (_, __) =>
            {
                if (_docId > 0) LoadDoc();
                else
                {
                    TxtJalali.Text = JalaliCalendar.Format(DateTime.Today);
                    TxtStatus.Text = "پیش‌نویس";
                }
            };
        }

        private void LoadDoc()
        {
            var h = AppServices.Outgoing.Get(_docId);
            if (h == null) return;
            _posted = h.IsPosted;
            TxtDocNo.Text = h.DocumentNumber;
            TxtDelivery.Text = h.DeliveryNumber;
            TxtJalali.Text = JalaliCalendar.Format(h.DocumentDate);
            TxtDesc.Text = h.Description;
            TxtStatus.Text = _posted ? "ثبت نهایی — قفل" : "پیش‌نویس";
            BtnUnlock.Visibility = _posted ? Visibility.Visible : Visibility.Collapsed;
            TxtDocNo.IsEnabled = TxtDelivery.IsEnabled = TxtJalali.IsEnabled = TxtDesc.IsEnabled = BtnSave.IsEnabled = !_posted;
            GridLines.ItemsSource = AppServices.Outgoing.GetLines(_docId);
        }

        private void OnSaveHeader(object sender, RoutedEventArgs e)
        {
            if (_posted) return;
            if (string.IsNullOrWhiteSpace(TxtDocNo.Text) || string.IsNullOrWhiteSpace(TxtDelivery.Text))
            {
                MessageBox.Show("شماره سند و حواله الزامی است.", "انباربان"); return;
            }
            if (!JalaliCalendar.TryParse(TxtJalali.Text, out var dt))
            {
                MessageBox.Show("تاریخ شمسی نادرست است.", "انباربان"); return;
            }
            var h = new OutgoingHeader
            {
                Id = _docId,
                DocumentNumber = TxtDocNo.Text.Trim(),
                DeliveryNumber = TxtDelivery.Text.Trim(),
                DocumentDate = dt,
                Description = TxtDesc.Text.Trim()
            };
            _docId = AppServices.Outgoing.SaveHeader(h);
            MessageBox.Show("حواله ذخیره شد.", "انباربان");
        }

        private void OnAddLine(object sender, RoutedEventArgs e)
        {
            if (_docId <= 0) { MessageBox.Show("ابتدا حواله را ذخیره کنید.", "انباربان"); return; }
            if (_posted) return;
            var dlg = new LineEditorWindow(true) { Owner = Window.GetWindow(this) };
            dlg.ShowDialog();
            if (!dlg.Ok) return;
            if (AppServices.Outgoing.WouldExceedStock(_docId, dlg.ProductId, dlg.Quantity, 0))
            {
                MessageBox.Show($"موجودی کافی نیست. موجودی فعلی: {AppServices.Stock.GetCurrentStock(dlg.ProductId)}", "انباربان");
                return;
            }
            AppServices.Outgoing.AddLine(_docId, dlg.ProductId, dlg.Quantity, dlg.DepartmentId);
            GridLines.ItemsSource = AppServices.Outgoing.GetLines(_docId);
        }

        private void OnDelLine(object sender, RoutedEventArgs e)
        {
            if (_posted || !(GridLines.SelectedItem is OutgoingLine line)) return;
            AppServices.Outgoing.DeleteLine(line.Id);
            GridLines.ItemsSource = AppServices.Outgoing.GetLines(_docId);
        }

        private void OnReview(object sender, RoutedEventArgs e)
        {
            var lines = AppServices.Outgoing.GetLines(_docId);
            if (lines.Count == 0) { MessageBox.Show("حداقل یک قلم وارد کنید.", "انباربان"); return; }
            var h = AppServices.Outgoing.Get(_docId)!;
            var w = new ConfirmOutgoingWindow(h, lines) { Owner = Window.GetWindow(this) };
            w.ShowDialog();
            if (w.Posted) LoadDoc();
        }

        private void OnUnlock(object sender, RoutedEventArgs e)
        {
            var h = AppServices.Outgoing.Get(_docId);
            if (h == null || !h.IsPosted) return;
            var code = Interaction.InputBox("کد مدیر:", "انباربان", "");
            if (AppServices.Unlock.TryUnlockAndUnpost("OUTGOING", _docId, h.DocumentNumber, code))
            {
                MessageBox.Show("سند باز شد.", "انباربان");
                LoadDoc();
            }
            else MessageBox.Show("کد نامعتبر است.", "انباربان");
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);
    }
}
