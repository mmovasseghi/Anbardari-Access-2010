# انباربان v3.0 — اپ ویندوزی + Access

## تفاوت با v2

| v2 | v3 |
|----|-----|
| فرم داخل Access | **اپ WPF جدا** (`Anbarban.exe`) |
| UI ساده Access | UI تیره، لوگو، دکمه‌های بزرگ |
| همان `Inventory.accdb` | همان دیتابیس — **بدون فرم Access** |

## دانلود

سورس و اسکریپت ساخت در GitHub. فایل exe روی **ویندوز 64-bit** با:

```bat
cd v3
dotnet build Anbarban.sln -c Release
scripts\Make-Portable.bat
```

خروجی: `release\Anbarban-v3-Portable\`

## پیش‌نیاز PC انبار

1. Windows 7 SP1+ (64-bit)
2. .NET Framework 4.8
3. **Microsoft Access Database Engine 2010 Redistributable — 64-bit** (ACE OLEDB)
4. `Data\Inventory.accdb` (از `build\ساخت-دیتابیس.bat` v2)

## امکانات

- ثبت ورود و خروج دو مرحله‌ای
- فروشنده، بخش، کالا
- تاریخ شمسی
- گزارش‌ها (جدول + کپی به Excel)
- قفل سند + کد مدیر (`tools\مولد-کد-مدیر.bat`)

## بکاپ

کپی `Data\Inventory.accdb`
