'------------------------------------------------------------------------------
' Form: frmMain — code behind
' Target: Microsoft Access 2010
' Visual: brand-first main menu (Persian UI, English object names)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private Sub Form_Load()
    Me.Caption = "سیستم انبارداری"
    ApplyMainTheme
End Sub

Private Sub ApplyMainTheme()
    On Error Resume Next

    UI_StyleFormBackground Me

    ' Brand title in header
    UI_StyleLabel Me!lblBrand, True
    Me!lblBrand.Caption = "سیستم انبارداری"
    Me!lblBrand.FontSize = 22

    Me!lblSubtitle.Caption = "ورود، خروج و کنترل موجودی کالا"
    Me!lblSubtitle.FontName = "Tahoma"
    Me!lblSubtitle.FontSize = 11
    Me!lblSubtitle.ForeColor = RGB(210, 222, 230)
    Me!lblSubtitle.TextAlign = 3

    ' Accent strip under header (label used as bar)
    Me!boxAccent.BackColor = UI_ColorAccent()
    Me!boxAccent.BackStyle = 1
    Me!boxAccent.SpecialEffect = 0
    Me!boxAccent.BorderStyle = 0

    UI_StylePrimaryButton Me!btnDocuments
    UI_StylePrimaryButton Me!btnProducts
    UI_StyleSecondaryButton Me!btnStockReport
    UI_StyleSecondaryButton Me!btnLowStockReport
    UI_StyleSecondaryButton Me!btnInOutReport
    UI_StyleSecondaryButton Me!btnProductSearch
    UI_StyleSecondaryButton Me!btnExit

    Me!btnDocuments.Caption = "ثبت سند انبار"
    Me!btnProducts.Caption = "مدیریت کالاها"
    Me!btnStockReport.Caption = "گزارش موجودی"
    Me!btnLowStockReport.Caption = "گزارش کم‌موجودی"
    Me!btnInOutReport.Caption = "گزارش ورود و خروج"
    Me!btnProductSearch.Caption = "جستجوی کالا"
    Me!btnExit.Caption = "خروج"

    On Error GoTo 0
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
    If MsgBox("از برنامه خارج می‌شوید؟", vbQuestion + vbYesNo, MSG_TITLE) = vbYes Then
        DoCmd.Quit acQuitSaveAll
    End If
End Sub
