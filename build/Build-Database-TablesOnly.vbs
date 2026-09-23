'==============================================================================
' Build-Database-TablesOnly.vbs
' فقط جداول و روابط — بدون فرم، بدون frmMain (برای انباربان v3)
' استفاده: cscript Build-Database-TablesOnly.vbs [مسیر\Inventory.accdb]
'==============================================================================
Option Explicit

Const dbRelationDeleteCascade = 4096

Dim fso, scriptDir, rootDir, dbPath, app, db, errCount

Set fso = CreateObject("Scripting.FileSystemObject")
scriptDir = fso.GetParentFolderName(WScript.ScriptFullName)
rootDir = fso.GetParentFolderName(scriptDir)

If WScript.Arguments.Count > 0 Then
  dbPath = WScript.Arguments(0)
Else
  dbPath = rootDir & "\database\Inventory.accdb"
End If

errCount = 0

Dim parentFolder
parentFolder = fso.GetParentFolderName(dbPath)
If Not fso.FolderExists(parentFolder) Then fso.CreateFolder parentFolder

On Error Resume Next
Set app = CreateObject("Access.Application")
If Err.Number <> 0 Then
  MsgBox "Microsoft Access یا ACE پیدا نشد." & vbCrLf & "برای v3 می‌توانید از داخل Anbarban.exe «ایجاد پایگاه» را بزنید.", 16, "انباربان"
  WScript.Quit 1
End If
On Error GoTo 0

app.Visible = False
app.UserControl = False

If fso.FileExists(dbPath) Then
  If MsgBox("فایل وجود دارد. جایگزین شود؟" & vbCrLf & dbPath, 4 + 32, "انباربان") <> 6 Then
    app.Quit
    WScript.Quit 0
  End If
  On Error Resume Next
  app.CloseCurrentDatabase
  fso.DeleteFile dbPath, True
  Err.Clear
  On Error GoTo 0
End If

On Error Resume Next
app.NewCurrentDatabase dbPath
If Err.Number <> 0 Then
  MsgBox "ساخت فایل ناموفق: " & Err.Description, 16, "انباربان"
  app.Quit
  WScript.Quit 1
End If
On Error GoTo 0

Set db = app.CurrentDb
CreateTables db
CreateRelations db

app.CloseCurrentDatabase
app.Quit

MsgBox "پایگاه آماده است (فقط جداول):" & vbCrLf & dbPath & vbCrLf & vbCrLf & "برای v3 این فایل را در پوشه Data کنار Anbarban.exe بگذارید.", 64, "انباربان"
WScript.Quit 0

' --- same as Build-Inventory.vbs (tables + relations only) ---
Sub SafeExec(db, sql)
  On Error Resume Next
  db.Execute sql, 64
  If Err.Number <> 0 Then errCount = errCount + 1
  Err.Clear
  On Error GoTo 0
End Sub

Sub CreateTables(db)
  SafeExec db, "CREATE TABLE Products (ID COUNTER PRIMARY KEY, ProductName TEXT(100), ProductCode TEXT(50), Unit TEXT(20), CurrentStock LONG, MinimumStock LONG, IsActive YESNO)"
  SafeExec db, "CREATE TABLE Suppliers (ID COUNTER PRIMARY KEY, SupplierName TEXT(100), SupplierInfo TEXT(255), IsActive YESNO)"
  SafeExec db, "CREATE TABLE Departments (ID COUNTER PRIMARY KEY, DepartmentName TEXT(100), IsActive YESNO)"
  SafeExec db, "CREATE TABLE IncomingDocuments (ID COUNTER PRIMARY KEY, DocumentNumber TEXT(50), InvoiceNumber TEXT(50), DocumentDate DATETIME, SupplierID LONG, Description TEXT(255), IsPosted YESNO, PostedAt DATETIME)"
  SafeExec db, "CREATE TABLE IncomingItems (ID COUNTER PRIMARY KEY, IncomingDocumentID LONG, ProductID LONG, Quantity LONG)"
  SafeExec db, "CREATE TABLE OutgoingDocuments (ID COUNTER PRIMARY KEY, DocumentNumber TEXT(50), DeliveryNumber TEXT(50), DocumentDate DATETIME, Description TEXT(255), IsPosted YESNO, PostedAt DATETIME)"
  SafeExec db, "CREATE TABLE OutgoingItems (ID COUNTER PRIMARY KEY, OutgoingDocumentID LONG, ProductID LONG, Quantity LONG, DepartmentID LONG)"
  SafeExec db, "CREATE TABLE UsedUnlockCodes (ID COUNTER PRIMARY KEY, UnlockCode TEXT(6), DocKind TEXT(10), DocumentID LONG, UsedAt DATETIME)"
End Sub

Sub CreateRelations(db)
  Dim rel, fld
  On Error Resume Next
  Set rel = db.CreateRelation("Suppliers_IncomingDocuments", "Suppliers", "IncomingDocuments", 0)
  Set fld = rel.CreateField("ID")
  fld.ForeignName = "SupplierID"
  rel.Fields.Append fld
  db.Relations.Append rel
  Err.Clear
  Set rel = db.CreateRelation("IncomingDocuments_IncomingItems", "IncomingDocuments", "IncomingItems", dbRelationDeleteCascade)
  Set fld = rel.CreateField("ID")
  fld.ForeignName = "IncomingDocumentID"
  rel.Fields.Append fld
  db.Relations.Append rel
  Err.Clear
  Set rel = db.CreateRelation("Products_IncomingItems", "Products", "IncomingItems", 0)
  Set fld = rel.CreateField("ID")
  fld.ForeignName = "ProductID"
  rel.Fields.Append fld
  db.Relations.Append rel
  Err.Clear
  Set rel = db.CreateRelation("OutgoingDocuments_OutgoingItems", "OutgoingDocuments", "OutgoingItems", dbRelationDeleteCascade)
  Set fld = rel.CreateField("ID")
  fld.ForeignName = "OutgoingDocumentID"
  rel.Fields.Append fld
  db.Relations.Append rel
  Err.Clear
  Set rel = db.CreateRelation("Products_OutgoingItems", "Products", "OutgoingItems", 0)
  Set fld = rel.CreateField("ID")
  fld.ForeignName = "ProductID"
  rel.Fields.Append fld
  db.Relations.Append rel
  Err.Clear
  Set rel = db.CreateRelation("Departments_OutgoingItems", "Departments", "OutgoingItems", 0)
  Set fld = rel.CreateField("ID")
  fld.ForeignName = "DepartmentID"
  rel.Fields.Append fld
  db.Relations.Append rel
  Err.Clear
  On Error GoTo 0
End Sub
