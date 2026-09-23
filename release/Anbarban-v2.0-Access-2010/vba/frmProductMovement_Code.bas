'------------------------------------------------------------------------------
' frmProductMovement — گردش یک کالا — Access 2010
' OpenArgs = ProductID
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_ProductID As Long

Private Sub Form_Load()
    Me.Caption = "گزارش گردش کالا"
    m_ProductID = Val(Nz(Me.OpenArgs, "0"))
    If m_ProductID <= 0 Then
        MsgBox "کالا مشخص نشده است.", vbExclamation, MSG_TITLE
        Exit Sub
    End If
    Me!txtProductID = m_ProductID
    ShowHeader
    ApplyFilter
End Sub

Private Sub ShowHeader()
    Dim rs As DAO.Recordset
    On Error Resume Next
    Set rs = CurrentDb.OpenRecordset( _
        "SELECT ProductName, ProductCode, Unit, CurrentStock, MinimumStock FROM Products WHERE ID=" & m_ProductID, _
        dbOpenSnapshot)
    If Not rs.EOF Then
        Me!txtProductName = Nz(rs!ProductName, "")
        Me!txtProductCode = Nz(rs!ProductCode, "")
        Me!txtUnit = Nz(rs!Unit, "")
        Me!txtCurrentStock = Nz(rs!CurrentStock, 0)
        Me!txtStatus = StockStatusText(rs!CurrentStock, rs!MinimumStock)
    End If
    rs.Close
    Set rs = Nothing
End Sub

Private Sub btnShow_Click()
    If Not ValidateDateRange(Me!txtFromDate, Me!txtToDate) Then Exit Sub
    ApplyFilter
End Sub

Private Sub ApplyFilter()
    On Error GoTo EH
    Me!subMovement.Form.RecordSource = "qryProductMovement"
    Me!subMovement.Requery
    Exit Sub
EH:
    MsgBox "نمایش گردش کالا انجام نشد.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnClear_Click()
    Me!txtFromDate = Null
    Me!txtToDate = Null
    ApplyFilter
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
End Sub

Public Function CurrentProductID() As Long
    CurrentProductID = m_ProductID
End Function
