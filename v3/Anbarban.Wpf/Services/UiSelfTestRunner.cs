using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Anbarban.Views;

namespace Anbarban.Services
{
    /// <summary>تست UI — --ui-test (سریع) و --ui-test-live (قابل مشاهده)</summary>
    public static class UiSelfTestRunner
    {
        public static bool IsHeadless(string[] args) =>
            Array.Exists(args, a => string.Equals(a, "--ui-test", StringComparison.OrdinalIgnoreCase));

        public static bool IsLive(string[] args) =>
            Array.Exists(args, a => string.Equals(a, "--ui-test-live", StringComparison.OrdinalIgnoreCase));

        public static bool IsVerbose(string[] args) =>
            Array.Exists(args, a => string.Equals(a, "--ui-test-verbose", StringComparison.OrdinalIgnoreCase));

        public static int Run(string[] args)
        {
            Environment.SetEnvironmentVariable("ANBARBAN_UI_TEST", "1");
            if (IsVerbose(args))
                Environment.SetEnvironmentVariable("ANBARBAN_UI_TEST_VERBOSE", "1");
            var logPath = Path.Combine(
                Path.GetDirectoryName(typeof(UiSelfTestRunner).Assembly.Location) ?? ".",
                "ui-test-log.txt");
            var log = new StringBuilder();
            var host = new HeadlessUiTestHost();

            var app = new App();
            app.InitializeComponent();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var failed = UiSelfTestCore.Execute(host, log);
            log.AppendLine();
            log.AppendLine(failed == 0 ? "RESULT: PASS" : "RESULT: FAIL (" + failed + ")");
            File.WriteAllText(logPath, log.ToString(), Encoding.UTF8);
            Console.WriteLine(log.ToString());
            Console.WriteLine("Log: " + logPath);
            return failed == 0 ? 0 : 1;
        }

        public static int RunLive(string[] args)
        {
            Environment.SetEnvironmentVariable("ANBARBAN_UI_TEST", "1");
            Environment.SetEnvironmentVariable("ANBARBAN_LIVE_TEST", "1");
            var pauseMs = 2200;
            foreach (var a in args)
            {
                const string prefix = "--ui-test-live-delay=";
                if (a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(a.Substring(prefix.Length), out var ms) && ms >= 0)
                    pauseMs = ms;
            }

            var app = new App();
            app.InitializeComponent();
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var win = new LiveTestRunnerWindow(pauseMs);
            app.MainWindow = win;
            win.Show();
            app.Run();
            return win.ExitCode;
        }

        private sealed class HeadlessUiTestHost : IUiTestHost
        {
            public bool Live => false;

            public void StepNotify(UiTestStepEvent ev) { }

            public void ShowPage(Page page)
            {
                const double w = 1180, h = 780;
                var window = new Window
                {
                    Width = w,
                    Height = h,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None,
                    ShowActivated = false,
                    Visibility = Visibility.Hidden
                };
                var frame = new Frame();
                window.Content = frame;
                window.Show();
                frame.Navigate(page);
                page.Measure(new Size(w, h));
                page.Arrange(new Rect(0, 0, w, h));
                page.UpdateLayout();
                window.Close();
            }

            public void ShowElement(FrameworkElement element, string caption)
            {
                var window = new Window
                {
                    Width = 500,
                    Height = 400,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None,
                    ShowActivated = false,
                    Visibility = Visibility.Hidden
                };
                window.Content = element;
                window.Show();
                element.UpdateLayout();
                window.Close();
            }

            public void PauseBetweenSteps() { }
        }
    }
}
