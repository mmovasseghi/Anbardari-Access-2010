# انباربان v3.0.3 — پرتابل ویندوز (64-bit)

## دانلود

فایل **`Anbarban-v3-Portable.zip`** را از این Release بگیرید.

1. Extract در هر پوشه (مثلاً `D:\Anbarban`)
2. دوبارکلیک **`شروع انباربان.bat`**

## پیش‌نیاز (یک بار روی PC)

| مورد | |
|------|--|
| **Windows** | 7 یا بالاتر |
| **.NET Framework 4.8** | [دانلود](https://dotnet.microsoft.com/download/dotnet-framework/net48) |
| **ACE OLEDB 12.0 — 64-bit** | Access Database Engine 2010 Redistributable (هم‌تراز x64 اپ) |

## اولین اجرا

- اگر `Data\Inventory.accdb` نیست، برنامه در صورت نصب ACE معمولاً **خودکار** پایگاه را می‌سازد.
- در غیر این صورت: **`ساخت-پایگاه-داده.bat`** در همان پوشه (فقط جداول، بدون frmMain در Access).

## داخل ZIP

| فایل | |
|------|--|
| `شروع انباربان.bat` | اجرای برنامه |
| `Anbarban.exe` | اپ انباربان v3 |
| `Data\` | پایگاه `Inventory.accdb` |
| `ساخت-پایگاه-داده.bat` | ساخت دستی accdb |
| `tools\` | مولد کد مدیر |
| `راهنما.txt` / `راهنما-نسخه۳.txt` | راهنما |
| `شروع-بخوانید.txt` | خلاصه فارسی |

## تغییرات نسبت به v3.0.2

- UI لوکس RTL، Icons8، منوی راست، SearchCombo و تقویم شمسی
- پیام‌های **AnbarbanDialog** و صفحه موفقیت بعد ثبت نهایی ورود/خروج
- گزارش‌ها: دکمه **گزارش‌گیری** ثابت پایین صفحه
- ثبات: جلوگیری از ثبت نهایی دوباره؛ ورود بدون کرش فروشنده حذف‌شده
- تست: `Anbarban.Tests` (۳۰ سناریو) + `Anbarban.exe --ui-test`

## بکاپ

کپی منظم: `Data\Inventory.accdb`

## سورس

ریپو: `v3/` — `dotnet build Anbarban.sln -c Release`
