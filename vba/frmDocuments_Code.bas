'------------------------------------------------------------------------------
' Form: frmDocuments — code behind
' Target: Microsoft Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "ثبت سند انبار"
    SetupTransactionCombo
    ApplyTheme
End Sub

Private Sub ApplyTheme()
    On Error Resume Next
    UI_StyleFormBackground Me
    UI_StyleLabel Me!lblTitle, True
    Me!lblTitle.Caption = "ثبت سند انبار"

    UI_StyleTextBox Me!txtDocumentNumber
    UI_StyleTextBox Me!txtDeliveryNumber
    UI_StyleTextBox Me!txtDocumentDate
    UI_StyleTextBox Me!txtSource
    UI_StyleTextBox Me!txtDestination
    UI_StyleTextBox Me!txtDescription
    UI_StyleTextBox Me!cboTransactionType

    UI_StylePrimaryButton Me!btnSave
    UI_StyleSecondaryButton Me!btnNew
    UI_StyleSecondaryButton Me!btnClose

    Me!lblSectionItems.Caption = "اقلام سند"
    Me!lblSectionItems.FontName = "Tahoma"
    Me!lblSectionItems.FontBold = True
    Me!lblSectionItems.ForeColor = UI_ColorHeader()
    On Error GoTo 0
End Sub

Private Sub SetupTransactionCombo()
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
    RefreshTransactionLock
    RefreshSubformLink
End Sub

Private Sub cboTransactionType_BeforeUpdate(Cancel As Integer)
    ' Prevent changing IN/OUT after items exist (would corrupt stock)
    If Not Me.NewRecord Then
        If DocumentHasItems(Nz(Me!ID, 0)) Then
            If Nz(Me!cboTransactionType.OldValue, "") <> Nz(Me!cboTransactionType.Value, "") Then
                MsgBox "پس از ثبت اقلام، تغییر نوع تراکنش مجاز نیست.", vbExclamation, MSG_TITLE
                Cancel = True
            End If
        End If
    End If
End Sub

Private Sub cboTransactionType_AfterUpdate()
    RefreshDeliveryRequirement
End Sub

Private Sub RefreshDeliveryRequirement()
    Dim isOut As Boolean
    isOut = (Nz(Me!TransactionType, "") = TRANSACTION_OUT)

    On Error Resume Next
    If isOut Then
        Me!lblDeliveryNumber.Caption = "شماره حواله *:"
        Me!lblDeliveryNumber.ForeColor = UI_ColorDanger()
    Else
        Me!lblDeliveryNumber.Caption = "شماره حواله:"
        Me!lblDeliveryNumber.ForeColor = UI_ColorText()
    End If
    On Error GoTo 0
End Sub

Private Sub RefreshTransactionLock()
    Dim locked As Boolean
    locked = False

    If Not Me.NewRecord Then
        locked = DocumentHasItems(Nz(Me!ID, 0))
    End If

    On Error Resume Next
    Me!cboTransactionType.Locked = locked
    Me!cboTransactionType.Enabled = True
    On Error GoTo 0
End Sub

' Called from frmDocumentItems after item save/delete
Public Sub RefreshLocks()
    RefreshTransactionLock
End Sub

Private Sub RefreshSubformLink()
    On Error Resume Next
    Me!subDocumentItems.Form.Requery
    On Error GoTo 0
End Sub

Private Sub btnSave_Click()
    On Error GoTo EH
    DoCmd.RunCommand acCmdSaveRecord
    RefreshTransactionLock
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
