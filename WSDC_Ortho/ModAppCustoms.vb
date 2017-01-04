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

End Module
