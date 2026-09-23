'------------------------------------------------------------------------------
' frmOutgoingItems — اقلام خروج + بخش هر قلم
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_OldProductID As Long
Private m_OldQuantity As Long
Private m_HadOld As Boolean

Private Sub Form_Load()
    Call SetupProductSearchCombo(Me!cboProductID)
    Call SetupDepartmentCombo(Me!cboDepartmentID)
End Sub

Private Sub cboProductID_Change()
    Call RefreshProductComboSearch(Me!cboProductID)
End Sub

Private Sub Form_Current()
    m_HadOld = False
    m_OldProductID = 0
    m_OldQuantity = 0
    On Error Resume Next
    If Nz(Me.Parent!IsPosted, False) Then
        Me.AllowEdits = False
    Else
        Me.AllowEdits = True
    End If
    If Not Me.NewRecord Then
        m_OldProductID = Nz(Me!ProductID, 0)
        m_OldQuantity = Nz(Me!Quantity, 0)
        m_HadOld = True
    End If
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    Dim parentFrm As Form
    Dim pid As Long
    Dim qty As Long
    Dim docID As Long

    On Error GoTo EH
    If Nz(Me.Parent!IsPosted, False) Then
        MsgBox ERR_POSTED_LOCKED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    Set parentFrm = Me.Parent
    If parentFrm.NewRecord Or Nz(parentFrm!ID, 0) = 0 Then
        MsgBox ERR_DOC_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    docID = parentFrm!ID
    Me!OutgoingDocumentID = docID

    pid = Nz(Me!ProductID, 0)
    qty = Nz(Me!Quantity, 0)

    If pid <= 0 Then
        MsgBox ERR_PRODUCT_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If
    If Nz(Me!DepartmentID, 0) <= 0 Then
        MsgBox ERR_DEPT_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If
    If Not RequirePositiveLong(qty) Then
        Cancel = True
        Exit Sub
    End If

    If OutgoingLineWouldExceedStock(docID, pid, qty, Nz(Me!ID, 0)) Then
        MsgBox ERR_STOCK_NEGATIVE & vbCrLf & "موجودی فعلی: " & GetProductCurrentStock(pid), vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If
    Exit Sub
EH:
    Cancel = True
End Sub

Private Sub Form_AfterUpdate()
    m_OldProductID = Nz(Me!ProductID, 0)
    m_OldQuantity = Nz(Me!Quantity, 0)
    m_HadOld = True
End Sub
