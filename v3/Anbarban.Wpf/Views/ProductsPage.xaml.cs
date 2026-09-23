using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Data;

namespace Anbarban.Views
{
    public partial class ProductsPage : Page
    {
        private readonly bool _lowStockOnly;
        private readonly ProductRepository _repo;

        public ProductsPage(bool lowStockOnly = false)
        {
            _lowStockOnly = lowStockOnly;
            InitializeComponent();
            if (lowStockOnly)
                TxtTitle.Text = "کالاهای کم‌موجودی";
            _repo = new ProductRepository(new AccessConnectionFactory());
            Loaded += (_, __) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                var items = _repo.Search(TxtSearch.Text);
                if (_lowStockOnly)
                    items = items.Where(p => p.CurrentStock <= p.MinimumStock).ToList();
                Grid.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "انباربان", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSearch() => LoadData();

        private void OnBack()
        {
            if (Window.GetWindow(this) is MainWindow w)
                w.NavigateTo(new HomePage());
        }
    }
}
