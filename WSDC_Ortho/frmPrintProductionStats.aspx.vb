Public Class frmPrintProductionStats
    Inherits System.Web.UI.Page
    Dim m_strContentAreaName As String = "MainContent"
    Dim m_cphContentHolder As ContentPlaceHolder = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        lblMessage.Text = ""
        If IsPostBack Then
        Else
            ' load ddls
            m_cphContentHolder = Page.Master.FindControl(m_strContentAreaName)
            LoadChildDropDown("DropDownList__Doctors_vw", m_cphContentHolder, "ddlDoctors")
        End If
    End Sub

    Private Sub LoadChildDropDown(ByRef strTableName As String, ByRef ContentHolder As ContentPlaceHolder, ByRef strObjectName As String)
        Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
        Dim blnFoundTheTable As Boolean = False
        Dim tblSelections As DataTable = g_IO_Execute_SQL("select * from " & strTableName & " order by descr ", blnFoundTheTable)
        If blnFoundTheTable Then
            ' is this an auto built table or one that is expected to auto load, must have a recid and Desc column
            If TypeOf (ctlControl) Is CheckBoxList Then
                CType(ctlControl, CheckBoxList).DataSource = tblSelections
                CType(ctlControl, CheckBoxList).DataValueField = "recid"
                CType(ctlControl, CheckBoxList).DataTextField = "descr"
                CType(ctlControl, CheckBoxList).DataBind()
            ElseIf TypeOf (ctlControl) Is DropDownList Then

                If UCase(CType(ctlControl, DropDownList).CssClass).Contains("PROMPTSELECTION") Then
                    'add option to dropdown
                    Dim rowSelection As DataRow = tblSelections.NewRow
                    rowSelection("recid") = -1
                    rowSelection("descr") = "Choose an option"
                    tblSelections.Rows.InsertAt(rowSelection, 0)
                End If

                CType(ctlControl, DropDownList).DataSource = tblSelections
                CType(ctlControl, DropDownList).DataValueField = "recid"
                CType(ctlControl, DropDownList).DataTextField = "descr"
                CType(ctlControl, DropDownList).DataBind()
            End If
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim dateFrom As String = Format(CType(dteBeginDate.Text, Date), "yyyy-MM-dd")
        Dim dateTo As String = Format(CType(dteEndDate.Text, Date), "yyyy-MM-dd")
        Dim strSQL As String = ""
        ' add in addl filters for doctor to strWhere
        If ddlDoctors.SelectedValue > -1 Then
            Dim strWhereFilter As String = " and contracts.Doctors_vw = '" & ddlDoctors.SelectedItem.Value & "'"
            lblMessage.Text = "Doctor filter currently not available."
        Else
            strSQL = "Select ar_lastCloseDate, ar_previousBal, prdBeginDate, prdEndDate, ttlCharges, ttlPayments, ttlAdjustments, " & _
            "0 as Adjustments, 0 AS yrTtlCharges, 0 AS yrTtlPayments, 0 AS yrUndistribPayments, 0 AS yrDistribPayments, 0 as yrAdjustments, (ISNULL" & _
            "((SELECT        SUM(ISNULL(AmountDue, 0)) AS Expr1" & _
            "    FROM            dbo.Invoices " & _
            "    WHERE        (PostDate >= '" & dateFrom & "') AND (PostDate <= '" & dateTo & " 23:59:59')), 0) + ISNULL " & _
            "((SELECT        SUM(ISNULL(procedure_amount, 0)) AS Expr1 " & _
            "    FROM            dbo.Claims " & _
            "    WHERE        (DateProcessed >= '" & dateFrom & "') AND (DateProcessed <= '" & dateTo & " 23:59:59')), 0)) AS invAmount, ISNULL" & _
            "((SELECT        SUM(ISNULL(PatientAmount + PrimaryAmount + SecondaryAmount, 0)) AS Expr1 " & _
            "    FROM            dbo.Payments " & _
            "    WHERE        (DatePosted >= '" & dateFrom & "') AND (DatePosted <= '" & dateTo & " 23:59:59')), 0) AS payAmount, ISNULL" & _
            "((SELECT        SUM(ISNULL(PatientAmount + PrimaryAmount + SecondaryAmount, 0)) AS Expr1 " & _
            "    FROM            dbo.Payments " & _
            "    WHERE        (DatePosted >= '" & dateFrom & "') AND (DatePosted <= '" & dateTo & " 23:59:59') AND (Contract_RECID = -1)), 0) AS undistribPayments, ISNULL" & _
            "((SELECT        SUM(ISNULL(PatientAmount + PrimaryAmount + SecondaryAmount, 0)) AS Expr1 " & _
            "    FROM            dbo.Payments " & _
            "    WHERE        (DatePosted >= '" & dateFrom & "') AND (DatePosted <= '" & dateTo & " 23:59:59') AND (Contract_RECID > 0)), 0) AS distribPayments " & _
            "FROM rptAR_MonthlySummary_vw where prdBeginDate >= '" & dateFrom & "' and prdEndDate <= '" & dateTo & " 23:59:59'"

            Dim tblReportData As DataTable = g_IO_Execute_SQL(strSQL, False)
            'dowload/print invoice
            If tblReportData.Rows.Count > 0 Then
                lblMessage.Text = ""
                ' send strSQL over in session variable 
                Session("rptProductionStats") = strSQL
                Session("rptParameters") = "Title||" & dteBeginDate.Text & " - " & dteEndDate.Text
                Session("rptCloseDate") = dateTo
                Response.Redirect("ReportViewer.aspx?rpt=rptProductionStats")
            Else
                lblMessage.Text = "No records found matching criteria entered...<br />"
            End If
        End If
    End Sub
End Class