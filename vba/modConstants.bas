'------------------------------------------------------------------------------
' Module: modConstants
' Access 2010 — lightweight inventory
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Const TRANSACTION_IN As String = "IN"
Public Const TRANSACTION_OUT As String = "OUT"
Public Const MSG_TITLE As String = "سیستم انبارداری"

Public Const ERR_QTY_POSITIVE As String = "تعداد باید بیشتر از صفر باشد."
Public Const ERR_DELIVERY_REQUIRED As String = "برای خروج، شماره حواله الزامی است."
Public Const ERR_STOCK_NEGATIVE As String = "موجودی کافی نیست."
Public Const ERR_PRODUCT_REQUIRED As String = "انتخاب کالا الزامی است."
Public Const ERR_DOC_REQUIRED As String = "ابتدا سند را ذخیره کنید."
Public Const ERR_TX_REQUIRED As String = "نوع تراکنش را انتخاب کنید."
