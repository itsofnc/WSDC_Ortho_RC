Public Class PaymentPosting
    Inherits System.Web.UI.Page
    Dim m_intItemsPerPage As Integer = 100000
    Dim m_intPageNo As Integer = 1

    Dim m_strTableName As String = "PaymentsTemp"
    Dim m_strFormCode As String = "PaymentsTemp"
    Dim m_strPrimaryKey As String = " recid "
    Dim m_strPreviousFormCode As String = ""
    Dim m_strContentAreaName As String = "MainContent"
    Dim m_cphContentHolder As ContentPlaceHolder = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Always needs to be called first
        Call g_RetrieveSessions(txtSessions)

        Dim blnViewMode As Boolean = False

        If IsNothing(Request.QueryString("cid")) Then
            'User wants to enter new payment, not coming from anywhere
            System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = -1
            litScripts.Text = "<script  type=""text/javascript"">"
            litScripts.Text &= "jQuery(""#divViewOptions"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#divViewOnly"").addClass(""hidden"");"
            litScripts.Text &= "</script>"

        Else
            'We are either coming from the contracts entry screen or viewing a temp payment from grid 
            'viewing/editing temp payment sends parameter "mid", so we can distinguish where we are coming from based on this parameter

            If IsNothing(Request.QueryString("mid")) Then
                'no view/edit mode
                litScripts.Text &= "<script type=""text/javascript"">jQuery(""#divEditOptions"").addClass(""hidden"");jQuery(""#divBtnAdd"").removeClass(""hidden"");</script>"
                'coming from contracts page 

                'Load patient info if coming from Contracts page - Chart number is sent in 
                If IsNothing(Request.QueryString("cno")) Then
                Else
                    txtChartNumber.Text = Request.QueryString("cno")
                    litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { getPatName('cht" & Request.QueryString("cno") & "');jQuery(""#divViewOptions"").addClass(""hidden""); });</script>"
                End If
            Else
                If Request.QueryString("mid") = "edit" Then
                    litScripts.Text = "<div id=""divViewOnly"" class=""hidden""></div>"
                    litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { getPatName('cht" & Request.QueryString("cno") & "'); });</script>"
                    litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#divViewOptions"").addClass(""hidden"");jQuery(""#divEditOptions"").removeClass(""hidden"");jQuery(""#divBtnAdd"").addClass(""hidden"");</script>"

                ElseIf Request.QueryString("mid") = "view" Then
                    litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { getPatName('cht" & Request.QueryString("cno") & "'); });</script>"
                    litViewOnly.Text = "<div id=""divViewOnly"" style=""width: 100%; height:100%; position: absolute; left:0px; z-index:999;"" ></div>"
                    blnViewMode = True
                End If

                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = Request.QueryString("cid") 'cid is actually the payment recid

                ' make sure contract exists, and gather data associated with it
                txtPaymentID.Text = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))

                'Check to see if we are editing an existing payment--txtPaymentID will be filled
                Dim strSql As String = "select * from PaymentsTemp_vw where recid = '" & txtPaymentID.Text & "'"
                Dim tblPayment As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblPayment.Rows.Count > 0 Then
                    ' get chart number for getPatName function that we will execute
                    litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { pullPaymentInfo('" & txtPaymentID.Text & "','" & blnViewMode & "'); });</script>"
                End If
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
            g_ModuleCode = "PT"
            litHeader.Text = "<h4 class=""col-md-12 text-center"" >Payment Posting</h4>"
            btnPost.Attributes.Add("onClick", "postData();")


            'If IsNothing(Request.QueryString("cno")) Then
            'Else
            '    txtChartNumber.Text = Request.QueryString("cno")
            'End If
            dtePaymentDate.Text = Date.Now.ToShortDateString
            Dim strValues As String = IIf(IsNothing(Request.QueryString("values")), Request.Form("values"), Request.QueryString("values"))
            '''''''''''''''''''''''''''''''''''''''

            Call g_LoadDropDowns(m_strTableName, m_cphContentHolder)

            'Build javascript data array for insurnace dropdown
            'litScripts.Text &= "<script type=""text/javascript"">var arrClaims = " & BuildJavascriptClaimsArray("-1", "-1") & ";</script>"
            ddlClaimNumber.Attributes.Add("onChange", "ClaimSelected(this.selectedValue);addChangeToTxt()")
            dolPatientAmount.Attributes.Add("OnKeyUp", "populateAmounts(); rdoChange('patAmt'); setOrig_Payment();")
            dolPrimaryAmount.Attributes.Add("OnKeyUp", "rdoChange('primAmt'); setOrig_Payment();")
            dolSecondaryAmount.Attributes.Add("OnKeyUp", " rdoChange('secAmt'); setOrig_Payment();")
            'Building dddl for insurance
            'Dummy dropdown
            Dim strSql = "select -1 as ddlIndex, '' as claimNumber"
            Dim intCounter As Integer = 0
            Dim tblInsurance As DataTable = g_IO_Execute_SQL(strSql, False)
            Dim row As DataRow = tblInsurance.NewRow
            row("ddlIndex") = -2
            row("claimNumber") = "Choose an option"
            tblInsurance.Rows.InsertAt(row, 0)
            row("ddlIndex") = -1

            For Each rowInsurance In tblInsurance.Rows
                rowInsurance("ddlIndex") = intCounter
            Next

            ddlClaimNumber.DataSource = tblInsurance
            ddlClaimNumber.DataValueField = "ddlIndex"
            ddlClaimNumber.DataTextField = "claimNumber"
            ddlClaimNumber.DataBind()


            'Build DDL
            Dim strWhere As String = m_strPrimaryKey & " = " & IIf(IsNothing(System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))), -1, System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)))
            Dim tblCurrentData As DataTable = g_getData(m_strTableName & "_formmap_vw", strWhere)

            If tblCurrentData.Rows.Count = 0 Then
                Call g_DefaultAllSessionVariables(m_strTableName, m_strFormCode, g_ModuleCode, m_strPrimaryKey, Session("user_link_id"))
            Else
                ' load database values to session variables
                Call g_LoadTableInfoFromDatabase(m_strTableName, tblCurrentData, m_strFormCode, g_ModuleCode, Session("user_link_id"))
            End If

            g_loadSessionsToPage(m_cphContentHolder, m_strFormCode, g_ModuleCode)

            litScripts.Text &= "<script type=""text/javascript"">var claimIndex = " & Session("PT_PaymentsTemp__txtClaimIndex") & "</script>"


            If strValues = "" Then
            Else
                Dim valuesArr() As String = Split(strValues, "||")
                txtChartNumber.Text = valuesArr(0)
                dolPatientAmount.Text = valuesArr(1)
                dolPrimaryAmount.Text = valuesArr(2)
                dolSecondaryAmount.Text = valuesArr(3)
                dolApplyToCurrentInvoice.Text = valuesArr(4)
                dolApplyToNextInvoice.Text = valuesArr(5)
                dolApplyToPastDue.Text = valuesArr(6)
                dolApplyToPrinciple.Text = valuesArr(7)
                ddlPaymentType.SelectedValue = valuesArr(8)
                txtPaymentReference.Text = valuesArr(9)
                ddlPaymentFor.SelectedValue = valuesArr(10)
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready ( function () { getPatName('cht'); });</script>"
            End If
        End If

        Call g_SendSessions(txtSessions)
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        'if txtContract_RECID is -1 then this patient does not have a contract yet, create patient record
        If txtContract_RECID.Text = "-1" Then
            CreatePatientData(txtChartNumber.Text)
        End If

        g_ModuleCode = "PT"
        Dim strValues As String = Nothing
        'Commenting out until I'm able to test thoroughly 
        'If chkKeepFrmValues.Checked = True Then
        '    Dim chtNum As String = txtChartNumber.Text
        '    Dim patAmount As String = dolPatientAmount.Text
        '    Dim primAmount As String = dolPrimaryAmount.Text
        '    Dim secAmount As String = dolSecondaryAmount.Text
        '    Dim curInvoice As String = dolApplyToCurrentInvoice.Text
        '    Dim nextInvoice As String = dolApplyToNextInvoice.Text
        '    Dim pastDue As String = dolApplyToPastDue.Text
        '    Dim curPrincipal As String = dolApplyToPrinciple.Text
        '    Dim paymentMethod As String = ddlPaymentType.SelectedValue
        '    Dim paymentReference As String = txtPaymentReference.Text
        '    Dim paymentDescription As String = ddlDescription.SelectedValue
        '    strValues = chtNum & "||" & patAmount & "||" & primAmount & "||" & secAmount & "||" & curInvoice & "||" & nextInvoice & "||" & pastDue & "||" & curPrincipal & "||" & paymentMethod & "||" & paymentReference & "||" & paymentDescription
        'End If

        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & IIf(IsNothing(System.Web.HttpContext.Current.Session(m_strPrimaryKey)), -1, System.Web.HttpContext.Current.Session(m_strPrimaryKey))

        Call g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, System.Web.HttpContext.Current.Session("user_link_id"))
        'LoadListTableToGrid(m_strTableName, m_intPageNo, 1)
        Response.Redirect("PaymentPosting.aspx?values=" & strValues)
        Call g_SendSessions(txtSessions)




    End Sub
    Public Sub CreatePatientData(ByVal strChartNum As String)
        Dim intPatientID As Integer = -1
        Dim strSQL As String = "select * from IMPROVIS_PatientData_vw where ChartNo = '" & strChartNum & "'"
        Dim tblPatient As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblPatient.Rows.Count > 0 Then
            ' patient found, set variable to use in insert below
            intPatientID = tblPatient.Rows(0)("PatientKey")
        End If

        'make sure this record is not already in the patients table from previous attempt to save contract and user hits back button and tries to save again!!!
        strSQL = "select recid from patients where patient_id = '" & intPatientID & "'"
        Dim tblPat As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblPat.Rows.Count = 0 Then
            Dim strNameMid As String = IIf(IsDBNull(tblPatient.Rows(0)("MiddleName")), "", tblPatient.Rows(0)("MiddleName"))
            ' build insert statement
            strSQL = "INSERT INTO Patients " & _
                "(patient_id" & _
                ", chart_number" & _
                ", name_last" & _
                ", name_first" & _
                ", name_mid" & _
                ", addr_other" & _
                ", addr_street" & _
                ", addr_city" & _
                ", addr_state" & _
                ", addr_zip" & _
                ", gender" & _
                ", dob)" & _
                " VALUES (" & _
                "'" & intPatientID & "', " & _
                "'" & strChartNum & "', " & _
                "'" & tblPatient.Rows(0)("LastName") & "', " & _
                "'" & tblPatient.Rows(0)("FirstName") & "', " & _
                "'" & strNameMid & "', " & _
                "'" & tblPatient.Rows(0)("Address2") & "', " & _
                "'" & tblPatient.Rows(0)("Address1") & "', " & _
                "'" & tblPatient.Rows(0)("City") & "', " & _
                "'" & tblPatient.Rows(0)("State") & "', " & _
                "'" & tblPatient.Rows(0)("ZipCode") & "', " & _
                "'" & IIf(tblPatient.Rows(0)("Gender") = "F", "1", "2") & "', " & _
                "'" & tblPatient.Rows(0)("DOB") & "')"

            ' check strSQL as I changed the pat # and chart # to varchar in my patients table...were int's before.
            g_IO_Execute_SQL(strSQL, False)
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'Always needs to be called first
        Call g_RetrieveSessions(txtSessions)

        g_ModuleCode = "PT"

        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & txtPaymentID.Text
        ' saving data so we are just going to update the record 
        Call g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, System.Web.HttpContext.Current.Session("user_link_id"))
        Call g_SendSessions(txtSessions)

        Response.Redirect("PaymentPosting.aspx")
    End Sub
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        ' delete payment
        Dim strSQL As String = "delete from PaymentsTemp where recid = '" & txtPaymentID.Text & "'"
        g_IO_Execute_SQL(strSQL, False)
        Response.Redirect("PaymentPosting.aspx")
    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("PaymentPosting.aspx")
    End Sub
End Class