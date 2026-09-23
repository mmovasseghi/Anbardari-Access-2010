'------------------------------------------------------------------------------
' frmProductSearch — Access 2010 — minimal
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "جستجوی کالا"
End Sub

Private Sub btnSearch_Click()
    On Error GoTo EH
    Me!subResults.Requery
    Exit Sub
EH:
    MsgBox "خطا در جستجو: " & Err.Description, vbExclamation, MSG_TITLE
End Sub

Private Sub btnClear_Click()
    Me!txtName = Null
    Me!txtCode = Null
    On Error Resume Next
    Me!subResults.Requery
End Sub

Private Sub btnClose_Click()
    DoCmd.Close acForm, Me.Name
End Sub
