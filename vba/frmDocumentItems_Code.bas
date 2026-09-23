'------------------------------------------------------------------------------
' frmDocumentItems — اقلام سند (Subform Datasheet) — Access 2010
'------------------------------------------------------------------------------
Option Compare Database
Option Explicit

Private m_OldProductID As Long
Private m_OldQuantity As Long
Private m_HadOld As Boolean
Private m_DelProductID As Long
Private m_DelQty As Long
Private m_DelTx As String

Private Sub Form_Load()
    Me!cboProductID.RowSourceType = "Table/Query"
    Me!cboProductID.RowSource = _
        "SELECT ID, ProductName & ' (' & Nz(ProductCode,'') & ')' AS ProductLabel, ProductName, ProductCode " & _
        "FROM Products WHERE IsActive=True ORDER BY ProductName;"
    Me!cboProductID.ColumnCount = 4
    Me!cboProductID.ColumnWidths = "0cm;5cm;0cm;0cm"
    Me!cboProductID.BoundColumn = 1
    Me!cboProductID.LimitToList = True
End Sub

Private Sub Form_Current()
    m_HadOld = False
    m_OldProductID = 0
    m_OldQuantity = 0
    If Not Me.NewRecord Then
        m_OldProductID = Nz(Me!ProductID, 0)
        m_OldQuantity = Nz(Me!Quantity, 0)
        m_HadOld = True
    End If
End Sub

Private Sub Form_BeforeUpdate(Cancel As Integer)
    Dim parentFrm As Form
    Dim tx As String
    Dim productID As Long
    Dim qty As Long
    Dim available As Long

    On Error GoTo EH

    Set parentFrm = Me.Parent
    If parentFrm.NewRecord Or Nz(parentFrm!ID, 0) = 0 Then
        MsgBox ERR_DOC_REQUIRED, vbExclamation, MSG_TITLE
        Cancel = True
        Exit Sub
    End If

    Me!DocumentID = parentFrm!ID

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
        Exit Sub
    End If

    If Not RequirePositiveLong(qty) Then
        Cancel = True
        Exit Sub
    End If

    If tx = TRANSACTION_OUT Then
        available = GetProductCurrentStock(productID)
        If m_HadOld And m_OldProductID = productID Then available = available + m_OldQuantity
        If qty > available Then
            MsgBox ERR_STOCK_NEGATIVE & vbCrLf & "موجودی فعلی: " & available, vbExclamation, MSG_TITLE
            Cancel = True
            Exit Sub
        End If
    End If

    If m_HadOld Then
        If m_OldProductID > 0 And m_OldQuantity > 0 Then
            If Not ApplyStockChange(m_OldProductID, m_OldQuantity, tx, -1) Then
                Cancel = True
                Exit Sub
            End If
        End If
    End If

    If Not ApplyStockChange(productID, qty, tx, 1) Then
        If m_HadOld Then
            If m_OldProductID > 0 And m_OldQuantity > 0 Then
                Call ApplyStockChange(m_OldProductID, m_OldQuantity, tx, 1)
            End If
        End If
        Cancel = True
        Exit Sub
    End If
    Exit Sub
EH:
    MsgBox "ثبت این ردیف انجام نشد. لطفاً کالا و تعداد را بررسی کنید.", vbExclamation, MSG_TITLE
    Cancel = True
End Sub

Private Sub Form_AfterUpdate()
    m_OldProductID = Nz(Me!ProductID, 0)
    m_OldQuantity = Nz(Me!Quantity, 0)
    m_HadOld = True
End Sub

Private Sub Form_BeforeDelConfirm(Cancel As Integer, Response As Integer)
    m_DelProductID = Nz(Me!ProductID, 0)
    m_DelQty = Nz(Me!Quantity, 0)
    m_DelTx = UCase$(GetDocumentTransactionType(Nz(Me!DocumentID, 0)))
    If Len(m_DelTx) = 0 Then
        On Error Resume Next
        m_DelTx = UCase$(Nz(Me.Parent!TransactionType, ""))
    End If
End Sub

Private Sub Form_AfterDelConfirm(Status As Integer)
    If Status = acDeleteOK Then
        If m_DelProductID > 0 And m_DelQty > 0 And Len(m_DelTx) > 0 Then
            Call ApplyStockChange(m_DelProductID, m_DelQty, m_DelTx, -1)
        End If
    End If
    m_DelProductID = 0
    m_DelQty = 0
    m_DelTx = ""
End Sub
