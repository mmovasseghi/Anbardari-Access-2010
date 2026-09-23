using System;

namespace Anbarban.Models
{
    public sealed class IdName
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public sealed class IncomingHeader
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = "";
        public string? InvoiceNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int SupplierId { get; set; }
        public string? Description { get; set; }
        public bool IsPosted { get; set; }
        public string SupplierName { get; set; } = "";
    }

    public sealed class IncomingLine
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
    }

    public sealed class OutgoingHeader
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = "";
        public string DeliveryNumber { get; set; } = "";
        public DateTime? DocumentDate { get; set; }
        public string? Description { get; set; }
        public bool IsPosted { get; set; }
    }

    public sealed class OutgoingLine
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "";
        public int CurrentStock { get; set; }
    }
}
