using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Anbarban.Controls;
using Anbarban.Data;
using Anbarban.Models;
using Anbarban.Views;
using Anbarban.Views.Dialogs;

namespace Anbarban.Services
{
    public enum UiTestStepState { Pending, Running, Ok, Fail }

    public sealed class UiTestStepEvent
    {
        public string Name { get; set; } = "";
        public UiTestStepState State { get; set; }
        public string Detail { get; set; } = "";
    }

    /// <summary>محیط اجرای تست — headless یا live.</summary>
    public interface IUiTestHost
    {
        void StepNotify(UiTestStepEvent ev);
        void ShowPage(Page page);
        void ShowElement(FrameworkElement element, string caption);
        void PauseBetweenSteps();
        bool Live { get; }
    }

    public static class UiSelfTestCore
    {
        public static int Execute(IUiTestHost host, StringBuilder log)
        {
            var failed = 0;
            int uiTestProductId;

            void RunStep(string name, Action body)
            {
                var verbose = string.Equals(Environment.GetEnvironmentVariable("ANBARBAN_UI_TEST_VERBOSE"), "1",
                    StringComparison.Ordinal);
                if (verbose)
                {
                    Console.WriteLine("[STEP] ▶ " + name);
                    Console.Out.Flush();
                }
                host.StepNotify(new UiTestStepEvent { Name = name, State = UiTestStepState.Running, Detail = "" });
                try
                {
                    body();
                    log.AppendLine("OK  " + name);
                    if (verbose)
                    {
                        Console.WriteLine("[STEP] ✓ " + name);
                        Console.Out.Flush();
                    }
                    host.StepNotify(new UiTestStepEvent { Name = name, State = UiTestStepState.Ok, Detail = "" });
                }
                catch (Exception ex)
                {
                    failed++;
                    UiError.Log(ex, "ui-test: " + name);
                    log.AppendLine("FAIL " + name + ": " + ex.GetType().Name + " — " + ex.Message);
                    log.AppendLine(ex.StackTrace);
                    host.StepNotify(new UiTestStepEvent
                    {
                        Name = name,
                        State = UiTestStepState.Fail,
                        Detail = ex.Message
                    });
                    if (host.Live)
                        host.PauseBetweenSteps();
                }
                host.PauseBetweenSteps();
            }

            RunStep("پاک‌سازی DB و داده نمونه", () =>
            {
                var dbPath = AccessConfig.GetDatabasePath();
                SampleDataSeeder.WipeAndRecreate(dbPath);
                SampleDataSeeder.SeedDemoData();
                log.AppendLine("DB  " + dbPath + " exists=" + File.Exists(dbPath));
            });

            var pages = new List<(string Title, Func<Page> Factory)>
            {
                ("خانه", () => new HomePage()),
                ("فروشندگان", () => new SuppliersPage()),
                ("بخش‌ها", () => new DepartmentsPage()),
                ("کالاها", () => new ProductsPage()),
                ("کم‌موجودی", () => new ProductsPage(lowStockOnly: true, stockMode: true)),
                ("ورود کالا", () => new IncomingPage()),
                ("خروج کالا", () => new OutgoingPage()),
                ("گزارش‌ها", () => new ReportsPage())
            };
            foreach (var (title, factory) in pages)
            {
                RunStep("باز کردن صفحه: " + title, () =>
                {
                    host.ShowPage(factory());
                });
            }

            RunStep("ذخیره فروشنده", () =>
            {
                var name = "فروشنده تست UI " + DateTime.Now.ToString("HHmmss");
                AppServices.Suppliers.Save(0, name, "تست خودکار", true);
                var n = AppServices.Suppliers.ListAll().Count;
                if (n < 1) throw new InvalidOperationException("supplier count=" + n);
            });

            uiTestProductId = 0;
            RunStep("ذخیره کالای UITEST", () =>
            {
                AppServices.Products.Save(0, "کالای تست UI", "UITEST", "عدد", 1, true);
                var row = AppServices.Products.GetByCode("UITEST");
                if (row == null) throw new InvalidOperationException("UITEST not found");
                uiTestProductId = row.Id;
            });

            RunStep("UI: ورود — ذخیره فاکتور، قلم، بررسی و ثبت نهایی", () =>
            {
                if (uiTestProductId <= 0) throw new InvalidOperationException("no UITEST id");
                UiTestAutomation.RunIncomingFullFlow(host);
            });

            RunStep("UI: خروج — بررسی و ثبت نهایی", () =>
            {
                if (uiTestProductId <= 0) throw new InvalidOperationException("no UITEST id");
                var avail = AppServices.Stock.GetCurrentStock(uiTestProductId);
                if (avail < 1) throw new InvalidOperationException("stock=" + avail);
                UiTestAutomation.RunOutgoingReviewFlow(host, uiTestProductId);
            });

            RunStep("UI: گزارش‌ها — دکمه گزارش‌گیری و نمایش موجودی", () =>
            {
                var page = new ReportsPage();
                host.ShowPage(page);
                page.UpdateLayout();
                if (!page.UiTest_IsRunButtonVisible())
                    throw new InvalidOperationException("دکمه گزارش‌گیری در صفحه نیست یا بیرون کادر است");
                var rows = page.UiTest_RunStockReport();
                if (rows < 1)
                    throw new InvalidOperationException("گزارش موجودی خالی است (بعد از داده نمونه)");
            });

            RunStep("باز شدن لیست کمبوباکس (▼)", () =>
            {
                var combo = new SearchComboBox();
                LiveComboSearch.AttachProducts(combo, 15);
                host.ShowElement(combo, "دکمه ▼ — لیست بدون تایپ");
                combo.BtnDrop.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                combo.UpdateLayout();
                if (combo.LstSuggest.Items.Count == 0)
                    throw new InvalidOperationException("browse list empty");
            });

            RunStep("جستجوی زنده کمبوباکس (ان)", () =>
            {
                var combo = new SearchComboBox();
                LiveComboSearch.AttachProducts(combo, 15);
                host.ShowElement(combo, "تایپ «ان» در کمبوباکس");
                combo.TxtQuery.Text = "ان";
                combo.FlushPendingSearch();
                combo.UpdateLayout();
                if (combo.LstSuggest.Items.Count == 0)
                    throw new InvalidOperationException("no suggestions");
            });

            RunStep("تقویم شمسی (امروز)", () =>
            {
                var picker = new JalaliDatePicker();
                host.ShowElement(picker, "تقویم شمسی");
                picker.SetGregorian(DateTime.Today);
                picker.UpdateLayout();
                if (!picker.TryGetGregorian(out _))
                    throw new InvalidOperationException("parse failed");
            });

            RunStep("پنجره قلم کالا (ان + انتخاب)", () =>
            {
                var dlg = new LineEditorWindow(includeDepartment: true);
                if (host.Live)
                {
                    dlg.Owner = Application.Current?.MainWindow;
                    dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    dlg.Show();
                }
                else
                {
                    dlg.ShowInTaskbar = false;
                    dlg.WindowStartupLocation = WindowStartupLocation.Manual;
                    dlg.Left = dlg.Top = -10000;
                    dlg.Show();
                }
                dlg.CboProduct.TxtQuery.Text = "ان";
                dlg.CboProduct.FlushPendingSearch();
                dlg.UpdateLayout();
                if (dlg.CboProduct.LstSuggest.Items.Count == 0)
                    throw new InvalidOperationException("no suggestions for ان");
                if (dlg.CboProduct.LstSuggest.Items[0] is not IdName pick)
                    throw new InvalidOperationException("bad item type");
                dlg.CboProduct.SetSelection(pick.Id, pick.Name);
                dlg.TxtQty.Text = "1";
                if (dlg.CboDept.SelectedId <= 0)
                {
                    var d0 = AppServices.Departments.ListActive();
                    if (d0.Count > 0)
                        LiveComboSearch.SetSelectedId(dlg.CboDept, d0[0].Id, d0[0].Name);
                }
                host.PauseBetweenSteps();
                dlg.Close();
            });

            return failed;
        }
    }
}
