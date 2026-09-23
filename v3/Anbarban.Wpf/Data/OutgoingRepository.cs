using System;
using System.Collections.Generic;
using System.Data.OleDb;
using Anbarban.Models;

namespace Anbarban.Data
{
    public sealed class OutgoingRepository
    {
        private readonly AccessConnectionFactory _db;
        private readonly Services.StockService _stock;

        public OutgoingRepository(AccessConnectionFactory db)
        {
            _db = db;
            _stock = new Services.StockService(db);
        }

        public OutgoingHeader? Get(int id)
        {
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(
                "SELECT ID, DocumentNumber, DeliveryNumber, DocumentDate, Description, IsPosted FROM OutgoingDocuments WHERE ID=?", conn);
            cmd.Parameters.Add(OleDbUtil.P("@id", id));
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new OutgoingHeader
            {
                Id = r.GetInt32(0),
                DocumentNumber = r.GetString(1),
                DeliveryNumber = r.GetString(2),
                DocumentDate = r.IsDBNull(3) ? null : r.GetDateTime(3),
                Description = r.IsDBNull(4) ? null : r.GetString(4),
                IsPosted = OleDbUtil.ToBool(r.GetValue(5))
            };
        }

        public int SaveHeader(OutgoingHeader h)
        {
            using var conn = _db.Open();
            if (h.Id <= 0)
            {
                OleDbUtil.ExecuteNonQuery(conn, null,
                    @"INSERT INTO OutgoingDocuments (DocumentNumber, DeliveryNumber, DocumentDate, Description, IsPosted)
                      VALUES (?,?,?,?,False)",
                    OleDbUtil.P("@dn", h.DocumentNumber), OleDbUtil.P("@dl", h.DeliveryNumber),
                    OleDbUtil.P("@dt", h.DocumentDate ?? (object)DateTime.Today), OleDbUtil.P("@desc", h.Description));
                return OleDbUtil.GetLastIdentity(conn, null, "OutgoingDocuments");
            }
            OleDbUtil.ExecuteNonQuery(conn, null,
                @"UPDATE OutgoingDocuments SET DocumentNumber=?, DeliveryNumber=?, DocumentDate=?, Description=?
                  WHERE ID=? AND IsPosted=False",
                OleDbUtil.P("@dn", h.DocumentNumber), OleDbUtil.P("@dl", h.DeliveryNumber),
                OleDbUtil.P("@dt", h.DocumentDate ?? (object)DateTime.Today), OleDbUtil.P("@desc", h.Description),
                OleDbUtil.P("@id", h.Id));
            return h.Id;
        }

        public List<OutgoingLine> GetLines(int docId)
        {
            var list = new List<OutgoingLine>();
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(
                @"SELECT oi.ID, oi.ProductID, p.ProductName, oi.Quantity, oi.DepartmentID, dep.DepartmentName, p.CurrentStock
                  FROM ((OutgoingItems oi INNER JOIN Products p ON oi.ProductID=p.ID)
                  INNER JOIN Departments dep ON oi.DepartmentID=dep.ID)
                  WHERE oi.OutgoingDocumentID=? ORDER BY oi.ID", conn);
            cmd.Parameters.Add(OleDbUtil.P("@d", docId));
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new OutgoingLine
                {
                    Id = r.GetInt32(0),
                    ProductId = r.GetInt32(1),
                    ProductName = r.GetString(2),
                    Quantity = Convert.ToInt32(r.GetValue(3)),
                    DepartmentId = r.GetInt32(4),
                    DepartmentName = r.GetString(5),
                    CurrentStock = Convert.ToInt32(r.GetValue(6))
                });
            return list;
        }

        public bool WouldExceedStock(int docId, int productId, int newQty, int excludeLineId)
        {
            var need = newQty;
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(
                "SELECT ID, ProductID, Quantity FROM OutgoingItems WHERE OutgoingDocumentID=?", conn);
            cmd.Parameters.Add(OleDbUtil.P("@d", docId));
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var lid = r.GetInt32(0);
                if (r.GetInt32(1) == productId && lid != excludeLineId)
                    need += Convert.ToInt32(r.GetValue(2));
            }
            return need > AppServices.Stock.GetCurrentStock(productId);
        }

        public void AddLine(int docId, int productId, int qty, int deptId)
        {
            using var conn = _db.Open();
            OleDbUtil.ExecuteNonQuery(conn, null,
                "INSERT INTO OutgoingItems (OutgoingDocumentID, ProductID, Quantity, DepartmentID) VALUES (?,?,?,?)",
                OleDbUtil.P("@d", docId), OleDbUtil.P("@p", productId), OleDbUtil.P("@q", qty), OleDbUtil.P("@dept", deptId));
        }

        public void UpdateLine(int lineId, int productId, int qty, int deptId)
        {
            using var conn = _db.Open();
            OleDbUtil.ExecuteNonQuery(conn, null,
                "UPDATE OutgoingItems SET ProductID=?, Quantity=?, DepartmentID=? WHERE ID=?",
                OleDbUtil.P("@p", productId), OleDbUtil.P("@q", qty), OleDbUtil.P("@dept", deptId), OleDbUtil.P("@id", lineId));
        }

        public void DeleteLine(int lineId)
        {
            using var conn = _db.Open();
            OleDbUtil.ExecuteNonQuery(conn, null, "DELETE FROM OutgoingItems WHERE ID=?", OleDbUtil.P("@id", lineId));
        }
    }
}
