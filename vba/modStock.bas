'------------------------------------------------------------------------------
' modStock — موجودی — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Const TRANSACTION_IN As String = "IN"
Public Const TRANSACTION_OUT As String = "OUT"

Public Function ApplyStockChange( _
    ByVal productID As Long, _
    ByVal quantity As Long, _
    ByVal transactionType As String, _
    ByVal deltaSign As Long _
) As Boolean

    Dim rs As DAO.Recordset
    Dim signedQty As Long
    Dim newStock As Long
    Dim curStock As Long

    ApplyStockChange = False
    If productID <= 0 Or quantity <= 0 Then Exit Function

    transactionType = UCase$(Trim$(transactionType))
    If transactionType = TRANSACTION_IN Then
        signedQty = quantity * deltaSign
    ElseIf transactionType = TRANSACTION_OUT Then
        signedQty = -quantity * deltaSign
    Else
        Exit Function
    End If

    Set rs = CurrentDb.OpenRecordset("SELECT CurrentStock FROM Products WHERE ID=" & productID, dbOpenDynaset)
    If rs.EOF Then
        rs.Close
        Set rs = Nothing
        Exit Function
    End If

    curStock = Nz(rs!CurrentStock, 0)
    newStock = curStock + signedQty
    If newStock < 0 Then
        rs.Close
        Set rs = Nothing
        Exit Function
    End If

    rs.Edit
    rs!CurrentStock = newStock
    rs.Update
    rs.Close
    Set rs = Nothing
    ApplyStockChange = True
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

Public Function DocumentHasIncomingItems(ByVal documentID As Long) As Boolean
    Dim rs As DAO.Recordset
    DocumentHasIncomingItems = False
    If documentID <= 0 Then Exit Function
    Set rs = CurrentDb.OpenRecordset("SELECT ID FROM IncomingItems WHERE IncomingDocumentID=" & documentID, dbOpenSnapshot)
    DocumentHasIncomingItems = Not rs.EOF
    rs.Close
    Set rs = Nothing
End Function

Public Function DocumentHasOutgoingItems(ByVal documentID As Long) As Boolean
    Dim rs As DAO.Recordset
    DocumentHasOutgoingItems = False
    If documentID <= 0 Then Exit Function
    Set rs = CurrentDb.OpenRecordset("SELECT ID FROM OutgoingItems WHERE OutgoingDocumentID=" & documentID, dbOpenSnapshot)
    DocumentHasOutgoingItems = Not rs.EOF
    rs.Close
    Set rs = Nothing
End Function

Public Sub GoMainMenu()
    On Error Resume Next
    DoCmd.Close acForm, Screen.ActiveForm.Name
    DoCmd.OpenForm "frmMain"
End Sub
