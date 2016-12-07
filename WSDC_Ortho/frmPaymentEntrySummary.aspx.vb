Public Class frmPaymentEntrySummary
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
            dteBeginDate.Text = Format(Date.Now, "MM/dd/yyyy")
            dteEndDate.Text = Format(Date.Now, "MM/dd/yyyy")
            ' load ddls
            m_cphContentHolder = Page.Master.FindControl(m_strContentAreaName)
            LoadChildDropDown("users_vw", m_cphContentHolder, "ddlUsers")
        End If

    End Sub
    Private Sub LoadChildDropDown(ByRef strTableName As String, ByRef ContentHolder As ContentPlaceHolder, ByRef strObjectName As String)
        Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
        Dim blnFoundTheTable As Boolean = False
        Dim tblSelections As DataTable = g_IO_Execute_SQL("select recid, " & g_userIdField & " from " & strTableName & " order by " & g_userIdField, blnFoundTheTable)
        If blnFoundTheTable Then
            ' is this an auto built table or one that is expected to auto load, must have a recid and Desc column
            If TypeOf (ctlControl) Is CheckBoxList Then
                CType(ctlControl, CheckBoxList).DataSource = tblSelections
                CType(ctlControl, CheckBoxList).DataValueField = "recid"
                CType(ctlControl, CheckBoxList).DataTextField = "user_id"
                CType(ctlControl, CheckBoxList).DataBind()
            ElseIf TypeOf (ctlControl) Is DropDownList Then

                If UCase(CType(ctlControl, DropDownList).CssClass).Contains("PROMPTSELECTION") Then
                    'add option to dropdown
                    Dim rowSelection As DataRow = tblSelections.NewRow
                    rowSelection("recid") = -1
                    rowSelection("user_id") = "All Users"
                    tblSelections.Rows.InsertAt(rowSelection, 0)
                End If

                CType(ctlControl, DropDownList).DataSource = tblSelections
                CType(ctlControl, DropDownList).DataValueField = "recid"
                CType(ctlControl, DropDownList).DataTextField = "user_id"
                CType(ctlControl, DropDownList).DataBind()
            End If
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim dateFrom As String = Format(CType(dteBeginDate.Text, Date), "MM/dd/yyyy")
        Dim dateTo As String = Format(CType(dteEndDate.Text, Date), "MM/dd/yyyy")
        Dim userID As String = ""
        Dim strWhereClause As String = " where DatePosted >= '" & dateFrom & "' And DatePosted <= '" & dateFrom & " 23:59:59'"
        If ddlUsers.SelectedValue > -1 Then
            userID = ddlUsers.SelectedItem.Text
            strWhereClause &= IIf(userID = "", "", " and Employee = '" & userID & "' ")
        End If
        Dim strSql As String = "select * from rptPaymentEntrySummary_vw " & strWhereClause
        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSql, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            strWhereClause = " where DatePosted >= '" & dateFrom & "' And DatePosted <= '" & dateTo & " 23:59:59'" & _
                IIf(userID = "", "", " and Employee = '" & userID & "' ")
            lblMessage.Text = ""
            If rdoReportType.SelectedValue = "Summary" Then
                Response.Redirect("ReportViewer.aspx?rpt=rptPaymentEntrySummary&where=" & strWhereClause)
            Else
                Response.Redirect("ReportViewer.aspx?rpt=rptPaymentEntryDetail&where=" & strWhereClause)
            End If
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If

    End Sub

End Class