Public Class frmPrintUndistributedPayments
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim strSQL As String = "SELECT * FROM payments_vw WHERE Doctor = ''"
        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSQL, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            lblMessage.Text = ""
            ' send strSQL over in session variable 
            Session("rptUndistributedPayments") = strSQL
            Response.Redirect("ReportViewer.aspx?rpt=rptUndistributedPayments")
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If
    End Sub
End Class