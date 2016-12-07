Public Class frmPrintDailySummary
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        If IsPostBack Then
        Else
            dteBeginDate.Text = Format(Date.Now, "MM/dd/yyyy")
            dteEndDate.Text = Format(Date.Now, "MM/dd/yyyy")
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Static Generator As System.Random = New System.Random()
        Dim strPDFId As String = Generator.Next(0, Format(Date.Now, "HHmmss"))
        Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "_" & Session("user_link_id") & "_" & strPDFId & ".PDF"
        Dim strPOFileName As String = Server.MapPath("downloads\") & strPOFileBase

        rptReport = New rptDailyTotalsSummary
        Dim strSQL As String = "Select TOP 1 * from rptDailyTotals_vw"
        Dim tblSummary As DataTable = g_IO_Execute_SQL(strSQL, False)
        ' build up table for each day of date range
        Dim ttlCharges As Integer = 0
        Dim ttlPayments As Integer = 0
        Dim ttlRefunds As Integer = 0
        Dim ttlNetChange As Integer = 0
        Dim strDate As String = dteBeginDate.Text
        Dim tblCharges As DataTable = Nothing
        Dim tblPayments As DataTable = Nothing
        Dim tblRefunds As DataTable = Nothing

        ' remove the data row selected, just need structure/layout from this view
        For i = tblSummary.Rows.Count - 1 To 0 Step -1
            tblSummary.Rows.RemoveAt(i)
        Next

        ' create a row for each day within date range specified
        Dim row As DataRow = Nothing
        Dim BeginDt As Date = Date.Parse(dteBeginDate.Text)
        Dim EndDt As Date = Date.Parse(dteEndDate.Text)
        Dim ttlDays As Integer = DateDiff(DateInterval.Day, BeginDt, EndDt)
        For i = 1 To ttlDays + 1
            row = tblSummary.NewRow
            ttlCharges = 0.0
            ttlPayments = 0.0
            ttlRefunds = 0.0
            ttlNetChange = 0.0

            ' gather totals of charges
            tblCharges = g_IO_Execute_SQL("select sum(amount) as ttlCharges from contracts where contractDate >= '" & strDate & "' and contractDate <= '" & strDate & " 23:59:59'", False)
            If IsDBNull(tblCharges.Rows(0)("ttlCharges")) Then
            Else
                ttlCharges = tblCharges.Rows(0)("ttlCharges")
            End If
            ' gather totals of payments
            tblPayments = g_IO_Execute_SQL("select sum(PatientAmount+PrimaryAmount+SecondaryAmount) as ttlPymt from payments WHERE (RECID = BaseRecid) And (DatePosted >= '" & strDate & "') And (DatePosted <= '" & strDate & " 23:59:59')", False)
            If IsDBNull(tblPayments.Rows(0)("ttlPymt")) Then
            Else
                ttlPayments = tblPayments.Rows(0)("ttlPymt")
            End If
            ' gather totals of refunds
            tblRefunds = g_IO_Execute_SQL("select sum(PatientAmount+PrimaryAmount+SecondaryAmount) as ttlPymt from ReversedPayments WHERE (RECID = BaseRecid) And (DatePosted >= '" & strDate & "') And (DatePosted <= '" & strDate & " 23:59:59')", False)
            If IsDBNull(tblRefunds.Rows(0)("ttlPymt")) Then
            Else
                ttlRefunds = tblRefunds.Rows(0)("ttlPymt")
            End If
            ' determine net change value
            ttlNetChange = ttlCharges - ttlPayments - ttlRefunds

            ' create daily summary row & reset array
            row("postingDate") = strDate
            row("charges") = ttlCharges
            row("payments") = ttlPayments
            row("refunds") = ttlRefunds
            row("netChange") = ttlNetChange
            tblSummary.Rows.Add(row)

            ' set date variable to next day
            strDate = DateAdd("d", i, dteBeginDate.Text)
        Next

        If tblSummary.Rows.Count > 0 Then
            lblMessage.Text = ""
            rptReport.SetDataSource(tblSummary)
            rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFileName)
            litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase & "&del=y"
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
            litFrameCall.Text = ""
        End If
    End Sub

End Class