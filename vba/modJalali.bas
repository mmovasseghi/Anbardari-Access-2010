'------------------------------------------------------------------------------
' modJalali — تبدیل تاریخ شمسی/میلادی (Access 2010، بدون وابستگی خارجی)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Function FormatJalali(ByVal gDate As Variant) As String
    Dim jy As Long, jm As Long, jd As Long
    If IsNull(gDate) Or Not IsDate(gDate) Then
        FormatJalali = ""
        Exit Function
    End If
    GregorianToJalali CLng(Year(CDate(gDate))), CLng(Month(CDate(gDate))), CLng(Day(CDate(gDate))), jy, jm, jd
    FormatJalali = Format$(jy, "0000") & "/" & Format$(jm, "00") & "/" & Format$(jd, "00")
End Function

Public Function ParseJalaliToDate(ByVal jalaliText As Variant) As Variant
    Dim parts() As String
    Dim jy As Long, jm As Long, jd As Long
    Dim gy As Long, gm As Long, gd As Long
    Dim s As String

    If IsNull(jalaliText) Then
        ParseJalaliToDate = Null
        Exit Function
    End If

    s = Trim$(Replace$(Replace$(CStr(jalaliText), "-", "/"), "\", "/"))
    If Len(s) = 0 Then
        ParseJalaliToDate = Null
        Exit Function
    End If

    parts = Split(s, "/")
    If UBound(parts) <> 2 Then
        ParseJalaliToDate = Null
        Exit Function
    End If

    If Not IsNumeric(parts(0)) Or Not IsNumeric(parts(1)) Or Not IsNumeric(parts(2)) Then
        ParseJalaliToDate = Null
        Exit Function
    End If

    jy = CLng(parts(0))
    jm = CLng(parts(1))
    jd = CLng(parts(2))
    If jy < 1300 Or jy > 1500 Or jm < 1 Or jm > 12 Or jd < 1 Or jd > 31 Then
        ParseJalaliToDate = Null
        Exit Function
    End If

    JalaliToGregorian jy, jm, jd, gy, gm, gd
    ParseJalaliToDate = DateSerial(gy, gm, gd)
End Function

Public Function JalaliFieldIsValid(ByVal jalaliText As Variant) As Boolean
    JalaliFieldIsValid = Not IsNull(ParseJalaliToDate(jalaliText))
End Function

Private Sub GregorianToJalali(ByVal gy As Long, ByVal gm As Long, ByVal gd As Long, _
    ByRef jy As Long, ByRef jm As Long, ByRef jd As Long)

    Dim g_d_m() As Integer
    Dim gy2 As Long, days As Long
    g_d_m = Array(0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334)

    If gy > 1600 Then
        gy2 = gy - 1600
    Else
        gy2 = gy - 621
    End If

    days = 365 * gy2 + (gy2 + 3) \ 4 - (gy2 + 99) \ 100 + (gy2 + 399) \ 400 - 80 + gd
    If gm > 2 Then days = days + g_d_m(gm - 1) + 1 Else days = days + g_d_m(gm - 1)

    jy = -979 + 33 * (days \ 12053)
    days = days Mod 12053
    jy = jy + 4 * (days \ 1461)
    days = days Mod 1461
    If days > 365 Then
        jy = jy + (days - 1) \ 365
        days = (days - 1) Mod 365
    End If

    If days < 186 Then
        jm = 1 + days \ 31
        jd = 1 + (days Mod 31)
    Else
        jm = 7 + (days - 186) \ 30
        jd = 1 + ((days - 186) Mod 30)
    End If
End Sub

Private Sub JalaliToGregorian(ByVal jy As Long, ByVal jm As Long, ByVal jd As Long, _
    ByRef gy As Long, ByRef gm As Long, ByRef gd As Long)

    Dim days As Long
    jy = jy - 979
    jm = jm - 1
    jd = jd - 1

    days = 365 * jy + (jy \ 33) * 8 + ((jy Mod 33) + 3) \ 4
    If jm < 7 Then
        days = days + jm * 31
    Else
        days = days + (jm - 7) * 30 + 186
    End If
    days = days + jd + 79

    gy = 1600 + 400 * (days \ 146097)
    days = days Mod 146097
    Dim leap As Boolean
    leap = True
    If days >= 36525 Then
        days = days - 1
        gy = gy + 100 * (days \ 36524)
        days = days Mod 36524
        If days >= 365 Then days = days + 1 Else leap = False
    End If
    gy = gy + 4 * (days \ 1461)
    days = days Mod 1461
    If days >= 366 Then
        leap = False
        days = days - 1
        gy = gy + days \ 365
        days = days Mod 365
    End If

    Dim sal_a() As Integer
    sal_a = Array(0, 31, IIf(leap, 29, 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31)
    gm = 0
    Do While gm < 12 And days >= sal_a(gm + 1)
        days = days - sal_a(gm + 1)
        gm = gm + 1
    Loop
    gm = gm + 1
    gd = days + 1
End Sub
