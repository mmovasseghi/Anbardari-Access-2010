'------------------------------------------------------------------------------
' modConstants — انباربان — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Const MSG_TITLE As String = "انباربان"
Public Const DOC_INCOMING As String = "INCOMING"
Public Const DOC_OUTGOING As String = "OUTGOING"

Public Const ERR_QTY_POSITIVE As String = "تعداد باید بیشتر از صفر باشد."
Public Const ERR_DELIVERY_REQUIRED As String = "برای خروج کالا، شماره حواله را وارد کنید."
Public Const ERR_STOCK_NEGATIVE As String = "موجودی این کالا کافی نیست."
Public Const ERR_PRODUCT_REQUIRED As String = "لطفاً کالا را از فهرست انتخاب کنید."
Public Const ERR_DEPT_REQUIRED As String = "لطفاً بخش تحویل‌گیرنده را انتخاب کنید."
Public Const ERR_DOC_REQUIRED As String = "ابتدا اطلاعات بالای سند را ذخیره کنید، بعد اقلام را وارد کنید."
Public Const ERR_SUPPLIER_REQUIRED As String = "فروشنده را از فهرست انتخاب کنید."
Public Const ERR_DOCNO_REQUIRED As String = "شماره سند را وارد کنید."
Public Const ERR_ITEMS_REQUIRED As String = "حداقل یک قلم کالا وارد کنید."
Public Const ERR_POSTED_LOCKED As String = "این سند نهایی شده و قابل تغییر نیست. برای اصلاح از مدیر کد یک‌بارمصرف بگیرید."
Public Const ERR_PRODUCT_NAME As String = "نام کالا را وارد کنید."
Public Const ERR_DATE_RANGE As String = "تاریخ «از» نباید بعد از تاریخ «تا» باشد."
Public Const ERR_JALALI_INVALID As String = "تاریخ شمسی را درست وارد کنید. مثال: 1403/06/15"
Public Const ERR_UNLOCK_INVALID As String = "کد وارد شده معتبر نیست یا قبلاً استفاده شده است."
Public Const MSG_CONFIRM_INCOMING As String = "لطفاً اطلاعات زیر را با فاکتور اصلی مطابقت دهید."
Public Const MSG_CONFIRM_OUTGOING As String = "لطفاً اطلاعات زیر را با حواله اصلی مطابقت دهید."

' کلید سازمان برای مولد کد آفلاین (با برنامه جدا یکسان باشد)
Public Const UNLOCK_ORG_SALT As String = "Anbarban-Offline-2010"
