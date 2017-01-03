Public Class frmPrintProviderSummary
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        lblMessage.Text = ""
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim dateFrom As String = Format(CType(dteBeginDate.Text, Date), "yyyy-MM-dd")
        Dim dateTo As String = Format(CType(dteEndDate.Text, Date), "yyyy-MM-dd")

        Dim dtBegYear As String = Format(CType(dteBeginDate.Text, Date), "yyyy") & "-01-01"
        ' use end of month instead of end of year...because running this 1st of following month
        'Dim dtEndYear As String = Format(CType(dteEndDate.Text, Date), "yyyy") & "-12-31"
        Dim dtEndYear As String = Format(CType(dteEndDate.Text, Date), "yyyy-MM-dd")
        ' *************************************************************************************

        Dim strSQL As String = ""
        ' 12/29/16 CS update payments with doctor recid (will eventually be fixed in payment entry...)
        strSQL = "update payments set doctors_vw = (select doctors_vw from contracts where recid = payments.contract_recid) where doctors_vw = -1 and dateposted >= '2016-12-01 00:00:00';"
        g_IO_Execute_SQL(strSQL, False)

        strSQL = "SELECT prv.doctor, (ISNULL" &
            "((SELECT        SUM(ISNULL(AmountDue, 0)) AS Expr1" &
            "    FROM            dbo.MonthEndInvoiceListing_vw i" &
            "    WHERE        (PostDate >= '" & dateFrom & " 00:00:00') AND (PostDate <= '" & dateTo & " 23:59:59') AND (i.doctor = prv.doctor)), 0) + ISNULL " &
            "((SELECT        SUM(ISNULL(procedure_amount, 0)) AS Expr1 " &
            "    FROM            dbo.MonthEndClaimListing_vw c" &
            "    WHERE        (DateProcessed >= '" & dateFrom & " 00:00:00') AND (DateProcessed <= '" & dateTo & " 23:59:59') AND (c.Physician = prv.doctor)), 0)) AS Charges,(0 - ISNULL" &
            "((SELECT        SUM(ISNULL(p.orig_payment, 0)) AS Expr1 " &
            "    FROM            dbo.MonthEndPaymentListing_vw p" &
            "    WHERE        (p.DatePosted >= '" & dateFrom & " 00:00:00') AND (p.DatePosted <= '" & dateTo & " 23:59:59') AND (p.adjustment = 0) AND (p.doctor = prv.doctor)), 0)) AS Payments, (0 - ISNULL" &
            "((SELECT        SUM(ISNULL(orig_payment, 0)) AS Expr1 " &
            "    FROM            dbo.MonthEndPaymentListing_vw p" &
            "    WHERE        (DatePosted >= '" & dateFrom & " 00:00:00') AND (DatePosted <= '" & dateTo & " 23:59:59') AND (p.adjustment = 1) AND (p.doctor = prv.doctor)), 0)) AS Adjustments, (ISNULL" &
            "((SELECT        SUM(ISNULL(AmountDue, 0)) AS Expr1" &
            "    FROM            dbo.MonthEndInvoiceListing_vw i" &
            "    WHERE        (PostDate >= '" & dtBegYear & " 00:00:00') AND (PostDate <= '" & dtEndYear & " 23:59:59') AND (i.doctor = prv.doctor)), 0) + ISNULL " &
            "((SELECT        SUM(ISNULL(procedure_amount, 0)) AS Expr1 " &
            "    FROM            dbo.MonthEndClaimListing_vw c" &
            "    WHERE        (DateProcessed >= '" & dtBegYear & " 00:00:00') AND (DateProcessed <= '" & dtEndYear & " 23:59:59') AND (c.Physician = prv.doctor)), 0)) AS ytdCharges, (0 - ISNULL" &
            "((SELECT        SUM(ISNULL(p.orig_payment, 0)) AS Expr1 " &
            "    FROM            dbo.MonthEndPaymentListing_vw p" &
            "    WHERE        (p.DatePosted >= '" & dtBegYear & " 00:00:00') AND (p.DatePosted <= '" & dtEndYear & " 23:59:59') AND (p.adjustment = 0) AND (p.doctor = prv.doctor)), 0)) AS ytdPayments, (0 - ISNULL" &
            "((SELECT        SUM(ISNULL(orig_payment, 0)) AS Expr1 " &
            "    FROM            dbo.MonthEndPaymentListing_vw p" &
            "    WHERE        (DatePosted >= '" & dtBegYear & " 00:00:00') AND (DatePosted <= '" & dtEndYear & " 23:59:59') AND (p.adjustment = 1) AND (p.doctor = prv.doctor)), 0)) AS ytdAdjustments " &
            " FROM rptProviderSummary_vw prv"

        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSQL, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            lblMessage.Text = ""
            ' send strSQL over in session variable 
            Session("rptProviderSummary") = strSQL
            Session("rptParameters") = "Title||" & dteBeginDate.Text & " - " & dteEndDate.Text
            Response.Redirect("ReportViewer.aspx?rpt=rptProviderSummary")
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If
    End Sub
End Class