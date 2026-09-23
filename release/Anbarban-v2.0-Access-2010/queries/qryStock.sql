' qryStock — پایه موجودی (گزارش ساده)

SELECT
    Products.ProductCode AS [کد کالا],
    Products.ProductName AS [نام کالا],
    Products.Unit AS [واحد],
    Products.CurrentStock AS [موجودی فعلی],
    Products.MinimumStock AS [حداقل موجودی],
    IIf([CurrentStock]<=[MinimumStock],"نیاز به تأمین","موجود") AS [وضعیت]
FROM Products
WHERE Products.IsActive = True
ORDER BY Products.ProductName;
