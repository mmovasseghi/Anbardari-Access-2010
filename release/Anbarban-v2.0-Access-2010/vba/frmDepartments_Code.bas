'------------------------------------------------------------------------------
' frmDepartments — مدیریت بخش‌ها
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "مدیریت بخش‌ها"
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    If IsBlank(Me!DepartmentName) Then
        MsgBox "نام بخش را وارد کنید.", vbExclamation, MSG_TITLE
        Cancel = True
    End If
End Sub

Private Sub btnNew_Click()
    DoCmd.GoToRecord , , acNewRec
    Me!IsActive = True
End Sub

Private Sub btnSave_Click()
    On Error GoTo EH
    DoCmd.RunCommand acCmdSaveRecord
    MsgBox "ذخیره شد.", vbInformation, MSG_TITLE
    Exit Sub
EH:
    MsgBox "ذخیره انجام نشد.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub
