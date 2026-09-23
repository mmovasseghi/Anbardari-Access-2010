'------------------------------------------------------------------------------
' Form: frmProductSearch — code behind
' Target: Microsoft Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "جستجوی کالا"
    ApplyTheme
End Sub

Private Sub ApplyTheme()
    On Error Resume Next
    UI_StyleFormBackground Me
    UI_StyleLabel Me!lblTitle, True
    Me!lblTitle.Caption = "جستجوی کالا"

    UI_StyleTextBox Me!txtName
    UI_StyleTextBox Me!txtCode
    UI_StylePrimaryButton Me!btnSearch
    UI_StyleSecondaryButton Me!btnClear
    UI_StyleSecondaryButton Me!btnClose
    On Error GoTo 0
End Sub

Private Sub btnSearch_Click()
    On Error GoTo EH
    Me!subResults.Form.RecordSource = "qryProductSearch"
    Me!subResults.Requery
    Exit Sub
EH:
    MsgBox "خطا در جستجو: " & Err.Description, vbExclamation, MSG_TITLE
End Sub

Private Sub btnClear_Click()
    Me!txtName = Null
    Me!txtCode = Null
    On Error Resume Next
    Me!subResults.Requery
    On Error GoTo 0
End Sub

Private Sub btnClose_Click()
    DoCmd.Close acForm, Me.Name
End Sub

Private Sub txtName_AfterUpdate()
    ' Enter-friendly search
End Sub

Private Sub txtName_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyReturn Then
        KeyCode = 0
        Call btnSearch_Click
    End If
End Sub

Private Sub txtCode_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyReturn Then
        KeyCode = 0
        Call btnSearch_Click
    End If
End Sub
