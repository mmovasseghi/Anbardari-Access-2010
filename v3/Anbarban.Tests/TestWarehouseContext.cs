using System;
using System.IO;
using System.Linq;
using Anbarban.Data;
using Anbarban.Models;
using Anbarban.Services;

namespace Anbarban.Tests
{
    /// <summary>پایگاه Access جدا برای هر تست — بدون UI.</summary>
    public sealed class TestWarehouseContext : IDisposable
    {
        private readonly string _dbPath;

        public AccessConnectionFactory Db { get; }
        public ProductRepository Products { get; }
        public SupplierRepository Suppliers { get; }
        public DepartmentRepository Departments { get; }
        public IncomingRepository Incoming { get; }
        public OutgoingRepository Outgoing { get; }
        public ReportRepository Reports { get; }
        public DocumentPostService Post { get; }
        public StockService Stock { get; }

        public TestWarehouseContext()
        {
            _dbPath = Path.Combine(Path.GetTempPath(), "AnbarbanTest_" + Guid.NewGuid().ToString("N") + ".accdb");
            Environment.SetEnvironmentVariable("ANBARBAN_DATABASE_PATH", _dbPath);
            if (!DatabaseBootstrap.EnsureDatabase(_dbPath))
                throw new InvalidOperationException("Could not create test database (ACE/ADOX?)");

            Db = new AccessConnectionFactory();
            Products = new ProductRepository(Db);
            Suppliers = new SupplierRepository(Db);
            Departments = new DepartmentRepository(Db);
            Incoming = new IncomingRepository(Db);
            Outgoing = new OutgoingRepository(Db);
            Reports = new ReportRepository(Db);
            Post = new DocumentPostService(Db);
            Stock = new StockService(Db);
        }

        public int NewSupplier(string name = "فروشنده تست")
        {
            Suppliers.Save(0, name, "تست", true);
            var row = Suppliers.ListAll().FirstOrDefault(x => x.Name == name);
            if (row.Id == 0) throw new InvalidOperationException("supplier not saved");
            return row.Id;
        }

        public int NewDepartment(string name = "بخش تست")
        {
            Departments.Save(0, name, true);
            foreach (var d in Departments.ListActive())
                if (d.Name == name) return d.Id;
            throw new InvalidOperationException("department not saved");
        }

        public int NewProduct(string name, string code, int min = 0)
        {
            Products.Save(0, name, code, "عدد", min, true);
            var p = Products.GetByCode(code);
            if (p == null) throw new InvalidOperationException("product not saved");
            return p.Id;
        }

        public int PostIncomingDoc(int supplierId, string docNo, params (int productId, int qty)[] lines)
        {
            var docId = Incoming.SaveHeader(new IncomingHeader
            {
                DocumentNumber = docNo,
                InvoiceNumber = "F-" + docNo,
                DocumentDate = DateTime.Today,
                SupplierId = supplierId,
                Description = "test"
            });
            foreach (var (pid, qty) in lines)
                Incoming.AddLine(docId, pid, qty);
            var err = Post.PostIncoming(docId);
            if (err != null) throw new InvalidOperationException("PostIncoming: " + err);
            return docId;
        }

        public int PostOutgoingDoc(int deptId, string deliveryNo, params (int productId, int qty)[] lines)
        {
            var docId = Outgoing.SaveHeader(new OutgoingHeader
            {
                DeliveryNumber = deliveryNo,
                DocumentDate = DateTime.Today,
                Description = "test"
            });
            foreach (var (pid, qty) in lines)
                Outgoing.AddLine(docId, pid, qty, deptId);
            var err = Post.PostOutgoing(docId);
            if (err != null) throw new InvalidOperationException("PostOutgoing: " + err);
            return docId;
        }

        public void DeleteSupplierRow(int supplierId)
        {
            using var conn = Db.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Suppliers WHERE ID=?";
            cmd.Parameters.AddWithValue("@id", supplierId);
            cmd.ExecuteNonQuery();
        }

        public string? TryPostOutgoing(int deptId, string deliveryNo, params (int productId, int qty)[] lines)
        {
            var docId = Outgoing.SaveHeader(new OutgoingHeader
            {
                DeliveryNumber = deliveryNo,
                DocumentDate = DateTime.Today
            });
            foreach (var (pid, qty) in lines)
                Outgoing.AddLine(docId, pid, qty, deptId);
            return Post.PostOutgoing(docId);
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_dbPath)) File.Delete(_dbPath);
            }
            catch { /* locked */ }
        }
    }
}
