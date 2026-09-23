# مشخصات فرم frmMain

منوی اصلی — برندمحور، ساده، سازگار با Access 2010.
جزئیات رنگ/فونت: `docs/UI_DESIGN.md`

## تنظیمات فرم

| ویژگی | مقدار |
|---|---|
| نام | `frmMain` |
| Record Source | (خالی) |
| Caption | سیستم انبارداری |
| Default View | Single Form |
| Scroll Bars | Neither |
| Record Selectors | No |
| Navigation Buttons | No |
| Auto Center | Yes |
| Border Style | Dialog یا Thin |
| Width | ~12cm |
| Detail Height | ~12cm |
| Header Height | ~1.8cm |

## کنترل‌ها

| نام | نوع | محل | متن |
|---|---|---|---|
| `lblBrand` | Label | Header | سیستم انبارداری |
| `lblSubtitle` | Label | Header | ورود، خروج و کنترل موجودی کالا |
| `boxAccent` | Label/Rectangle | زیر Header | (خالی — نوار رنگی) |
| `btnDocuments` | Command Button | Detail | ثبت سند انبار |
| `btnProducts` | Command Button | Detail | مدیریت کالاها |
| `btnStockReport` | Command Button | Detail | گزارش موجودی |
| `btnLowStockReport` | Command Button | Detail | گزارش کم‌موجودی |
| `btnInOutReport` | Command Button | Detail | گزارش ورود و خروج |
| `btnProductSearch` | Command Button | Detail | جستجوی کالا |
| `btnExit` | Command Button | Detail | خروج |

## اندازه دکمه‌ها

- عرض حدود `6cm`
- ارتفاع حدود `0.9cm`
- فاصله عمودی یکنواخت
- `btnDocuments` بالاترین دکمه عملیاتی (اقدام اصلی)

## کد

`vba/frmMain_Code.bas` را در ماژول فرم paste کنید.

## Startup

File → Options → Current Database → Display Form = `frmMain`
