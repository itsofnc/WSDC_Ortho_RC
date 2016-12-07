Public Class frmPrintPaymentReceipts
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        If IsPostBack Then
        Else
            lblMessage.Text = ""
        End If
        
    End Sub

    Private Sub btnPrintReceipt_Click(sender As Object, e As EventArgs) Handles btnPrintReceipt.Click
        Dim dateFrom As String = ""
        Dim dateTo As String = ""
        Try
            dateFrom = Format(CType(dteBeginDate.Text, Date), "yyyy-MM-dd")
        Catch ex As Exception
        End Try
        Try
            dateTo = Format(CType(dteEndDate.Text, Date), "yyyy-MM-dd")
        Catch ex As Exception
        End Try
        Dim strPayerName As String = txtPayerName.Text
        Dim strChartNum As String = txtChartNumber.Text
        Dim blnContinue As Boolean = False

        Dim strWhereClause As String = " where RECID = BaseRecid "
        If Trim(strPayerName) <> "" Then
            strWhereClause &= " and PayerName like '%" & strPayerName & "%' "
            blnContinue = True
        End If
        If dateFrom <> "" And dateTo <> "" Then
            strWhereClause &= " and DatePosted >= '" & dateFrom & " 00:00:00 ' and DatePosted <= '" & dateTo & "  23:59:59 '"
            blnContinue = True
        End If
        If strChartNum <> "" Then
            strWhereClause &= " and ChartNumber = '" & strChartNum & "'"
            blnContinue = True
        End If
        If blnContinue Then
            Dim strSql As String = "select * from rptPaymentReceipt_vw as Payments_vw " & strWhereClause
            Dim tblReportData As DataTable = g_IO_Execute_SQL(strSql, False)
            'dowload/print invoice
            If tblReportData.Rows.Count > 0 Then
                lblMessage.Text = ""
                Session("rptPaymentReceipt") = strSql & " order by ChartNumber"
                Response.Redirect("ReportViewer.aspx?rpt=rptPaymentReceipt")
            Else
                lblMessage.Text = "No records found matching criteria entered...<br />"
            End If
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If
        
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("Dashboard.aspx")
    End Sub
End Class