'------------------------------------------------------------------------------
' modValidation — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Public Function IsBlank(ByVal v As Variant) As Boolean
    If IsNull(v) Then
        IsBlank = True
    ElseIf Len(Trim$(CStr(v))) = 0 Then
        IsBlank = True
    Else
        IsBlank = False
    End If
End Function

Public Function RequirePositiveLong(ByVal v As Variant) As Boolean
    If IsBlank(v) Or (Not IsNumeric(v)) Or (CLng(v) <= 0) Then
        MsgBox ERR_QTY_POSITIVE, vbExclamation, MSG_TITLE
        RequirePositiveLong = False
    Else
        RequirePositiveLong = True
    End If
End Function

Public Function ValidateIncomingHeader(ByVal documentNumber As Variant, ByVal supplierID As Variant, ByVal jalaliDate As Variant) As Boolean
    If IsBlank(documentNumber) Then
        MsgBox ERR_DOCNO_REQUIRED, vbExclamation, MSG_TITLE
        ValidateIncomingHeader = False
        Exit Function
    End If
    If Nz(supplierID, 0) <= 0 Then
        MsgBox ERR_SUPPLIER_REQUIRED, vbExclamation, MSG_TITLE
        ValidateIncomingHeader = False
        Exit Function
    End If
    If Not IsBlank(jalaliDate) Then
        If Not JalaliFieldIsValid(jalaliDate) Then
            MsgBox ERR_JALALI_INVALID, vbExclamation, MSG_TITLE
            ValidateIncomingHeader = False
            Exit Function
        End If
    End If
    ValidateIncomingHeader = True
End Function

Public Function ValidateOutgoingHeader(ByVal documentNumber As Variant, ByVal deliveryNumber As Variant, ByVal jalaliDate As Variant) As Boolean
    If IsBlank(documentNumber) Then
        MsgBox ERR_DOCNO_REQUIRED, vbExclamation, MSG_TITLE
        ValidateOutgoingHeader = False
        Exit Function
    End If
    If IsBlank(deliveryNumber) Then
        MsgBox ERR_DELIVERY_REQUIRED, vbExclamation, MSG_TITLE
        ValidateOutgoingHeader = False
        Exit Function
    End If
    If Not IsBlank(jalaliDate) Then
        If Not JalaliFieldIsValid(jalaliDate) Then
            MsgBox ERR_JALALI_INVALID, vbExclamation, MSG_TITLE
            ValidateOutgoingHeader = False
            Exit Function
        End If
    End If
    ValidateOutgoingHeader = True
End Function

Public Function ValidateDateRange(ByVal fromDate As Variant, ByVal toDate As Variant) As Boolean
    If IsBlank(fromDate) Or IsBlank(toDate) Then
        ValidateDateRange = True
        Exit Function
    End If
    If CDate(fromDate) > CDate(toDate) Then
        MsgBox ERR_DATE_RANGE, vbExclamation, MSG_TITLE
        ValidateDateRange = False
    Else
        ValidateDateRange = True
    End If
End Function

Public Function ValidateJalaliDateRange(ByVal fromJ As Variant, ByVal toJ As Variant) As Boolean
    Dim f As Variant, t As Variant
    If IsBlank(fromJ) Or IsBlank(toJ) Then
        ValidateJalaliDateRange = True
        Exit Function
    End If
    f = ParseJalaliToDate(fromJ)
    t = ParseJalaliToDate(toJ)
    If IsNull(f) Or IsNull(t) Then
        MsgBox ERR_JALALI_INVALID, vbExclamation, MSG_TITLE
        ValidateJalaliDateRange = False
        Exit Function
    End If
    If CDate(f) > CDate(t) Then
        MsgBox ERR_DATE_RANGE, vbExclamation, MSG_TITLE
        ValidateJalaliDateRange = False
    Else
        ValidateJalaliDateRange = True
    End If
End Function

Public Function StockStatusText(ByVal currentStock As Variant, ByVal minimumStock As Variant) As String
    If Nz(currentStock, 0) <= Nz(minimumStock, 0) Then
        StockStatusText = "نیاز به تأمین"
    Else
        StockStatusText = "موجود"
    End If
End Function

Public Sub SyncJalaliToBoundDate(ByVal jalaliControl As Variant, ByRef boundDateField As Variant)
    Dim d As Variant
    If IsBlank(jalaliControl) Then
        boundDateField = Null
        Exit Sub
    End If
    d = ParseJalaliToDate(jalaliControl)
    If IsNull(d) Then
        boundDateField = Null
    Else
        boundDateField = CDate(d)
    End If
End Sub

Public Sub LoadJalaliFromDate(ByVal boundDate As Variant, ByRef jalaliControl As Variant)
    If IsNull(boundDate) Or Not IsDate(boundDate) Then
        jalaliControl = Null
    Else
        jalaliControl = FormatJalali(boundDate)
    End If
End Sub

Public Sub LockFormIfPosted(ByVal frm As Form, ByVal isPosted As Boolean)
    Dim ctl As Control
    On Error Resume Next
    frm.AllowEdits = Not isPosted
    For Each ctl In frm.Controls
        If ctl.ControlType = acTextBox Or ctl.ControlType = acComboBox Then
            If ctl.Name <> "txtJalaliDate" Then
                ctl.Locked = isPosted
            End If
        End If
    Next ctl
End Sub
