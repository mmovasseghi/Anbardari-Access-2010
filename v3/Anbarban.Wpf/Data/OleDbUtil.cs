using System;
using System.Data.OleDb;

namespace Anbarban.Data
{
    internal static class OleDbUtil
    {
        /// <summary>Access YESNO via ACE OleDb expects -1 / 0, not true/false.</summary>
        public static object YesNo(bool value) => value ? (object)(-1) : 0;

        public static OleDbParameter P(string name, object? value)
        {
            if (value is bool b)
                return new OleDbParameter(name, YesNo(b));
            return new OleDbParameter(name, value ?? DBNull.Value);
        }

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

        /// <summary>Access LIKE — پیشوند (شروع با متن جستجو).</summary>
        public static string LikePrefix(string term)
        {
            term = (term ?? "").Trim();
            if (term.Length == 0) return "%";
            term = term.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
            return term + "%";
        }
    }
}
