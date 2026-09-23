'------------------------------------------------------------------------------
' frmProducts — Access 2010 — minimal
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "مدیریت کالاها"
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    If IsBlank(Me!ProductName) Then
        MsgBox "نام کالا الزامی است.", vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If
    If Not IsNull(Me!CurrentStock) Then
        If Me!CurrentStock < 0 Then
            MsgBox ERR_STOCK_NEGATIVE, vbExclamation, MSG_TITLE
            Cancel = True
        End If
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
End Sub

Private Sub btnClose_Click()
    DoCmd.Close acForm, Me.Name
End Sub
