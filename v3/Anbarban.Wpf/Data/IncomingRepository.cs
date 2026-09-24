using System;
using System.Collections.Generic;
using System.Data.OleDb;
using Anbarban.Models;

namespace Anbarban.Data
{
    public sealed class IncomingRepository
    {
        private readonly AccessConnectionFactory _db;
        public IncomingRepository(AccessConnectionFactory db) => _db = db;

        public IncomingHeader? Get(int id)
        {
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(
                @"SELECT d.ID, d.DocumentNumber, d.InvoiceNumber, d.DocumentDate, d.SupplierID, d.Description, d.IsPosted, s.SupplierName
                  FROM IncomingDocuments d LEFT JOIN Suppliers s ON d.SupplierID=s.ID WHERE d.ID=?", conn);
            cmd.Parameters.Add(OleDbUtil.P("@id", id));
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapHeader(r);
        }

        public int SaveHeader(IncomingHeader h)
        {
            using var conn = _db.Open();
            if (h.Id <= 0)
            {
                OleDbUtil.ExecuteNonQuery(conn, null,
                    @"INSERT INTO IncomingDocuments (DocumentNumber, InvoiceNumber, DocumentDate, SupplierID, Description, IsPosted)
                      VALUES (?,?,?,?,?,False)",
                    OleDbUtil.P("@dn", h.DocumentNumber), OleDbUtil.P("@inv", h.InvoiceNumber),
                    OleDbUtil.P("@dt", h.DocumentDate ?? (object)DateTime.Today),
                    OleDbUtil.P("@sid", h.SupplierId), OleDbUtil.P("@desc", h.Description));
                return OleDbUtil.GetLastIdentity(conn, null, "IncomingDocuments");
            }
            OleDbUtil.ExecuteNonQuery(conn, null,
                @"UPDATE IncomingDocuments SET DocumentNumber=?, InvoiceNumber=?, DocumentDate=?, SupplierID=?, Description=?
                  WHERE ID=? AND IsPosted=False",
                OleDbUtil.P("@dn", h.DocumentNumber), OleDbUtil.P("@inv", h.InvoiceNumber),
                OleDbUtil.P("@dt", h.DocumentDate ?? (object)DateTime.Today),
                OleDbUtil.P("@sid", h.SupplierId), OleDbUtil.P("@desc", h.Description), OleDbUtil.P("@id", h.Id));
            return h.Id;
        }

        public List<IncomingLine> GetLines(int docId)
        {
            var list = new List<IncomingLine>();
            using var conn = _db.Open();
            using var cmd = new OleDbCommand(
                @"SELECT ii.ID, ii.ProductID, p.ProductName, ii.Quantity
                  FROM IncomingItems ii INNER JOIN Products p ON ii.ProductID=p.ID
                  WHERE ii.IncomingDocumentID=? ORDER BY ii.ID", conn);
            cmd.Parameters.Add(OleDbUtil.P("@d", docId));
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new IncomingLine
                {
                    Id = r.GetInt32(0),
                    ProductId = r.GetInt32(1),
                    ProductName = r.IsDBNull(2) ? "" : r.GetString(2),
                    Quantity = Convert.ToInt32(r.GetValue(3))
                });
            return list;
        }

        public void AddLine(int docId, int productId, int qty)
        {
            using var conn = _db.Open();
            OleDbUtil.ExecuteNonQuery(conn, null,
                "INSERT INTO IncomingItems (IncomingDocumentID, ProductID, Quantity) VALUES (?,?,?)",
                OleDbUtil.P("@d", docId), OleDbUtil.P("@p", productId), OleDbUtil.P("@q", qty));
        }

        public void UpdateLine(int lineId, int productId, int qty)
        {
            using var conn = _db.Open();
            OleDbUtil.ExecuteNonQuery(conn, null,
                "UPDATE IncomingItems SET ProductID=?, Quantity=? WHERE ID=?",
                OleDbUtil.P("@p", productId), OleDbUtil.P("@q", qty), OleDbUtil.P("@id", lineId));
        }

        public void DeleteLine(int lineId)
        {
            using var conn = _db.Open();
            OleDbUtil.ExecuteNonQuery(conn, null, "DELETE FROM IncomingItems WHERE ID=?", OleDbUtil.P("@id", lineId));
        }

        private static IncomingHeader MapHeader(OleDbDataReader r) => new IncomingHeader
        {
            Id = r.GetInt32(0),
            DocumentNumber = r.IsDBNull(1) ? "" : r.GetString(1),
            InvoiceNumber = r.IsDBNull(2) ? null : r.GetString(2),
            DocumentDate = r.IsDBNull(3) ? null : r.GetDateTime(3),
            SupplierId = r.GetInt32(4),
            Description = r.IsDBNull(5) ? null : r.GetString(5),
            IsPosted = OleDbUtil.ToBool(r.GetValue(6)),
            SupplierName = r.IsDBNull(7) ? "" : r.GetString(7)
        };
    }
}
