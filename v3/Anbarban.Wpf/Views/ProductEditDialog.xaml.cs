using System;
using System.Windows;

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

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("نام کالا الزامی است.", "انباربان"); return;
            }
            if (!int.TryParse(TxtMin.Text, out var m) || m < 0)
            {
                MessageBox.Show("حداقل موجودی نامعتبر است.", "انباربان"); return;
            }
            MinimumStock = m;
            DialogResult = true;
        }
    }
}
