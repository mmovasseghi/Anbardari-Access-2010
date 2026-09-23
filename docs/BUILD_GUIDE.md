# راهنمای ساخت — Access 2010 (اپراتورمحور، سبک)

## 1) فایل
Blank Database → `database/Inventory.accdb`

## 2) جداول و روابط
طبق `docs/SCHEMA.md` و `sql/02_relationships.md`

## 3) ماژول‌های VBA (فقط ۳ تا)
1. `modConstants`
2. `modValidation`
3. `modStock`

## 4) کوئری‌ها
از پوشه `queries/` بسازید (نام‌ها دقیق):

- qryStock
- qryLowStock
- qryStockView
- qryProductSearch
- qryInOut
- qryProductMovement

## 5) فرم‌ها (ترتیب ساخت)

1. `frmDocumentItems` (Datasheet)
2. `frmDocuments` + Subform
3. `frmProducts`
4. `frmProductSearchResults` (Datasheet از qryProductSearch) → داخل `frmProductSearch`
5. `frmStockResults` (Datasheet از qryStockView) → داخل `frmStockView`
6. `frmMovementResults` (Datasheet از qryProductMovement) → داخل `frmProductMovement`
7. `frmInOutReport`
8. `frmMain`

کد هر فرم را از `vba/*_Code.bas` کپی کنید.

## 6) گزارش‌ها
- `rptLowStock` ← qryLowStock
- `rptInOut` ← qryInOut (Landscape)
- اختیاری: `rptStock` ← qryStock

## 7) Startup
Display Form = `frmMain`

## 8) تست اپراتوری
1. کالای جدید (موجودی دستی قفل است)
2. ثبت ورود کالا با چند قلم
3. ثبت خروج بدون حواله → پیام فارسی
4. خروج بیش از موجودی → «موجودی فعلی: …»
5. جستجوی کالا با چند حرف
6. مشاهده موجودی + فقط کم‌موجودی
7. گزارش ورود و خروج با فیلتر تاریخ
8. گردش یک کالا از جستجو

جزئیات UX: `docs/OPERATOR_UX.md`
