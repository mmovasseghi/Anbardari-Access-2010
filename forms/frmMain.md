# مشخصات فرم frmMain

## تنظیمات فرم

| ویژگی | مقدار |
|---|---|
| نام | `frmMain` |
| Record Source | (خالی — unbound) |
| Caption | منوی اصلی — سیستم انبارداری |
| Default View | Single Form |
| Scroll Bars | Neither |
| Record Selectors | No |
| Navigation Buttons | No |
| Pop Up | No |
| Modal | No |

## کنترل‌ها

| نام کنترل | نوع | Caption / متن | رویداد |
|---|---|---|---|
| `lblTitle` | Label | سیستم انبارداری | — |
| `btnProducts` | Command Button | مدیریت کالاها | `btnProducts_Click` |
| `btnDocuments` | Command Button | ثبت سند | `btnDocuments_Click` |
| `btnStockReport` | Command Button | گزارش موجودی | `btnStockReport_Click` |
| `btnLowStockReport` | Command Button | گزارش کم‌موجودی | `btnLowStockReport_Click` |
| `btnInOutReport` | Command Button | گزارش ورود و خروج | `btnInOutReport_Click` |
| `btnProductSearch` | Command Button | جستجوی کالا | `btnProductSearch_Click` |
| `btnExit` | Command Button | خروج | `btnExit_Click` |

## چیدمان پیشنهادی (ساده)

- عنوان بالا
- دکمه‌ها زیر هم یا در دو ستون ساده
- بدون کارت/تزیین اضافه

## کد

محتوای `vba/frmMain_Code.bas` را در ماژول فرم paste کنید.

## Startup Form (اختیاری)

File → Options → Current Database → Display Form = `frmMain`
