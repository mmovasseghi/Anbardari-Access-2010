using Anbarban.Data;
using Anbarban.Services;

namespace Anbarban
{
    public static class AppServices
    {
        public static AccessConnectionFactory Db { get; } = new AccessConnectionFactory();
        public static ProductRepository Products { get; } = new ProductRepository(Db);
        public static SupplierRepository Suppliers { get; } = new SupplierRepository(Db);
        public static DepartmentRepository Departments { get; } = new DepartmentRepository(Db);
        public static IncomingRepository Incoming { get; } = new IncomingRepository(Db);
        public static OutgoingRepository Outgoing { get; } = new OutgoingRepository(Db);
        public static ReportRepository Reports { get; } = new ReportRepository(Db);
        public static DocumentPostService Post { get; } = new DocumentPostService(Db);
        public static UnlockWorkflow Unlock { get; } = new UnlockWorkflow(Db);
        public static StockService Stock { get; } = new StockService(Db);
    }
}
