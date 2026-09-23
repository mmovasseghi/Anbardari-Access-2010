# مشخصات فرم frmDocuments

## تنظیمات فرم

| ویژگی | مقدار |
|---|---|
| نام | `frmDocuments` |
| Record Source | `Documents` |
| Caption | ثبت سند انبار |
| Default View | Single Form |
| Navigation Buttons | Yes |

## کنترل‌های هدر سند

| نام کنترل | نوع | Control Source | برچسب فارسی | نکات |
|---|---|---|---|---|
| `txtID` | Text Box | `ID` | شناسه | Locked = Yes |
| `txtDocumentNumber` | Text Box | `DocumentNumber` | شماره سند | |
| `txtDeliveryNumber` | Text Box | `DeliveryNumber` | شماره حواله | برای OUT الزامی |
| `lblDeliveryNumber` | Label | — | شماره حواله: | Caption در VBA عوض می‌شود |
| `cboTransactionType` | Combo Box | `TransactionType` | نوع تراکنش | Value List: `IN;ورود;OUT;خروج` |
| `txtDocumentDate` | Text Box | `DocumentDate` | تاریخ سند | Format: Short Date |
| `txtSource` | Text Box | `Source` | مبدأ | |
| `txtDestination` | Text Box | `Destination` | مقصد | |
| `txtDescription` | Text Box | `Description` | توضیحات | |
| `subDocumentItems` | Subform | — | اقلام سند | Source Object = `frmDocumentItems` |
| `btnSave` | Command Button | — | ذخیره | |
| `btnNew` | Command Button | — | سند جدید | |
| `btnClose` | Command Button | — | بستن | |

## تنظیم Combo نوع تراکنش

| ویژگی | مقدار |
|---|---|
| Row Source Type | Value List |
| Row Source | `IN;ورود;OUT;خروج` |
| Column Count | 2 |
| Column Widths | `0cm;3cm` |
| Bound Column | 1 |
| Limit To List | Yes |

کاربر «ورود / خروج» را می‌بیند؛ مقدار ذخیره‌شده `IN` / `OUT` است.

## Subform

| ویژگی Subform Control | مقدار |
|---|---|
| Name | `subDocumentItems` |
| Source Object | `frmDocumentItems` |
| Link Master Fields | `ID` |
| Link Child Fields | `DocumentID` |
| Default View (child) | Datasheet |

## کد

محتوای `vba/frmDocuments_Code.bas` را در ماژول فرم paste کنید.

## ترتیب کار کاربر

1. نوع تراکنش را انتخاب کند
2. برای خروج، شماره حواله را وارد کند
3. سند را ذخیره کند
4. اقلام را در Subform وارد کند
