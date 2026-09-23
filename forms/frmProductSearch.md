# مشخصات فرم frmProductSearch

## تنظیمات فرم

| ویژگی | مقدار |
|---|---|
| نام | `frmProductSearch` |
| Record Source | (خالی — unbound) |
| Caption | جستجوی کالا |
| Default View | Single Form |
| Navigation Buttons | No |

## کنترل‌ها

| نام کنترل | نوع | Control Source | برچسب فارسی |
|---|---|---|---|
| `txtName` | Text Box | unbound | نام کالا |
| `txtCode` | Text Box | unbound | کد کالا |
| `btnSearch` | Command Button | — | جستجو |
| `btnClear` | Command Button | — | پاک کردن |
| `btnClose` | Command Button | — | بستن |
| `subResults` | Subform / Datasheet | — | نتایج |

## نتایج جستجو

ساده‌ترین روش Access 2010:

1. یک فرم Continuous/Datasheet به نام `frmProductSearchResults` بسازید با Record Source = `qryProductSearch`
2. آن را به‌عنوان Source Object کنترل `subResults` قرار دهید

یا:

- یک List Box / Datasheet ساده به کوئری وصل کنید

## کد

محتوای `vba/frmProductSearch_Code.bas` را در ماژول فرم paste کنید.

## وابستگی

کوئری `qryProductSearch` باید وجود داشته باشد (فایل `queries/qryProductSearch.sql`).
