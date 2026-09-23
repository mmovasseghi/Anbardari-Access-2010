'------------------------------------------------------------------------------
' modApp — میانبرهای عمومی
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Function App_BackMain() As Boolean
    On Error Resume Next
    DoCmd.Close acForm, Screen.ActiveForm.Name
    DoCmd.OpenForm "frmMain"
    App_BackMain = True
End Function
