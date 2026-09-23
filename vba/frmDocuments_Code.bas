'------------------------------------------------------------------------------
' frmDocuments — Access 2010 — minimal + safe locks
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "ثبت سند انبار"
    Me!cboTransactionType.RowSourceType = "Value List"
    Me!cboTransactionType.RowSource = "IN;ورود;OUT;خروج"
    Me!cboTransactionType.ColumnCount = 2
    Me!cboTransactionType.ColumnWidths = "0cm;3cm"
    Me!cboTransactionType.BoundColumn = 1
    Me!cboTransactionType.LimitToList = True
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    If Not ValidateDocumentHeader(Me!TransactionType, Me!DeliveryNumber) Then
        Cancel = True
        Exit Sub
    End If
    If IsNull(Me!DocumentDate) Then Me!DocumentDate = Date
End Sub

Private Sub Form_Current()
    Call RefreshDeliveryLabel
    Call RefreshTxLock
End Sub

Private Sub cboTransactionType_BeforeUpdate(Cancel As Integer)
    If Me.NewRecord Then Exit Sub
    If DocumentHasItems(Nz(Me!ID, 0)) Then
        If Nz(Me!cboTransactionType.OldValue, "") <> Nz(Me!cboTransactionType.Value, "") Then
            MsgBox "پس از ثبت اقلام، نوع تراکنش قابل تغییر نیست.", vbExclamation, MSG_TITLE
            Cancel = True
        End If
    End If
End Sub

Private Sub cboTransactionType_AfterUpdate()
    Call RefreshDeliveryLabel
End Sub

Private Sub RefreshDeliveryLabel()
    On Error Resume Next
    If Nz(Me!TransactionType, "") = TRANSACTION_OUT Then
        Me!lblDeliveryNumber.Caption = "شماره حواله *:"
    Else
        Me!lblDeliveryNumber.Caption = "شماره حواله:"
    End If
End Sub

Private Sub RefreshTxLock()
    On Error Resume Next
    If Me.NewRecord Then
        Me!cboTransactionType.Locked = False
    Else
        Me!cboTransactionType.Locked = DocumentHasItems(Nz(Me!ID, 0))
    End If
End Sub

Public Sub RefreshLocks()
    Call RefreshTxLock
End Sub

Private Sub btnSave_Click()
    On Error GoTo EH
    DoCmd.RunCommand acCmdSaveRecord
    Call RefreshTxLock
    Exit Sub
EH:
    MsgBox "ذخیره نشد: " & Err.Description, vbExclamation, MSG_TITLE
End Sub

Private Sub btnNew_Click()
    DoCmd.GoToRecord , , acNewRec
End Sub

Private Sub btnClose_Click()
    DoCmd.Close acForm, Me.Name
End Sub
