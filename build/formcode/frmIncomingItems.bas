'------------------------------------------------------------------------------
' frmIncomingItems — اقلام ورود (بدون تغییر موجودی تا ثبت نهایی)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Call SetupProductSearchCombo(Me!cboProductID)
End Sub

Private Sub cboProductID_Change()
    Call RefreshProductComboSearch(Me!cboProductID)
End Sub

Private Sub Form_Current()
    On Error Resume Next
    If Nz(Me.Parent!IsPosted, False) Then
        Me.AllowEdits = False
    Else
        Me.AllowEdits = True
    End If
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    Dim parentFrm As Form

    On Error GoTo EH
    If Nz(Me.Parent!IsPosted, False) Then
        MsgBox ERR_POSTED_LOCKED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    Set parentFrm = Me.Parent
    If parentFrm.NewRecord Or Nz(parentFrm!ID, 0) = 0 Then
        MsgBox ERR_DOC_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    Me!IncomingDocumentID = parentFrm!ID

    If Nz(Me!ProductID, 0) <= 0 Then
        MsgBox ERR_PRODUCT_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    If Not RequirePositiveLong(Me!Quantity) Then
        Cancel = True
        Exit Sub
    End If
    Exit Sub
EH:
    Cancel = True
End Sub
