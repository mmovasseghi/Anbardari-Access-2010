using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Data;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class ProductsPage : Page
    {
        private readonly bool _lowStockOnly;
        private readonly bool _stockMode;

        public ProductsPage(bool lowStockOnly = false, bool stockMode = false)
        {
            _lowStockOnly = lowStockOnly;
            _stockMode = stockMode;
            InitializeComponent();
            if (lowStockOnly) TxtTitle.Text = "کالاهای کم‌موجودی";
            else if (stockMode) TxtTitle.Text = "مشاهده موجودی انبار";
            Loaded += (_, __) =>
            {
                if (_stockMode)
                    BtnNew.Visibility = BtnEdit.Visibility = Visibility.Collapsed;
                LiveComboSearch.AttachTextBox(TxtSearch, LoadData);
                LoadData();
            };
        }

        private void LoadData()
        {
            try
            {
                var items = AppServices.Products.Search(TxtSearch.Text);
                if (_lowStockOnly)
                    items = items.Where(p => p.CurrentStock <= p.MinimumStock).ToList();
                Grid.ItemsSource = items;
            }
            catch (Exception ex) { AnbarbanDialog.Error(ex.Message, Window.GetWindow(this)); }
        }

        private void OnSearch(object sender, RoutedEventArgs e) => LoadData();

        private void OnNew(object sender, RoutedEventArgs e)
        {
            if (_stockMode) return;
            var dlg = new ProductEditDialog(0, "", "", "عدد", 0) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true) ReloadAfterSave(dlg);
        }

        private void OnEdit(object sender, RoutedEventArgs e)
        {
            if (_stockMode || !(Grid.SelectedItem is ProductRow row)) return;
            var dlg = new ProductEditDialog(row.Id, row.Name, row.Code, row.Unit, row.MinimumStock) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true) ReloadAfterSave(dlg);
        }

        private void ReloadAfterSave(ProductEditDialog dlg)
        {
            AppServices.Products.Save(dlg.ProductId, dlg.ProductName, dlg.ProductCode, dlg.Unit, dlg.MinimumStock, true);
            LoadData();
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);
    }
}
