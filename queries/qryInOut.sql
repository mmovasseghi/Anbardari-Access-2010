' qryInOut — گزارش ورود و خروج با فیلتر فرم frmInOutReport
' وابستگی به کنترل‌های فرم فیلتر

SELECT
    Documents.DocumentDate AS [تاریخ],
    Documents.DocumentNumber AS [شماره سند],
    Documents.DeliveryNumber AS [شماره حواله],
    IIf([Documents].[TransactionType]="IN","ورود","خروج") AS [نوع عملیات],
    Products.ProductName AS [کالا],
    Products.ProductCode AS [کد کالا],
    DocumentItems.Quantity AS [تعداد],
    Documents.Source AS [مبدأ],
    Documents.Destination AS [مقصد],
    Documents.Description AS [توضیحات]
FROM (Documents
    INNER JOIN DocumentItems ON Documents.ID = DocumentItems.DocumentID)
    INNER JOIN Products ON DocumentItems.ProductID = Products.ID
WHERE
    (
        Forms!frmInOutReport!cboType Is Null
        Or Forms!frmInOutReport!cboType = ""
        Or Documents.TransactionType = Forms!frmInOutReport!cboType
    )
    AND
    (
        Forms!frmInOutReport!txtFromDate Is Null
        Or Documents.DocumentDate >= Forms!frmInOutReport!txtFromDate
    )
    AND
    (
        Forms!frmInOutReport!txtToDate Is Null
        Or Documents.DocumentDate <= Forms!frmInOutReport!txtToDate
    )
    AND
    (
        Forms!frmInOutReport!cboProduct Is Null
        Or Forms!frmInOutReport!cboProduct = ""
        Or DocumentItems.ProductID = Forms!frmInOutReport!cboProduct
    )
    AND
    (
        Forms!frmInOutReport!txtDocNo Is Null
        Or Forms!frmInOutReport!txtDocNo = ""
        Or Documents.DocumentNumber Like "*" & Forms!frmInOutReport!txtDocNo & "*"
    )
    AND
    (
        Forms!frmInOutReport!txtDeliveryNo Is Null
        Or Forms!frmInOutReport!txtDeliveryNo = ""
        Or Documents.DeliveryNumber Like "*" & Forms!frmInOutReport!txtDeliveryNo & "*"
    )
ORDER BY Documents.DocumentDate DESC, Documents.DocumentNumber;
