# مشخصات فرم frmProductSearch

## تنظیمات

| ویژگی | مقدار |
|---|---|
| نام | `frmProductSearch` |
| Record Source | (خالی) |
| Caption | جستجوی کالا |
| Default View | Single Form |
| Has Form Header | Yes |
| Navigation Buttons | No |

## کنترل‌ها

| نام | نوع | برچسب فارسی |
|---|---|---|
| `lblTitle` | Label | جستجوی کالا |
| `txtName` | Text Box (unbound) | نام کالا |
| `txtCode` | Text Box (unbound) | کد کالا |
| `btnSearch` | Command Button | جستجو |
| `btnClear` | Command Button | پاک کردن |
| `btnClose` | Command Button | بستن |
| `subResults` | Subform | نتایج (`qryProductSearch`) |

## نتایج

یک فرم Datasheet به نام `frmProductSearchResults` با Record Source = `qryProductSearch` بسازید و به‌عنوان Source Object کنترل `subResults` بگذارید.

## کد

`vba/frmProductSearch_Code.bas`
