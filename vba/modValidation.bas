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

Public Function ValidateDocumentHeader(ByVal transactionType As Variant, ByVal deliveryNumber As Variant) As Boolean
    Dim tx As String

    If IsBlank(transactionType) Then
        MsgBox ERR_TX_REQUIRED, vbExclamation, MSG_TITLE
        ValidateDocumentHeader = False
        Exit Function
    End If

    tx = UCase$(CStr(transactionType))
    If tx <> TRANSACTION_IN And tx <> TRANSACTION_OUT Then
        MsgBox ERR_TX_REQUIRED, vbExclamation, MSG_TITLE
        ValidateDocumentHeader = False
        Exit Function
    End If

    If tx = TRANSACTION_OUT Then
        If IsBlank(deliveryNumber) Then
            MsgBox ERR_DELIVERY_REQUIRED, vbExclamation, MSG_TITLE
            ValidateDocumentHeader = False
            Exit Function
        End If
    End If

    ValidateDocumentHeader = True
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

Public Function StockStatusText(ByVal currentStock As Variant, ByVal minimumStock As Variant) As String
    If Nz(currentStock, 0) <= Nz(minimumStock, 0) Then
        StockStatusText = "نیاز به تأمین"
    Else
        StockStatusText = "موجود"
    End If
End Function
