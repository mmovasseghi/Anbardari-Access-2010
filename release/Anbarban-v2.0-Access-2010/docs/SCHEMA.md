# شمای دیتابیس — انباربان (Access 2010)

## Products — کالاها

| فیلد | نوع | فارسی |
|---|---|---|
| ID | AutoNumber PK | شناسه |
| ProductName | Text(100) Required | نام کالا |
| ProductCode | Text(50) | کد کالا |
| Unit | Text(20) | واحد |
| CurrentStock | Long, Default 0 | موجودی فعلی — **فقط با ثبت نهایی ورود/خروج** |
| MinimumStock | Long, Default 0 | حداقل موجودی |
| IsActive | Yes/No, Default Yes | فعال |

## Suppliers — فروشندگان

| فیلد | نوع | فارسی |
|---|---|---|
| ID | AutoNumber PK | شناسه |
| SupplierName | Text(100) Required | نام فروشنده |
| SupplierInfo | Text(255) | مشخصات |
| IsActive | Yes/No | فعال |

## Departments — بخش‌ها

| فیلد | نوع | فارسی |
|---|---|---|
| ID | AutoNumber PK | شناسه |
| DepartmentName | Text(100) Required | نام بخش |
| IsActive | Yes/No | فعال |

## IncomingDocuments — اسناد ورود

| فیلد | نوع | فارسی |
|---|---|---|
| ID | AutoNumber PK | شناسه |
| DocumentNumber | Text(50) Required | شماره سند |
| InvoiceNumber | Text(50) | شماره فاکتور |
| DocumentDate | Date/Time | تاریخ (ذخیره میلادی؛ نمایش شمسی) |
| SupplierID | Long FK → Suppliers | فروشنده |
| Description | Text(255) | توضیحات |
| IsPosted | Yes/No, Default No | ثبت نهایی شده |
| PostedAt | Date/Time | زمان ثبت نهایی |

## IncomingItems — اقلام ورود

| فیلد | نوع | فارسی |
|---|---|---|
| ID | AutoNumber PK | شناسه |
| IncomingDocumentID | Long FK | سند ورود |
| ProductID | Long FK | کالا |
| Quantity | Long | تعداد (>0) |

## OutgoingDocuments — اسناد خروج

| فیلد | نوع | فارسی |
|---|---|---|
| ID | AutoNumber PK | شناسه |
| DocumentNumber | Text(50) Required | شماره سند |
| DeliveryNumber | Text(50) Required | شماره حواله |
| DocumentDate | Date/Time | تاریخ |
| Description | Text(255) | توضیحات |
| IsPosted | Yes/No | ثبت نهایی |
| PostedAt | Date/Time | زمان ثبت نهایی |

## OutgoingItems — اقلام خروج

| فیلد | نوع | فارسی |
|---|---|---|
| ID | AutoNumber PK | شناسه |
| OutgoingDocumentID | Long FK | سند خروج |
| ProductID | Long FK | کالا |
| Quantity | Long | تعداد |
| DepartmentID | Long FK | بخش تحویل‌گیرنده |

## UsedUnlockCodes — audit کد مدیر

| فیلد | نوع | توضیح |
|---|---|---|
| UnlockCode | Text(6) | کد مصرف‌شده |
| DocKind | Text(10) | `INCOMING` یا `OUTGOING` |
| DocumentID | Long | شناسه سند |
| UsedAt | Date/Time | زمان استفاده |

## روابط

```
Suppliers (1) ── (∞) IncomingDocuments
IncomingDocuments (1) ── (∞) IncomingItems ── Products
OutgoingDocuments (1) ── (∞) OutgoingItems ── Products
Departments (1) ── (∞) OutgoingItems
```

Cascade Delete: فقط از سند به اقلام (حذف سند پیش‌نویس → اقلام).

## منطق موجودی

```
IsPosted = False  → موجودی تغییر نمی‌کند
IsPosted = True   → ورود: +Quantity   خروج: -Quantity (هرگز منفی نشود)
```
