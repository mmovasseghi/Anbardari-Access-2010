SELECT
    ii.IncomingDocumentID,
    p.ProductName AS [کالا],
    p.ProductCode AS [کد],
    ii.Quantity AS [تعداد],
    p.Unit AS [واحد]
FROM IncomingItems AS ii
INNER JOIN Products AS p ON ii.ProductID = p.ID
WHERE ii.IncomingDocumentID = Nz(Forms!frmIncomingConfirm!txtDocID, 0);
