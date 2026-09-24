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
            PostedPanel.NewDocumentClicked += (_, __) => Navigation.GoOutgoing(this);
            PostedPanel.HomeClicked += (_, __) => Navigation.GoHome(this);
            PostedPanel.UnlockClicked += (_, __) => OnUnlock(this, new RoutedEventArgs());
            Loaded += (_, __) =>
            {
                if (_docId > 0) LoadDoc();
                else
                {
                    PickerJalali.SelectedDate = DateTime.Today;
                    TxtStatus.Text = "پیش‌نویس — بعد از ثبت نهایی موجودی کم می‌شود";
                }
            };
        }

        private Window? OwnerWin => Window.GetWindow(this);

        private void LoadDoc()
        {
            var h = AppServices.Outgoing.Get(_docId);
            if (h == null) return;
            _posted = h.IsPosted;
            if (_posted)
            {
                ShowPostedScreen(h);
                return;
            }
            ShowForm();
            TxtDelivery.Text = h.DeliveryNumber;
            PickerJalali.SetGregorian(h.DocumentDate);
            TxtDesc.Text = h.Description;
            TxtStatus.Text = "هنوز ثبت نهایی نشده";
            GridLines.ItemsSource = AppServices.Outgoing.GetLines(_docId);
        }

        private void ShowPostedScreen(OutgoingHeader h)
        {
            FormScroll.Visibility = Visibility.Collapsed;
            PostedPanel.Visibility = Visibility.Visible;
            PostedPanel.Configure(
                "ثبت خروج جدید",
                "درخواست شما انجام شد",
                "این حواله ثبت نهایی شده و از موجودی کم شده است.",
                $"حواله {h.DeliveryNumber} · تاریخ {JalaliCalendar.Format(h.DocumentDate)}",
                true);
        }

        private void ShowForm()
        {
            PostedPanel.Visibility = Visibility.Collapsed;
            FormScroll.Visibility = Visibility.Visible;
        }

        private void OnSaveHeader(object sender, RoutedEventArgs e)
        {
            if (_posted) return;
            if (string.IsNullOrWhiteSpace(TxtDelivery.Text))
            {
                AnbarbanDialog.Warn("شماره حواله را وارد کنید.", OwnerWin); return;
            }
            if (!PickerJalali.TryGetGregorian(out var dt))
            {
                AnbarbanDialog.Warn("تاریخ شمسی را درست وارد کنید (مثل 1405/07/01).", OwnerWin); return;
            }
            var h = new OutgoingHeader
            {
                Id = _docId,
                DeliveryNumber = TxtDelivery.Text.Trim(),
                DocumentDate = dt,
                Description = TxtDesc.Text.Trim()
            };
            try
            {
                _docId = AppServices.Outgoing.SaveHeader(h);
                TxtStatus.Text = "سرِ حواله ذخیره شد؛ حالا اقلام را بزنید";
                if (!UiTestMode.SuppressSuccessPopups)
                    AnbarbanDialog.Success("حواله ذخیره شد.\nحالا اقلام خروج را اضافه کنید.", OwnerWin);
            }
            catch (Exception ex) { UiError.Show(ex, "ذخیره حواله انجام نشد."); }
        }

        private void OnAddLine(object sender, RoutedEventArgs e)
        {
            if (_docId <= 0) { AnbarbanDialog.Info("اول دکمه «ذخیره حواله» را بزنید.", OwnerWin); return; }
            if (_posted) return;
            var dlg = new LineEditorWindow(true) { Owner = OwnerWin };
            dlg.ShowDialog();
            if (!dlg.Ok) return;
            if (AppServices.Outgoing.WouldExceedStock(_docId, dlg.ProductId, dlg.Quantity, 0))
            {
                AnbarbanDialog.Warn($"موجودی کافی نیست.\nموجودی فعلی: {AppServices.Stock.GetCurrentStock(dlg.ProductId)}", OwnerWin);
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
            if (lines.Count == 0) { AnbarbanDialog.Warn("حداقل یک قلم وارد کنید.", OwnerWin); return; }
            var h = AppServices.Outgoing.Get(_docId)!;
            var w = new ConfirmOutgoingWindow(h, lines) { Owner = OwnerWin };
            w.ShowDialog();
            if (!w.Posted) return;
            _posted = true;
            var posted = AppServices.Outgoing.Get(_docId);
            if (posted != null) ShowPostedScreen(posted);
        }

        private void OnUnlock(object sender, RoutedEventArgs e)
        {
            var h = AppServices.Outgoing.Get(_docId);
            if (h == null || !h.IsPosted) return;
            var code = Interaction.InputBox("کد مدیر:", "انباربان", "");
            if (AppServices.Unlock.TryUnlockAndUnpost("OUTGOING", _docId, h.DeliveryNumber, code))
            {
                _posted = false;
                AnbarbanDialog.Success("سند برای اصلاح باز شد.", OwnerWin);
                ShowForm();
                LoadDoc();
            }
            else AnbarbanDialog.Warn("کد نامعتبر است.", OwnerWin);
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);
    }
}
