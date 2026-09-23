SELECT
    d.DocumentDate AS DocDate,
    d.DocumentNumber AS [شماره سند],
    d.DeliveryNumber AS [شماره حواله],
    dep.DepartmentName AS [بخش],
    p.ProductName AS [کالا],
    oi.Quantity AS [تعداد]
FROM ((OutgoingDocuments AS d
    INNER JOIN OutgoingItems AS oi ON d.ID = oi.OutgoingDocumentID)
    INNER JOIN Products AS p ON oi.ProductID = p.ID)
    INNER JOIN Departments AS dep ON oi.DepartmentID = dep.ID
WHERE d.IsPosted = True
    AND (Forms!frmReports!cboDepartment Is Null OR Forms!frmReports!cboDepartment = "" OR oi.DepartmentID = Forms!frmReports!cboDepartment)
    AND (Forms!frmReports!txtFromGreg Is Null OR d.DocumentDate >= Forms!frmReports!txtFromGreg)
    AND (Forms!frmReports!txtToGreg Is Null OR d.DocumentDate <= Forms!frmReports!txtToGreg)
ORDER BY d.DocumentDate DESC;
