SELECT * FROM (
    SELECT
        d.DocumentDate AS DocDate,
        d.DocumentNumber AS [شماره سند],
        d.InvoiceNumber AS [شماره فاکتور],
        Null AS [شماره حواله],
        "ورود" AS [نوع عملیات],
        s.SupplierName AS [فروشنده / بخش],
        p.ProductName AS [کالا],
        ii.Quantity AS [تعداد],
        d.Description AS [توضیحات]
    FROM ((IncomingDocuments AS d
        INNER JOIN IncomingItems AS ii ON d.ID = ii.IncomingDocumentID)
        INNER JOIN Products AS p ON ii.ProductID = p.ID)
        INNER JOIN Suppliers AS s ON d.SupplierID = s.ID
    WHERE d.IsPosted = True
    UNION ALL
    SELECT
        d.DocumentDate,
        d.DocumentNumber,
        Null,
        d.DeliveryNumber,
        "خروج",
        dep.DepartmentName,
        p.ProductName,
        oi.Quantity,
        d.Description
    FROM ((OutgoingDocuments AS d
        INNER JOIN OutgoingItems AS oi ON d.ID = oi.OutgoingDocumentID)
        INNER JOIN Products AS p ON oi.ProductID = p.ID)
        INNER JOIN Departments AS dep ON oi.DepartmentID = dep.ID
    WHERE d.IsPosted = True
) AS Q
WHERE
    (Forms!frmReports!txtFromGreg Is Null OR DocDate >= Forms!frmReports!txtFromGreg)
    AND (Forms!frmReports!txtToGreg Is Null OR DocDate <= Forms!frmReports!txtToGreg)
    AND (Forms!frmReports!cboProduct Is Null OR Forms!frmReports!cboProduct = "" OR [کالا] IN (SELECT ProductName FROM Products WHERE ID=Forms!frmReports!cboProduct))
    AND (Forms!frmReports!txtDocNo Is Null OR Forms!frmReports!txtDocNo = "" OR [شماره سند] Like "*" & Forms!frmReports!txtDocNo & "*")
    AND (Forms!frmReports!txtInvoiceNo Is Null OR Forms!frmReports!txtInvoiceNo = "" OR [شماره فاکتور] Like "*" & Forms!frmReports!txtInvoiceNo & "*")
    AND (Forms!frmReports!txtDeliveryNo Is Null OR Forms!frmReports!txtDeliveryNo = "" OR [شماره حواله] Like "*" & Forms!frmReports!txtDeliveryNo & "*")
ORDER BY DocDate DESC;
