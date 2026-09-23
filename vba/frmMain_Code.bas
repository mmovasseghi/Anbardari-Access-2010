'------------------------------------------------------------------------------
' frmMain — منوی اصلی اپراتور — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "سیستم انبارداری"
End Sub

Private Sub btnIn_Click()
    DoCmd.OpenForm "frmDocuments", , , , , , TRANSACTION_IN
End Sub

Private Sub btnOut_Click()
    DoCmd.OpenForm "frmDocuments", , , , , , TRANSACTION_OUT
End Sub

Private Sub btnProducts_Click()
    DoCmd.OpenForm "frmProducts"
End Sub

Private Sub btnSearch_Click()
    DoCmd.OpenForm "frmProductSearch"
End Sub

Private Sub btnStock_Click()
    DoCmd.OpenForm "frmStockView"
End Sub

Private Sub btnInOutReport_Click()
    DoCmd.OpenForm "frmInOutReport"
End Sub

Private Sub btnLowStock_Click()
    DoCmd.OpenReport "rptLowStock", acViewPreview
End Sub

Private Sub btnExit_Click()
    DoCmd.Quit acQuitSaveAll
End Sub
