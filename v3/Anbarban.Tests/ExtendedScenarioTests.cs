using System;
using Anbarban.Models;
using Xunit;

namespace Anbarban.Tests
{
    public class ExtendedScenarioTests
    {
        [Fact]
        public void Low_stock_report_flags_minimum()
        {
            using var ctx = new TestWarehouseContext();
            var pid = ctx.NewProduct("کم‌موجود", "LOW-1", min: 10);
            var sup = ctx.NewSupplier();
            ctx.PostIncomingDoc(sup, "IN-LOW", (pid, 3));
            var all = ctx.Reports.RunStock(false);
            var low = ctx.Reports.RunStock(true);
            Assert.True(all.Rows.Count >= 1);
            Assert.True(low.Rows.Count >= 1);
        }

        [Fact]
        public void Reports_by_supplier_department_delivery()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier("گزارش‌گر");
            var dept = ctx.NewDepartment("آشپزخانه");
            var pid = ctx.NewProduct("گوجه", "TOM");
            ctx.PostIncomingDoc(sup, "S-IN", (pid, 12));
            ctx.PostOutgoingDoc(dept, "DEL-99", (pid, 4));

            Assert.True(ctx.Reports.RunBySupplier(sup, null, null).Rows.Count >= 1);
            Assert.True(ctx.Reports.RunByDepartment(dept, null, null).Rows.Count >= 1);
            Assert.True(ctx.Reports.RunByDelivery("DEL").Rows.Count >= 1);
        }

        [Fact]
        public void Unpost_incoming_restores_stock()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var pid = ctx.NewProduct("برگشت", "UN-IN");
            var docId = ctx.PostIncomingDoc(sup, "U-IN", (pid, 25));
            Assert.Equal(25, ctx.Stock.GetCurrentStock(pid));
            var err = ctx.Post.UnpostIncoming(docId);
            Assert.Null(err);
            Assert.Equal(0, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Unpost_outgoing_restores_stock()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("برگشت خروج", "UN-OUT");
            ctx.PostIncomingDoc(sup, "BOOT", (pid, 40));
            var docId = ctx.PostOutgoingDoc(dept, "U-OUT", (pid, 8));
            Assert.Equal(32, ctx.Stock.GetCurrentStock(pid));
            var err = ctx.Post.UnpostOutgoing(docId);
            Assert.Null(err);
            Assert.Equal(40, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Incoming_get_when_supplier_row_missing()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier("موقت");
            var docId = ctx.Incoming.SaveHeader(new IncomingHeader
            {
                DocumentNumber = "ORPHAN",
                SupplierId = sup,
                DocumentDate = DateTime.Today
            });
            ctx.DeleteSupplierRow(sup);
            var h = ctx.Incoming.Get(docId);
            Assert.NotNull(h);
            Assert.Equal("ORPHAN", h!.DocumentNumber);
            Assert.True(string.IsNullOrEmpty(h.SupplierName));
        }

        [Fact]
        public void Random_400_mixed_operations_never_negative_stock()
        {
            using var ctx = new TestWarehouseContext();
            var rng = new Random(42);
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("رندوم", "RND");
            ctx.PostIncomingDoc(sup, "SEED", (pid, 5000));
            for (var i = 0; i < 400; i++)
            {
                var stock = ctx.Stock.GetCurrentStock(pid);
                if (rng.Next(2) == 0)
                {
                    var q = rng.Next(1, 40);
                    ctx.PostIncomingDoc(sup, "RI" + i, (pid, q));
                }
                else
                {
                    var q = rng.Next(1, 30);
                    if (q <= stock)
                        ctx.PostOutgoingDoc(dept, "RO" + i, (pid, q));
                    else
                    {
                        var err = ctx.TryPostOutgoing(dept, "RO-F" + i, (pid, q));
                        Assert.NotNull(err);
                    }
                }
                Assert.True(ctx.Stock.GetCurrentStock(pid) >= 0);
            }
        }

        [Fact]
        public void Outgoing_rejects_when_stock_zero()
        {
            using var ctx = new TestWarehouseContext();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("خالی", "EMPTY-Z");
            var err = ctx.TryPostOutgoing(dept, "BIG", (pid, 100_000));
            Assert.NotNull(err);
            Assert.Equal(0, ctx.Stock.GetCurrentStock(pid));
        }
    }
}
