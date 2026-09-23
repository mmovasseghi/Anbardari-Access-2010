# سازگاری Microsoft Access 2010

این پروژه عمداً فقط از قابلیت‌های Access 2010 استفاده می‌کند.

## مجاز

- جداول ACE / `.accdb`
- انواع داده: AutoNumber, Text, Number (Long), Date/Time, Yes/No
- روابط با Referential Integrity
- فرم‌ها و Subform/Datasheet
- Combo Box با Value List و Table/Query
- گزارش‌های کلاسیک
- VBA با DAO
- کوئری‌های Select ساده Access SQL

## غیرمجاز / استفاده نشده

- Large Number
- Short Text (نام نوع داده نسخه‌های جدید)
- Data Macros پیشرفته به‌عنوان جایگزین اصلی منطق
- Power Automate / اتصال ابری اجباری
- ویژگی‌های فقط Access 2013 / 2016 / 365
- وب اپ، .NET، Python، یا وابستگی خارجی برای اجرای اصلی

## منطق کسب‌وکار

منطق موجودی و اعتبارسنجی در VBA ماژول‌ها و رویدادهای فرم پیاده شده تا روی Access 2010 پایدار بماند.
