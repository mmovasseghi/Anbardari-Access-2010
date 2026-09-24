using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Anbarban.Data;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class MainWindow : Window
    {
        private const double DrawerWidth = 320;
        private Button? _activeNav;
        private bool _menuOpen;
        private bool _menuAnimating;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            PreviewKeyDown += (_, e) =>
            {
                if (e.Key == Key.Escape && _menuOpen)
                    CloseMenu();
            };
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = AccessConfig.GetDatabasePath();
                TxtDbStatus.Text = File.Exists(path)
                    ? "پایگاه: " + Path.GetFileName(path)
                    : "پایگاه یافت نشد";
                NavigateTo(new HomePage(), "خانه", "همان کارهایی که هر روز لازم دارید");
                SetActiveNav(NavHome);
            }
            catch (Exception ex)
            {
                AnbarbanDialog.Error(ex.Message, this);
            }
        }

        public void NavigateTo(Page page, string title, string? subtitle = null)
        {
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;
            MainFrame.Navigate(page);
            TxtPageTitle.Text = title;
            TxtPageSubtitle.Text = subtitle ?? "";
        }

        private void OnToggleMenu(object sender, RoutedEventArgs e)
        {
            if (_menuOpen) CloseMenu();
            else OpenMenu();
        }

        private void OpenMenu()
        {
            if (_menuAnimating) return;
            _menuOpen = true;
            MenuOverlay.Visibility = Visibility.Visible;
            NavDrawer.Visibility = Visibility.Visible;
            NavDrawerTransform.X = DrawerWidth;

            var slide = new DoubleAnimation(DrawerWidth, 0, TimeSpan.FromMilliseconds(260))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220));
            _menuAnimating = true;
            slide.Completed += (_, _) => _menuAnimating = false;
            NavDrawerTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, slide);
            MenuOverlay.BeginAnimation(UIElement.OpacityProperty, fade);
        }

        private void CloseMenu()
        {
            if (!_menuOpen) return;
            _menuOpen = false;
            _menuAnimating = true;

            var slide = new DoubleAnimation(0, DrawerWidth, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(180));
            slide.Completed += (_, _) =>
            {
                NavDrawer.Visibility = Visibility.Collapsed;
                MenuOverlay.Visibility = Visibility.Collapsed;
                NavDrawerTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, null);
                NavDrawerTransform.X = DrawerWidth;
                _menuAnimating = false;
            };
            NavDrawerTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, slide);
            MenuOverlay.BeginAnimation(UIElement.OpacityProperty, fade);
        }

        private void OnOverlayClick(object sender, MouseButtonEventArgs e) => CloseMenu();

        private void SetActiveNav(Button btn)
        {
            if (_activeNav != null)
                _activeNav.Style = (Style)FindResource("NavDrawerButton");
            _activeNav = btn;
            btn.Style = (Style)FindResource("NavDrawerButtonActive");
        }

        private void Nav(Button btn, Page page, string title, string subtitle)
        {
            SetActiveNav(btn);
            NavigateTo(page, title, subtitle);
            CloseMenu();
        }

        private void OnNavHome(object sender, RoutedEventArgs e) =>
            Nav(NavHome, new HomePage(), "خانه", "همان کارهایی که هر روز لازم دارید");

        private void OnNavIncoming(object sender, RoutedEventArgs e) =>
            Nav(NavIncoming, new IncomingPage(), "ثبت ورود کالا", "فاکتور خرید را اینجا می‌زنید");

        private void OnNavOutgoing(object sender, RoutedEventArgs e) =>
            Nav(NavOutgoing, new OutgoingPage(), "ثبت خروج / حواله", "کالا از انبار خارج می‌شود");

        private void OnNavStock(object sender, RoutedEventArgs e) =>
            Nav(NavStock, new ProductsPage(stockMode: true), "موجودی انبار", "ببینید الان چند تا دارید");

        private void OnNavReports(object sender, RoutedEventArgs e) =>
            Nav(NavReports, new ReportsPage(), "گزارش‌ها", "بازه و نوع را انتخاب کنید");

        private void OnNavProducts(object sender, RoutedEventArgs e) =>
            Nav(NavProducts, new ProductsPage(), "مدیریت کالاها", "کالای جدید یا ویرایش");

        private void OnNavSuppliers(object sender, RoutedEventArgs e) =>
            Nav(NavSuppliers, new SuppliersPage(), "فروشندگان", "کسی که ازش می‌خرید");

        private void OnNavDepts(object sender, RoutedEventArgs e) =>
            Nav(NavDepts, new DepartmentsPage(), "بخش‌ها", "جایی که کالا می‌رود");
    }
}
