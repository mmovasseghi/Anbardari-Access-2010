' Query name: qryProductSearch
' Purpose: جستجوی کالا بر اساس نام یا کد
' Uses form controls on frmProductSearch:
'   Forms!frmProductSearch!txtName
'   Forms!frmProductSearch!txtCode
'
' If a criterion is blank, that filter is ignored.

SELECT
    Products.ID,
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    Products.Unit AS [واحد],
    Products.CurrentStock AS [موجودی فعلی],
    Products.MinimumStock AS [حداقل موجودی],
    Products.IsActive AS [فعال]
FROM Products
WHERE
    (
        Forms!frmProductSearch!txtName Is Null
        Or Forms!frmProductSearch!txtName = ""
        Or Products.ProductName Like "*" & Forms!frmProductSearch!txtName & "*"
    )
    AND
    (
        Forms!frmProductSearch!txtCode Is Null
        Or Forms!frmProductSearch!txtCode = ""
        Or Products.ProductCode Like "*" & Forms!frmProductSearch!txtCode & "*"
    )
ORDER BY Products.ProductName;
