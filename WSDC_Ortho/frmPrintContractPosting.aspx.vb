Public Class frmPrintContractPosting
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
            ' load ddls
            m_cphContentHolder = Page.Master.FindControl(m_strContentAreaName)
            LoadChildDropDown("DropDownList__Doctors_vw", m_cphContentHolder, "ddlDoctor")
            LoadChildDropDown("DropDownList__TransactionCodes_vw", m_cphContentHolder, "ddlCpt_Code")
        End If

    End Sub
    Private Sub LoadChildDropDown(ByRef strTableName As String, ByRef ContentHolder As ContentPlaceHolder, ByRef strObjectName As String)
        Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
        Dim blnFoundTheTable As Boolean = False
        Dim tblSelections As DataTable = g_IO_Execute_SQL("select * from " & strTableName & " order by Descr", blnFoundTheTable)
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
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Static Generator As System.Random = New System.Random()
        Dim strPDFId As String = Generator.Next(0, Format(Date.Now, "HHmmss"))
        Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "_" & Session("user_link_id") & "_" & strPDFId & ".PDF"
        Dim strPOFileName As String = Server.MapPath("downloads\") & strPOFileBase

        rptReport = New rptContractPosting
        Dim strSQL As String = "Select * from rptContractPosting_vw"
        Dim strWhere As String = " where ContractDate >= '" & Format(CType(dteBeginDate.Text, Date), "MM/dd/yyyy") & "' and ContractDate <= '" & Format(CType(dteEndDate.Text, Date), "MM/dd/yyyy") & "  23:59:59 '"

        ' add in addl filters for doctor, cpt_code to strWhere
        If ddlDoctor.SelectedValue > -1 Then
            strWhere &= " and doctor = '" & ddlDoctor.SelectedItem.Text & "'"
        End If
        If ddlCpt_Code.SelectedValue > -1 Then
            strWhere &= " and trans_code_recid = '" & ddlCpt_Code.SelectedValue & "'"
        End If
        strSQL &= strWhere
        Dim tblContracts As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblContracts.Rows.Count > 0 Then
            lblMessage.Text = ""
            rptReport.SetDataSource(tblContracts)
            rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFileName)
            litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase & "&del=y"
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
            litFrameCall.Text = ""
        End If
    End Sub
End Class