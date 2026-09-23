using System;
using System.Data.OleDb;
using Anbarban.Data;

namespace Anbarban.Services
{
    public sealed class StockService
    {
        private readonly AccessConnectionFactory _db;

        public StockService(AccessConnectionFactory db) => _db = db;

        public int GetCurrentStock(int productId, OleDbConnection? conn = null, OleDbTransaction? tx = null)
        {
            var own = conn == null;
            if (own) { conn = _db.Open(); tx = null; }
            try
            {
                var o = OleDbUtil.ExecuteScalar(conn!, tx,
                    "SELECT CurrentStock FROM Products WHERE ID=?", OleDbUtil.P("@id", productId));
                return o == null || o is DBNull ? 0 : Convert.ToInt32(o);
            }
            finally { if (own) conn!.Dispose(); }
        }

        public bool ApplyChange(OleDbConnection conn, OleDbTransaction tx, int productId, int quantity, bool isIncoming, int sign)
        {
            if (productId <= 0 || quantity <= 0) return false;
            var delta = (isIncoming ? quantity : -quantity) * sign;
            var cur = GetCurrentStock(productId, conn, tx);
            var newStock = cur + delta;
            if (newStock < 0) return false;
            OleDbUtil.ExecuteNonQuery(conn, tx,
                "UPDATE Products SET CurrentStock=? WHERE ID=?",
                OleDbUtil.P("@s", newStock), OleDbUtil.P("@id", productId));
            return true;
        }
    }
}
