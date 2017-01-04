Imports System.Net.Mail
Imports System.Net.Mail.SmtpClient
Imports System.Web.HttpContext

Module ModMainOrtho
    '***********************************Last Edit**************************************************

    'Last Edit Date: 03/5/15
    '  Last Edit By: CS
    'Last Edit Proj: WSDC_ORTHO
    '-----------------------------------------------------
    'Change Log: 
    '3/16/15 - CS
    '   Insert into payments was failing b/c missing field 'payerName' in insert statement
    '   When posting payments, need to loop open invoices until payment applied completely 
    '7/14/14 - t3
    '   added siteDisplayName app settings variable
    ' 
    '7/11/14 - t3
    '   clean to all common code/ remove patient/mod auto coder specifics.
    '    
    '**********************************************************************************************

    Public Function g_PostPendingData(ByVal ActiveTableName As String, _
                             ByVal PostingToTableName As String, _
                             ByVal WherePhrase_DoNotIncludeTheWordWhere As String, _
                             ByVal FormName As String, _
                             ByVal blnDeleteRecord As Boolean) As Integer

        ' function returns # of records archived

        Dim strSQL As String = "Select * from " & ActiveTableName & " where " & WherePhrase_DoNotIncludeTheWordWhere
        Dim tblActiveData As DataTable = g_IO_Execute_SQL(strSQL, False)

        ' get field information for archive table so as to know auto increment filed
        Dim strAutoIncFields As String = ""
        Dim strDelim As String = ""
        Dim tblPostingToTable As DataTable = g_getTableColumnInfo(PostingToTableName)
        For Each rowColumn In tblPostingToTable.Rows
            If rowColumn("AutoInc") = 1 Then
                strAutoIncFields &= strDelim & UCase(rowColumn("FieldName"))
                strDelim = ","
            End If
        Next
        Dim arrAutoIncFields() As String = Split(strAutoIncFields, ",")


        'move this over to the history table
        Dim nvcInsert As New NameValueCollection
        For Each rowItem As DataRow In tblActiveData.Rows
            For Each colFields As DataColumn In tblActiveData.Columns
                Dim blnIncludeColumn As Boolean = True
                If IsDBNull(rowItem(colFields.ColumnName)) Then
                    blnIncludeColumn = False
                    'skip adding this column since it's NULL
                Else
                    Dim strColumnName As String = UCase(colFields.ColumnName)
                    For Each strArrColumnName As String In arrAutoIncFields
                        If strArrColumnName = strColumnName Then
                            blnIncludeColumn = False
                            Exit For
                        End If
                    Next
                End If
                If blnIncludeColumn Then
                    nvcInsert(colFields.ColumnName) = rowItem(colFields.ColumnName)
                End If
            Next
            g_IO_SQLInsert(PostingToTableName, nvcInsert, FormName)
        Next

        If blnDeleteRecord Then
            ' delete the records from the active
            g_IO_SQLDelete(ActiveTableName, WherePhrase_DoNotIncludeTheWordWhere)
        End If

        Return tblActiveData.Rows.Count
    End Function

    Public Sub g_moveInsuranceToHistory(strChart, strCoverage)
        Dim ArchiveTableName As String = "Patient_Insurance_History"
        Dim strSQL As String = "Select * from patient_insurance where chart_number = '" & strChart & "' and coverage_type = '" & strCoverage & "'"

        Dim tblActiveData As DataTable = g_IO_Execute_SQL(strSQL, False)

        ' get field information for archive table so as to know auto increment filed
        Dim strAutoIncFields As String = ""
        Dim strDelim As String = ""
        Dim tblArchiveTable As DataTable = g_getTableColumnInfo(ArchiveTableName)
        For Each rowColumn In tblArchiveTable.Rows
            If rowColumn("AutoInc") = 1 Then
                strAutoIncFields &= strDelim & UCase(rowColumn("FieldName"))
                strDelim = ","
            End If
        Next
        Dim arrAutoIncFields() As String = Split(strAutoIncFields, ",")

        'move this over to the history table
        Dim nvcInsert As New NameValueCollection
        For Each rowItem As DataRow In tblActiveData.Rows
            For Each colFields As DataColumn In tblActiveData.Columns
                Dim blnIncludeColumn As Boolean = True
                If IsDBNull(rowItem(colFields.ColumnName)) Then
                    blnIncludeColumn = False
                    'skip adding this column since it's NULL
                Else
                    Dim strColumnName As String = UCase(colFields.ColumnName)
                    For Each strArrColumnName As String In arrAutoIncFields
                        If strArrColumnName = strColumnName Then
                            blnIncludeColumn = False
                            Exit For
                        End If
                    Next
                End If
                If blnIncludeColumn Then
                    nvcInsert(colFields.ColumnName) = rowItem(colFields.ColumnName)
                End If
            Next
            g_IO_SQLInsert(ArchiveTableName, nvcInsert, "Contracts_PI")
        Next

        ' delete the records from the active
        g_IO_SQLDelete("Patient_Insurance", "chart_number = '" & strChart & "' and coverage_type = '" & strCoverage & "'")

        ' update contract & patient record
        If strCoverage = "1" Then
            strSQL = "update patients set plan_pri = 0 where chart_number = '" & strChart & "'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "update contracts set PrimaryInsurancePlans_vw = 0, PrimaryCoverageAmt = 0, PrimaryInitialPayment = 0, PrimaryInstallmentAmt = 0, PrimaryRemainingBalance = 0, PrimaryAmountBilled = 0, PrimaryClaim_Amount_Initial = 0, PrimaryClaim_Amount_Installment = 0, PrimaryInNetworkAdjustment = 0 where chartNumber = '" & strChart & "'"
            g_IO_Execute_SQL(strSQL, False)
        Else
            strSQL = "update patients set plan_sec = 0 where chart_number = '" & strChart & "'"
            strSQL = "update contracts set SecodnaryInsurancePlans_vw = 0, SecondaryCoverageAmt = 0, SecondaryInitialPayment = 0, SecondaryInstallmentAmt = 0, SecondaryRemainingBalance = 0, SecondaryAmountBilled = 0, SecondaryClaim_Amount_Initial = 0, SecondaryClaim_Amount_Installment = 0, SecondaryInNetworkAdjustment = 0 where chartNumber = '" & strChart & "'"
            g_IO_Execute_SQL(strSQL, False)
        End If

        
    End Sub
    Public Function BuildJavascriptClaimsArray(ByVal strContractsRecid As String, ByVal strChartNumber As String)
        Dim strSql As String = ""
        If strContractsRecid = -1 Then
            'insurance_name is now '' since we are displaying insurance name in ddl
            strSql = "select type, insurance_name  from openClaimsDDL_vw  where ChartNumber = '" & strChartNumber & "'  order by claimNumber asc  "
        Else
            'insurance_name is now '' since we are displaying insurance name in ddl
            strSql = "select type, insurance_name from openClaimsDDL_vw  where contracts_recid = '" & strContractsRecid & "'  order by claimNumber asc  "
        End If
        Dim strClaimsArray As String = "[['',''],['','']"
        ' build a two deminsional javascript array to be used by the insurance dropdown to extract claim id and insurance type (prim or Sec)
        For Each rowInsurance In g_IO_Execute_SQL(strSql, False).Rows
            strClaimsArray &= ",[""" & rowInsurance("type") & """,""" & rowInsurance("insurance_name") & """]"
        Next
        Return strClaimsArray & "]"
    End Function

    Public Sub AttachInvoicePayments(ByVal ContractNumber As String, ByVal InvoiceNumber As String, ByVal InvoiceAmount As Decimal, ByVal InvoiceType As String, ByVal PaymentRecid As Integer)
        Dim tblPayments As DataTable = g_IO_Execute_SQL("Select * from payments where recid = " &
                                                        PaymentRecid & " order by recid", False)
        Dim tblInvoices As DataTable = g_IO_Execute_SQL("Select * from invoices where InvoiceNo = '" &
                                                        InvoiceNumber & "' order by recid", False)
        Dim intLastPaymentRecid As Integer = -1
        ' is there an invoice available to post a payment to?
        If tblInvoices.Rows.Count > 0 Then
            'Should only be one payment 
            For Each rowPayment As DataRow In tblPayments.Rows
                Dim decOrigPaymentAmt As Decimal = rowPayment("orig_payment")

                Dim strInvoiceStatus As String = "O"
                Dim decInvoiceAmtPaid As Decimal = tblInvoices.Rows(0)("AmountPaid")
                Dim decInvoiceAmtNeeded As Decimal = InvoiceAmount - decInvoiceAmtPaid
                Dim decInvoiceCurrentDue As Decimal = tblInvoices.Rows(0)("Current_Due")
                Dim decInvoiceTotalDue As Decimal = tblInvoices.Rows(0)("Total_Due")

                Dim decOverpaymentAmount As Decimal = 0
                Dim decPaymentAmount As Decimal = 0

                decOverpaymentAmount = 0
                decPaymentAmount = rowPayment("patientamount")

                If decPaymentAmount >= decInvoiceAmtNeeded Then
                    ' payment is satisfied
                    decOverpaymentAmount = decPaymentAmount - decInvoiceAmtNeeded  ' this is the amount that will be posted to a sister payment record about to be created and tied to this payment record
                    decPaymentAmount = decInvoiceAmtNeeded  ' reduce the payment amount on this payment record to the amount that will be posted to this invoice and tie this payment to this invoice
                    strInvoiceStatus = "C"
                    decInvoiceAmtPaid = InvoiceAmount
                Else
                    ' Invoice is partially paid by this payment
                    strInvoiceStatus = "O"
                    'update amount paid
                    decInvoiceAmtPaid += decPaymentAmount
                End If
                ' recalculate invoice stored current and total due in case they need to reprint the invoice
                decInvoiceCurrentDue -= decPaymentAmount
                decInvoiceTotalDue -= decPaymentAmount

                g_IO_Execute_SQL("Update payments set Invoices_recid = '" & tblInvoices.Rows(0)("recid") & "', PatientAmount= " & decPaymentAmount & ",  " & IIf(InvoiceType = "current", "ApplyToCurrentInvoice", "ApplyToPastDue") & " = " & decPaymentAmount & " where recid = " & rowPayment("recid"), False)

                g_IO_Execute_SQL("update invoices set status = '" & strInvoiceStatus & "', AmountPaid = " & decInvoiceAmtPaid & _
                                 ", Current_Due = " & decInvoiceCurrentDue & ", Total_Due = " & decInvoiceTotalDue & _
                                 " where InvoiceNo = '" & InvoiceNumber & "'", False)

                'Do we have money left over to be applied to another invoice
                ' 3/6/15 CS Need to loop invoices until payment applied completely 
                '           and/or left with remaining open payment to apply later...
                If decOverpaymentAmount > 0 Then
                    '10/7/16 CS Added new field doctors_vw and put it in this section even tho commented out, just in cas it gets implemented again in the future??
                    ' 11/2/15 CS Do not check for other invoices here, because another payment may have already been entered and pending post to possible same invoices (ie past due selected)
                    'Check to see if we have any open invoices open that overpayment can be applied to 
                    'Dim tblOpenInvoices As DataTable = g_IO_Execute_SQL("Select * from invoices where recid >= (select min(recid) from Invoices " & _
                    '"where status = 'O' and Contracts_recid = '" & ContractNumber & "') and Contracts_recid = '" & ContractNumber & "' " & _
                    '" and PostDate  < DATEADD(day, -30, GETDATE()) ", False)
                    'If tblOpenInvoices.Rows.Count > 0 Then
                    '    ' overpayment made, create a new payment record with the overage to be posted to the next available invoice 
                    '    Dim strSQL = "insert into payments (DatePosted, sys_users_recid, patientNumber, ChartNumber, " & _
                    '                "PatientAmount,PaymentType,PaymentReference,Comments,Contract_recid,Invoices_recid, " & _
                    '                "baserecid,PayerName,PaymentSelection, " & _
                    '                IIf(InvoiceType = "current", "ApplyToCurrentInvoice", "ApplyToPastDue") & _
                    '                " , orig_payment, PaymentFor, Doctors_vw)" & _
                    '                " values (" & _
                    '                "'" & rowPayment("DatePosted") & "'," & _
                    '                rowPayment("sys_users_recid") & "," & _
                    '                "'" & rowPayment("patientNumber") & "'," & _
                    '                "'" & rowPayment("ChartNumber") & "'," & _
                    '                 decOverpaymentAmount & "," & _
                    '                "'" & rowPayment("PaymentType") & "'," & _
                    '                "'" & rowPayment("PaymentReference").replace("'", "''") & "'," & _
                    '                "'" & rowPayment("Comments").replace("'", "''") & "'," & _
                    '                "'" & rowPayment("contract_recid") & "'," & _
                    '                "-1," & _
                    '               "'" & rowPayment("BaseRecid") & "'," & _
                    '               "'" & rowPayment("payername").replace("'", "''") & "'," & _
                    '               "'" & rowPayment("PaymentSelection") & "'," & _
                    '                 decOverpaymentAmount & "," & _
                    '                 decOrigPaymentAmt & "," & _
                    '               "'" & rowPayment("PaymentFor").replace("'", "''") & "'," & _
                    '               rowPayment("doctors_vw") & _
                    '                 ")"
                    '    ' 3/6/15 CS Add this back in at end of statement, once field added to live db...
                    '    ' " , orig_payment, DateApplied)" & _
                    '    ' ...
                    '    '   decOverpaymentAmount & "," & _
                    '    '   "'" & Format(CType(Date.Now, Date), "yyyy-MM-dd") & "')" & _

                    '    g_IO_Execute_SQL(strSQL, False)
                    '    intLastPaymentRecid = g_IO_GetLastRecId()

                    '    'attach invoice to payment
                    '    AttachInvoicePayments(ContractNumber, tblOpenInvoices.Rows(0)("InvoiceNo"), tblOpenInvoices.Rows(0)("AmountDue"), InvoiceType, intLastPaymentRecid)
                    'Else
                    'No open invoices-create a new payment record with the overage to be posted to the next available invoice 
                    ' 3/6/15 CS Value for payerName was missing from insert statments VALUES...
                    ' 10/7/16 CS new field doctors_vw
                    Dim strSQL = "insert into payments (DatePosted, sys_users_recid, patientNumber, ChartNumber, PatientAmount,PaymentType,PaymentReference," &
                                "Comments,Contract_recid,Invoices_recid,baserecid,payername, PaymentSelection,  ApplyToNextInvoice, orig_payment,paymentfor, Doctors_vw)" &
                                " values (" &
                                "'" & rowPayment("DatePosted") & "'," &
                                rowPayment("sys_users_recid") & "," &
                                "'" & rowPayment("patientNumber") & "'," &
                                "'" & rowPayment("ChartNumber") & "'," &
                                 decOverpaymentAmount & "," &
                                "'" & rowPayment("PaymentType") & "'," &
                                "'" & rowPayment("PaymentReference").replace("'", "''") & "'," &
                                "'" & rowPayment("Comments").replace("'", "''") & "'," &
                                "'" & rowPayment("contract_recid") & "'," &
                                "-1," &
                                "'" & rowPayment("BaseRecid") & "'," &
                                "'" & rowPayment("payername").replace("'", "''") & "'," &
                                "'" & rowPayment("PaymentSelection") & "'," &
                                decOverpaymentAmount & "," &
                                decOrigPaymentAmt & "," &
                                "'" & rowPayment("PaymentFor").replace("'", "''") & "'," &
                                 rowPayment("Doctors_vw") &
                                ")"
                    g_IO_Execute_SQL(strSQL, False)
                    'End If

                End If
            Next
        End If
    End Sub
    Public Sub AttachUnprocessedClaimPayments(ByVal ContractID As String, ByVal ClaimType As Integer)
        '11/29/16 CS need to use contract recid not chart number, multiple contracts per chart number now
        AttachClaimPayments(ContractID, -1, ClaimType, -1)
    End Sub
    Public Sub AttachClaimPayments(ByVal ContractID As String, ByVal ClaimNumber As String, ByVal ClaimType As Integer, ByVal PaymentRecid As Integer)

        '11/29/16 CS need to use contract recid not chart number, multiple contracts per chart number now
        Dim tblClaims As DataTable = g_IO_Execute_SQL("Select * from claims where status= 'O'" &
            " and [type] = " & ClaimType &
            " and procedure_amount > 0 and AmountPaid < procedure_amount " &
            " and Contracts_Recid = '" & ContractID & "'" &
            IIf(ClaimNumber = -1, " ", IIf(ClaimNumber = -99, " ", " and claimNumber = " & ClaimNumber)) &
            " order by recid", False)

        ' is there a payment available to post to claim?
        For Each rowClaims As DataRow In tblClaims.Rows

            Dim tblPayments As DataTable = g_IO_Execute_SQL("Select * from payments where contract_recid = '" & rowClaims("contracts_recid") & "'" &
                IIf(PaymentRecid = -1, " and claimnumber = '-1' and ApplyToClaim <> 0 and PaymentSelection = " & IIf(ClaimType = 0, "'PrimaryAmount'", "'SecondaryAmount'"), " and recid = " & PaymentRecid) &
                "  order by case when " & IIf(ClaimType = 0, "PrimaryAmount", "SecondaryAmount") & " >= 0 then 1 else 0 end, DatePosted desc", False)

            Dim strClaimStatus As String = "O"
            Dim decClaimAmtPaid As Decimal = rowClaims("AmountPaid")
            Dim decClaimAmtNeeded As Decimal = rowClaims("procedure_amount") - decClaimAmtPaid
            ' loop open payment records for primary (type = 0) or secondary (type=1) claims and process them until the claim is paid or until the payments available are exhausted

            Dim decOverpaymentAmount As Decimal = 0
            Dim decPaymentAmount As Decimal = 0

            For Each rowPayment As DataRow In tblPayments.Rows

                decOverpaymentAmount = 0
                decPaymentAmount = rowPayment("ApplyToClaim")

                If decPaymentAmount >= decClaimAmtNeeded Then
                    ' payment is satisfied
                    decOverpaymentAmount = decPaymentAmount - decClaimAmtNeeded  ' this is the amount that will be posted to a sister payment record about to be created and tied to this payment record
                    ' 12/2/15 allow full amount to show as amount 'paid' on claim, even when payment is more than needed
                    'decPaymentAmount = decClaimAmtNeeded  ' reduce the payment amount on this payment record to the amount that will be posted to this claim and tie this payment to this claim
                    ' 12/15/16 CS Not sure why we removed this code last year, and set amount paid to the full claim amount??
                    ' but we do need to set the payment amount to what is actually being applied to the claim with this payment, not the full claim amount
                    'decClaimAmtPaid = rowClaims("procedure_amount")
                    decPaymentAmount = decClaimAmtNeeded
                    '12/15/16 CS update total amount paid on claim (not sure why this wasn't being set at this point in the logic??)
                    decClaimAmtPaid += decPaymentAmount
                    strClaimStatus = "C"
                Else
                    ' Claim is partially paid by this payment
                    strClaimStatus = "O"
                    'update amount paid
                    decClaimAmtPaid += decPaymentAmount
                End If


                g_IO_Execute_SQL("Update payments set claimnumber = '" & rowClaims("ClaimNumber") & "',ApplyToClaim= " & decPaymentAmount &
                                 "," & rowPayment("PaymentSelection") & "= " & decPaymentAmount &
                                 " where recid = " & rowPayment("recid"), False)

                ' 11/16/15 CS Added code to close claim with an adjustment if under paid by $5.00
                Dim decPaymentDifference = decClaimAmtNeeded - decClaimAmtPaid
                If decClaimAmtNeeded > decClaimAmtPaid And decPaymentDifference <= 5.0 Then
                    strClaimStatus = "C"
                    ' close claim
                    Dim strSQL As String = "update claims set" &
                        " ClosedDate='" & Date.Now() & "'" &
                        ", ClosedReason='Insurance paid less than expected (auto-closed).'" &
                        " where claimNumber='" & rowClaims("ClaimNumber") & "'"
                    g_IO_Execute_SQL(strSQL, False)

                    ' create a payment record for an adjustment to close out the claim
                    ' 10/7/16 CS New Field Doctors_vw
                    ' 11/29/16 CS All dollar amounts on this adj record need to be set to decPaymentDifference, 
                    Dim intLastPaymentRecid As Integer = -1
                    strSQL = "insert into payments (DatePosted, sys_users_recid, patientNumber, ChartNumber, PrimaryAmount, SecondaryAmount, ApplyToClaim, " &
                            "PaymentType, PaymentReference, Contract_recid, Invoices_recid, claimNumber, baserecid, PaymentSelection, orig_Payment, " &
                            "Comments, PayerName,PaymentFor, Doctors_vw)" &
                            " values (" &
                            "'" & Format(Date.Now, "MM/dd/yyyy") & "'," &
                            rowPayment("sys_users_recid") & "," &
                            "'" & rowPayment("patientNumber") & "'," &
                            "'" & rowPayment("ChartNumber") & "'," &
                            IIf(rowPayment("PaymentSelection") = "PrimaryAmount", decPaymentDifference, 0.0) & "," &
                            IIf(rowPayment("PaymentSelection") = "SecondaryAmount", decPaymentDifference, 0.0) & "," &
                            decPaymentDifference & "," &
                            "'14'," &
                            "'ADJ-Closed Claim'," &
                            "'" & rowPayment("contract_recid") & "'," &
                            "'-1'," &
                            "'" & rowClaims("ClaimNumber") & "'," &
                            intLastPaymentRecid & "," &
                            "'" & rowPayment("PaymentSelection") & "'," &
                            decPaymentDifference & "," &
                            "'Insurance paid less than expected (auto-closed).'," &
                            "'" & rowPayment("PayerName").replace("'", "''") & "'," &
                            "'1'," &
                            rowPayment("Doctors_vw") &
                            ")"

                    g_IO_Execute_SQL(strSQL, False)

                    ' update BaseRECID on payment record just created
                    intLastPaymentRecid = g_IO_GetLastRecId()
                    strSQL = "update Payments set BaseRecid = " & intLastPaymentRecid & " where recid=" & intLastPaymentRecid

                    g_IO_Execute_SQL(strSQL, False)
                Else
                    ' 12/15/16 CS Need to reduce claim amount needed so that when possibly loop to another payment found, it will handle the amount that is NOW remaining after this payment above just posted to the claim...
                    decClaimAmtNeeded -= decClaimAmtPaid
                    If decClaimAmtNeeded < 0 Then
                        decClaimAmtNeeded = 0
                    End If
                End If



                If decOverpaymentAmount > 0 Then
                    ' 12/2/15 CS If overpaid, apply overpayment to remaining balance left to bill out
                    Dim InsType As String = IIf(ClaimType = 0, "PrimaryRemainingBalance", "SecondaryRemainingBalance")
                    '11/29/16 CS need to use contract recid not chart number, multiple contracts per chart number now
                    'g_IO_Execute_SQL("update contracts set " & InsType & " = " & InsType & " - " & decOverpaymentAmount & " where chartNumber = '" & rowClaims("ChartNumber") & "'", False)
                    g_IO_Execute_SQL("update contracts set " & InsType & " = " & InsType & " - " & decOverpaymentAmount & " where recid = '" & rowClaims("contracts_recid") & "'", False)

                    ' 10/7/16 CS Added new field doctors_vw and put it in this section even tho commented out, just in cas it gets implemented again in the future??
                    ' 11/1116 CS overpayment made, create a new payment record with the overage to be posted to principle 
                    Dim strSQL = "insert into payments (DatePosted, sys_users_recid, patientNumber, ChartNumber, PrimaryAmount, SecondaryAmount, " &
                                "PaymentType, PaymentReference, Contract_recid, Invoices_recid, claimNumber, baserecid, PaymentSelection, orig_Payment, " &
                                "Comments, PayerName,PaymentFor, Doctors_vw)" &
                                " values (" &
                                "'" & rowPayment("DatePosted") & "'," &
                                rowPayment("sys_users_recid") & "," &
                                "'" & rowPayment("patientNumber") & "'," &
                                "'" & rowPayment("ChartNumber") & "'," &
                                IIf(rowPayment("PaymentSelection") = "PrimaryAmount", decOverpaymentAmount, 0.0) & "," &
                                IIf(rowPayment("PaymentSelection") = "SecondaryAmount", decOverpaymentAmount, 0.0) & "," &
                                "'" & rowPayment("PaymentType") & "'," &
                                "'" & rowPayment("PaymentReference").replace("'", "''") & "'," &
                                "'" & rowPayment("contract_recid") & "'," &
                                "''," &
                                "'-99'," &
                                "'" & rowPayment("BaseRecid") & "'," &
                                "'" & rowPayment("PaymentSelection") & "'," &
                                rowPayment("orig_payment") & "," &
                                "'" & rowPayment("comments").replace("'", "''") & "'," &
                                "'" & rowPayment("PayerName").replace("'", "''") & "'," &
                                "'" & rowPayment("PaymentFor").replace("'", "''") & "'," &
                                rowPayment("doctors_vw") &
                                ")"
                    g_IO_Execute_SQL(strSQL, False)
                End If

                g_IO_Execute_SQL("update claims set status = '" & strClaimStatus & "', AmountPaid = " & decClaimAmtPaid & " where claimnumber = '" & rowClaims("ClaimNumber") & "'", False)

            Next


        Next

    End Sub

    Public Function ReprintClaimByRecid(ByRef CommaSepClaimsRecids As String, ByRef CommaSepClaimRecids As String)
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "__TYPE__" & System.Web.HttpContext.Current.Session("user_link_userid") & ".PDF"
        Dim strPOFilePath As String = HttpContext.Current.Server.MapPath("downloads\")
        Dim strMinDocumentNumber As String = ""
        Dim strMaxDocumentNumber As String = ""

        rptReport = New rptClaim

        ' code to reprint a claim
        Dim tbltry As DataTable = g_IO_Execute_SQL("Select * from claims where recid in (" & CommaSepClaimsRecids & ")", False)

        rptReport.SetDataSource(tbltry)

        If tbltry.Rows.Count > 0 Then
            strPOFileBase = strPOFileBase.Replace("__TYPE__", "_claims_" & strMinDocumentNumber & "-" & strMaxDocumentNumber & "_")



            rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFilePath & strPOFileBase)



            Return strPOFileBase
            'End If

            Exit Function
        End If
        Return Nothing
    End Function

    Public Function ReprintClaimByClaimNumber(ByRef CommaSepClaimsRecids As String, ByRef CommaSepClaimNumbers As String) As String
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "__TYPE__" & System.Web.HttpContext.Current.Session("user_link_id") & ".PDF"
        Dim strPOFilePath As String = HttpContext.Current.Server.MapPath("downloads\")
        Dim strMinDocumentNumber As String = ""
        Dim strMaxDocumentNumber As String = ""

        rptReport = New rptClaim

        '   code to reprint a claim
        Dim tbltry As DataTable = g_IO_Execute_SQL("Select * from claims where claimnumber in ('" & CommaSepClaimNumbers.Replace(",", "','") & "') order by ClaimNumber", False)

        Try
            strMinDocumentNumber = tbltry.Rows(0)("ClaimNumber")
        Catch ex As Exception

        End Try
        Try
            strMaxDocumentNumber = tbltry.Rows(tbltry.Rows.Count - 1)("ClaimNumber")
        Catch ex As Exception

        End Try


        rptReport.SetDataSource(tbltry)
        Dim strReturnName As String = ""

        If tbltry.Rows.Count > 0 Then
            strPOFileBase = strPOFileBase.Replace("__TYPE__", "_ClaimsReprint_" & strMinDocumentNumber & "-" & strMaxDocumentNumber & "_")
            strReturnName = strPOFileBase

            rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFilePath & strPOFileBase)

            Return strReturnName
            'End If

            Exit Function
        End If
        Return strReturnName
    End Function

    Public Function g_ReprintInvoiceByInvoiceNumber(ByRef CommaSepInvoicesRecids As String, ByRef CommaSepInvoiceNumbers As String) As String
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "__TYPE__" & System.Web.HttpContext.Current.Session("user_link_id") & ".PDF"
        Dim strPOFilePath As String = HttpContext.Current.Server.MapPath("downloads\")
        Dim strMinDocumentNumber As String = ""
        Dim strMaxDocumentNumber As String = ""

        rptReport = New rptInvoice

        '   code to reprint a Invoice
        Dim tbltry As DataTable = g_IO_Execute_SQL("Select * from Invoices where InvoiceNo in ('" & CommaSepInvoiceNumbers.Replace(",", "','").Replace(" ", "") & "') order by InvoiceNo", False)

        Try
            strMinDocumentNumber = tbltry.Rows(0)("InvoiceNo")
        Catch ex As Exception

        End Try
        Try
            strMaxDocumentNumber = tbltry.Rows(tbltry.Rows.Count - 1)("InvoiceNo")
        Catch ex As Exception

        End Try

        ' pull in payment history
        For i = tbltry.Rows.Count - 1 To 0 Step -1

            Dim rowInvoices As DataRow = tbltry.Rows(i)

            Dim tblPayment As DataTable = g_IO_Execute_SQL("select * from Invoices_HistoryDetail where invoices_recid = " & rowInvoices("recid"), False)

            Select Case rowInvoices("Total_due")
                Case 0
                    ' zero due are sorted to the bottom and will not be printed
                    rowInvoices("recid") = 1
                Case Else
                    rowInvoices("recid") = 0
            End Select


            For Each rowPayment In tblPayment.Rows
                Dim rowPaymentDetail As DataRow = tbltry.NewRow

                For Each colColumn As DataColumn In tbltry.Columns
                    rowPaymentDetail(colColumn.ColumnName) = rowInvoices(colColumn.ColumnName)
                Next

                For Each colColumn As DataColumn In tblPayment.Columns
                    If UCase(colColumn.ColumnName) = "INVOICES_RECID" Or UCase(colColumn.ColumnName) = "RECID" Then
                    Else
                        rowPaymentDetail(colColumn.ColumnName) = rowPayment(colColumn.ColumnName)
                    End If
                Next

                rowPaymentDetail("current_due") = Math.Round(rowPayment("Amountdue") - rowPayment("AmountPaid"), 2)

                tbltry.Rows.Add(rowPaymentDetail)
            Next


        Next

        rptReport.SetDataSource(tbltry)
        Dim strReturnName As String = ""

        If tbltry.Rows.Count > 0 Then
            strPOFileBase = strPOFileBase.Replace("__TYPE__", "_InvoicesReprint_" & strMinDocumentNumber & "-" & strMaxDocumentNumber & "_")
            strReturnName = strPOFileBase

            rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFilePath & strPOFileBase)

            Return strReturnName
            'End If

            Exit Function
        End If
        Return strReturnName
    End Function

    Public Sub g_ReverseInvoicePayment(ByVal Payments_Recid As String, ByVal PaymentType As String, ByVal ReversalComment As String, ByRef intNewPaymentsBaseRecid As Integer)


        Dim strSQL = "select recid, invoices_recid, DatePosted,applytocurrentinvoice,applyToPastDue, ApplyToPrinciple, applytonextinvoice, sys_users_recid, patientNumber, ChartNumber, PatientAmount, " &
         "PaymentType, PaymentReference, Contract_recid, baserecid, payername, paymentselection, orig_payment, Comments, PaymentFor, Doctors_vw " &
         "from payments where BaseRecid = " & Payments_Recid & " order by recid"

        Dim tblInvoicePayment As DataTable = g_IO_Execute_SQL(strSQL, False)

        ' handle any pre-payments made to this contract (not yet assigned to an invoice)
        ' read ALL unattached payments associated with this contract and try to attach them to this invoice
        If tblInvoicePayment.Rows.Count > 0 Then
            Dim rowPayment As DataRow = Nothing
            For Each rowPayment In tblInvoicePayment.Rows
                Dim decOrigPaymentAmt As Decimal = rowPayment("orig_payment")

                If rowPayment("invoices_recid") = -1 Then
                    ' if this payment has not been applied to an invoice yet, flag it so that it never does
                    rowPayment("invoices_recid") = -99
                    g_IO_Execute_SQL("update payments set invoices_recid = -99 where recid = " & rowPayment("recid"), False)
                ElseIf rowPayment("invoices_recid") = -99 Then
                    ' this payment was applied to principle, so adjust contract remaining balance back to what it was before payment was applied
                    strSQL = "update contracts set PatientRemainingBalance = PatientRemainingBalance + " & rowPayment("PatientAmount") & " where recid = '" & rowPayment("contract_recid") & "'"
                    g_IO_Execute_SQL(strSQL, False)
                Else
                    'reopen the invoice and adjust payment made
                    g_IO_Execute_SQL("update invoices set status = 'O', current_due = current_due + " & rowPayment("applytocurrentinvoice") & ", Total_Due = Total_Due + " & rowPayment("applytocurrentinvoice") & " + " & rowPayment("applyToPastDue") & ", AmountPaid = AmountPaid - " & rowPayment("applytocurrentinvoice") & " - " & rowPayment("applyToPastDue") & " where recid = " & rowPayment("invoices_recid"), False)
                End If
                ' set Apply To quantity's to reversed amounts
                rowPayment("PatientAmount") = 0 - rowPayment("PatientAmount")
                rowPayment("applytocurrentinvoice") = 0 - rowPayment("applytocurrentinvoice")
                rowPayment("applyToPastDue") = 0 - rowPayment("applyToPastDue")
                rowPayment("ApplyToPrinciple") = 0 - rowPayment("ApplyToPrinciple")
                rowPayment("applytonextinvoice") = 0 - rowPayment("applytonextinvoice")

                ' create reversal payment record
                '10/7/16 CS Added new field doctors_vw 
                strSQL = "insert into payments (DatePosted, contract_recid, sys_users_recid, patientNumber, ChartNumber, " &
                            "PatientAmount, applytocurrentinvoice,applyToPastDue,applytonextinvoice,PaymentType,PaymentReference, " &
                            "Invoices_recid,baserecid, payername, paymentselection, orig_payment, Comments, PaymentFor, Doctors_vw)" &
                            " values (" &
                            "'" & Format(Date.Now, "yyyy-MM-dd HH:mm:ss") & "'," &
                            rowPayment("contract_recid") & "," &
                            IIf(IsNothing(System.Web.HttpContext.Current.Session("user_link_id")), rowPayment("Sys_Users_RECID"), System.Web.HttpContext.Current.Session("user_link_id")) & "," &
                            "'" & rowPayment("patientNumber") & "'," &
                            "'" & rowPayment("ChartNumber") & "'," &
                            rowPayment("PatientAmount") & "," &
                            rowPayment("applytocurrentinvoice") & "," &
                            rowPayment("applyToPastDue") & "," &
                            rowPayment("applytonextinvoice") & "," &
                            "'" & PaymentType & "'," &
                            "'Reversal of Payment/Adjustment'," &
                            rowPayment("Invoices_recid") & "," &
                            intNewPaymentsBaseRecid & "," &
                            "'" & rowPayment("payername").replace("'", "''") & "'," &
                            "'" & rowPayment("paymentselection") & "'" & "," &
                            -decOrigPaymentAmt & "," &
                            "'" & ReversalComment.Replace("'", "''") & "'," &
                            "'" & rowPayment("PaymentFor") & "'," &
                            rowPayment("doctors_vw") &
                            ")"
                g_IO_Execute_SQL(strSQL, False)

                intNewPaymentsBaseRecid = g_IO_GetLastRecId()

                ' rlo 2016-12-21 - this new record should remain a part of the original payment stream
                '    g_IO_Execute_SQL("Update payments set baserecid = " & intNewPaymentsBaseRecid & " where recid =" & intNewPaymentsBaseRecid, False)

                ' 1/4/17 CS not sure why this was commented out, above by rlo, this IS a new stream of payments today so that it shows up on reports this day/month
                g_IO_Execute_SQL("update payments set baserecid = " & intNewPaymentsBaseRecid & " where recid = " & intNewPaymentsBaseRecid, False)

                ' tie the two payment records together (original payment and the reversal of it)
                g_IO_Execute_SQL("update payments set ReversedBy_recid = " & intNewPaymentsBaseRecid & " where recid = " & rowPayment("recid"), False)
            Next
        End If


    End Sub

    Public Sub g_ReverseClaimPayment(ByVal Payments_Base_Recid As Integer, ByVal PaymentType As String, ByVal ReversalComment As String, ByRef intNewPaymentsBaseRecid As Integer, ByVal OriginalPaymentType As String)

        Dim strSQL = "select recid, claimNumber, DatePosted,applytoclaim, sys_users_recid, patientNumber, ChartNumber, PrimaryAmount, SecondaryAmount, " &
         "PaymentType, PaymentReference, Contract_recid, baserecid, payername, paymentselection, orig_payment, Comments, PaymentFor, Doctors_vw " &
         "from payments where baserecid = " & Payments_Base_Recid & " order by recid"

        Dim tblClaimPayment As DataTable = g_IO_Execute_SQL(strSQL, False)

        If tblClaimPayment.Rows.Count > 0 Then
            Dim rowPayment As DataRow = Nothing
            For Each rowPayment In tblClaimPayment.Rows
                Dim decOrigPaymentAmt As Decimal = rowPayment("orig_payment")

                If Trim(rowPayment("ClaimNumber")) = "" Or Trim(rowPayment("ClaimNumber")) = "-1" Then
                    ' if this payment has not been applied to a claim yet, flag it so that it never does
                    rowPayment("ClaimNumber") = -99
                    g_IO_Execute_SQL("update payments set ClaimNumber = -99 where recid = " & rowPayment("recid"), False)
                ElseIf rowPayment("ClaimNumber") = "-99" Then
                    ' this is a payment that was applied to the insurance account balance, so add the amount back to their balance
                    strSQL = "update contracts set " & rowPayment("PaymentSelection").Replace("Amount", "") & "RemainingBalance = " & rowPayment("PaymentSelection").Replace("Amount", "") & "RemainingBalance + " & rowPayment("PatientAmount") & " where recid = '" & rowPayment("contract_recid") & "'"
                    g_IO_Execute_SQL(strSQL, False)
                Else
                    'reopen the claim and adjust payment made
                    If OriginalPaymentType = "14" Then
                        g_IO_Execute_SQL("update claims set status = 'O' where ClaimNumber = " & rowPayment("claimNumber"), False)
                    Else
                        g_IO_Execute_SQL("update claims set status = 'O', AmountPaid = AmountPaid - " & rowPayment("applytoclaim") & " where ClaimNumber = " & rowPayment("claimNumber"), False)
                    End If
                End If
                '10/7/16 CS Added new field doctors_vw 
                strSQL = "insert into payments (DatePosted, contract_recid, sys_users_recid, patientNumber, ChartNumber, " &
                            "PrimaryAmount, SecondaryAmount,applytoClaim,PaymentType,PaymentReference, " &
                            "ClaimNumber,baserecid, payername, paymentselection, orig_payment, Comments, PaymentFor, Doctors_vw)" &
                            " values (" &
                            "'" & Format(Date.Now, "yyyy-MM-dd HH:mm:ss") & "'," &
                            rowPayment("contract_recid") & "," &
                            IIf(IsNothing(System.Web.HttpContext.Current.Session("user_link_id")), rowPayment("Sys_Users_RECID"), System.Web.HttpContext.Current.Session("user_link_id")) & "," &
                            "'" & rowPayment("patientNumber") & "'," &
                            "'" & rowPayment("ChartNumber") & "'," &
                             "-" & rowPayment("PrimaryAmount") & "," &
                            "-" & rowPayment("SecondaryAmount") & "," &
                            "-" & rowPayment("applyToClaim") & "," &
                            "'" & PaymentType & "'," &
                            "'Reversal of Payment/Adjustment'," &
                            rowPayment("ClaimNumber") & "," &
                            intNewPaymentsBaseRecid & "," &
                            "'" & rowPayment("payername").replace("'", "''") & "'," &
                            "'" & rowPayment("paymentselection") & "'" & "," &
                            -decOrigPaymentAmt & "," &
                            "'" & ReversalComment.Replace("'", "''") & "'," &
                            "'" & rowPayment("PaymentFor") & "'," &
                            rowPayment("doctors_vw") &
                            ")"
                g_IO_Execute_SQL(strSQL, False)

                intNewPaymentsBaseRecid = g_IO_GetLastRecId()
                g_IO_Execute_SQL("Update payments set baserecid = " & intNewPaymentsBaseRecid & " where recid =" & intNewPaymentsBaseRecid, False)

                ' tie the two payment records together
                g_IO_Execute_SQL("update payments set ReversedBy_recid = " & intNewPaymentsBaseRecid & " where recid = " & Payments_Base_Recid, False)
            Next
        End If


    End Sub


    'Public Sub g_AdditionalChargeInvoice(ByVal Contracts_Recid As Integer, ByVal NSFCharge As Decimal)

    '10/7/16 CS Added new field doctors_vw (even tho this sub is commented out...just in case ever implemented in the future??)
    '    Dim strFields As String = _
    '       "InvoiceNo]           " & _
    '       ",[Contracts_recid]   " & _
    '       ",[AmountDue]         " & _
    '       ",[AmountPaid]        " & _
    '       ",[PastDue30]         " & _
    '       ",[PastDueOver30]     " & _
    '       ",[PostDate]          " & _
    '       ",[InvoiceType]       " & _
    '       ",[Status]            " & _
    '       ",[Patients_Recid]    " & _
    '       ",[credit]            " & _
    '       ",[Descr]             " & _
    '       ",[sys_users_recid]   " & _
    '       ",[ChartNumber]       " & _
    '       ",[account_no]        " & _
    '       ",[Bill_date]         " & _
    '       ",[terms]             " & _
    '       ",[name]              " & _
    '       ",[address_Line1]     " & _
    '       ",[address_Line2]     " & _
    '       ",[address_Line3]     " & _
    '       ",[Amount_Due]        " & _
    '       ",[Current_Due]       " & _
    '       ",[Adjusted]          " & _
    '       ",[Paid]              " & _
    '       ",[Total_Due]         " & _
    '       ",[PastDueLessThan30] " & _
    '       ",[Doctors_vw]        "

    '    Dim strInsertSQL As String = "insert into invoices ( " & strFields & ") select " & strFields & " from invoices where invoiceNo = " & ReversedInvoiceNo
    '    g_IO_Execute_SQL(strInsertSQL, False)

    '    Dim intNewInvoiceRecid As Integer = 0
    '    intNewInvoiceRecid = g_IO_GetLastRecId()
    '    Dim strNewInvoiceNumber As String = g_generateNewInvoiceNo(intNewInvoiceRecid)

    '    g_IO_Execute_SQL("update invoices set invoiceno = '" & strNewInvoiceNumber & "' where recid=" & intNewInvoiceRecid, False)

    '    Dim tblInvoices As DataTable = g_IO_Execute_SQL("select * from invoices where recid=" & intNewInvoiceRecid, False)

    '    If tblInvoices.Rows.Count > 0 Then
    '        Dim rowNewInvoice As DataRow = tblInvoices.Rows(0)

    '        g_getInvoiceDelingquences(rowNewInvoice)

    '        g_generate_invoice_history(tblInvoices, 0)

    '        ' update the new invoice record to represent the NSF charge (which might be 0)

    '        ' subtract any pre-payment processed above from current due
    '        rowNewInvoice("Current_due") = rowNewInvoice("Amountdue") - rowNewInvoice("Amountpaid")

    '        If rowNewInvoice("Current_due") < 0 Then rowNewInvoice("Current_due") = 0

    '        rowNewInvoice("Total_due") = rowNewInvoice("Current_due") + rowNewInvoice("PastDueLessThan30") + rowNewInvoice("PastDue30") + rowNewInvoice("PastDueOver30")
    '        rowNewInvoice("Adjusted") = 0

    '        g_IO_Execute_SQL("update invoices set " & _
    '        "current_due = " & rowNewInvoice("Current_due") & "," & _
    '        "PastDueLessThan30 = " & rowNewInvoice("PastDueLessThan30") & "," & _
    '        "PastDue30 = " & rowNewInvoice("PastDue30") & "," & _
    '       "PastDueOver30 = " & rowNewInvoice("PastDueOver30") & "," & _
    '        "Total_due = " & rowNewInvoice("TotalDue") & _
    '        " where recid = " & intNewInvoiceRecid, False)

    '    End If

    '    g_ReprintInvoiceByInvoiceNumber(intNewInvoiceRecid, "")

    'End Sub

    ' create a new Invoice number:  1st 2 digits are YY, last 6 digits are sequential until max is hit, then wraps back to 0 -- built from invoice RECID

    Public Function g_generateNewInvoiceNo(ByVal InvoiceRecid As Integer) As String
        Dim intInvoiceDigitsBeyondYY As Integer = 6

        Dim intMaxNumber As Integer = Space(intInvoiceDigitsBeyondYY).Replace(" ", "9")

        ' Mod math will produce 0 to MaxNumber from the recid.  When recid reaches max it will wrap back to 0
        Dim strNewInvoiceNumber As String = CStr(Year(Today)).Substring(2, 2) & CStr(InvoiceRecid Mod (intMaxNumber + 1)).PadLeft(intInvoiceDigitsBeyondYY, "0")
        Return strNewInvoiceNumber
    End Function

    Public Sub g_generate_invoice_history(ByRef tblContracts As DataTable, ByVal TableRowIndex As Integer, ByVal InvoiceDate As Date)
        g_generate_invoice_history(tblContracts, TableRowIndex, Nothing, False, InvoiceDate)
    End Sub
    Public Sub g_generate_invoice_history(ByRef tblContracts As DataTable, ByVal TableRowIndex As Integer, ByRef tblPreviewPaymentHistory As DataTable, ByVal Preview As Boolean, ByVal InvoiceDate As Date)
        ' prepare Invoice history
        Dim strSQL = "Select * from invoices where contracts_recid = " & tblContracts.Rows(TableRowIndex)("recid") &
                " and (recid >= (select min(recid) from Invoices where contracts_recid = " & tblContracts.Rows(TableRowIndex)("recid") & " and Status = 'O' )" &
                " or recid = " & tblContracts.Rows(TableRowIndex)("NewInvoiceRecid") & ") and postdate <= '" & Format(InvoiceDate, "yyyy-MM-dd") & "' " &
                " order by postDate"

        Dim tblHistory As DataTable = g_IO_Execute_SQL(strSQL, False)

        Dim dteFirstPaymentDateForInvoice As Date

        Dim nvcPaymentInvoiceDate As NameValueCollection = New NameValueCollection
        Dim nvcPaymentInvoiceNos As NameValueCollection = New NameValueCollection
        Dim nvcPaymentInvoiceDelim As NameValueCollection = New NameValueCollection


        For Each rowHistory As DataRow In tblHistory.Rows

            ' preview invoices will not exist in the live data table so will never be processed in this for loop
            If Format(CDate(rowHistory("postdate")), "yyyyMMdd") = Format(Date.Now, "yyyyMMdd") Then
                ' this invoice was just created today and is not a part of history, however pull any payment records linked
            Else
                ' create a history detail line for the invoice, will get sorted into place by the report writer using chartnumber, postdate
                dteFirstPaymentDateForInvoice = tblHistory.Rows(0)("PostDate")

                Dim rowHistoryDetail As DataRow = tblContracts.NewRow
                rowHistoryDetail("chartnumber") = tblContracts.Rows(TableRowIndex)("chartNumber")
                rowHistoryDetail("bill_date") = rowHistory("PostDate")
                rowHistoryDetail("AmountDue") = rowHistory("AmountDue")
                rowHistoryDetail("AmountPaid") = rowHistory("AmountPaid")
                rowHistoryDetail("Current_due") = rowHistory("AmountDue") - rowHistory("AmountPaid")
                rowHistoryDetail("descr") = "(" & Left((Trim(rowHistory("InvoiceNo")) & ") " & rowHistory("descr")), 50)
                rowHistoryDetail("InvoiceNo") = tblContracts.Rows(TableRowIndex)("InvoiceNo")
                rowHistoryDetail("account_no") = tblContracts.Rows(TableRowIndex)("account_no")
                rowHistoryDetail("name") = tblContracts.Rows(TableRowIndex)("name")
                rowHistoryDetail("Address_line1") = tblContracts.Rows(TableRowIndex)("Address_line1")
                rowHistoryDetail("Address_line2") = tblContracts.Rows(TableRowIndex)("Address_line2")
                rowHistoryDetail("Address_line3") = tblContracts.Rows(TableRowIndex)("Address_line3")
                rowHistoryDetail("Total_Due") = tblContracts.Rows(TableRowIndex)("Total_Due")
                rowHistoryDetail("Statement_Date") = Format(InvoiceDate, "MM/dd/yyyy")

                Select Case tblContracts.Rows(TableRowIndex)("Total_due")
                    Case 0
                        ' zero due are sorted to the bottom and will not be printed
                        rowHistoryDetail("recid") = 1
                    Case Else
                        rowHistoryDetail("recid") = 0
                End Select

                tblContracts.Rows.Add(rowHistoryDetail)

                ' record the history displayed on this invoice to facilitate reprinting
                If Preview Then
                Else
                    strSQL = "INSERT INTO Invoices_HistoryDetail" &
                        "([Invoices_recid],[ChartNumber],[Bill_Date],[AmountDue],[amountPaid],[Descr],[InvoiceNo])" &
                    " VALUES (" &
                    tblContracts.Rows(TableRowIndex)("NewInvoiceRecid") &
                    ",'" & rowHistoryDetail("chartnumber") & "'" &
                    ",'" & Format(rowHistoryDetail("bill_date"), "yyyy-MM-dd") & "'" &
                    "," & rowHistoryDetail("AmountDue") &
                    "," & rowHistoryDetail("AmountPaid") &
                    ",'" & rowHistoryDetail("descr") & "'" &
                    ",'" & rowHistoryDetail("InvoiceNo") & "'" &
                    ")"
                    g_IO_Execute_SQL(strSQL, False)
                End If

                ' for the report
                rowHistoryDetail("InvoiceNo") = tblContracts.Rows(TableRowIndex)("InvoiceNo")

            End If

            ' payment history
            strSQL = "select * from payments where invoices_recid = " & rowHistory("recid")
            Dim tblPaymentHistory As DataTable = g_IO_Execute_SQL(strSQL, False)


            For Each rowPaymentHistory As DataRow In tblPaymentHistory.Rows

                strSQL = "Select descr from DropDownList__PaymentType where recid = " & rowPaymentHistory("paymenttype")
                Dim tblPaymentType As DataTable = g_IO_Execute_SQL(strSQL, False)
                Dim strPaymentType As String = "Payment"
                If tblPaymentType.Rows.Count = 0 Then
                Else
                    strPaymentType = tblPaymentType.Rows(0)("descr")
                End If

                If IsNothing(nvcPaymentInvoiceDate(CStr(rowPaymentHistory("baserecid")))) Then
                    nvcPaymentInvoiceDate(CStr(rowPaymentHistory("baserecid"))) = rowPaymentHistory("DatePosted")
                    nvcPaymentInvoiceNos(CStr(rowPaymentHistory("baserecid"))) = ""
                    nvcPaymentInvoiceDelim(CStr(rowPaymentHistory("baserecid"))) = Trim(strPaymentType) & ": " & FormatCurrency(rowPaymentHistory("Orig_payment")) & "   Applied to: "
                End If

                nvcPaymentInvoiceNos(CStr(rowPaymentHistory("baserecid"))) &= nvcPaymentInvoiceDelim(CStr(rowPaymentHistory("baserecid"))) & rowHistory("InvoiceNo")
                nvcPaymentInvoiceDelim(CStr(rowPaymentHistory("baserecid"))) = ","
            Next



        Next


        ' if this is a preview of invoices then add any temp payment record activity generated for the preview
        If IsNothing(tblPreviewPaymentHistory) Then
        Else
            For Each rowPaymentHistory As DataRow In tblPreviewPaymentHistory.Rows
                If rowPaymentHistory("chartnumber") = tblContracts.Rows(TableRowIndex)("chartNumber") Then
                    strSQL = "Select descr from DropDownList__PaymentType where recid = " & rowPaymentHistory("paymenttype")
                    Dim tblPaymentType As DataTable = g_IO_Execute_SQL(strSQL, False)
                    Dim strPaymentType As String = "Payment"
                    If tblPaymentType.Rows.Count = 0 Then
                    Else
                        strPaymentType = tblPaymentType.Rows(0)("descr")
                    End If

                    If IsNothing(nvcPaymentInvoiceDate(CStr(rowPaymentHistory("baserecid")))) Then
                        nvcPaymentInvoiceDate(CStr(rowPaymentHistory("baserecid"))) = rowPaymentHistory("DatePosted")
                        nvcPaymentInvoiceNos(CStr(rowPaymentHistory("baserecid"))) = ""
                        nvcPaymentInvoiceDelim(CStr(rowPaymentHistory("baserecid"))) = Trim(strPaymentType) & ": " & FormatCurrency(rowPaymentHistory("Orig_payment")) & "   Applied to: "
                    End If

                    nvcPaymentInvoiceNos(CStr(rowPaymentHistory("baserecid"))) &= nvcPaymentInvoiceDelim(CStr(rowPaymentHistory("baserecid"))) & tblContracts.Rows(TableRowIndex)("InvoiceNo")
                    nvcPaymentInvoiceDelim(CStr(rowPaymentHistory("baserecid"))) = ","

                End If
            Next
        End If


        For Each strPaymentRecid As String In nvcPaymentInvoiceDate.Keys
            Dim rowPaymentHistoryDetail As DataRow = tblContracts.NewRow
            rowPaymentHistoryDetail("chartnumber") = tblContracts.Rows(TableRowIndex)("chartNumber")
            rowPaymentHistoryDetail("bill_date") = nvcPaymentInvoiceDate(strPaymentRecid)
            rowPaymentHistoryDetail("AmountDue") = 0
            rowPaymentHistoryDetail("AmountPaid") = 0
            rowPaymentHistoryDetail("Current_due") = 0
            rowPaymentHistoryDetail("descr") = nvcPaymentInvoiceNos(strPaymentRecid)
            rowPaymentHistoryDetail("InvoiceNo") = tblContracts.Rows(TableRowIndex)("InvoiceNo")
            rowPaymentHistoryDetail("account_no") = tblContracts.Rows(TableRowIndex)("account_no")
            rowPaymentHistoryDetail("name") = tblContracts.Rows(TableRowIndex)("name")
            rowPaymentHistoryDetail("Address_line1") = tblContracts.Rows(TableRowIndex)("Address_line1")
            rowPaymentHistoryDetail("Address_line2") = tblContracts.Rows(TableRowIndex)("Address_line2")
            rowPaymentHistoryDetail("Address_line3") = tblContracts.Rows(TableRowIndex)("Address_line3")
            rowPaymentHistoryDetail("Total_Due") = tblContracts.Rows(TableRowIndex)("Total_Due")

            Select Case tblContracts.Rows(TableRowIndex)("Total_due")
                Case 0
                    ' zero due are sorted to the bottom and will not be printed
                    rowPaymentHistoryDetail("recid") = 1
                Case Else
                    rowPaymentHistoryDetail("recid") = 0
            End Select

            tblContracts.Rows.Add(rowPaymentHistoryDetail)

            ' record the history displayed on this invoice to facilitate reprinting
            If Preview Then
            Else
                strSQL = "INSERT INTO Invoices_HistoryDetail" &
                    "([Invoices_recid],[ChartNumber],[Bill_Date],[AmountDue],[AmountPaid],[Descr],[InvoiceNo])" &
                " VALUES (" &
                tblContracts.Rows(TableRowIndex)("NewInvoiceRecid") &
                ",'" & rowPaymentHistoryDetail("chartnumber") & "'" &
                ",'" & Format(rowPaymentHistoryDetail("bill_date"), "yyyy-MM-dd") & "'" &
                "," & rowPaymentHistoryDetail("AmountDue") &
                "," & rowPaymentHistoryDetail("AmountPaid") &
                ",'" & rowPaymentHistoryDetail("descr").replace("'", "''") & "'" &
                ",'" & rowPaymentHistoryDetail("InvoiceNo") & "'" &
                ")"

                g_IO_Execute_SQL(strSQL, False)
            End If

        Next


    End Sub

    Public Function g_getInvoiceDelingquences(ByRef Contract As DataRow) As Integer
        ' use the view to extract the payment aging info for this invoice
        Dim strSQL = "select contracts_recid as recid, * from PatientInvoiceAging_vw where contracts_recid = " & Contract("recid")
        Dim tblPatientContractAging As DataTable = g_IO_Execute_SQL(strSQL, False)

        Contract("PastDueLessThan30") = tblPatientContractAging.Rows(0)("amount_delinquent_01_30")
        Contract("PastDue30") = tblPatientContractAging.Rows(0)("amount_delinquent_31_60")
        Contract("PastDueOver30") = tblPatientContractAging.Rows(0)("amount_delinquent_61_90") + tblPatientContractAging.Rows(0)("amount_delinquent_91_Plus")

        Return tblPatientContractAging.Rows(0)("Patients_recid")
    End Function

    Public Function g_getInvoiceDelingquences(ByRef Contract As DataRow, ByVal InvoiceDate As Date) As Integer
        ' use the view to extract the payment aging info for this invoice
        Dim strSQL = "select contracts_recid as recid, * from PatientInvoiceAging_fn('" & Format(InvoiceDate, "yyyy/MM/dd") & "'," & Contract("recid") & ")"
        Dim tblPatientContractAging As DataTable = g_IO_Execute_SQL(strSQL, False)

        Contract("PastDueLessThan30") = tblPatientContractAging.Rows(0)("amount_delinquent_01_30")
        Contract("PastDue30") = tblPatientContractAging.Rows(0)("amount_delinquent_31_60")
        Contract("PastDueOver30") = tblPatientContractAging.Rows(0)("amount_delinquent_61_90") + tblPatientContractAging.Rows(0)("amount_delinquent_91_Plus")

        Return tblPatientContractAging.Rows(0)("Patients_recid")
    End Function


    Public Function g_createInvoice(ByVal SQLQuery As String, ByVal InvoiceType As String) As String
        Return g_createInvoice(SQLQuery, InvoiceType, 0, False, Nothing)
    End Function

    Public Function g_createInvoice(ByVal SQLQuery As String, ByVal InvoiceType As String, ByVal AmountDue As Decimal) As String
        ' override to allow system to create a misc invoice (ie. Reverse Payment, Close Claim, Nothing)
        Return g_createInvoice(SQLQuery, InvoiceType, AmountDue, False, Nothing)
    End Function

    Public Function g_createInvoice(ByVal SQLQuery As String, ByVal InvoiceType As String, ByVal AmountDue As Decimal, ByVal OverrideDate As Date) As String
        ' override to allow system to create a misc invoice (ie. Reverse Payment, Close Claim, OverrideDate)
        Return g_createInvoice(SQLQuery, InvoiceType, AmountDue, False, OverrideDate)
    End Function

    Public Function g_createInvoice(ByVal SQLQuery As String, ByVal InvoiceType As String, ByVal AmountDue As Decimal, ByVal Preview As Boolean, ByVal InvoiceDate As Date) As String
        Dim rptReport = New rptInvoice
        Dim strMinDocumentNumber As String = ""
        Dim strMaxDocumentNumber As String = ""
        If IsDate(InvoiceDate) Then
        Else
            InvoiceDate = Date.Now
        End If

        Dim strSQL = SQLQuery.Substring(UCase(SQLQuery).IndexOf(" FROM CONTRACTS "))

        ' build the Select clause to tack to the table and where clause just built  (add some fields that will be needed to print the invoice
        ' 10/7/16 CS Added new field to invoices, doctors_vw, and need to populate it from the contract
        strSQL = "SELECT recid, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientNumber, PatientFirstPay, ChartNumber, Doctors_vw " &
                ",CONVERT(VARCHAR(10), contractdate,101) as ContractDate, '' as InvoiceNo, Account_ID as account_no, cast('" & Format(InvoiceDate, "yyyy-MM-dd") & " 00:00:00' as datetime) as bill_date " &
                ",'Payment due on receipt of invoice.' as terms" &
                ",'' as name" &
                ",' ' as co_Name" &
                ",' ' as address_line1" &
                ",' ' as address_line2" &
                ",' ' as address_line3" &
                ", PatientMonthlyPayment as AmountDue" &
                ", PatientMonthlyPayment as Current_Due" &
                ",PatientInitialPayment" &
                ",PatientInitialBalance" &
                ",PatientFirstPay" &
                ", 0.00 as AmountPaid" &
                ", 0 as Adjusted" &
                ", 0.00 as PastDueLessThan30" &
                ", 0.00 as PastDue30" &
                ", 0.00 as PastDueOver30" &
                ", 0.00 as total_due" &
                ", 0.00 as Overpaid" &
                ", -1 as NewInvoiceRecid" &
                ", PatientREmainingBalance as RemainingBalance" &
                ", 'INITIAL ORTHO TX INSTALLMENT' as descr_init" &
                ", 'PERIOD ORTHO TX INSTALLMENT' as descr" &
                ", '" & Format(InvoiceDate, "MM/dd/yyyy") & "' as Statement_Date" &
                 " " & strSQL

        Dim tblContracts = g_IO_Execute_SQL(strSQL, False)
        Dim strListOfContractsAlreadyInvoicedThisMonth As String = ""
        Dim strListDelim As String = ""
        Dim tblPaymentsPreview As DataTable = g_IO_Execute_SQL("Select * from payments where recid = -1", False)


        For Each rowContract As DataRow In tblContracts.Rows

            Dim blnNoInvoiceMadeThisMonth As Boolean = False


            If AmountDue > 0 Then
                blnNoInvoiceMadeThisMonth = True
                ' see if we have a session variable available, else default the invoice description
                Dim strDescr As String = "Misc Charge"
                If IsNothing(System.Web.HttpContext.Current.Session("InvoiceDescr")) Then
                Else
                    strDescr = System.Web.HttpContext.Current.Session("InvoiceDescr")
                    ' clear session variable in case user jumps between closing claims and reversing payments
                    System.Web.HttpContext.Current.Session.Remove("InvoiceDescr")
                End If
                ' this is a request for initial invoice (relayed from the contracts screen)
                rowContract("AmountDue") = AmountDue
                rowContract("Current_Due") = AmountDue
                rowContract("descr") = strDescr
                rowContract("Bill_date") = Date.Now
            Else

                ' has there already been an invoice made against this contract? if not then this is the first invoice
                strSQL = "Select cast(count(*) as tinyint) as invoicedAlready from invoices where InvoiceType = 'I' and Contracts_recid = " & rowContract("recid")
                Dim tblInvoices As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblInvoices.Rows(0)("InvoicedAlready") Or rowContract("RemainingBalance") < rowContract("PatientInitialBalance") Then
                Else
                    ' this is a request for initial invoice (relayed from the contracts screen)
                    rowContract("AmountDue") = rowContract("PatientInitialPayment")
                    rowContract("Current_Due") = rowContract("PatientInitialPayment")
                    rowContract("descr") = rowContract("descr_init")
                    'rowContract("Bill_date") = rowContract("PatientFirstPay")
                    rowContract("Bill_date") = Date.Now
                End If

                ' is there already an invoice posted for this month (double check - should have been weeded out in the display grid)
                Dim intPaymentRecordForThisInvoice As Integer = -1

                strSQL = "Select cast(count(*) as tinyint) as invoicedAlready from invoices where InvoiceType = 'I' and Contracts_recid = " & rowContract("recid") &
                            " and postdate between " &
                            "CONVERT(VARCHAR(25),DATEADD(dd,-(DAY('" & Format(InvoiceDate, "yyy-MM-dd") & "')-1),'" & Format(InvoiceDate, "yyy-MM-dd") & "'),101) and " &
                            "dateadd(day,1,CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(DATEADD(mm,1,'" & Format(InvoiceDate, "yyyy-MM-dd") & "'))),DATEADD(mm,1,'" & Format(InvoiceDate, "yyyy-MM-dd") & "')),101))"

                tblInvoices = g_IO_Execute_SQL(strSQL, False)
                If tblInvoices.Rows(0)("InvoicedAlready") Then
                Else
                    blnNoInvoiceMadeThisMonth = True
                End If
            End If

            ' extract patients name and address
            strSQL = "Select RTrim(name_first) + ' ' + LTrim(RTrim(substring(isnull(name_mid,' ') + ' ',1,1))) + ' ' + Name_Last as Name," &
                " isnull(addr_other,'') as address1, addr_street as address2," &
                "isnull(RTrim(addr_city) + ', ' + rtrim(addr_state) + '  ' + addr_zip,'') as address3" &
                " from Patients where Chart_number = '" & rowContract("ChartNumber") & "'"
            Dim tblPatient As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblPatient.Rows.Count = 0 Then
            Else
                rowContract("Name") = tblPatient.Rows(0)("Name")
                rowContract("Address_line1") = tblPatient.Rows(0)("address1")
                rowContract("Address_line2") = tblPatient.Rows(0)("address2")
                rowContract("Address_line3") = tblPatient.Rows(0)("address3")

                If Trim(rowContract("Address_line1")) = "" Then
                    rowContract("Address_line1") = tblPatient.Rows(0)("address2")
                    rowContract("Address_line2") = tblPatient.Rows(0)("address3")
                    rowContract("Address_line3") = ""

                End If
            End If

            ' if all clear then print this invoice
            If blnNoInvoiceMadeThisMonth Then



                ' use the view to extract the payment aging info for this invoice
                Dim intPatients_recid = g_getInvoiceDelingquences(rowContract, InvoiceDate)

                ' get any payment setting out there designated to post to next invoice (from a pre-payment or overpayment)
                strSQL = "select recid, DatePosted, applytonextinvoice, sys_users_recid, patientNumber, ChartNumber, PatientAmount, " &
                         "PaymentType, PaymentReference, Comments,PaymentFor, Contract_recid, baserecid, payername, paymentselection, orig_payment " &
                         "from payments where ChartNumber = " & rowContract("ChartNumber") &
                         " and invoices_recid = -1  and applytonextinvoice <> 0 and PaymentSelection = 'PatientAmount' " &
                         " order by case when PatientAmount >= 0 then 1 else 0 end, DatePosted desc"
                Dim tblPendingInvoicePayment As DataTable = g_IO_Execute_SQL(strSQL, False)


                ' are there unpaid invoices against this contract?
                'tblInvoices = g_IO_Execute_SQL("Select isnull(sum(AmountDue - AmountPaid),0) as UnpaidInvoices from invoices where contracts_recid = " & rowContract("recid") & " and status = 'O'", False)
                'Dim decUnpaidInvoicesAmount = tblInvoices.Rows(0)("UnpaidInvoices")
                ' adjust amount due if this is the last payment
                ' 6/10/15 CS Added check for InvoiceType b/c we don't want to do this w/ manual invoices (ie reversed payment NSF, claim closed/patient billed, etc.)
                If rowContract("RemainingBalance") < rowContract("Amountdue") And InvoiceType = "I" Then
                    rowContract("Amountdue") = rowContract("RemainingBalance")
                End If


                Dim intNewInvoiceRecid As Integer = -99999

                If Preview Then
                Else

                    ' go ahead and create new invoice record which will audit this invoice and show that this invoice has been processed this month
                    ' 10/7/16 CS Added new field, doctors_vw, to store the doctor assigned to contract at time of this transaction
                    ' 1/4/17 CS Need to populate PostDate with the date sent in b/c the user can now change the date of the invoice (visible on invoice and some reports).
                    strSQL = "insert into Invoices (Contracts_recid,AmountDue,PostDate,PastDueLessThan30,PastDue30,PastDueOver30,InvoiceType,status, patients_recid,Descr,sys_users_recid," &
                             "chartNumber,account_no,bill_date,terms,name,address_line1,address_line2,address_line3,current_due,Total_Due, Doctors_vw" &
                             ")" &
                                " values (" &
                                "'" & rowContract("recid") & "'," &
                                rowContract("AmountDue") & "," &
                                "'" & Format(InvoiceDate, "yyyy-MM-dd") & "'," &
                                rowContract("PastDueLessThan30") & "," &
                                rowContract("PastDue30") & "," &
                                rowContract("PastDueOver30") & "," &
                                "'" & InvoiceType & "'," &
                                "'O'," &
                                "'" & intPatients_recid & "'," &
                                "'" & rowContract("descr").replace("'", "''") & "'," &
                                System.Web.HttpContext.Current.Session("user_link_id") & "," &
                                "'" & rowContract("ChartNumber") & "'," &
                                "'" & rowContract("account_no") & "'," &
                                "'" & Format(InvoiceDate, "yyyy-MM-dd") & "'," &
                                "'" & rowContract("terms").replace("'", "''") & "'," &
                                "'" & rowContract("name").replace("'", "''") & "'," &
                                "'" & rowContract("address_line1").replace("'", "''") & "'," &
                                "'" & rowContract("address_line2").replace("'", "''") & "'," &
                                "'" & rowContract("address_line3").replace("'", "''") & "'," &
                                 rowContract("current_due") & "," &
                                 rowContract("total_due") & "," &
                                 rowContract("Doctors_vw") &
                                ")"

                    g_IO_Execute_SQL(strSQL, False)

                    intNewInvoiceRecid = g_IO_GetLastRecId()

                    If AmountDue > 0 Then
                    Else
                        g_IO_Execute_SQL("update contracts set PatientRemainingMonths = PatientRemainingMonths - 1" &
                                         ",PatientRemainingBalance = PatientRemainingBalance - " & rowContract("AmountDue") &
                                        ",PatientAmountBilled = PatientAmountBilled + " & rowContract("AmountDue") &
                                        " where recid = " & rowContract("recid"), False)
                    End If

                End If

                rowContract("NewInvoiceRecid") = intNewInvoiceRecid



                ' handle any pre-payments made to this contract (not yet assigned to an invoice)
                ' read ALL unattached payments associated with this contract and try to attach them to this invoice
                If tblPendingInvoicePayment.Rows.Count > 0 And AmountDue = 0 Then
                    Dim rowPayment As DataRow = Nothing
                    For Each rowPayment In tblPendingInvoicePayment.Rows
                        'If rowPayment("DatePosted") > InvoiceDate Then
                        Dim decOrigPaymentAmt As Decimal = rowPayment("orig_payment")
                        rowContract("AmountPaid") += rowPayment("applytonextinvoice") ' must be flagged as going to next invoice (accumulate if multiple payment records are used against this invoice)

                        If rowContract("AmountPaid") > rowContract("AmountDue") Then

                            ' overpayment - this payment is larger than this invoice, post enough to cover the invoice and close this payment record, then create a new payment record with excess

                            ' apply current payment record to this invoice, reset this payment as if it was for the amount of the invoice but store excess on the record to show what happened
                            '  applytonextinvoice + overpaymentpassedtonextinvoice = original payment amt
                            ' 01/14/15 CS Update contract_recid also, in case payment was posted prior to contract existing

                            '  process current payment

                            If Preview Then


                                Dim rowPreviewPaymentsHolding As DataRow = tblPaymentsPreview.NewRow

                                rowPreviewPaymentsHolding("DatePosted") = rowPayment("DatePosted")
                                rowPreviewPaymentsHolding("Sys_Users_RECID") = rowPayment("Sys_Users_RECID")
                                rowPreviewPaymentsHolding("PatientNumber") = rowPayment("PatientNumber")
                                rowPreviewPaymentsHolding("ChartNumber") = rowPayment("ChartNumber")
                                rowPreviewPaymentsHolding("PatientAmount") = rowPayment("PatientAmount")
                                rowPreviewPaymentsHolding("PaymentType") = rowPayment("PaymentType")
                                rowPreviewPaymentsHolding("PaymentReference") = rowPayment("PaymentReference")
                                rowPreviewPaymentsHolding("Contract_RECID") = rowPayment("Contract_RECID")
                                rowPreviewPaymentsHolding("ApplyToNextInvoice") = rowPayment("ApplyToNextInvoice")
                                rowPreviewPaymentsHolding("BaseRecid") = rowPayment("BaseRecid")
                                rowPreviewPaymentsHolding("Invoices_RECID") = -1
                                rowPreviewPaymentsHolding("orig_payment") = rowPayment("orig_payment")

                                tblPaymentsPreview.Rows.Add(rowPreviewPaymentsHolding)

                            Else

                                strSQL = "update payments set " &
                                            "invoices_recid=" & intNewInvoiceRecid &
                                            ",contract_recid=" & rowContract("recid") &
                                            ",PatientAmount=" & rowContract("AmountDue") &
                                            ",applyToCurrentInvoice=" & rowContract("AmountDue") &
                                            ",OverPaymentPassedToNextInvoice = " & rowContract("AmountPaid") - rowContract("AmountDue") &
                                            ",applytonextinvoice = 0" &
                                            " where recid = " & rowPayment("recid")

                                g_IO_Execute_SQL(strSQL, False)

                                ' overpayment amt needs to be pushed to the next month  - -  Create a new payment record with excess flagged as going to the next invoice
                                ' 2/28/15 modified insert value for contract recid. It was using rowPayment(contract_recid), changed to rowContract("recid")
                                ' 12/21/15 fixed spelling error that was causing this to fail in SQL (changed 'PaymetFor' to 'PaymentFor' in fields list of statement)
                                ' 10/7/16 CS Added new field, doctors_vw, to store the doctor assigned to contract at time of this transaction
                                strSQL = "insert into payments (DatePosted, contract_recid, sys_users_recid, patientNumber, ChartNumber, PatientAmount, applytonextinvoice, " &
                                             "PaymentType,PaymentReference,Invoices_recid,baserecid, payername, paymentselection, orig_payment, Comments, PaymentFor, Doctors_vw)" &
                                             " values (" &
                                             "'" & rowPayment("DatePosted") & "'," &
                                             rowContract("recid") & "," &
                                             rowPayment("sys_users_recid") & "," &
                                             "'" & rowPayment("patientNumber") & "'," &
                                             "'" & rowPayment("ChartNumber") & "'," &
                                              rowContract("amountPaid") - rowContract("AmountDue") & "," &
                                              rowContract("amountPaid") - rowContract("AmountDue") & "," &
                                             "'" & rowPayment("PaymentType") & "'," &
                                             "'" & rowPayment("PaymentReference").replace("'", "''") & "'," &
                                             "-1," &
                                             rowPayment("baserecid") & "," &
                                             "'" & rowPayment("payername").replace("'", "''") & "'," &
                                             "'" & rowPayment("paymentselection") & "'" & "," &
                                             decOrigPaymentAmt & ", " &
                                             "'" & rowPayment("Comments").replace("'", "''") & "'," &
                                             "'" & rowPayment("PaymentFor") & "'," &
                                             rowContract("Doctors_vw") &
                                             ")"

                                g_IO_Execute_SQL(strSQL, False)

                            End If

                            rowContract("Overpaid") = rowContract("AmountPaid") - rowContract("AmountDue")
                            rowContract("AmountPaid") = rowContract("AmountDue")

                            Exit For    ' invoice is satisfied, don't look at any more unapplied payment records in the queue, leave them for the next invoice

                        Else
                            ' apply full current payment record to this invoice
                            ' 3/2/15 CS Move payment amount to applyToCurrentInvoice & relieve applyToNextInvoice
                            'strSQL = "update payments set " & _
                            '            "invoices_recid=" & intNewInvoiceRecid & _
                            '            " where recid = " & rowPayment("recid")

                            If Preview Then

                                Dim rowPreviewPaymentsHolding As DataRow = tblPaymentsPreview.NewRow

                                rowPreviewPaymentsHolding("DatePosted") = rowPayment("DatePosted")
                                rowPreviewPaymentsHolding("Sys_Users_RECID") = rowPayment("Sys_Users_RECID")
                                rowPreviewPaymentsHolding("PatientNumber") = rowPayment("PatientNumber")
                                rowPreviewPaymentsHolding("ChartNumber") = rowPayment("ChartNumber")
                                rowPreviewPaymentsHolding("PatientAmount") = rowPayment("PatientAmount")
                                rowPreviewPaymentsHolding("PaymentType") = rowPayment("PaymentType")
                                rowPreviewPaymentsHolding("PaymentReference") = rowPayment("PaymentReference")
                                rowPreviewPaymentsHolding("Contract_RECID") = rowPayment("Contract_RECID")
                                rowPreviewPaymentsHolding("ApplyToNextInvoice") = rowPayment("ApplyToNextInvoice")
                                rowPreviewPaymentsHolding("BaseRecid") = rowPayment("BaseRecid")
                                rowPreviewPaymentsHolding("Invoices_RECID") = -1
                                rowPreviewPaymentsHolding("orig_payment") = rowPayment("orig_payment")

                                tblPaymentsPreview.Rows.Add(rowPreviewPaymentsHolding)

                            Else
                                strSQL = "update payments set " &
                                            "invoices_recid=" & intNewInvoiceRecid &
                                            ",contract_recid=" & rowContract("recid") &
                                            ",applyToCurrentInvoice=" & rowPayment("applytonextinvoice") &
                                            ",applytonextinvoice = 0" &
                                            " where recid = " & rowPayment("recid")

                                g_IO_Execute_SQL(strSQL, False)
                            End If
                        End If

                        'End If
                    Next
                End If


                ' create a new Invoice number:  1st 2 digits are YY, last 6 digits are sequential until max is hit, then wraps back to 0 -- built from invoice RECID

                Dim strNewInvoiceNumber As String = g_generateNewInvoiceNo(intNewInvoiceRecid)

                Select Case Trim(strMinDocumentNumber)
                    Case ""
                        strMinDocumentNumber = strNewInvoiceNumber
                        strMaxDocumentNumber = strNewInvoiceNumber
                    Case Else
                        strMaxDocumentNumber = strNewInvoiceNumber
                End Select

                rowContract("InvoiceNo") = strNewInvoiceNumber


                ' subtract any pre-payment processed above from current due
                rowContract("Current_due") = rowContract("Amountdue") - rowContract("Amountpaid")

                If rowContract("Current_due") < 0 Then rowContract("Current_due") = 0

                rowContract("Total_due") = rowContract("Current_due") + rowContract("PastDueLessThan30") + rowContract("PastDue30") + rowContract("PastDueOver30")
                rowContract("Adjusted") = 0
                If Preview Then
                Else
                    strSQL = "update invoices set InvoiceNo = " & strNewInvoiceNumber & " ,amountPaid=" & rowContract("AmountPaid") &
                            ",status='" & IIf(rowContract("AmountPaid") = rowContract("AmountDue"), "C", "O") & "',credit=" & rowContract("Overpaid") &
                            ",current_due=" & rowContract("current_Due") & ",total_due=" & rowContract("total_due") &
                            ",adjusted=" & rowContract("adjusted") & ",amountdue=" & rowContract("amountdue") &
                            " where recid = " & intNewInvoiceRecid

                    g_IO_Execute_SQL(strSQL, False)
                End If

            Else
                strListOfContractsAlreadyInvoicedThisMonth &= strListDelim & rowContract("recid")
                strListDelim = ","
            End If



        Next


        strListOfContractsAlreadyInvoicedThisMonth = "," & strListOfContractsAlreadyInvoicedThisMonth.Replace(" ", "") & ","

        ' add history
        For i = tblContracts.Rows.Count - 1 To 0 Step -1
            If strListOfContractsAlreadyInvoicedThisMonth.Contains("," & tblContracts.Rows(i)("recid") & ",") And InvoiceType <> "M" Then
                ' remove any contracts that have already been invoiced
                tblContracts.Rows.RemoveAt(i)
            Else

                'generate history records for this invoice, used by report writer to print history detail
                If Preview Then
                    g_generate_invoice_history(tblContracts, i, tblPaymentsPreview, True, InvoiceDate)
                Else
                    g_generate_invoice_history(tblContracts, i, InvoiceDate)
                End If


                ' set up sort key (recid) for invoices that are being printed
                Select Case tblContracts.Rows(i)("Total_due")
                    Case 0
                        ' zero due are sorted to the bottom and will not be printed
                        tblContracts.Rows(i)("recid") = 1
                    Case Else
                        tblContracts.Rows(i)("recid") = 0
                End Select

                tblContracts.Rows(i)("descr") = "(" & tblContracts.Rows(i)("invoiceNo") & ") " & tblContracts.Rows(i)("descr")

            End If
        Next



        'dowload/print invoice n
        If tblContracts.Rows.Count > 0 Then
            Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "__TYPE__" & System.Web.HttpContext.Current.Session("user_link_userid") & ".PDF"
            Dim strPOFilePath As String = HttpContext.Current.Server.MapPath("downloads\")

            rptReport.SetDataSource(tblContracts)

            strPOFileBase = strPOFileBase.Replace("__TYPE__", "_invoices_" & strMinDocumentNumber & "-" & strMaxDocumentNumber & "_")
            rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFilePath & strPOFileBase)
            Return strPOFileBase
        End If
        Return ""
    End Function

    Public Function CreateInsuranceClaims(ByRef strContractID As String, ByRef blnPreview As Boolean, ByRef strClaimType As String, ByRef strProcedureDate As String)
        ' 11/29/16 CS Changed ByRef ChartNumber to Contract Recid and all references in code below
        ' Also updated anywhere this function was called to send contract recid instead of chartnumber bc there can be more than 1 contract per chartnumber

        Dim blnInitialClaim As Boolean = False
        Dim blnProcessPrimaryClaim As Boolean = False
        Dim blnProcessSecondaryClaim As Boolean = False
        Dim intMonthsSinceContractPaymentsStarted As Integer = 0
        Dim dteProcessDate As Date = Date.Now

        Dim strSQL As String = ""
        Dim strWhere As String = ""
        If strContractID = "" Then
            strWhere = " where ChartNumber in (-1"
            ' no contract recid sent in from claims processing b/c uses frmListManager recid list (filtered by user)
            Dim strSQL_base As String = Split(System.Web.HttpContext.Current.Session("LM_SQL"), "order by")(0)
            strSQL = Right(strSQL_base, strSQL_base.Length - UCase(strSQL_base).IndexOf(" FROM "))
            strSQL = "SELECT ChartNumber, PrimaryInsurancePlans_vw,SecondaryInsurancePlans_vw from contracts where recid in (select recid " & strSQL & ")"
            Dim tblContracts As DataTable = g_IO_Execute_SQL(strSQL, False)
            For Each row In tblContracts.Rows
                strWhere &= "," & row("chartNumber")
            Next
        Else
            ' contract recid sent in from calling form (contract entry/claims processing for specific contract, or from payment processing creating secondary claim on-the-fly)
            strWhere = " where contracts_recid = " & strContractID
        End If
        strWhere &= ")"

        ' rodneys codde
        Dim strViewName As String = "Unprocessed" & strClaimType & "InsuranceClaimsCurrentMonth_vw"

        'RLO 10/25/16 - let user override the date processed on the claims selected ----------------------

        If IsDate(strProcedureDate) Then
            ' we need to extract the view and replace GETDATE with the override date
            dteProcessDate = strProcedureDate
            strProcedureDate = Format(dteProcessDate, "MM/dd/yyyy")

            'get the actual SELECT from the VIEW
            Dim strViewTSQL = g_IO_Execute_SQL("select definition as ViewSQL from sys.objects o join sys.sql_modules m on m.object_id = o.object_id " &
                                               "where o.object_id = object_id( '" & strViewName & "') and o.type = 'V'", False).Rows(0)("ViewSQL")
            strViewTSQL = Mid(strViewTSQL, UCase(strViewTSQL).IndexOf("SELECT") + 1)

            strViewName = "(" & strViewTSQL.replace("GETDATE()", "'" & strProcedureDate & "'") & ") as temp "
        Else
            strProcedureDate = Format(Date.Now, "MM/dd/yyyy")
        End If
        strSQL = "Select *, cast(0.00 As money) As claim_amount, Procedure_Date As FirstClaimDate, '' as claimNumber from " & strViewName & " " & strWhere & " order by insurance_name"

        'RLO 10/25/16  ----------------------

        Dim tblClaims As DataTable = g_IO_Execute_SQL(strSQL, False)
        Dim strColumnNames As String = ""
        Dim strcolumnNamesDelim As String = ""
        For Each colColumn As DataColumn In tblClaims.Columns
            If ",RECID,CLAIM_AMOUNT,FIRSTCLAIMDATE,CLAIMNUMBER,".Contains("," & Trim(UCase(colColumn.ColumnName)) & ",") Then
            Else
                strColumnNames &= strcolumnNamesDelim & colColumn.ColumnName & " "
                strcolumnNamesDelim = ", "
            End If
        Next

        Dim intNewEntriesPoint As Integer = tblClaims.Rows.Count

        ' loop through possible claims, in reverse order, so that we can remove any rows that the claim is not to be processed
        For index = tblClaims.Rows.Count - 1 To 0 Step -1

            Dim rowClaims As DataRow = tblClaims.Rows(index)
            blnInitialClaim = False

            If IsDBNull(rowClaims("contract_date")) Then
                ' 11/18/15 make sure we have a contract date (should never be empty but somehow a contract got saved without a date)
                intMonthsSinceContractPaymentsStarted = -1
            ElseIf rowClaims("contract_date") > Format(CType(strProcedureDate, Date), "yyyy-MM-dd") And strContractID = "" Then
                ' 12/21/15 Rebecca wants to ignore any contracts beyond this current date, during batch processing of claims (no specific chart number sent in)
                intMonthsSinceContractPaymentsStarted = -1
            Else
                intMonthsSinceContractPaymentsStarted = DateDiff(DateInterval.Month, rowClaims("contract_date"), dteProcessDate)
            End If

            ' 7/30/15 do not process claims where contract date is in the future (beyond current month) 
            If intMonthsSinceContractPaymentsStarted < 0 Then
                tblClaims.Rows.RemoveAt(index)
            Else

                If rowClaims("Type") = 0 Then
                    ' this is a primary claim
                    Dim strCurrMonth As String = CType(Month(CDate(strProcedureDate)), String)
                    strSQL = "(Select count(*) as alreadyProcessed from Claims where contracts_recid=" & tblClaims.Rows(index)("contracts_recid") &
                        " and MONTH(procedure_date) = '" & strCurrMonth & "'" & " and type = 0 and plan_id = '" & tblClaims.Rows(index)("plan_id") & "')"
                    Dim tblClaimsProcessed As DataTable = g_IO_Execute_SQL(strSQL, False)

                    ' 11/2/15 CS Check for a claim ever processed for this contract & insurance plan
                    strSQL = "(Select count(*) as alreadyProcessed from Claims where contracts_recid=" & tblClaims.Rows(index)("contracts_recid") &
                        " and type = 0 and plan_id = '" & tblClaims.Rows(index)("plan_id") & "')"
                    Dim tblInitialClaimsProcessed As DataTable = g_IO_Execute_SQL(strSQL, False)

                    If tblClaimsProcessed.Rows(0)("alreadyProcessed") = 0 And tblInitialClaimsProcessed.Rows(0)("alreadyProcessed") = 0 Then
                        ' initial claim

                        rowClaims("procedure_amount") = rowClaims("PrimaryInitialPayment")
                        rowClaims("Claim_amount") = rowClaims("PrimaryClaim_amount_Initial")

                        rowClaims("FirstClaimDate") = Split(CStr(rowClaims("contract_date")), " ")(0)
                        rowClaims("procedure_Date") = Split(CStr(rowClaims("contract_date")), " ")(0)

                        ' 11/2/15 CS Print patient contract months on initial claim
                        rowClaims("RemainingMonths") = rowClaims("PatientContractMonths")

                        blnProcessPrimaryClaim = True
                        blnInitialClaim = True

                    Else
                        ' installment claim
                        rowClaims("procedure_desc") = "PERIOD ORTHO TX INSTALLMENT"
                        rowClaims("procedure_code") = "D8670"
                        rowClaims("procedure_amount") = rowClaims("PrimaryInstallmentAmt")
                        rowClaims("Claim_amount") = rowClaims("PrimaryClaim_amount_Installment")
                        rowClaims("FirstClaimDate") = Split(CStr(rowClaims("contract_date")), " ")(0)
                        ' set date to current date (required by insurance)
                        rowClaims("procedure_date") = Format(Date.Now, "MM/dd/yyyy")
                        ' sending in procedure date from payment entry (to match secondary claim to primary claim's procedure date)
                        If IsDate(strProcedureDate) Then
                            rowClaims("procedure_date") = Format(CDate(strProcedureDate), "MM/dd/yyyy")
                        End If

                        Select Case rowClaims("PrimaryBillingFrequency_vw")
                            Case 1
                                ' Monthly
                                blnProcessPrimaryClaim = True
                            Case 2
                                ' Quarterly
                                If intMonthsSinceContractPaymentsStarted Mod 3 = 0 Then
                                    blnProcessPrimaryClaim = True
                                End If
                            Case 3
                                ' Yearly
                                If intMonthsSinceContractPaymentsStarted Mod 12 = 0 Then
                                    blnProcessPrimaryClaim = True
                                End If
                            Case Else
                                blnProcessPrimaryClaim = False
                        End Select

                    End If
                Else
                    ' this is a secondary claim
                    ' 11/29/2016 Need to use procedure date sent in, if provided
                    'Dim strCurrMonth As String = CType(Month(Date.Now), String)
                    Dim strCurrMonth As String = CType(Month(CDate(strProcedureDate)), String)

                    strSQL = "(Select count(*) as alreadyProcessed from Claims where contracts_recid=" & tblClaims.Rows(index)("contracts_recid") &
                        " and MONTH(procedure_date) = '" & strCurrMonth & "'" & " and type = 1 and plan_id = '" & tblClaims.Rows(index)("plan_id") & "')"
                    Dim tblClaimsProcessed As DataTable = g_IO_Execute_SQL(strSQL, False)

                    ' 11/2/15 CS Check for a claim ever processed for this contract & insurance plan
                    strSQL = "(Select count(*) as alreadyProcessed from Claims where contracts_recid=" & tblClaims.Rows(index)("contracts_recid") &
                        " and type = 1 and plan_id = '" & tblClaims.Rows(index)("plan_id") & "')"

                    Dim tblInitialClaimsProcessed As DataTable = g_IO_Execute_SQL(strSQL, False)

                    If tblClaimsProcessed.Rows(0)("alreadyProcessed") = 0 And tblInitialClaimsProcessed.Rows(0)("alreadyProcessed") = 0 Then

                        ' initial claim

                        rowClaims("procedure_amount") = rowClaims("SecondaryInitialPayment")
                        rowClaims("Claim_amount") = rowClaims("SecondaryClaim_amount_Initial")

                        rowClaims("FirstClaimDate") = Split(CStr(rowClaims("contract_date")), " ")(0)
                        rowClaims("procedure_Date") = Split(CStr(rowClaims("contract_date")), " ")(0)

                        ' 11/2/15 CS Print patient contract months on initial claim
                        rowClaims("RemainingMonths") = rowClaims("PatientContractMonths")

                        blnProcessSecondaryClaim = True
                        blnInitialClaim = True
                    Else
                        ' installment claim
                        rowClaims("procedure_desc") = "PERIOD ORTHO TX INSTALLMENT"
                        rowClaims("procedure_code") = "D8670"
                        rowClaims("procedure_amount") = rowClaims("SecondaryInstallmentAmt")
                        rowClaims("Claim_amount") = rowClaims("SecondaryClaim_amount_Installment")
                        rowClaims("FirstClaimDate") = Split(CStr(rowClaims("contract_date")), " ")(0)
                        ' set date to current date (required by insurance)
                        rowClaims("procedure_date") = Format(CDate(strProcedureDate), "MM/dd/yyyy")
                        ' sending in procedure date from payment entry (to match secondary claim to primary claim's procedure date)
                        If IsDate(strProcedureDate) Then
                            rowClaims("procedure_date") = Format(CDate(strProcedureDate), "MM/dd/yyyy")
                        End If

                        Select Case rowClaims("SecondaryBillingFrequency_vw")
                            Case 1
                                ' monthly
                                blnProcessSecondaryClaim = True
                            Case 2
                                ' Quarterly
                                If intMonthsSinceContractPaymentsStarted Mod 3 = 0 Then
                                    blnProcessSecondaryClaim = True
                                End If

                            Case 3
                                ' Yearly
                                If intMonthsSinceContractPaymentsStarted Mod 12 = 0 Then
                                    blnProcessSecondaryClaim = True
                                End If
                            Case Else
                                blnProcessSecondaryClaim = False
                        End Select
                    End If

                    If blnProcessSecondaryClaim Then
                        '  need a secondary insurance claim form printed

                        Dim rowOtherCoverage As DataRow = tblClaims.NewRow  ' make a copy of the current record

                        For Each colColumn As DataColumn In tblClaims.Columns
                            ' move the info from the live to the copy
                            rowOtherCoverage(colColumn.ColumnName) = rowClaims(colColumn.ColumnName)
                        Next

                        'If intMonthsSinceContractPaymentsStarted = 0 Then
                        '    ' initial claim, use initial payment amount
                        '    rowClaims("procedure_amount") = rowClaims("SecondaryInitialPayment")
                        'End If


                        ' move primary info to secondary and visa-versa
                        rowClaims("insurance_name") = rowOtherCoverage("other_policyholder_company")
                        rowClaims("other_policyholder_company") = rowOtherCoverage("insurance_name")

                        rowClaims("insurance_address1") = rowOtherCoverage("other_policyholder_address1")
                        rowClaims("other_policyholder_address1") = rowOtherCoverage("insurance_address1")

                        rowClaims("insurance_address2") = rowOtherCoverage("other_policyholder_address2")
                        rowClaims("other_policyholder_address2") = rowOtherCoverage("insurance_address2")

                        rowClaims("insurance_city") = rowOtherCoverage("other_policyholder_city")
                        rowClaims("other_policyholder_city") = rowOtherCoverage("insurance_city")

                        rowClaims("insurance_state") = rowOtherCoverage("other_policyholder_state")
                        rowClaims("other_policyholder_state") = rowOtherCoverage("insurance_state")

                        rowClaims("insurance_zip") = rowOtherCoverage("other_policyholder_zip")
                        rowClaims("other_policyholder_zip") = rowOtherCoverage("insurance_zip")

                        rowClaims("other_policyholder_Name") = Trim(rowOtherCoverage("Policyholder_name_first")) & " " & Trim(rowOtherCoverage("Policyholder_name_mid")) & " " & Trim(rowOtherCoverage("Policyholder_name_last"))

                        Try
                            Dim strName As String = rowOtherCoverage("other_policyholder_name")
                            rowClaims("Policyholder_name_first") = Split(strName, " ")(0)
                            strName = Trim(Mid(strName, strName.IndexOf(" ") + 2))
                            rowClaims("Policyholder_name_mid") = Split(strName, " ")(0)
                            If strName.IndexOf(" ") = -1 Then
                                rowClaims("Policyholder_name_last") = rowClaims("Policyholder_name_mid")
                                rowClaims("Policyholder_name_mid") = ""
                            Else
                                rowClaims("Policyholder_name_last") = Split(strName, " ")(1)

                            End If
                        Catch ex As Exception
                            rowClaims("Policyholder_name_last") = rowOtherCoverage("other_policyholder_name")
                            rowClaims("Policyholder_name_mid") = ""
                            rowClaims("Policyholder_name_first") = ""
                        End Try

                        rowClaims("policyholder_dob") = rowOtherCoverage("other_policyholder_dob")
                        rowClaims("other_policyholder_dob") = rowOtherCoverage("policyholder_dob")

                        rowClaims("Policyholder_gender") = rowOtherCoverage("other_policyholder_gender")
                        rowClaims("other_policyholder_gender") = rowOtherCoverage("policyholder_gender")

                        rowClaims("Policyholder_gender") = rowOtherCoverage("other_policyholder_gender")
                        rowClaims("other_policyholder_gender") = rowOtherCoverage("policyholder_gender")

                        rowClaims("Policyholder_subscriberID") = rowOtherCoverage("other_policyholder_subscriberID")
                        rowClaims("other_policyholder_subscriberID") = rowOtherCoverage("policyholder_subscriberID")

                        rowClaims("Policyholder_plan") = rowOtherCoverage("other_policyholder_plan")
                        rowClaims("other_policyholder_plan") = rowOtherCoverage("policyholder_plan")

                        rowClaims("Policyholder_employer") = rowOtherCoverage("other_policyholder_employer")
                        rowClaims("other_policyholder_employer") = rowOtherCoverage("policyholder_employer")

                    End If


                End If


                Dim strClaimNumber As String = ""
                Dim strClaimRecid As String = ""
                Dim strInsert As String = ""


                If blnProcessPrimaryClaim Then
                    ' save  primary claim info processed in the CLAIMS table, save the claim type as 0 -- primary
                    If blnPreview Then
                        strClaimNumber = Format(CDate(strProcedureDate), "yy") & "PREVIEW"
                    Else
                        strInsert = "insert into claims (" & strColumnNames & ",sys_users_recid) " & "Select " & strColumnNames & "," & System.Web.HttpContext.Current.Session("user_link_id") & " as sys_users_recid from UnprocessedPrimaryInsuranceClaimsCurrentMonth_vw where contracts_recid = " & rowClaims("contracts_recid")
                        g_IO_Execute_SQL(strInsert, False)

                        strClaimRecid = g_IO_GetLastRecId()
                        strClaimNumber = Format(CDate(strProcedureDate), "yy") & Right(Trim(strClaimRecid), 4).PadLeft(4, "0")

                        g_IO_Execute_SQL("update claims set FirstClaimDate='" & rowClaims("FirstClaimDate") & "', procedure_date='" & rowClaims("procedure_date") & "', Claim_Amount=" & rowClaims("Claim_Amount") & "," &
                                         "procedure_amount = " & rowClaims("procedure_amount") & ", procedure_desc = '" & rowClaims("procedure_desc") & "', procedure_code = '" & rowClaims("procedure_code") & "'" &
                                         ", claimnumber = '" & strClaimNumber & "', account_id = '" & rowClaims("Account_id") & strClaimNumber &
                                         "' where recid = " & strClaimRecid, False)

                    End If

                    rowClaims("claimNumber") = strClaimNumber
                    rowClaims("account_id") = rowClaims("Account_id") & strClaimNumber

                    'Update Contract Info
                    If blnPreview Then
                    Else
                        ' 5/12/16 CFS - Not sure why we were not updating these amounts when the Initial claim processes 
                        '               but as far as I know they should be updated! Commented out If blnInitialClaim...
                        'If blnInitialClaim Then
                        'Else

                        g_IO_Execute_SQL("update contracts set PrimaryRemainingBalance = PrimaryRemainingBalance - " & rowClaims("procedure_amount") &
                                                 ",PrimaryAmountBilled = PrimaryAmountBilled + " & rowClaims("procedure_amount") &
                                                    " where recid = " & rowClaims("contracts_recid"), False)
                        'End If
                        '11/29/16 CS need to send in contract recid, not chart number as there are multiple contract per chart number now
                        'AttachClaimPayments(tblClaims.Rows(index)("ChartNumber"), strClaimNumber, 0, -1)
                        AttachClaimPayments(tblClaims.Rows(index)("contracts_recid"), strClaimNumber, 0, -1)

                    End If

                ElseIf blnProcessSecondaryClaim Then

                    If blnPreview Then
                        strClaimNumber = Format(CDate(strProcedureDate), "yy") & "PREVIEW"
                    Else
                        strInsert = "insert into claims (" & strColumnNames & ",sys_users_recid) " &
                                "Select " & strColumnNames & "," & System.Web.HttpContext.Current.Session("user_link_id") & " as sys_users_recid " &
                                "from UnprocessedSecondaryInsuranceClaimsCurrentMonth_vw where contracts_recid = " & rowClaims("contracts_recid")
                        g_IO_Execute_SQL(strInsert, False)

                        strClaimRecid = g_IO_GetLastRecId()
                        strClaimNumber = Format(CDate(strProcedureDate), "yy") & Right(Trim(strClaimRecid), 4).PadLeft(4, "0")

                        g_IO_Execute_SQL("update claims set FirstClaimDate='" & rowClaims("FirstClaimDate") & "', " &
                                         "procedure_date='" & rowClaims("procedure_date") & "', " &
                                         "Claim_Amount=" & rowClaims("Claim_Amount") & "," &
                                         "procedure_amount = " & rowClaims("procedure_amount") & ", " &
                                         "procedure_desc = '" & rowClaims("procedure_desc") & "', " &
                                         "procedure_code = '" & rowClaims("procedure_code") & "', " &
                                         "claimnumber = '" & strClaimNumber & "', " &
                                         "account_id = '" & rowClaims("Account_id") & strClaimNumber &
                                         "' where recid = " & strClaimRecid, False)
                    End If

                    rowClaims("claimNumber") = strClaimNumber
                    rowClaims("account_id") = rowClaims("Account_id") & strClaimNumber

                    If blnPreview Then
                    Else
                        If blnInitialClaim Then
                        Else

                            g_IO_Execute_SQL("update contracts set SecondaryRemainingBalance = SecondaryRemainingBalance - " & rowClaims("procedure_amount") &
                                                 ",SecondaryAmountBilled = SecondaryAmountBilled + " & rowClaims("procedure_amount") &
                                                 " where recid = " & rowClaims("contracts_recid"), False)
                        End If
                        '11/29/16 CS need to send in contract recid, not chart number as there are multiple contract per chart number now
                        'AttachClaimPayments(tblClaims.Rows(index)("ChartNumber"), strClaimNumber, 1, -1)
                        AttachClaimPayments(tblClaims.Rows(index)("contracts_recid"), strClaimNumber, 1, -1)

                    End If

                End If
            End If

        Next

        Return tblClaims

    End Function
End Module
