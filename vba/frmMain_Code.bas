'------------------------------------------------------------------------------
' Form: frmMain — code behind
' Target: Microsoft Access 2010
' Paste into the form's VBA module (Design View > View Code)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "منوی اصلی — سیستم انبارداری"
End Sub

Private Sub btnProducts_Click()
    DoCmd.OpenForm "frmProducts"
End Sub

Private Sub btnDocuments_Click()
    DoCmd.OpenForm "frmDocuments"
End Sub

Private Sub btnStockReport_Click()
    DoCmd.OpenReport "rptStock", acViewPreview
End Sub

Private Sub btnLowStockReport_Click()
    DoCmd.OpenReport "rptLowStock", acViewPreview
End Sub

Private Sub btnInOutReport_Click()
    DoCmd.OpenReport "rptInOut", acViewPreview
End Sub

Private Sub btnProductSearch_Click()
    DoCmd.OpenForm "frmProductSearch"
End Sub

Private Sub btnExit_Click()
    DoCmd.Quit acQuitSaveAll
End Sub
