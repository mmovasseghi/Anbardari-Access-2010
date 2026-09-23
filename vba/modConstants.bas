'------------------------------------------------------------------------------
' modConstants — Access 2010 — پیام‌های فارسی برای اپراتور
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Const TRANSACTION_IN As String = "IN"
Public Const TRANSACTION_OUT As String = "OUT"
Public Const MSG_TITLE As String = "سیستم انبارداری"

Public Const ERR_QTY_POSITIVE As String = "تعداد باید بیشتر از صفر باشد."
Public Const ERR_DELIVERY_REQUIRED As String = "برای خروج کالا، شماره حواله را وارد کنید."
Public Const ERR_STOCK_NEGATIVE As String = "موجودی این کالا کافی نیست."
Public Const ERR_PRODUCT_REQUIRED As String = "لطفاً کالا را از فهرست انتخاب کنید."
Public Const ERR_DOC_REQUIRED As String = "ابتدا اطلاعات بالای سند را ذخیره کنید، بعد اقلام را وارد کنید."
Public Const ERR_TX_REQUIRED As String = "نوع سند مشخص نیست. از منوی اصلی ورود یا خروج را انتخاب کنید."
Public Const ERR_PRODUCT_NAME As String = "نام کالا را وارد کنید."
Public Const ERR_DATE_RANGE As String = "تاریخ «از» نباید بعد از تاریخ «تا» باشد."
