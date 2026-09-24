using System.Windows;
using Anbarban.Services;
using Anbarban.Views;

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
                LiveComboSearch.AttachDepartments(CboDept);
            }
            LiveComboSearch.AttachProductsAsIdName(CboProduct, includeCode: true);
            if (productId.HasValue)
            {
                var p = AppServices.Products.GetById(productId.Value);
                if (p != null)
                {
                    var label = p.Name + (string.IsNullOrEmpty(p.Code) ? "" : " (" + p.Code + ")");
                    LiveComboSearch.SetSelectedId(CboProduct, p.Id, label);
                }
            }
            if (qty.HasValue) TxtQty.Text = qty.Value.ToString();
            if (deptId.HasValue)
            {
                foreach (var d in AppServices.Departments.ListActive())
                {
                    if (d.Id == deptId.Value)
                    {
                        LiveComboSearch.SetSelectedId(CboDept, d.Id, d.Name);
                        break;
                    }
                }
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e) => Close();

        private void OnOk(object sender, RoutedEventArgs e)
        {
            if (CboProduct.SelectedId <= 0)
            {
                AnbarbanDialog.Warn("کالا را از لیست انتخاب کنید.", this); return;
            }
            ProductId = CboProduct.SelectedId;

            if (!int.TryParse(TxtQty.Text, out var q) || q <= 0)
            {
                AnbarbanDialog.Warn("تعداد باید بیشتر از صفر باشد.", this); return;
            }
            Quantity = q;
            if (DeptPanel.Visibility == Visibility.Visible)
            {
                if (CboDept.SelectedId <= 0)
                {
                    AnbarbanDialog.Warn("بخش را از لیست انتخاب کنید.", this); return;
                }
                DepartmentId = CboDept.SelectedId;
            }
            Ok = true;
            Close();
        }
    }
}
