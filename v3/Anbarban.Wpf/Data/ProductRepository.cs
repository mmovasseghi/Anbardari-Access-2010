using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace Anbarban.Data
{
    public sealed class ProductRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string Unit { get; set; } = "";
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class ProductRepository
    {
        private readonly AccessConnectionFactory _db;

        public ProductRepository(AccessConnectionFactory db) => _db = db;

        public IList<ProductRow> Search(string? term, int max = 200)
        {
            var list = new List<ProductRow>();
            using var conn = _db.Open();
            using var cmd = conn.CreateCommand();
            var sql = @"SELECT TOP " + max + @" ID, ProductName, ProductCode, Unit, CurrentStock, MinimumStock, IsActive
                        FROM Products WHERE IsActive=True";
            if (!string.IsNullOrWhiteSpace(term))
            {
                sql += " AND (ProductName LIKE ? OR ProductCode LIKE ?)";
                cmd.Parameters.AddWithValue("@p1", "%" + term.Trim() + "%");
                cmd.Parameters.AddWithValue("@p2", "%" + term.Trim() + "%");
            }
            sql += " ORDER BY ProductName";
            cmd.CommandText = sql;
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new ProductRow
                {
                    Id = r.GetInt32(0),
                    Name = r.IsDBNull(1) ? "" : r.GetString(1),
                    Code = r.IsDBNull(2) ? "" : r.GetString(2),
                    Unit = r.IsDBNull(3) ? "" : r.GetString(3),
                    CurrentStock = r.IsDBNull(4) ? 0 : Convert.ToInt32(r.GetValue(4)),
                    MinimumStock = r.IsDBNull(5) ? 0 : Convert.ToInt32(r.GetValue(5)),
                    IsActive = !r.IsDBNull(6) && r.GetBoolean(6)
                });
            }
            return list;
        }

        public void Save(int id, string name, string code, string unit, int minimum, bool active)
        {
            using var conn = _db.Open();
            if (id <= 0)
            {
                OleDbUtil.ExecuteNonQuery(conn, null,
                    "INSERT INTO Products (ProductName, ProductCode, Unit, CurrentStock, MinimumStock, IsActive) VALUES (?,?,?,0,?,?)",
                    OleDbUtil.P("@n", name), OleDbUtil.P("@c", code), OleDbUtil.P("@u", unit),
                    OleDbUtil.P("@m", minimum), OleDbUtil.P("@a", active));
            }
            else
            {
                OleDbUtil.ExecuteNonQuery(conn, null,
                    "UPDATE Products SET ProductName=?, ProductCode=?, Unit=?, MinimumStock=?, IsActive=? WHERE ID=?",
                    OleDbUtil.P("@n", name), OleDbUtil.P("@c", code), OleDbUtil.P("@u", unit),
                    OleDbUtil.P("@m", minimum), OleDbUtil.P("@a", active), OleDbUtil.P("@id", id));
            }
        }
    }
}
