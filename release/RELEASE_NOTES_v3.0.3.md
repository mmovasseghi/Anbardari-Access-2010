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
| **ACE OLEDB 64-bit** | در اولین اجرا **نصب/دانلود خودکار** از داخل برنامه (پوشه `redist`) |

## اولین اجرا

- اگر `Data\Inventory.accdb` نیست، پس از نصب ACE برنامه معمولاً **خودکار** پایگاه را می‌سازد.
- در غیر این صورت: **`ساخت-پایگاه-داده.bat`** (فقط جداول، بدون frmMain).

## رفع خطای «ACE ثبت نشده»

- تشخیص خودکار provider: `ACE 16.0 / 12.0 / 15.0`
- دیالوگ «انجام نشد» با **نصب خودکار**، دانلود از مایکروسافت، باز کردن `redist`، **دوباره امتحان**
- اسکریپت `redist/install-ace.ps1` برای دانلود `AccessDatabaseEngine_X64.exe`

## UI

- پیام‌های **AnbarbanDialog** — قابل **جابه‌جایی با موس** (مثل MessageBox)
- UI لوکس RTL، SearchCombo، تقویم شمسی، گزارش‌گیری ثابت پایین صفحه

## بکاپ

کپی منظم: `Data\Inventory.accdb`

## سورس

ریپو: `v3/` — `dotnet build Anbarban.sln -c Release`
