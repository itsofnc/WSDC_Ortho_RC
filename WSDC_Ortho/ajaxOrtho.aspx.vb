Public Class ajaxOrtho
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strAction As String = IIf(IsNothing(Request.QueryString("action")), Request.Form("action"), Request.QueryString("action"))

        '01.02.17 cpb sub-routines for each operation instead of all in the load event
        Select Case strAction
            Case "postPending"
                postPending()
            Case "getPatName"
                Dim strClaimsInvoiceTbl As String = ""
                getPatName(strClaimsInvoiceTbl)  'intCountClaimsInvoice
            Case "pullPaymentInfo"
                pullPaymentInfo()
            Case "moveInsuranceToHistory"
                moveInsuranceToHistory()
            Case "getTransAmount"
                getTransAmount()
        End Select
        '01.02.17 eom--------------------------------------------------------------------------


        'Dim strTable As String = IIf(IsNothing(Request.QueryString("tb")), Request.Form("tb"), Request.QueryString("tb"))
        'Dim blnViewMode As Boolean = IIf(IsNothing(Request.QueryString("vwMode")), Request.Form("vwMode"), Request.QueryString("vwMode"))
        'Dim strWhereUser As String = Nothing
        'Dim strClaimsInvoiceTbl As String = ""
        'Dim intCountClaimsInvoice = 0
        'If IsNothing(Session("user_link_id")) Then
        'Else
        '    strWhereUser = Session("user_link_id")
        'End If

    End Sub

    '.01.02.17 cpb post pending in subroutine
    Private Sub postPending()
        ' 7/15/16 cpb pulled this into private subroutine

        If IsNothing(Session("user_link_id")) Then
            ' 9/19/16 cpb changed this routine to only run if we have a user id, as we should ALWAYS have a current user id
        Else
            Dim strWhereUser As String = Session("user_link_id")
            Dim strTempTable As String = "PaymentsTemp"
            Dim strTempDetailTable As String = "PaymentsTempDetail"

            ' check for any pending payments for current user
            Dim strSQL As String = "Select * From " & strTempTable & " Where Sys_Users_RECID = '" & strWhereUser & "'"
            Dim tblPaymentsTemp As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblPaymentsTemp.Rows.Count > 0 Then
                ' build index of payment types
                strSQL = "Select * From DropDownList__PaymentType"
                Dim tblPaymentTypes As DataTable = g_IO_Execute_SQL(strSQL, False)
                Dim nvcPaymentTypes As New NameValueCollection
                For Each paymentType As DataRow In tblPaymentTypes.Rows
                    nvcPaymentTypes.Add(paymentType("recid"), paymentType("type"))
                Next

                ' init for loop of payments
                Dim intLastPaymentRecid As Integer = -1
                Dim intOrigPaymentRecid As Integer = -1
                Dim tblPaymentTempDetail As DataTable = Nothing

                ' loop payments to be processed
                For Each paymentRow In tblPaymentsTemp.Rows

                    If paymentRow("Contract_RECID") = -1 Then
                        ' No contract found for this payment, use global to create a payment record and  then update the payment record
                        Dim decPatientAmount As Decimal = 0.0
                        Dim intInvoicesRecId As Integer = -1
                        Dim decApplyToPrinciple As Decimal = 0.0
                        Dim decApplyToNextInvoice As Decimal = 0.0
                        If paymentRow("ApplyToPrinciple") > 0 Then
                            decPatientAmount = paymentRow("ApplyToPrinciple")
                            decApplyToPrinciple = paymentRow("ApplyToPrinciple")
                            intInvoicesRecId = -99          ' this hard coded rec id is trigger for month end processing
                        End If
                        If paymentRow("ApplyToNextInvoice") > 0 Then
                            decPatientAmount = paymentRow("ApplyToNextInvoice")
                            decApplyToNextInvoice = paymentRow("ApplyToNextInvoice")
                            intInvoicesRecId = -1           ' this hard coded rec id is trigger for month end processing
                        End If
                        ' create a payment record, then update the apply to amounts properly
                        g_PostPendingData(strTempTable, "Payments", "recid=" & (paymentRow("recid")), "PaymentPosting.aspx", False)
                        intLastPaymentRecid = g_IO_GetLastRecId()
                        If intOrigPaymentRecid = -1 Then
                            intOrigPaymentRecid = intLastPaymentRecid
                        End If
                        strSQL = "update Payments set PatientAmount = '" & decPatientAmount & "'" &
                            ", orig_payment = '" & paymentRow("PatientAmount") & "'" &
                            ", ApplyToCurrentInvoice = '0'" &
                            ", ApplyToPastDue = '0'" &
                            ", ApplyToPrinciple = '" & decApplyToPrinciple & "'" &
                            ", ApplyToNextInvoice = '" & decApplyToNextInvoice & "'" &
                            ", Invoices_RECID = = '" & intInvoicesRecId & "'" &
                            ", BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                        g_IO_Execute_SQL(strSQL, False)

                    Else
                        'Payment has a contract 
                        'Payment is either applied to Patient Balance(Current Invoice/Past Invoice/Principle/Next Invoice), Primary Bal, Sec Bl, or PAYMENT IS A REFUND

                        ' post payment based on payment type
                        Dim strPaymentFrom As String = ""
                        If paymentRow("patientAmount") > 0 Then
                            strPaymentFrom = "P"
                        Else
                            strPaymentFrom = "I"
                        End If

                        If strPaymentFrom = "P" Then
                            ' ---Patient Payment---
                            ' check for payment to current or past due invoices - if so, loop invoices creating payment records as necessary
                            If paymentRow("ApplyToCurrentInvoice") > 0 Or paymentRow("ApplyToPastDue") > 0 Then
                                ' apply payment to patient invoices & create payment record
                                g_applyPaymentToInvoices(paymentRow, intOrigPaymentRecid, strTempTable)
                            End If
                            If paymentRow("ApplyToPrinciple") > 0 Then
                                ' no invoices to post to, just create entry for month end updating
                                Dim decApplyToPrinciple As Decimal = paymentRow("ApplyToPrinciple")
                                ' create a payment record, then update the apply to amounts properly
                                g_PostPendingData(strTempTable, "Payments", "recid=" & (paymentRow("recid")), "PaymentPosting.aspx", False)
                                intLastPaymentRecid = g_IO_GetLastRecId()
                                If intOrigPaymentRecid = -1 Then
                                    intOrigPaymentRecid = intOrigPaymentRecid
                                End If
                                strSQL = "Update Payments set PatientAmount = '" & decApplyToPrinciple & "'" &
                                    ", orig_payment = '" & paymentRow("PatientAmount") & "'" &
                                    ", ApplyToPrinciple = '" & decApplyToPrinciple & "'" &
                                    ", Invoices_RECID = '-99'" &
                                    ", BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                                g_IO_Execute_SQL(strSQL, False)
                                'Update contract remaining paymentRow; 1/29/2016 CS Need to reduce contract's remaining balanace to bill out
                                strSQL = "Update Contracts set PatientRemainingBalance = PatientRemainingBalance - " & decApplyToPrinciple &
                                    " where recid='" & paymentRow("Contract_recid") & "'"
                                g_IO_Execute_SQL(strSQL, False)
                            End If
                            If paymentRow("ApplyToNextInvoice") > 0 Then
                                ' no invoices to post to, just create entry for month end updating
                                Dim decApplyToNextInvoice As Decimal = paymentRow("ApplyToNextInvoice")
                                ' move pending payment into payments table, then update the apply to amounts properly
                                g_PostPendingData(strTempTable, "Payments", "recid=" & (paymentRow("recid")), "PaymentPosting.aspx", False)
                                intLastPaymentRecid = g_IO_GetLastRecId()
                                If intOrigPaymentRecid = -1 Then
                                    intOrigPaymentRecid = intLastPaymentRecid
                                End If
                                strSQL = "Update Payments set PatientAmount = '" & decApplyToNextInvoice & "'" &
                                    ", orig_payment = '" & paymentRow("PatientAmount") & "'" &
                                    ", ApplyToNextInvoice = '" & decApplyToNextInvoice & "'" &
                                    ", Invoices_RECID = '-1'" &
                                    ", BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                                g_IO_Execute_SQL(strSQL, False)

                            End If
                        Else
                            ' ---insurance payment---
                            Dim decPaymentAmt As Decimal = 0.00
                            Dim strSQLWhere As String = ""
                            Dim decOriginalPaymentAmt As Decimal = paymentRow("PrimaryAmount") + paymentRow("SecondaryAmount")
                            ' get details for this payment and post payments from details
                            strSQL = "Select * From paymentsTempDetail Where paymentsTempRecId = '" & paymentRow("recid") & "'"
                            tblPaymentTempDetail = g_IO_Execute_SQL(strSQL, False)
                            For Each pmtDetail In tblPaymentTempDetail.Rows
                                ' create a payment record, then update the apply to amounts properly
                                '   - need a payment record for each distribution from the details file
                                g_PostPendingData(strTempTable, "Payments", "recid=" & (paymentRow("recid")), "PaymentPosting.aspx", False)
                                intLastPaymentRecid = g_IO_GetLastRecId()
                                If intOrigPaymentRecid = -1 Then
                                    intOrigPaymentRecid = intLastPaymentRecid
                                End If
                                If pmtDetail("paymentId") = "PrimaryWait" Then
                                    decPaymentAmt = pmtDetail("paymentAmount")
                                    strSQL = "Update Payments Set " &
                                        "orig_payment = '" & decOriginalPaymentAmt & "'" &
                                        ", BaseRecid = " & intOrigPaymentRecid &
                                        ", ClaimNumber = '-1'"                          ' no claim -- will need to be manually applied
                                    ' ", PrimaryAmount = '" & decPaymentAmt & "'" & -- 01.10.17 cpb took this out b/c payment could be from secondary, but applied to primary wait. and primary has already been updated.
                                    strSQL &= " Where recId= '" & intLastPaymentRecid & "'"
                                    g_IO_Execute_SQL(strSQL, False)
                                ElseIf pmtDetail("paymentId") = "SecondaryWait" Then
                                    ' 01.17.17 cpb must do an update so that the baserecid gets updated, it is currently sitting at -1, also claim number is at InsurnaceName-SeondaryWait
                                    strSQL = "Update Payments Set " &
                                        "orig_payment = '" & decOriginalPaymentAmt & "'" &
                                        ", BaseRecid = " & intOrigPaymentRecid &
                                        ", ClaimNumber = '-1'"
                                    strSQL &= " Where recId= '" & intLastPaymentRecid & "'"
                                    g_IO_Execute_SQL(strSQL, False)
                                    ' 1/3/2017 CS email insurance distribution group that a secondary payment received without a claim 
                                    ' (we don't know what the procedure date needs to be from what that they are paying, so WSDC needs to manually gen claim or they can opt to apply to contract balance)
                                    Dim strEmailTo As String = IIf(IsNothing(ConfigurationManager.AppSettings("emailAutomatedClaimTo")), "", ConfigurationManager.AppSettings("emailAutomatedClaimTo"))
                                    Dim strEmailMessage As String = "A Secondary insurance payment was received without a claim to allocate to. " & vbCrLf
                                    strEmailMessage &= " A claim will need to be processed manually to attach to this payment, or you can opt to allow this payment to be applied to the contract balance. " & vbCrLf
                                    'strEmailMessage &= " Insurance Provider: " & tblClaims.Rows(0)("other_insurancecompanyname") & vbCrLf
                                    strEmailMessage &= " Contact your software vendor for assistance if you feel this email was sent in error. "
                                    g_sendEmail(strEmailTo, "Secondary Payment Received Without a Claim", strEmailMessage)
                                ElseIf pmtDetail("paymentId") = "PrimaryBalance" Or pmtDetail("paymentId") = "SecondaryBalance" Then
                                    Dim strSQLContract As String = ""
                                    decPaymentAmt = pmtDetail("paymentAmount")
                                    strSQL = "Update Payments Set " &
                                    "orig_payment = '" & decOriginalPaymentAmt & "'" &
                                    ", BaseRecid = " & intOrigPaymentRecid &
                                    ", ClaimNumber = '-99'"
                                    strSQLContract = "Update Contracts Set "
                                    If pmtDetail("paymentId") = "PrimaryBalance" Then
                                        ' 01.10.17 cpb - do not need to update amount fields, they are already set and paymentid does not control who supplied the payment, just where it is applied
                                        strSQL &= ", ApplyToPrimaryBalance = '" & decPaymentAmt & "'"  '&
                                        '", PrimaryAmount = '" & decPaymentAmt & "'"
                                        strSQLContract &= "PrimaryRemainingBalance = PrimaryRemainingBalance - " & decPaymentAmt
                                    Else
                                        strSQL &= ", ApplyToSecondaryBalance = '" & decPaymentAmt & "'" ' &
                                        ' ", SecondaryAmount = '" & decPaymentAmt & "'"
                                        strSQLContract &= "SecondaryRemainingBalance = SecondaryRemainingBalance - " & decPaymentAmt
                                    End If
                                    strSQL &= " Where recId= '" & intLastPaymentRecid & "'"
                                    strSQLContract &= " Where recId = '" & paymentRow("Contract_RECID") & "'"
                                    g_IO_Execute_SQL(strSQL, False)
                                    ' 01/07/17 cpb need to update patient data to reduce primary or secondary balance
                                    g_IO_Execute_SQL(strSQLContract, False)
                                Else
                                    ' must have a specific claim number
                                    ' 11/2/16 payment amount is only amount paying to this specific claim
                                    Dim decPayemtAmt As Decimal = pmtDetail("paymentAmount")

                                    strSQL = "update Payments set " &
                                        "ApplyToClaim = '" & decPayemtAmt & "'" &
                                        ", orig_payment = '" & decOriginalPaymentAmt & "'" &
                                        ", BaseRecid = " & intOrigPaymentRecid
                                    strSQL &= IIf(paymentRow("primaryAmount") > 0, ", primaryAmount = '" & decPayemtAmt & "'", ", secondaryAmount = '" & decPayemtAmt & "'")
                                    strSQL &= " where recid=" & intLastPaymentRecid
                                    g_IO_Execute_SQL(strSQL, False)

                                    'Pull Claim Information
                                    ' 1/3/2017 CS added pull of claim type & procedure date so we can see if this is a primary claim being paid 
                                    ' claim procedure date = actually the claim filed to insurance date should only ever happen once as a month.
                                    ' and if so see if we need to process a secondary claim for the same procedure date now that primary has paid
                                    Dim tblClaimInfo As DataTable = g_IO_Execute_SQL(" Select procedure_amount, procedure_date from claims where claimnumber = '" & pmtDetail("paymentId") & "'", False)
                                    If tblClaimInfo.Rows.Count = 1 Then
                                        '11/29/2016 CS Need to specify contract id, bc ChartNumber can be on more than 1 contract
                                        'AttachClaimPayments(row("ChartNumber"), row("ClaimNumber"), 0, intLastPaymentRecid)
                                        AttachClaimPayments(paymentRow("Contract_RECID"), pmtDetail("paymentId"), 0, intLastPaymentRecid)
                                        '01.11.17 cpb/cfs - comment this out -- ask Rodney about fixing view to find secondary claims that need to be processed.
                                        '------if not then perhaps this can just write to a file logging primary ins payments.
                                        'If paymentRow("primaryAmount") > 0 Then
                                        '    ' 01/29/2016 CS Automatically create a Secondary Insurance Claim when the Primary Insurance payment received
                                        '    ' 11/29/2016 CS make sure this contract has secondary insurance, with an open balance
                                        '    strSQL = "select SecondaryRemainingBalance from contracts where recid = '" & paymentRow("Contract_RECID") & "' and SecondaryRemainingBalance > 0"
                                        '    Dim tblSecInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
                                        '    If tblSecInsurance.Rows.Count > 0 Then
                                        '        ' set procedure date from claim that was just paid
                                        '        Dim dteProcedureDate As String = tblClaimInfo.Rows(0)("procedure_date")
                                        '        ' 11/11/16 Make sure we do not already have a secondary claim processed for this procedure date (may have been done manually)
                                        '        strSQL = "select ClaimNumber From Claims where contracts_recid = '" & paymentRow("Contract_RECID") & "' And procedure_date = '" & dteProcedureDate & "' and type = 1"
                                        '        Dim tblSecClaim As DataTable = g_IO_Execute_SQL(strSQL, False)
                                        '        If tblSecClaim.Rows.Count = 0 Then
                                        '            Dim tblClaims As DataTable = CreateInsuranceClaims(pmtDetail("ChartNumber"), False, "Secondary", dteProcedureDate)
                                        '            If tblClaims.Rows.Count > 0 Then
                                        '                ' email insurance distribution group that a claim has been processed and needs to be printed
                                        '                Dim strEmailTo As String = IIf(IsNothing(ConfigurationManager.AppSettings("emailAutomatedClaimTo")), "", ConfigurationManager.AppSettings("emailAutomatedClaimTo"))
                                        '                Dim strEmailMessage As String = "A Secondary insurance claim was processed as the result of receiving payment from Primary insurance provider & no secondary claim found matching that procedure date." & vbCrLf
                                        '                strEmailMessage &= " Insurance Provider: " & tblClaims.Rows(0)("insurance_name") & vbCrLf
                                        '                strEmailMessage &= " Patient Chart #: " & tblClaims.Rows(0)("chartNumber") & vbCrLf
                                        '                strEmailMessage &= " Claim No: " & tblClaims.Rows(0)("claimNumber") & vbCrLf
                                        '                strEmailMessage &= " Procedure Date: " & dteProcedureDate & vbCrLf
                                        '                strEmailMessage &= " Contact your software vendor for assistance if you feel this email was sent in error. "
                                        '                g_sendEmail(strEmailTo, "Secondary Claim Processed via Payment Entry", strEmailMessage)
                                        '            End If
                                        '        End If
                                        '    End If
                                        'End If

                                    End If
                                End If


                                ' remove the temp record just processed; remove while processing so if any new are added, we only remove what we processed.
                                strSQL = "Delete From PaymentsTempDetail Where recid ='" & pmtDetail("recid") & "'"
                                g_IO_Execute_SQL(strSQL, False)
                            Next

                        End If

                        ' remove payments temp all detail record are processed or patient payment is processed
                        strSQL = "Delete From PaymentsTemp Where recid = '" & paymentRow("recid") & "'"
                        g_IO_Execute_SQL(strSQL, False)
                        'reset last & orgi payment recid- finished with processing payment
                        intLastPaymentRecid = -1
                        intOrigPaymentRecid = -1
                    End If
                Next
            Else
                litMessage.Text = "No pending payments were found for posting."
            End If
        End If
    End Sub
    ' 01.02.17 eom

    '01/02/17 cpb move to subroutine
    Private Sub getPatName(ByRef strClaimsInvoiceTbl As String)     'ByRef intCountClaimsInvoice As Integer, 
        Dim strID As String = IIf(IsNothing(Request.Form("id")), Request.QueryString("id"), Request.Form("id"))
        Dim strFrm As String = IIf(IsNothing(Request.Form("frm")), Request.QueryString("frm"), Request.Form("frm"))
        Dim strCon As String = IIf(IsNothing(Request.Form("con")), Request.QueryString("con"), Request.Form("con"))
        'Boolean for add new contract when one contract already exists for the patient
        Dim blnAddNew As Boolean = IIf(IsNothing(Request.Form("addNew")), Request.QueryString("addNew"), Request.Form("addNEw"))
        Dim strVal As String = strID.Substring(3)
        Dim strSql As String = ""
        ' 03/14/16 cpb will need to following for common routine
        Dim strLitMessage As String = ""
        Dim tblPatient As DataTable = Nothing
        Dim strChartNo As String = IIf(strID.IndexOf("cht") = 0, strVal, "")
        Dim strFirstName As String = IIf(strID.IndexOf("fst") = 0, strVal, "")
        Dim strLastName As String = IIf(strID.IndexOf("lst") = 0, strVal, "")
        Dim strModalDDL As String = ""

        ' set this up as a case statement to make a little easier to read -- may want to pull other events out to own routine later?
        Select Case strFrm
            Case "PaymentEntry"
                ' 01.11.17 cpb change insurance table to to an array to be able to separate by insurnace type
                ' 01.11.17 cpb - this will never be used - payment posting now only using ajaxOrtho to update the "post payments" grid -- left for backward compability
                Dim arrInsuranceTable() As String = Nothing
                getPatNameFromPaymentEntry(arrInsuranceTable) 'intCountClaimsInvoice,
                strClaimsInvoiceTbl = arrInsuranceTable(0) + arrInsuranceTable(1) & arrInsuranceTable(2)
                Exit Sub
            Case "Contracts"
                'We are coming from contract entry form that does not need to search for a specific contract
                ' 1/2/17 CS Added Account Id as an option to search by on Contract Entry
                If strID.IndexOf("cht") = 0 Then
                    'Searching on Chart # from Improvis patients...
                    strSql = "SELECT '' as Account_Id, isnull(ip.ChartNo, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, " &
                        "FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB " &
                        "FROM IMPROVIS_PatientData_vw ip " &
                        "WHERE ip.ChartNo = '" & strVal & "'"
                    'We do need to see if their is an existing contract and prompt to verify if user wants to open existing contract or add a new contract
                    'if the user has already seen this prompt and wishes to add new contract then addNew will be set to true and will bypass prompt
                    If blnAddNew = True Then
                    Else
                        Dim strSqlCheck As String = "SELECT recid, ContractDate, isnull(account_id, ' ') account_id, isnull(ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName " &
                        "FROM [Contracts_vw] where ChartNumber = '" & strVal & "'"
                        Dim tblConCheck As DataTable = g_IO_Execute_SQL(strSqlCheck, False)
                        If tblConCheck.Rows.Count > 0 Then
                            strModalDDL = "<h4>&nbsp;A contract already exists for " & tblConCheck.Rows(0)("PatientName") & ".</h4>" &
                           "    <div id=""intCID"" class=""hidden"">" & tblConCheck.Rows(0)("recid") & "</div>" &
                           "    <div id=""intChtID"" class=""hidden"">" & tblConCheck.Rows(0)("ChartNumber") & "</div>" &
                           "    <div id=""intActID"" class=""hidden"">" & tblConCheck.Rows(0)("Account_Id") & "</div>" &
                           "    <div id=""strName"" class=""hidden"">" & tblConCheck.Rows(0)("PatientName") & "</div>"
                            If tblConCheck.Rows.Count > 1 Then
                                'More than one contract may exist, prompt user to select correct contract
                                strModalDDL &= "<script  type=""text/javascript"">jQuery(""#btnGoToContract"").addClass(""hidden"");</script>" &
                                              "<div class=""form-group"">" &
                                              "        <label class=""col-sm-4 control-label"">Select Contract to View:</label>" &
                                              "       <div class=""col-sm-8"">" &
                                              "        <select onchange=""getPatNameCID(this);"" class=""form-control"" style=""width:150px"">" &
                                              "         <option value = ""-1"">Select Contract To View</option>#Options#" &
                                              "        </select>" &
                                              "       </div>" &
                                              "</div>" &
                                              "<div> &nbsp; </div>"
                                Dim strDDLOptions As String = ""
                                For Each row In tblConCheck.Rows
                                    strDDLOptions &= "<option value = """ & row("recid") & """>" & row("Account_Id") & " - Banding Date:" & row("ContractDate") & "</option>"
                                Next
                                strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
                            Else
                                ' only 1 contract found matching criteria, previous popup could have hidden this button, need to be sure it's visible now
                                strModalDDL &= "<script  type=""text/javascript"">jQuery(""#btnGoToContract"").removeClass(""hidden"");</script>"
                            End If
                            litMessage.Text = strModalDDL
                            Exit Sub
                        End If
                    End If
                Else
                    'Searching on Account # from existing contracts
                    strSql = "Select isnull(con.Account_Id, ' ') Account_Id, isnull(con.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, " &
                        "FirstName, MiddleName, LastName, isnull(con.addr_other, ' ') Address1, isnull(con.addr_street, ' ') Address2, isnull(con.addr_city, ' ') City, " &
                        "isnull(con.addr_state, ' ') State, isnull(con.addr_zip, ' ') ZipCode, Gender, DOB " &
                        "FROM Contracts_vw con " &
                        "WHERE con.Account_ID = '" & strVal & "'"

                    'We do need to see if their is an existing contract and prompt to verify if user wants to open existing contract or add a new contract
                    'if the user has already seen this prompt and wishes to add new contract then addNew will be set to true and will bypass prompt
                    If blnAddNew = True Then
                    Else
                        Dim strSqlCheck As String = "SELECT recid, ContractDate, isnull(account_id, ' ') account_id, isnull(ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName " &
                        "FROM [Contracts_vw] where Account_Id = '" & strVal & "'"
                        Dim tblConCheck As DataTable = g_IO_Execute_SQL(strSqlCheck, False)
                        If tblConCheck.Rows.Count > 0 Then
                            strModalDDL = "<h4>&nbsp;A contract already exists for " & tblConCheck.Rows(0)("PatientName") & ".</h4>" &
                           "    <div id=""intCID"" class=""hidden"">" & tblConCheck.Rows(0)("recid") & "</div>" &
                           "    <div id=""intChtID"" class=""hidden"">" & tblConCheck.Rows(0)("ChartNumber") & "</div>" &
                           "    <div id=""intActID"" class=""hidden"">" & tblConCheck.Rows(0)("Account_Id") & "</div>" &
                           "    <div id=""strName"" class=""hidden"">" & tblConCheck.Rows(0)("PatientName") & "</div>"
                            If tblConCheck.Rows.Count > 1 Then
                                'We should never hit this code when searching by account id, but just in case...
                                'More than one contract may exist, prompt user to select correct contract
                                strModalDDL &= "<script  type=""text/javascript"">jQuery(""#btnGoToContract"").addClass(""hidden"");</script>" &
                                              "<div class=""form-group"">" &
                                              "        <label class=""col-sm-4 control-label"">Select Contract to View:</label>" &
                                              "       <div class=""col-sm-8"">" &
                                              "        <select onchange=""getPatNameCID(this);"" class=""form-control"" style=""width:150px"">" &
                                              "         <option value = ""-1"">Select Contract To View</option>#Options#" &
                                              "        </select>" &
                                              "       </div>" &
                                              "</div>" &
                                              "<div> &nbsp; </div>"
                                Dim strDDLOptions As String = ""
                                For Each row In tblConCheck.Rows
                                    strDDLOptions &= "<option value = """ & row("recid") & """>" & row("Account_Id") & " - Banding Date:" & row("ContractDate") & "</option>"
                                Next
                                strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
                            Else
                                ' only 1 contract found matching criteria, previous popup could have hidden this button, need to be sure it's visible now
                                strModalDDL &= "<script  type=""text/javascript"">jQuery(""#btnGoToContract"").removeClass(""hidden"");</script>"
                            End If
                            litMessage.Text = strModalDDL
                            Exit Sub
                        End If


                    End If
                End If
            Case "Patients"
                'We are coming from patient entry form that does not need to search for a specific contract
                If strID.IndexOf("cht") = 0 Then
                    'Searching on Chart #
                    strSql = "SELECT RECID, isnull(ip.patient_id, ' ') PatientKey, isnull(ip.Chart_Number, ' ') ChartNumber,  isnull([Name_First] + ' ' + [Name_Last], '') PatientName, " &
                        "FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB " &
                        "FROM  IMPROVIS_PatientData_vw ip " &
                        "where ip.Chart_Number = '" & strVal & "'"
                Else
                    'Searching on Patient #
                    strSql = "SELECT , isnull(ip.patient_id, ' ') PatientKey, isnull(ip.Chart_Number, ' ') ChartNumber,  isnull([Name_First] + ' ' + [Name_Last], '') PatientName, " &
                        "FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB " &
                        "FROM  IMPROVIS_PatientData_vw ip " &
                        "where ip.patient_id = '" & strVal & "'"
                End If
                'We do need to see if their is an existing patient and prompt to verify add new patient
                Dim tblConCheck As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblConCheck.Rows.Count > 0 Then
                    'A patient already exists, prompt user
                    'Dim strModalDDL As String =
                    strModalDDL =
                       "<h4>A patient record already exists for " & tblConCheck.Rows(0)("PatientName") & ".</h4>" &
                       "    <div id=""intChtID"" class=""hidden"">" & tblConCheck.Rows(0)("ChartNumber") & "</div>" &
                       "    <div id=""intPatID"" class=""hidden"">" & tblConCheck.Rows(0)("PatientKey") & "</div>" &
                       "    <div id=""strName"" class=""hidden"">" & tblConCheck.Rows(0)("PatientName") & "</div>" &
                       "    <div id=""intCID"" class=""hidden"">" & tblConCheck.Rows(0)("RECID") & "</div>"
                    litMessage.Text = strModalDDL
                    Exit Sub
                End If
            Case "PaymentReceipt"
                If strID.IndexOf("fst") = 0 Then
                    'Searching on First Name
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " &
                        " FROM Patients_vw where name_first like '%" & strVal & "%'"
                ElseIf strID.IndexOf("lst") = 0 Then
                    'Searching on Last Name
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " &
                        " FROM Patients_vw where name_last like '%" & strVal & "%'"
                ElseIf strID.IndexOf("cht") = 0 Then
                    'Searching on Last Name
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " &
                        " FROM Patients_vw where Chart_Number = '" & strVal & "'"
                End If
                If IsNothing(strCon) Then
                Else
                    strVal = strCon
                    'searching on recid#
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " &
                        " FROM Patients_vw where recid = '" & strVal & "'"
                End If

                'Was more than one patient found with First/Last Name search 
                Dim tblPatientChk As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblPatientChk.Rows.Count > 1 Then
                    'More than one patient was found, prompt user to select correct patient
                    'Dim strModalDDL As String =
                    strModalDDL =
                                      "<h4>More than 1 patient was found for """ & strVal & """. Please select a patient.</h4>" &
                                      "<div class=""form-group"">" &
                                      "        <label for=""intContractID"" class="" col-sm-3 control-label"">Patient:</label>" &
                                      "       <div class=""col-sm-9"">" &
                                      "        <select id=""intContractID"" onchange=""getPatNameCID(this,'" & IIf(strID.IndexOf("lst") = 0, "lst", "fst") & "');"" class=""form-control"" style=""width:150px"">" &
                                      "         <option value = ""-1"">Choose one</option>#Options#" &
                                      "        </select>" &
                                      "       </div>" &
                                      "</div>"
                    Dim strDDLOptions As String = ""
                    For Each row In tblPatientChk.Rows
                        strDDLOptions &= "<option value = """ & row("recid") & """>" & row("PatientName") & " - Chart #" & row("Chart_Number") & "</option>"
                    Next
                    strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
                    litMessage.Text = strModalDDL
                    Exit Sub
                End If
        End Select

        'get Patient Info
        Dim tblPatientName As DataTable = Nothing
        If IsNothing(tblPatient) Then
            tblPatientName = g_IO_Execute_SQL(strSql, False)
        Else
            tblPatientName = tblPatient
        End If

        Dim blnPastDueInv As Boolean = False

        If tblPatientName.Rows.Count = 1 Then
            If tblPatientName.Rows(0)("PatientName") = "" Then
                litMessage.Text = "Patient Name Not Found"
            Else
                If strFrm = "PaymentEntry" Then
                    ' shoud have always been handled above.
                ElseIf strFrm = "Contracts" Then
                    'Coming from Contracts page
                    ' 1/2/17 CS Added Account Id to this return text
                    litMessage.Text = tblPatientName.Rows(0)("Account_Id") & "~~" & tblPatientName.Rows(0)("PatientName") & "~~" & tblPatientName.Rows(0)("ChartNumber") &
                            "~~" & tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("MiddleName") & "~~" & tblPatientName.Rows(0)("LastName") &
                            "~~" & tblPatientName.Rows(0)("Address1") & "~~" & tblPatientName.Rows(0)("Address2") & "~~" & tblPatientName.Rows(0)("City") &
                            "~~" & tblPatientName.Rows(0)("State") & "~~" & tblPatientName.Rows(0)("ZipCode") & "~~" & tblPatientName.Rows(0)("Gender") &
                            "~~" & tblPatientName.Rows(0)("DOB") & "~~"
                ElseIf strFrm = "Patients" Then
                    'Coming from Patients page
                    litMessage.Text = tblPatientName.Rows(0)("PatientKey") & "~~" & tblPatientName.Rows(0)("PatientName") & "~~" & tblPatientName.Rows(0)("ChartNumber") &
                        "~~" & tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("MiddleName") & "~~" & tblPatientName.Rows(0)("LastName") &
                        "~~" & tblPatientName.Rows(0)("Address1") & "~~" & tblPatientName.Rows(0)("Address2") & "~~" & tblPatientName.Rows(0)("City") &
                        "~~" & tblPatientName.Rows(0)("State") & "~~" & tblPatientName.Rows(0)("ZipCode") & "~~" & tblPatientName.Rows(0)("Gender") &
                        "~~" & tblPatientName.Rows(0)("DOB") & "~~"
                ElseIf strFrm = "PaymentReceipt" Then
                    'Coming from Patients page
                    litMessage.Text = tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("LastName") & "~~" & tblPatientName.Rows(0)("Chart_Number")
                End If

            End If
        ElseIf tblPatientName.Rows.Count < 1 Then
            'If strFrm = "PaymentEntry" Then
            '    'Payment Entry- No Contract found for Patient
            '    litMessage.Text = "No patient found in Improvis."
            'Else
            'If strFrm = "Contracts" Then
            'Contract Entry-Patient not found in Improvis
            litMessage.Text = "No patient found in Improvis."
            'End If
        ElseIf tblPatientName.Rows.Count > 1 Then
            If IsNothing(strCon) Then
                ' this needed for backward compability
            End If

            '---modal is built up in g_patientSearch, but does  not match this ddl, so had to be left here for backward compability.
            'If strModalDDL = "" Then
            ' may have been built if from payments entry and global lookup was used.
            'Dim strModalDDL As String =
            strModalDDL =
                               "<h4>More than 1 contract was found for " & tblPatientName.Rows(0)("PatientName") & ". Please select a contract.</h4>" &
                               "<div class=""form-group"">" &
                               "        <label for=""intContractID"" class="" col-sm-3 control-label""> Contract #:</label>" &
                               "       <div class=""col-sm-9"">" &
                               "        <select id=""intContractID"" onchange=""getPatNameCID(this,'" & IIf(strID.IndexOf("cht") = 0, "cht", "pat") & "');"" class=""form-control"" style=""width:150px"">" &
                               "         <option value = ""-1"">Choose one</option>#Options#" &
                               "        </select>" &
                               "       </div>" &
                               "</div>"
            Dim strDDLOptions As String = ""
            For Each row In tblPatientName.Rows
                ' 1/2/17 CS Show the recid in the ddl b/c the modal will already show the patient name 
                ' (Rebecca knows to pick the higher #...may need to revisit this later...was a quick fix)
                'strDDLOptions &= "<option value = """ & row("recid") & """>" & row("PatientName") & " - " & row("ChartNumber") & "</option>"
                strDDLOptions &= "<option value = """ & row("recid") & """>" & row("recid") & "</option>"
            Next
            strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
            litMessage.Text = strModalDDL
            'End If
            'litMessage.Text = strModalDDL
        End If
    End Sub

    ' 01.02.17 - cpb new routine
    Private Sub getPatNameFromPaymentEntry(ByRef arrClaimsInvoiceTbl() As String) 'ByRef intCountClaimsInvoice As Integer, 

        Dim strID As String = IIf(IsNothing(Request.Form("id")), Request.QueryString("id"), Request.Form("id"))
        Dim strCon As String = IIf(IsNothing(Request.Form("con")), Request.QueryString("con"), Request.Form("con"))
        Dim strVal As String = strID.Substring(3)
        Dim strSql As String = ""
        ' 03/14/16 cpb will need to following for common routine
        Dim strLitMessage As String = ""
        Dim tblPatientName As DataTable = Nothing
        Dim strChartNo As String = IIf(strID.IndexOf("cht") = 0, strVal, "")
        Dim strFirstName As String = IIf(strID.IndexOf("fst") = 0, strVal, "")
        Dim strLastName As String = IIf(strID.IndexOf("lst") = 0, strVal, "")
        Dim strModalDDL As String = ""

        Dim blnPastDueInv As Boolean = False

        ' 03/14/16 cpb use common routine to lookup patient
        If IsNothing(strCon) Then
            tblPatientName = g_patientSearch(strChartNo, strFirstName, strLastName, "", strSql, strLitMessage, strModalDDL)
            If strLitMessage <> "" Then
                litMessage.Text = strLitMessage
            End If
        Else
            strVal = strCon
            'searching on contract#
            strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey, ' ') PatientKey, isnull(Account_Id, '') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [Contracts] c left outer join " &
                " [IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where c.recid = '" & strVal & "'"
            tblPatientName = g_IO_Execute_SQL(strSql, False)

        End If

        If tblPatientName.Rows.Count = 1 Then
            If tblPatientName.Rows(0)("PatientName") = "" Then
                litMessage.Text = "Patient Name Not Found"
            Else
                Dim strDDLValues As String = ""
                Dim strDDLText As String = ""
                If strSql.Contains("Contracts") Then

                    Dim intPatRemaining As Decimal = g_getPatientRemainingBalance(tblPatientName.Rows("0"))
                    Dim decNextInvoice As Decimal = g_getPatientNextInvoice(tblPatientName.Rows("0"))

                    'Pull insurance info for Payment Entry page only & build data
                    Dim tblPrimInsurInfo As DataTable = Nothing
                    Dim tblSecInsurInfo As DataTable = Nothing
                    g_getPatientInsurance(tblPatientName.Rows(0)("PrimaryInsurancePlans_vw"), tblPatientName.Rows(0)("SecondaryInsurancePlans_vw"), tblPrimInsurInfo, tblSecInsurInfo)

                    'pull Claim info 
                    Dim intClaimCount As Integer = 0
                    Dim strInsuranceScript As String = ""
                    Dim strOtherInsList As String = ""
                    Dim strInsRefList As String = ""
                    g_getPatientClaims(True, tblPatientName.Rows(0), -1, strDDLValues, strDDLText, arrClaimsInvoiceTbl, intClaimCount, strInsuranceScript, strOtherInsList, strInsRefList) ', intPatRemaining, decNextInvoice, litMessage.Text)     'intCountClaimsInvoice, 

                    'Pull Current Invoice info (0-30 Days)
                    Dim strCurrentInvoiceSql As String = " Select * from Invoices where status = 'O' and recid >= (Select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "') and contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'  and PostDate  >= DATEADD(day, -30, GETDATE()) order by Bill_date desc "
                    ' used to be ---Dim strInvoiceSql As String = "Select * from PatientInvoiceAging_vw where Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'"
                    Dim tblCurrentInvoiceInfo As DataTable = g_IO_Execute_SQL(strCurrentInvoiceSql, False)

                    'Get Current Invoices
                    ' 03/21/16 cpb added for common code reference.
                    '   --left loop below for backward compability -- not sure if used elsewhere
                    'Dim intCurAmt As Decimal = 0
                    Dim strPatientInvoices As String = ""
                    Dim intCurAmt As Decimal = g_getPatientCurrentAmount(tblPatientName.Rows("0"), strPatientInvoices)

                    Dim intCurInvoice As Integer = -1
                    If tblCurrentInvoiceInfo.Rows.Count > 0 Then
                        'open invoices found
                        For Each row In tblCurrentInvoiceInfo.Rows
                            ' 03/21/16 cpb remmed out, this amount now got with global routine
                            'intCurAmt = CDec(row("AmountDue")) - CDec(row("AmountPaid"))

                            'intCountClaimsInvoice += 1
                            ' 01.11.17 add invoice to open grid for primary insurance
                            arrClaimsInvoiceTbl(0) &= "<tr id=""rowinvoice" & row("InvoiceNo") & """> <th scope=""row"">Invoice</th>" &
                                    "<td>" & row("InvoiceNo") & "</td> " &
                                    "<td>" & Format(row("Bill_date"), "MM/dd/yyyy") & "</td> " &
                                    "<td>$" & Format(row("AmountDue"), "0.00") & "</td> " &
                                    "<td>$" & Format(intCurAmt, "0.00") & " Due</td> " &
                                    "<td><span class=""input-group"">" &
                                        "<span class=""input-group-addon"">$</span>" &
                                        "<input ID=""invoice" & row("InvoiceNo") & """ type=""text"" class=""form-control"" onkeyup=""updateRemaining();checkOverPaymentMaxInput(" & intCurAmt & ", this.id)"" value=""0"" MaxLength=""10""  max=""" & intCurAmt & """  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True"" runat=""server"" />" &
                                        "</span></td>" &
                                    "<td></td> " &
                                    "<td>" & row("InvoiceType") & "</td>" &
                                    "<td>" & row("Descr") & "</td>" &
                                "</tr> "
                            '"<td><input id=""txt" & row("InvoiceNo") & """ type=""text"" runat=""server"" /></td> " &
                        Next
                    Else
                        'no open invoices 
                    End If



                    ' 03/21/16 cpb, use common routine to get past due amount
                    '   --left build of strClaimesINvoiceTbl; not used in payment posting, but not sure if used else where
                    'Pull Past Due Invoice info (31+ days)
                    Dim strInvoiceTablePastDue As String = ""
                    Dim intPastDue As Decimal = g_getPatientPastDueAmount(tblPatientName.Rows("0"), strInvoiceTablePastDue)

                    Dim strPastDueInvoiceSql As String = " select * from Invoices where status = 'O' and recid >= (select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "') and contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'  and PostDate  < DATEADD(day, -30, GETDATE()) order by Bill_date desc"
                    ' used to be ---Dim strInvoiceSql As String = "Select * from PatientInvoiceAging_vw where Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'"
                    Dim tblPastDueInvoiceInfo As DataTable = g_IO_Execute_SQL(strPastDueInvoiceSql, False)
                    'Get Past Due Invoices
                    'Dim intPastDue As Decimal = 0
                    If tblPastDueInvoiceInfo.Rows.Count > 0 Then
                        '    'open past due invoices found
                        For Each row In tblPastDueInvoiceInfo.Rows
                            'Past Due Invoices
                            ' 03/21/16 cpb remmed out, this amount now got with global routine
                            'intPastDue = 0
                            'intPastDue += CDec(row("AmountDue")) - CDec(row("AmountPaid"))
                            blnPastDueInv = True

                            'intCountClaimsInvoice += 1
                            ' 01.11.17 cpb i think this was beginning of codeing building up a grid of invoice data. but this routine is not used.
                            arrClaimsInvoiceTbl(0) &= "<tr  id=""rowinvoice" & row("InvoiceNo") & """> <th scope=""row"">Invoice</th>" &
                                    "<td>" & row("InvoiceNo") & "</td> " &
                                    "<td>" & Format(row("Bill_date"), "MM/dd/yyyy") & "</td> " &
                                    "<td>$" & Format(row("AmountDue"), "0.00") & "</td> " &
                                    "<td>$" & Format(intPastDue, "0.00") & " Due</td> " &
                                    "<td><span class=""input-group"">" &
                                        "<span class=""input-group-addon"">$</span>" &
                                        "<input ID=""invoice" & row("InvoiceNo") & """ type=""text"" class=""form-control"" onkeyup=""updateRemaining();checkOverPaymentMaxInput(" & intPastDue & ", this.id)""  value=""0""  MaxLength=""10""  max=""" & intPastDue & """  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True"" runat=""server"" />" &
                                        "</span></td>" &
                                    "<td></td> " &
                                    "<td>" & row("InvoiceType") & "</td>" &
                                    "<td>" & row("Descr") & "</td>" &
                                "</tr> "
                            '"<td><input id=""txt" & row("InvoiceNo") & """ type=""text"" runat=""server"" /></td> " &
                        Next
                    Else
                        'no open invoices 
                    End If

                    ' 1 - patiet key
                    ' 2 - contract
                    ' 3 - chart#
                    ' 4 - recid
                    ' 5 - primary insurance
                    ' 6 - secondary insurance
                    ' 7 - patient monthly payment
                    ' 8 - current amount
                    ' 9 - patient remaining (c2)
                    '10 - patient remaining
                    '11 - past due (c2)
                    '12 - past due
                    '13 - first name
                    '14 - last name
                    '15 - ddl values
                    '16 - ddltext
                    '17 - doctors_vw
                    '18 - cliams array
                    '19 - claims inv table
                    litMessage.Text = tblPatientName.Rows(0)("PatientKey") & "~~" &
                        tblPatientName.Rows(0)("PatientName") & IIf(IsNothing(tblPatientName.Rows(0)("Account_Id")), "", " - Contract #:" & tblPatientName.Rows(0)("Account_Id")) & "~~" &
                        tblPatientName.Rows(0)("ChartNumber") & "~~" &
                        tblPatientName.Rows(0)("recid") & "~~"
                    If tblPrimInsurInfo.Rows.Count > 0 Then
                        litMessage.Text &= " Primary Insurance: " & tblPrimInsurInfo.Rows(0)("ins_company_name") & "~~"
                    Else
                        litMessage.Text &= " Primary Insurance: N/A " & "~~"
                    End If
                    If tblSecInsurInfo.Rows.Count > 0 Then
                        litMessage.Text &= " Secondary Insurance: " & tblSecInsurInfo.Rows(0)("ins_company_name") & "~~"
                    Else
                        litMessage.Text &= " Secondary Insurance: N/A " & "~~"
                    End If
                    litMessage.Text &= IIf(IsNothing(tblPatientName.Rows(0)("PatientMonthlyPayment")), "", intCurAmt.ToString("C2")) & "~~" &
                        intCurAmt & "~~" &
                        intPatRemaining.ToString("C2") & "~~" &
                        intPatRemaining & "~~" &
                        intPastDue.ToString("C2") & "~~" &
                        intPastDue & "~~" &
                        intCurInvoice & "~~" &
                        blnPastDueInv & "~~" &
                        tblPatientName.Rows(0)("FirstName") & "~~" &
                        tblPatientName.Rows(0)("LastName") & "~~" &
                        strDDLValues & "~~" &
                        strDDLText & "~~" &
                        tblPatientName.Rows(0)("Doctors_vw") & "~~" &
                        BuildJavascriptClaimsArray(tblPatientName.Rows(0)("recid"), "-1") & "~~" &
                        arrClaimsInvoiceTbl(0) & arrClaimsInvoiceTbl(1) & arrClaimsInvoiceTbl(2)
                    'strClaimsInvoiceTbl  01.11.17 cpb need insurance to be separated by type
                Else
                    ''pull Claim info 
                    Dim intClaimCount As Integer = 0
                    Dim strInsuranceScript As String = ""
                    Dim strOtherInsList As String = ""
                    Dim strInsRefList As String = ""
                    ' 01.11.17 cpb need insurance to be separted by type
                    'g_getPatientClaims(True, tblPatientName.Rows(0), -1, strDDLValues, strDDLText, strClaimsInvoiceTbl, intClaimCount, strInsuranceScript, strOtherInsList, strInsRefList) ', 0, 0, litMessage.Text)   ' intCountClaimsInvoice, 
                    g_getPatientClaims(True, tblPatientName.Rows(0), -1, strDDLValues, strDDLText, arrClaimsInvoiceTbl, intClaimCount, strInsuranceScript, strOtherInsList, strInsRefList) ', 0, 0, litMessage.Text)   ' intCountClaimsInvoice, 
                End If
            End If
        ElseIf tblPatientName.Rows.Count < 1 Then
            'Payment Entry- No Contract found for Patient
            litMessage.Text = "No patient found in Improvis."
        ElseIf tblPatientName.Rows.Count > 1 Then
            If IsNothing(strCon) Then
                ' this needed for backward compability
            End If

            '---modal is built up in g_patientSearch, but does  not match this ddl, so had to be left here for backward compability.
            'If strModalDDL = "" Then
            ' may have been built if from payments entry and global lookup was used.
            strModalDDL =
                        "<h4>More than 1 contract was found for " & tblPatientName.Rows(0)("PatientName") & ". Please select a contract.</h4>" &
                        "<div class=""form-group"">" &
                        "        <label for=""intContractID"" class="" col-sm-3 control-label""> Contract #:</label>" &
                        "       <div class=""col-sm-9"">" &
                        "        <select id=""intContractID"" onchange=""getPatNameCID(this,'" & IIf(strID.IndexOf("cht") = 0, "cht", "pat") & "');"" class=""form-control"" style=""width:150px"">" &
                        "         <option value = ""-1"">Choose one</option>#Options#" &
                        "        </select>" &
                        "       </div>" &
                        "</div>"
            Dim strDDLOptions As String = ""
            For Each row In tblPatientName.Rows
                strDDLOptions &= "<option value = """ & row("recid") & """>" & row("PatientName") & " - " & row("ChartNumber") & "</option>"
            Next
            strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
            litMessage.Text = strModalDDL
        End If
    End Sub


    Private Sub pullPaymentInfo()
        Dim blnViewMode As Boolean = IIf(IsNothing(Request.QueryString("vwMode")), Request.Form("vwMode"), Request.QueryString("vwMode"))
        Dim strID As String = IIf(IsNothing(Request.Form("id")), Request.QueryString("id"), Request.Form("id"))
        'searching on patient id, viewing or editing a payment

        'Get Payment Info
        Dim strSql As String = "select * from PaymentsTemp where recid = '" & strID & "'"
        Dim tblPaymentInfo As DataTable = g_IO_Execute_SQL(strSql, False)

        'get Patient Info
        'Searching on Chart #
        strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey,' ') PatientKey, isnull(Account_Id,'') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], ' ') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [Contracts] c left outer join " &
                " [IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where c.ChartNumber = '" & tblPaymentInfo.Rows(0)("ChartNumber") & "' and ip.ChartNo = '" & tblPaymentInfo.Rows(0)("ChartNumber") & "'"

        Dim tblPatientName As DataTable = g_IO_Execute_SQL(strSql, False)
        Dim blnPastDueInv As Boolean = False
        If tblPatientName.Rows.Count > 0 Then
            If tblPatientName.Rows(0)("PatientName") = "" Then
                litMessage.Text = "Patient Name Not Found"
            Else
                'Pull Patient RemainingBalance
                Dim intPatRemaining As Decimal = IIf(IsNothing(tblPatientName.Rows(0)("PatientRemainingBalance")), 0, CDec(tblPatientName.Rows(0)("PatientRemainingBalance")))

                'Pull insurance info for Payment Entry page only & build data

                Dim tblPrimInsurInfo As DataTable = g_IO_Execute_SQL("select ins_company_name from dbo.DropDownList__InsurancePlans where recid = '" & tblPatientName.Rows(0)("PrimaryInsurancePlans_vw") & "'", False)
                Dim tblSecInsurInfo As DataTable = g_IO_Execute_SQL("select ins_company_name from dbo.DropDownList__InsurancePlans where recid = '" & tblPatientName.Rows(0)("SecondaryInsurancePlans_vw") & "'", False)
                'Pull Invoice info
                Dim strInvoiceSql As String = " select * from Invoices  where status = 'O' and  recid >= (select min(recid) from Invoices where status = 0 and Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "') and contracts_recid = '" & tblPatientName.Rows(0)("recid") & "' order by PostDate desc"
                Dim tblInvoiceInfo As DataTable = g_IO_Execute_SQL(strInvoiceSql, False)

                'Pull Current Invoice info (0-30 Days)
                Dim strCurrentInvoiceSql As String = " select * from Invoices where status = 'O' and recid >= (select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "') and contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'  and PostDate  >= DATEADD(day, -30, GETDATE()) "
                ' used to be ---Dim strInvoiceSql As String = "Select * from PatientInvoiceAging_vw where Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'"
                Dim tblCurrentInvoiceInfo As DataTable = g_IO_Execute_SQL(strCurrentInvoiceSql, False)

                'Get Current Invoices
                Dim intCurAmt As Decimal = 0
                Dim intCurInvoice As Integer = -1
                If tblCurrentInvoiceInfo.Rows.Count > 0 Then
                    'open invoices found
                    For Each row In tblCurrentInvoiceInfo.Rows
                        intCurAmt = CDec(row("AmountDue")) - CDec(row("AmountPaid"))
                    Next
                Else
                    'no open invoices 
                End If

                'Pull Past Due Invoice info (31+ days)
                Dim strPastDueInvoiceSql As String = " select * from Invoices where status = 'O' and recid >= (select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "') and contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'  and PostDate  < DATEADD(day, -30, GETDATE()) "
                ' used to be ---Dim strInvoiceSql As String = "Select * from PatientInvoiceAging_vw where Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'"
                Dim tblPastDueInvoiceInfo As DataTable = g_IO_Execute_SQL(strPastDueInvoiceSql, False)


                'Get Past Due Invoices
                Dim intPastDue As Decimal = 0
                If tblPastDueInvoiceInfo.Rows.Count > 0 Then
                    'open past due invoices found
                    For Each row In tblPastDueInvoiceInfo.Rows
                        'Past Due Invoices
                        intPastDue += CDec(row("AmountDue")) - CDec(row("AmountPaid"))
                        blnPastDueInv = True
                    Next
                Else
                    'no open invoices 
                End If


                litMessage.Text &= tblPatientName.Rows(0)("PatientKey") & "~~" & tblPatientName.Rows(0)("PatientName") & " - Contract #:" & tblPatientName.Rows(0)("Account_Id") & "~~" &
                        tblPatientName.Rows(0)("ChartNumber") & "~~" & tblPatientName.Rows(0)("recid") & "~~"
                If tblPrimInsurInfo.Rows.Count > 0 Then
                    litMessage.Text &= " Primary Insurance: " & tblPrimInsurInfo.Rows(0)("ins_company_name") & "~~"
                Else
                    litMessage.Text &= " Primary Insurance: N/A " & "~~"
                End If
                If tblSecInsurInfo.Rows.Count > 0 Then
                    litMessage.Text &= " Secondary Insurance: " & tblSecInsurInfo.Rows(0)("ins_company_name") & "~~"
                Else
                    litMessage.Text &= " Secondary Insurance: N/A " & "~~"

                End If
                litMessage.Text &= IIf(IsNothing(tblPatientName.Rows(0)("PatientMonthlyPayment")), "", intCurAmt.ToString("C2")) & "~~" & intCurAmt & "~~" &
                        intPatRemaining.ToString("C2") & "~~" & intPatRemaining & "~~" &
                        intPastDue.ToString("C2") & "~~" & intPastDue & "~~" & intCurInvoice & "~~" & blnPastDueInv & "~~" & blnViewMode & "~~"
            End If
        Else
            'This is a payment that has no contract 
            strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey,' ') PatientKey, isnull(ip.ChartNo, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], ' ') PatientName, FirstName ,LastName " &
                " FROM [IMPROVIS_PatientData_vw] ip where ChartNo = '" & tblPaymentInfo.Rows(0)("ChartNumber") & "'"
            tblPatientName = g_IO_Execute_SQL(strSql, False)
            litMessage.Text &= tblPatientName.Rows(0)("PatientKey") & "~~" & tblPatientName.Rows(0)("PatientName") & " - Contract #: N/A" & "~~" &
                    tblPatientName.Rows(0)("ChartNumber") & "~~" & "-1" & "~~" & " " & "~~" & "" & "" & "~~" & "" & "~~" &
                    "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & False & "~~" & blnViewMode & "~~"
        End If

        'Only pulling payment info from payments queue (so we do not need to pull orig_payment since payment has not been processed yet)
        If tblPaymentInfo.Rows(0)("PaymentSelection") = "PatientAmount" Then
            litMessage.Text &= "PatientAmount" & "~~" & tblPaymentInfo.Rows(0)("PatientAmount") &
                    "~~" & tblPaymentInfo.Rows(0)("ApplyToCurrentInvoice") & "~~" & tblPaymentInfo.Rows(0)("ApplyToPastDue") &
                    "~~" & tblPaymentInfo.Rows(0)("ApplyToPrinciple") & "~~" & tblPaymentInfo.Rows(0)("ApplyToNextInvoice") & "~~" & tblPaymentInfo.Rows(0)("DatePosted") & "~~"
        ElseIf tblPaymentInfo.Rows(0)("PaymentSelection") = "PrimaryAmount" Then
            litMessage.Text &= "PrimaryAmount" & "~~" & tblPaymentInfo.Rows(0)("PrimaryAmount") & "~~" & "0" & "~~" & "0" &
                    "~~" & "0" & "~~" & "0" & "~~" & tblPaymentInfo.Rows(0)("DatePosted") & "~~"
        ElseIf tblPaymentInfo.Rows(0)("PaymentSelection") = "SecondaryAmount" Then
            litMessage.Text &= "SecondaryAmount" & "~~" & tblPaymentInfo.Rows(0)("SecondaryAmount") & "~~" & "0" & "~~" & "0" &
                    "~~" & "0" & "~~" & "0" & "~~" & tblPaymentInfo.Rows(0)("DatePosted") & "~~"
        End If

        litMessage.Text &= tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("LastName") & "~~" & tblPaymentInfo.Rows(0)("PayerName") & "~~" & tblPaymentInfo.Rows(0)("ClaimNumber") & "~~" & tblPaymentInfo.Rows(0)("Comments") & "~~" & tblPaymentInfo.Rows(0)("PaymentReference") & "~~" & CInt(tblPaymentInfo.Rows(0)("ClaimIndex")) & "~~"


    End Sub

    Private Sub moveInsuranceToHistory()
        Dim strChart As String = IIf(IsNothing(Request.Form("id")), Request.QueryString("id"), Request.Form("id"))
        Dim strCoverage As String = IIf(IsNothing(Request.Form("covType")), Request.QueryString("covType"), Request.Form("covType"))
        g_moveInsuranceToHistory(strChart, strCoverage)

    End Sub

    Private Sub getTransAmount()
        ' get transactioncode default amount
        Dim strTrCode As String = IIf(IsNothing(Request.QueryString("trcode")), Request.Form("trcode"), Request.QueryString("trcode"))
        Dim intTrAmount As Integer = 0
        Dim strSQL As String = "select amount_billable from DropDownList__TransactionCodes where recid = '" & strTrCode & "'"
        Dim tblTrCodes As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblTrCodes.Rows.Count > 0 Then
            intTrAmount = CInt(tblTrCodes.Rows(0)("amount_billable"))
        End If
        litMessage.Text = Math.Round(intTrAmount, 2)
    End Sub
End Class