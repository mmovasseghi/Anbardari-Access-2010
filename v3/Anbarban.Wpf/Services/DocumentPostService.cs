using System;
using System.Collections.Generic;
using System.Data.OleDb;
using Anbarban.Data;

namespace Anbarban.Services
{
    public sealed class DocumentPostService
    {
        private readonly AccessConnectionFactory _db;
        private readonly StockService _stock;

        public DocumentPostService(AccessConnectionFactory db)
        {
            _db = db;
            _stock = new StockService(db);
        }

        public string? PostIncoming(int documentId)
        {
            using var conn = _db.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                var items = LoadIncomingItems(conn, tx, documentId);
                if (items.Count == 0) return "حداقل یک قلم کالا وارد کنید.";
                foreach (var (pid, qty) in items)
                {
                    if (!_stock.ApplyChange(conn, tx, pid, qty, true, 1))
                    {
                        tx.Rollback();
                        return "ثبت نهایی انجام نشد. کالا یا تعداد را بررسی کنید.";
                    }
                }
                OleDbUtil.ExecuteNonQuery(conn, tx,
                    "UPDATE IncomingDocuments SET IsPosted=True, PostedAt=? WHERE ID=?",
                    OleDbUtil.P("@d", DateTime.Now), OleDbUtil.P("@id", documentId));
                tx.Commit();
                return null;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return ex.Message;
            }
        }

        public string? PostOutgoing(int documentId)
        {
            using var conn = _db.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                var items = LoadOutgoingItems(conn, tx, documentId);
                if (items.Count == 0) return "حداقل یک قلم کالا وارد کنید.";
                var totals = new Dictionary<int, int>();
                foreach (var (pid, qty, _) in items)
                    totals[pid] = totals.GetValueOrDefault(pid) + qty;
                foreach (var kv in totals)
                {
                    var avail = _stock.GetCurrentStock(kv.Key, conn, tx);
                    if (kv.Value > avail)
                        return $"موجودی کالا کافی نیست.\nموجودی فعلی: {avail}\nتعداد درخواستی: {kv.Value}";
                }
                foreach (var (pid, qty, _) in items)
                {
                    if (!_stock.ApplyChange(conn, tx, pid, qty, false, 1))
                    {
                        tx.Rollback();
                        return "ثبت نهایی خروج انجام نشد.";
                    }
                }
                OleDbUtil.ExecuteNonQuery(conn, tx,
                    "UPDATE OutgoingDocuments SET IsPosted=True, PostedAt=? WHERE ID=?",
                    OleDbUtil.P("@d", DateTime.Now), OleDbUtil.P("@id", documentId));
                tx.Commit();
                return null;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return ex.Message;
            }
        }

        public string? UnpostIncoming(int documentId)
        {
            using var conn = _db.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                foreach (var (pid, qty) in LoadIncomingItems(conn, tx, documentId))
                {
                    if (!_stock.ApplyChange(conn, tx, pid, qty, false, 1))
                    {
                        tx.Rollback();
                        return "برگشت موجودی ممکن نیست (موجودی کافی نیست).";
                    }
                }
                OleDbUtil.ExecuteNonQuery(conn, tx,
                    "UPDATE IncomingDocuments SET IsPosted=False, PostedAt=Null WHERE ID=?",
                    OleDbUtil.P("@id", documentId));
                tx.Commit();
                return null;
            }
            catch (Exception ex) { tx.Rollback(); return ex.Message; }
        }

        public string? UnpostOutgoing(int documentId)
        {
            using var conn = _db.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                foreach (var (pid, qty, _) in LoadOutgoingItems(conn, tx, documentId))
                {
                    if (!_stock.ApplyChange(conn, tx, pid, qty, true, 1))
                    {
                        tx.Rollback();
                        return "برگشت موجودی انجام نشد.";
                    }
                }
                OleDbUtil.ExecuteNonQuery(conn, tx,
                    "UPDATE OutgoingDocuments SET IsPosted=False, PostedAt=Null WHERE ID=?",
                    OleDbUtil.P("@id", documentId));
                tx.Commit();
                return null;
            }
            catch (Exception ex) { tx.Rollback(); return ex.Message; }
        }

        private static List<(int ProductId, int Qty)> LoadIncomingItems(OleDbConnection conn, OleDbTransaction tx, int docId)
        {
            var list = new List<(int, int)>();
            using var cmd = new OleDbCommand(
                "SELECT ProductID, Quantity FROM IncomingItems WHERE IncomingDocumentID=?", conn, tx);
            cmd.Parameters.Add(OleDbUtil.P("@d", docId));
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add((r.GetInt32(0), Convert.ToInt32(r.GetValue(1))));
            return list;
        }

        private static List<(int ProductId, int Qty, int Dept)> LoadOutgoingItems(OleDbConnection conn, OleDbTransaction tx, int docId)
        {
            var list = new List<(int, int, int)>();
            using var cmd = new OleDbCommand(
                "SELECT ProductID, Quantity, DepartmentID FROM OutgoingItems WHERE OutgoingDocumentID=?", conn, tx);
            cmd.Parameters.Add(OleDbUtil.P("@d", docId));
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add((r.GetInt32(0), Convert.ToInt32(r.GetValue(1)), r.GetInt32(2)));
            return list;
        }
    }
}
