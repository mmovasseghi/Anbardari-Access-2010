using System;
using System.Windows;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class ProductEditDialog : Window
    {
        public int ProductId { get; private set; }
        public string ProductName => TxtName.Text.Trim();
        public string ProductCode => TxtCode.Text.Trim();
        public string Unit => TxtUnit.Text.Trim();
        public int MinimumStock { get; private set; }

        public ProductEditDialog(int id, string name, string code, string unit, int min)
        {
            InitializeComponent();
            ProductId = id;
            TxtName.Text = name;
            TxtCode.Text = code;
            TxtUnit.Text = unit;
            TxtMin.Text = min.ToString();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                AnbarbanDialog.Warn("نام کالا الزامی است.", this); return;
            }
            if (!int.TryParse(TxtMin.Text, out var m) || m < 0)
            {
                AnbarbanDialog.Warn("حداقل موجودی نامعتبر است.", this); return;
            }
            MinimumStock = m;
            DialogResult = true;
        }
    }
}
