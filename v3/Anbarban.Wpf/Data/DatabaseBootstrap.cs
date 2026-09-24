using System;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;
namespace Anbarban.Data
{
    /// <summary>ساخت Inventory.accdb فقط با جداول — بدون Access و بدون frmMain.</summary>
    public static class DatabaseBootstrap
    {
        public static bool EnsureDatabase(string dbPath)
        {
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(dbPath))
            {
                if (!TryCreateEmptyFile(dbPath))
                    return false;
            }

            if (!SchemaExists(dbPath))
                CreateSchema(dbPath);
            else
                ApplyMigrations(dbPath);

            return File.Exists(dbPath) && SchemaExists(dbPath);
        }

        private static void ApplyMigrations(string dbPath)
        {
            try
            {
                using var conn = new OleDbConnection(AccessConfig.BuildConnectionString(dbPath));
                conn.Open();
                TryDropColumn(conn, "OutgoingDocuments", "DocumentNumber");
            }
            catch
            {
                // ignore — migration best-effort
            }
        }

        private static void TryDropColumn(OleDbConnection conn, string table, string column)
        {
            try
            {
                OleDbUtil.ExecuteNonQuery(conn, null, $"ALTER TABLE {table} DROP COLUMN {column}");
            }
            catch
            {
                // column already removed or unsupported
            }
        }

        public static bool SchemaExists(string dbPath)
        {
            try
            {
                using var conn = new OleDbConnection(AccessConfig.BuildConnectionString(dbPath));
                conn.Open();
                using var cmd = new OleDbCommand("SELECT TOP 1 ID FROM Products", conn);
                cmd.ExecuteScalar();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>ساخت فایل accdb خالی با یک provider مشخص یا اولین ACE در دسترس.</summary>
        public static bool TryCreateEmptyFile(string dbPath, string? provider = null)
        {
            var candidates = provider != null
                ? new[] { provider }
                : new[]
                {
                    "Microsoft.ACE.OLEDB.16.0",
                    "Microsoft.ACE.OLEDB.12.0",
                    "Microsoft.ACE.OLEDB.15.0"
                };

            var catalogType = Type.GetTypeFromProgID("ADOX.Catalog");
            if (catalogType == null) return false;

            foreach (var p in candidates)
            {
                try
                {
                    dynamic catalog = Activator.CreateInstance(catalogType)!;
                    var connStr = $"Provider={p};Data Source={dbPath};";
                    catalog.Create(connStr);
                    Marshal.ReleaseComObject(catalog);
                    return true;
                }
                catch
                {
                    try { if (File.Exists(dbPath)) File.Delete(dbPath); } catch { /* ignore */ }
                }
            }

            return false;
        }

        private static void CreateSchema(string dbPath)
        {
            using var conn = new OleDbConnection(AccessConfig.BuildConnectionString(dbPath));
            conn.Open();
            foreach (var sql in SchemaSql)
                OleDbUtil.ExecuteNonQuery(conn, null, sql);
        }

        private static readonly string[] SchemaSql =
        {
            "CREATE TABLE Products (ID COUNTER PRIMARY KEY, ProductName TEXT(100), ProductCode TEXT(50), Unit TEXT(20), CurrentStock LONG, MinimumStock LONG, IsActive YESNO)",
            "CREATE TABLE Suppliers (ID COUNTER PRIMARY KEY, SupplierName TEXT(100), SupplierInfo TEXT(255), IsActive YESNO)",
            "CREATE TABLE Departments (ID COUNTER PRIMARY KEY, DepartmentName TEXT(100), IsActive YESNO)",
            "CREATE TABLE IncomingDocuments (ID COUNTER PRIMARY KEY, DocumentNumber TEXT(50), InvoiceNumber TEXT(50), DocumentDate DATETIME, SupplierID LONG, Description TEXT(255), IsPosted YESNO, PostedAt DATETIME)",
            "CREATE TABLE IncomingItems (ID COUNTER PRIMARY KEY, IncomingDocumentID LONG, ProductID LONG, Quantity LONG)",
            "CREATE TABLE OutgoingDocuments (ID COUNTER PRIMARY KEY, DeliveryNumber TEXT(50), DocumentDate DATETIME, Description TEXT(255), IsPosted YESNO, PostedAt DATETIME)",
            "CREATE TABLE OutgoingItems (ID COUNTER PRIMARY KEY, OutgoingDocumentID LONG, ProductID LONG, Quantity LONG, DepartmentID LONG)",
            "CREATE TABLE UsedUnlockCodes (ID COUNTER PRIMARY KEY, UnlockCode TEXT(6), DocKind TEXT(10), DocumentID LONG, UsedAt DATETIME)"
        };
    }
}
