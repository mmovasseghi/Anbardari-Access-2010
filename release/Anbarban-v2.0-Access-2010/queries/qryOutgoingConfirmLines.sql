SELECT
    oi.OutgoingDocumentID,
    p.ProductName AS [کالا],
    oi.Quantity AS [خروج],
    p.CurrentStock AS [موجودی فعلی],
    (p.CurrentStock - oi.Quantity) AS [بعد از خروج],
    d.DepartmentName AS [بخش]
FROM ((OutgoingItems AS oi
INNER JOIN Products AS p ON oi.ProductID = p.ID)
INNER JOIN Departments AS d ON oi.DepartmentID = d.ID)
WHERE oi.OutgoingDocumentID = Nz(Forms!frmOutgoingConfirm!txtDocID, 0);
