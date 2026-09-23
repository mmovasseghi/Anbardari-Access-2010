'------------------------------------------------------------------------------
' frmProductSearch — جستجوی ساده کالا — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "جستجوی کالا"
    On Error Resume Next
    Me!lblHint.Caption = "نام کالا یا کد کالا را وارد کنید"
End Sub

Private Sub btnSearch_Click()
    ApplySearch
End Sub

Private Sub txtSearch_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyReturn Then
        KeyCode = 0
        ApplySearch
    End If
End Sub

Private Sub ApplySearch()
    On Error GoTo EH
    Me!subResults.Form.RecordSource = "qryProductSearch"
    Me!subResults.Requery
    Exit Sub
EH:
    MsgBox "جستجو انجام نشد. متن جستجو را بررسی کنید.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnClear_Click()
    Me!txtSearch = Null
    On Error Resume Next
    Me!subResults.Requery
End Sub

Private Sub btnMovement_Click()
    Dim pid As Long
    On Error GoTo EH
    pid = Nz(Me!subResults.Form!ID, 0)
    If pid <= 0 Then
        MsgBox "ابتدا یک کالا را از نتایج انتخاب کنید.", vbExclamation, MSG_TITLE
        Exit Sub
    End If
    DoCmd.OpenForm "frmProductMovement", , , , , , CStr(pid)
    Exit Sub
EH:
    MsgBox "لطفاً یک ردیف از نتایج را انتخاب کنید.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub
