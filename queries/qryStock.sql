' Query name: qryStock
' Purpose: گزارش موجودی
' Create in Access 2010: Create > Query Design > SQL View

SELECT
    Products.ID,
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    Products.Unit AS [واحد],
    Products.CurrentStock AS [موجودی فعلی],
    Products.MinimumStock AS [حداقل موجودی],
    IIf([CurrentStock] <= [MinimumStock], "کم‌موجودی", "عادی") AS [وضعیت],
    Products.IsActive AS [فعال]
FROM Products
WHERE Products.IsActive = True
ORDER BY Products.ProductName;
