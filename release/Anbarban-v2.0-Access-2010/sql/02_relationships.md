# روابط — انباربان (Access 2010)

| از | به | Cascade Delete |
|---|---|---|
| Suppliers.ID → IncomingDocuments.SupplierID | One-Many | No |
| IncomingDocuments.ID → IncomingItems.IncomingDocumentID | One-Many | **Yes** |
| Products.ID → IncomingItems.ProductID | One-Many | No |
| OutgoingDocuments.ID → OutgoingItems.OutgoingDocumentID | One-Many | **Yes** |
| Products.ID → OutgoingItems.ProductID | One-Many | No |
| Departments.ID → OutgoingItems.DepartmentID | One-Many | No |

همه با **Enforce Referential Integrity** فعال.
