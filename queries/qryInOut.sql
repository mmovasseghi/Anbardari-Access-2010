' Query name: qryInOut
' Purpose: گزارش ورود و خروج

SELECT
    Documents.DocumentDate AS [تاریخ],
    Documents.DocumentNumber AS [شماره سند],
    Documents.DeliveryNumber AS [شماره حواله],
    IIf([Documents].[TransactionType]="IN","ورود","خروج") AS [نوع],
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    DocumentItems.Quantity AS [تعداد],
    Products.Unit AS [واحد],
    Documents.Source AS [مبدأ],
    Documents.Destination AS [مقصد],
    Documents.Description AS [توضیحات]
FROM (Documents
    INNER JOIN DocumentItems ON Documents.ID = DocumentItems.DocumentID)
    INNER JOIN Products ON DocumentItems.ProductID = Products.ID
ORDER BY Documents.DocumentDate DESC, Documents.DocumentNumber, Products.ProductName;
