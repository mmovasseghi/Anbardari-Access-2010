using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    public partial class HomePage : Page
    {
        public HomePage() => InitializeComponent();

        private void OnIncoming(object sender, RoutedEventArgs e) => Navigation.Go(this, new IncomingPage());
        private void OnOutgoing(object sender, RoutedEventArgs e) => Navigation.Go(this, new OutgoingPage());
        private void OnReports(object sender, RoutedEventArgs e) => Navigation.Go(this, new ReportsPage());
        private void OnStock(object sender, RoutedEventArgs e) => Navigation.Go(this, new ProductsPage(stockMode: true));
        private void OnSuppliers(object sender, RoutedEventArgs e) => Navigation.Go(this, new SuppliersPage());
        private void OnDepartments(object sender, RoutedEventArgs e) => Navigation.Go(this, new DepartmentsPage());
        private void OnProducts(object sender, RoutedEventArgs e) => Navigation.Go(this, new ProductsPage());
    }
}
