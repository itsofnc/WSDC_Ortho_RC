Module ModAppCustoms

    ' This module will contain custom addtions to standard modules that are needed for your specific install
    Public Sub g_setAppConfigurationKeys()

        ' control settings to any web config settings for new install
        ' note this will only run if the new install setting in web config is set to true

        ' turn new install off so this only runs once
        g_changeAppSettings("newInstall", "false")
    End Sub

    Public Sub g_UserRoles_AppFields()
        ' Add any additional fields or record entries in the dropdownlist__userroles table
        'Dim strSQL As String = "INSERT INTO [DropDownList__UserRoles] ([DESCR]) VALUES('SalesRep'); "
        'strSQL &= "INSERT INTO [DropDownList__UserRoles] ([DESCR]) VALUES('Customer')"
        'g_IO_Execute_SQL(strSQL, False)
    End Sub

    Public Sub g_Sys_Users_AppFields()
        ' Add any additional fields that are needed in the Sys_Users table and view
        'Dim strSQL As String = "ALTER TABLE sys_users ADD salesRepRecId; " & _
        '        "ALTER TABLE [dbo].[sys_Users] ADD  CONSTRAINT [DF_sys_Users_salesRepRecId_access]  DEFAULT ((-1)) FOR [salesRepRecId]"
        'g_IO_Execute_SQL(strSQL, False)
        'strSQL = "ALTER TABLE sys_users ADD customerRecId; " & _
        '    "ALTER TABLE [dbo].[sys_Users] ADD  CONSTRAINT [DF_sys_Users_customerRecId_access]  DEFAULT ((-1)) FOR [customerRecId]"
        'g_IO_Execute_SQL(strSQL, False)
    End Sub

    Public Sub g_Sys_Users_AppFieldsDefaultRecord()
        ' Defafault any values in Sys_Users needed for the custom fields
    End Sub

    Public Function g_LoginRedirect(ByVal strWhereFrom As String) As String
        Dim strLoginRedirect As String = ""
        If UCase(strWhereFrom) = "LOGIN" Then
            strLoginRedirect = "Dashboard.aspx"
        Else
            strLoginRedirect = "Default.aspx"
            If UCase(System.Web.HttpContext.Current.Session("user_role")) = "ADMINISTRATOR" Then
                strLoginRedirect = "Dashboard.aspx"
            Else
                strLoginRedirect = "Dashboard.aspx"
            End If
        End If

        Return strLoginRedirect
    End Function

    Public Function g_ReadGuestUser()

        ' 2/2/16 T3 custom for your app, read your guest account
        Dim strSQL As String = "Select * From users_vw Where " & g_userIdField & " = 'Guest'"
        Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
        Return tblUser
    End Function

    Public Function g_SaveNewUser(ByRef strRecid As String) As String
        Dim strReturnString As String = ""
        If strRecid = -1 Then
            Dim strSql As String = " select " & g_userEmailField & " from sys_users where recid = '" & strRecid & "'"
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSql, False)
            Dim strEmail As String = ""
            strEmail = tblUser.Rows(0)(g_userEmailField)
            Dim strEmailMessage As String = ""

            Dim strMessage As String = ""
            strMessage = "<div class=""container""><p>Your registration has been received. <br /><br /> " &
                "Please check your email for further instructions or contact us.</p></div> <br /><br /> "
            If g_confirmNewUser Then
                System.Web.HttpContext.Current.Session("MessageTitle") = "Thank you for registering."
                System.Web.HttpContext.Current.Session("RelayMessage") = strMessage
                System.Web.HttpContext.Current.Session("PageRelay") = ""
                strReturnString = "REDIRECT||frmMessaging.aspx||"
                ' send email welcoming new user with instructions about account confirmation... 
                strEmailMessage = "Thank you for registering as a new user on " & g_SiteDisplayName & "." &
                     vbCrLf & vbCrLf &
                     "Your account is pending verification.  You will receive a confirmation email with further instructions. " &
                    vbCrLf & vbCrLf & "If you need addional help, contact " & g_SiteDisplayName & " support."
            Else
                ' send them to login page or see if user account has a default page setup...
                strReturnString = "REDIRECT||" & g_LoginRedirect("Login") & "||"
                ' send email welcoming to site and general info
                strEmailMessage = "Thank you for registering as a new user on " & g_SiteDisplayName & "." &
                    vbCrLf & vbCrLf & "If you need addional help, contact " & g_SiteDisplayName & " support."
            End If
            g_sendEmail(strEmail, g_SiteDisplayName & " Account Registration", strEmailMessage)
        End If
        Return strReturnString
    End Function

    Public Function g_getUserRoleOptions(ByRef intUserRole As Integer) As DataTable
        ' determine what user roles are visible during add/edit of users
        ' NOTE: need to send at least one entry back for this ddl!!

        Dim strSQL = "Select recid, descr From DropDownList__UserRoles"
        If intUserRole <> 1 Then
            strSQL &= " Where descr <> 'Administrator'"
        End If
        strSQL &= " Order By Descr"
        Dim tblUserRoles As DataTable = g_IO_Execute_SQL(strSQL, False)

        Return tblUserRoles

    End Function

    Public Function g_BuildNavigation() As String()

        ' build up navigation for this app
        Dim arrNavigation() As String = Nothing
        ReDim Preserve arrNavigation(1)
        arrNavigation(0) = "<ul class=""nav navbar-nav "">"
        ' check user's role for accounts maintenance
        Dim strPerm As String = "0000"
        If System.Web.HttpContext.Current.Session("AccountView") Then
            Mid(strPerm, 1, 1) = "1"
        End If
        If System.Web.HttpContext.Current.Session("AccountEdit") Then
            Mid(strPerm, 2, 1) = "1"
        End If
        If System.Web.HttpContext.Current.Session("AccountDelete") Then
            Mid(strPerm, 3, 1) = "1"
        End If
        If System.Web.HttpContext.Current.Session("AccountAdd") Then
            Mid(strPerm, 4, 1) = "1"
        End If


        If UCase(System.Web.HttpContext.Current.Session("user_role")) = "ADMINISTRATOR" Then
            'Admin Nav
            arrNavigation(0) &=
                            "<li id=""Contracts"" class=""active dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Contracts<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu scrollable-menu"" role=""menu"">" &
                            "        <li><a href=""frmListManager.aspx?id=Contracts_vw&perm=1101&mid=Contracts&add=ContractEntry.aspx"" onclick=""setListSession('contracts');"">Contract History</a></li>" &
                            "        <li><a href=""ContractEntry.aspx"" onclick=""setListSession('');"">Contract Entry</a></li>" &
                            "    </ul>" &
                            "</li>" &
                            "<li id=""#Payments"" class=""dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Payments<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu scrollable-menu"" role=""menu"">" &
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&vo=true&perm=1001&add=PaymentPosting.aspx"" onclick=""setListSession('');"">Payment History</a></li>" &
                            "        <li id=""#PaymentPosting""><a href=""PaymentPosting.aspx"" onclick=""setListSession('');"">Payment Entry</a></li>" &
                            "    </ul>" &
                            "</li>" &
                            "<li id=""#Reports"" class=""dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Reports<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu scrollable-menu"" role=""menu"">" &
                            "        <li id=""#ContractPostingReport""><a href=""frmPrintContractPosting.aspx"">Contract Posting Report</a></li>" &
                            "        <li id=""#ContractStatus""><a href=""ContractStatus.aspx"" onclick=""setListSession('');"">Contract Status View</a></li>" &
                            "        <li id=""#InsuranceTracking""><a href=""frmPrintInsuranceTrackingReport.aspx"" onclick=""setListSession('');"">Insurance Tracking Report</a></li>" &
                            "        <li id=""#OpenRecvRpt""><a href=""frmPrintOpenReceivables.aspx"" onclick=""setListSession('');"">Open Receivables by Patient Report</a></li>" &
                            "        <li id=""#OpenClaims""><a href=""frmListManager.aspx?id=rptInsuranceTrackingReport_vw&vo=true&perm=1000"">Open Insurance Claims Listing</a></li>" &
                            "        <li class=""divider""><a href=""#"">-------------</a></li>" &
                            "        <li id=""#PaymentReceipt""><a href=""frmPrintPaymentReceipts.aspx"" onclick=""setListSession('');"">Payment Receipt</a></li>" &
                            "        <li id=""#PaymentsRecord""><a href=""frmPrintPaymentsRecord.aspx"" onclick=""setListSession('');"">Payments Record</a></li>" &
                            "        <li id=""#PaymentSummary""><a href=""frmPaymentEntrySummary.aspx"" onclick=""setListSession('');"">Payment Summary</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&vo=true&perm=1001&add=PaymentPosting.aspx"">Posted Payments</a></li>" &
                            "        <li><a href=""frmPrintPaymentAllocationReport.aspx"">Payment Allocation Report</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=PatientPaymentsApplied_vw&vo=true&perm=1000"" onclick=""setListSession('');"">Patient Payments Applied</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=InsurancePaymentsApplied_vw&vo=true&perm=1000"" onclick=""setListSession('');"">Insurance Payments Applied</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=PaymentsUnposted_vw&vo=true&perm=1000"">Un-Posted Payments</a></li>" &
                            "        <li class=""divider""><a href=""#"">-------------</a></li>" &
                            "        <li id=""#UndistributedPayments""><a href=""frmPrintUndistributedPayments.aspx"" onclick=""setListSession('');"">Undistributed Payments Report</a></li>" &
                            "    </ul>" &
                            "</li>" &
                            "<li id=""#Processing"" class=""dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Claims/Invoices<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu scrollable-menu"" role=""menu"">" &
                            "        <li id=""#Invoices""><a href=""frmClaimsProcessing.aspx?i=1"" onclick=""setListSession('');"">Process Invoices</a></li>" &
                            "        <li class=""divider""></li>" &
                            "        <li id=""#Claims""><a href=""frmClaimsProcessing.aspx?c=1"" onclick=""setListSession('');"">Process Primary Claims</a></li>" &
                            "        <li id=""#Claims""><a href=""frmClaimsProcessing.aspx?c=2"" onclick=""setListSession('');"">Process Secondary Claims</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=SecondaryClaimsToBeProcessed_vw&vo=true&perm=1000"" onclick=""setListSession('');"">Secondary Insurance Claims Pending</a></li>" &
                            "       <li id=""#ClaimsReprint""><a href=""frmClaimsReprint.aspx"" onclick=""setListSession('');"">Reprint Claims</a></li>" &
                            "       <li id=""#InvoicesReprint""><a href=""frmInvoicesReprint.aspx"" onclick=""setListSession('');"">Reprint Invoices</a></li>" &
                            "    </ul>" &
                            "</li>" &
                            "<li id=""#MonthEnd"" class=""dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Month End Reports<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu scrollable-menu"" role=""menu"">" &
                            "        <li id=""#MonthlySummary""><a href=""frmPrintMonthlySummary.aspx"" onclick=""setListSession('');"">Group Summary</a></li>" &
                            "        <li id=""#ProviderSummary""><a href=""frmPrintProviderSummary.aspx"" onclick=""setListSession('');"">Provider Summary</a></li>" &
                            "        <li id=""#ClaimsListing""><a href=""frmPrintClaimsListing.aspx"" onclick=""setListSession('');"">Claims Listing</a></li>" &
                            "        <li id=""#InvoiceListing""><a href=""frmPrintInvoiceListing.aspx"" onclick=""setListSession('');"">Invoice Listing</a></li>" &
                            "        <li id=""#PaymentListing""><a href=""frmPrintPaymentListing.aspx"" onclick=""setListSession('');"">Payment Listing</a></li>" &
                            "    </ul>" &
                            "</li>" &
                            "<li id=""#Administration"" class=""dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Administration<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu scrollable-menu"" role=""menu"">" &
                            "       <li id=""#Users""><a href=""frmListManager.aspx?id=sys_users&perm=1101"" onclick=""setListSession('');"">Maintain Users</a></li>" &
                            "       <li id=""#MasterLists""><a href=""frmListManager.aspx?perm=1111"" onclick=""setListSession('');"">Master Lists</a></li>" &
                            "    </ul>" &
                            "</li>" &
                        "</ul>"
        Else
            'User Nav
            arrNavigation(0) &=
                            "<li id=""#Contracts"" class=""active dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Contracts<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu"" role=""menu"">" &
                            "        <li><a href=""frmListManager.aspx?id=Contracts_vw&perm=1000&mid=Contracts&add=ContractEntry.aspx"" onclick=""setListSession('contracts');"">Contract History</a></li>" &
                            "        <li class=""divider""></li>" &
                            "        <li id=""#ContractEntry"" class=""sr-only""><a href=""ContractEntry.aspx?vo=true"" onclick=""setListSession('');"">Contract Entry</a></li>" &
                            "    </ul>" &
                            "</li>" &
                            "<li id=""#Payments"" class=""dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Payments<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu"" role=""menu"">" &
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&vo=true&perm=1001&add=PaymentPosting.aspx"" onclick=""setListSession('');"">Payment History</a></li>" &
                            "        <li class=""divider""></li>" &
                            "        <li id=""#PaymentPosting""><a href=""PaymentPosting.aspx"" onclick=""setListSession('');"">Payment Entry</a></li>" &
                            "    </ul>" &
                            "</li>" &
                            "<li id=""#Reports"" class=""dropdown"">" &
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Reports<span class=""caret""></span></a>" &
                            "    <ul class=""dropdown-menu"" role=""menu"">" &
                            "        <li id=""#ContractPostingReport""><a href=""frmPrintContractPosting.aspx"">Contract Posting Report</a></li>" &
                            "        <li id=""#ContractStatus""><a href=""ContractStatus.aspx"" onclick=""setListSession('');"">Contract Status View</a></li>" &
                            "        <li id=""#InsuranceTracking""><a href=""frmPrintInsuranceTrackingReport.aspx"" onclick=""setListSession('');"">Insurance Tracking Report</a></li>" &
                            "        <li id=""#OpenRecvRpt""><a href=""frmPrintOpenReceivables.aspx"" onclick=""setListSession('');"">Open Receivables by Patient Report</a></li>" &
                            "        <li class=""divider""><a href=""#"">-------------</a></li>" &
                            "        <li id=""#PaymentReceipt""><a href=""frmPrintPaymentReceipts.aspx"" onclick=""setListSession('');"">Payment Receipt</a></li>" &
                            "        <li id=""#PaymentsRecord""><a href=""frmPrintPaymentsRecord.aspx"" onclick=""setListSession('');"">Payments Record</a></li>" &
                            "        <li id=""#PaymentSummary""><a href=""frmPaymentEntrySummary.aspx"" onclick=""setListSession('');"">Payment Summary</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&vo=true&perm=1001&add=PaymentPosting.aspx"">Posted Payments</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=PatientPaymentsApplied_vw&vo=true&perm=1000"" onclick=""setListSession('');"">Patient Payments Applied</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=InsurancePaymentsApplied_vw&vo=true&perm=1000"" onclick=""setListSession('');"">Insurance Payments Applied</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=PaymentsReversed_vw&vo=true&perm=1000"">Reversed Payments</a></li>" &
                            "        <li><a href=""frmListManager.aspx?id=PaymentsUnposted_vw&vo=true&perm=1000"">Un-Posted Payments</a></li>" &
                            "    </ul>" &
                            "</li>" &
                        "</ul>"
        End If
        arrNavigation(0) &= "</ul>"
        arrNavigation(1) = "href=""Dashboard.aspx"""

        Return arrNavigation
    End Function

    Public Sub g_setSiteUserSessions(ByVal rowUser As DataRow)

        ' Example of how this is used (from SDWebPortal project)

        ' 09/17/15 cpb called from frmLogin to set site specific users sessions for quick access
        '
        ' this site sessions--
        ' -- repRecId
        ' -- dealerRecId
        ' -- dealerPrimary
        ' -- repName
        ' -- dealerName
        ' -- userRoleRecId

        'System.Web.HttpContext.Current.Session("repRecId") = rowUser("repRecId")
        'System.Web.HttpContext.Current.Session("dealerRecId") = rowUser("dealerRecId")
        'System.Web.HttpContext.Current.Session("dealerPrimary") = rowUser("dealerPrimary")
        'System.Web.HttpContext.Current.Session("userRoleRecId") = rowUser("user_role")
        'If rowUser("repRecId") > -1 Then
        '    Dim strSQL As String = "Select repName From mstRep Where recid = '" & rowUser("repRecId") & "'"
        '    Dim tblRep As DataTable = g_IO_Execute_SQL(strSQL, False)
        '    System.Web.HttpContext.Current.Session("repName") = tblRep.Rows("0")("repName")
        'Else
        '    System.Web.HttpContext.Current.Session("repName") = "No Rep Assigned"
        'End If
        'If rowUser("dealerRecId") > -1 Then
        '    Dim strSQL As String = "Select dealerName From mstDealer Where recid = '" & rowUser("dealerRecId") & "'"
        '    Dim tblRep As DataTable = g_IO_Execute_SQL(strSQL, False)
        '    System.Web.HttpContext.Current.Session("dealerName") = tblRep.Rows("0")("dealerName")
        'Else
        '    System.Web.HttpContext.Current.Session("dealerName") = "No Dealer Assigned"
        'End If
    End Sub

    Public Sub g_requestLogin(ByRef strName As String, ByRef strEmail As String)

        Dim strMessage As String = "A new request for login has been received from the " & g_SiteDisplayName & " web site." & "<br />" & "<br />" &
            "The request was received from: " & strName & "<br />" &
            "  Email address is: " & strEmail & "<br />" & "<br /><br />" &
            "The following email was sent to " & strName & "<br /><br />"
        Dim strRequestMessage = "Hello " & strName & "," & "<br />" & "<br />" &
                "Thank you for your interest in " & g_SiteDisplayName & "." & "<br />" & "<br />" &
                "Your request for a login invitation has been sent to administration." & "<br />" & "<br />" &
                "Once the request has been reviewed by administration, your invitation with instructions will be sent. " & "<br /><br />" &
                "Thank you," & "<br /><br />" &
                g_SiteDisplayName
        strMessage &= strRequestMessage
        g_sendEmail(g_defaultEmail, "Request for Login to ASGA", strMessage, "", "emailTemplate.html")
        g_sendEmail(strEmail, "ASGA - Request for Invitation", strRequestMessage, "", "emailTemplate.html")

    End Sub
    '------------------------------------------------------------------------------------------------------------------------
    ' 01.02.17 cpb - the following routines were added for the new payment postings screen-----------------------------
    Public Function g_patientSearch(ByVal strChartNo As String, ByVal strFirstName As String, ByVal strLastName As String, ByVal strContract As String,
                                     ByRef strSQL As String, ByRef strLitMessage As String, ByRef strModalDDL As String) As DataTable

        ' 01/09/17 add contract date for specific contract selection
        strLitMessage = ""
        strSQL = "SELECT c.recid, c.Doctors_vw, isnull(ip.PatientKey,' ') PatientKey, " &
                "c.ContractDate, " &
                "isnull(Account_Id,'') Account_Id, " &
                "isnull(ip.ChartNo, ' ') ChartNumber,  " &
                "isnull([FirstName] + ' ' + [LastName], ' ') PatientName, " &
                "FirstName ,LastName, " &
                "PrimaryInsurancePlans_vw, SecondaryInsurancePlans_vw, " &
                "PatientMonthlyPayment, PatientRemainingBalance,  " &
                "PrimaryRemainingBalance, SecondaryRemainingBalance, " &
                "PrimaryInstallmentAmt, SecondaryInstallmentAmt " &
            "From [IMPROVIS_PatientData_vw] ip " &
                "left outer join " &
                "[Contracts] c on c.ChartNumber = ip.ChartNo "

        Dim strWhere As String = " Where "
        Dim strWhereDelim As String = ""
        If strContract = "" Then
            If Trim(strFirstName) = "" Then
            Else
                strWhere &= strWhereDelim & " ip.FirstName like '" & Trim(strFirstName) & "' "
                strWhereDelim = " and "
            End If
            If Trim(strLastName) = "" Then
            Else
                strWhere &= strWhereDelim & "ip.LastName like '" & Trim(strLastName) & "' "
                strWhereDelim = " and "
            End If
            If Trim(strChartNo) = "" Then
            Else
                strWhere &= strWhereDelim & "ip.ChartNo like '" & Trim(strChartNo) & "' "
                strWhereDelim = " and "
            End If
        Else
            'Searching on Contract #
            strWhere &= " c.recid = '" & strContract & "'"
        End If
        If strWhere = " Where " Then
        Else
            strSQL &= strWhere
        End If
        strSQL &= " Order By Account_Id desc, lastName, firstName "

        'Was more than one patient found with First/Last Name search 
        Dim tblPatientChk As DataTable = g_IO_Execute_SQL(strSQL, False)
        strModalDDL = ""
        Dim blnContractsFound As Boolean = True     ' 01.09.17 added to know if data from contracts or just improvis data
        If tblPatientChk.Rows.Count = 0 Then
            blnContractsFound = False
            'Need to pull patient data from Improvis only (no contract found)
            strSQL = "SELECT isnull(ip.PatientKey, ' ') PatientKey, " &
                        "isnull(ip.ChartNo, ' ') ChartNumber,  " &
                        "isnull([FirstName] + ' ' + [LastName], '') PatientName, " &
                        "FirstName, LastName, " &
                        "'-1' as PrimaryInsurancePlans_vw, '-1' as SecondaryInsurancePlans_vw, " &
                        "'0' as PatientMonthlyPayment, '0' as PatientRemainingBalance,  " &
                        "'0' as PrimaryRemainingBalance, '0' as SecondaryRemainingBalance, " &
                        "'0' as PrimaryInstallmentAmt, '0' as SecondaryInstallmentAmt," &
                        "'' as Account_Id " &
                    "FROM IMPROVIS_PatientData_vw ip "
            strWhere = " Where "
            strWhereDelim = ""
            If Trim(strFirstName) = "" Then
            Else
                strWhere &= strWhereDelim & " ip.FirstName like '" & Trim(strFirstName) & "' "
                strWhereDelim = " and "
            End If
            If Trim(strLastName) = "" Then
            Else
                strWhere &= strWhereDelim & "ip.LastName like '" & Trim(strLastName) & "' "
                strWhereDelim = " and "
            End If
            If Trim(strChartNo) = "" Then
            Else
                strWhere &= strWhereDelim & "ip.ChartNo like '" & Trim(strChartNo) & "' "
                strWhereDelim = " and "
            End If
            If strWhere = " Where " Then
            Else
                strSQL &= strWhere
            End If
            tblPatientChk = g_IO_Execute_SQL(strSQL, False)
        End If

        If tblPatientChk.Rows.Count > 1 Then
            'More than one patient was found, prompt user to select correct patient
            ' for backward compability--it is always expecting a single value.
            Dim strGetPatNameType As String = ""
            If strChartNo = "" Then
            Else
                strGetPatNameType = "cht"
            End If
            If strLastName = "" Then
            Else
                strGetPatNameType = "lst"
            End If
            If strFirstName = "" Then
            Else
                strGetPatNameType = "fst"
            End If
            '--
            strModalDDL = "<h4>More than 1 patient was found. Please select a patient.</h4>" &
                              "<div class=""form-group"">" &
                              "        <label for=""intChartNum"" class="" col-sm-3 control-label"">Patient:</label>" &
                              "       <div class=""col-sm-9"">" &
                              "        <select id=""intChartNum"" onchange=""getPatNameCID(this,'" & strGetPatNameType & "');"" class=""form-control"" style=""width:150px"">" &
                              "         <option value = ""-1"">Choose one</option>#Options#" &
                              "        </select>" &
                              "       </div>" &
                              "</div>"
            Dim strDDLOptions As String = ""
            Dim blnFirstNoAccountId As Boolean = True
            For Each patient In tblPatientChk.Rows
                'strDDLOptions &= "<option value = """ & row("ChartNumber") & """>" & row("PatientName") & " - Chart #" & row("ChartNumber") & "</option>"
                If patient("Account_Id") = "" And blnFirstNoAccountId Then
                    strDDLOptions &= "<option value = ""-1"">" & "-".PadRight(50, "-") & "</option>"
                    blnFirstNoAccountId = False
                End If
                strDDLOptions &= "<option value = """ & patient("ChartNumber") & """>" & patient("ChartNumber").PadLeft(6, "0") & " - " & patient("PatientName").PadRight(50, " ") &
                    IIf(patient("Account_Id") = "", "", " - " & patient("Account_Id")) &
                    " - " & patient("ContractDate") &
                    "</option>"
            Next
            strModalDDL = strModalDDL.Replace("#Options#", strDDLOptions)
            'strLitMessage = strModalDDL
        End If
        Return tblPatientChk
    End Function

    Public Function g_getPatientCurrentAmount(ByVal rowPatient As DataRow, ByRef strInvoicesTableCurrent As String) As Decimal
        ' 3/21/16 cpb - added routine for common access
        '   - fixed bug, ajaxOrtho routine assumes one record decCurInv += instead of =
        'Pull Current Invoice info (0-30 Days)
        Dim strSQL As String = " Select * from Invoices where status = 'O' and " &
            "recid >= (Select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & rowPatient("recid") & "') and " &
            "contracts_recid = '" & rowPatient("recid") & "'  and PostDate  >= DATEADD(day, -30, GETDATE()) order by Bill_date desc "
        Dim tblCurrentInvoiceInfo As DataTable = g_IO_Execute_SQL(strSQL, False)

        Dim decCurInv As Decimal = 0
        Dim decCurInvTtl As Decimal = 0
        Dim intCurInvoice As Integer = -1
        ' 01.11.17 cpb add for forward possibly adding invoice details later
        For Each row In tblCurrentInvoiceInfo.Rows
            decCurInv = CDec(row("AmountDue")) - CDec(row("AmountPaid"))
            decCurInvTtl += CDec(row("AmountDue")) - CDec(row("AmountPaid"))
            strInvoicesTableCurrent &=
                "<tr>" &
                    "<td style=""text-align:center"">Current</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceName" & row("InvoiceNo") & """>" & row("name") & "</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceNo" & row("InvoiceNo") & """>" & row("InvoiceNo") & "</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceDate" & row("InvoiceNo") & """>" & Format(row("PostDate"), "mm/DD/yyyy") & "</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceBal" & row("InvoiceNo") & """>" & FormatCurrency(decCurInv, 2).Replace("$", "").Replace(",", "") & "</td>" &
                    "<td></td>" &
                    "<td></td>" &
                "</tr>"
        Next

        Return decCurInvTtl

    End Function

    Public Function g_getPatientPastDueAmount(ByVal rowPatient As DataRow, ByRef strInvoicesTablePastDue As String) As Decimal
        ' 3/21/16 cpb - added routine for common access
        '   - fixed bug, ajaxOrtho routine steps on decPastDue so always only get 1 row, fix += applied
        'Pull Past Due Invoice info (31+ days)
        Dim stSQL As String = "Select * from Invoices where status = 'O' and " &
            "recid >= (select min(recid) from Invoices where status = 'o' and Contracts_recid = '" & rowPatient("recid") & "') and " &
            "contracts_recid = '" & rowPatient("recid") & "'  and " &
            "PostDate  < DATEADD(day, -30, GETDATE()) order by Bill_date desc"
        Dim tblPastDueInvoiceInfo As DataTable = g_IO_Execute_SQL(stSQL, False)

        Dim decPastDue As Decimal = 0
        Dim decPastDueTtl As Decimal = 0
        For Each row In tblPastDueInvoiceInfo.Rows
            decPastDue = CDec(row("AmountDue")) - CDec(row("AmountPaid"))
            decPastDueTtl += CDec(row("AmountDue")) - CDec(row("AmountPaid"))
            strInvoicesTablePastDue &=
                "<tr>" &
                    "<td style=""text-align:center"">Past Due</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceName" & row("InvoiceNo") & """>" & row("name") & "</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceNo" & row("InvoiceNo") & """>" & row("InvoiceNo") & "</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceDate" & row("InvoiceNo") & """>" & Format(row("PostDate"), "MM/dd/yyyy") & "</td>" &
                    "<td style=""text-align:center""ID=""txtInvoiceBal" & row("InvoiceNo") & """>" & FormatCurrency(decPastDue, 2).Replace("$", "").Replace(",", "") & "</td>" &
                    "<td></td>" &
                    "<td></td>" &
                "</tr>"
        Next

        Return decPastDueTtl

    End Function

    Public Function g_getPatientRemainingBalance(ByVal rowPatient As DataRow) As Decimal
        If IsDBNull(rowPatient("PatientRemainingBalance")) Then
            Return 0
        Else
            Return IIf(IsNothing(rowPatient("PatientRemainingBalance")), 0, CDec(rowPatient("PatientRemainingBalance")))
        End If
    End Function

    Public Function g_getPatientNextInvoice(ByRef rowPatient As DataRow) As Decimal
        Dim decNextInvoice As Decimal = 0
        Dim decPatRemaining As Decimal = 0
        Dim decPatMonthlyPay As Decimal = 0
        If IsDBNull(rowPatient("PatientRemainingBalance")) Then
        Else
            decPatRemaining = IIf(IsNothing(rowPatient("PatientRemainingBalance")), 0, CDec(rowPatient("PatientRemainingBalance")))
        End If
        If IsDBNull(rowPatient("PatientMonthlyPayment")) Then
        Else
            decPatMonthlyPay = IIf(IsNothing(rowPatient("PatientMonthlyPayment")), 0, CDec(rowPatient("PatientMonthlyPayment")))
        End If
        If decPatRemaining < decPatMonthlyPay Then
            decNextInvoice = decPatRemaining
        Else
            decNextInvoice = decPatMonthlyPay
        End If

        Return decNextInvoice
    End Function

    Public Sub g_getPatientInsurance(ByVal intPrimaryInsRecId As Integer, ByVal intSecondaryInsRecId As Integer,
                                     ByRef tblPrimInsurInfo As DataTable, ByRef tblSecInsurInfo As DataTable)
        Dim strSQL As String = "Select ins_company_name, plan_id from dbo.DropDownList__InsurancePlans where recid = '" & intPrimaryInsRecId & "'"
        tblPrimInsurInfo = g_IO_Execute_SQL(strSQL, False)
        strSQL = "select ins_company_name, plan_id from dbo.DropDownList__InsurancePlans where recid = '" & intSecondaryInsRecId & "'"
        tblSecInsurInfo = g_IO_Execute_SQL(strSQL, False)
    End Sub

    Public Function g_getPatientClaims(ByVal blnByContract As Boolean, ByVal rowPatient As DataRow, ByVal intPaymentTempRecId As Integer,
                                       ByRef strDDLValues As String, ByRef strDDLText As String,
                                       ByRef arrInsuranceTable() As String,
                                       ByRef intInsCount As Integer,
                                       ByRef strClaimsScript As String, ByRef strOtherInsList As String,
                                       ByRef strInsRefList As String) As DataTable
        ' builds data for insurnace payment for ddl and now show as grid

        ' 01.11.17 cpb break apart different insurnaces
        Dim strInsuranceTablePrimary As String = ""
        Dim strInsuranceTableSecondary As String = ""
        Dim strInsuranceTableOther As String = ""


        ' setup defaults - always avaialble even if no claims
        ' --for ddl
        strDDLValues = "-2"
        strDDLText = "Choose an option"
        strDDLValues &= ",-1"
        strDDLText &= ",Waiting for claim to be processed"
        ' -- for table
        intInsCount = 0

        ' get outstanding insurance claims and add to ddl & new table display
        Dim strSQL As String = ""
        Dim strSQLPending As String = ""
        Dim strSQLCurrentPayment As String = ""
        Dim strInsuranceRow As String = ""
        Dim strPrimaryPlanId As String = ""
        Dim strSecondaryPlanId As String = ""
        Dim tblPending As DataTable = Nothing
        Dim tblCurrentPayment As DataTable = Nothing
        Dim decCurrentPaymentDol As Decimal = 0.0
        Dim decPendingDol As Decimal = 0.0
        Dim decMax As Decimal = 0.0
        strInsRefList = ""
        Dim strInsRefListDelim As String = ""

        '--first get primary insurance claims
        If IsDBNull(rowPatient("PrimaryInsurancePlans_vw")) Then
        Else

            If rowPatient("PrimaryInsurancePlans_vw") > -1 Then
                strSQL = "Select plan_id, plan_name From DropDownList__InsurancePlans_vw Where recid = '" & rowPatient("PrimaryInsurancePlans_vw") & "'"
                Dim tblPrimaryInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblPrimaryInsurance.Rows.Count > 0 Then
                    If blnByContract Then
                        strSQL = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("recid") & "' and plan_id = '" & tblPrimaryInsurance.Rows("0")("plan_id") & "'" & " order by procedure_date desc "  'DateProcessed
                    Else
                        strSQL = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("ChartNumber") & "' and plan_id = '" & tblPrimaryInsurance.Rows("0")("plan_id") & "'" & " order by procedure_date desc "  'DateProcessed
                    End If
                    addClaimsToInsuranceTableByPlanId(strSQL, tblPrimaryInsurance.Rows("0")("plan_name"), blnByContract, rowPatient, intInsCount, strClaimsScript, strInsuranceTablePrimary, strDDLValues, strDDLText, strInsRefList, strInsRefListDelim, intPaymentTempRecId, "Primary")
                    strPrimaryPlanId = tblPrimaryInsurance.Rows("0")("plan_id")
                End If
                ' primary claim balance
                If rowPatient("PrimaryRemainingBalance") = 0 Then
                Else
                    ' look for pending payments for primary claim balance
                    decCurrentPaymentDol = 0.0
                    decPendingDol = 0.0
                    decMax = rowPatient("PrimaryRemainingBalance")
                    strSQLPending = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'PrimaryBalance'"
                    tblPending = g_IO_Execute_SQL(strSQLPending, False)
                    If tblPending.Rows.Count > 0 Then
                        If intPaymentTempRecId > -1 Then
                            strSQLCurrentPayment = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'PrimaryBalance' and paymentsTempRecId = '" & intPaymentTempRecId & "'"
                            tblCurrentPayment = g_IO_Execute_SQL(strSQLCurrentPayment, False)
                            If tblCurrentPayment.Rows.Count > 0 Then
                                decCurrentPaymentDol = tblCurrentPayment.Rows(0)("paymentAmount")
                            End If
                        End If
                        decPendingDol += tblPending.Rows("0")("paymentAmount") - decCurrentPaymentDol
                    End If
                    decMax -= decPendingDol
                    strInsuranceRow =
                    "<tr>" &
                        "<td style=""text-align:center""ID=""txtInsName" & intInsCount & """>" & tblPrimaryInsurance.Rows("0")("plan_name") & "(Primary)</td>" &
                        "<td colspan=""4""ID=""txtInsID" & intInsCount & """>Insurance Balance" & "</td>" &
                        "<td style=""text-align:right"" ID=""dolInsOpen" & intInsCount & """>" & FormatCurrency(rowPatient("PrimaryRemainingBalance"), 2) & "</td>"
                    If decPendingDol > 0 Then
                        strInsuranceRow &= "<td style=""text-align:right"" ID=""dolInsPrev" & intInsCount & """>" & FormatCurrency(decPendingDol, 2) & "</td>"
                    Else
                        strInsuranceRow &= "<td></td>"
                    End If
                    strInsuranceRow &=
                            "<td style=""text-align:right""  class=""pull-right"">" &
                                "<span class=""input-group"">" &
                                    "<span class=""input-group-addon""><a onclick=""applyInsAmt('dolInsPayment" & intInsCount & "', '" & FormatCurrency(decMax, 2).Replace("$", "").Replace(",", "") & "');"" title=""Apply Remaining Insurnace Balance"" style=""cursor:pointer;"" ><i class=""fa fa-calculator""></i></a></span>" &
                                    "<span class=""input-group-addon"">$</span>" &
                                    "<input ID=""dolInsPayment" & intInsCount & """ name=""dolInsPayment" & intInsCount & "" &
                                    " type=""text"" class=""DB form-control"" Style=""text-align:right; max-width:130px;"" onblur =""checkFieldPaymentAmount(this.id, this.value, " & FormatCurrency(rowPatient("PrimaryRemainingBalance"), 2).Replace("$", "").Replace(",", "") & " );calculateTotalPayment()"" runat=""server""></input>" &
                                "</span>" &
                            "</td" &
                        "</tr>"
                    strInsuranceTablePrimary &= strInsuranceRow
                    strClaimsScript &= "<script>jQuery('#dolInsPayment" & intInsCount & "').autoNumeric('init', {aSep: ',', aDec: '.'});</script>"
                    strInsRefList &= strInsRefListDelim & intInsCount & "~~" &
                        tblPrimaryInsurance.Rows("0")("plan_name") & "~~" &
                        "PrimaryBalance" & "~~" &
                        rowPatient("PrimaryRemainingBalance") & "~~" &
                        FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "") & "~~" &
                        FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "")
                    strInsRefListDelim = "||"
                    intInsCount += 1
                End If
                ' waiting on primary claim to be processed
                ' look for pending payments for primary claim waiting to be processed
                decCurrentPaymentDol = 0.0
                decPendingDol = 0.0
                decMax = rowPatient("PrimaryInstallmentAmt")
                strSQLPending = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'PrimaryWait'"
                tblPending = g_IO_Execute_SQL(strSQLPending, False)
                If tblPending.Rows.Count > 0 Then
                    If intPaymentTempRecId > -1 Then
                        strSQLCurrentPayment = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'PrimaryWait' and paymentsTempRecId = '" & intPaymentTempRecId & "'"
                        tblCurrentPayment = g_IO_Execute_SQL(strSQLCurrentPayment, False)
                        If tblCurrentPayment.Rows.Count > 0 Then
                            decCurrentPaymentDol = tblCurrentPayment.Rows(0)("paymentAmount")
                        End If
                    End If
                    decPendingDol += tblPending.Rows("0")("paymentAmount") - decCurrentPaymentDol
                End If
                decMax -= decPendingDol
                strInsuranceRow =
                "<tr>" &
                    "<td style=""text-align:center""ID=""txtInsName" & intInsCount & """>" & tblPrimaryInsurance.Rows("0")("plan_name") & "(Primary)</td>" &
                    "<td colspan=""4""ID=""txtInsID" & intInsCount & """>Waiting on Claim to be Proccessed" & "</td>" &
                    "<td style=""text-align:right"" ID=""dolClaimOpen" & intInsCount & """>" & FormatCurrency(rowPatient("PrimaryInstallmentAmt"), 2) & "</td>"
                If decPendingDol > 0 Then
                    strInsuranceRow &= "<td style=""text-align:right"" ID=""dolInsPrev" & intInsCount & """>" & FormatCurrency(decPendingDol, 2) & "</td>"
                Else
                    strInsuranceRow &= "<td></td>"
                End If
                strInsuranceRow &=
                    "<td style=""text-align:right""  class=""pull-right"">" &
                        "<span class=""input-group"">" &
                            "<span class=""input-group-addon""><a onclick=""applyInsAmt('dolInsPayment" & intInsCount & "', '" & FormatCurrency(decMax, 2).Replace("$", "").Replace(",", "") & "');"" title=""Apply Installment Amount"" style=""cursor:pointer;"" ><i class=""fa fa-calculator""></i></a></span>" &
                            "<span class=""input-group-addon"">$</span>" &
                            "<input ID=""dolInsPayment" & intInsCount & """ name=""dolInsPayment" & intInsCount & "" &
                            " type=""text"" class=""DB form-control"" Style=""text-align:right; max-width:130px;"" " &
                            "onblur =""checkFieldPaymentAmount(this.id, this.value, " & FormatCurrency(rowPatient("PrimaryInstallmentAmt"), 2).Replace("$", "").Replace(",", "") & " );calculateTotalPayment()"" runat=""server""></input>" &
                        "</span>" &
                    "</td" &
                "</tr>"
                strInsuranceTablePrimary &= strInsuranceRow
                strClaimsScript &= "<script>jQuery('#dolInsPayment" & intInsCount & "').autoNumeric('init', {aSep: ',', aDec: '.'});</script>"
                strInsRefList &= strInsRefListDelim & intInsCount & "~~" &
                    tblPrimaryInsurance.Rows("0")("plan_name") & "~~" &
                    "PrimaryWait" & "~~" &
                    rowPatient("PrimaryRemainingBalance") & "~~" &
                    FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "") & "~~" &
                    FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "")
                strInsRefListDelim = "||"
                intInsCount += 1
            End If
        End If
        '--next get secondary insurance claims
        If IsDBNull(rowPatient("SecondaryInsurancePlans_vw")) Then
        Else
            If rowPatient("SecondaryInsurancePlans_vw") > -1 Then
                strSQL = "Select plan_id, plan_name  From DropDownList__InsurancePlans_vw Where recid = '" & rowPatient("SecondaryInsurancePlans_vw") & "'"
                Dim tblSecondaryInsurnace As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblSecondaryInsurnace.Rows.Count > 0 Then
                    If blnByContract Then
                        strSQL = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("recid") & "' and plan_id = '" & tblSecondaryInsurnace.Rows("0")("plan_id") & "'" & " order by procedure_date desc "        'DateProcessed
                    Else
                        strSQL = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("ChartNumber") & "' and plan_id = '" & tblSecondaryInsurnace.Rows("0")("plan_id") & "'" & " order by procedure_date desc "  'DateProcessed
                    End If
                    addClaimsToInsuranceTableByPlanId(strSQL, tblSecondaryInsurnace.Rows("0")("plan_name"), blnByContract, rowPatient, intInsCount, strClaimsScript, strInsuranceTableSecondary, strDDLValues, strDDLText, strInsRefList, strInsRefListDelim, intPaymentTempRecId, "Secondary")
                    strSecondaryPlanId = tblSecondaryInsurnace.Rows("0")("plan_id")
                    If rowPatient("SecondaryRemainingBalance") = 0 Then
                    Else
                        ' look for pending payments for secondary claim balance
                        decCurrentPaymentDol = 0.0
                        decPendingDol = 0.0
                        decMax = rowPatient("SecondaryRemainingBalance")
                        strSQLPending = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'SecondaryBalance'"
                        tblPending = g_IO_Execute_SQL(strSQLPending, False)
                        If tblPending.Rows.Count > 0 Then
                            If intPaymentTempRecId > -1 Then
                                strSQLCurrentPayment = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'SecondaryBalance' and paymentsTempRecId = '" & intPaymentTempRecId & "'"
                                tblCurrentPayment = g_IO_Execute_SQL(strSQLCurrentPayment, False)
                                If tblCurrentPayment.Rows.Count > 0 Then
                                    decCurrentPaymentDol = tblCurrentPayment.Rows(0)("paymentAmount")
                                End If
                            End If
                            decPendingDol += tblPending.Rows("0")("paymentAmount") - decCurrentPaymentDol
                        End If
                        decMax -= decPendingDol
                        strInsuranceRow =
                            "<tr>" &
                                "<td style=""text-align:center""ID=""txtInsName" & intInsCount & """>" & tblSecondaryInsurnace.Rows("0")("plan_name") & "(Secondary)</td>" &
                                "<td colspan=""4""ID=""txtInsID" & intInsCount & """>Insurance Balance" & "</td>" &
                                "<td style=""text-align:right"" ID=""dolInsOpen" & intInsCount & """>" & FormatCurrency(rowPatient("SecondaryRemainingBalance"), 2) & "</td>"
                        If decPendingDol > 0 Then
                            strInsuranceRow &= "<td style=""text-align:right"" ID=""dolInsPrev" & intInsCount & """>" & FormatCurrency(decPendingDol, 2) & "</td>"
                        Else
                            strInsuranceRow &= "<td></td>"
                        End If
                        strInsuranceRow &=
                                "<td style=""text-align:right""  class=""pull-right"">" &
                                    "<span class=""input-group"">" &
                                        "<span class=""input-group-addon""><a onclick=""applyInsAmt('dolInsPayment" & intInsCount & "', '" & FormatCurrency(decMax, 2).Replace("$", "").Replace(",", "") & "');"" title=""Apply Remaining Insurnace Balance"" style=""cursor:pointer;"" ><i class=""fa fa-calculator""></i></a></span>" &
                                        "<span class=""input-group-addon"">$</span>" &
                                        "<input ID=""dolInsPayment" & intInsCount & """ name=""dolInsPayment" & intInsCount & "" &
                                        " type=""text"" class=""DB form-control"" Style=""text-align:right; max-width:130px;"" " &
                                        "onblur =""checkFieldPaymentAmount(this.id, this.value, " & FormatCurrency(rowPatient("SecondaryRemainingBalance"), 2).Replace("$", "").Replace(",", "") & " );calculateTotalPayment()"" runat=""server""></input>" &
                                    "</span>" &
                                "</td" &
                            "</tr>"
                        strInsuranceTableSecondary &= strInsuranceRow
                        strClaimsScript &= "<script>jQuery('#dolInsPayment" & intInsCount & "').autoNumeric('init', {aSep: ',', aDec: '.'});</script>"
                        strInsRefList &= strInsRefListDelim & intInsCount & "~~" &
                            tblSecondaryInsurnace.Rows("0")("plan_name") & "~~" &
                            "SecondaryBalance" & "~~" &
                            rowPatient("SecondaryRemainingBalance") & "~~" &
                            FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "") & "~~" &
                            FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "")
                        strInsRefListDelim = "||"
                        intInsCount += 1
                    End If
                    ' waiting on secondary claim to be processed
                    ' look for pending payments for secondary claim waiting
                    decCurrentPaymentDol = 0.0
                    decPendingDol = 0.0
                    decMax = rowPatient("SecondaryInstallmentAmt")
                    strSQLPending = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'SecondaryWait'"
                    tblPending = g_IO_Execute_SQL(strSQLPending, False)
                    If tblPending.Rows.Count > 0 Then
                        If intPaymentTempRecId > -1 Then
                            strSQLCurrentPayment = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = 'SecondaryWait' and paymentsTempRecId = '" & intPaymentTempRecId & "'"
                            tblCurrentPayment = g_IO_Execute_SQL(strSQLCurrentPayment, False)
                            If tblCurrentPayment.Rows.Count > 0 Then
                                decCurrentPaymentDol = tblCurrentPayment.Rows(0)("paymentAmount")
                            End If
                        End If
                        decPendingDol += tblPending.Rows("0")("paymentAmount") - decCurrentPaymentDol
                    End If
                    decMax -= decPendingDol
                    strInsuranceRow =
                    "<tr>" &
                        "<td style=""text-align:center""ID=""txtInsName" & intInsCount & """>" & tblSecondaryInsurnace.Rows("0")("plan_name") & "(Secondary)</td>" &
                        "<td colspan=""4""ID=""txtInsID" & intInsCount & """>Waiting on Claim to be Proccessed" & "</td>" &
                        "<td style=""text-align:right"" ID=""dolClaimOpen" & intInsCount & """>" & FormatCurrency(rowPatient("SecondaryInstallmentAmt"), 2) & "</td>"
                    If decPendingDol > 0 Then
                        strInsuranceRow &= "<td style=""text-align:right"" ID=""dolInsPrev" & intInsCount & """>" & FormatCurrency(decPendingDol, 2) & "</td>"
                    Else
                        strInsuranceRow &= "<td></td>"
                    End If
                    strInsuranceRow &=
                        "<td style=""text-align:right""  class=""pull-right"">" &
                            "<span class=""input-group"">" &
                                "<span class=""input-group-addon""><a onclick=""applyInsAmt('dolInsPayment" & intInsCount & "', '" & FormatCurrency(decMax, 2).Replace("$", "").Replace(",", "") & "');"" title=""Apply Installment Amount"" style=""cursor:pointer;"" ><i class=""fa fa-calculator""></i></a></span>" &
                                "<span class=""input-group-addon"">$</span>" &
                                "<input ID=""dolInsPayment" & intInsCount & """ name=""dolInsPayment" & intInsCount & "" &
                                " type=""text"" class=""DB form-control"" Style=""text-align:right; max-width:130px;"" " &
                                "onblur =""checkFieldPaymentAmount(this.id, this.value, " & FormatCurrency(rowPatient("SecondaryInstallmentAmt"), 2).Replace("$", "").Replace(",", "") & " );calculateTotalPayment()"" runat=""server""></input>" &
                            "</span>" &
                        "</td" &
                    "</tr>"
                    strInsuranceTableSecondary &= strInsuranceRow
                    strClaimsScript &= "<script>jQuery('#dolInsPayment" & intInsCount & "').autoNumeric('init', {aSep: ',', aDec: '.'});</script>"
                    strInsRefList &= strInsRefListDelim & intInsCount & "~~" &
                        tblSecondaryInsurnace.Rows("0")("plan_name") & "~~" &
                        "SecondaryWait" & "~~" & rowPatient("PrimaryRemainingBalance") & "~~" &
                        FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "") & "~~" &
                        FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "")
                    strInsRefListDelim = "||"
                    intInsCount += 1
                End If
            End If
        End If
        '--now add any rows not belonging to current insurances.
        If IsDBNull(rowPatient("recid")) Then
        Else
            Dim strSQLPlanId As String = ""
            If blnByContract Then
                strSQL = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("recid") & "' and plan_id != '" & strPrimaryPlanId & "' and plan_id != '" & strSecondaryPlanId & "'" & " order by plan_id, procedure_date desc "    'DateProcessed
                strSQLPlanId = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("recid")
            Else
                strSQL = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("ChartNumber") & "' and plan_id != '" & strPrimaryPlanId & "' and plan_id != '" & strSecondaryPlanId & "'" & " order by plan_id, procedure_date desc "  'DateProcessed
                strSQLPlanId = "select * from openClaimsDDL_vw where contracts_recid = '" & rowPatient("ChartNumber")
            End If
            Dim strPlanId As String = ""
            Dim strPlanName As String = ""
            Dim delimOtherInsList As String = ""
            Dim tblOtherClaims As DataTable = g_IO_Execute_SQL(strSQL, False)
            For Each claim As DataRow In tblOtherClaims.Rows
                If claim("plan_id") = strPlanId Then
                Else
                    strSQL = "Select plan_name From DropDownList__InsurancePlans_vw where plan_id='" & claim("plan_id") & "'"
                    Dim tblInsurancePlan As DataTable = g_IO_Execute_SQL(strSQL, False)
                    If tblInsurancePlan.Rows.Count > 0 Then
                        strPlanName = tblInsurancePlan.Rows("0")("plan_name")
                    Else
                        strPlanName = claim("plan_id")
                    End If
                    strSQL = strSQLPlanId & "' and plan_id = '" & claim("plan_id") & "' order by DateProcessed desc "
                    addClaimsToInsuranceTableByPlanId(strSQL, strPlanName, blnByContract, rowPatient, intInsCount, strClaimsScript, strInsuranceTableOther, strDDLValues, strDDLText, strInsRefList, strInsRefListDelim, intPaymentTempRecId, "Other")
                    strPlanId = claim("plan_id")
                    strOtherInsList &= delimOtherInsList & strPlanName
                    delimOtherInsList = "||"
                End If
            Next
        End If

        ' now add insurance strings into the returning array
        arrInsuranceTable(0) = strInsuranceTablePrimary
        arrInsuranceTable(1) = strInsuranceTableSecondary
        arrInsuranceTable(2) = strInsuranceTableOther

        ' build and return data tbl to link to ddl
        Dim intClaimIndex As Integer = 0
        strSQL = "Select -1 as ddlIndex, '' as claimNumber"
        Dim tblClaimNumber As DataTable = g_IO_Execute_SQL(strSQL, False)
        tblClaimNumber.Rows.RemoveAt(0)
        Dim arrValues() As String = Split(strDDLValues, ",")
        Dim arrText() As String = Split(strDDLText, ",")
        Dim intClaimsDDLCounter As Integer = 0
        For intClaimsDDLCounter = arrValues.Length - 1 To 1 Step -1
            Dim row As DataRow = tblClaimNumber.NewRow
            row("ddlIndex") = arrValues(intClaimsDDLCounter)
            row("claimNumber") = arrText(intClaimsDDLCounter)
            tblClaimNumber.Rows.InsertAt(row, 0)
        Next
        Return tblClaimNumber

    End Function

    Private Sub addClaimsToInsuranceTableByPlanId(ByVal strSQL As String, ByVal strPlanName As String, ByVal blnByContract As Boolean, ByVal rowPatient As DataRow,
                                                  ByRef intInsCount As Integer, ByRef strClaimsScript As String, ByRef strClaimsTable As String,
                                                  ByRef strDDLValues As String, ByRef strDDLText As String,
                                                  ByRef strInsRefList As String, ByRef strInsRefListDelim As String, ByRef intPaymentTempRecId As Integer,
                                                  ByRef strInsurnaceType As String)
        Dim tblClaims As DataTable
        tblClaims = g_IO_Execute_SQL(strSQL, False)
        Dim strInsuranceRow As String = ""
        strClaimsScript = ""
        Dim strSQLCurrentPayment As String = ""
        Dim tblCurrentPayment As DataTable
        For Each rowClaims In tblClaims.Rows
            ' add each claim to the ddl
            strDDLValues &= "," & intInsCount
            strDDLText &= "," & rowClaims("ClaimNumber") & " - " &
                                rowClaims("procedure_date") & " - $" &                   'rowClaims("DateProcessed")
                                FormatCurrency(rowClaims("claimAmount"), 2) & " Expected - $" &
                                FormatCurrency(rowClaims("OpenAmount"), 2) & " Due - " &
                                IIf(rowClaims("type") = "0", rowClaims("insurance_name"), rowClaims("other_policyholder_company"))
            ' look for pending payments for current claim
            Dim decMax As Decimal = rowClaims("OpenAmount")
            Dim decCurrentPaymentDol As Decimal = 0.0
            Dim decPendingDol As Decimal = 0.0
            Dim strSQLPending As String = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = '" & rowClaims("ClaimNumber") & "'"
            Dim tblPending As DataTable = g_IO_Execute_SQL(strSQLPending, False)
            For Each pendingPayment As DataRow In tblPending.Rows
                decPendingDol += pendingPayment("paymentAmount")
            Next
            If tblPending.Rows.Count > 0 Then
                If intPaymentTempRecId > -1 Then
                    strSQLCurrentPayment = "Select paymentAmount From PaymentsTempDetail Where chartNumber = '" & rowPatient("chartNumber") & "' and paymentId = '" & rowClaims("ClaimNumber") & "' and paymentsTempRecId = '" & intPaymentTempRecId & "'"
                    tblCurrentPayment = g_IO_Execute_SQL(strSQLCurrentPayment, False)
                    If tblCurrentPayment.Rows.Count > 0 Then
                        decCurrentPaymentDol = tblCurrentPayment.Rows(0)("paymentAmount")
                    End If
                End If
                decPendingDol -= decCurrentPaymentDol
            End If
            decMax -= decPendingDol
            strInsuranceRow =
            "<tr>" &
                "<td style=""text-align:center"" ID=""txtInsName" & intInsCount & """>" & strPlanName & "(" & strInsurnaceType & ")" & "</td>" &
                "<td style=""text-align:center"">" &
                    IIf(rowClaims("type") = "0", Trim(rowClaims("policyholder_name_first")) & " " & rowClaims("policyholder_name_last"), rowClaims("other_policyholder_name")) & "</td>" &
                "<td style=""text-align:center""ID=""txtInsID" & intInsCount & """>" & rowClaims("ClaimNumber") & "</td>" &
                "<td style=""text-align:center"">" & rowClaims("procedure_date") & "</td>" &     'rowClaims("DateProcessed")
                "<td style=""text-align:right"">" & FormatCurrency(rowClaims("claimAmount"), 2) & "</td>" &
                "<td style=""text-align:right"" ID=""dolInsOpen" & intInsCount & """>" & FormatCurrency(rowClaims("OpenAmount"), 2) & "</td>"
            If decPendingDol > 0 Then
                strInsuranceRow &= "<td style=""text-align:right"" ID=""dolInsPrev" & intInsCount & """>" & FormatCurrency(decPendingDol, 2) & "</td>"
            Else
                strInsuranceRow &= "<td></td>"
            End If
            If decMax > 0 Then
                strInsuranceRow &=
                "<td style=""text-align:right""  class=""pull-right"">" &
                        "<span class=""input-group"">" &
                            "<span class=""input-group-addon"">" &
                                "<a onclick=""applyInsAmt('dolInsPayment" & intInsCount & "', '" & FormatCurrency(decMax, 2).Replace("$", "").Replace(",", "") & "');" &
                                """ title=""Apply full claim amount"" style=""cursor:pointer;""><i class=""fa fa-calculator""></i></a></span>" &
                                "<span class=""input-group-addon"">$</span>" &
                                "<input ID=""dolInsPayment" & intInsCount & """ name=""dolInsPayment" & intInsCount & "" &
                                " type=""text"" class=""DB form-control"" Style=""text-align:right; max-width:130px;"" " &
                                "onblur =""checkFieldPaymentAmount(this.id, this.value, " & FormatCurrency(decMax, 2).Replace("$", "").Replace(",", "") & " );calculateTotalPayment()""></input>" &
                            "</span>" &
                        "</td"
            Else
                strInsuranceRow &= "<td></td>"
            End If
            strInsuranceRow &=
                "</tr>"

            strClaimsTable &= strInsuranceRow
            strClaimsScript &= "<script>jQuery('#dolInsPayment" & intInsCount & "').autoNumeric('init', {aSep: ',', aDec: '.'});</script>"
            If decCurrentPaymentDol = 0 Then
            Else
            End If
            strInsRefList &= strInsRefListDelim &
                intInsCount & "~~" & strPlanName & "~~" &
                rowClaims("ClaimNumber") & "~~" &
                rowClaims("OpenAmount") & "~~" &
                FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "") & "~~" &
                FormatCurrency(decCurrentPaymentDol, 2).Replace("$", "").Replace(",", "")
            strInsRefListDelim = "||"

            intInsCount += 1

        Next
    End Sub

    ' 01.02.17 cpb eom
    '------------------------------------------------------------------------------------------------------------------------

End Module
