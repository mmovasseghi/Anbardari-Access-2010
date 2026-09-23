# شمای دیتابیس

هدف: Microsoft Access 2010

## Products

| فیلد | نوع Access 2010 | توضیحات |
|---|---|---|
| ID | AutoNumber | Primary Key |
| ProductName | Text (100) | نام کالا — الزامی |
| ProductCode | Text (50) | کد کالا |
| Unit | Text (20) | واحد |
| CurrentStock | Number (Long Integer) | موجودی فعلی — پیش‌فرض 0 |
| MinimumStock | Number (Long Integer) | حداقل موجودی — پیش‌فرض 0 |
| IsActive | Yes/No | فعال بودن — پیش‌فرض Yes |

## Documents

| فیلد | نوع Access 2010 | توضیحات |
|---|---|---|
| ID | AutoNumber | Primary Key |
| DocumentNumber | Text (50) | شماره سند |
| DeliveryNumber | Text (50) | شماره حواله — برای OUT الزامی |
| TransactionType | Text (10) | مقدار ذخیره‌شده: `IN` یا `OUT` |
| DocumentDate | Date/Time | تاریخ سند |
| Source | Text (100) | مبدأ |
| Destination | Text (100) | مقصد |
| Description | Text (255) | توضیحات |

## DocumentItems

| فیلد | نوع Access 2010 | توضیحات |
|---|---|---|
| ID | AutoNumber | Primary Key |
| DocumentID | Number (Long Integer) | FK → Documents.ID |
| ProductID | Number (Long Integer) | FK → Products.ID |
| Quantity | Number (Long Integer) | تعداد — باید > 0 |

## روابط

```
Documents (1) ──── (∞) DocumentItems
Products  (1) ──── (∞) DocumentItems
```

Referential Integrity: فعال

## منطق موجودی

```
اگر TransactionType = IN  → CurrentStock = CurrentStock + Quantity
اگر TransactionType = OUT → CurrentStock = CurrentStock - Quantity
CurrentStock هیچ‌وقت < 0 نمی‌شود
```
