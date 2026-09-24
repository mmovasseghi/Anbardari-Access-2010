using System;
using Anbarban.Models;
using Anbarban.Services;
using Xunit;

namespace Anbarban.Tests
{
    public class MasterDataAndReportTests
    {
        [Fact]
        public void Supplier_crud_list()
        {
            using var ctx = new TestWarehouseContext();
            for (var i = 0; i < 15; i++)
                ctx.NewSupplier("فروشنده " + i);
            Assert.True(ctx.Suppliers.ListAll().Count >= 15);
            var found = ctx.Suppliers.SearchActive("فروشنده 3");
            Assert.Contains(found, x => x.Name.Contains("3"));
        }

        [Fact]
        public void Product_search_by_code()
        {
            using var ctx = new TestWarehouseContext();
            ctx.NewProduct("نام طولانی", "CODE-XYZ");
            var row = ctx.Products.GetByCode("CODE-XYZ");
            Assert.NotNull(row);
            Assert.Equal("نام طولانی", row!.Name);
        }

        [Fact]
        public void Reports_stock_and_movement()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("گزارش", "RPT-1");
            ctx.PostIncomingDoc(sup, "IN-R", (pid, 30));
            ctx.PostOutgoingDoc(dept, "OUT-R", (pid, 5));

            var stock = ctx.Reports.RunStock(false);
            Assert.True(stock.Rows.Count >= 1);

            var move = ctx.Reports.RunMovement(pid, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            Assert.True(move.Rows.Count >= 2);

            var combined = ctx.Reports.RunCombined(DateTime.Today.AddDays(-7), DateTime.Today.AddDays(1), null, null, null);
            Assert.True(combined.Rows.Count >= 1);
        }

        [Theory]
        [InlineData("1404/01/01")]
        [InlineData("1403/12/29")]
        [InlineData("1405/07/01")]
        [InlineData("invalid")]
        [InlineData("")]
        public void Jalali_parse_cases(string input)
        {
            var ok = JalaliCalendar.TryParse(input, out _);
            if (input == "invalid" || string.IsNullOrEmpty(input))
                Assert.False(ok);
            else
                Assert.True(ok);
        }

        [Fact]
        public void Incoming_lines_add_delete_before_post()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var p1 = ctx.NewProduct("L1", "L1");
            var p2 = ctx.NewProduct("L2", "L2");
            var docId = ctx.Incoming.SaveHeader(new IncomingHeader
            {
                DocumentNumber = "LINES",
                SupplierId = sup,
                DocumentDate = DateTime.Today
            });
            ctx.Incoming.AddLine(docId, p1, 10);
            ctx.Incoming.AddLine(docId, p2, 20);
            var lines = ctx.Incoming.GetLines(docId);
            Assert.Equal(2, lines.Count);
            ctx.Incoming.DeleteLine(lines[0].Id);
            Assert.Single(ctx.Incoming.GetLines(docId));
            var err = ctx.Post.PostIncoming(docId);
            Assert.Null(err);
            Assert.Equal(20, ctx.Stock.GetCurrentStock(p2));
            Assert.Equal(0, ctx.Stock.GetCurrentStock(p1));
        }
    }
}
