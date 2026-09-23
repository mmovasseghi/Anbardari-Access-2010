using System;
using System.Windows;
using Anbarban.Data;

namespace Anbarban
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                var path = AccessConfig.GetDatabasePath();
                if (!DatabaseBootstrap.EnsureDatabase(path))
                {
                    MessageBox.Show(AccessConnectionFactory.BuildHelpMessage(path), "انباربان",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "انباربان", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
