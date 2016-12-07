Public Class ajaxOrtho
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strAction As String = IIf(IsNothing(Request.QueryString("action")), Request.Form("action"), Request.QueryString("action"))
        Dim strTable As String = IIf(IsNothing(Request.QueryString("tb")), Request.Form("tb"), Request.QueryString("tb"))
        Dim blnViewMode As Boolean = IIf(IsNothing(Request.QueryString("vwMode")), Request.Form("vwMode"), Request.QueryString("vwMode"))
        Dim strWhereUser As String = Nothing
        Dim strClaimsInvoiceTbl As String = ""
        Dim intCountClaimsInvoice = 0
        If IsNothing(Session("user_link_id")) Then
        Else
            strWhereUser = Session("user_link_id")
        End If

        If strAction = "postPending" Then
            'This routine is to post data from pending table to live table 
            'Send Pending Tbl, Live Tbl, 
            Dim strSQL As String = "Select * from " & strTable & IIf(IsNothing(strWhereUser), "", " where Sys_Users_RECID = '" & strWhereUser & "'")
            Dim tblPaymentsTemp As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblPaymentsTemp.Rows.Count > 0 Then
                Dim intLastPaymentRecid As Integer = -1
                Dim intOrigPaymentRecid As Integer = -1
                For Each row In tblPaymentsTemp.Rows
                    'Check to see if contract_RECID = -1 (Payment for patient that has no contract yet)
                    If row("Contract_RECID") = -1 Then
                        'Just Create payment records since payment does not have a contract associated with it
                        If row("ApplyToPrinciple") > 0 Then
                            'Create payment record for payment made towards principle bal
                            g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)
                            intLastPaymentRecid = g_IO_GetLastRecId()
                            If intOrigPaymentRecid = -1 Then
                                intOrigPaymentRecid = g_IO_GetLastRecId()
                            End If

                            'Update Payments & set Apply To Amount 
                            strSQL = "update Payments set PatientAmount = '" & row("ApplyToPrinciple") & "', orig_payment = '" & row("PatientAmount") & "', ApplyToCurrentInvoice = '0', ApplyToPastDue = '0', ApplyToNextInvoice = '0', Invoices_RECID = -99, BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                            g_IO_Execute_SQL(strSQL, False)
                        End If
                        If row("ApplyToNextInvoice") > 0 Then
                            'Create payment record for payment made towards next invoice
                            g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)
                            intLastPaymentRecid = g_IO_GetLastRecId()
                            If intOrigPaymentRecid = -1 Then
                                intOrigPaymentRecid = g_IO_GetLastRecId()
                            End If

                            strSQL = "update Payments set PatientAmount = '" & row("ApplyToNextInvoice") & "', orig_payment = '" & row("PatientAmount") & "', ApplyToCurrentInvoice = '0', ApplyToPastDue = '0', ApplyToPrinciple = '0', Invoices_RECID = -1, BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                            g_IO_Execute_SQL(strSQL, False)
                        End If
                    Else
                        'Payment has a contract 
                        'Payment is either applied to Patient Balance(Current Invoice/Past Invoice/Principle/Next Invoice), Primary Bal, Sec Bl, or PAYMENTS IS A REFUND

                        'Pull contract info for patient
                        Dim tblContracts As DataTable = g_IO_Execute_SQL("select * from Contracts where recid = '" & row("Contract_RECID") & "'", False)
                        Dim intContractRecid As Integer = tblContracts.Rows(0)("recid")
                        Dim strUpdateSql As String = ""

                        If row("PaymentSelection") = "PatientAmount" Then
                            'Patient Payment
                            If row("ApplyToCurrentInvoice") > 0 Then
                                'Applied to Current Invoice(s) with in the last 30 days
                                'Create payment record(s) for payment made towards current invoice(s)
                                g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)

                                intLastPaymentRecid = g_IO_GetLastRecId()
                                If intOrigPaymentRecid = -1 Then
                                    intOrigPaymentRecid = g_IO_GetLastRecId()
                                End If

                                'Update payment record (patientamount,orig_payment, base recid, & apply to amounts)
                                strSQL = "update Payments set PatientAmount = '" & row("ApplyToCurrentInvoice") & "', orig_payment = '" & row("PatientAmount") & "',  BaseRecid = " & intOrigPaymentRecid & ", ApplyToPastDue = '0', ApplyToPrinciple = '0', ApplyToNextInvoice = '0' where recid=" & intLastPaymentRecid
                                g_IO_Execute_SQL(strSQL, False)

                                'Pull invoice info to attach current invoice to payment
                                Dim tblCurrentInvoices As DataTable = g_IO_Execute_SQL("Select * from invoices where status = 'O' and recid >= (select min(recid) from Invoices where status = 'O' and Contracts_recid = '" & row("Contract_RECID") & "') and contracts_recid = '" & row("Contract_recid") & "'" &
                                              " and PostDate  >= DATEADD(day, -30, GETDATE()) ", False)
                                If tblCurrentInvoices.Rows.Count > 0 Then
                                    'Attach most recent invoice to payment
                                    AttachInvoicePayments(tblCurrentInvoices.Rows(0)("Contracts_recid"), tblCurrentInvoices.Rows(0)("InvoiceNo"), tblCurrentInvoices.Rows(0)("AmountDue"), "current", intLastPaymentRecid)
                                End If
                            End If

                            'Applied to Past Due Invoice(s) over 30 days
                            If row("ApplyToPastDue") > 0 Then
                                'Create payment record(s) for payment made towards past due invoice(s)
                                g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)

                                intLastPaymentRecid = g_IO_GetLastRecId()
                                If intOrigPaymentRecid = -1 Then
                                    intOrigPaymentRecid = g_IO_GetLastRecId()
                                End If

                                'Update payment record (patientamount,orig_payment, base recid, & apply to amounts)
                                strSQL = "update Payments set PatientAmount = '" & row("ApplyToPastDue") & "', orig_payment = '" & row("PatientAmount") & "', BaseRecid = " & intOrigPaymentRecid & ", ApplyToCurrentInvoice = '0', ApplyToPrinciple = '0', ApplyToNextInvoice = '0'  where recid=" & intLastPaymentRecid
                                g_IO_Execute_SQL(strSQL, False)

                                'Pull invoice info to attach past due invoice to payment
                                Dim tblPastDueInvoices As DataTable = g_IO_Execute_SQL("Select * from invoices where status = 'O' and recid >= (select min(recid) from Invoices where status = 'O' and Contracts_recid = '" & row("Contract_recid") & "') and contracts_recid = '" & row("Contract_recid") & "'" &
                                              " and PostDate  < DATEADD(day, -30, GETDATE()) ", False)
                                If tblPastDueInvoices.Rows.Count > 0 Then
                                    'Attach most recent invoice to payment
                                    AttachInvoicePayments(tblPastDueInvoices.Rows(0)("Contracts_recid"), tblPastDueInvoices.Rows(0)("InvoiceNo"), tblPastDueInvoices.Rows(0)("AmountDue"), "pastdue", intLastPaymentRecid)
                                End If
                            End If

                            'payment will be applied to account balance or Next Invoice
                            If row("ApplyToPrinciple") > 0 Then
                                'Create payment record for payment made towards principle bal
                                g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)
                                intLastPaymentRecid = g_IO_GetLastRecId()
                                If intOrigPaymentRecid = -1 Then
                                    intOrigPaymentRecid = g_IO_GetLastRecId()
                                End If
                                'Update Payments & set Apply To Amount 
                                strSQL = "update Payments set PatientAmount = '" & row("ApplyToPrinciple") & "', orig_payment = '" & row("PatientAmount") & "', ApplyToCurrentInvoice = '0', ApplyToPastDue = '0', ApplyToNextInvoice = '0' , BaseRecid = " & intOrigPaymentRecid & "  where recid=" & intLastPaymentRecid
                                g_IO_Execute_SQL(strSQL, False)
                                ' 1/29/2016 CS Need to reduce contract's remaining balanace to bill out
                                'Update contract remaining balance
                                strSQL = "update Contracts set PatientRemainingBalance = PatientRemainingBalance - " & row("ApplyToPrinciple") & " where recid=" & intContractRecid
                                g_IO_Execute_SQL(strSQL, False)
                            End If

                            If row("ApplyToNextInvoice") > 0 Then
                                'Create payment record for payment made towards next invoice
                                g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)
                                intLastPaymentRecid = g_IO_GetLastRecId()
                                If intOrigPaymentRecid = -1 Then
                                    intOrigPaymentRecid = g_IO_GetLastRecId()
                                End If

                                strSQL = "update Payments set  PatientAmount = '" & row("ApplyToNextInvoice") & "', orig_payment = '" & row("PatientAmount") & "',  ApplyToCurrentInvoice = '0', ApplyToPastDue = '0', ApplyToPrinciple = '0' , BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                                g_IO_Execute_SQL(strSQL, False)
                            End If


                        End If

                        If row("PaymentSelection") = "PrimaryAmount" Then
                            g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)
                            intLastPaymentRecid = g_IO_GetLastRecId()
                            If intOrigPaymentRecid = -1 Then
                                intOrigPaymentRecid = g_IO_GetLastRecId()
                            End If
                            strSQL = "update Payments set ApplyToClaim = '" & row("PrimaryAmount") & "', orig_payment = '" & row("PrimaryAmount") & "', BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                            g_IO_Execute_SQL(strSQL, False)

                            'Do we have a claim to attach payment to? 
                            If row("ClaimNumber") <> "" OrElse row("ClaimNumber") <> "-1" OrElse row("ClaimNumber") <> "-99" Then
                                'Pull Primary Claim Amount
                                Dim tblPrimClaimAmt As DataTable = g_IO_Execute_SQL(" select procedure_amount from claims where claimnumber = '" & row("ClaimNumber") & "'", False)
                                If tblPrimClaimAmt.Rows.Count = 1 Then
                                    '11/29/2016 CS Need to specify contract id, bc ChartNumber can be on more than 1 contract
                                    'AttachClaimPayments(row("ChartNumber"), row("ClaimNumber"), 0, intLastPaymentRecid)
                                    AttachClaimPayments(row("Contract_RECID"), row("ClaimNumber"), 0, intLastPaymentRecid)
                                End If
                            End If

                            ' 01/29/2016 CS Automatically create a Secondary Insurance Claim when the Primary Insurance payment received
                            '11/29/2016 CS make sure this contract has secondary insurance, with an open balance
                            strSQL = "select SecondaryRemainingBalance from contracts where chartNumber = '" & row("ChartNumber") & "' and SecondaryRemainingBalance > 0"
                            Dim tblSecInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
                            If tblSecInsurance.Rows.Count > 0 Then
                                ' get procedure date from claim that was just paid
                                Dim dteProcedureDate As String = Format(Today(), "YYYY-mm-dd")
                                strSQL = "select procedure_date from claims where claimNumber = '" & row("ClaimNumber") & "'"
                                Dim tblClaimInfo As DataTable = g_IO_Execute_SQL(strSQL, False)
                                If tblClaimInfo.Rows.Count > 0 Then
                                    dteProcedureDate = Format(tblClaimInfo("procedure_date"), "YYYY-mm-dd")
                                End If
                                ' 11/11/16 Make sure we do not already have a secondary claim processed for this procedure date (maybe done manually)
                                strSQL = "select ClaimNumber From Claims where chartNumber = '" & row("ChartNumber") & "' And procedure_date = '" & Format(dteProcedureDate, "YYYY-mm-dd") & "' and type = 1"
                                Dim tblSecClaim As DataTable = g_IO_Execute_SQL(strSQL, False)
                                If tblSecClaim.Rows.Count = 0 Then
                                    Dim tblClaims As DataTable = CreateInsuranceClaims(row("ChartNumber"), False, "Secondary", dteProcedureDate)
                                    If tblClaims.Rows.Count > 0 Then
                                        ' email insurance distribution group that a claim has been processed and needs to be printed
                                        Dim strEmailTo As String = IIf(IsNothing(ConfigurationManager.AppSettings("emailAutomatedClaimTo")), "", ConfigurationManager.AppSettings("emailAutomatedClaimTo"))
                                        Dim strEmailMessage As String = "A Secondary insurance claim was processed as the result of receiving payment from Primary insurance provider & no secondary claim found matching that procedure date." & vbCrLf
                                        strEmailMessage &= " Insurance Provider: " & tblClaims.Rows(0)("insurance_name") & vbCrLf
                                        strEmailMessage &= " Patient Chart #: " & tblClaims.Rows(0)("chartNumber") & vbCrLf
                                        strEmailMessage &= " Claim No: " & tblClaims.Rows(0)("claimNumber") & vbCrLf
                                        strEmailMessage &= " Procedure Date: " & dteProcedureDate
                                        g_sendEmail(strEmailTo, "Secondary Claim Processed via Payment Entry", strEmailMessage)
                                    End If
                                End If
                            End If
                        End If

                        If row("PaymentSelection") = "SecondaryAmount" Then
                            g_PostPendingData(strTable, "Payments", "recid=" & (row("recid")), "PaymentPosting.aspx", False)
                            intLastPaymentRecid = g_IO_GetLastRecId()
                            If intOrigPaymentRecid = -1 Then
                                intOrigPaymentRecid = g_IO_GetLastRecId()
                            End If
                            strSQL = "update Payments set ApplyToClaim = '" & row("SecondaryAmount") & "', orig_payment = '" & row("SecondaryAmount") & "', BaseRecid = " & intOrigPaymentRecid & " where recid=" & intLastPaymentRecid
                            g_IO_Execute_SQL(strSQL, False)

                            'Do we have a claim to attach payment to? 
                            If row("ClaimNumber") <> "" OrElse row("ClaimNumber") <> "-1" OrElse row("ClaimNumber") <> "-99" Then
                                'Pull Secondary Claim Amount
                                Dim tblPrimClaimAmt As DataTable = g_IO_Execute_SQL(" select procedure_amount from claims where claimnumber = '" & row("ClaimNumber") & "'", False)
                                If tblPrimClaimAmt.Rows.Count = 1 Then
                                    '11/29/2016 CS Need to specify contract id, bc ChartNumber can be on more than 1 contract
                                    'AttachClaimPayments(row("ChartNumber"), row("ClaimNumber"), 1, intLastPaymentRecid)
                                    AttachClaimPayments(row("Contract_RECID"), row("ClaimNumber"), 1, intLastPaymentRecid)
                                End If
                            Else
                                ' 01/29/2016 CS Automatically create a Secondary Insurance Claim when a secondary payment is received and no claim exists to attach it to
                                Dim dteProcedureDate As String = Format(Today(), "YYYY-mm-dd")
                                If row("ClaimNumber") = "-1" Then
                                    Dim tblClaims As DataTable = CreateInsuranceClaims(row("ChartNumber"), False, "Secondary", dteProcedureDate)
                                    If tblClaims.Rows.Count > 0 Then
                                        ' email insurance distribution group that a claim has been processed and needs to be printed
                                        Dim strEmailTo As String = IIf(IsNothing(ConfigurationManager.AppSettings("emailAutomatedClaimTo")), "", ConfigurationManager.AppSettings("emailAutomatedClaimTo"))
                                        Dim strEmailMessage As String = "A Secondary insurance claim was processed as the result of receiving payment from the Secondary insurance provider and no claim was available or selected to apply the payment to." & vbCrLf
                                        strEmailMessage &= " Insurance Provider: " & tblClaims.Rows(0)("other_insurancecompanyname") & vbCrLf
                                        strEmailMessage &= " Patient Chart #: " & tblClaims.Rows(0)("chartNumber") & vbCrLf
                                        strEmailMessage &= " Claim No: " & tblClaims.Rows(0)("claimNumber") & vbCrLf
                                        strEmailMessage &= " Procedure Date: " & dteProcedureDate
                                        g_sendEmail(strEmailTo, "Secondary Claim Processed via Payment Entry", strEmailMessage)
                                    End If
                                End If
                            End If
                        End If
                        End If
                        'reset last & orgi payment recid- finished with processing payment
                        intLastPaymentRecid = -1
                        intOrigPaymentRecid = -1
                Next
                'Delete PaymentsTemp records
                strSQL = "Delete from PaymentsTemp" & IIf(IsNothing(strWhereUser), "", " where Sys_Users_RECID = '" & strWhereUser & "'")
                g_IO_Execute_SQL(strSQL, False)
            End If
        ElseIf strAction = "getPatName" Then
            Dim strID As String = IIf(IsNothing(Request.Form("id")), Request.QueryString("id"), Request.Form("id"))
            Dim strFrm As String = IIf(IsNothing(Request.Form("frm")), Request.QueryString("frm"), Request.Form("frm"))
            Dim strCon As String = IIf(IsNothing(Request.Form("con")), Request.QueryString("con"), Request.Form("con"))
            'Boolean for add new contract when one contract already exists for the patient
            Dim blnAddNew As Boolean = IIf(IsNothing(Request.Form("addNew")), Request.QueryString("addNew"), Request.Form("addNew"))
            Dim strVal As String = strID.Substring(3)
            Dim strSql As String = ""
            If strFrm = "PaymentEntry" Then
                If strID.IndexOf("cht") = 0 Then
                    ' 11/29/2016 CS Added doctors_vw value to sql strings to send back to payment entry
                    'Searching on Chart #
                    strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey,' ') PatientKey, isnull(Account_Id,'') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], ' ') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [WSDC_Ortho].[dbo].[Contracts] c left outer join " &
                        " [WSDC_Ortho].[dbo].[IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where c.ChartNumber = '" & strVal & "' and ip.ChartNo = '" & strVal & "'"
                ElseIf strID.IndexOf("con") = 0 Then
                    'Searching on Contract #
                    strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey, ' ') PatientKey, isnull(Account_Id,'') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [WSDC_Ortho].[dbo].[Contracts] c left outer join " &
                        " [WSDC_Ortho].[dbo].[IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where c.recid = '" & strVal & "'"
                ElseIf strID.IndexOf("fst") = 0 Then
                    'Searching on First Name
                    strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey, ' ') PatientKey, isnull(Account_Id,'') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [WSDC_Ortho].[dbo].[Contracts] c left outer join " &
                        " [WSDC_Ortho].[dbo].[IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where ip.FirstName like '%" & strVal & "%'"
                ElseIf strID.IndexOf("lst") = 0 Then
                    'Searching on Last Name
                    strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey, ' ') PatientKey, isnull(Account_Id,'') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [WSDC_Ortho].[dbo].[Contracts] c left outer join " &
                        " [WSDC_Ortho].[dbo].[IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where ip.LastName like '%" & strVal & "%'"
                End If
                If IsNothing(strCon) Then
                Else
                    strVal = strCon
                    ' 11/29/2016 CS Added doctors_vw value to sql strings to send back to payment entry
                    'searching on contract#
                    strSql = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey, ' ') PatientKey, isnull(Account_Id, '') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [WSDC_Ortho].[dbo].[Contracts] c left outer join " &
                        " [WSDC_Ortho].[dbo].[IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where c.recid = '" & strVal & "'"
                End If

                'Was more than one patient found with First/Last Name search 
                Dim tblPatientChk As DataTable = g_IO_Execute_SQL(strSql, False)
                Dim strModalDDL As String = ""
                If tblPatientChk.Rows.Count > 1 Then
                    'More than one patient was found, prompt user to select correct patient
                    strModalDDL =
                                      "<h4>More than 1 patient was found for """ & strVal & """. Please select a patient.</h4>" & _
                                      "<div class=""form-group"">" & _
                                      "        <label for=""intContractID"" class="" col-sm-3 control-label"">Patient:</label>" & _
                                      "       <div class=""col-sm-9"">" & _
                                      "        <select id=""intContractID"" onchange=""getPatNameCID(this,'" & IIf(strID.IndexOf("lst") = 0, "lst", IIf(strID.IndexOf("fst") = 0, "fst", "cht")) & "');"" class=""form-control"" style=""width:150px"">" & _
                                      "         <option value = ""-1"">Choose one</option>#Options#" & _
                                      "        </select>" & _
                                      "       </div>" & _
                                      "</div>"
                    Dim strDDLOptions As String = ""
                    For Each row In tblPatientChk.Rows
                        strDDLOptions &= "<option value = """ & row("recid") & """>" & row("PatientName") & " - Contract #" & row("recid") & "</option>"
                    Next
                    strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
                    litMessage.Text = strModalDDL
                    Exit Sub
                ElseIf tblPatientChk.Rows.Count = 0 Then
                    'Need to pull patient data from Improvis only (no contract found)
                    If strID.IndexOf("cht") = 0 Then
                        'Searching on Chart #
                        strSql = "SELECT isnull(ip.PatientKey, ' ') PatientKey, isnull(ip.ChartNo, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, " & _
                            "FirstName, LastName " & _
                            "FROM IMPROVIS_PatientData_vw ip " & _
                            "WHERE ip.ChartNo = '" & strVal & "'"
                    ElseIf strID.IndexOf("fst") = 0 Then
                        'Searching on First Name
                        strSql = "SELECT isnull(ip.PatientKey, ' ') PatientKey, isnull(ip.ChartNo, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, " & _
                                "FirstName, LastName " & _
                                "FROM IMPROVIS_PatientData_vw ip " & _
                                "WHERE ip.FirstName like '%" & strVal & "%'"
                    ElseIf strID.IndexOf("lst") = 0 Then
                        'Searching on Last Name
                        strSql = "SELECT isnull(ip.PatientKey, ' ') PatientKey, isnull(ip.ChartNo, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], '') PatientName, " & _
                                "FirstName, LastName " & _
                                "FROM IMPROVIS_PatientData_vw ip " & _
                                "WHERE  ip.LastName like '%" & strVal & "%'"
                    End If

                    Dim tblPatientChkImprovis As DataTable = g_IO_Execute_SQL(strSql, False)
                    If tblPatientChkImprovis.Rows.Count > 1 Then
                        'More than one patient was found, prompt user to select correct patient
                        strModalDDL = "<h4>More than 1 patient was found for """ & strVal & """. Please select a patient.</h4>" & _
                                          "<div class=""form-group"">" & _
                                          "        <label for=""intChartNum"" class="" col-sm-3 control-label"">Patient:</label>" & _
                                          "       <div class=""col-sm-9"">" & _
                                          "        <select id=""intChartNum"" onchange=""getPatNameCID(this,'" & IIf(strID.IndexOf("lst") = 0, "lst", IIf(strID.IndexOf("fst") = 0, "fst", "cht")) & "');"" class=""form-control"" style=""width:150px"">" & _
                                          "         <option value = ""-1"">Choose one</option>#Options#" & _
                                          "        </select>" & _
                                          "       </div>" & _
                                          "</div>"
                        Dim strDDLOptions As String = ""
                        For Each row In tblPatientChkImprovis.Rows
                            strDDLOptions &= "<option value = """ & row("ChartNumber") & """>" & row("PatientName") & " - Chart #" & row("ChartNumber") & "</option>"
                        Next
                        strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
                        litMessage.Text = strModalDDL
                        Exit Sub
                    End If
                End If
            ElseIf strFrm = "Contracts" Then
                'We are coming from contract entry form that does not need to search for a specific contract
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
                        "FROM [WSDC_Ortho].[dbo].[Contracts_vw] where ChartNumber = '" & strVal & "'"
                        Dim tblConCheck As DataTable = g_IO_Execute_SQL(strSqlCheck, False)
                        If tblConCheck.Rows.Count > 0 Then
                            Dim strModalDDL As String = "<h4>&nbsp;A contract already exists for " & tblConCheck.Rows(0)("PatientName") & ".</h4>" &
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
                        "FROM [WSDC_Ortho].[dbo].[Contracts_vw] where Account_Id = '" & strVal & "'"
                        Dim tblConCheck As DataTable = g_IO_Execute_SQL(strSqlCheck, False)
                        If tblConCheck.Rows.Count > 0 Then
                            Dim strModalDDL As String = "<h4>&nbsp;A contract already exists for " & tblConCheck.Rows(0)("PatientName") & ".</h4>" &
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

            ElseIf strFrm = "Patients" Then
                'We are coming from patient entry form that does not need to search for a specific contract
                If strID.IndexOf("cht") = 0 Then
                    'Searching on Chart #
                    strSql = "SELECT RECID, isnull(ip.patient_id, ' ') PatientKey, isnull(ip.Chart_Number, ' ') ChartNumber,  isnull([Name_First] + ' ' + [Name_Last], '') PatientName, " & _
                        "FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB " & _
                        "FROM  IMPROVIS_PatientData_vw ip " & _
                        "where ip.Chart_Number = '" & strVal & "'"
                Else
                    'Searching on Patient #
                    strSql = "SELECT , isnull(ip.patient_id, ' ') PatientKey, isnull(ip.Chart_Number, ' ') ChartNumber,  isnull([Name_First] + ' ' + [Name_Last], '') PatientName, " & _
                        "FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB " & _
                        "FROM  IMPROVIS_PatientData_vw ip " & _
                        "where ip.patient_id = '" & strVal & "'"
                End If
                'We do need to see if their is an existing patient and prompt to verify add new patient
                Dim tblConCheck As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblConCheck.Rows.Count > 0 Then
                    'A patient already exists, prompt user
                    Dim strModalDDL As String =
                       "<h4>A patient record already exists for " & tblConCheck.Rows(0)("PatientName") & ".</h4>" & _
                       "    <div id=""intChtID"" class=""hidden"">" & tblConCheck.Rows(0)("ChartNumber") & "</div>" & _
                       "    <div id=""intPatID"" class=""hidden"">" & tblConCheck.Rows(0)("PatientKey") & "</div>" & _
                       "    <div id=""strName"" class=""hidden"">" & tblConCheck.Rows(0)("PatientName") & "</div>" & _
                       "    <div id=""intCID"" class=""hidden"">" & tblConCheck.Rows(0)("RECID") & "</div>"
                    litMessage.Text = strModalDDL
                    Exit Sub
                End If
            ElseIf strFrm = "PaymentReceipt" Then
                If strID.IndexOf("fst") = 0 Then
                    'Searching on First Name
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " & _
                        " FROM Patients_vw where name_first like '%" & strVal & "%'"
                ElseIf strID.IndexOf("lst") = 0 Then
                    'Searching on Last Name
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " & _
                        " FROM Patients_vw where name_last like '%" & strVal & "%'"
                ElseIf strID.IndexOf("cht") = 0 Then
                    'Searching on Last Name
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " & _
                        " FROM Patients_vw where Chart_Number = '" & strVal & "'"
                End If
                If IsNothing(strCon) Then
                Else
                    strVal = strCon
                    'searching on recid#
                    strSql = "SELECT recid, Chart_Number, PatientName, name_first as FirstName, name_last as LastName " & _
                        " FROM Patients_vw where recid = '" & strVal & "'"
                End If

                'Was more than one patient found with First/Last Name search 
                Dim tblPatientChk As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblPatientChk.Rows.Count > 1 Then
                    'More than one patient was found, prompt user to select correct patient
                    Dim strModalDDL As String =
                                      "<h4>More than 1 patient was found for """ & strVal & """. Please select a patient.</h4>" & _
                                      "<div class=""form-group"">" & _
                                      "        <label for=""intContractID"" class="" col-sm-3 control-label"">Patient:</label>" & _
                                      "       <div class=""col-sm-9"">" & _
                                      "        <select id=""intContractID"" onchange=""getPatNameCID(this,'" & IIf(strID.IndexOf("lst") = 0, "lst", "fst") & "');"" class=""form-control"" style=""width:150px"">" & _
                                      "         <option value = ""-1"">Choose one</option>#Options#" & _
                                      "        </select>" & _
                                      "       </div>" & _
                                      "</div>"
                    Dim strDDLOptions As String = ""
                    For Each row In tblPatientChk.Rows
                        strDDLOptions &= "<option value = """ & row("recid") & """>" & row("PatientName") & " - Chart #" & row("Chart_Number") & "</option>"
                    Next
                    strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
                    litMessage.Text = strModalDDL
                    Exit Sub
                End If
            End If

            'get Patient Info
            Dim tblPatientName As DataTable = g_IO_Execute_SQL(strSql, False)
            Dim blnPastDueInv As Boolean = False

            If tblPatientName.Rows.Count = 1 Then
                If tblPatientName.Rows(0)("PatientName") = "" Then
                    litMessage.Text = "Patient Name Not Found"
                Else
                    If strFrm = "PaymentEntry" Then
                        If strSql.Contains("Contracts") Then

                            'Pull Patient RemainingBalance
                            Dim intPatRemaining As Decimal = IIf(IsNothing(tblPatientName.Rows(0)("PatientRemainingBalance")), 0, CDec(tblPatientName.Rows(0)("PatientRemainingBalance")))

                            'Patient monthly payment
                            Dim decPatMonthlyPay As Decimal = IIf(IsNothing(tblPatientName.Rows(0)("PatientMonthlyPayment")), 0, CDec(tblPatientName.Rows(0)("PatientMonthlyPayment")))
                            'Next Invoice Amount
                            Dim decNextInvoice As Decimal = 0
                            If intPatRemaining < decPatMonthlyPay Then
                                decNextInvoice = intPatRemaining
                            Else
                                decNextInvoice = decPatMonthlyPay
                            End If
                            'Pull insurance info for Payment Entry page only & build data

                            Dim tblPrimInsurInfo As DataTable = g_IO_Execute_SQL("select ins_company_name from dbo.DropDownList__InsurancePlans where recid = '" & tblPatientName.Rows(0)("PrimaryInsurancePlans_vw") & "'", False)
                            Dim tblSecInsurInfo As DataTable = g_IO_Execute_SQL("select ins_company_name from dbo.DropDownList__InsurancePlans where recid = '" & tblPatientName.Rows(0)("SecondaryInsurancePlans_vw") & "'", False)

                            'pull Claim info 
                            Dim tblClaimInfo As DataTable = g_IO_Execute_SQL("select -1 as ddlIndex, '' as claimNumber", False)
                            'Build javascript data array for insurnace dropdown

                            'Building ddl for claims
                            strSql = "select * from openClaimsDDL_vw where contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'  order by DateProcessed desc "
                            Dim intCounter As Integer = 0
                            Dim tblClaims As DataTable = g_IO_Execute_SQL(strSql, False)

                            Dim strDDLValues As String = "-2"
                            Dim strDDLText As String = "Choose an option"
                            strDDLValues &= ",-1"
                            strDDLText &= ",Waiting for claim to be processed"

                            If intCountClaimsInvoice = 0 Then
                                strClaimsInvoiceTbl &= "<Table id=""tblClaimInvoiceData"" Class=""table""><thead> <tr> <th> Claim/Invoice</th> <th>#</th><th>Date</th> <th>Expected</th><th>Due</th><th>Amount " &
                                                    "</th><th>Insurance</th></th><th>Type</th><th>Descr</th></tr></thead> " &
                                        "<tbody>"
                                strClaimsInvoiceTbl &= "<tr><th scope = ""row"" ></th><td colspan=""3"">Apply To Principle</td>" &
                                                "<td>$" & Format(intPatRemaining, "0.00") & " Due</td> " &
                                                "<td colspan=""1""><span class=""input-group""><span class=""input-group-addon"">$</span><input id=""inputApplyToPrinciple""   class=""DB form-control"" onkeyup=""updateRemaining()"" type=""text"" value=""0""  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True""  runat=""server"" /></span></td>" &
                                                "<td></td><td></td><td></td></tr>"
                                strClaimsInvoiceTbl &= "<tr><th scope = ""row"" ></th><td colspan=""3"">Apply To Next Invoice</td>" &
                                                        "<td>$" & Format(decNextInvoice, "0.00") & " Due</td> " &
                                                           "<td colspan=""1""><span class=""input-group""><span class=""input-group-addon"">$</span><input id=""inputApplyToNextInvoice""   class=""DB form-control"" onkeyup=""updateRemaining()"" type=""text"" value=""0""  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True""  runat=""server"" /></span></td><td></td><td></td><td></td></tr>"
                                strClaimsInvoiceTbl &= "<tr><th scope = ""row"" ></th><td colspan=""3"">Waiting on claim to be processed </td>" &
                                    "<td>Not available</td> " &
                                                           "<td colspan=""1""><span class=""input-group""><span class=""input-group-addon"">$</span><input id=""inputWaitingOnClaim""  class=""form-control"" onkeyup=""updateRemaining()"" type=""text"" value=""0""  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True""  runat=""server"" /></span></td><td></td><td></td><td></td></tr>"
                            End If

                            For Each rowClaims In tblClaims.Rows
                                strDDLValues &= "," & intCounter
                                strDDLText &= "," & rowClaims("ClaimNumber") & " - " & rowClaims("procedure_date") & " - $" & rowClaims("claimAmount") & " Expected - $" & rowClaims("OpenAmount") & " Due - " & IIf(rowClaims("type") = "0", rowClaims("insurance_name"), rowClaims("other_policyholder_company"))

                                intCounter += 1
                                intCountClaimsInvoice += 1

                                strClaimsInvoiceTbl &= "<tr id=""rowclaim" & rowClaims("ClaimNumber") & """> <th scope=""row"">Claim</th>" &
                                        "<td>" & rowClaims("ClaimNumber") & "</td> " &
                                        "<td>" & rowClaims("procedure_date") & "</td> " &
                                        "<td>$" & Format(rowClaims("claimAmount"), "0.00") & "</td> " &
                                        "<td>$" & Format(rowClaims("OpenAmount"), "0.00") & " Due</td> " &
                                        "<td><span class=""input-group"">" &
                                            "<span class=""input-group-addon"">$</span>" &
                                            "<input ID=""claim" & rowClaims("ClaimNumber") & """ type=""text"" class=""form-control"" onkeyup=""updateRemaining();checkOverPaymentMaxInput(" & rowClaims("OpenAmount") & ", this.id)"" value=""0"" MaxLength=""10""  max=""" & rowClaims("OpenAmount") & """  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True"" runat=""server"" />" &
                                            "</span></td>" &
                                        "<td>" & rowClaims("insurance_name") & "</td> <td></td><td></td>" &
                                "</tr> "
                                '"<td><input id=""txt" & rowClaims("ClaimNumber") & """ type=""text"" runat=""server"" /></td> " &
                            Next

                            'Pull Current Invoice info (0-30 Days)
                            Dim strCurrentInvoiceSql As String = " Select * from Invoices where status = 'O' and recid >= (Select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "') and contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'  and PostDate  >= DATEADD(day, -30, GETDATE()) order by Bill_date desc "
                            ' used to be ---Dim strInvoiceSql As String = "Select * from PatientInvoiceAging_vw where Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'"
                            Dim tblCurrentInvoiceInfo As DataTable = g_IO_Execute_SQL(strCurrentInvoiceSql, False)

                            'Get Current Invoices
                            Dim intCurAmt As Decimal = 0
                            Dim intCurInvoice As Integer = -1
                            If tblCurrentInvoiceInfo.Rows.Count > 0 Then
                                'open invoices found
                                For Each row In tblCurrentInvoiceInfo.Rows
                                    intCurAmt = CDec(row("AmountDue")) - CDec(row("AmountPaid"))

                                    intCountClaimsInvoice += 1
                                    strClaimsInvoiceTbl &= "<tr id=""rowinvoice" & row("InvoiceNo") & """> <th scope=""row"">Invoice</th>" &
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



                            'Pull Past Due Invoice info (31+ days)
                            Dim strPastDueInvoiceSql As String = " select * from Invoices where status = 'O' and recid >= (select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "') and contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'  and PostDate  < DATEADD(day, -30, GETDATE()) order by Bill_date desc"
                            ' used to be ---Dim strInvoiceSql As String = "Select * from PatientInvoiceAging_vw where Contracts_recid = '" & tblPatientName.Rows(0)("recid") & "'"
                            Dim tblPastDueInvoiceInfo As DataTable = g_IO_Execute_SQL(strPastDueInvoiceSql, False)


                            'Get Past Due Invoices
                            Dim intPastDue As Decimal = 0
                            If tblPastDueInvoiceInfo.Rows.Count > 0 Then
                                'open past due invoices found
                                For Each row In tblPastDueInvoiceInfo.Rows
                                    'Past Due Invoices
                                    intPastDue = 0
                                    intPastDue += CDec(row("AmountDue")) - CDec(row("AmountPaid"))
                                    blnPastDueInv = True

                                    intCountClaimsInvoice += 1
                                    strClaimsInvoiceTbl &= "<tr  id=""rowinvoice" & row("InvoiceNo") & """> <th scope=""row"">Invoice</th>" &
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

                            If intCountClaimsInvoice = 0 Then
                                strClaimsInvoiceTbl &= "<tr> <th scope = ""row"" > No open claims/invoices</th></tr>"
                            End If

                            If intCountClaimsInvoice > 0 Then
                                strClaimsInvoiceTbl &= "</tbody></table>"
                            End If



                            litMessage.Text = tblPatientName.Rows(0)("PatientKey") & "~~" & _
                                tblPatientName.Rows(0)("PatientName") & IIf(IsNothing(tblPatientName.Rows(0)("Account_Id")), "", " - Contract #:" & tblPatientName.Rows(0)("Account_Id")) & "~~" & _
                                tblPatientName.Rows(0)("ChartNumber") & "~~" & _
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
                            ' 11/29/2016 CS Added doctors_vw value to data string to send back to payment entry
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
                                strClaimsInvoiceTbl

                        Else
                            'pull Claim info 
                            Dim tblClaimInfo As DataTable = g_IO_Execute_SQL("select -1 as ddlIndex, '' as claimNumber", False)
                            'Build javascript data array for insurnace dropdown

                            'Building ddl for claims ( pulling claim #, DateProcessed, & Insurance Name)
                            strSql = "select * from openClaimsDDL_vw where ChartNumber = '" & tblPatientName.Rows(0)("ChartNumber") & "'  order by DateProcessed desc "
                            Dim intCounter As Integer = 0
                            Dim tblClaims As DataTable = g_IO_Execute_SQL(strSql, False)

                            Dim strDDLValues As String = "-2"
                            Dim strDDLText As String = "Choose an option"
                            strDDLValues &= ",-1"
                            strDDLText &= ",Waiting for claim to be processed"

                            If intCountClaimsInvoice = 0 Then
                                strClaimsInvoiceTbl &= "<Table Class=""table""><thead> <tr> <th> Claim/Invoice</th> <th>#</th><th>Date</th> <th>Expected</th><th>Due</th><th>Amount " &
                                                    "</th><th>Insurance</th></tr></thead> " &
                                        "<tbody>"
                                strClaimsInvoiceTbl &= "<tr><th scope = ""row"" ></th><td colspan=""3"">Apply To Principle</td>" &
                                                "<td>Contract not yet generated</td> " &
                                                "<td colspan=""1""><span class=""input-group""><span class=""input-group-addon"">$</span><input id=""inputApplyToPrinciple""   class=""DB form-control"" onkeyup=""updateRemaining()"" type=""text"" value=""0""  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True""  runat=""server"" /></span></td>" &
                                                "<td></td><td></td><td></td></tr>"
                                strClaimsInvoiceTbl &= "<tr><th scope = ""row"" ></th><td colspan=""4"">Apply To Next Invoice</td>" &
                                                           "<td colspan=""2""><span Class=""input-group""><span Class=""input-group-addon"">$</span><input id=""inputApplyToNextInvoice""   Class=""DB form-control"" onkeyup=""updateRemaining()"" type=""text"" value=""0""  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True""  runat=""server"" /></span></td></tr>"
                                strClaimsInvoiceTbl &= "<tr><th scope = ""row"" ></th><td colspan=""4"">Waiting on claim to be processed </td>" &
                                                           "<td colspan=""2""><span class=""input-group""><span class=""input-group-addon"">$</span><input id=""inputWaitingOnClaim""  class=""form-control"" onkeyup=""updateRemaining()"" type=""text"" value=""0""  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True""  runat=""server"" /></span></td></tr>"
                            End If

                            For Each rowClaims In tblClaims.Rows
                                'bombed after rodney
                                'rowClaims("ddlIndex") = intCounter

                                strDDLValues &= "," & intCounter
                                strDDLText &= "," & rowClaims("ClaimNumber") & " - " & rowClaims("DateProcessed") & " - $" & rowClaims("claimAmount") & " Expected - $" & rowClaims("OpenAmount") & " Due - " & rowClaims("insurance_name")

                                intCounter += 1

                                intCountClaimsInvoice += 1
                                strClaimsInvoiceTbl &= "<tr  id=""rowclaim" & rowClaims("ClaimNumber") & """> <th scope=""row"">Claim</th>" &
                                        "<td>" & rowClaims("ClaimNumber") & "</td> " &
                                        "<td>" & rowClaims("DateProcessed") & "</td> " &
                                        "<td>$" & Format(rowClaims("claimAmount"), "0.00") & "</td> " &
                                        "<td>$" & Format(rowClaims("OpenAmount"), "0.00") & " Due</td> " &
                                        "<td><span class=""input-group"">" &
                                            "<span class=""input-group-addon"">$</span>" &
                                            "<input ID=""claim" & rowClaims("ClaimNumber") & """ type=""text"" class=""form-control"" onkeyup=""updateRemaining();checkOverPaymentMaxInput(" & rowClaims("OpenAmount") & ", this.id)"" value=""0"" MaxLength=""10""  max=""" & rowClaims("OpenAmount") & """  Style=""max-width: 130px;"" Display=""Dynamic"" EnableClientScript=""True"" runat=""server"" />" &
                                            "</span></td>" &
                                        "<td>" & rowClaims("insurance_name") & "</td><td></td><td></td>" &
                                "</tr> "
                            Next


                            'No contract data available, just pulling patient info from Improvis
                            ' 11/29/2016 CS Added doctors_vw value to sql strings to send back to payment entry
                            litMessage.Text = tblPatientName.Rows(0)("PatientKey") & "~~" & tblPatientName.Rows(0)("PatientName") & " - Contract #: N/A" & "~~" &
                            tblPatientName.Rows(0)("ChartNumber") & "~~" & "-1" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("LastName") & "~~" & strDDLValues & "~~" & strDDLText & "~~" & tblPatientName.Rows(0)("Doctors_vw") & "~~" & BuildJavascriptClaimsArray("-1", tblPatientName.Rows(0)("ChartNumber")) & "~~"

                            If intCountClaimsInvoice = 0 Then
                                strClaimsInvoiceTbl &= "<tr> <th scope = ""row"" > No open claims/invoices</th></tr>"
                            End If

                            If intCountClaimsInvoice > 0 Then
                                strClaimsInvoiceTbl &= "</tbody></table>"
                            End If

                        End If
                    ElseIf strFrm = "Contracts" Then
                        'Coming from Contracts page
                        litMessage.Text = tblPatientName.Rows(0)("Account_Id") & "~~" & tblPatientName.Rows(0)("PatientName") & "~~" & tblPatientName.Rows(0)("ChartNumber") &
                            "~~" & tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("MiddleName") & "~~" & tblPatientName.Rows(0)("LastName") &
                            "~~" & tblPatientName.Rows(0)("Address1") & "~~" & tblPatientName.Rows(0)("Address2") & "~~" & tblPatientName.Rows(0)("City") &
                            "~~" & tblPatientName.Rows(0)("State") & "~~" & tblPatientName.Rows(0)("ZipCode") & "~~" & tblPatientName.Rows(0)("Gender") &
                            "~~" & tblPatientName.Rows(0)("DOB") & "~~"
                        Dim test As String = ""
                    ElseIf strFrm = "Patients" Then
                        'Coming from Patients page
                        litMessage.Text = tblPatientName.Rows(0)("PatientKey") & "~~" & tblPatientName.Rows(0)("PatientName") & "~~" & tblPatientName.Rows(0)("ChartNumber") & _
                            "~~" & tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("MiddleName") & "~~" & tblPatientName.Rows(0)("LastName") & _
                            "~~" & tblPatientName.Rows(0)("Address1") & "~~" & tblPatientName.Rows(0)("Address2") & "~~" & tblPatientName.Rows(0)("City") & _
                            "~~" & tblPatientName.Rows(0)("State") & "~~" & tblPatientName.Rows(0)("ZipCode") & "~~" & tblPatientName.Rows(0)("Gender") & _
                            "~~" & tblPatientName.Rows(0)("DOB") & "~~"
                    ElseIf strFrm = "PaymentReceipt" Then
                        'Coming from Patients page
                        litMessage.Text = tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("LastName") & "~~" & tblPatientName.Rows(0)("Chart_Number")
                    End If

                End If
            ElseIf tblPatientName.Rows.Count < 1 Then
                If strFrm = "PaymentEntry" Then
                    'Payment Entry- No Contract found for Patient
                    litMessage.Text = "No patient found in Improvis."
                ElseIf strFrm = "Contracts" Then
                    'Contract Entry-Patient not found in Improvis
                    litMessage.Text = "No patient found in Improvis."
                End If
            ElseIf tblPatientName.Rows.Count > 1 Then
                Dim strModalDDL As String =
                                       "<h4>More than 1 contract was found for " & tblPatientName.Rows(0)("PatientName") & ". Please select a contract.</h4>" & _
                                       "<div class=""form-group"">" & _
                                       "        <label for=""intContractID"" class="" col-sm-3 control-label""> Contract #:</label>" & _
                                       "       <div class=""col-sm-9"">" & _
                                       "        <select id=""intContractID"" onchange=""getPatNameCID(this,'" & IIf(strID.IndexOf("cht") = 0, "cht", "pat") & "');"" class=""form-control"" style=""width:150px"">" & _
                                       "         <option value = ""-1"">Choose one</option>#Options#" & _
                                       "        </select>" & _
                                       "       </div>" & _
                                       "</div>"
                Dim strDDLOptions As String = ""
                For Each row In tblPatientName.Rows
                    strDDLOptions &= "<option value = """ & row("recid") & """>" & row("recid") & "</option>"
                Next
                strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
                litMessage.Text = strModalDDL
            End If
        ElseIf strAction = "pullPaymentInfo" Then
            Dim strID As String = IIf(IsNothing(Request.Form("id")), Request.QueryString("id"), Request.Form("id"))
            'searching on patient id, viewing or editing a payment

            'Get Payment Info
            Dim strSql As String = "select * from PaymentsTemp where recid = '" & strID & "'"
            Dim tblPaymentInfo As DataTable = g_IO_Execute_SQL(strSql, False)

            'get Patient Info
            'Searching on Chart #
            ' 11/29/2016 CS Added doctors_vw value to sql strings to send back to payment entry
            strSql = "SELECT c.recid, c.doctors_vw, isnull(ip.PatientKey,' ') PatientKey, isnull(Account_Id,'') Account_Id, isnull(c.ChartNumber, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], ' ') PatientName, FirstName ,LastName, PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, PatientMonthlyPayment, PatientRemainingBalance  FROM [WSDC_Ortho].[dbo].[Contracts] c left outer join " &
                " [WSDC_Ortho].[dbo].[IMPROVIS_PatientData_vw] ip on c.ChartNumber = ip.ChartNo where c.ChartNumber = '" & tblPaymentInfo.Rows(0)("ChartNumber") & "' and ip.ChartNo = '" & tblPaymentInfo.Rows(0)("ChartNumber") & "'"

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


                    litMessage.Text &= tblPatientName.Rows(0)("PatientKey") & "~~" & tblPatientName.Rows(0)("PatientName") & " - Contract #:" & tblPatientName.Rows(0)("Account_Id") & "~~" & _
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
                    litMessage.Text &= IIf(IsNothing(tblPatientName.Rows(0)("PatientMonthlyPayment")), "", intCurAmt.ToString("C2")) & "~~" & intCurAmt & "~~" & _
                        intPatRemaining.ToString("C2") & "~~" & intPatRemaining & "~~" & _
                        intPastDue.ToString("C2") & "~~" & intPastDue & "~~" & intCurInvoice & "~~" & blnPastDueInv & "~~" & blnViewMode & "~~"
                End If
            Else
                'This is a payment that has no contract 
                strSql = "SELECT isnull(ip.PatientKey,' ') PatientKey, isnull(ip.ChartNo, ' ') ChartNumber,  isnull([FirstName] + ' ' + [LastName], ' ') PatientName, FirstName ,LastName " & _
                " FROM [WSDC_Ortho].[dbo].[IMPROVIS_PatientData_vw] ip where ChartNo = '" & tblPaymentInfo.Rows(0)("ChartNumber") & "'"
                tblPatientName = g_IO_Execute_SQL(strSql, False)
                litMessage.Text &= tblPatientName.Rows(0)("PatientKey") & "~~" & tblPatientName.Rows(0)("PatientName") & " - Contract #: N/A" & "~~" & _
                    tblPatientName.Rows(0)("ChartNumber") & "~~" & "-1" & "~~" & " " & "~~" & "" & "" & "~~" & "" & "~~" & _
                    "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & "" & "~~" & False & "~~" & blnViewMode & "~~"
            End If

            'Only pulling payment info from payments queue (so we do not need to pull orig_payment since payment has not been processed yet)
            If tblPaymentInfo.Rows(0)("PaymentSelection") = "PatientAmount" Then
                litMessage.Text &= "PatientAmount" & "~~" & tblPaymentInfo.Rows(0)("PatientAmount") & _
                    "~~" & tblPaymentInfo.Rows(0)("ApplyToCurrentInvoice") & "~~" & tblPaymentInfo.Rows(0)("ApplyToPastDue") & _
                    "~~" & tblPaymentInfo.Rows(0)("ApplyToPrinciple") & "~~" & tblPaymentInfo.Rows(0)("ApplyToNextInvoice") & "~~" & tblPaymentInfo.Rows(0)("DatePosted") & "~~"
            ElseIf tblPaymentInfo.Rows(0)("PaymentSelection") = "PrimaryAmount" Then
                litMessage.Text &= "PrimaryAmount" & "~~" & tblPaymentInfo.Rows(0)("PrimaryAmount") & "~~" & "0" & "~~" & "0" & _
                    "~~" & "0" & "~~" & "0" & "~~" & tblPaymentInfo.Rows(0)("DatePosted") & "~~"
            ElseIf tblPaymentInfo.Rows(0)("PaymentSelection") = "SecondaryAmount" Then
                litMessage.Text &= "SecondaryAmount" & "~~" & tblPaymentInfo.Rows(0)("SecondaryAmount") & "~~" & "0" & "~~" & "0" & _
                    "~~" & "0" & "~~" & "0" & "~~" & tblPaymentInfo.Rows(0)("DatePosted") & "~~"
            End If
            ' 11/29/2016 CS Added doctors_vw value to end of data string going back to payment entry
            litMessage.Text &= tblPatientName.Rows(0)("FirstName") & "~~" & tblPatientName.Rows(0)("LastName") & "~~" & tblPaymentInfo.Rows(0)("PayerName") & "~~" & tblPaymentInfo.Rows(0)("ClaimNumber") & "~~" & tblPaymentInfo.Rows(0)("Comments") & "~~" & tblPaymentInfo.Rows(0)("PaymentReference") & "~~" & CInt(tblPaymentInfo.Rows(0)("ClaimIndex")) & "~~" & tblPaymentInfo.Rows(0)("Doctors_vw") & "~~"

        ElseIf strAction = "moveInsuranceToHistory" Then
            Dim strChart As String = IIf(IsNothing(Request.Form("id")), Request.QueryString("id"), Request.Form("id"))
            Dim strCoverage As String = IIf(IsNothing(Request.Form("covType")), Request.QueryString("covType"), Request.Form("covType"))

            g_moveInsuranceToHistory(strChart, strCoverage)

        ElseIf strAction = "getTransAmount" Then
            ' get transactioncode default amount
            Dim strTrCode As String = IIf(IsNothing(Request.QueryString("trcode")), Request.Form("trcode"), Request.QueryString("trcode"))
            Dim intTrAmount As Integer = 0
            Dim strSQL As String = "select amount_billable from DropDownList__TransactionCodes where recid = '" & strTrCode & "'"
            Dim tblTrCodes As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblTrCodes.Rows.Count > 0 Then
                intTrAmount = CInt(tblTrCodes.Rows(0)("amount_billable"))
            End If
            litMessage.Text = Math.Round(intTrAmount, 2)
        End If
    End Sub
End Class