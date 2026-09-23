using System.Windows;
using System.Windows.Controls;

namespace Anbarban.Views
{
    public partial class HomePage : Page
    {
        public HomePage() => InitializeComponent();

        private MainWindow? Shell => Window.GetWindow(this) as MainWindow;

        private void OnIncoming() => Shell?.NavigateTo(new PlaceholderPage("ثبت ورود کالا", "صفحه ورود دو مرحله‌ای — در حال پیاده‌سازی v3"));
        private void OnOutgoing() => Shell?.NavigateTo(new PlaceholderPage("ثبت خروج / حواله", "صفحه خروج دو مرحله‌ای — در حال پیاده‌سازی v3"));
        private void OnProducts() => Shell?.NavigateTo(new ProductsPage());
        private void OnSuppliers() => Shell?.NavigateTo(new PlaceholderPage("فروشندگان", "به‌زودی"));
        private void OnDepartments() => Shell?.NavigateTo(new PlaceholderPage("بخش‌ها", "به‌زودی"));
        private void OnReports() => Shell?.NavigateTo(new PlaceholderPage("گزارش‌ها", "به‌زودی"));
        private void OnStock() => Shell?.NavigateTo(new ProductsPage());
        private void OnLowStock() => Shell?.NavigateTo(new ProductsPage(lowStockOnly: true));

        private void OnExit() => Application.Current.Shutdown();
    }
}
