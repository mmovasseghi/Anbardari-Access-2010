'------------------------------------------------------------------------------
' Module: modApp
' Public UI helpers for operator forms (Access 2010)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Function App_OpenIn() As Boolean
    DoCmd.OpenForm "frmDocuments", , , , , , TRANSACTION_IN
    App_OpenIn = True
End Function

Public Function App_OpenOut() As Boolean
    DoCmd.OpenForm "frmDocuments", , , , , , TRANSACTION_OUT
    App_OpenOut = True
End Function

Public Function App_OpenProducts() As Boolean
    DoCmd.OpenForm "frmProducts"
    App_OpenProducts = True
End Function

Public Function App_OpenSearch() As Boolean
    DoCmd.OpenForm "frmProductSearch"
    App_OpenSearch = True
End Function

Public Function App_OpenStock() As Boolean
    DoCmd.OpenForm "frmStockView"
    App_OpenStock = True
End Function

Public Function App_OpenInOutReport() As Boolean
    DoCmd.OpenForm "frmInOutReport"
    App_OpenInOutReport = True
End Function

Public Function App_OpenLowStock() As Boolean
    DoCmd.OpenReport "rptLowStock", acViewPreview
    App_OpenLowStock = True
End Function

Public Function App_ExitApp() As Boolean
    DoCmd.Quit acQuitSaveAll
    App_ExitApp = True
End Function

Public Function App_BackMain() As Boolean
    On Error Resume Next
    DoCmd.Close acForm, Screen.ActiveForm.Name
    DoCmd.OpenForm "frmMain"
    App_BackMain = True
End Function
