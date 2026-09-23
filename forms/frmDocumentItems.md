# مشخصات فرم frmDocumentItems (Subform)

## تنظیمات

| ویژگی | مقدار |
|---|---|
| نام | `frmDocumentItems` |
| Record Source | `DocumentItems` |
| Caption | اقلام سند |
| Default View | Datasheet |

## کنترل‌ها

| نام | نوع | Control Source | Datasheet Caption |
|---|---|---|---|
| `txtID` | Text Box | `ID` | شناسه (می‌توان مخفی کرد) |
| `txtDocumentID` | Text Box | `DocumentID` | کد سند (مخفی) |
| `cboProductID` | Combo Box | `ProductID` | کالا |
| `txtQuantity` | Text Box | `Quantity` | تعداد |

## Combo کالا

| ویژگی | مقدار |
|---|---|
| Row Source | `SELECT ID, ProductName, ProductCode FROM Products WHERE IsActive = True ORDER BY ProductName;` |
| Column Count | 3 |
| Column Widths | `0cm;4cm;2cm` |
| Bound Column | 1 |
| Limit To List | Yes |

## منطق موجودی (تست‌شده)

- IN → افزایش موجودی
- OUT → کاهش موجودی
- Quantity > 0
- موجودی منفی ممنوع
- حذف قلم فقط بعد از تأیید کاربر روی موجودی اثر می‌گذارد (`AfterDelConfirm`)

## کد

`vba/frmDocumentItems_Code.bas`
