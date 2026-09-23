using System;
using System.Configuration;

namespace Anbarban.Services
{
    public static class UnlockService
    {
        private const string Alphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

        public static string ComputeCode(string docKind, int documentId, string documentNumber)
        {
            var salt = ConfigurationManager.AppSettings["UnlockOrgSalt"] ?? "Anbarban-Offline-2010";
            var seed = $"{docKind.ToUpperInvariant().Trim()}|{documentId}|{documentNumber.Trim()}|{salt}";
            int h = 5381;
            foreach (var ch in seed)
                h = ((h * 33) ^ ch) & 0x7FFFFFFF;
            var code = "";
            for (int i = 0; i < 6; i++)
            {
                code += Alphabet[h % 32];
                h = (h * 1103515245 + 12345) & 0x7FFFFFFF;
            }
            return code;
        }
    }
}
