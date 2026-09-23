# روابط دیتابیس (Access 2010)

در Access 2010 از منوی **Database Tools → Relationships** استفاده کنید.

## روابط مورد نیاز

| از | به | نوع | Referential Integrity |
|---|---|---|---|
| `Documents.ID` | `DocumentItems.DocumentID` | One-to-Many | فعال |
| `Products.ID` | `DocumentItems.ProductID` | One-to-Many | فعال |

## تنظیمات پیشنهادی هر رابطه

1. جداول `Documents`، `Products`، `DocumentItems` را به پنجره Relationships اضافه کنید.
2. فیلد کلید اصلی را روی فیلد خارجی بکشید.
3. گزینهٔ **Enforce Referential Integrity** را فعال کنید.
4. پیشنهاد: **Cascade Delete Related Records** فقط برای رابطهٔ Documents → DocumentItems فعال شود تا با حذف سند، اقلام آن هم حذف شوند.
5. برای Products → DocumentItems معمولاً Cascade Delete را خاموش بگذارید تا کالای دارای تراکنش به‌راحتی حذف نشود.

## ایندکس‌های پیشنهادی (اختیاری ولی مفید)

- `Products.ProductCode` — Indexed (Duplicates OK) یا No Duplicates اگر کد یکتا می‌خواهید
- `Documents.DocumentNumber` — Indexed (Duplicates OK)
- `Documents.TransactionType` — Indexed (Duplicates OK)
- `DocumentItems.DocumentID` — Indexed (Duplicates OK)
- `DocumentItems.ProductID` — Indexed (Duplicates OK)

## نکته Access 2010

رابطه را از رابط کاربری بسازید؛ دستور `ALTER TABLE ... ADD CONSTRAINT` در همهٔ محیط‌های Access 2010 پایدار نیست.
