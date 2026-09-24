using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class SuppliersPage : Page
    {
        private int _id;
        private List<SupplierVm> _items = new();

        public SuppliersPage()
        {
            InitializeComponent();
            Loaded += (_, __) =>
            {
                LiveComboSearch.AttachTextBox(TxtFilter, ApplyFilter);
                Reload();
            };
        }

        private void Reload()
        {
            if (!UiError.Try(() =>
                {
                    _items = AppServices.Suppliers.ListAll()
                        .Select(x => new SupplierVm(x.Id, x.Name, x.Info, x.Active)).ToList();
                    ApplyFilter();
                }, "بارگذاری فهرست فروشندگان ممکن نشد."))
                Grid.ItemsSource = _items.Count > 0 ? _items : null;
        }

        private void ApplyFilter()
        {
            var term = (TxtFilter?.Text ?? "").Trim();
            var view = string.IsNullOrEmpty(term)
                ? _items
                : _items.Where(x => x.Name.StartsWith(term, System.StringComparison.CurrentCultureIgnoreCase)
                                    || x.Name.Contains(term)).ToList();
            Grid.ItemsSource = null;
            Grid.ItemsSource = view;
        }

        private void OnSelect(object sender, SelectionChangedEventArgs e)
        {
            if (Grid.SelectedItem is not SupplierVm s) return;
            _id = s.Id;
            TxtName.Text = s.Name;
            TxtInfo.Text = s.Info;
            ChkActive.IsChecked = s.Active;
        }

        private void OnNew(object sender, RoutedEventArgs e)
        {
            _id = 0;
            TxtName.Text = TxtInfo.Text = "";
            ChkActive.IsChecked = true;
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                AnbarbanDialog.Warn("نام فروشنده را وارد کنید.", Window.GetWindow(this)); return;
            }
            if (!UiError.Try(() =>
                {
                    AppServices.Suppliers.Save(_id, TxtName.Text.Trim(), TxtInfo.Text.Trim(), ChkActive.IsChecked == true);
                }, "ذخیره فروشنده انجام نشد. پایگاه داده یا ACE را بررسی کنید."))
                return;
            Reload();
            OnNew(sender, e);
            AnbarbanDialog.Success("فروشنده ذخیره شد.", Window.GetWindow(this));
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);

        private sealed class SupplierVm
        {
            public int Id { get; }
            public string Name { get; }
            public string Info { get; }
            public bool Active { get; }
            public string ActiveLabel => Active ? "بله" : "خیر";
            public SupplierVm(int id, string name, string info, bool active) =>
                (Id, Name, Info, Active) = (id, name, info, active);
        }
    }
}
