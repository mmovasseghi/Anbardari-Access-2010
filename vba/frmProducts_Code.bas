'------------------------------------------------------------------------------
' frmProducts — مدیریت کالاها — Access 2010
' موجودی فعلی فقط خواندنی است
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "مدیریت کالاها"
    On Error Resume Next
    Me!txtCurrentStock.Locked = True
    Me!txtCurrentStock.Enabled = True
    Me!txtCurrentStock.TabStop = False
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    If IsBlank(Me!ProductName) Then
        MsgBox ERR_PRODUCT_NAME, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If
    If Not IsNull(Me!MinimumStock) Then
        If Me!MinimumStock < 0 Then
            MsgBox "حداقل موجودی نمی‌تواند منفی باشد.", vbExclamation, MSG_TITLE
            Cancel = True
        End If
    End If
End Sub

Private Sub btnNew_Click()
    DoCmd.GoToRecord , , acNewRec
    Me!IsActive = True
    Me!CurrentStock = 0
    Me!MinimumStock = 0
End Sub

Private Sub btnSave_Click()
    On Error GoTo EH
    DoCmd.RunCommand acCmdSaveRecord
    MsgBox "اطلاعات کالا ذخیره شد.", vbInformation, MSG_TITLE
    Exit Sub
EH:
    MsgBox "ذخیره انجام نشد. نام کالا را بررسی کنید.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnFind_Click()
    Dim s As String
    s = InputBox("نام یا کد کالا را وارد کنید:", MSG_TITLE)
    If Len(Trim$(s)) = 0 Then Exit Sub
    Me.Filter = "ProductName Like '*" & Replace(s, "'", "''") & "*' OR ProductCode Like '*" & Replace(s, "'", "''") & "*'"
    Me.FilterOn = True
End Sub

Private Sub btnShowAll_Click()
    Me.FilterOn = False
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub
