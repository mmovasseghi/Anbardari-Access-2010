using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Anbarban.Controls;
using Anbarban.Data;
using Anbarban.Models;

namespace Anbarban.Views
{
    internal static class LiveComboSearch
    {
        private const int DefaultMax = 35;

        public static void AttachProducts(SearchComboBox cbo, int max = DefaultMax) =>
            cbo.SetLoader(term =>
            {
                var list = new List<IdName>();
                foreach (var p in AppServices.Products.Search(string.IsNullOrWhiteSpace(term) ? null : term, max))
                    list.Add(new IdName { Id = p.Id, Name = p.Name });
                return list;
            });

        public static void AttachProductsAsIdName(SearchComboBox cbo, int max = DefaultMax, bool includeCode = false) =>
            cbo.SetLoader(term =>
            {
                var list = new List<IdName>();
                foreach (var p in AppServices.Products.Search(string.IsNullOrWhiteSpace(term) ? null : term, max))
                {
                    var name = p.Name;
                    if (includeCode && !string.IsNullOrEmpty(p.Code))
                        name += " (" + p.Code + ")";
                    list.Add(new IdName { Id = p.Id, Name = name });
                }
                return list;
            });

        public static void AttachSuppliers(SearchComboBox cbo, int max = DefaultMax) =>
            cbo.SetLoader(term =>
                AppServices.Suppliers.SearchActive(string.IsNullOrWhiteSpace(term) ? null : term, max));

        public static void AttachDepartments(SearchComboBox cbo, int max = DefaultMax) =>
            cbo.SetLoader(term =>
                AppServices.Departments.SearchActive(string.IsNullOrWhiteSpace(term) ? null : term, max));

        public static void SetSelectedId(SearchComboBox cbo, int id, string displayName) =>
            cbo.SetSelection(id, displayName);

        public static void AttachTextBox(TextBox box, Action refresh) =>
            box.TextChanged += (_, __) => refresh();
    }
}
