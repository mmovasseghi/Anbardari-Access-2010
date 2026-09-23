'------------------------------------------------------------------------------
' Module: modValidation — Access 2010
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
    If IsBlank(transactionType) Then
        MsgBox ERR_TX_REQUIRED, vbExclamation, MSG_TITLE
        ValidateDocumentHeader = False
        Exit Function
    End If

    If UCase$(CStr(transactionType)) <> TRANSACTION_IN And UCase$(CStr(transactionType)) <> TRANSACTION_OUT Then
        MsgBox "نوع تراکنش نامعتبر است.", vbExclamation, MSG_TITLE
        ValidateDocumentHeader = False
        Exit Function
    End If

    If UCase$(CStr(transactionType)) = TRANSACTION_OUT Then
        If IsBlank(deliveryNumber) Then
            MsgBox ERR_DELIVERY_REQUIRED, vbExclamation, MSG_TITLE
            ValidateDocumentHeader = False
            Exit Function
        End If
    End If

    ValidateDocumentHeader = True
End Function
