# راهنمای ساخت — Access 2010 (حداقلی)

## 1) فایل

Blank Database → `database/Inventory.accdb`

## 2) جداول

طبق `docs/SCHEMA.md`:

- Products
- Documents
- DocumentItems

## 3) روابط

Database Tools → Relationships  
`Documents.ID` → `DocumentItems.DocumentID` (RI)  
`Products.ID` → `DocumentItems.ProductID` (RI)

## 4) VBA Modules

Alt+F11 → Insert → Module  
فقط این سه تا:

1. `modConstants`
2. `modValidation`
3. `modStock`

متن را از فایل‌های `vba/` کپی کنید.

## 5) Queries

SQL فایل‌های `queries/` را در Query SQL View بسازید.

## 6) Forms

| فرم | منبع |
|---|---|
| frmProducts | Products |
| frmDocumentItems | DocumentItems (Datasheet) |
| frmDocuments | Documents + Subform |
| frmProductSearch | unbound + نتایج |
| frmMain | unbound منو |

کد هر فرم: `vba/*_Code.bas`

Combo نوع تراکنش: `IN;ورود;OUT;خروج` (Column Count=2, Widths=`0cm;3cm`)  
Combo کالا: `SELECT ID, ProductName, ProductCode FROM Products WHERE IsActive=True ORDER BY ProductName;`

Subform link: Master `ID` / Child `DocumentID`

## 7) Reports

از `reports/REPORTS.md`

## 8) Startup

Current Database → Display Form = `frmMain`

## 9) تست سریع

1. ۲ کالا بسازید  
2. سند ورود + چند قلم  
3. سند خروج با حواله  
4. خروج بیش از موجودی → باید خطا بدهد  
5. خروج بدون حواله → باید خطا بدهد  

## سیستم ضعیف

- Compact & Repair بعد از کار زیاد  
- همزمان چند فرم سنگین باز نکنید  
- گزارش‌ها را Preview کنید نه Print فوری روی دیتای خیلی بزرگ  
