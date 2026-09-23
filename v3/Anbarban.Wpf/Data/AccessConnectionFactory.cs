using System;
using System.Data.OleDb;
using System.IO;

namespace Anbarban.Data
{
    public sealed class AccessConnectionFactory
    {
        private readonly string _connectionString;

        public AccessConnectionFactory()
        {
            var path = AccessConfig.GetDatabasePath();
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    "فایل پایگاه داده پیدا نشد. ابتدا Inventory.accdb را در پوشه Data قرار دهید یا از اسکریپت v2 بسازید.\n\n" + path);
            _connectionString = AccessConfig.BuildConnectionString(path);
        }

        public OleDbConnection Open()
        {
            var c = new OleDbConnection(_connectionString);
            c.Open();
            return c;
        }
    }
}
