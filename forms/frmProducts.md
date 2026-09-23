# مشخصات فرم frmProducts

## تنظیمات فرم

| ویژگی | مقدار |
|---|---|
| نام | `frmProducts` |
| Record Source | `Products` |
| Caption | مدیریت کالاها |
| Default View | Single Form یا Continuous Form |
| Navigation Buttons | Yes |

## کنترل‌ها (همه با Control Source متناظر)

| نام کنترل | نوع | Control Source | برچسب فارسی |
|---|---|---|---|
| `txtID` | Text Box | `ID` | شناسه (Locked = Yes) |
| `txtProductName` | Text Box | `ProductName` | نام کالا |
| `txtProductCode` | Text Box | `ProductCode` | کد کالا |
| `txtUnit` | Text Box | `Unit` | واحد |
| `txtCurrentStock` | Text Box | `CurrentStock` | موجودی فعلی |
| `txtMinimumStock` | Text Box | `MinimumStock` | حداقل موجودی |
| `chkIsActive` | Check Box | `IsActive` | فعال |
| `btnNew` | Command Button | — | جدید |
| `btnClose` | Command Button | — | بستن |

## پیش‌فرض‌ها

- `CurrentStock` پیش‌فرض: `0`
- `MinimumStock` پیش‌فرض: `0`
- `IsActive` پیش‌فرض: `Yes`

## کد

محتوای `vba/frmProducts_Code.bas` را در ماژول فرم paste کنید.
