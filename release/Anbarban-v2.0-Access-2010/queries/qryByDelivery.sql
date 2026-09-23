SELECT
    d.DocumentDate AS DocDate,
    d.DeliveryNumber AS [شماره حواله],
    d.DocumentNumber AS [شماره سند],
    p.ProductName AS [کالا],
    oi.Quantity AS [تعداد],
    dep.DepartmentName AS [بخش]
FROM ((OutgoingDocuments AS d
    INNER JOIN OutgoingItems AS oi ON d.ID = oi.OutgoingDocumentID)
    INNER JOIN Products AS p ON oi.ProductID = p.ID)
    INNER JOIN Departments AS dep ON oi.DepartmentID = dep.ID
WHERE d.IsPosted = True
    AND (Forms!frmReports!txtDeliveryNo Is Null OR Forms!frmReports!txtDeliveryNo = "" OR d.DeliveryNumber Like "*" & Forms!frmReports!txtDeliveryNo & "*")
ORDER BY d.DeliveryNumber, p.ProductName;
