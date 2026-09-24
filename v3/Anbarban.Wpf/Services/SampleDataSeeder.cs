using System;
using System.Data.OleDb;
using System.IO;
using Anbarban.Data;
using Anbarban.Models;

namespace Anbarban.Services
{
    /// <summary>پاک‌سازی پایگاه و پر کردن با داده شبیه انبار واقعی (میوه، پارچه، پوشاک و …).</summary>
    public static class SampleDataSeeder
    {
        public static bool IsRequested(string[] args) =>
            Array.Exists(args, a => string.Equals(a, "--reset-demo", StringComparison.OrdinalIgnoreCase));

        public static void WipeAndRecreate(string dbPath)
        {
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(dbPath))
            {
                if (!DatabaseBootstrap.EnsureDatabase(dbPath))
                    throw new IOException("ساخت پایگاه داده ممکن نشد. ACE OLEDB را بررسی کنید.");
                return;
            }

            ClearAllTables(dbPath);
        }

        public static void ClearAllTables(string dbPath)
        {
            if (!File.Exists(dbPath)) return;
            using var conn = new OleDbConnection(AccessConfig.BuildConnectionString(dbPath));
            conn.Open();
            foreach (var sql in new[]
                     {
                         "DELETE FROM IncomingItems", "DELETE FROM OutgoingItems",
                         "DELETE FROM IncomingDocuments", "DELETE FROM OutgoingDocuments",
                         "DELETE FROM UsedUnlockCodes", "DELETE FROM Products",
                         "DELETE FROM Suppliers", "DELETE FROM Departments"
                     })
                OleDbUtil.ExecuteNonQuery(conn, null, sql);
        }

        public static void SeedDemoData()
        {
            foreach (var (name, info) in Suppliers)
                AppServices.Suppliers.Save(0, name, info, true);

            foreach (var name in Departments)
                AppServices.Departments.Save(0, name, true);

            foreach (var (name, code, unit, min) in Products)
                AppServices.Products.Save(0, name, code, unit, min, true);

            var supplierIds = AppServices.Suppliers.ListActive();
            var deptIds = AppServices.Departments.ListActive();
            var products = AppServices.Products.Search(null, 500);
            if (supplierIds.Count == 0 || deptIds.Count == 0 || products.Count == 0)
                throw new InvalidOperationException("داده نمونه کامل نشد.");

            var day = DateTime.Today;
            for (var i = 1; i <= 10; i++)
            {
                var h = new IncomingHeader
                {
                    DocumentNumber = $"ورود-{i:000}",
                    InvoiceNumber = $"فاک-{1400 + i}",
                    DocumentDate = day.AddDays(-i * 2),
                    SupplierId = supplierIds[i % supplierIds.Count].Id,
                    Description = "خرید نمونه انبار"
                };
                var docId = AppServices.Incoming.SaveHeader(h);
                var pIn = products[i % products.Count];
                AppServices.Incoming.AddLine(docId, pIn.Id, 40 + i);
                for (var l = 0; l < 2; l++)
                {
                    var pExtra = products[(i + l + 1) % products.Count];
                    AppServices.Incoming.AddLine(docId, pExtra.Id, 15);
                }
                var err = AppServices.Post.PostIncoming(docId);
                if (err != null) throw new InvalidOperationException("ورود: " + err);
            }

            for (var i = 1; i <= 8; i++)
            {
                var h = new OutgoingHeader
                {
                    DeliveryNumber = $"حواله-{i:000}",
                    DocumentDate = day.AddDays(-i),
                    Description = "تحویل به بخش / فروشگاه"
                };
                var docId = AppServices.Outgoing.SaveHeader(h);
                var p = products[i % products.Count];
                AppServices.Outgoing.AddLine(docId, p.Id, 2, deptIds[i % deptIds.Count].Id);
                var err = AppServices.Post.PostOutgoing(docId);
                if (err != null) throw new InvalidOperationException("خروج: " + err);
            }
        }

        public static int RunReset(string[] args)
        {
            var path = AccessConfig.GetDatabasePath();
            try
            {
                WipeAndRecreate(path);
                SeedDemoData();
                Console.WriteLine("OK: پایگاه پاک شد و داده نمونه واقعی بارگذاری شد.");
                Console.WriteLine("مسیر: " + path);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAIL: " + ex.Message);
                return 1;
            }
        }

        private static readonly (string Name, string Info)[] Suppliers =
        {
            ("تره‌بار مرکزی بازار", "میوه و سبزی — تحویل صبح"),
            ("میز فروشی لباس", "پوشاک آماده فروش"),
            ("پارچه‌سرای نوین", "پارچه و متری"),
            ("عمده‌فروشی خشکبار امین", "آجیل و خشکبار"),
            ("پوشاک کارخانه‌ای رضوی", "لباس عمده")
        };

        private static readonly string[] Departments =
        {
            "انبار خشک",
            "سردخانه",
            "ویترین فروشگاه",
            "خط بسته‌بندی",
            "واحد ارسال"
        };

        private static readonly (string Name, string Code, string Unit, int Min)[] Products =
        {
            ("موز", "FR-001", "کیلو", 15),
            ("سیب درجه یک", "FR-002", "کیلو", 20),
            ("پرتقال", "FR-003", "کیلو", 12),
            ("گوجه فرنگی", "VG-001", "کیلو", 10),
            ("خیار", "VG-002", "کیلو", 8),
            ("پیاز", "VG-003", "کیلو", 25),
            ("سیب‌زمینی", "VG-004", "کیلو", 30),
            ("انگور", "FR-004", "کیلو", 10),
            ("هلو", "FR-005", "کیلو", 8),
            ("کاهو", "VG-005", "عدد", 15),
            ("پارچه کتان سفید", "TX-001", "متر", 50),
            ("پارچه جین آبی", "TX-002", "متر", 40),
            ("پارچه مخمل", "TX-003", "متر", 20),
            ("نخ پنبه", "TX-004", "بسته", 30),
            ("دکمه پلاستیکی", "TX-005", "بسته", 10),
            ("تیشرت مردانه سفید", "CL-001", "عدد", 25),
            ("شلوار جین", "CL-002", "عدد", 20),
            ("مانتو زنانه", "CL-003", "عدد", 15),
            ("کلاه بافتنی", "CL-004", "عدد", 10),
            ("جوراب پنبه‌ای", "CL-005", "جفت", 40),
            ("پرده مخمل", "HM-001", "متر", 15),
            ("حوله حمام", "HM-002", "عدد", 20),
            ("ظرف شیشه‌ای", "HM-003", "عدد", 12),
            ("بشقاب یکبارمصرف", "HM-004", "بسته", 25),
            ("پسته خام", "DR-001", "کیلو", 8),
            ("بادام درختی", "DR-002", "کیلو", 6),
            ("نمک تصفیه", "GR-001", "کیلو", 20),
            ("روغن آفتابگردان", "GR-002", "لیتر", 15),
            ("برنج هاشمی", "GR-003", "کیلو", 30),
            ("ماکارونی", "GR-004", "بسته", 25),
            ("شکر", "GR-005", "کیلو", 20),
            ("چای سبز", "GR-006", "بسته", 10),
            ("کالای تست UI", "UITEST", "عدد", 1)
        };
    }
}
