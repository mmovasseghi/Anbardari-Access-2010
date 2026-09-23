' qryProductMovement — گردش یک کالا
' وابستگی: Forms!frmProductMovement!txtProductID
'           Forms!frmProductMovement!txtFromDate
'           Forms!frmProductMovement!txtToDate

SELECT
    Documents.DocumentDate AS [تاریخ],
    Documents.DocumentNumber AS [شماره سند],
    Documents.DeliveryNumber AS [شماره حواله],
    IIf([Documents].[TransactionType]="IN","ورود","خروج") AS [نوع عملیات],
    DocumentItems.Quantity AS [تعداد],
    Documents.Source AS [مبدأ],
    Documents.Destination AS [مقصد],
    Documents.Description AS [توضیحات]
FROM (Documents
    INNER JOIN DocumentItems ON Documents.ID = DocumentItems.DocumentID)
WHERE
    DocumentItems.ProductID = Forms!frmProductMovement!txtProductID
    AND
    (
        Forms!frmProductMovement!txtFromDate Is Null
        Or Documents.DocumentDate >= Forms!frmProductMovement!txtFromDate
    )
    AND
    (
        Forms!frmProductMovement!txtToDate Is Null
        Or Documents.DocumentDate <= Forms!frmProductMovement!txtToDate
    )
ORDER BY Documents.DocumentDate DESC, Documents.DocumentNumber;
