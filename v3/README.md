# انباربان نسخه ۳ — اپ WPF + پایگاه Access

## معماری

```
┌─────────────────────────────┐
│  Anbarban.exe (WPF .NET 4.8) │  ← UI زیبا، فارسی، RTL
│  Win7+ | پرتابل | سبک        │
└──────────────┬──────────────┘
               │ OLE DB (ACE 12.0)
               ▼
┌─────────────────────────────┐
│  Data\Inventory.accdb        │  ← همان شمای v2 (۷ جدول)
│  بدون فرم/گزارش در Access    │
└─────────────────────────────┘
```

- **منطق کسب‌وکار:** همان v2 (ثبت دو مرحله‌ای، شمسی، کد مدیر، …) — مرحله‌به‌مرحله به C# منتقل می‌شود.
- **Access:** فقط جداول + روابط؛ ساخت اولیه با `build/ساخت-دیتابیس.bat` از v2 (بدون نیاز به فرم‌های Access).

## پیش‌نیاز روی PC انبار (Win7 + 4GB)

| مورد | توضیح |
|------|--------|
| .NET Framework **4.8** | [دانلود مایکروسافت](https://dotnet.microsoft.com/download/dotnet-framework/net48) |
| **ACE OLEDB 12.0** | Access Database Engine 2010 Redistributable — معمولاً **x86** روی PCهای قدیمی |
| `Inventory.accdb` | در پوشه `Data` کنار exe |

## ساخت روی ویندوز (توسعه‌دهنده)

```bat
cd v3
dotnet build Anbarban.sln -c Release
```

یا Visual Studio 2019/2022 → باز کردن `Anbarban.sln` → Build Release.

خروجی: `Anbarban.Wpf\bin\Release\net48\`

## بسته پرتابل

```bat
v3\scripts\Make-Portable.bat
```

ساختار پیشنهادی:

```
Anbarban-v3-Portable/
  Anbarban.exe
  Anbarban.exe.config
  Data/Inventory.accdb
  Assets/logo.png
  راهنما.txt
```

## وضعیت

**نسخه ۳ کامل است** (منطق v2 در اپ WPF). جزئیات: `docs/V3_ROADMAP.md`

| بخش | وضعیت |
|-----|--------|
| ورود / خروج دو مرحله‌ای | ✅ |
| کالا، فروشنده، بخش | ✅ |
| گزارش‌ها + موجودی | ✅ |
| کد مدیر | ✅ |
| build **x64** | ✅ |

## لوگو

فایل: `Anbarban.Wpf/Assets/logo.png`
