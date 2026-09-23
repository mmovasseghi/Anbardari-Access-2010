# frmDocuments (سبک)

Record Source: `Documents`

کنترل‌های لازم:

- `cboTransactionType` → Value List `IN;ورود;OUT;خروج`
- `txtDocumentNumber`, `txtDeliveryNumber`, `lblDeliveryNumber`
- `txtDocumentDate`, `txtSource`, `txtDestination`, `txtDescription`
- `subDocumentItems` → Source=`frmDocumentItems` / Link `ID`=`DocumentID`
- `btnSave`, `btnNew`, `btnClose`

کد: `vba/frmDocuments_Code.bas`
