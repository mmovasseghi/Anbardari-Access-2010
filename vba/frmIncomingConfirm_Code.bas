'------------------------------------------------------------------------------
' frmIncomingConfirm — مرحله دوم ورود
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_DocID As Long

Private Sub Form_Load()
    m_DocID = CLng(Nz(Me.OpenArgs, 0))
    Me.Caption = "تأیید نهایی ورود کالا"
    Me!lblMessage.Caption = MSG_CONFIRM_INCOMING
    Me!txtDocID = m_DocID
    Me!subLines.SourceObject = "qryIncomingConfirmLines"
    LoadHeader
End Sub

Private Sub LoadHeader()
    Dim rs As DAO.Recordset
    If m_DocID <= 0 Then Exit Sub
    Set rs = CurrentDb.OpenRecordset( _
        "SELECT d.DocumentNumber, d.InvoiceNumber, d.DocumentDate, s.SupplierName, d.Description " & _
        "FROM IncomingDocuments d INNER JOIN Suppliers s ON d.SupplierID=s.ID WHERE d.ID=" & m_DocID, dbOpenSnapshot)
    If Not rs.EOF Then
        Me!txtSummary = _
            "شماره سند: " & Nz(rs!DocumentNumber, "") & vbCrLf & _
            "شماره فاکتور: " & Nz(rs!InvoiceNumber, "") & vbCrLf & _
            "تاریخ: " & FormatJalali(rs!DocumentDate) & vbCrLf & _
            "فروشنده: " & Nz(rs!SupplierName, "") & vbCrLf & _
            "توضیحات: " & Nz(rs!Description, "")
    End If
    rs.Close
    Set rs = Nothing
End Sub

Private Sub btnBack_Click()
    DoCmd.Close acForm, Me.Name
End Sub

Private Sub btnConfirm_Click()
    If m_DocID <= 0 Then Exit Sub
    If PostIncomingDocument(m_DocID) Then
        MsgBox "ورود کالا با موفقیت ثبت نهایی شد و موجودی به‌روز شد.", vbInformation, MSG_TITLE
        DoCmd.Close acForm, Me.Name
        On Error Resume Next
        Forms!frmIncoming.Requery
    End If
End Sub
