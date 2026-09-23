'------------------------------------------------------------------------------
' Form: frmProducts — code behind
' Target: Microsoft Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "مدیریت کالاها"
    ApplyTheme
End Sub

Private Sub ApplyTheme()
    On Error Resume Next
    UI_StyleFormBackground Me
    UI_StyleLabel Me!lblTitle, True
    Me!lblTitle.Caption = "مدیریت کالاها"

    UI_StyleTextBox Me!txtProductName
    UI_StyleTextBox Me!txtProductCode
    UI_StyleTextBox Me!txtUnit
    UI_StyleTextBox Me!txtCurrentStock
    UI_StyleTextBox Me!txtMinimumStock

    UI_StylePrimaryButton Me!btnNew
    UI_StyleSecondaryButton Me!btnClose
    On Error GoTo 0
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    If IsBlank(Me!ProductName) Then
        MsgBox "نام کالا الزامی است.", vbExclamation, MSG_TITLE
        Cancel = True
        Me!ProductName.SetFocus
        Exit Sub
    End If

    If Not IsNull(Me!CurrentStock) Then
        If Me!CurrentStock < 0 Then
            MsgBox ERR_STOCK_NEGATIVE, vbExclamation, MSG_TITLE
            Cancel = True
            Me!CurrentStock.SetFocus
            Exit Sub
        End If
    End If

    If Not IsNull(Me!MinimumStock) Then
        If Me!MinimumStock < 0 Then
            MsgBox "حداقل موجودی نمی‌تواند منفی باشد.", vbExclamation, MSG_TITLE
            Cancel = True
            Me!MinimumStock.SetFocus
            Exit Sub
        End If
    End If
End Sub

Private Sub Form_Current()
    Dim low As Boolean
    On Error Resume Next
    low = (Nz(Me!CurrentStock, 0) <= Nz(Me!MinimumStock, 0)) And (Not Me.NewRecord)
    If low Then
        Me!lblStockHint.Caption = "وضعیت: کم‌موجودی"
        Me!lblStockHint.ForeColor = UI_ColorDanger()
    Else
        Me!lblStockHint.Caption = "وضعیت: عادی"
        Me!lblStockHint.ForeColor = UI_ColorAccent()
    End If
    Me!lblStockHint.FontName = "Tahoma"
    On Error GoTo 0
End Sub

Private Sub btnClose_Click()
    DoCmd.Close acForm, Me.Name
End Sub

Private Sub btnNew_Click()
    DoCmd.GoToRecord , , acNewRec
End Sub
