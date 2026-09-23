'------------------------------------------------------------------------------
' frmOutgoing — ثبت خروج / حواله
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "ثبت خروج / حواله"
    If Me.NewRecord Then
        Me!DocumentDate = Date
        Me!txtJalaliDate = FormatJalali(Date)
    End If
End Sub

Private Sub Form_Current()
    On Error Resume Next
    Call LoadJalaliFromDate(Me!DocumentDate, Me!txtJalaliDate)
    Call LockFormIfPosted(Me, Nz(Me!IsPosted, False))
    If Nz(Me!IsPosted, False) Then
        Me!lblPosted.Caption = "وضعیت: ثبت نهایی شده"
    Else
        Me!lblPosted.Caption = "وضعیت: پیش‌نویس"
    End If
End Sub

Private Sub txtJalaliDate_AfterUpdate()
    Call SyncJalaliToBoundDate(Me!txtJalaliDate, Me!DocumentDate)
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    If Nz(Me!IsPosted, False) Then
        MsgBox ERR_POSTED_LOCKED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If
    Call SyncJalaliToBoundDate(Me!txtJalaliDate, Me!DocumentDate)
    If Not ValidateOutgoingHeader(Me!DocumentNumber, Me!DeliveryNumber, Me!txtJalaliDate) Then
        Cancel = True
        Exit Sub
    End If
    If IsNull(Me!DocumentDate) Then Me!DocumentDate = Date
End Sub

Private Sub btnSave_Click()
    On Error GoTo EH
    DoCmd.RunCommand acCmdSaveRecord
    MsgBox "اطلاعات حواله ذخیره شد. حالا اقلام را وارد کنید.", vbInformation, MSG_TITLE
    Exit Sub
EH:
    MsgBox "ذخیره انجام نشد.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnReview_Click()
    On Error GoTo EH
    If Nz(Me!IsPosted, False) Then Exit Sub
    DoCmd.RunCommand acCmdSaveRecord
    If Nz(Me!ID, 0) = 0 Then Exit Sub
    If Not DocumentHasOutgoingItems(Nz(Me!ID, 0)) Then
        MsgBox ERR_ITEMS_REQUIRED, vbExclamation, MSG_TITLE
        Exit Sub
    End If
    DoCmd.OpenForm "frmOutgoingConfirm", , , , , , CStr(Me!ID)
    Exit Sub
EH:
End Sub

Private Sub btnUnlock_Click()
    If Not Nz(Me!IsPosted, False) Then Exit Sub
    If PromptUnlockIfPosted(DOC_OUTGOING, Nz(Me!ID, 0), Nz(Me!DocumentNumber, ""), True) Then
        Me.Requery
    End If
End Sub

Private Sub btnNew_Click()
    DoCmd.GoToRecord , , acNewRec
    Me!DocumentDate = Date
    Me!txtJalaliDate = FormatJalali(Date)
    Me!IsPosted = False
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub
