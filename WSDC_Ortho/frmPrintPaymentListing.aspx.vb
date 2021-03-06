﻿Public Class frmPrintPaymentListing
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim dateFrom As String = Format(CType(dteBeginDate.Text, Date), "yyyy-MM-dd")
        Dim dateTo As String = Format(CType(dteEndDate.Text, Date), "yyyy-MM-dd")
        Dim userID As String = ""
        Dim strWhereClause As String = " where DatePosted >= '" & dateFrom & "  00:00:00  ' and DatePosted <= '" & dateTo & " 23:59:59' "
        If txtChartNo.Text <> "" Then
            strWhereClause &= " and chartNumber = '" & txtChartNo.Text & "'"
        End If
        Dim strOrderBy As String = " order by patientname "

        Dim strSql As String = ""
        ' 12/29/16 CS update payments with doctor recid (will eventually be fixed in payment entry...)
        strSql = "update payments set doctors_vw = (select doctors_vw from contracts where recid = payments.contract_recid) where doctors_vw = -1 and dateposted >= '2016-12-01 00:00:00';"
        g_IO_Execute_SQL(strSql, False)

        strSql = "select * from MonthEndPaymentListing_vw " & strWhereClause & strOrderBy
        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSql, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            'strWhereClause = " where DatePosted >= '" & Format(CType(dateFrom, Date), "MM/dd/yyyy") & "' and DatePosted <= '" & Format(CType(dateTo, Date), "MM/dd/yyyy") & "' " & IIf(userID = "", "", " and user_id = '" & userID & "' ")
            lblMessage.Text = ""
            Session("rptPaymentListing") = strSql
            Session("rptParameters") = "Title||" & dteBeginDate.Text & " - " & dteEndDate.Text
            Response.Redirect("ReportViewer.aspx?rpt=rptPaymentListing&where=" & strWhereClause)
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If
    End Sub
End Class