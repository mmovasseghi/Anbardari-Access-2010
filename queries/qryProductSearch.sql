' qryProductSearch
' Form dependency: Forms!frmProductSearch!txtSearch

SELECT
    Products.ID,
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    Products.Unit AS [واحد],
    Products.CurrentStock AS [موجودی فعلی],
    Products.MinimumStock AS [حداقل موجودی],
    IIf([CurrentStock]<=[MinimumStock],"نیاز به تأمین","موجود") AS [وضعیت موجودی]
FROM Products
WHERE
    (
        Forms!frmProductSearch!txtSearch Is Null
        Or Forms!frmProductSearch!txtSearch = ""
        Or Products.ProductName Like "*" & Forms!frmProductSearch!txtSearch & "*"
        Or Products.ProductCode Like "*" & Forms!frmProductSearch!txtSearch & "*"
    )
ORDER BY Products.ProductName;
