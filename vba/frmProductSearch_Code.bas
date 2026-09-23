'------------------------------------------------------------------------------
' Form: frmProductSearch — code behind
' Target: Microsoft Access 2010
' Unbound search controls + datasheet/subform bound to qryProductSearch
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "جستجوی کالا"
End Sub

Private Sub btnSearch_Click()
    On Error GoTo EH
    Me!subResults.Form.RecordSource = "qryProductSearch"
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
    On Error GoTo 0
End Sub

Private Sub btnClose_Click()
    DoCmd.Close acForm, Me.Name
End Sub
