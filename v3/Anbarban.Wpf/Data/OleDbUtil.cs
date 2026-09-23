using System;
using System.Data.OleDb;

namespace Anbarban.Data
{
    internal static class OleDbUtil
    {
        public static OleDbParameter P(string name, object? value) =>
            new OleDbParameter(name, value ?? DBNull.Value);

        public static int ExecuteNonQuery(OleDbConnection conn, OleDbTransaction? tx, string sql, params OleDbParameter[] ps)
        {
            using var cmd = new OleDbCommand(sql, conn, tx);
            cmd.Parameters.AddRange(ps);
            return cmd.ExecuteNonQuery();
        }

        public static object? ExecuteScalar(OleDbConnection conn, OleDbTransaction? tx, string sql, params OleDbParameter[] ps)
        {
            using var cmd = new OleDbCommand(sql, conn, tx);
            cmd.Parameters.AddRange(ps);
            return cmd.ExecuteScalar();
        }

        public static int GetLastIdentity(OleDbConnection conn, OleDbTransaction? tx, string table)
        {
            var o = ExecuteScalar(conn, tx, $"SELECT MAX(ID) FROM {table}");
            return o == null || o is DBNull ? 0 : Convert.ToInt32(o);
        }

        public static bool ToBool(object? v)
        {
            if (v == null || v is DBNull) return false;
            if (v is bool b) return b;
            return Convert.ToInt32(v) != 0;
        }
    }
}
