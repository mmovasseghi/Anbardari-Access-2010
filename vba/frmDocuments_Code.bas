'------------------------------------------------------------------------------
' frmDocuments — ثبت ورود یا خروج (OpenArgs = IN / OUT)
' Access 2010 — رابط فارسی اپراتور
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_Mode As String

Private Sub Form_Load()
    m_Mode = UCase$(Nz(Me.OpenArgs, TRANSACTION_IN))
    If m_Mode <> TRANSACTION_IN And m_Mode <> TRANSACTION_OUT Then
        m_Mode = TRANSACTION_IN
    End If

    SetupModeUI
End Sub

Private Sub SetupModeUI()
    On Error Resume Next

    If m_Mode = TRANSACTION_OUT Then
        Me.Caption = "ثبت خروج / حواله"
        Me!lblTitle.Caption = "ثبت خروج / حواله"
        Me!lblSource.Caption = "مبدأ:"
        Me!lblDestination.Caption = "مقصد:"
        Me!lblDeliveryNumber.Visible = True
        Me!txtDeliveryNumber.Visible = True
        Me!lblDeliveryNumber.Caption = "شماره حواله:"
    Else
        Me.Caption = "ثبت ورود کالا"
        Me!lblTitle.Caption = "ثبت ورود کالا"
        Me!lblSource.Caption = "مبدأ:"
        Me!lblDestination.Caption = "مقصد:"
        Me!lblDeliveryNumber.Visible = False
        Me!txtDeliveryNumber.Visible = False
    End If

    ' نوع تراکنش برای کاربر دیده نشود؛ خودکار پر می‌شود
    Me!cboTransactionType.Visible = False
    Me!lblTransactionType.Visible = False
End Sub

Private Sub Form_Current()
    On Error Resume Next
    If Me.NewRecord Then
        Me!TransactionType = m_Mode
        If IsNull(Me!DocumentDate) Then Me!DocumentDate = Date
    Else
        ' اگر سند قبلی با نوع دیگر است، حالت UI را با رکورد هماهنگ کن
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
    MsgBox "سند ذخیره شد. حالا می‌توانید اقلام کالا را پایین صفحه وارد کنید.", vbInformation, MSG_TITLE
    Exit Sub
EH:
    MsgBox "ذخیره انجام نشد. لطفاً فیلدهای لازم را کامل کنید.", vbExclamation, MSG_TITLE
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
    ' نگه داشته شده برای سازگاری با زیر‌فرم اقلام
End Sub
