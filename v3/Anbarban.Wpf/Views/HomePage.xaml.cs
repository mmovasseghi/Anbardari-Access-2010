using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    public partial class HomePage : Page
    {
        public HomePage() => InitializeComponent();

        private void OnIncoming() => Navigation.Go(this, new IncomingPage());
        private void OnOutgoing() => Navigation.Go(this, new OutgoingPage());
        private void OnProducts() => Navigation.Go(this, new ProductsPage());
        private void OnSuppliers() => Navigation.Go(this, new SuppliersPage());
        private void OnDepartments() => Navigation.Go(this, new DepartmentsPage());
        private void OnReports() => Navigation.Go(this, new ReportsPage());
        private void OnStock() => Navigation.Go(this, new ProductsPage(lowStockOnly: false, stockMode: true));
        private void OnLowStock() => Navigation.Go(this, new ProductsPage(lowStockOnly: true, stockMode: true));
        private void OnExit() => Application.Current.Shutdown();
    }
}
