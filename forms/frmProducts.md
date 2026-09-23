# مشخصات فرم frmProducts

## تنظیمات

| ویژگی | مقدار |
|---|---|
| نام | `frmProducts` |
| Record Source | `Products` |
| Caption | مدیریت کالاها |
| Default View | Single Form |
| Has Form Header | Yes |
| Navigation Buttons | Yes |

## کنترل‌ها

| نام | نوع | Control Source | برچسب فارسی |
|---|---|---|---|
| `lblTitle` | Label | — | مدیریت کالاها |
| `txtID` | Text Box | `ID` | شناسه (Locked) |
| `txtProductName` | Text Box | `ProductName` | نام کالا |
| `txtProductCode` | Text Box | `ProductCode` | کد کالا |
| `txtUnit` | Text Box | `Unit` | واحد |
| `txtCurrentStock` | Text Box | `CurrentStock` | موجودی فعلی |
| `txtMinimumStock` | Text Box | `MinimumStock` | حداقل موجودی |
| `chkIsActive` | Check Box | `IsActive` | فعال |
| `lblStockHint` | Label | — | وضعیت موجودی |
| `btnNew` | Command Button | — | جدید |
| `btnClose` | Command Button | — | بستن |

## پیش‌فرض‌ها

- `CurrentStock` = 0
- `MinimumStock` = 0
- `IsActive` = Yes

## کد

`vba/frmProducts_Code.bas`
