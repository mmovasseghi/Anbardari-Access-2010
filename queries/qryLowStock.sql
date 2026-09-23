' Query name: qryLowStock
' Purpose: گزارش کالاهای کم‌موجودی

SELECT
    Products.ID,
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    Products.Unit AS [واحد],
    Products.CurrentStock AS [موجودی فعلی],
    Products.MinimumStock AS [حداقل موجودی],
    ([MinimumStock] - [CurrentStock]) AS [کمبود]
FROM Products
WHERE Products.IsActive = True
  AND Products.CurrentStock <= Products.MinimumStock
ORDER BY ([MinimumStock] - [CurrentStock]) DESC, Products.ProductName;
