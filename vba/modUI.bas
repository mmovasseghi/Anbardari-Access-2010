'------------------------------------------------------------------------------
' Module: modUI
' Target: Microsoft Access 2010
' Purpose: Shared visual styling helpers (DAO/VBA only, no modern features)
' Color direction: cool slate + teal accent, Persian-friendly Tahoma
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

' Access Long colors via RGB()
Public Function UI_ColorBg() As Long
    UI_ColorBg = RGB(236, 241, 245)          ' soft slate background
End Function

Public Function UI_ColorSurface() As Long
    UI_ColorSurface = RGB(255, 255, 255)     ' white panels
End Function

Public Function UI_ColorHeader() As Long
    UI_ColorHeader = RGB(28, 58, 77)         ' deep slate header
End Function

Public Function UI_ColorAccent() As Long
    UI_ColorAccent = RGB(20, 110, 110)       ' teal accent
End Function

Public Function UI_ColorAccentDark() As Long
    UI_ColorAccentDark = RGB(14, 82, 82)
End Function

Public Function UI_ColorText() As Long
    UI_ColorText = RGB(28, 32, 36)
End Function

Public Function UI_ColorMuted() As Long
    UI_ColorMuted = RGB(90, 102, 112)
End Function

Public Function UI_ColorDanger() As Long
    UI_ColorDanger = RGB(160, 48, 48)
End Function

Public Function UI_ColorLine() As Long
    UI_ColorLine = RGB(198, 210, 218)
End Function

Public Sub UI_StyleFormBackground(ByRef frm As Form)
    On Error Resume Next
    frm.Section(acDetail).BackColor = UI_ColorBg()
    frm.Section(acHeader).BackColor = UI_ColorHeader()
    On Error GoTo 0
End Sub

Public Sub UI_StylePrimaryButton(ByRef btn As Control)
    On Error Resume Next
    btn.FontName = "Tahoma"
    btn.FontSize = 11
    btn.FontBold = True
    btn.ForeColor = RGB(255, 255, 255)
    btn.BackColor = UI_ColorAccent()
    btn.BackStyle = 1 ' Normal
    btn.SpecialEffect = 0 ' Flat (Access 2010)
    btn.BorderColor = UI_ColorAccentDark()
    btn.BorderWidth = 1
    On Error GoTo 0
End Sub

Public Sub UI_StyleSecondaryButton(ByRef btn As Control)
    On Error Resume Next
    btn.FontName = "Tahoma"
    btn.FontSize = 10
    btn.FontBold = False
    btn.ForeColor = UI_ColorHeader()
    btn.BackColor = UI_ColorSurface()
    btn.BackStyle = 1
    btn.SpecialEffect = 0
    btn.BorderColor = UI_ColorLine()
    btn.BorderWidth = 1
    On Error GoTo 0
End Sub

Public Sub UI_StyleLabel(ByRef lbl As Control, Optional ByVal isHeader As Boolean = False)
    On Error Resume Next
    lbl.FontName = "Tahoma"
    If isHeader Then
        lbl.FontSize = 18
        lbl.FontBold = True
        lbl.ForeColor = RGB(255, 255, 255)
    Else
        lbl.FontSize = 10
        lbl.FontBold = False
        lbl.ForeColor = UI_ColorText()
    End If
    lbl.TextAlign = 3 ' Right (better for Persian)
    On Error GoTo 0
End Sub

Public Sub UI_StyleTextBox(ByRef txt As Control)
    On Error Resume Next
    txt.FontName = "Tahoma"
    txt.FontSize = 10
    txt.ForeColor = UI_ColorText()
    txt.BackColor = UI_ColorSurface()
    txt.BorderColor = UI_ColorLine()
    txt.SpecialEffect = 0
    txt.BorderWidth = 1
    txt.TextAlign = 3 ' Right
    On Error GoTo 0
End Sub

Public Function DocumentHasItems(ByVal documentID As Long) As Boolean
    Dim db As DAO.Database
    Dim rs As DAO.Recordset

    DocumentHasItems = False
    If documentID <= 0 Then Exit Function

    Set db = CurrentDb
    Set rs = db.OpenRecordset( _
        "SELECT COUNT(*) AS Cnt FROM DocumentItems WHERE DocumentID = " & documentID, _
        dbOpenSnapshot)

    If Not rs.EOF Then
        DocumentHasItems = (Nz(rs!Cnt, 0) > 0)
    End If

    rs.Close
    Set rs = Nothing
    Set db = Nothing
End Function
