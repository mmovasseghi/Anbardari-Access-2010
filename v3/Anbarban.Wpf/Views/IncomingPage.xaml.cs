using System;
using System.Linq;
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
            Loaded += (_, __) => Init();
        }

        private void Init()
        {
            CboSupplier.ItemsSource = AppServices.Suppliers.ListActive();
            if (_docId > 0) LoadDoc();
            else
            {
                TxtJalali.Text = JalaliCalendar.Format(DateTime.Today);
                TxtStatus.Text = "پیش‌نویس — موجودی هنوز تغییر نکرده";
            }
        }

        private void LoadDoc()
        {
            var h = AppServices.Incoming.Get(_docId);
            if (h == null) return;
            _posted = h.IsPosted;
            TxtDocNo.Text = h.DocumentNumber;
            TxtInvoice.Text = h.InvoiceNumber;
            TxtJalali.Text = JalaliCalendar.Format(h.DocumentDate);
            CboSupplier.SelectedValue = h.SupplierId;
            TxtDesc.Text = h.Description;
            TxtStatus.Text = _posted ? "ثبت نهایی شده — قفل" : "پیش‌نویس";
            BtnUnlock.Visibility = _posted ? Visibility.Visible : Visibility.Collapsed;
            SetReadOnly(_posted);
            GridLines.ItemsSource = AppServices.Incoming.GetLines(_docId);
        }

        private void SetReadOnly(bool ro)
        {
            TxtDocNo.IsEnabled = !ro;
            TxtInvoice.IsEnabled = !ro;
            TxtJalali.IsEnabled = !ro;
            CboSupplier.IsEnabled = !ro;
            TxtDesc.IsEnabled = !ro;
            BtnSave.IsEnabled = !ro;
        }

        private void OnSaveHeader()
        {
            if (_posted) return;
            if (string.IsNullOrWhiteSpace(TxtDocNo.Text))
            {
                MessageBox.Show("شماره سند را وارد کنید.", "انباربان"); return;
            }
            if (CboSupplier.SelectedValue == null)
            {
                MessageBox.Show("فروشنده را انتخاب کنید.", "انباربان"); return;
            }
            if (!JalaliCalendar.TryParse(TxtJalali.Text, out var dt))
            {
                MessageBox.Show("تاریخ شمسی را درست وارد کنید. مثال: 1404/01/15", "انباربان"); return;
            }
            var h = new IncomingHeader
            {
                Id = _docId,
                DocumentNumber = TxtDocNo.Text.Trim(),
                InvoiceNumber = TxtInvoice.Text.Trim(),
                DocumentDate = dt,
                SupplierId = Convert.ToInt32(CboSupplier.SelectedValue),
                Description = TxtDesc.Text.Trim()
            };
            try
            {
                _docId = AppServices.Incoming.SaveHeader(h);
                TxtStatus.Text = "ذخیره شد — اقلام را وارد کنید";
                MessageBox.Show("اطلاعات فاکتور ذخیره شد.", "انباربان");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "انباربان"); }
        }

        private void OnAddLine()
        {
            if (_docId <= 0) { MessageBox.Show("ابتدا اطلاعات فاکتور را ذخیره کنید.", "انباربان"); return; }
            if (_posted) return;
            var dlg = new LineEditorWindow(false) { Owner = Window.GetWindow(this) };
            dlg.ShowDialog();
            if (!dlg.Ok) return;
            AppServices.Incoming.AddLine(_docId, dlg.ProductId, dlg.Quantity);
            GridLines.ItemsSource = AppServices.Incoming.GetLines(_docId);
        }

        private void OnDelLine()
        {
            if (_posted || GridLines.SelectedItem is not IncomingLine line) return;
            AppServices.Incoming.DeleteLine(line.Id);
            GridLines.ItemsSource = AppServices.Incoming.GetLines(_docId);
        }

        private void OnReview()
        {
            if (_docId <= 0) return;
            var lines = AppServices.Incoming.GetLines(_docId);
            if (lines.Count == 0)
            {
                MessageBox.Show("حداقل یک قلم کالا وارد کنید.", "انباربان"); return;
            }
            var h = AppServices.Incoming.Get(_docId)!;
            var w = new ConfirmIncomingWindow(h, lines) { Owner = Window.GetWindow(this) };
            w.ShowDialog();
            if (w.Posted) LoadDoc();
        }

        private void OnUnlock()
        {
            var h = AppServices.Incoming.Get(_docId);
            if (h == null || !h.IsPosted) return;
            var code = Interaction.InputBox("کد ۶ حرفی مدیر را وارد کنید:", "انباربان", "");
            if (AppServices.Unlock.TryUnlockAndUnpost("INCOMING", _docId, h.DocumentNumber, code))
            {
                MessageBox.Show("سند برای اصلاح باز شد.", "انباربان");
                LoadDoc();
            }
            else MessageBox.Show("کد معتبر نیست یا قبلاً استفاده شده.", "انباربان");
        }

        private void OnBack() => Navigation.GoHome(this);
    }
}
