'------------------------------------------------------------------------------
' frmInOutReport — فیلتر ساده گزارش ورود و خروج — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "گزارش ورود و خروج"
    SetupCombos
    ClearFilters
End Sub

Private Sub SetupCombos()
    On Error Resume Next
    Me!cboType.RowSourceType = "Value List"
    Me!cboType.RowSource = "IN;ورود کالا;OUT;خروج / حواله"
    Me!cboType.ColumnCount = 2
    Me!cboType.ColumnWidths = "0cm;4cm"
    Me!cboType.BoundColumn = 1
    Me!cboType.LimitToList = False

    Me!cboProduct.RowSourceType = "Table/Query"
    Me!cboProduct.RowSource = "SELECT ID, ProductName FROM Products WHERE IsActive=True ORDER BY ProductName;"
    Me!cboProduct.ColumnCount = 2
    Me!cboProduct.ColumnWidths = "0cm;5cm"
    Me!cboProduct.BoundColumn = 1
    Me!cboProduct.LimitToList = False
End Sub

Private Sub ClearFilters()
    Me!cboType = Null
    Me!txtFromDate = Null
    Me!txtToDate = Null
    Me!cboProduct = Null
    Me!txtDocNo = Null
    Me!txtDeliveryNo = Null
End Sub

Private Sub btnShow_Click()
    If Not ValidateDateRange(Me!txtFromDate, Me!txtToDate) Then Exit Sub
    On Error GoTo EH
    DoCmd.OpenReport "rptInOut", acViewPreview
    Exit Sub
EH:
    MsgBox "نمایش گزارش انجام نشد. فیلترها را بررسی کنید.", vbExclamation, MSG_TITLE
End Sub

Private Sub btnClear_Click()
    ClearFilters
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub
