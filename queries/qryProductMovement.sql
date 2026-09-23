SELECT * FROM (
    SELECT
        d.DocumentDate AS DocDate,
        d.DocumentNumber AS [شماره سند],
        d.InvoiceNumber AS [شماره فاکتور],
        Null AS [شماره حواله],
        "ورود" AS [نوع],
        ii.Quantity AS [تعداد],
        s.SupplierName AS [فروشنده / بخش],
        d.Description AS [توضیحات]
    FROM ((IncomingDocuments AS d
        INNER JOIN IncomingItems AS ii ON d.ID = ii.IncomingDocumentID)
        INNER JOIN Suppliers AS s ON d.SupplierID = s.ID)
    WHERE d.IsPosted = True AND ii.ProductID = Nz(Forms!frmReports!txtProductID, 0)
    UNION ALL
    SELECT
        d.DocumentDate,
        d.DocumentNumber,
        Null,
        d.DeliveryNumber,
        "خروج",
        oi.Quantity,
        dep.DepartmentName,
        d.Description
    FROM (OutgoingDocuments AS d
        INNER JOIN OutgoingItems AS oi ON d.ID = oi.OutgoingDocumentID)
        INNER JOIN Departments AS dep ON oi.DepartmentID = dep.ID
    WHERE d.IsPosted = True AND oi.ProductID = Nz(Forms!frmReports!txtProductID, 0)
) AS Q
WHERE
    (Forms!frmReports!txtFromGreg Is Null OR DocDate >= Forms!frmReports!txtFromGreg)
    AND (Forms!frmReports!txtToGreg Is Null OR DocDate <= Forms!frmReports!txtToGreg)
ORDER BY DocDate DESC;
