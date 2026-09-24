using System;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Models;
using Anbarban.Services;
using Anbarban.Views.Dialogs;
using Microsoft.VisualBasic;

namespace Anbarban.Views
{
    public partial class IncomingPage : Page
    {
        private int _docId;
        private bool _posted;

        public IncomingPage(int existingId = 0)
        {
            InitializeComponent();
            _docId = existingId;
            PostedPanel.NewDocumentClicked += (_, __) => Navigation.GoIncoming(this);
            PostedPanel.HomeClicked += (_, __) => Navigation.GoHome(this);
            PostedPanel.UnlockClicked += (_, __) => OnUnlock(this, new RoutedEventArgs());
            Loaded += (_, __) => Init();
        }

        private void Init()
        {
            LiveComboSearch.AttachSuppliers(CboSupplier);
            if (_docId > 0) LoadDoc();
            else
            {
                PickerJalali.SelectedDate = DateTime.Today;
                TxtStatus.Text = "هنوز ثبت نهایی نشده؛ موجودی عوض نمی‌شود";
            }
        }

        private void LoadDoc()
        {
            var h = AppServices.Incoming.Get(_docId);
            if (h == null) return;
            _posted = h.IsPosted;
            if (_posted)
            {
                ShowPostedScreen(h);
                return;
            }
            ShowForm();
            TxtDocNo.Text = h.DocumentNumber;
            TxtInvoice.Text = h.InvoiceNumber;
            PickerJalali.SetGregorian(h.DocumentDate);
            LiveComboSearch.SetSelectedId(CboSupplier, h.SupplierId, h.SupplierName);
            TxtDesc.Text = h.Description;
            TxtStatus.Text = "هنوز ثبت نهایی نشده";
            GridLines.ItemsSource = AppServices.Incoming.GetLines(_docId);
        }

        private void ShowPostedScreen(IncomingHeader h)
        {
            FormScroll.Visibility = Visibility.Collapsed;
            PostedPanel.Visibility = Visibility.Visible;
            var supplier = string.IsNullOrWhiteSpace(h.SupplierName) ? "—" : h.SupplierName;
            PostedPanel.Configure(
                "ثبت ورود جدید",
                "درخواست شما انجام شد",
                "این فاکتور ثبت نهایی شده و موجودی انبار به‌روز است.",
                $"سند {h.DocumentNumber} · فروشنده: {supplier}",
                true);
        }

        private void ShowForm()
        {
            PostedPanel.Visibility = Visibility.Collapsed;
            FormScroll.Visibility = Visibility.Visible;
        }

        private void SetReadOnly(bool ro)
        {
            TxtDocNo.IsEnabled = !ro;
            TxtInvoice.IsEnabled = !ro;
            PickerJalali.IsEnabled = !ro;
            CboSupplier.IsEnabled = !ro;
            TxtDesc.IsEnabled = !ro;
            BtnSave.IsEnabled = !ro;
        }

        private Window? OwnerWin => Window.GetWindow(this);

        private void OnSaveHeader(object sender, RoutedEventArgs e)
        {
            if (_posted) return;
            if (string.IsNullOrWhiteSpace(TxtDocNo.Text))
            {
                AnbarbanDialog.Warn("شماره سند را وارد کنید.", OwnerWin); return;
            }
            if (CboSupplier.SelectedId <= 0)
            {
                AnbarbanDialog.Warn("فروشنده را از لیست انتخاب کنید.\nچند حرف از نام را بنویسید تا لیست کوتاه شود.", OwnerWin); return;
            }
            if (!PickerJalali.TryGetGregorian(out var dt))
            {
                AnbarbanDialog.Warn("تاریخ را مثل 1405/07/01 وارد کنید.", OwnerWin); return;
            }
            var h = new IncomingHeader
            {
                Id = _docId,
                DocumentNumber = TxtDocNo.Text.Trim(),
                InvoiceNumber = TxtInvoice.Text.Trim(),
                DocumentDate = dt,
                SupplierId = CboSupplier.SelectedId,
                Description = TxtDesc.Text.Trim()
            };
            try
            {
                _docId = AppServices.Incoming.SaveHeader(h);
                TxtStatus.Text = "سرِ فاکتور ذخیره شد؛ حالا اقلام را بزنید";
                if (!UiTestMode.SuppressSuccessPopups)
                    AnbarbanDialog.Success("اطلاعات فاکتور ذخیره شد.\nحالا می‌توانید اقلام را اضافه کنید.", OwnerWin);
            }
            catch (Exception ex) { AnbarbanDialog.Error(ex.Message, OwnerWin); }
        }

