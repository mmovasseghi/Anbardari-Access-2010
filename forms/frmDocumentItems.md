# مشخصات فرم frmDocumentItems (Subform)

## تنظیمات فرم

| ویژگی | مقدار |
|---|---|
| نام | `frmDocumentItems` |
| Record Source | `DocumentItems` |
| Caption | اقلام سند |
| Default View | Datasheet |
| Navigation Buttons | No |

## کنترل‌ها

| نام کنترل | نوع | Control Source | برچسب فارسی | نکات |
|---|---|---|---|---|
| `txtID` | Text Box | `ID` | شناسه | می‌توان مخفی کرد |
| `txtDocumentID` | Text Box | `DocumentID` | کد سند | معمولاً مخفی؛ از Link پر می‌شود |
| `cboProductID` | Combo Box | `ProductID` | کالا | نمایش نام، ذخیره ID |
| `txtQuantity` | Text Box | `Quantity` | تعداد | Long Integer > 0 |

## تنظیم Combo کالا

| ویژگی | مقدار |
|---|---|
| Row Source Type | Table/Query |
| Row Source | `SELECT ID, ProductName, ProductCode FROM Products WHERE IsActive = True ORDER BY ProductName;` |
| Column Count | 3 |
| Column Widths | `0cm;4cm;2cm` |
| Bound Column | 1 |
| Limit To List | Yes |

کاربر نام (و کد) کالا را می‌بیند؛ `ProductID` ذخیره می‌شود.

## اتصال به فرم والد

روی کنترل Subform در `frmDocuments`:

- Link Master Fields = `ID`
- Link Child Fields = `DocumentID`

## کد

محتوای `vba/frmDocumentItems_Code.bas` را در ماژول فرم paste کنید.

این کد:

- Quantity > 0 را اجباری می‌کند
- برای OUT وجود DeliveryNumber را از والد چک می‌کند
- موجودی را برای IN افزایش و برای OUT کاهش می‌دهد
- از منفی شدن موجودی جلوگیری می‌کند
- هنگام ویرایش/حذف، اثر قبلی موجودی را اصلاح می‌کند
