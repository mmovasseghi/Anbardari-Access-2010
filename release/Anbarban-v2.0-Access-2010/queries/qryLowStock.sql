' qryLowStock — کالاهای رو به اتمام

SELECT
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    Products.Unit AS [واحد],
    Products.CurrentStock AS [موجودی فعلی],
    Products.MinimumStock AS [حداقل موجودی]
FROM Products
WHERE Products.IsActive = True
  AND Products.CurrentStock <= Products.MinimumStock
ORDER BY Products.ProductName;
