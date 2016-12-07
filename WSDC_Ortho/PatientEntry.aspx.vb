Public Class PatientEntry
    Inherits System.Web.UI.Page
    Dim m_strTableName As String = "Patients"
    Dim m_strPrimaryKey As String = " recid "
    Dim m_strFormCode As String = "Patients"
    Dim m_strContentAreaName As String = "MainContent"
    Dim m_cphContentHolder As ContentPlaceHolder = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        'Always needs to be called first
        Call g_RetrieveSessions(txtSessions)

        'CFS 09/24/14 REmoved And txtRecID.Text = "" from If statement here...
        If IsNothing(Request.QueryString("cid")) Then
            System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = -1
            litPatientID.Text = "<h4 class=""col-md-12 text-center"">New Patient Entry</h4>"
            litScripts.Text = "<script  type=""text/javascript"">"
            litScripts.Text &= "jQuery(""#divViewOptions"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#divViewOnly"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#divPatientOptions"").addClass(""hidden"");"
            litScripts.Text &= "</script>"
        Else
            If IsNothing(Request.QueryString("cid")) Then
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = txtRecID.Text
            Else
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = Request.QueryString("cid")
            End If
            ' make sure patient exists, and gather data associated with it
            txtRecID.Text = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))
            Dim strSql As String = "select * from Patients where recid = '" & txtRecID.Text & "'"
            Dim tblPatient As DataTable = g_IO_Execute_SQL(strSql, False)
            If tblPatient.Rows.Count > 0 Then
                litPatientID.Text = "<h3 class=""text-center col-sm-12"">Patient ID: " & tblPatient.Rows(0)("Patient_Id") & "</h3>"
                lblPatientName.Text = Trim(tblPatient.Rows(0)("name_first")) & " " & tblPatient.Rows(0)("name_last") & ""
            Else
                litPatientID.Text = "<h3 class=""text-center col-sm-12"">Patient ID: " & tblPatient.Rows(0)("Patient_Id") & "</h3>"
            End If
            btnSubmit.Visible = False
            btnCancelNew.Visible = False
        End If


        If IsNothing(Request.QueryString("mid")) Then
        Else
            If Request.QueryString("mid") = "edit" Then
                litScripts.Text = "<div id=""divViewOnly"" class=""hidden""></div>"
                litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#divEditOptions"").removeClass(""hidden"");jQuery(""#divViewOptions"").addClass(""hidden"");</script>"
            ElseIf Request.QueryString("mid") = "view" Then
                litScripts.Text = "<div id=""divViewOnly"" style=""width: 100%; height:100%; position: absolute; top:200px; left:0px; z-index:999;"" ></div>"
            End If
        End If

        'This code is use at design time to create your database based on your page layout
        ' the m_cphContentHolder var is also used in daily processing for web page layout access (EX: to auto fill dropdowns)
        m_cphContentHolder = Page.Master.FindControl(m_strContentAreaName)
        If g_ModeCreateDatabase Then
            Call g_BuildBDTableFromForm(m_strTableName, m_cphContentHolder, m_strFormCode, g_ModuleCode, True)
            Exit Sub
        End If

        If IsPostBack Then
        Else
            g_ModuleCode = "PA"
            Dim strWhere As String = m_strPrimaryKey & " = " & IIf(IsNothing(System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))), -1, System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)))
            Dim tblCurrentData As DataTable = g_getData(m_strTableName & "_formmap_vw", strWhere)

            Call g_LoadDropDowns(m_strTableName, m_cphContentHolder)

            If tblCurrentData.Rows.Count = 0 Then
                Call g_DefaultAllSessionVariables(m_strTableName, m_strFormCode, g_ModuleCode, m_strPrimaryKey, Session("user_link_id"))
            Else
                ' load database values to session variables
                Call g_LoadTableInfoFromDatabase(m_strTableName, tblCurrentData, m_strFormCode, g_ModuleCode, Session("user_link_id"))
            End If

            g_loadSessionsToPage(m_cphContentHolder, m_strFormCode, g_ModuleCode)

        End If

        ' load page elements to session variables
        Call g_SendSessions(txtSessions)
    End Sub
    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & IIf(IsNothing(System.Web.HttpContext.Current.Session(m_strPrimaryKey)), -1, System.Web.HttpContext.Current.Session(m_strPrimaryKey))

        'post patient data to table
        Call g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, System.Web.HttpContext.Current.Session("user_link_id"))

        'Get New patient Recid
        Dim intPatientRecid As Integer = g_IO_GetLastRecId()

        Call g_SendSessions(txtSessions)

        ' reload form w/ no querystring to enter a new patient immediately
        Response.Redirect("PatientEntry.aspx?cid=" & intPatientRecid & "&mid=view")

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'Always needs to be called first
        Call g_RetrieveSessions(txtSessions)

        Dim cID As String = txtRecID.Text
        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & cID

        ' saving data so we are just going to update the record 
        Call g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, Session("P_SysUserRECID"))
        Call g_SendSessions(txtSessions)

        Response.Redirect("PatientEntry.aspx?cid=" & cID & "&mid=edit")
    End Sub
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        ' code to delete this patient if no history exists for it...
        Dim strSQL As String = "select * from Payments where PatientNumber = '" & txtPatient_Id.Text & "'"
        Dim tblPatientHistory As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblPatientHistory.Rows.Count > 0 Then
            ' historical data found, cannot delete this patient
            lblMessage.Text = "<script>alert('This patient cannot be deleted.  Historical data found.');</script>"
        Else
            ' delete patient and redirect to patient listing
            strSQL = "delete from Patients where recid = '" & txtRecID.Text & "'"
            g_IO_Execute_SQL(strSQL, False)
            Response.Redirect("frmListmanager.aspx?id=Patients&add=PatientEntry.aspx")
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("PatientEntry.aspx?cid=" & txtRecID.Text & "&mid=view")
    End Sub

    Private Sub btnPaymentPosting_Click(sender As Object, e As EventArgs) Handles btnPaymentPosting.Click
        Response.Redirect("PaymentPosting.aspx?cid=" & txtRecID.Text & "&pid=" & txtPatient_Id.Text & "&cno=" & txtChart_Number.Text)
    End Sub

    Private Sub btnContractStatus_Click(sender As Object, e As EventArgs) Handles btnContractStatus.Click
        Response.Redirect("ContractStatus.aspx?cid=" & txtRecID.Text)
    End Sub

    Private Sub btnPaymentHistory_Click(sender As Object, e As EventArgs) Handles btnPaymentHistory.Click
        Response.Redirect("frmPaymentHistory.aspx?id=PaymentsPosted_vw&add=PaymentPosting.aspx&search=" & txtChart_Number.Text)
    End Sub

    Private Sub btnCancelNew_Click(sender As Object, e As EventArgs) Handles btnCancelNew.Click
        Response.Redirect("frmListmanager.aspx?id=Patients&add=PatientEntry.aspx")
    End Sub
End Class