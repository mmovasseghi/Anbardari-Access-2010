'------------------------------------------------------------------------------
' frmOutgoingConfirm — مرحله دوم خروج
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_DocID As Long

Private Sub Form_Load()
    m_DocID = CLng(Nz(Me.OpenArgs, 0))
    Me.Caption = "تأیید نهایی خروج / حواله"
    Me!lblMessage.Caption = MSG_CONFIRM_OUTGOING
    Me!txtDocID = m_DocID
    Me!subLines.SourceObject = "qryOutgoingConfirmLines"
    LoadHeader
End Sub

Private Sub LoadHeader()
    Dim rs As DAO.Recordset
    If m_DocID <= 0 Then Exit Sub
    Set rs = CurrentDb.OpenRecordset( _
        "SELECT DocumentNumber, DeliveryNumber, DocumentDate, Description FROM OutgoingDocuments WHERE ID=" & m_DocID, dbOpenSnapshot)
    If Not rs.EOF Then
        Me!txtSummary = _
            "شماره سند: " & Nz(rs!DocumentNumber, "") & vbCrLf & _
            "شماره حواله: " & Nz(rs!DeliveryNumber, "") & vbCrLf & _
            "تاریخ: " & FormatJalali(rs!DocumentDate) & vbCrLf & _
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
    If PostOutgoingDocument(m_DocID) Then
        MsgBox "خروج کالا ثبت نهایی شد و موجودی به‌روز شد.", vbInformation, MSG_TITLE
        DoCmd.Close acForm, Me.Name
        On Error Resume Next
        Forms!frmOutgoing.Requery
    End If
End Sub
