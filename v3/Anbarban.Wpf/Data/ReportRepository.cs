using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace Anbarban.Data
{
    public sealed class ReportRepository
    {
        private readonly AccessConnectionFactory _db;
        public ReportRepository(AccessConnectionFactory db) => _db = db;

        public DataTable RunStock(bool lowOnly)
        {
            var sql = lowOnly
                ? @"SELECT ProductCode AS [کد], ProductName AS [نام], Unit AS [واحد], CurrentStock AS [موجودی], MinimumStock AS [حداقل]
                    FROM Products WHERE IsActive=True AND CurrentStock<=MinimumStock ORDER BY ProductName"
                : @"SELECT ProductCode AS [کد], ProductName AS [نام], Unit AS [واحد], CurrentStock AS [موجودی], MinimumStock AS [حداقل]
                    FROM Products WHERE IsActive=True ORDER BY ProductName";
            return Fill(sql);
        }

        public DataTable RunMovement(int productId, DateTime? from, DateTime? to)
        {
            var sql = @"
SELECT DocDate AS [تاریخ], [نوع], [تعداد], [طرف], [شماره سند], [توضیحات] FROM (
 SELECT d.DocumentDate AS DocDate, 'ورود' AS [نوع], ii.Quantity AS [تعداد], s.SupplierName AS [طرف],
        d.DocumentNumber AS [شماره سند], d.Description AS [توضیحات]
 FROM ((IncomingDocuments d INNER JOIN IncomingItems ii ON d.ID=ii.IncomingDocumentID)
       INNER JOIN Suppliers s ON d.SupplierID=s.ID)
 WHERE d.IsPosted=True AND ii.ProductID=?
 UNION ALL
 SELECT d.DocumentDate, 'خروج', oi.Quantity, dep.DepartmentName, d.DocumentNumber, d.Description
 FROM ((OutgoingDocuments d INNER JOIN OutgoingItems oi ON d.ID=oi.OutgoingDocumentID)
       INNER JOIN Departments dep ON oi.DepartmentID=dep.ID)
 WHERE d.IsPosted=True AND oi.ProductID=?
) AS Q WHERE (? IS NULL OR DocDate>=?) AND (? IS NULL OR DocDate<=?)
ORDER BY DocDate DESC";
            var dt = new DataTable();
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.Add(OleDbUtil.P("@p1", productId));
            cmd.Parameters.Add(OleDbUtil.P("@p2", productId));
            cmd.Parameters.Add(OleDbUtil.P("@f1", from.HasValue ? (object)from.Value : DBNull.Value));
            cmd.Parameters.Add(OleDbUtil.P("@f2", from.HasValue ? (object)from.Value : DBNull.Value));
            cmd.Parameters.Add(OleDbUtil.P("@t1", to.HasValue ? (object)to.Value : DBNull.Value));
            cmd.Parameters.Add(OleDbUtil.P("@t2", to.HasValue ? (object)to.Value : DBNull.Value));
            using var ad = new OleDbDataAdapter(cmd);
            ad.Fill(dt);
            return dt;
        }

        public DataTable RunCombined(DateTime? from, DateTime? to, string? docNo, string? invoice, string? delivery)
        {
            var sql = @"
SELECT DocDate AS [تاریخ], [نوع], [شماره سند], [فاکتور], [حواله], [طرف], [کالا], [تعداد] FROM (
 SELECT d.DocumentDate AS DocDate, 'ورود' AS [نوع], d.DocumentNumber AS [شماره سند], d.InvoiceNumber AS [فاکتور],
        '' AS [حواله], s.SupplierName AS [طرف], p.ProductName AS [کالا], ii.Quantity AS [تعداد]
 FROM ((IncomingDocuments d INNER JOIN IncomingItems ii ON d.ID=ii.IncomingDocumentID)
       INNER JOIN Products p ON ii.ProductID=p.ID) INNER JOIN Suppliers s ON d.SupplierID=s.ID
 WHERE d.IsPosted=True
 UNION ALL
 SELECT d.DocumentDate, 'خروج', d.DocumentNumber, '', d.DeliveryNumber, dep.DepartmentName, p.ProductName, oi.Quantity
 FROM ((OutgoingDocuments d INNER JOIN OutgoingItems oi ON d.ID=oi.OutgoingDocumentID)
       INNER JOIN Products p ON oi.ProductID=p.ID) INNER JOIN Departments dep ON oi.DepartmentID=dep.ID
 WHERE d.IsPosted=True
) AS Q
WHERE (? IS NULL OR DocDate>=?) AND (? IS NULL OR DocDate<=?)
ORDER BY DocDate DESC";
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(sql, conn);
            AddDate(cmd, from, to);
            return Fill(cmd);
        }

        public DataTable RunBySupplier(int supplierId, DateTime? from, DateTime? to)
        {
            var sql = @"
SELECT d.DocumentDate AS [تاریخ], d.DocumentNumber AS [سند], p.ProductName AS [کالا], ii.Quantity AS [تعداد]
FROM ((IncomingDocuments d INNER JOIN IncomingItems ii ON d.ID=ii.IncomingDocumentID)
      INNER JOIN Products p ON ii.ProductID=p.ID)
WHERE d.IsPosted=True AND d.SupplierID=?
  AND (? IS NULL OR d.DocumentDate>=?) AND (? IS NULL OR d.DocumentDate<=?)
ORDER BY d.DocumentDate DESC";
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.Add(OleDbUtil.P("@sid", supplierId));
            AddDate(cmd, from, to);
            return Fill(cmd);
        }

        public DataTable RunByDepartment(int deptId, DateTime? from, DateTime? to)
        {
            var sql = @"
SELECT d.DocumentDate AS [تاریخ], dep.DepartmentName AS [بخش], p.ProductName AS [کالا], oi.Quantity AS [تعداد]
FROM ((OutgoingDocuments d INNER JOIN OutgoingItems oi ON d.ID=oi.OutgoingDocumentID)
      INNER JOIN Products p ON oi.ProductID=p.ID)
      INNER JOIN Departments dep ON oi.DepartmentID=dep.ID
WHERE d.IsPosted=True AND oi.DepartmentID=?
  AND (? IS NULL OR d.DocumentDate>=?) AND (? IS NULL OR d.DocumentDate<=?)
ORDER BY d.DocumentDate DESC";
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.Add(OleDbUtil.P("@did", deptId));
            AddDate(cmd, from, to);
            return Fill(cmd);
        }

        public DataTable RunByDelivery(string deliveryNo)
        {
            var sql = @"
SELECT d.DeliveryNumber AS [حواله], p.ProductName AS [کالا], oi.Quantity AS [تعداد], dep.DepartmentName AS [بخش]
FROM ((OutgoingDocuments d INNER JOIN OutgoingItems oi ON d.ID=oi.OutgoingDocumentID)
      INNER JOIN Products p ON oi.ProductID=p.ID) INNER JOIN Departments dep ON oi.DepartmentID=dep.ID
WHERE d.IsPosted=True AND d.DeliveryNumber LIKE ?
ORDER BY d.DeliveryNumber";
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.Add(OleDbUtil.P("@d", "%" + deliveryNo + "%"));
            return Fill(cmd);
        }

        private static void AddDate(OleDbCommand cmd, DateTime? from, DateTime? to)
        {
            cmd.Parameters.Add(OleDbUtil.P("@f1", from.HasValue ? (object)from.Value : DBNull.Value));
            cmd.Parameters.Add(OleDbUtil.P("@f2", from.HasValue ? (object)from.Value : DBNull.Value));
            cmd.Parameters.Add(OleDbUtil.P("@t1", to.HasValue ? (object)to.Value : DBNull.Value));
            cmd.Parameters.Add(OleDbUtil.P("@t2", to.HasValue ? (object)to.Value : DBNull.Value));
        }

        private DataTable Fill(string sql)
        {
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(sql, conn);
            return Fill(cmd);
        }

        private static DataTable Fill(OleDbCommand cmd)
        {
            var dt = new DataTable();
            using var ad = new OleDbDataAdapter(cmd);
            ad.Fill(dt);
            return dt;
        }
    }
}
