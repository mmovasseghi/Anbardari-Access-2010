'------------------------------------------------------------------------------
' Form: frmDocuments — code behind
' Target: Microsoft Access 2010
' Record Source: Documents
' Contains subform control: subDocumentItems (Source Object = frmDocumentItems)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "ثبت سند انبار"
    SetupTransactionCombo
End Sub

Private Sub SetupTransactionCombo()
    ' Value List: stored;displayed
    ' Row Source Type = Value List
    ' Column Count = 2
    ' Column Widths = 0cm;3cm
    ' Bound Column = 1
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

    If IsNull(Me!DocumentDate) Then
        Me!DocumentDate = Date
    End If
End Sub

Private Sub Form_Current()
    RefreshDeliveryRequirement
    RefreshSubformLink
End Sub

Private Sub cboTransactionType_AfterUpdate()
    RefreshDeliveryRequirement
End Sub

Private Sub RefreshDeliveryRequirement()
    Dim isOut As Boolean
    isOut = (Nz(Me!TransactionType, "") = TRANSACTION_OUT)

    ' Visual hint only — validation is enforced in BeforeUpdate
    If isOut Then
        Me!lblDeliveryNumber.Caption = "شماره حواله *:"
    Else
        Me!lblDeliveryNumber.Caption = "شماره حواله:"
    End If
End Sub

Private Sub RefreshSubformLink()
    On Error Resume Next
    Me!subDocumentItems.Form.Requery
    On Error GoTo 0
End Sub

Private Sub btnSave_Click()
    On Error GoTo EH
    DoCmd.RunCommand acCmdSaveRecord
    MsgBox "سند ذخیره شد.", vbInformation, MSG_TITLE
    Exit Sub
EH:
    MsgBox "ذخیره انجام نشد: " & Err.Description, vbExclamation, MSG_TITLE
End Sub

Private Sub btnNew_Click()
    DoCmd.GoToRecord , , acNewRec
End Sub

Private Sub btnClose_Click()
    DoCmd.Close acForm, Me.Name
End Sub
