using System;

namespace Anbarban.Services
{
    internal static class TextSearch
    {
        public static string SqlStartsWith(string? term) =>
            string.IsNullOrWhiteSpace(term) ? "%" : term.Trim() + "%";

        public static bool StartsWith(string? field, string? term)
        {
            if (string.IsNullOrWhiteSpace(term)) return true;
            return (field ?? "").StartsWith(term.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
