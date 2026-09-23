SELECT
    d.DocumentDate AS DocDate,
    d.DocumentNumber AS [شماره سند],
    d.InvoiceNumber AS [شماره فاکتور],
    s.SupplierName AS [فروشنده],
    p.ProductName AS [کالا],
    ii.Quantity AS [تعداد]
FROM ((IncomingDocuments AS d
    INNER JOIN IncomingItems AS ii ON d.ID = ii.IncomingDocumentID)
    INNER JOIN Products AS p ON ii.ProductID = p.ID)
    INNER JOIN Suppliers AS s ON d.SupplierID = s.ID
WHERE d.IsPosted = True
    AND (Forms!frmReports!cboSupplier Is Null OR Forms!frmReports!cboSupplier = "" OR d.SupplierID = Forms!frmReports!cboSupplier)
    AND (Forms!frmReports!txtFromGreg Is Null OR d.DocumentDate >= Forms!frmReports!txtFromGreg)
    AND (Forms!frmReports!txtToGreg Is Null OR d.DocumentDate <= Forms!frmReports!txtToGreg)
ORDER BY d.DocumentDate DESC;
