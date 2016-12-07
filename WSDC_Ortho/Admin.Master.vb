Public Class Admin
    Inherits System.Web.UI.MasterPage
    '***********************************Last Edit**************************************************

    'Last Edit Date: 09/29/14 
    '  Last Edit By: cpb/rlo/cfs/cbp                        
    'Last Edit Proj: WSDC_Ortho
    '-----------------------------------------------------
    'Change Log: 
    '12/02/14 - cp - added Master List Menu Option
    '09/29/14 - t3- Added litScripts for getFieldType 
    '8/26/14 - t3
    '-AdminMaster.aspx- added focus listener to master page to resolve back button issue after logout 
    'still need to format div html for session timeout
    '
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("~/Default.aspx")
            Exit Sub
        Else
            ' get user's name to display here...
            Dim strUserName As String = "Administrator"
            Dim strSQL As String = "select * from sys_users where recid = " & Session("user_link_id")
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblUser.Rows.Count > 0 Then
                strUserName = tblUser.Rows(0)("user_id")
            End If
            litHeaderGreeting.Text = "<p class=""navbar-text navbar-right""><span style=""color:white"">Welcome, " & strUserName & "&nbsp;&nbsp;</span><input id=""btnLogout"" onclick=""logOut();"" class=""btn btn-sm btn-warning"" value=""Logout"" type=""button""/></p>"
            '<input id=""btnLogout"" class=""btn btn-sm btn-warning"" value=""Logout"" type=""button""/>
        End If

        If Session("user_type") = "1" Then
            'Admin Nav
            litNavigation.Text = "<ul class=""nav navbar-nav"">" & _
                            "<li id=""Contracts"" class=""active dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Contracts<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "        <li><a href=""frmListManager.aspx?id=Contracts_vw&mid=Contracts&add=ContractEntry.aspx"" onclick=""setListSession('contracts');"">Contract History</a></li>" & _
                            "        <li><a href=""ContractEntry.aspx"" onclick=""setListSession('');"">Contract Entry</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                            "<li id=""#Payments"" class=""dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Payments<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPosted_vw&vo=true"" onclick=""setListSession('');"">Payment History</a></li>" & _
                            "        <li id=""#PaymentPosting""><a href=""PaymentPosting.aspx"" onclick=""setListSession('');"">Payment Entry</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                            "<li id=""#Reports"" class=""dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Reports<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "        <li id=""#ContractPostingReport""><a href=""frmPrintContractPosting.aspx"">Contract Posting Report</a></li>" & _
                            "        <li id=""#ContractStatus""><a href=""ContractStatus.aspx"" onclick=""setListSession('');"">Contract Status View</a></li>" & _
                            "        <li id=""#InsuranceTracking""><a href=""frmPrintInsuranceTrackingReport.aspx"" onclick=""setListSession('');"">Insurance Tracking Report</a></li>" & _
                            "        <li id=""#OpenRecvRpt""><a href=""frmPrintOpenReceivables.aspx"" onclick=""setListSession('');"">Open Receivables by Patient Report</a></li>" & _
                            "        <li class=""divider""><a href=""#"">-------------</a></li>" & _
                            "        <li id=""#PaymentReceipt""><a href=""frmPrintPaymentReceipts.aspx"" onclick=""setListSession('');"">Payment Receipt</a></li>" & _
                            "        <li id=""#PaymentsRecord""><a href=""frmPrintPaymentsRecord.aspx"" onclick=""setListSession('');"">Payments Record</a></li>" & _
                            "        <li id=""#PaymentSummary""><a href=""frmPaymentEntrySummary.aspx"" onclick=""setListSession('');"">Payment Summary</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPosted_vw&add=PaymentPosting.aspx&vo=true"">Posted Payments</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PatientPaymentsApplied_vw&vo=true"" onclick=""setListSession('');"">Patient Payments Applied</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=InsurancePaymentsApplied_vw&vo=true"" onclick=""setListSession('');"">Insurance Payments Applied</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsReversed_vw&vo=true"">Reversed Payments</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsUnposted_vw&vo=true"">Un-Posted Payments</a></li>" & _
                            "        <li class=""divider""><a href=""#"">-------------</a></li>" & _
                            "        <li id=""#DailyTotals""><a href=""frmPrintDailySummary.aspx"" onclick=""setListSession('');"">Summary of Daily Totals</a></li>" & _
                            "        <li id=""#MonthlySummary""><a href=""frmPrintMonthlySummary.aspx"" onclick=""setListSession('');"">Monthly Group Summary</a></li>" & _
                            "        <li id=""#ProductionStats""><a href=""frmPrintProductionStats.aspx"" onclick=""setListSession('');"">Production Statistics Report</a></li>" & _
                            "        <li id=""#UndistributedPayments""><a href=""frmPrintUndistributedPayments.aspx"" onclick=""setListSession('');"">Undistributed Payments Report</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                            "<li id=""#Processing"" class=""dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Claims/Invoices<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "        <li id=""#Invoices""><a href=""frmClaimsProcessing.aspx?i=1"" onclick=""setListSession('');"">Process Invoices</a></li>" & _
                            "        <li class=""divider""></li>" & _
                            "        <li id=""#Claims""><a href=""frmClaimsProcessing.aspx?c=1"" onclick=""setListSession('');"">Process Primary Claims</a></li>" & _
                            "        <li id=""#Claims""><a href=""frmClaimsProcessing.aspx?c=2"" onclick=""setListSession('');"">Process Secondary Claims</a></li>" & _
                            "       <li id=""#ClaimsReprint""><a href=""frmClaimsReprint.aspx"" onclick=""setListSession('');"">Reprint Claims</a></li>" & _
                            "       <li id=""#InvoicesReprint""><a href=""frmInvoicesReprint.aspx"" onclick=""setListSession('');"">Reprint Invoices</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                            "<li id=""#Administration"" class=""dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Administration<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "       <li id=""#MasterLists""><a href=""frmListManager.aspx"" onclick=""setListSession('');"">Master Lists</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                        "</ul>"
        Else
            'User Nav
            litNavigation.Text = "<ul class=""nav navbar-nav"">" & _
                            "<li id=""#Contracts"" class=""active dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Contracts<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "        <li><a href=""frmListManager.aspx?id=Contracts_vw&vo=true"" onclick=""setListSession('contracts');"">Contract History</a></li>" & _
                            "        <li class=""divider""></li>" & _
                            "        <li id=""#ContractEntry"" class=""sr-only""><a href=""ContractEntry.aspx?vo=true"" onclick=""setListSession('');"">Contract Entry</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                            "<li id=""#Payments"" class=""dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Payments<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPosted_vw&vo=true"" onclick=""setListSession('');"">Payment History</a></li>" & _
                            "        <li class=""divider""></li>" & _
                            "        <li id=""#PaymentPosting""><a href=""PaymentPosting.aspx"" onclick=""setListSession('');"">Payment Entry</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                            "<li id=""#Reports"" class=""dropdown"">" & _
                            "    <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-expanded=""false"">Reports<span class=""caret""></span></a>" & _
                            "    <ul class=""dropdown-menu"" role=""menu"">" & _
                            "        <li id=""#ContractPostingReport""><a href=""frmPrintContractPosting.aspx"">Contract Posting Report</a></li>" & _
                            "        <li id=""#ContractStatus""><a href=""ContractStatus.aspx"" onclick=""setListSession('');"">Contract Status View</a></li>" & _
                            "        <li id=""#InsuranceTracking""><a href=""frmPrintInsuranceTrackingReport.aspx"" onclick=""setListSession('');"">Insurance Tracking Report</a></li>" & _
                            "        <li id=""#OpenRecvRpt""><a href=""frmPrintOpenReceivables.aspx"" onclick=""setListSession('');"">Open Receivables by Patient Report</a></li>" & _
                            "        <li class=""divider""><a href=""#"">-------------</a></li>" & _
                            "        <li id=""#PaymentReceipt""><a href=""frmPrintPaymentReceipts.aspx"" onclick=""setListSession('');"">Payment Receipt</a></li>" & _
                            "        <li id=""#PaymentsRecord""><a href=""frmPrintPaymentsRecord.aspx"" onclick=""setListSession('');"">Payments Record</a></li>" & _
                            "        <li id=""#PaymentSummary""><a href=""frmPaymentEntrySummary.aspx"" onclick=""setListSession('');"">Payment Summary</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsPosted_vw&vo=true"">Posted Payments</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PatientPaymentsApplied_vw&vo=true"" onclick=""setListSession('');"">Patient Payments Applied</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=InsurancePaymentsApplied_vw&vo=true"" onclick=""setListSession('');"">Insurance Payments Applied</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsReversed_vw&vo=true"">Reversed Payments</a></li>" & _
                            "        <li><a href=""frmListManager.aspx?id=PaymentsUnposted_vw&vo=true"">Un-Posted Payments</a></li>" & _
                            "    </ul>" & _
                            "</li>" & _
                        "</ul>"
        End If
        litScripts.Text = "<script type=""text/javascript"" language=""javascript"">var g_FormObjectTypes=""" & g_strFormObjectTypes & """;</script>"

    End Sub

   
End Class