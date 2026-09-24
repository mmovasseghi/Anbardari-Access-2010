using System;
using System.Windows;
using Anbarban.Services;

namespace Anbarban
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (SampleDataSeeder.IsRequested(args))
            {
                Environment.Exit(SampleDataSeeder.RunReset(args));
                return;
            }

            if (UiSelfTestRunner.IsLive(args))
            {
                Environment.Exit(UiSelfTestRunner.RunLive(args));
                return;
            }

            if (UiSelfTestRunner.IsHeadless(args) || UiSelfTestRunner.IsVerbose(args))
            {
                Environment.Exit(UiSelfTestRunner.Run(args));
                return;
            }

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
