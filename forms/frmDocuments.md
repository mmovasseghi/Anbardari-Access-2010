# مشخصات فرم frmDocuments

## تنظیمات

| ویژگی | مقدار |
|---|---|
| نام | `frmDocuments` |
| Record Source | `Documents` |
| Caption | ثبت سند انبار |
| Default View | Single Form |
| Has Form Header | Yes |
| Navigation Buttons | Yes |

## کنترل‌ها

| نام | نوع | Control Source | برچسب فارسی |
|---|---|---|---|
| `lblTitle` | Label | — | ثبت سند انبار (در Header) |
| `txtID` | Text Box | `ID` | شناسه (Locked) |
| `txtDocumentNumber` | Text Box | `DocumentNumber` | شماره سند |
| `txtDeliveryNumber` | Text Box | `DeliveryNumber` | شماره حواله |
| `lblDeliveryNumber` | Label | — | شماره حواله: |
| `cboTransactionType` | Combo Box | `TransactionType` | نوع تراکنش |
| `txtDocumentDate` | Text Box | `DocumentDate` | تاریخ سند |
| `txtSource` | Text Box | `Source` | مبدأ |
| `txtDestination` | Text Box | `Destination` | مقصد |
| `txtDescription` | Text Box | `Description` | توضیحات |
| `lblSectionItems` | Label | — | اقلام سند |
| `subDocumentItems` | Subform | — | Source Object = `frmDocumentItems` |
| `btnSave` | Command Button | — | ذخیره |
| `btnNew` | Command Button | — | سند جدید |
| `btnClose` | Command Button | — | بستن |

## Combo نوع تراکنش

| ویژگی | مقدار |
|---|---|
| Row Source Type | Value List |
| Row Source | `IN;ورود;OUT;خروج` |
| Column Count | 2 |
| Column Widths | `0cm;3cm` |
| Bound Column | 1 |
| Limit To List | Yes |

## Subform

| ویژگی | مقدار |
|---|---|
| Name | `subDocumentItems` |
| Source Object | `frmDocumentItems` |
| Link Master Fields | `ID` |
| Link Child Fields | `DocumentID` |

## رفتار مهم

- برای OUT، حواله الزامی است
- بعد از ثبت اقلام، نوع تراکنش قفل می‌شود
- تاریخ اگر خالی باشد، تاریخ روز گذاشته می‌شود

## کد

`vba/frmDocuments_Code.bas`
