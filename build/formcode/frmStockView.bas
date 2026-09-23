'------------------------------------------------------------------------------
' frmStockView — مشاهده موجودی انبار — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_LowOnly As Boolean

Private Sub Form_Load()
    Me.Caption = "مشاهده موجودی انبار"
    m_LowOnly = False
    Me!chkLowOnly = False
    ApplyFilter
End Sub

Private Sub btnSearch_Click()
    ApplyFilter
End Sub

Private Sub txtSearch_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyReturn Then
        KeyCode = 0
        ApplyFilter
    End If
End Sub

Private Sub btnLowOnly_Click()
    m_LowOnly = True
    Me!chkLowOnly = True
    ApplyFilter
End Sub

Private Sub chkLowOnly_AfterUpdate()
    m_LowOnly = Nz(Me!chkLowOnly, False)
    ApplyFilter
End Sub

Private Sub btnShowAll_Click()
    Me!txtSearch = Null
    Me!chkLowOnly = False
    m_LowOnly = False
    ApplyFilter
End Sub

Private Sub ApplyFilter()
    On Error GoTo EH
    Me!subStock.Form.RecordSource = "qryStockView"
    Me!subStock.Requery
    Exit Sub
EH:
    MsgBox "نمایش موجودی انجام نشد.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnMovement_Click()
    Dim pid As Long
    On Error GoTo EH
    pid = Nz(Me!subStock.Form!ID, 0)
    If pid <= 0 Then
        MsgBox "ابتدا یک کالا را انتخاب کنید.", vbExclamation, MSG_TITLE
        Exit Sub
    End If
    DoCmd.OpenForm "frmProductMovement", , , , , , CStr(pid)
    Exit Sub
EH:
    MsgBox "لطفاً یک کالا را از لیست انتخاب کنید.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub
