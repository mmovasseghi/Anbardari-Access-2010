'------------------------------------------------------------------------------
' frmDocuments — ثبت ورود یا خروج (OpenArgs = IN / OUT)
' Access 2010 — رابط فارسی اپراتور
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_Mode As String

Private Sub Form_Load()
    m_Mode = UCase$(Nz(Me.OpenArgs, TRANSACTION_IN))
    If m_Mode <> TRANSACTION_IN And m_Mode <> TRANSACTION_OUT Then m_Mode = TRANSACTION_IN
    SetupModeUI
End Sub

Private Sub SetupModeUI()
    On Error Resume Next
    If m_Mode = TRANSACTION_OUT Then
        Me.Caption = "ثبت خروج / حواله"
        Me!lblTitle.Caption = "ثبت خروج / حواله"
        Me!lblDeliveryNumber.Visible = True
        Me!txtDeliveryNumber.Visible = True
    Else
        Me.Caption = "ثبت ورود کالا"
        Me!lblTitle.Caption = "ثبت ورود کالا"
        Me!lblDeliveryNumber.Visible = False
        Me!txtDeliveryNumber.Visible = False
        Me!DeliveryNumber = Null
    End If
    Me!txtTransactionType.Visible = False
End Sub

Private Sub Form_Current()
    On Error Resume Next
    If Me.NewRecord Then
        Me!TransactionType = m_Mode
        If IsNull(Me!DocumentDate) Then Me!DocumentDate = Date
    Else
        If Nz(Me!TransactionType, "") <> "" Then
            m_Mode = UCase$(Me!TransactionType)
            SetupModeUI
        End If
    End If
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    Me!TransactionType = m_Mode
    If Not ValidateDocumentHeader(Me!TransactionType, Me!DeliveryNumber) Then
        Cancel = True
        Exit Sub
    End If
    If IsNull(Me!DocumentDate) Then Me!DocumentDate = Date
End Sub

Private Sub btnSave_Click()
    On Error GoTo EH
    Me!TransactionType = m_Mode
    DoCmd.RunCommand acCmdSaveRecord
    MsgBox "سند ذخیره شد. حالا اقلام کالا را وارد کنید.", vbInformation, MSG_TITLE
    Exit Sub
EH:
    MsgBox "ذخیره انجام نشد. فیلدهای لازم را کامل کنید.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnNew_Click()
    DoCmd.GoToRecord , , acNewRec
    Me!TransactionType = m_Mode
    Me!DocumentDate = Date
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub

Public Sub RefreshLocks()
End Sub
