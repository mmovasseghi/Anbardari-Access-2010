'------------------------------------------------------------------------------
' Form: frmDocumentItems — code behind (Subform / Datasheet)
' Target: Microsoft Access 2010
' Stock reversal on delete is done in AfterDelConfirm (Access 2010-safe)
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_OldProductID As Long
Private m_OldQuantity As Long
Private m_HadOldValues As Boolean
Private m_PendingDeleteProductID As Long
Private m_PendingDeleteQty As Long
Private m_PendingDeleteTx As String

Private Sub Form_Load()
    SetupProductCombo
End Sub

Private Sub SetupProductCombo()
    Me!cboProductID.RowSourceType = "Table/Query"
    Me!cboProductID.RowSource = _
        "SELECT ID, ProductName, ProductCode FROM Products WHERE IsActive = True ORDER BY ProductName;"
    Me!cboProductID.ColumnCount = 3
    Me!cboProductID.ColumnWidths = "0cm;4cm;2cm"
    Me!cboProductID.BoundColumn = 1
    Me!cboProductID.LimitToList = True
End Sub

Private Sub Form_Current()
    m_HadOldValues = False
    m_OldProductID = 0
    m_OldQuantity = 0

    If Not Me.NewRecord Then
        m_OldProductID = Nz(Me!ProductID, 0)
        m_OldQuantity = Nz(Me!Quantity, 0)
        m_HadOldValues = True
    End If
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    Dim docID As Long
    Dim tx As String
    Dim productID As Long
    Dim qty As Long
    Dim parentFrm As Form

    On Error GoTo EH

    Set parentFrm = Me.Parent
    If parentFrm.NewRecord Then
        MsgBox ERR_DOC_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    docID = Nz(parentFrm!ID, 0)
    If docID <= 0 Then
        MsgBox ERR_DOC_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    Me!DocumentID = docID

    If Not ValidateDocumentHeader(parentFrm!TransactionType, parentFrm!DeliveryNumber) Then
        Cancel = True
        Exit Sub
    End If

    tx = UCase$(Nz(parentFrm!TransactionType, ""))
    productID = Nz(Me!ProductID, 0)
    qty = Nz(Me!Quantity, 0)

    If productID <= 0 Then
        MsgBox ERR_PRODUCT_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Me!cboProductID.SetFocus
        Exit Sub
    End If

    If Not RequirePositiveLong(qty, "تعداد") Then
        Cancel = True
        Me!Quantity.SetFocus
        Exit Sub
    End If

    ' Early friendly check for OUT before mutating
    If tx = TRANSACTION_OUT Then
        Dim available As Long
        available = GetProductCurrentStock(productID)
        If m_HadOldValues Then
            If m_OldProductID = productID Then
                available = available + m_OldQuantity
            End If
        End If
        If qty > available Then
            MsgBox ERR_STOCK_NEGATIVE & vbCrLf & "موجودی قابل خروج: " & available, vbExclamation, MSG_TITLE
            Cancel = True
            Exit Sub
        End If
    End If

    If m_HadOldValues Then
        If m_OldProductID > 0 And m_OldQuantity > 0 Then
            If Not ApplyStockChange(m_OldProductID, m_OldQuantity, tx, -1) Then
                Cancel = True
                Exit Sub
            End If
        End If
    End If

    If Not ApplyStockChange(productID, qty, tx, 1) Then
        If m_HadOldValues Then
            If m_OldProductID > 0 And m_OldQuantity > 0 Then
                Call ApplyStockChange(m_OldProductID, m_OldQuantity, tx, 1)
            End If
        End If
        Cancel = True
        Exit Sub
    End If

    Exit Sub
EH:
    MsgBox "خطا در ذخیره قلم سند: " & Err.Description, vbExclamation, MSG_TITLE
    Cancel = True
End Sub

Private Sub Form_AfterUpdate()
    m_OldProductID = Nz(Me!ProductID, 0)
    m_OldQuantity = Nz(Me!Quantity, 0)
    m_HadOldValues = True

    ' Refresh parent lock state (transaction type)
    On Error Resume Next
    Call Me.Parent.RefreshLocks
    On Error GoTo 0
End Sub

Private Sub Form_BeforeDelConfirm(Cancel As Integer, Response As Integer)
    ' Only capture values here. Do NOT change stock yet.
    ' If user cancels the delete dialog, stock must remain unchanged.
    Dim tx As String

    m_PendingDeleteProductID = 0
    m_PendingDeleteQty = 0
    m_PendingDeleteTx = ""

    tx = UCase$(GetDocumentTransactionType(Nz(Me!DocumentID, 0)))
    If Len(tx) = 0 Then
        On Error Resume Next
        tx = UCase$(Nz(Me.Parent!TransactionType, ""))
        On Error GoTo 0
    End If

    m_PendingDeleteProductID = Nz(Me!ProductID, 0)
    m_PendingDeleteQty = Nz(Me!Quantity, 0)
    m_PendingDeleteTx = tx
End Sub

Private Sub Form_AfterDelConfirm(Status As Integer)
    If Status <> acDeleteOK Then
        m_PendingDeleteProductID = 0
        m_PendingDeleteQty = 0
        m_PendingDeleteTx = ""
        Exit Sub
    End If

    If m_PendingDeleteProductID > 0 And m_PendingDeleteQty > 0 And Len(m_PendingDeleteTx) > 0 Then
        If Not ApplyStockChange(m_PendingDeleteProductID, m_PendingDeleteQty, m_PendingDeleteTx, -1) Then
            MsgBox "قلم حذف شد ولی اصلاح موجودی کامل نشد. موجودی را بررسی کنید.", vbCritical, MSG_TITLE
        End If
    End If

    On Error Resume Next
    Call Me.Parent.RefreshLocks
    On Error GoTo 0

    m_PendingDeleteProductID = 0
    m_PendingDeleteQty = 0
    m_PendingDeleteTx = ""
End Sub
