# انباربان v3.0.5 — آفلاین (بدون اینترنت روی PC انبار)

## دو فایل جدا در Release

| فایل | محتوا |
|------|--------|
| **`Anbarban-v3-App.zip`** | برنامه (`Anbarban.exe`، Data، ابزارها) |
| **`Anbarban-v3-Prerequisites.zip`** | .NET 4.8 (در صورت دانلود در CI) + **ACE 64-bit** + `نصب-پیش‌نیازها.bat` |

روی PC **بدون نت**: هر دو ZIP را یک‌بار (با USB یا PC دیگر) بگیرید.

## نصب روی انبار

1. هر دو ZIP را در **یک پوشه** Extract کنید (مثلاً `D:\Anbarban`).
2. مطمئن شوید `prerequisites\AccessDatabaseEngine_X64.exe` کنار `Anbarban.exe` است.
3. `prerequisites\نصب-پیش‌نیازها.bat` را اجرا کنید.
4. `شروع انباربان.bat`.

## داخل برنامه

- **بدون دانلود از اینترنت** — فقط فایل‌های محلی.
- دکمه‌ها: باز کردن پوشه `prerequisites`، نمایش فایل نصب در Explorer، شروع نصب ACE.

## سازگاری

`Anbarban-v3-Portable.zip` همان محتوای **App** است (نام قدیمی).
