# راهنمای ساخت دیتابیس در Microsoft Access 2010

این راهنما مخصوص کسی است که Repository را روی ویندوز دانلود کرده و Access 2010 دارد.

## پیش‌نیاز

- Windows
- Microsoft Access 2010
- فایل‌های همین Repository

## مرحله 1 — ساخت فایل پایگاه‌داده

1. Access 2010 را باز کنید.
2. **Blank Database** را انتخاب کنید.
3. نام فایل را مثلاً `Inventory.accdb` بگذارید.
4. محل ذخیره: ترجیحاً پوشه `database/` داخل همین پروژه.

## مرحله 2 — ساخت جداول

برای هر جدول: Create → Table Design

### جدول Products

| Field Name | Data Type | Field Size / تنظیمات |
|---|---|---|
| ID | AutoNumber | Primary Key |
| ProductName | Text | 100 — Required = Yes |
| ProductCode | Text | 50 |
| Unit | Text | 20 |
| CurrentStock | Number | Long Integer — Default = 0 |
| MinimumStock | Number | Long Integer — Default = 0 |
| IsActive | Yes/No | Default = Yes |

ذخیره با نام: `Products`

### جدول Documents

| Field Name | Data Type | Field Size / تنظیمات |
|---|---|---|
| ID | AutoNumber | Primary Key |
| DocumentNumber | Text | 50 |
| DeliveryNumber | Text | 50 |
| TransactionType | Text | 10 — Required = Yes |
| DocumentDate | Date/Time | Default می‌تواند Date() باشد |
| Source | Text | 100 |
| Destination | Text | 100 |
| Description | Text | 255 |

ذخیره با نام: `Documents`

### جدول DocumentItems

| Field Name | Data Type | Field Size / تنظیمات |
|---|---|---|
| ID | AutoNumber | Primary Key |
| DocumentID | Number | Long Integer — Required = Yes |
| ProductID | Number | Long Integer — Required = Yes |
| Quantity | Number | Long Integer — Required = Yes |

ذخیره با نام: `DocumentItems`

جزئیات بیشتر: `docs/SCHEMA.md` و `sql/01_create_tables.sql`

## مرحله 3 — روابط

1. Database Tools → Relationships
2. هر سه جدول را اضافه کنید.
3. `Documents.ID` را روی `DocumentItems.DocumentID` بکشید → Enforce Referential Integrity
4. `Products.ID` را روی `DocumentItems.ProductID` بکشید → Enforce Referential Integrity
5. ذخیره Relationships

جزئیات: `sql/02_relationships.md`

## مرحله 4 — ماژول‌های VBA

1. Alt + F11 (Visual Basic Editor)
2. Insert → Module
3. سه ماژول بسازید و محتوا را از این فایل‌ها کپی کنید:

| نام ماژول در Access | فایل سورس |
|---|---|
| `modConstants` | `vba/modConstants.bas` |
| `modValidation` | `vba/modValidation.bas` |
| `modStock` | `vba/modStock.bas` |
| `modUI` | `vba/modUI.bas` |

نکته: اگر Access هنگام Import فایل `.bas` خط Header می‌خواهد، فقط متن داخل فایل را Copy/Paste کنید (از `Option Compare Database` به بعد).

## مرحله 5 — کوئری‌ها

Create → Query Design → SQL View

| نام کوئری | فایل |
|---|---|
| `qryStock` | `queries/qryStock.sql` |
| `qryLowStock` | `queries/qryLowStock.sql` |
| `qryInOut` | `queries/qryInOut.sql` |
| `qryProductSearch` | `queries/qryProductSearch.sql` |

فقط متن SQL خالص را paste کنید (خطوط توضیح با `'` را حذف کنید اگر Access خطا داد).

## مرحله 5b — تم ظاهری

قبل از ساخت فرم‌ها، `docs/UI_DESIGN.md` را بخوانید.
ماژول `modUI` رنگ‌ها و فونت Tahoma را در `Form_Load` اعمال می‌کند.
نام کنترل‌های عنوان/دکمه باید با مشخصات فرم یکی باشد.

## مرحله 6 — فرم‌ها

طبق مشخصات پوشه `forms/` بسازید:

1. `frmProducts`
2. `frmDocumentItems` (Default View = Datasheet)
3. `frmDocuments` + Subform `subDocumentItems`
4. `frmProductSearch`
5. `frmMain`

برای هر فرم:

- کنترل‌ها را مطابق فایل مشخصات بسازید
- Captionها فارسی باشند
- کد رویداد را از فایل `vba/*_Code.bas` مربوطه در View Code فرم paste کنید

## مرحله 7 — گزارش‌ها

طبق `reports/REPORTS.md`:

- `rptStock` از `qryStock`
- `rptLowStock` از `qryLowStock`
- `rptInOut` از `qryInOut`

## مرحله 8 — تست سریع

1. از `frmProducts` چند کالا ثبت کنید (موجودی اولیه 0 یا مقدار دلخواه).
2. از `frmDocuments` یک سند **ورود** بسازید، ذخیره کنید، چند قلم وارد کنید.
3. موجودی کالاها باید افزایش یابد.
4. یک سند **خروج** با شماره حواله بسازید و خروج بزنید.
5. اگر تعداد خروج بیشتر از موجودی باشد، باید پیام خطا بگیرید و ذخیره نشود.
6. بدون شماره حواله نباید بتوان خروج ثبت کرد.
7. گزارش‌ها و جستجو را از منوی اصلی باز کنید.

## مرحله 9 — نمایش منوی اصلی هنگام باز شدن

File → Options → Current Database → Display Form = `frmMain`

## نکات مهم سازگاری Access 2010

- از Data Macros پیچیده Access جدیدتر لازم نیست؛ منطق در VBA است
- از نوع داده Short Text / Large Number استفاده نکنید
- از قابلیت‌های فقط Access 2013+ استفاده نکنید
- DAO کافی است؛ نیازی به ADO نیست

## بعد از ساخت

فایل نهایی `Inventory.accdb` را می‌توانید در پوشه `database/` قرار دهید و در Git commit کنید
(اگر حجم مناسب است و داده حساس ندارد).

برای شروع تمیز، می‌توانید یک نسخه بدون داده نمونه هم نگه دارید.
