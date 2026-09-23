using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    public partial class SuppliersPage : Page
    {
        private int _id;
        private List<SupplierVm> _items = new();

        public SuppliersPage()
        {
            InitializeComponent();
            Loaded += (_, __) => Reload();
        }

        private void Reload()
        {
            _items = AppServices.Suppliers.ListAll().Select(x => new SupplierVm(x.Id, x.Name, x.Info, x.Active)).ToList();
            Grid.ItemsSource = _items;
        }

        private void OnSelect(object sender, SelectionChangedEventArgs e)
        {
            if (Grid.SelectedItem is not SupplierVm s) return;
            _id = s.Id;
            TxtName.Text = s.Name;
            TxtInfo.Text = s.Info;
            ChkActive.IsChecked = s.Active;
        }

        private void OnNew()
        {
            _id = 0;
            TxtName.Text = TxtInfo.Text = "";
            ChkActive.IsChecked = true;
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("نام فروشنده را وارد کنید.", "انباربان"); return;
            }
            AppServices.Suppliers.Save(_id, TxtName.Text.Trim(), TxtInfo.Text.Trim(), ChkActive.IsChecked == true);
            Reload();
            MessageBox.Show("ذخیره شد.", "انباربان");
        }

        private void OnBack() => Navigation.GoHome(this);

        private sealed class SupplierVm
        {
            public int Id { get; }
            public string Name { get; }
            public string Info { get; }
            public bool Active { get; }
            public SupplierVm(int id, string name, string info, bool active) =>
                (Id, Name, Info, Active) = (id, name, info, active);
        }
    }
}
