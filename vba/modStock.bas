'------------------------------------------------------------------------------
' Module: modStock
' Target: Microsoft Access 2010
' Purpose: Stock calculation for IN / OUT document items
' Uses DAO only (Access 2010 compatible)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

' Apply stock change for a saved DocumentItems row.
' deltaSign: +1 for adding effect, -1 for reversing a previous effect
Public Function ApplyStockChange( _
    ByVal productID As Long, _
    ByVal quantity As Long, _
    ByVal transactionType As String, _
    ByVal deltaSign As Long _
) As Boolean

    Dim db As DAO.Database
    Dim rs As DAO.Recordset
    Dim signedQty As Long
    Dim newStock As Long

    ApplyStockChange = False

    If productID <= 0 Then
        MsgBox ERR_PRODUCT_REQUIRED, vbExclamation, MSG_TITLE
        Exit Function
    End If

    If quantity <= 0 Then
        MsgBox ERR_QTY_POSITIVE, vbExclamation, MSG_TITLE
        Exit Function
    End If

    transactionType = UCase$(Trim$(transactionType))

    If transactionType = TRANSACTION_IN Then
        signedQty = quantity * deltaSign
    ElseIf transactionType = TRANSACTION_OUT Then
        signedQty = -quantity * deltaSign
    Else
        MsgBox "نوع تراکنش نامعتبر است.", vbExclamation, MSG_TITLE
        Exit Function
    End If

    Set db = CurrentDb
    Set rs = db.OpenRecordset( _
        "SELECT ID, CurrentStock FROM Products WHERE ID = " & productID, _
        dbOpenDynaset)

    If rs.EOF Then
        rs.Close
        Set rs = Nothing
        Set db = Nothing
        MsgBox "کالای انتخاب‌شده یافت نشد.", vbExclamation, MSG_TITLE
        Exit Function
    End If

    newStock = Nz(rs!CurrentStock, 0) + signedQty

    If newStock < 0 Then
        rs.Close
        Set rs = Nothing
        Set db = Nothing
        MsgBox ERR_STOCK_NEGATIVE, vbExclamation, MSG_TITLE
        Exit Function
    End If

    rs.Edit
    rs!CurrentStock = newStock
    rs.Update

    rs.Close
    Set rs = Nothing
    Set db = Nothing

    ApplyStockChange = True
End Function

Public Function GetDocumentTransactionType(ByVal documentID As Long) As String
    Dim db As DAO.Database
    Dim rs As DAO.Recordset

    GetDocumentTransactionType = ""

    If documentID <= 0 Then Exit Function

    Set db = CurrentDb
    Set rs = db.OpenRecordset( _
        "SELECT TransactionType FROM Documents WHERE ID = " & documentID, _
        dbOpenSnapshot)

    If Not rs.EOF Then
        GetDocumentTransactionType = Nz(rs!TransactionType, "")
    End If

    rs.Close
    Set rs = Nothing
    Set db = Nothing
End Function

Public Function GetProductCurrentStock(ByVal productID As Long) As Long
    Dim db As DAO.Database
    Dim rs As DAO.Recordset

    GetProductCurrentStock = 0

    If productID <= 0 Then Exit Function

    Set db = CurrentDb
    Set rs = db.OpenRecordset( _
        "SELECT CurrentStock FROM Products WHERE ID = " & productID, _
        dbOpenSnapshot)

    If Not rs.EOF Then
        GetProductCurrentStock = Nz(rs!CurrentStock, 0)
    End If

    rs.Close
    Set rs = Nothing
    Set db = Nothing
End Function

' Preview whether an OUT quantity would make stock negative
Public Function CanIssueQuantity(ByVal productID As Long, ByVal quantity As Long) As Boolean
    If quantity <= 0 Then
        CanIssueQuantity = False
        Exit Function
    End If

    CanIssueQuantity = (GetProductCurrentStock(productID) >= quantity)
End Function
