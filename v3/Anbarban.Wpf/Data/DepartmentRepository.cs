using System.Collections.Generic;
using System.Data.OleDb;
using Anbarban.Models;

namespace Anbarban.Data
{
    public sealed class DepartmentRepository
    {
        private readonly AccessConnectionFactory _db;
        public DepartmentRepository(AccessConnectionFactory db) => _db = db;

        public IList<IdName> ListActive()
        {
            var list = new List<IdName>();
            using var conn = _db.Open();
            using var cmd = new OleDbCommand("SELECT ID, DepartmentName FROM Departments WHERE IsActive=True ORDER BY DepartmentName", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new IdName { Id = r.GetInt32(0), Name = r.IsDBNull(1) ? "" : r.GetString(1) });
            return list;
        }

        public IList<(int Id, string Name, bool Active)> ListAll()
        {
            var list = new List<(int, string, bool)>();
            using var conn = _db.Open();
            using var cmd = new OleDbCommand("SELECT ID, DepartmentName, IsActive FROM Departments ORDER BY DepartmentName", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add((r.GetInt32(0), r.GetString(1), OleDbUtil.ToBool(r.GetValue(2))));
            return list;
        }

        public void Save(int id, string name, bool active)
        {
            using var conn = _db.Open();
            if (id <= 0)
                OleDbUtil.ExecuteNonQuery(conn, null, "INSERT INTO Departments (DepartmentName, IsActive) VALUES (?,?)",
                    OleDbUtil.P("@n", name), OleDbUtil.P("@a", active));
            else
                OleDbUtil.ExecuteNonQuery(conn, null, "UPDATE Departments SET DepartmentName=?, IsActive=? WHERE ID=?",
                    OleDbUtil.P("@n", name), OleDbUtil.P("@a", active), OleDbUtil.P("@id", id));
        }
    }
}
