Public Class frmPrintOpenReceivables
    Inherits System.Web.UI.Page
    Dim m_strContentAreaName As String = "MainContent"
    Dim m_cphContentHolder As ContentPlaceHolder = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        If IsPostBack Then
        Else
            txtChartNo.Text = ""
            ' load ddls
            m_cphContentHolder = Page.Master.FindControl(m_strContentAreaName)
            LoadPatientDropDown("Patients_vw", m_cphContentHolder, "ddlPatients", "")
            LoadProviderDropDown("DropDownList__Doctors_vw", m_cphContentHolder, "ddlProviders", "")
        End If
        ddlPatients.Attributes.Add("onChange", "addChangeToTxt();")
        litScripts.Text = "<script>function getPatName() { jQuery('#" & btnSearch.ClientID & "').click();} </script>"
    End Sub

    Private Sub LoadPatientDropDown(ByRef strTableName As String, ByRef ContentHolder As ContentPlaceHolder, ByRef strObjectName As String, ByRef strWhereSearch As String)
        Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
        Dim blnFoundTheTable As Boolean = False
        Dim strSQL As String = "select * from " & strTableName & strWhereSearch & " order by PatientName "
        Dim tblSelections As DataTable = g_IO_Execute_SQL(strSQL, blnFoundTheTable)
        If blnFoundTheTable Then
            ' is this an auto built table or one that is expected to auto load, must have a recid and Desc column
            If TypeOf (ctlControl) Is CheckBoxList Then
                CType(ctlControl, CheckBoxList).DataSource = tblSelections
                CType(ctlControl, CheckBoxList).DataValueField = "chart_number"
                CType(ctlControl, CheckBoxList).DataTextField = "PatientName"
                CType(ctlControl, CheckBoxList).DataBind()
            ElseIf TypeOf (ctlControl) Is DropDownList Then

                If UCase(CType(ctlControl, DropDownList).CssClass).Contains("PROMPTSELECTION") Then
                    'add option to dropdown
                    Dim rowSelection As DataRow = tblSelections.NewRow
                    rowSelection("chart_number") = -1
                    rowSelection("PatientName") = "Choose an option"
                    tblSelections.Rows.InsertAt(rowSelection, 0)
                End If

                CType(ctlControl, DropDownList).DataSource = tblSelections
                CType(ctlControl, DropDownList).DataValueField = "chart_number"
                CType(ctlControl, DropDownList).DataTextField = "PatientName"
                CType(ctlControl, DropDownList).DataBind()
            End If
        End If
    End Sub

    Private Sub LoadProviderDropDown(ByRef strTableName As String, ByRef ContentHolder As ContentPlaceHolder, ByRef strObjectName As String, ByRef strWhereSearch As String)
        Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
        Dim blnFoundTheTable As Boolean = False
        Dim strSQL As String = "select RECID, DESCR from " & strTableName & strWhereSearch & " order by DESCR "
        Dim tblSelections As DataTable = g_IO_Execute_SQL(strSQL, blnFoundTheTable)
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

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim strWhereSearch As String = ""
        If txtChartNo.Text <> "" Then
            strWhereSearch = " where chart_number = '" & txtChartNo.Text & "' "
            ddlPatients.SelectedValue = txtChartNo.Text
        Else
            strWhereSearch = ""
            ddlPatients.SelectedValue = "-1"
        End If
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Static Generator As System.Random = New System.Random()
        Dim strPDFId As String = Generator.Next(0, Format(Date.Now, "HHmmss"))
        Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "_" & Session("user_link_id") & "_" & strPDFId & ".PDF"
        Dim strPOFileName As String = Server.MapPath("downloads\") & strPOFileBase

        rptReport = New rptInsuranceTracking
        Dim strSQL As String = "Select * from rptOpenReceivablesByPatient_vw"
        Dim strWhere As String = " where openBalance > 0"
        ' add in addl filters for doctor, cpt_code to strWhere
        If ddlPatients.SelectedValue > -1 Then
            strWhere &= " and chart_number = '" & ddlPatients.SelectedItem.Value & "'"
        End If
        If ddlProviders.SelectedValue > -1 Then
            strWhere &= " and provider = '" & ddlProviders.SelectedItem.Text & "'"
        End If
        strSQL &= strWhere

        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSQL, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            lblMessage.Text = ""
            ' send strSQL over in session variable 
            Session("rptOpenReceivablesByPatient") = strSQL
            Response.Redirect("ReportViewer.aspx?rpt=rptOpenReceivablesByPatient")
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("Dashboard.aspx")
    End Sub
End Class