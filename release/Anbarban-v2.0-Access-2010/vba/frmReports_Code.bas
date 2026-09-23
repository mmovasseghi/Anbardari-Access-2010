'------------------------------------------------------------------------------
' frmReports — جستجو و گزارش‌های ترکیبی
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "جستجو و گزارش‌ها"
    SetupCombos
    ClearFilters
End Sub

Private Sub SetupCombos()
    On Error Resume Next
    Me!cboReportType.RowSourceType = "Value List"
    Me!cboReportType.RowSource = _
        "STOCK;موجودی فعلی;" & _
        "LOW;کالاهای کم‌موجودی;" & _
        "IN;گزارش ورود;" & _
        "OUT;گزارش خروج;" & _
        "BOTH;ورود و خروج;" & _
        "PRODUCT;گردش یک کالا;" & _
        "SUPPLIER;بر اساس فروشنده;" & _
        "DEPT;بر اساس بخش;" & _
        "DELIVERY;بر اساس حواله"
    Me!cboReportType.ColumnCount = 2
    Me!cboReportType.ColumnWidths = "0cm;5cm"
    Me!cboReportType.BoundColumn = 1
    Me!cboReportType.LimitToList = True

    Me!cboProduct.RowSource = "SELECT ID, ProductName FROM Products WHERE IsActive=True ORDER BY ProductName;"
    Me!cboProduct.ColumnCount = 2
    Me!cboProduct.ColumnWidths = "0cm;5cm"
    Me!cboProduct.BoundColumn = 1

    Me!cboSupplier.RowSource = "SELECT ID, SupplierName FROM Suppliers WHERE IsActive=True ORDER BY SupplierName;"
    Me!cboSupplier.ColumnCount = 2
    Me!cboSupplier.ColumnWidths = "0cm;5cm"
    Me!cboSupplier.BoundColumn = 1

    Me!cboDepartment.RowSource = "SELECT ID, DepartmentName FROM Departments WHERE IsActive=True ORDER BY DepartmentName;"
    Me!cboDepartment.ColumnCount = 2
    Me!cboDepartment.ColumnWidths = "0cm;5cm"
    Me!cboDepartment.BoundColumn = 1
End Sub

Private Sub ClearFilters()
    Me!cboReportType = "BOTH"
    Me!txtFromJ = Null
    Me!txtToJ = Null
    Me!cboProduct = Null
    Me!cboSupplier = Null
    Me!cboDepartment = Null
    Me!txtDocNo = Null
    Me!txtInvoiceNo = Null
    Me!txtDeliveryNo = Null
End Sub

Private Sub ApplyGregorianFilters()
    If IsBlank(Me!txtFromJ) Then
        Me!txtFromGreg = Null
    Else
        Me!txtFromGreg = ParseJalaliToDate(Me!txtFromJ)
    End If
    If IsBlank(Me!txtToJ) Then
        Me!txtToGreg = Null
    Else
        Me!txtToGreg = ParseJalaliToDate(Me!txtToJ)
    End If
End Sub

Private Sub btnShow_Click()
    Dim rpt As String
    If Not ValidateJalaliDateRange(Me!txtFromJ, Me!txtToJ) Then Exit Sub
    Call ApplyGregorianFilters
    rpt = Nz(Me!cboReportType, "BOTH")
    Select Case rpt
        Case "STOCK"
            DoCmd.OpenReport "rptStock", acViewPreview
        Case "LOW"
            DoCmd.OpenReport "rptLowStock", acViewPreview
        Case "IN"
            DoCmd.OpenReport "rptIncoming", acViewPreview
        Case "OUT"
            DoCmd.OpenReport "rptOutgoing", acViewPreview
        Case "BOTH"
            DoCmd.OpenReport "rptInOut", acViewPreview
        Case "PRODUCT"
            If Nz(Me!cboProduct, 0) <= 0 Then
                MsgBox "کالا را انتخاب کنید.", vbExclamation, MSG_TITLE
                Exit Sub
            End If
            Me!txtProductID = Me!cboProduct
            DoCmd.OpenReport "rptProductMovement", acViewPreview
        Case "SUPPLIER"
            DoCmd.OpenReport "rptBySupplier", acViewPreview
        Case "DEPT"
            DoCmd.OpenReport "rptByDepartment", acViewPreview
        Case "DELIVERY"
            DoCmd.OpenReport "rptByDelivery", acViewPreview
        Else
            DoCmd.OpenReport "rptInOut", acViewPreview
    End Select
End Sub

Private Sub btnExcel_Click()
    MsgBox "برای خروجی Excel: گزارش را باز کنید، سپس از منوی Access «خارج کردن» → Excel استفاده کنید.", vbInformation, MSG_TITLE
End Sub

Private Sub btnSearchProduct_Click()
    DoCmd.OpenForm "frmProductSearch"
End Sub

Private Sub btnClear_Click()
    ClearFilters
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
    DoCmd.OpenForm "frmMain"
End Sub
