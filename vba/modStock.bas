'------------------------------------------------------------------------------
' Module: modStock — Access 2010 / DAO only / low overhead
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

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

    If productID <= 0 Or quantity <= 0 Then
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
    Set rs = db.OpenRecordset("SELECT CurrentStock FROM Products WHERE ID=" & productID, dbOpenDynaset)

    If rs.EOF Then
        rs.Close
        Set rs = Nothing
        MsgBox "کالا یافت نشد.", vbExclamation, MSG_TITLE
        Exit Function
    End If

    newStock = Nz(rs!CurrentStock, 0) + signedQty
    If newStock < 0 Then
        rs.Close
        Set rs = Nothing
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
    Dim rs As DAO.Recordset
    GetDocumentTransactionType = ""
    If documentID <= 0 Then Exit Function
    Set rs = CurrentDb.OpenRecordset("SELECT TransactionType FROM Documents WHERE ID=" & documentID, dbOpenSnapshot)
    If Not rs.EOF Then GetDocumentTransactionType = Nz(rs!TransactionType, "")
    rs.Close
    Set rs = Nothing
End Function

Public Function GetProductCurrentStock(ByVal productID As Long) As Long
    Dim rs As DAO.Recordset
    GetProductCurrentStock = 0
    If productID <= 0 Then Exit Function
    Set rs = CurrentDb.OpenRecordset("SELECT CurrentStock FROM Products WHERE ID=" & productID, dbOpenSnapshot)
    If Not rs.EOF Then GetProductCurrentStock = Nz(rs!CurrentStock, 0)
    rs.Close
    Set rs = Nothing
End Function

Public Function DocumentHasItems(ByVal documentID As Long) As Boolean
    Dim rs As DAO.Recordset
    DocumentHasItems = False
    If documentID <= 0 Then Exit Function
    Set rs = CurrentDb.OpenRecordset("SELECT ID FROM DocumentItems WHERE DocumentID=" & documentID, dbOpenSnapshot)
    DocumentHasItems = Not rs.EOF
    rs.Close
    Set rs = Nothing
End Function
