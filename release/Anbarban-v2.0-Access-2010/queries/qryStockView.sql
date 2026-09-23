' qryStockView
' Form dependency: Forms!frmStockView!txtSearch , Forms!frmStockView!chkLowOnly

SELECT
    Products.ID,
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    Products.Unit AS [واحد],
    Products.CurrentStock AS [موجودی فعلی],
    Products.MinimumStock AS [حداقل موجودی],
    IIf([CurrentStock]<=[MinimumStock],"نیاز به تأمین","موجود") AS [وضعیت]
FROM Products
WHERE
    Products.IsActive = True
    AND
    (
        Forms!frmStockView!txtSearch Is Null
        Or Forms!frmStockView!txtSearch = ""
        Or Products.ProductName Like "*" & Forms!frmStockView!txtSearch & "*"
        Or Products.ProductCode Like "*" & Forms!frmStockView!txtSearch & "*"
    )
    AND
    (
        Forms!frmStockView!chkLowOnly Is Null
        Or Forms!frmStockView!chkLowOnly = False
        Or Products.CurrentStock <= Products.MinimumStock
    )
ORDER BY Products.ProductName;
