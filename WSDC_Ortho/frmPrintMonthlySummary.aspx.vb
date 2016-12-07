Public Class frmPrintMonthlySummary
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
        Dim dteCloseDate As String = DateAdd("d", -1, dteBeginDate.Text)

        Dim strSQL As String = "Select ar_lastCloseDate, ar_previousBal, (0 - ISNULL" & _
            "((SELECT        SUM(ISNULL(orig_payment, 0)) AS Expr1 " & _
            "    FROM            dbo.Payments p" & _
            "	 LEFT OUTER JOIN dbo.DropDownList__PaymentType pt ON p.paymentType = pt.recid" & _
            "    WHERE        (p.DatePosted >= '" & dateFrom & "') AND (p.DatePosted <= '" & dateTo & " 23:59:59') " & _
            "	 AND (pt.adjustment = 1) AND (p.recid = p.baseRecid)), 0)) AS ttlAdjustments, (ISNULL" & _
            "((SELECT        SUM(ISNULL(AmountDue, 0)) AS Expr1" & _
            "    FROM            dbo.Invoices " & _
            "    WHERE        (PostDate >= '" & dateFrom & "') AND (PostDate <= '" & dateTo & " 23:59:59')), 0) + ISNULL " & _
            "((SELECT        SUM(ISNULL(procedure_amount, 0)) AS Expr1 " & _
            "    FROM            dbo.Claims " & _
            "    WHERE        (DateProcessed >= '" & dateFrom & "') AND (DateProcessed <= '" & dateTo & " 23:59:59')), 0)) AS invAmount, (0 - ISNULL" & _
            "((SELECT        SUM(ISNULL(p.orig_payment, 0)) AS Expr1 " & _
            "    FROM            dbo.Payments p" & _
            "	 LEFT OUTER JOIN dbo.DropDownList__PaymentType pt ON p.paymentType = pt.recid" & _
            "    WHERE        (p.DatePosted >= '" & dateFrom & "') AND (p.DatePosted <= '" & dateTo & " 23:59:59') " & _
            "    AND (pt.adjustment = 0) AND (p.recid = p.baseRecid)), 0)) AS payAmount " & _
            " FROM rptAR_MonthlySummary_vw where ar_lastCloseDate = '" & dteCloseDate & "'"

        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSQL, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            ' 6/7/16 CS update account balances for period end
            ' 11/11/16 CS Note: the payAmount and ttlAdjustments values are already set as negative values from SQL select above
            Dim intEndingBalance As Integer = tblReportData.Rows(0)("ar_previousBal") + tblReportData.Rows(0)("invAmount") + tblReportData.Rows(0)("payAmount") + tblReportData.Rows(0)("ttlAdjustments")
            Dim strUpdate As String = "UPDATE [dbo].[AR_PeriodClosings]                   " &
            "   SET [endingBalance] = " & intEndingBalance.ToString &
            "      ,[ttlCharges] = " & tblReportData.Rows(0)("invAmount") &
            "      ,[ttlPayments] = " & 0 - tblReportData.Rows(0)("payAmount") &
            "      ,[ttlAdjustments] = " & 0 - tblReportData.Rows(0)("ttlAdjustments") &
            "   WHERE closeDate = '" & dateTo & "'"
            g_IO_Execute_SQL(strUpdate, False)

            ' print report
            lblMessage.Text = ""
            ' send strSQL over in session variable 
            Session("rptMonthlySummary") = strSQL
            Session("rptParameters") = "Title||" & dteBeginDate.Text & " - " & dteEndDate.Text
            Response.Redirect("ReportViewer.aspx?rpt=rptMonthlySummary")
        Else
            lblMessage.Text = "No records found matching criteria entered...<br/>"
        End If
    End Sub
End Class