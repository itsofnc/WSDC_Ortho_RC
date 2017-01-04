Public Class frmPrintInvoiceListing
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim dateFrom As String = Format(CType(dteBeginDate.Text, Date), "yyyy-MM-dd")
        Dim dateTo As String = Format(CType(dteEndDate.Text, Date), "yyyy-MM-dd")
        Dim userID As String = ""
        ' 1/4/17 CS Need to pull invoices based on 'DateProcessed', b/c 'PostDate' (ie invoice date) can now be backdated on invoices by user
        Dim strWhereClause As String = " where DateProcessed >= '" & dateFrom & "  00:00:00  ' and DateProcessed <= '" & dateTo & " 23:59:59' "
        Dim strOrderBy As String = " order by patientname "
        Dim strSql As String = "select * from MonthEndInvoiceListing_vw " & strWhereClause & strOrderBy
        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSql, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            lblMessage.Text = ""
            Session("rptInvoiceListing") = strSql
            Session("rptParameters") = "Title||" & dteBeginDate.Text & " - " & dteEndDate.Text
            Response.Redirect("ReportViewer.aspx?rpt=rptInvoiceListing&where=" & strWhereClause)
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If
    End Sub
End Class