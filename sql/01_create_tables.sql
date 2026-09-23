'==============================================================================
' SQL for Microsoft Access 2010 — create tables
' Run each statement separately in Access:
'   Create > Query Design > SQL View > paste one statement > Run
' Or create tables manually using Table Design (see docs/BUILD_GUIDE.md).
' Note: Access SQL DDL support is limited; Table Design UI is the safest path.
'==============================================================================

' --- Products ---
CREATE TABLE Products (
    ID AUTOINCREMENT CONSTRAINT PK_Products PRIMARY KEY,
    ProductName TEXT(100) NOT NULL,
    ProductCode TEXT(50),
    Unit TEXT(20),
    CurrentStock LONG DEFAULT 0,
    MinimumStock LONG DEFAULT 0,
    IsActive YESNO DEFAULT True
);

' --- Documents ---
CREATE TABLE Documents (
    ID AUTOINCREMENT CONSTRAINT PK_Documents PRIMARY KEY,
    DocumentNumber TEXT(50),
    DeliveryNumber TEXT(50),
    TransactionType TEXT(10) NOT NULL,
    DocumentDate DATETIME,
    Source TEXT(100),
    Destination TEXT(100),
    Description TEXT(255)
);

' --- DocumentItems ---
CREATE TABLE DocumentItems (
    ID AUTOINCREMENT CONSTRAINT PK_DocumentItems PRIMARY KEY,
    DocumentID LONG NOT NULL,
    ProductID LONG NOT NULL,
    Quantity LONG NOT NULL
);
