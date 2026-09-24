using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Anbarban.Services;

namespace Anbarban.Views
{
    public partial class LiveTestRunnerWindow : Window, IUiTestHost
    {
        private readonly Dictionary<string, ListBoxItem> _stepItems = new Dictionary<string, ListBoxItem>();
        private readonly int _pauseMs;
        private readonly StringBuilder _log = new();
        public int ExitCode { get; private set; }

        public bool Live => true;

        private bool _running;

        public LiveTestRunnerWindow(int pauseMs = 1400)
        {
            _pauseMs = pauseMs;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PreviewFrame.Visibility = Visibility.Visible;
            PreviewHost.Visibility = Visibility.Collapsed;
            Topmost = true;
            Activate();
            Focus();
            TxtCurrent.Text = "منتظر شروع شما — دکمه «شروع تست زنده» را بزنید.";
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            if (_running) return;
            _running = true;
            StartOverlay.Visibility = Visibility.Collapsed;
            BtnStart.IsEnabled = false;
            BtnClose.Visibility = Visibility.Collapsed;
            BtnAgain.Visibility = Visibility.Collapsed;
            TxtResult.Text = "";
            StepsList.Items.Clear();
            _stepItems.Clear();
            _log.Clear();
            Topmost = true;
            Activate();
            Dispatcher.BeginInvoke(new Action(RunSequence), DispatcherPriority.ApplicationIdle);
        }

        private void RunSequence()
        {
            try
            {
                var failed = UiSelfTestCore.Execute(this, _log);
                _log.AppendLine();
                _log.AppendLine(failed == 0 ? "RESULT: PASS" : "RESULT: FAIL (" + failed + ")");
                var logPath = Path.Combine(
                    Path.GetDirectoryName(typeof(LiveTestRunnerWindow).Assembly.Location) ?? ".",
                    "ui-test-log.txt");
                File.WriteAllText(logPath, _log.ToString(), Encoding.UTF8);
                ExitCode = failed == 0 ? 0 : 1;
                TxtResult.Text = failed == 0
                    ? "✓ همه مراحل موفق — لاگ: " + logPath
                    : "✗ " + failed + " خطا — لاگ: " + logPath;
                TxtResult.Foreground = failed == 0
                    ? new SolidColorBrush(Color.FromRgb(0x6F, 0xC9, 0x8F))
                    : new SolidColorBrush(Color.FromRgb(0xE8, 0x8C, 0x6C));
                BtnClose.Visibility = Visibility.Visible;
                BtnAgain.Visibility = Visibility.Visible;
                BtnStart.IsEnabled = true;
                Topmost = false;
            }
            catch (Exception ex)
            {
                UiError.Log(ex, "LiveTestRunner");
                ExitCode = 1;
                TxtResult.Text = "خطای غیرمنتظره: " + ex.Message;
                BtnClose.Visibility = Visibility.Visible;
                BtnAgain.Visibility = Visibility.Visible;
                BtnStart.IsEnabled = true;
                Topmost = false;
            }
            finally
            {
                _running = false;
            }
        }

        public void StepNotify(UiTestStepEvent ev)
        {
            Dispatcher.Invoke(() =>
            {
                TxtCurrent.Text = ev.State switch
                {
                    UiTestStepState.Running => "▶ در حال اجرا: " + ev.Name,
                    UiTestStepState.Ok => "✓ انجام شد: " + ev.Name,
                    UiTestStepState.Fail => "✗ خطا: " + ev.Name + (string.IsNullOrEmpty(ev.Detail) ? "" : " — " + ev.Detail),
                    _ => ev.Name
                };

                if (!_stepItems.TryGetValue(ev.Name, out var item))
                {
                    item = new ListBoxItem { Content = ev.Name, Tag = ev.Name };
                    _stepItems[ev.Name] = item;
                    StepsList.Items.Add(item);
                }

                item.Foreground = ev.State switch
                {
                    UiTestStepState.Ok => new SolidColorBrush(Color.FromRgb(0x6F, 0xC9, 0x8F)),
                    UiTestStepState.Fail => new SolidColorBrush(Color.FromRgb(0xE8, 0x8C, 0x6C)),
                    UiTestStepState.Running => new SolidColorBrush(Color.FromRgb(0xD4, 0xAF, 0x37)),
                    _ => new SolidColorBrush(Color.FromRgb(0xA8, 0xB5, 0xB0))
                };
                if (ev.State == UiTestStepState.Ok)
                    item.Content = "✓ " + ev.Name;
                else if (ev.State == UiTestStepState.Fail)
                    item.Content = "✗ " + ev.Name;
                else if (ev.State == UiTestStepState.Running)
                    item.Content = "▶ " + ev.Name;

                StepsList.SelectedItem = item;
                StepsList.ScrollIntoView(item);
            });
        }

        public void ShowPage(Page page)
        {
            Dispatcher.Invoke(() =>
            {
                PreviewHost.Visibility = Visibility.Collapsed;
                PreviewHost.Content = null;
                PreviewFrame.Visibility = Visibility.Visible;
                TxtPreviewCaption.Text = "پیش‌نمایش صفحه";
                PreviewFrame.Navigate(page);
                page.UpdateLayout();
            });
        }

        public void ShowElement(FrameworkElement element, string caption)
        {
            Dispatcher.Invoke(() =>
            {
                PreviewFrame.Visibility = Visibility.Collapsed;
                PreviewHost.Visibility = Visibility.Visible;
                TxtPreviewCaption.Text = caption;
                PreviewHost.Content = element;
                element.UpdateLayout();
            });
        }

        public void PauseBetweenSteps()
        {
            if (_pauseMs <= 0) return;
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_pauseMs) };
            timer.Tick += (_, __) =>
            {
                timer.Stop();
                frame.Continue = false;
            };
            timer.Start();
            Dispatcher.PushFrame(frame);
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(ExitCode);
        }
    }
}
