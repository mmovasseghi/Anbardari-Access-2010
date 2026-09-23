using System;
using System.IO;
using System.Windows;
using Anbarban.Data;

namespace Anbarban.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = AccessConfig.GetDatabasePath();
                TxtDbStatus.Text = File.Exists(path)
                    ? "پایگاه: " + Path.GetFileName(path)
                    : "پایگاه یافت نشد — Data\\Inventory.accdb";
                MainFrame.Navigate(new HomePage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "انباربان", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void NavigateTo(Page page) => MainFrame.Navigate(page);
    }
}
