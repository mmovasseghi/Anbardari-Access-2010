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
            Grid.ItemsSource = AppServices.Departments.ListAll().Select(d => new { Id = d.Id, Name = d.Name, Active = d.Active }).ToList();

        private void OnSelect(object sender, SelectionChangedEventArgs e)
        {
            dynamic? row = Grid.SelectedItem;
            if (row == null) return;
            _id = row.Id;
            TxtName.Text = row.Name;
            ChkActive.IsChecked = row.Active;
        }

        private void OnNew() { _id = 0; TxtName.Text = ""; ChkActive.IsChecked = true; }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("نام بخش را وارد کنید.", "انباربان"); return;
            }
            AppServices.Departments.Save(_id, TxtName.Text.Trim(), ChkActive.IsChecked == true);
            Reload();
            MessageBox.Show("ذخیره شد.", "انباربان");
        }

        private void OnBack() => Navigation.GoHome(this);
    }
}
