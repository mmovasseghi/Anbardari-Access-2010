'------------------------------------------------------------------------------
' Form: frmDocumentItems — code behind (Subform / Datasheet)
' Target: Microsoft Access 2010
' Record Source: DocumentItems
' Used as Source Object of subDocumentItems on frmDocuments
' Link Master Fields: ID
' Link Child Fields: DocumentID
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_OldProductID As Long
Private m_OldQuantity As Long
Private m_HadOldValues As Boolean

Private Sub Form_Load()
    SetupProductCombo
End Sub

Private Sub SetupProductCombo()
    ' Combo shows ProductName, stores ProductID
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

    ' Ensure parent document exists / is saved
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

    ' Sync DocumentID from parent link
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

    ' Reverse previous stock effect if editing existing row
    If m_HadOldValues Then
        If m_OldProductID > 0 And m_OldQuantity > 0 Then
            If Not ApplyStockChange(m_OldProductID, m_OldQuantity, tx, -1) Then
                Cancel = True
                Exit Sub
            End If
        End If
    End If

    ' Apply new stock effect
    If Not ApplyStockChange(productID, qty, tx, 1) Then
        ' Try to restore old effect if reverse already happened
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
End Sub

Private Sub Form_BeforeDelConfirm(Cancel As Integer, Response As Integer)
    Dim tx As String
    Dim productID As Long
    Dim qty As Long

    On Error GoTo EH

    tx = UCase$(GetDocumentTransactionType(Nz(Me!DocumentID, 0)))
    If Len(tx) = 0 Then
        tx = UCase$(Nz(Me.Parent!TransactionType, ""))
    End If

    productID = Nz(Me!ProductID, 0)
    qty = Nz(Me!Quantity, 0)

    If productID > 0 And qty > 0 And Len(tx) > 0 Then
        If Not ApplyStockChange(productID, qty, tx, -1) Then
            Cancel = True
            Response = acDataErrContinue
            Exit Sub
        End If
    End If

    Exit Sub
EH:
    MsgBox "خطا در حذف قلم سند: " & Err.Description, vbExclamation, MSG_TITLE
    Cancel = True
    Response = acDataErrContinue
End Sub
