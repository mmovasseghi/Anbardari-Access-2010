'==============================================================================
' Microsoft Access 2010 — جداول سیستم انباربان (۷ جدول اصلی)
' هر دستور را جداگانه در SQL View اجرا کنید یا از build/ساخت-دیتابیس.bat
'==============================================================================

' --- Products — کالاها ---
CREATE TABLE Products (
    ID AUTOINCREMENT CONSTRAINT PK_Products PRIMARY KEY,
    ProductName TEXT(100) NOT NULL,
    ProductCode TEXT(50),
    Unit TEXT(20),
    CurrentStock LONG DEFAULT 0,
    MinimumStock LONG DEFAULT 0,
    IsActive YESNO DEFAULT True
);

' --- Suppliers — فروشندگان ---
CREATE TABLE Suppliers (
    ID AUTOINCREMENT CONSTRAINT PK_Suppliers PRIMARY KEY,
    SupplierName TEXT(100) NOT NULL,
    SupplierInfo TEXT(255),
    IsActive YESNO DEFAULT True
);

' --- Departments — بخش‌ها ---
CREATE TABLE Departments (
    ID AUTOINCREMENT CONSTRAINT PK_Departments PRIMARY KEY,
    DepartmentName TEXT(100) NOT NULL,
    IsActive YESNO DEFAULT True
);

' --- IncomingDocuments — اسناد ورود ---
CREATE TABLE IncomingDocuments (
    ID AUTOINCREMENT CONSTRAINT PK_IncomingDocuments PRIMARY KEY,
    DocumentNumber TEXT(50) NOT NULL,
    InvoiceNumber TEXT(50),
    DocumentDate DATETIME,
    SupplierID LONG NOT NULL,
    Description TEXT(255),
    IsPosted YESNO DEFAULT False,
    PostedAt DATETIME
);

' --- IncomingItems — اقلام ورود ---
CREATE TABLE IncomingItems (
    ID AUTOINCREMENT CONSTRAINT PK_IncomingItems PRIMARY KEY,
    IncomingDocumentID LONG NOT NULL,
    ProductID LONG NOT NULL,
    Quantity LONG NOT NULL
);

' --- OutgoingDocuments — اسناد خروج / حواله ---
CREATE TABLE OutgoingDocuments (
    ID AUTOINCREMENT CONSTRAINT PK_OutgoingDocuments PRIMARY KEY,
    DocumentNumber TEXT(50) NOT NULL,
    DeliveryNumber TEXT(50) NOT NULL,
    DocumentDate DATETIME,
    Description TEXT(255),
    IsPosted YESNO DEFAULT False,
    PostedAt DATETIME
);

' --- OutgoingItems — اقلام خروج ---
CREATE TABLE OutgoingItems (
    ID AUTOINCREMENT CONSTRAINT PK_OutgoingItems PRIMARY KEY,
    OutgoingDocumentID LONG NOT NULL,
    ProductID LONG NOT NULL,
    Quantity LONG NOT NULL,
    DepartmentID LONG NOT NULL
);

' --- UsedUnlockCodes — کدهای یک‌بارمصرف مصرف‌شده (باز کردن سند نهایی) ---
CREATE TABLE UsedUnlockCodes (
    ID AUTOINCREMENT CONSTRAINT PK_UsedUnlockCodes PRIMARY KEY,
    UnlockCode TEXT(6) NOT NULL,
    DocKind TEXT(10) NOT NULL,
    DocumentID LONG NOT NULL,
    UsedAt DATETIME
);
