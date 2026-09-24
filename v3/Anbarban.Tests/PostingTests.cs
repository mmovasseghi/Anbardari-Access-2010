using System;
using System.Linq;
using Anbarban.Models;
using Xunit;

namespace Anbarban.Tests
{
    public class PostingTests
    {
        [Fact]
        public void Incoming_increases_stock()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var pid = ctx.NewProduct("کالا الف", "P-A");
            ctx.PostIncomingDoc(sup, "IN-1", (pid, 50));
            Assert.Equal(50, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Outgoing_decreases_stock()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("کالا ب", "P-B");
            ctx.PostIncomingDoc(sup, "IN-1", (pid, 100));
            ctx.PostOutgoingDoc(dept, "H-1", (pid, 40));
            Assert.Equal(60, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Outgoing_fails_when_over_stock()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("کالا ج", "P-C");
            ctx.PostIncomingDoc(sup, "IN-1", (pid, 5));
            var err = ctx.TryPostOutgoing(dept, "H-FAIL", (pid, 999));
            Assert.NotNull(err);
            Assert.Contains("کافی نیست", err);
            Assert.Equal(5, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Incoming_fails_without_lines()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var docId = ctx.Incoming.SaveHeader(new IncomingHeader
            {
                DocumentNumber = "EMPTY",
                SupplierId = sup,
                DocumentDate = DateTime.Today
            });
            var err = ctx.Post.PostIncoming(docId);
            Assert.NotNull(err);
        }

        [Fact]
        public void Incoming_get_works_after_save()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier("تره‌بار X");
            var docId = ctx.Incoming.SaveHeader(new IncomingHeader
            {
                DocumentNumber = "HDR-1",
                SupplierId = sup,
                DocumentDate = DateTime.Today
            });
            var h = ctx.Incoming.Get(docId);
            Assert.NotNull(h);
            Assert.Equal("HDR-1", h!.DocumentNumber);
            Assert.Equal("تره‌بار X", h.SupplierName);
        }

        [Fact]
        public void Double_post_incoming_blocked()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var pid = ctx.NewProduct("کالا د", "P-D");
            var docId = ctx.PostIncomingDoc(sup, "IN-2", (pid, 10));
            var err = ctx.Post.PostIncoming(docId);
            Assert.NotNull(err);
            Assert.Equal(10, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Double_post_outgoing_blocked()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("کالا ه", "P-H");
            ctx.PostIncomingDoc(sup, "IN-H", (pid, 50));
            var docId = ctx.PostOutgoingDoc(dept, "OUT-H", (pid, 5));
            var err = ctx.Post.PostOutgoing(docId);
            Assert.NotNull(err);
            Assert.Equal(45, ctx.Stock.GetCurrentStock(pid));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(99)]
        [InlineData(500)]
        public void Incoming_various_quantities(int qty)
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var pid = ctx.NewProduct("کالا Q" + qty, "PQ" + qty);
            ctx.PostIncomingDoc(sup, "IN-Q" + qty, (pid, qty));
            Assert.Equal(qty, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Multiple_incoming_documents_accumulate()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var pid = ctx.NewProduct("موز تست", "FR-T");
            for (var i = 1; i <= 20; i++)
                ctx.PostIncomingDoc(sup, "IN-M" + i, (pid, i));
            Assert.Equal(210, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Outgoing_multiple_lines_same_product()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("کالا چندخط", "ML-1");
            ctx.PostIncomingDoc(sup, "IN-ML", (pid, 100));
            var docId = ctx.Outgoing.SaveHeader(new OutgoingHeader { DeliveryNumber = "O-ML", DocumentDate = DateTime.Today });
            ctx.Outgoing.AddLine(docId, pid, 10, dept);
            ctx.Outgoing.AddLine(docId, pid, 15, dept);
            var err = ctx.Post.PostOutgoing(docId);
            Assert.Null(err);
            Assert.Equal(75, ctx.Stock.GetCurrentStock(pid));
        }

        [Fact]
        public void Stress_250_in_out_cycles()
        {
            using var ctx = new TestWarehouseContext();
            var sup = ctx.NewSupplier();
            var dept = ctx.NewDepartment();
            var pid = ctx.NewProduct("استرس", "STR-1");
            ctx.PostIncomingDoc(sup, "IN-BOOT", (pid, 10_000));
            for (var i = 0; i < 250; i++)
            {
                ctx.PostOutgoingDoc(dept, "O-" + i, (pid, 3));
                ctx.PostIncomingDoc(sup, "I-" + i, (pid, 5));
            }
            var expected = 10_000 + 250 * (5 - 3);
            Assert.Equal(expected, ctx.Stock.GetCurrentStock(pid));
        }
    }
}
