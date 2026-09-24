using System;
using System.IO;
using Anbarban;
using Anbarban.Data;
using Anbarban.Models;

static class Program
{
    static int Main()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), "AnbarbanSmoke_" + Guid.NewGuid().ToString("N") + ".accdb");
        try
        {
            Environment.SetEnvironmentVariable("ANBARBAN_DATABASE_PATH", dbPath);
            if (!DatabaseBootstrap.EnsureDatabase(dbPath))
            {
                Console.WriteLine("FAIL: could not create database (ACE/ADOX missing?)");
                return 1;
            }

            RunScenario();
            Console.WriteLine("OK: smoke test passed");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("FAIL: " + ex);
            return 2;
        }
        finally
        {
            try { if (File.Exists(dbPath)) File.Delete(dbPath); } catch { /* locked */ }
        }
    }

    static void RunScenario()
    {
        for (var i = 1; i <= 5; i++)
            AppServices.Suppliers.Save(0, $"فروشنده تست {i}", $"اطلاعات {i}", true);

        for (var i = 1; i <= 5; i++)
            AppServices.Departments.Save(0, $"بخش {i}", true);

        var supplierIds = AppServices.Suppliers.ListActive();
        var deptIds = AppServices.Departments.ListActive();
        if (supplierIds.Count < 5) throw new Exception("expected 5 suppliers");

        for (var i = 1; i <= 30; i++)
            AppServices.Products.Save(0, $"کالا {i}", $"C{i:D4}", "عدد", i % 5, true);

        var products = AppServices.Products.Search(null);
        if (products.Count < 30) throw new Exception("expected 30 products");

        for (var d = 1; d <= 10; d++)
        {
            var h = new IncomingHeader
            {
                DocumentNumber = $"IN-{d:D3}",
                InvoiceNumber = $"F-{d}",
                DocumentDate = DateTime.Today,
                SupplierId = supplierIds[d % supplierIds.Count].Id,
                Description = "تست ورود"
            };
            var docId = AppServices.Incoming.SaveHeader(h);
            for (var l = 0; l < 2; l++)
                AppServices.Incoming.AddLine(docId, products[(d + l) % products.Count].Id, 10 + d);
            var err = AppServices.Post.PostIncoming(docId);
            if (err != null) throw new Exception("post incoming: " + err);
        }

        for (var d = 1; d <= 8; d++)
        {
            var h = new OutgoingHeader
            {
                DeliveryNumber = $"H-{d}",
                DocumentDate = DateTime.Today,
                Description = "تست خروج"
            };
            var docId = AppServices.Outgoing.SaveHeader(h);
            AppServices.Outgoing.AddLine(docId, products[d % products.Count].Id, 5, deptIds[d % deptIds.Count].Id);
            var err = AppServices.Post.PostOutgoing(docId);
            if (err != null) throw new Exception("post outgoing: " + err);
        }

        AppServices.Suppliers.Save(0, "فروشنده بعد از تست", "", true);
        if (AppServices.Suppliers.ListAll().Count < 6) throw new Exception("supplier list after insert failed");
    }
}
