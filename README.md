# سیستم انبارداری — Access 2010 (سبک)

پروژهٔ ساده برای کامپیوتر ضعیف (رم حدود ۴ گیگ / CPU قدیمی).
فقط Microsoft Access 2010 — بدون وابستگی اضافه.

## Repository

https://github.com/mmovasseghi/Anbardari-Access-2010

## چه کار می‌کند؟

- کالاها
- سند ورود (`IN`) و خروج (`OUT`)
- چند قلم در هر سند
- موجودی منفی نمی‌شود
- برای خروج، شماره حواله الزامی است
- گزارش موجودی / کم‌موجودی / ورود-خروج / جستجو

## ساخت روی ویندوز (خلاصه)

1. Access 2010 → Blank Database → `database/Inventory.accdb`
2. جداول را از `docs/SCHEMA.md` بسازید
3. روابط را از `sql/02_relationships.md` بگذارید
4. سه ماژول VBA: `modConstants` / `modValidation` / `modStock`
5. کوئری‌های پوشه `queries/`
6. فرم‌ها از پوشه `forms/` + کدهای `vba/*_Code.bas`
7. گزارش‌ها از `reports/REPORTS.md`
8. Display Form = `frmMain`

راهنمای کامل: `docs/BUILD_GUIDE.md`

## ظاهر (سبک، یک‌بار در Design)

رنگ/فونت را در Design View تنظیم کنید — نه با VBA سنگین هنگام باز شدن فرم.
جزئیات: `docs/UI_DESIGN.md`

## نکات سیستم ضعیف

- فقط ۳ ماژول VBA
- بدون تم پویا در Form_Load
- فرم‌ها Single Form ساده
- تعداد کم کنترل
- فایل `.accdb` را Compact & Repair گاه‌به‌گاه بزنید
