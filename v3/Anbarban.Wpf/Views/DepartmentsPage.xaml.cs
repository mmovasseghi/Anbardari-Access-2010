using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    public partial class DepartmentsPage : Page
    {
        private int _id;

        public DepartmentsPage()
        {
            InitializeComponent();
            Loaded += (_, __) => Reload();
        }

        private void Reload() =>
            Grid.ItemsSource = AppServices.Departments.ListAll()
                .Select(d => new DeptVm(d.Id, d.Name, d.Active)).ToList();

        private sealed class DeptVm
        {
            public int Id { get; }
            public string Name { get; }
            public bool Active { get; }
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
                MessageBox.Show("نام بخش را وارد کنید.", "انباربان"); return;
            }
            AppServices.Departments.Save(_id, TxtName.Text.Trim(), ChkActive.IsChecked == true);
            Reload();
            MessageBox.Show("ذخیره شد.", "انباربان");
        }

        private void OnBack(object sender, RoutedEventArgs e) => Navigation.GoHome(this);
    }
}
