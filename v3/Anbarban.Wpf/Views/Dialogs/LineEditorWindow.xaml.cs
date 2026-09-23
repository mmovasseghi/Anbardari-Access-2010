using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Models;

namespace Anbarban.Views.Dialogs
{
    public partial class LineEditorWindow : Window
    {
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public int DepartmentId { get; private set; }
        public bool Ok { get; private set; }

        public LineEditorWindow(bool includeDepartment, int? productId = null, int? qty = null, int? deptId = null)
        {
            InitializeComponent();
            if (includeDepartment)
            {
                DeptPanel.Visibility = Visibility.Visible;
                CboDept.ItemsSource = AppServices.Departments.ListActive();
            }
            var products = AppServices.Products.Search(null, 500);
            CboProduct.ItemsSource = products.Select(p => new IdName { Id = p.Id, Name = p.Name + (string.IsNullOrEmpty(p.Code) ? "" : " (" + p.Code + ")") }).ToList();
            CboProduct.SelectionChanged += (_, __) => FilterProducts();
            CboProduct.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler((_, __) => FilterProducts()), true);
            if (productId.HasValue) CboProduct.SelectedValue = productId;
            if (qty.HasValue) TxtQty.Text = qty.Value.ToString();
            if (deptId.HasValue) CboDept.SelectedValue = deptId;
        }

        private void FilterProducts()
        {
            var t = (CboProduct.Text ?? "").Trim();
            if (t.Length < 1) return;
            var products = AppServices.Products.Search(t, 80);
            CboProduct.ItemsSource = products.Select(p => new IdName { Id = p.Id, Name = p.Name }).ToList();
        }

        private void OnCancel() => Close();

        private void OnOk()
        {
            if (CboProduct.SelectedValue is int pid) ProductId = pid;
            else if (!int.TryParse(CboProduct.SelectedValue?.ToString(), out ProductId) || ProductId <= 0)
            {
                MessageBox.Show("کالا را انتخاب کنید.", "انباربان"); return;
            }
            if (!int.TryParse(TxtQty.Text, out var q) || q <= 0)
            {
                MessageBox.Show("تعداد باید بیشتر از صفر باشد.", "انباربان"); return;
            }
            Quantity = q;
            if (DeptPanel.Visibility == Visibility.Visible)
            {
                if (CboDept.SelectedValue is int d) DepartmentId = d;
                else if (!int.TryParse(CboDept.SelectedValue?.ToString(), out DepartmentId) || DepartmentId <= 0)
                {
                    MessageBox.Show("بخش را انتخاب کنید.", "انباربان"); return;
                }
            }
            Ok = true;
            Close();
        }
    }
}
