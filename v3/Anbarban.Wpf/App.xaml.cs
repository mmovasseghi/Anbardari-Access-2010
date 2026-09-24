using System;
using System.Windows;
using System.Windows.Markup;
using Anbarban.Data;
using Anbarban.Services;
using Anbarban.Views;

namespace Anbarban
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += (_, args) =>
            {
                UiError.Show(args.Exception, "یک خطا پیش آمد؛ برنامه باز می‌ماند.");
                args.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                    UiError.Show(ex, "برنامه با خطا بسته می‌شود.");
            };

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage("fa-IR")));

            base.OnStartup(e);

            if (string.Equals(Environment.GetEnvironmentVariable("ANBARBAN_LIVE_TEST"), "1", StringComparison.Ordinal))
                return;

            try
            {
                var path = AccessConfig.GetDatabasePath();
                if (!DatabaseBootstrap.EnsureDatabase(path))
                {
                    AnbarbanDialog.Warn(AccessConnectionFactory.BuildHelpMessage(path), null, "راه‌اندازی پایگاه");
                }
            }
            catch (Exception ex)
            {
                AnbarbanDialog.Error(ex.Message, null);
            }

            var main = new MainWindow();
            MainWindow = main;
            main.Show();
        }
    }
}
