Public Class frmPrintPaymentsRecord
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
            LoadChildDropDownUsers("users_vw", m_cphContentHolder, "ddlUsers")
            LoadChildDropDownPaymentTypes("DropDownList__PaymentType_vw", m_cphContentHolder, "ddlPaymentType")
        End If

    End Sub
    Private Sub LoadChildDropDownUsers(ByRef strTableName As String, ByRef ContentHolder As ContentPlaceHolder, ByRef strObjectName As String)
        Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
        Dim blnFoundTheTable As Boolean = False
        Dim tblUsers As DataTable = g_IO_Execute_SQL("select recid, " & g_userIdField & " from " & strTableName & " order by " & g_userIdField, blnFoundTheTable)
        If blnFoundTheTable Then
            ' is this an auto built table or one that is expected to auto load, must have a recid and Desc column
            If TypeOf (ctlControl) Is CheckBoxList Then
                CType(ctlControl, CheckBoxList).DataSource = tblUsers
                CType(ctlControl, CheckBoxList).DataValueField = "recid"
                CType(ctlControl, CheckBoxList).DataTextField = "user_id"
                CType(ctlControl, CheckBoxList).DataBind()
            ElseIf TypeOf (ctlControl) Is DropDownList Then

                If UCase(CType(ctlControl, DropDownList).CssClass).Contains("PROMPTSELECTION") Then
                    'add option to dropdown
                    Dim rowSelection As DataRow = tblUsers.NewRow
                    rowSelection("recid") = -1
                    rowSelection("user_id") = "All Users"
                    tblUsers.Rows.InsertAt(rowSelection, 0)
                End If

                CType(ctlControl, DropDownList).DataSource = tblUsers
                CType(ctlControl, DropDownList).DataValueField = "recid"
                CType(ctlControl, DropDownList).DataTextField = "user_id"
                CType(ctlControl, DropDownList).DataBind()
            End If
        End If
    End Sub

    Private Sub LoadChildDropDownPaymentTypes(ByRef strTableName As String, ByRef ContentHolder As ContentPlaceHolder, ByRef strObjectName As String)
        Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
        Dim blnFoundTheTable As Boolean = False
        Dim tblPaymentTypes As DataTable = g_IO_Execute_SQL("select recid, descr from " & strTableName & " order by descr", blnFoundTheTable)
        If blnFoundTheTable Then
            ' is this an auto built table or one that is expected to auto load, must have a recid and Desc column
            If TypeOf (ctlControl) Is CheckBoxList Then
                CType(ctlControl, CheckBoxList).DataSource = tblPaymentTypes
                CType(ctlControl, CheckBoxList).DataValueField = "recid"
                CType(ctlControl, CheckBoxList).DataTextField = "descr"
                CType(ctlControl, CheckBoxList).DataBind()
            ElseIf TypeOf (ctlControl) Is DropDownList Then

                If UCase(CType(ctlControl, DropDownList).CssClass).Contains("PROMPTSELECTION") Then
                    'add option to dropdown
                    Dim rowSelection As DataRow = tblPaymentTypes.NewRow
                    rowSelection("recid") = -1
                    rowSelection("descr") = "All Payment Types"
                    tblPaymentTypes.Rows.InsertAt(rowSelection, 0)
                End If

                CType(ctlControl, DropDownList).DataSource = tblPaymentTypes
                CType(ctlControl, DropDownList).DataValueField = "recid"
                CType(ctlControl, DropDownList).DataTextField = "descr"
                CType(ctlControl, DropDownList).DataBind()
            End If
        End If
    End Sub


    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim dateFrom As String = Format(CType(dteBeginDate.Text, Date), "yyyy-MM-dd")
        Dim dateTo As String = Format(CType(dteEndDate.Text, Date), "yyyy-MM-dd")
        Dim userID As String = ""
        Dim paymentType As String = ""
        Dim strWhereClause As String = " where DatePosted >= '" & dateFrom & "  00:00:00  ' and DatePosted <= '" & dateTo & " 23:59:59' "
        If ddlUsers.SelectedValue > -1 Then
            userID = ddlUsers.SelectedItem.Text
            strWhereClause &= IIf(userID = "", "", " and " & g_userIdField & " = '" & userID & "' ")
        End If
        If ddlPaymentType.SelectedValue > -1 Then
            paymentType = ddlPaymentType.SelectedItem.Text
            strWhereClause &= IIf(paymentType = "", "", " and PaymentType = '" & paymentType & "' ")
        End If
        Dim strSql As String = "select * from rptPaymentsRecord_vw " & strWhereClause
        Dim tblReportData As DataTable = g_IO_Execute_SQL(strSql, False)
        'dowload/print invoice
        If tblReportData.Rows.Count > 0 Then
            'strWhereClause = " where DatePosted >= '" & Format(CType(dateFrom, Date), "MM/dd/yyyy") & "' and DatePosted <= '" & Format(CType(dateTo, Date), "MM/dd/yyyy") & "' " & IIf(userID = "", "", " and user_id = '" & userID & "' ")
            lblMessage.Text = ""
            Session("rptPaymentsRecord") = "select * from rptPaymentsRecord_vw " & strWhereClause
            If rdoReportType.SelectedValue = "Summary" Then
                Response.Redirect("ReportViewer.aspx?rpt=rptPaymentsRecordSummary")
            Else
                Response.Redirect("ReportViewer.aspx?rpt=rptPaymentsRecord")
            End If
        Else
            lblMessage.Text = "No records found matching criteria entered...<br />"
        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("Dashboard.aspx")
    End Sub
End Class