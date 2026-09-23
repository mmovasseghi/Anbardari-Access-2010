# مشخصات frmDocuments — ثبت ورود یا خروج

یک فرم مشترک؛ از منوی اصلی با OpenArgs باز می‌شود:

- `IN` → ثبت ورود کالا
- `OUT` → ثبت خروج / حواله

Record Source: `Documents`

## کنترل‌های قابل‌دید (فارسی)

| کنترل | ورود | خروج |
|---|---|---|
| `lblTitle` | ثبت ورود کالا | ثبت خروج / حواله |
| `txtDocumentNumber` | شماره سند | شماره سند |
| `txtDocumentDate` | تاریخ | تاریخ |
| `txtDeliveryNumber` + `lblDeliveryNumber` | مخفی | شماره حواله (اجباری) |
| `txtSource` | مبدأ | مبدأ |
| `txtDestination` | مقصد | مقصد |
| `txtDescription` | توضیحات | توضیحات |
| `subDocumentItems` | اقلام | اقلام |
| `btnSave` | ذخیره سند | ذخیره سند |
| `btnNew` | سند جدید | سند جدید |
| `btnBack` | بازگشت به منوی اصلی | بازگشت به منوی اصلی |

## کنترل‌های مخفی از کاربر

| کنترل | توضیح |
|---|---|
| `cboTransactionType` / `TransactionType` | خودکار IN یا OUT |
| `txtID` | اختیاری مخفی |

## Subform

`subDocumentItems` → `frmDocumentItems`  
Link Master=`ID` / Child=`DocumentID`

کد: `vba/frmDocuments_Code.bas`
