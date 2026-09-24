using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class DepartmentsPage : Page
    {
        private int _id;
        private List<DeptVm> _items = new();

        public DepartmentsPage()
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
            UiError.Try(() =>
            {
                _items = AppServices.Departments.ListAll()
                    .Select(d => new DeptVm(d.Id, d.Name, d.Active)).ToList();
                ApplyFilter();
            }, "بارگذاری بخش‌ها ممکن نشد.");
        }

        private void ApplyFilter()
        {
            var term = (TxtFilter?.Text ?? "").Trim();
            var view = string.IsNullOrEmpty(term)
                ? _items
                : _items.Where(d => d.Name.StartsWith(term, System.StringComparison.CurrentCultureIgnoreCase)
                                    || d.Name.Contains(term)).ToList();
            Grid.ItemsSource = null;
            Grid.ItemsSource = view;
        }

        private sealed class DeptVm
        {
            public int Id { get; }
            public string Name { get; }
            public bool Active { get; }
            public string ActiveLabel => Active ? "بله" : "خیر";
            public DeptVm(int id, string name, bool active) => (Id, Name, Active) = (id, name, active);
        }

        private void OnSelect(object sender, SelectionChangedEventArgs e)
        {
            if (!(Grid.SelectedItem is DeptVm row)) return;
            _id = row.Id;
            TxtName.Text = row.Name;
            ChkActive.IsChecked = row.Active;
        }

        private void OnNew(object sender, RoutedEventArgs e) { _id = 0; TxtName.Text = ""; ChkActive.IsChecked = true; }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                AnbarbanDialog.Warn("نام بخش را وارد کنید.", Window.GetWindow(this)); return;
            }
            if (!UiError.Try(() =>
                    AppServices.Departments.Save(_id, TxtName.Text.Trim(), ChkActive.IsChecked == true),
                "ذخیره بخش انجام نشد."))
                return;
            Reload();
            OnNew(sender, e);
            AnbarbanDialog.Success("بخش ذخیره شد.", Window.GetWindow(this));
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);
    }
}