        private void OnAddLine(object sender, RoutedEventArgs e)
        {
            if (_docId <= 0) { AnbarbanDialog.Info("اول دکمه «ذخیره اطلاعات فاکتور» را بزنید.", OwnerWin); return; }
            if (_posted) return;
            var dlg = new LineEditorWindow(false) { Owner = OwnerWin };
            dlg.ShowDialog();
            if (!dlg.Ok) return;
            AppServices.Incoming.AddLine(_docId, dlg.ProductId, dlg.Quantity);
            GridLines.ItemsSource = AppServices.Incoming.GetLines(_docId);
        }

        private void OnDelLine(object sender, RoutedEventArgs e)
        {
            if (_posted || !(GridLines.SelectedItem is IncomingLine line)) return;
            AppServices.Incoming.DeleteLine(line.Id);
            GridLines.ItemsSource = AppServices.Incoming.GetLines(_docId);
        }

        private void OnReview(object sender, RoutedEventArgs e)
        {
            if (_docId <= 0) return;
            var lines = AppServices.Incoming.GetLines(_docId);
            if (lines.Count == 0)
            {
                AnbarbanDialog.Warn("حداقل یک قلم کالا وارد کنید.", OwnerWin); return;
            }
            var h = AppServices.Incoming.Get(_docId);
            if (h == null)
            {
                AnbarbanDialog.Warn(
                    "سرِ فاکتور پیدا نشد.\nدوباره فاکتور را ذخیره کنید یا فروشنده را برگردانید.",
                    OwnerWin);
                return;
            }
            var w = new ConfirmIncomingWindow(h, lines) { Owner = OwnerWin };
            w.ShowDialog();
            if (!w.Posted) return;
            _posted = true;
            var posted = AppServices.Incoming.Get(_docId);
            if (posted != null) ShowPostedScreen(posted);
        }

        private void OnUnlock(object sender, RoutedEventArgs e)
        {
            var h = AppServices.Incoming.Get(_docId);
            if (h == null || !h.IsPosted) return;
            var code = Interaction.InputBox("کد ۶ حرفی مدیر را وارد کنید:", "انباربان", "");
            if (AppServices.Unlock.TryUnlockAndUnpost("INCOMING", _docId, h.DocumentNumber, code))
            {
                _posted = false;
                AnbarbanDialog.Success("سند برای اصلاح باز شد.", OwnerWin);
                ShowForm();
                LoadDoc();
            }
            else AnbarbanDialog.Warn("کد معتبر نیست یا قبلاً استفاده شده.", OwnerWin);
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);

        internal void UiTest_FillHeader(string docNo, int supplierId, string supplierName)
        {
            TxtDocNo.Text = docNo;
            LiveComboSearch.SetSelectedId(CboSupplier, supplierId, supplierName);
            PickerJalali.SelectedDate = DateTime.Today;
        }

        internal void UiTest_ClickSaveHeader() => OnSaveHeader(this, new RoutedEventArgs());

        internal void UiTest_ClickAddLine() => OnAddLine(this, new RoutedEventArgs());

        internal void UiTest_ClickReview() => OnReview(this, new RoutedEventArgs());

        internal int GetDocIdForTest() => _docId;
    }
}
