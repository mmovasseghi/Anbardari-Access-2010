using System.Collections.Generic;
using System.Data.OleDb;
using Anbarban.Models;

namespace Anbarban.Data
{
    public sealed class SupplierRepository
    {
        private readonly AccessConnectionFactory _db;
        public SupplierRepository(AccessConnectionFactory db) => _db = db;

        public IList<IdName> ListActive()
        {
            var list = new List<IdName>();
            using var conn = _db.Open();
            using var cmd = new OleDbCommand("SELECT ID, SupplierName FROM Suppliers WHERE IsActive=True ORDER BY SupplierName", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new IdName { Id = r.GetInt32(0), Name = r.IsDBNull(1) ? "" : r.GetString(1) });
            return list;
        }

        public IList<(int Id, string Name, string Info, bool Active)> ListAll()
        {
            var list = new List<(int, string, string, bool)>();
            using var conn = _db.Open();
            using var cmd = new OleDbCommand("SELECT ID, SupplierName, SupplierInfo, IsActive FROM Suppliers ORDER BY SupplierName", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add((r.GetInt32(0), r.GetString(1), r.IsDBNull(2) ? "" : r.GetString(2), OleDbUtil.ToBool(r.GetValue(3))));
            return list;
        }

        public void Save(int id, string name, string info, bool active)
        {
            using var conn = _db.Open();
            if (id <= 0)
            {
                OleDbUtil.ExecuteNonQuery(conn, null,
                    "INSERT INTO Suppliers (SupplierName, SupplierInfo, IsActive) VALUES (?,?,?)",
                    OleDbUtil.P("@n", name), OleDbUtil.P("@i", info), OleDbUtil.P("@a", active));
            }
            else
            {
                OleDbUtil.ExecuteNonQuery(conn, null,
                    "UPDATE Suppliers SET SupplierName=?, SupplierInfo=?, IsActive=? WHERE ID=?",
                    OleDbUtil.P("@n", name), OleDbUtil.P("@i", info), OleDbUtil.P("@a", active), OleDbUtil.P("@id", id));
            }
        }
    }
}
