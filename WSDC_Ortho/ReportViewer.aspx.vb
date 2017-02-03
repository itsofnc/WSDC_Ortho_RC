Public Class ReportViewer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Static Generator As System.Random = New System.Random()
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim strPDFId As String = Generator.Next(0, Format(Date.Now, "HHmmss"))
        Dim strSql As String = ""
        Dim blnAddWhere = False
        'cp - need to just send where clause instead of individual parameters
        Dim strWhereClause As String = IIf(IsNothing(Request.QueryString("where")), "", Request.QueryString("where"))
        
        Dim strRptSelection As String = Request.QueryString("rpt")
        Dim tblReportData As DataTable = Nothing
        Select Case strRptSelection
            Case "rptPaymentEntrySummary"
                rptReport = New rptPaymentEntrySummary
                strSql = "select * from rptPaymentEntrySummary_vw " & strWhereClause & " order by employee"
                tblReportData = g_IO_Execute_SQL(strSql, False)
                If tblReportData.Rows.Count > 0 Then
                    rptReport.SetDataSource(tblReportData)
                End If
            Case "rptPaymentEntryDetail"
                rptReport = New rptPaymentEntryDetail
                strSql = "select * from rptPaymentEntrySummary_vw " & strWhereClause & " order by employee"
                tblReportData = g_IO_Execute_SQL(strSql, False)
                If tblReportData.Rows.Count > 0 Then
                    rptReport.SetDataSource(tblReportData)
                End If
            Case "rptPaymentReceipt"
                If IsNothing(Session("rptPaymentReceipt")) Then
                    ' get out...cannot run report without sql string pre-built
                    Exit Sub
                Else
                    rptReport = New rptPaymentReceipt
                    strSql = Session("rptPaymentReceipt")
                    tblReportData = g_IO_Execute_SQL(strSql, False)
                    If tblReportData.Rows.Count > 0 Then
                        rptReport.SetDataSource(tblReportData)
                    End If
                End If
            Case "rptMonthlySummary"
                ' 2/28/15 CS Sending SQL string in session variable (built in frmPrintMonthlySummary)
                If IsNothing(Session("rptMonthlySummary")) Then
                    ' get out...cannot run report without sql string pre-built
                    Exit Sub
                Else
                    rptReport = New rptMonthlySummary
                    strSql = Session("rptMonthlySummary")
                    tblReportData = g_IO_Execute_SQL(strSql, False)
                    If tblReportData.Rows.Count > 0 Then
                        rptReport.SetDataSource(tblReportData)
                    End If
                End If
            Case "rptProviderSummary"
                If IsNothing(Session("rptProviderSummary")) Then
                    ' get out...cannot run report without sql string pre-built
                    Exit Sub
                Else
                    rptReport = New rptProviderSummary
                    strSql = Session("rptProviderSummary")
                    tblReportData = g_IO_Execute_SQL(strSql, False)
                    If tblReportData.Rows.Count > 0 Then
                        rptReport.SetDataSource(tblReportData)
                    End If
                End If
            Case "rptOpenReceivablesByPatient"
                If IsNothing(Session("rptOpenReceivablesByPatient")) Then
                    ' get out...cannot run report without sql string pre-built
                    Exit Sub
                Else
                    rptReport = New rptOpenReceivablesByPatient
                    strSql = Session("rptOpenReceivablesByPatient")
                    tblReportData = g_IO_Execute_SQL(strSql, False)
                    If tblReportData.Rows.Count > 0 Then
                        rptReport.SetDataSource(tblReportData)
                    End If
                End If
            Case "rptUndistributedPayments"
                If IsNothing(Session("rptUndistributedPayments")) Then
                    ' get out...cannot run report without sql string pre-built
                    Exit Sub
                Else
                    rptReport = New rptUndistributedPayments
                    strSql = Session("rptUndistributedPayments")
                    tblReportData = g_IO_Execute_SQL(strSql, False)
                    If tblReportData.Rows.Count > 0 Then
                        rptReport.SetDataSource(tblReportData)
                    End If
                End If
            Case "rptProductionStats"
                ' 2/28/15 CS Sending SQL string in session variable (built in frmPrintMonthlySummary)
                If IsNothing(Session("rptProductionStats")) Then
                    ' get out...cannot run report without sql string pre-built
                    Exit Sub
                Else
                    rptReport = New rptProductionStats
                    strSql = Session("rptProductionStats")
                    tblReportData = g_IO_Execute_SQL(strSql, False)
                    If tblReportData.Rows.Count > 0 Then
                        ' override close date w/ current period close date
                        For Each row As DataRow In tblReportData.Rows
                            row("ar_lastCloseDate") = Session("rptCloseDate")
                            ' pull hard coded previous month totals (for now)
                            ' ...need to pull in all invoices & payments from QSI (or check ones we did pull in)
                            ' ...not adding up to January report totals
                            ' *** need to figure out how to handle pulling YTD totals ***
                            row("yrTtlCharges") = row("invAmount") + row("ttlCharges")
                            row("yrTtlPayments") = row("payAmount") + row("ttlPayments")
                            row("yrAdjustments") = row("Adjustments") + row("ttlAdjustments")
                        Next
                        rptReport.SetDataSource(tblReportData)
                    End If
                End If
            Case "rptPaymentsRecord"
                rptReport = New rptPaymentsRecord
                strSql = Session("rptPaymentsRecord")
                tblReportData = g_IO_Execute_SQL(strSql, False)
                If tblReportData.Rows.Count > 0 Then
                    rptReport.SetDataSource(tblReportData)
                End If
            Case "rptPaymentsRecordSummary"
                rptReport = New rptPaymentsRecordSummary
                strSql = Session("rptPaymentsRecord")
                tblReportData = g_IO_Execute_SQL(strSql, False)
                If tblReportData.Rows.Count > 0 Then
                    rptReport.SetDataSource(tblReportData)
                End If
            Case "rptClaimsListing"
                rptReport = New rptClaimsListing
                strSql = Session("rptClaimsListing")
                tblReportData = g_IO_Execute_SQL(strSql, False)
                If tblReportData.Rows.Count > 0 Then
                    rptReport.SetDataSource(tblReportData)
                End If
            Case "rptInvoiceListing"
                rptReport = New rptInvoiceListing
                strSql = Session("rptInvoiceListing")
                tblReportData = g_IO_Execute_SQL(strSql, False)
                If tblReportData.Rows.Count > 0 Then
                    rptReport.SetDataSource(tblReportData)
                End If
            Case "rptPaymentListing"
                rptReport = New rptPaymentListing
                strSql = Session("rptPaymentListing")
                tblReportData = g_IO_Execute_SQL(strSql, False)
                If tblReportData.Rows.Count > 0 Then
                    rptReport.SetDataSource(tblReportData)
                End If
        End Select

        ' see if we sent in report parameters
        If IsNothing(Session("rptParameters")) Then
        Else
            Dim arrParameters() As String = Split(Session("rptParameters"), "~~")
            For Each parameter In arrParameters
                Dim arrParamValues() As String = Split(parameter, "||")
                rptReport.SetParameterValue(arrParamValues(0), arrParamValues(1))
            Next
        End If
        crvReport.ReportSource = rptReport
        crvReport.HyperlinkTarget = "_parent"
    End Sub

    Private Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        '02/20/15 T3 
        'Possible fix to "The maximum report processing jobs limit configured by your system administrator has been reached" error
        'crvReport.Dispose()
    End Sub
End Class