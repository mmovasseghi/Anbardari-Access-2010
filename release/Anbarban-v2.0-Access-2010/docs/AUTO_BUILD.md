# ساخت خودکار دیتابیس

روی ویندوز با **Microsoft Access 2010**:

1. کل پوشه پروژه را کپی کنید  
2. بروید داخل پوشه `build`  
3. روی یکی از این‌ها دوبارکلیک کنید:
   - `ساخت-دیتابیس.bat`
   - یا `Build.bat`

اسکریپت خودش:

- فایل `database\Inventory.accdb` را می‌سازد  
- جداول و روابط را می‌سازد  
- ماژول‌های VBA را وارد می‌کند  
- فرم‌ها، کوئری‌ها و گزارش‌های پایه را می‌سازد  
- منوی اصلی را باز می‌کند  

## نیاز

- ویندوز  
- Microsoft Access 2010 (یا سازگار)  

## اگر آنتی‌ویروس گیر داد

روی `Build-Inventory.vbs` راست‌کلیک → Properties → Unblock (اگر بود)  
یا از Command Prompt:

```bat
cd build
cscript //nologo Build-Inventory.vbs
```

## لاگ

اگر مشکلی بود: `build\build-log.txt`
