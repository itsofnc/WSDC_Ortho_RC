Public Class ContractEntry
    Inherits System.Web.UI.Page
    Dim m_strTableName As String = "Contracts"
    Dim m_strPrimaryKey As String = " recid "
    Dim m_strFormCode As String = "CE"
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
            litContractID.Text = "<h4 class=""col-md-12 text-center"">New Contract Entry</h4>"
            litScripts.Text = "<script  type=""text/javascript"">"
            litScripts.Text &= "jQuery(""#divViewOptions"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#divViewOnly"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#divEditOptions"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#divContractOptions"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#divEndOfContractLookup"").addClass(""hidden"");"
            litScripts.Text &= "</script>"
            litShowPatientData.Text = "<span id=""showPatientData"" class=""text-center col-sm-12 hidden""><span class=""glyphicon glyphicon-eye-open""></span><a href=""#patinfo""> View Patient Information</a></span>"
        Else
            If IsNothing(Request.QueryString("cid")) Then
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = txtRecID.Text
            Else
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = Request.QueryString("cid")
            End If
            litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#divContractSearch"").addClass(""hidden"");</script>"
            ' make sure contract exists, and gather data associated with it
            txtRecID.Text = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))
            Dim strSql As String = "select * from Contracts_vw where recid = '" & txtRecID.Text & "'"
            Dim tblContracts As DataTable = g_IO_Execute_SQL(strSql, False)
            If tblContracts.Rows.Count > 0 Then
                litContractID.Text = "<h3 class=""text-center col-sm-12"">Account #: " & tblContracts.Rows(0)("Account_Id") & " Chart #: " & tblContracts.Rows(0)("ChartNumber") & "</h3>"
                If IsDBNull(tblContracts.Rows(0)("EndContractDate")) Then
                Else
                    litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#divEndOfContractLookup"").removeClass(""hidden"");</script>"
                End If
                lblPatientName.Text = Trim(tblContracts.Rows(0)("FirstName")) & " " & tblContracts.Rows(0)("LastName") & ""
                litShowPatientData.Text = "<span id=""showPatientData"" class=""text-center col-sm-12""><span class=""glyphicon glyphicon-eye-open""></span><a href=""#"" onclick=""showPatientInfo()""> View Patient Information</a></span>"
            Else
                litContractID.Text = "<h3 class=""text-center col-sm-12"">Account #: " & tblContracts.Rows(0)("Account_Id") & " Chart #: " & tblContracts.Rows(0)("ChartNumber") & "</h3>"
                litShowPatientData.Text = "<span id=""showPatientData"" class=""text-center col-sm-12 hidden""><span class=""glyphicon glyphicon-eye-open""></span><a href=""#"" onclick=""showPatientInfo()""> View Patient Information</a></span>"
            End If
            btnSubmit.Visible = False
            btnCancelNew.Visible = False
        End If

        If UCase(Session("user_role")) = "ADMINISTRATOR" Or Session("user_role") = "1" Then
            'Admin Nav
            If IsNothing(Request.QueryString("mid")) Then
                litScripts.Text &= "<div id=""divViewOnly"" class=""hidden""></div>"
            Else
                If Request.QueryString("mid") = "edit" Then
                    litScripts.Text &= "<div id=""divViewOnly"" class=""hidden""></div>"
                    If IsNothing(Request.QueryString("cid")) Then
                        'Edit Mode of New Contract handled above
                    Else
                        'Edit mode of existing contract 
                        litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#divEditOptions"").removeClass(""hidden"");jQuery(""#divViewOptions"").addClass(""hidden"");jQuery(""#divEndOfContractLookup"").removeClass(""hidden"");</script>"
                    End If
                ElseIf Request.QueryString("mid") = "view" Then
                    'Admin view mode 
                    litScripts.Text &= "<div id=""divViewOnly"" style=""width: 100%; height:100%; position: absolute; top:200px; left:0px; z-index:999;"" ></div>"
                    litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#editLink"").removeClass(""hidden"");</script>"
                End If
            End If
        Else
            'user
            'View mode, no edit options 
            litScripts.Text &= "<div id=""divViewOnly"" style=""width: 100%; height:100%; position: absolute; top:200px; left:0px; z-index:999;"" ></div>"
            litScripts.Text &= "jQuery(""#divViewOptions"").removeClass(""hidden"");"
            litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#editLink"").addClass(""hidden"");jQuery(""#generateClaimLink"").addClass(""hidden"");</script>"
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
            g_ModuleCode = "C"
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

            ' 10/28 CFS: Load child data to modals
            LoadChildDropDown("DropDownList__Gender_vw", m_cphContentHolder, "ddlGender")
            LoadChildDropDown("DropDownList__Gender_vw", m_cphContentHolder, "ddlInsured_Gender")
            LoadChildDropDown("DropDownList__Gender_vw", m_cphContentHolder, "ddlInsured_Gender_sec")
            LoadChildDropDown("DropDownList__Relation_vw", m_cphContentHolder, "ddlRelationship")
            LoadChildDropDown("DropDownList__Relation_vw", m_cphContentHolder, "ddlRelationship_sec")

            Dim strSQL As String = "Select * from patients where chart_number = '" & txtChartNumber.Text & "'"
            Dim tblPat As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblPat.Rows.Count > 0 Then
                txtname_first.Text = tblPat.Rows(0)("name_first")
                txtname_mid.Text = tblPat.Rows(0)("name_mid")
                txtname_last.Text = tblPat.Rows(0)("name_last")
                txtaddr_other.Text = tblPat.Rows(0)("addr_other")
                txtaddr_street.Text = tblPat.Rows(0)("addr_street")
                txtaddr_city.Text = tblPat.Rows(0)("addr_city")
                txtaddr_state.Text = tblPat.Rows(0)("addr_state")
                txtaddr_zip.Text = tblPat.Rows(0)("addr_zip")
                ddlGender.SelectedValue = tblPat.Rows(0)("gender")
                dteDOB.Text = tblPat.Rows(0)("dob")
            Else
                txtname_first.Text = ""
                txtname_mid.Text = ""
                txtname_last.Text = ""
                txtaddr_other.Text = ""
                txtaddr_street.Text = ""
                txtaddr_city.Text = ""
                txtaddr_state.Text = ""
                txtaddr_zip.Text = ""
                ddlGender.Text = ""
                dteDOB.Text = ""
            End If

            strSQL = "Select * from patient_insurance_vw where chart_number = '" & txtChartNumber.Text & "'"
            Dim tblInsPrimary As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblInsPrimary.Rows.Count > 0 Then
                For Each row In tblInsPrimary.Rows
                    If row("coverage_type") = 1 Then
                        If IsDBNull(row("ins_company_name")) Then
                        Else
                            lblPrimaryInsuranceProvider.Text = row("ins_company_name")
                        End If
                        txtInsured_name_last.Text = row("Insured_name_last")
                        txtInsured_name_first.Text = row("Insured_name_first")
                        txtInsured_name_mid.Text = row("Insured_name_mid")
                        txtInsured_name_suffx.Text = row("Insured_name_suffx")
                        txtInsured_addr_street.Text = row("Insured_addr_street")
                        txtInsured_addr_city.Text = row("Insured_addr_city")
                        txtInsured_addr_state.Text = row("Insured_addr_state")
                        txtInsured_addr_zip.Text = row("Insured_addr_zip")
                        dteInsured_Date_Birth.Text = IIf(IsDBNull(row("Insured_Date_Birth")), "", row("Insured_Date_Birth"))
                        ddlInsured_Gender.SelectedValue = row("Insured_Gender")
                        ddlRelationship.SelectedValue = row("Relationship")
                        dteDate_Effective.Text = IIf(IsDBNull(row("Date_Effective")), "", row("Date_Effective"))
                        dteDate_Canceled.Text = IIf(IsDBNull(row("Date_Canceled")), "", row("Date_Canceled"))
                        txtEmployer_Name.Text = row("Employer_Name")
                        txtGroup_Number.Text = row("Group_Number")
                        txtPolicy_Number.Text = row("Policy_Number")
                        dolLifetimeMax.Text = row("lifetimeMax")
                    Else
                        If IsDBNull(row("ins_company_name")) Then
                        Else
                            lblSecondaryInsuranceProvider.Text = row("ins_company_name")
                        End If
                        txtInsured_name_last_sec.Text = row("Insured_name_last")
                        txtInsured_name_first_sec.Text = row("Insured_name_first")
                        txtInsured_name_mid_sec.Text = row("Insured_name_mid")
                        txtInsured_name_suffx_sec.Text = row("Insured_name_suffx")
                        txtInsured_addr_street_sec.Text = row("Insured_addr_street")
                        txtInsured_addr_city_sec.Text = row("Insured_addr_city")
                        txtInsured_addr_state_sec.Text = row("Insured_addr_state")
                        txtInsured_addr_zip_sec.Text = row("Insured_addr_zip")
                        dteInsured_Date_Birth_sec.Text = IIf(IsDBNull(row("Insured_Date_Birth")), "", row("Insured_Date_Birth"))
                        ddlInsured_Gender_sec.SelectedValue = row("Insured_Gender")
                        ddlRelationship_sec.SelectedValue = row("Relationship")
                        dteDate_Effective_sec.Text = IIf(IsDBNull(row("Date_Effective")), "", row("Date_Effective"))
                        dteDate_Canceled_sec.Text = IIf(IsDBNull(row("Date_Canceled")), "", row("Date_Canceled"))
                        txtEmployer_Name_sec.Text = row("Employer_Name")
                        txtGroup_Number_sec.Text = row("Group_Number")
                        txtPolicy_Number_sec.Text = row("Policy_Number")
                        dolLifetimeMax_sec.Text = row("lifetimeMax")
                    End If
                Next
            Else
                ' no insurance data found, default all textboxes
                lblPrimaryInsuranceProvider.Text = ""
                txtInsured_name_last.Text = ""
                txtInsured_name_first.Text = ""
                txtInsured_name_mid.Text = ""
                txtInsured_name_suffx.Text = ""
                txtInsured_addr_street.Text = ""
                txtInsured_addr_city.Text = ""
                txtInsured_addr_state.Text = ""
                txtInsured_addr_zip.Text = ""
                dteInsured_Date_Birth.Text = ""
                ddlInsured_Gender.SelectedValue = 0
                ddlRelationship.SelectedValue = 0
                dteDate_Effective.Text = ""
                dteDate_Canceled.Text = ""
                txtEmployer_Name.Text = ""
                txtGroup_Number.Text = ""
                txtPolicy_Number.Text = ""
                dolLifetimeMax.Text = 0

                lblSecondaryInsuranceProvider.Text = ""
                txtInsured_name_last_sec.Text = ""
                txtInsured_name_first_sec.Text = ""
                txtInsured_name_mid_sec.Text = ""
                txtInsured_name_suffx_sec.Text = ""
                txtInsured_addr_street_sec.Text = ""
                txtInsured_addr_city_sec.Text = ""
                txtInsured_addr_state_sec.Text = ""
                txtInsured_addr_zip_sec.Text = ""
                dteInsured_Date_Birth_sec.Text = ""
                ddlInsured_Gender_sec.SelectedValue = 0
                ddlRelationship_sec.SelectedValue = 0
                dteDate_Effective_sec.Text = ""
                dteDate_Canceled_sec.Text = ""
                txtEmployer_Name_sec.Text = ""
                txtGroup_Number_sec.Text = ""
                txtPolicy_Number_sec.Text = ""
                dolLifetimeMax_sec.Text = 0
            End If

            ' set max length of AddNotes textbox so they cannot exceed database max
            txtAddNotes.MaxLength = 8000 - Len(Trim(txtComments.Text))

            ' 12/22/15 CS If contract status=Closed show end of contract div inputs
            If ddlStatus_vw.SelectedValue = 2 Then
                litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#divEndOfContract"").removeClass(""hidden"");</script>"
            End If
        End If

        ' load page elements to session variables
        Call g_SendSessions(txtSessions)

    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' 10/28 - CFS: create all child tables associated with contract ---
        Dim strSQL As String = ""

        ' get insurance plan_id (when applicable)
        Dim strPlan_Pri As String = ""
        Dim strPlan_Sec As String = ""
        If ddlPrimaryInsurancePlans_vw.SelectedValue > -1 Then
            strSQL = "select plan_id from DropDownList__InsurancePlans where recid = " & ddlPrimaryInsurancePlans_vw.SelectedValue
            Dim tblInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblInsurance.Rows.Count > 0 Then
                strPlan_Pri = tblInsurance.Rows(0)("plan_id")
            End If
        End If
        If ddlSecondaryInsurancePlans_vw.SelectedValue > -1 Then
            strSQL = "select plan_id from DropDownList__InsurancePlans where recid = " & ddlSecondaryInsurancePlans_vw.SelectedValue
            Dim tblInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblInsurance.Rows.Count > 0 Then
                strPlan_Sec = tblInsurance.Rows(0)("plan_id")
            End If
        End If

        ' insert/update patients record
        strSQL = "select recid from patients where chart_number = '" & txtChartNumber.Text & "'"
        Dim tblPat As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblPat.Rows.Count = 0 Then

            ' set variables for any possible NULLS that would cause building insert string to fail
            'Dim strNameMid As String = IIf(IsDBNull(tblPatient.Rows(0)("MiddleName")), "", UCase(CType(tblPatient.Rows(0)("MiddleName"), String)))

            ' build insert statement
            strSQL = "INSERT INTO Patients " &
                "(chart_number" &
                ", plan_pri" &
                ", plan_sec" &
                ", name_last" &
                ", name_first" &
                ", name_mid" &
                ", addr_other" &
                ", addr_street" &
                ", addr_city" &
                ", addr_state" &
                ", addr_zip" &
                ", gender" &
                ", dob)" &
                " VALUES (" &
                "'" & txtChartNumber.Text & "', " &
                "'" & strPlan_Pri & "', " &
                "'" & strPlan_Sec & "', " &
                "'" & UCase(txtname_last.Text).Replace("'", "''") & "', " &
                "'" & UCase(txtname_first.Text).Replace("'", "''") & "', " &
                "'" & UCase(txtname_mid.Text).Replace("'", "''") & "', " &
                "'" & UCase(txtaddr_other.Text).Replace("'", "''") & "', " &
                "'" & UCase(txtaddr_street.Text).Replace("'", "''") & "', " &
                "'" & UCase(txtaddr_city.Text).Replace("'", "''") & "', " &
                "'" & UCase(txtaddr_state.Text) & "', " &
                "'" & txtaddr_zip.Text & "', " &
                "'" & ddlGender.SelectedValue & "', " &
                "'" & dteDOB.Text & "')"

            g_IO_Execute_SQL(strSQL, False)
        Else
            ' update existing patient with any data available to change by user
            strSQL = "UPDATE Patients Set " &
                "plan_pri='" & strPlan_Pri & "'" &
                ", plan_sec='" & strPlan_Sec & "'" &
                ", name_last='" & UCase(txtname_last.Text).Replace("'", "''") & "'" &
                ", name_first='" & UCase(txtname_first.Text).Replace("'", "''") & "'" &
                ", name_mid='" & UCase(txtname_mid.Text).Replace("'", "''") & "'" &
                ", addr_other='" & UCase(txtaddr_other.Text).Replace("'", "''") & "'" &
                ", addr_street='" & UCase(txtaddr_street.Text).Replace("'", "''") & "'" &
                ", addr_city='" & UCase(txtaddr_city.Text).Replace("'", "''") & "'" &
                ", addr_state='" & UCase(txtaddr_state.Text) & "'" &
                ", addr_zip='" & txtaddr_zip.Text & "'" &
                ", gender=" & ddlGender.SelectedValue &
                ", dob='" & dteDOB.Text & "')" &
                " where chart_number = '" & txtChartNumber.Text & "'"
            g_IO_Execute_SQL(strSQL, False)
        End If

        ' create patient insurance record (data in modal popup- not part of contract)
        If ddlPrimaryInsurancePlans_vw.SelectedValue > -1 Then
            CreateInsuranceRecord("1")
        End If
        If ddlSecondaryInsurancePlans_vw.SelectedValue > -1 Then
            CreateInsuranceRecord("2")
        End If
        '--- End of child table updates ---

        ' get next account id
        ' NOTE: this has to be done before g_loadPageToSessions so that it picks up this textbox auto-changed value
        Dim intNewAccountID As Integer = -1
        strSQL = "select top 1 account_id from contracts order by account_id desc"
        Dim tblAccounts As DataTable = g_IO_Execute_SQL(strSQL, False)
        intNewAccountID = CType(tblAccounts.Rows(0)("account_id"), Integer) + 1
        txtAccount_ID.Text = CType(intNewAccountID, String)

        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & IIf(IsNothing(System.Web.HttpContext.Current.Session(m_strPrimaryKey)), -1, System.Web.HttpContext.Current.Session(m_strPrimaryKey))

        'post contract data to table
        Call g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, System.Web.HttpContext.Current.Session("user_link_id"))

        'Get New Contract Recid
        Dim intContractRecid As Integer = g_IO_GetLastRecId()

        Call g_SendSessions(txtSessions)

        ' reload form w/ no querystring to enter a new contract immediately
        Response.Redirect("ContractEntry.aspx?cid=" & intContractRecid & "&mid=view")

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' 10/28 - CFS: Update all child tables associated with contract ---
        Dim strSQL As String = ""
        Dim strPlan_Pri As String = ""
        Dim strPlan_Sec As String = ""
        If ddlPrimaryInsurancePlans_vw.SelectedValue > -1 Then
            strSQL = "select plan_id from DropDownList__InsurancePlans where recid = " & ddlPrimaryInsurancePlans_vw.SelectedValue
            Dim tblInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblInsurance.Rows.Count > 0 Then
                strPlan_Pri = tblInsurance.Rows(0)("plan_id")
            End If
        End If
        If ddlSecondaryInsurancePlans_vw.SelectedValue > -1 Then
            strSQL = "select plan_id from DropDownList__InsurancePlans where recid = " & ddlSecondaryInsurancePlans_vw.SelectedValue
            Dim tblInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblInsurance.Rows.Count > 0 Then
                strPlan_Sec = tblInsurance.Rows(0)("plan_id")
            End If
        End If

        ' insert or update primary insurance record
        If ddlPrimaryInsurancePlans_vw.SelectedValue > -1 Then
            strSQL = "select * from patient_insurance where chart_number = '" & txtChartNumber.Text & "' and coverage_type = '1'"
            Dim tblPatInsPrim As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblPatInsPrim.Rows.Count > 0 Then
                strSQL = "Update Patient_Insurance " &
                    "set insured_name_last = '" & UCase(txtInsured_name_last.Text).Replace("'", "''") & "', " &
                    "insured_name_first = '" & UCase(txtInsured_name_first.Text).Replace("'", "''") & "', " &
                    "insured_name_mid = '" & UCase(txtInsured_name_mid.Text).Replace("'", "''") & "', " &
                    "insured_name_suffx = '" & UCase(txtInsured_name_suffx.Text).Replace("'", "''") & "', " &
                    "insured_addr_street = '" & UCase(txtInsured_addr_street.Text).Replace("'", "''") & "', " &
                    "insured_addr_city = '" & UCase(txtInsured_addr_city.Text).Replace("'", "''") & "', " &
                    "insured_addr_state = '" & UCase(txtInsured_addr_state.Text).Replace("'", "''") & "', " &
                    "insured_addr_zip = '" & txtInsured_addr_zip.Text & "', " &
                    "insured_date_birth = " & IIf(IsDate(dteInsured_Date_Birth.Text), "'" & dteInsured_Date_Birth.Text & "'", "NULL") & ", " &
                    "insured_gender = '" & ddlInsured_Gender.SelectedValue & "', " &
                    "relationship = '" & ddlRelationship.SelectedValue & "', " &
                    "plan_id = '" & strPlan_Pri & "', " &
                    "date_effective = " & IIf(IsDate(dteDate_Effective.Text), "'" & dteDate_Effective.Text & "'", "NULL") & ", " &
                    "date_canceled = " & IIf(IsDate(dteDate_Canceled.Text), "'" & dteDate_Canceled.Text & "'", "NULL") & ", " &
                    "employer_name = '" & UCase(txtEmployer_Name.Text).Replace("'", "''") & "', " &
                    "group_number = '" & UCase(txtGroup_Number.Text).Replace("'", "''") & "', " &
                    "policy_number = '" & UCase(txtPolicy_Number.Text).Replace("'", "''") & "', " &
                    "lifetimeMax = " & dolLifetimeMax.Text &
                    " where chart_number = '" & txtChartNumber.Text & "' and coverage_type = '1'"

                g_IO_Execute_SQL(strSQL, False)
            Else
                ' create new record 
                CreateInsuranceRecord("1")
            End If
        End If

        If ddlSecondaryInsurancePlans_vw.SelectedValue > -1 Then
            strSQL = "select * from patient_insurance where chart_number = '" & txtChartNumber.Text & "' and coverage_type = '2'"
            Dim tblPatInsSec As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblPatInsSec.Rows.Count > 0 Then
                strSQL = "Update Patient_Insurance " &
                    "set insured_name_last = '" & UCase(txtInsured_name_last_sec.Text).Replace("'", "''") & "', " &
                    "insured_name_first = '" & UCase(txtInsured_name_first_sec.Text).Replace("'", "''") & "', " &
                    "insured_name_mid = '" & UCase(txtInsured_name_mid_sec.Text).Replace("'", "''") & "', " &
                    "insured_name_suffx = '" & UCase(txtInsured_name_suffx_sec.Text).Replace("'", "''") & "', " &
                    "insured_addr_street = '" & UCase(txtInsured_addr_street_sec.Text).Replace("'", "''") & "', " &
                    "insured_addr_city = '" & UCase(txtInsured_addr_city_sec.Text).Replace("'", "''") & "', " &
                    "insured_addr_state = '" & UCase(txtInsured_addr_state_sec.Text).Replace("'", "''") & "', " &
                    "insured_addr_zip = '" & txtInsured_addr_zip_sec.Text & "', " &
                    "insured_date_birth = " & IIf(IsDate(dteInsured_Date_Birth_sec.Text), "'" & dteInsured_Date_Birth_sec.Text & "'", "NULL") & ", " &
                    "insured_gender = '" & ddlInsured_Gender_sec.SelectedValue & "', " &
                    "relationship = '" & ddlRelationship_sec.SelectedValue & "', " &
                    "plan_id = '" & strPlan_Sec & "', " &
                    "date_effective = " & IIf(IsDate(dteDate_Effective_sec.Text), "'" & dteDate_Effective_sec.Text & "'", "NULL") & ", " &
                    "date_canceled = " & IIf(IsDate(dteDate_Effective_sec.Text), "'" & dteDate_Effective_sec.Text & "'", "NULL") & ", " &
                    "employer_name = '" & UCase(txtEmployer_Name_sec.Text).Replace("'", "''") & "', " &
                    "group_number = '" & UCase(txtGroup_Number_sec.Text).Replace("'", "''") & "', " &
                    "policy_number = '" & UCase(txtPolicy_Number_sec.Text).Replace("'", "''") & "', " &
                    "lifetimeMax = " & dolLifetimeMax_sec.Text &
                    " where chart_number = '" & txtChartNumber.Text & "' and coverage_type = '2'"

                g_IO_Execute_SQL(strSQL, False)
            Else
                ' create new record
                CreateInsuranceRecord("2")
            End If
        End If

        ' update patient master record
        strSQL = "update Patients set " &
            "plan_pri = '" & strPlan_Pri & "'" &
            ", plan_sec = '" & strPlan_Sec & "'" &
            ", name_last = '" & UCase(txtname_last.Text).Replace("'", "''") & "'" &
            ", name_first = '" & UCase(txtname_first.Text).Replace("'", "''") & "'" &
            ", name_mid = '" & UCase(txtname_mid.Text).Replace("'", "''") & "'" &
            ", addr_other = '" & UCase(txtaddr_other.Text).Replace("'", "''") & "'" &
            ", addr_street = '" & UCase(txtaddr_street.Text).Replace("'", "''") & "'" &
            ", addr_city = '" & UCase(txtaddr_city.Text).Replace("'", "''") & "'" &
            ", addr_state = '" & UCase(txtaddr_state.Text).Replace("'", "''") & "'" &
            ", addr_zip = '" & txtaddr_zip.Text & "'" &
            ", gender = '" & ddlGender.SelectedValue & "'" &
            ", dob = '" & Format(CType(dteDOB.Text, Date), "yyyy-MM-dd") & "'" &
            " where chart_number = '" & txtChartNumber.Text & "'"

        g_IO_Execute_SQL(strSQL, False)
        '--- End of child table updates ---

        Dim cID As String = txtRecID.Text
        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & cID

        ' saving data so we are just going to update the record 
        Call g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, Session("P_SysUserRECID"))
        Call g_SendSessions(txtSessions)

        Response.Redirect("ContractEntry.aspx?cid=" & cID & "&mid=view")
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        ' code to delete this contract if no history exists for it...
        Dim strSQL As String = "select * from Payments where contract_recid = '" & txtRecID.Text & "'"
        Dim tblContractHistory As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblContractHistory.Rows.Count > 0 Then
            ' historical data found, cannot delete this contract
            lblMessage.Text = "<script>alert('This contract cannot be deleted.  Historical data found.');</script>"
        Else
            ' delete contract and redirect to contract listing
            strSQL = "delete from Contracts where recid = '" & txtRecID.Text & "'"
            g_IO_Execute_SQL(strSQL, False)
            Response.Redirect("frmListmanager.aspx?id=Contracts_vw&add=ContractEntry.aspx")
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Response.Redirect("ContractEntry.aspx?cid=" & txtRecID.Text)
    End Sub

    Private Sub btnPaymentPosting_Click(sender As Object, e As EventArgs) Handles btnPaymentPosting.Click
        Response.Redirect("PaymentPosting.aspx?cid=" & txtRecID.Text & "&cno=" & txtChartNumber.Text)
    End Sub

    Private Sub btnContractStatus_Click(sender As Object, e As EventArgs) Handles btnContractStatus.Click
        Response.Redirect("ContractStatus.aspx?cid=" & txtChartNumber.Text)
        'Response.Redirect("ContractStatus.aspx")
    End Sub

    Private Sub btnPaymentHistory_Click(sender As Object, e As EventArgs) Handles btnPaymentHistory.Click
        Response.Redirect("frmListmanager.aspx?id=Payments_vw&wsn=chartNumber = '" & txtRecID.Text & "'&add=PaymentPosting.aspx")
    End Sub

    Private Sub btnCancelNew_Click(sender As Object, e As EventArgs) Handles btnCancelNew.Click
        Response.Redirect("frmListmanager.aspx?id=Contracts_vw&add=ContractEntry.aspx")
    End Sub

    Private Sub btnGenerateClaim_Click(sender As Object, e As EventArgs) Handles btnGenerateClaim.Click
        Response.Redirect("frmClaimsProcessing.aspx?rl=" & txtRecID.Text)
    End Sub

    Private Sub btnAddNote_Click(sender As Object, e As EventArgs) Handles btnAddNote.Click

        ' add notes entered in modal to db textbox w/ notes that already exists
        txtComments.Text = Format(Date.Now, "MM/dd/yyyy") & " " & Session("user_name") & ": " & txtAddNotes.Text & vbCrLf & vbCrLf & txtComments.Text

        Dim cID As String = txtRecID.Text
        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & cID

        ' saving data so we are just going to update the record 
        Call g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, Session("P_SysUserRECID"))
        Call g_SendSessions(txtSessions)

        Response.Redirect("ContractEntry.aspx?cid=" & cID & "&mid=view")
    End Sub

    Private Sub CreateInsuranceRecord(ByRef strCoverageType As String)
        Dim strSQL As String = ""
        If strCoverageType = "1" Then
            Dim strPlanID As String = "-1"
            strSQL = "select plan_id from DropDownList__InsurancePlans where recid = " & ddlPrimaryInsurancePlans_vw.SelectedValue
            Dim tblInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblInsurance.Rows.Count > 0 Then
                strPlanID = tblInsurance.Rows(0)("plan_id")
                Dim strDteCanceled As String = "NULL"
                If IsDate(dteDate_Canceled.Text) Then
                    strDteCanceled = "'" & dteDate_Canceled.Text & "'"
                End If
                ' make sure patient_insurance record does not already exist
                strSQL = "select * from patient_insurance where chart_number = '" & txtChartNumber.Text & "' and coverage_type = '1'"
                Dim tblPatIns As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblPatIns.Rows.Count = 0 Then
                    strSQL = "INSERT INTO Patient_Insurance (" &
                        "chart_number" &
                        ", insured_name_last" &
                        ", insured_name_first" &
                        ", insured_name_mid" &
                        ", insured_name_suffx" &
                        ", insured_addr_street" &
                        ", insured_addr_city" &
                        ", insured_addr_state" &
                        ", insured_addr_zip" &
                        ", insured_date_birth" &
                        ", insured_gender" &
                        ", relationship" &
                        ", coverage_type " &
                        ", plan_id" &
                        ", date_effective" &
                        ", date_canceled" &
                        ", employer_name" &
                        ", group_number" &
                        ", policy_number" &
                        ", lifetimeMax)" &
                        " VALUES (" &
                        "'" & txtChartNumber.Text & "', " &
                        "'" & UCase(txtInsured_name_last.Text).Replace("'", "''") & "', " &
                        "'" & UCase(txtInsured_name_first.Text).Replace("'", "''") & "', " &
                        "'" & UCase(txtInsured_name_mid.Text).Replace("'", "''") & "', " &
                        "'" & UCase(txtInsured_name_suffx.Text).Replace("'", "''") & "', " &
                        "'" & UCase(txtInsured_addr_street.Text).Replace("'", "''") & "', " &
                        "'" & UCase(txtInsured_addr_city.Text).Replace("'", "''") & "', " &
                        "'" & UCase(txtInsured_addr_state.Text) & "', " &
                        "'" & txtInsured_addr_zip.Text & "', " &
                        IIf(IsDate(dteInsured_Date_Birth.Text), "'" & dteInsured_Date_Birth.Text & "'", "NULL") & ", " &
                        "'" & ddlInsured_Gender.SelectedValue & "', " &
                        "'" & ddlRelationship.SelectedValue & "', " &
                        "'1', " &
                        "'" & strPlanID & "', " &
                        IIf(IsDate(dteDate_Effective.Text), "'" & dteDate_Effective.Text & "'", "NULL") & ", " &
                        IIf(IsDate(dteDate_Canceled.Text), "'" & dteDate_Canceled.Text & "'", "NULL") & ", " &
                        "'" & UCase(txtEmployer_Name.Text).Replace("'", "''") & "', " &
                        "'" & UCase(txtGroup_Number.Text) & "', " &
                        "'" & UCase(txtPolicy_Number.Text) & "', " &
                        "'" & dolLifetimeMax.Text & "')"
                    g_IO_Execute_SQL(strSQL, False)
                Else
                    strSQL = "update Patient_Insurance set " &
                        "chart_number = '" & txtChartNumber.Text & "', " &
                        "insured_name_last = '" & UCase(txtInsured_name_last.Text).Replace("'", "''") & "', " &
                        "insured_name_first = '" & UCase(txtInsured_name_first.Text).Replace("'", "''") & "', " &
                        "insured_name_mid = '" & UCase(txtInsured_name_mid.Text).Replace("'", "''") & "', " &
                        "insured_name_suffx = '" & UCase(txtInsured_name_suffx.Text).Replace("'", "''") & "', " &
                        "insured_addr_street = '" & UCase(txtInsured_addr_street.Text).Replace("'", "''") & "', " &
                        "insured_addr_city = '" & UCase(txtInsured_addr_city.Text).Replace("'", "''") & "', " &
                        "insured_addr_state = '" & UCase(txtInsured_addr_state.Text) & "', " &
                        "insured_addr_zip = '" & txtInsured_addr_zip.Text & "', " &
                        "insured_date_birth = '" & IIf(IsDate(dteInsured_Date_Birth.Text), "'" & dteInsured_Date_Birth.Text & "'", "NULL") & ", " &
                        "insured_gender = '" & ddlInsured_Gender.SelectedValue & "', " &
                        "relationship = '" & ddlRelationship.SelectedValue & "', " &
                        "coverage_type = '1', " &
                        "plan_id = '" & strPlanID & "', " &
                        "date_effective = '" & IIf(IsDate(dteDate_Effective.Text), "'" & dteDate_Effective.Text & "'", "NULL") & ", " &
                        "date_canceled = '" & IIf(IsDate(dteDate_Canceled.Text), "'" & dteDate_Canceled.Text & "'", "NULL") & ", " &
                        "employer_name = '" & UCase(txtEmployer_Name.Text).Replace("'", "''") & "', " &
                        "group_number = '" & UCase(txtGroup_Number.Text) & "', " &
                        "policy_number = '" & UCase(txtPolicy_Number.Text) & "', " &
                        "lifetimeMax = '" & dolLifetimeMax.Text & "'" &
                        " where chart_number = '" & txtChartNumber.Text & "' coverage_type = '1'"
                    g_IO_Execute_SQL(strSQL, False)
                End If
            End If
        Else
            Dim strPlanID As String = "-1"
            strSQL = "select plan_id from DropDownList__InsurancePlans where recid = " & ddlSecondaryInsurancePlans_vw.SelectedValue
            Dim tblInsurance As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblInsurance.Rows.Count > 0 Then
                strPlanID = tblInsurance.Rows(0)("plan_id")
                Dim strDteCanceled As String = "NULL"
                If IsDate(dteDate_Canceled_sec.Text) Then
                    strDteCanceled = "'" & dteDate_Canceled_sec.Text & "'"
                End If
                ' make sure patient_insurance record does not already exist
                strSQL = "select * from patient_insurance where chart_number = '" & txtChartNumber.Text & "' and coverage_type = '2'"
                Dim tblPatIns As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblPatIns.Rows.Count = 0 Then
                    strSQL = "INSERT INTO Patient_Insurance (" &
                            "chart_number" &
                            ", insured_name_last" &
                            ", insured_name_first" &
                            ", insured_name_mid" &
                            ", insured_name_suffx" &
                            ", insured_addr_street" &
                            ", insured_addr_city" &
                            ", insured_addr_state" &
                            ", insured_addr_zip" &
                            ", insured_date_birth" &
                            ", insured_gender" &
                            ", relationship" &
                            ", coverage_type " &
                            ", plan_id" &
                            ", date_effective" &
                            ", date_canceled" &
                            ", employer_name" &
                            ", group_number" &
                            ", policy_number" &
                            ", lifetimeMax)" &
                            " VALUES (" &
                            "'" & txtChartNumber.Text & "', " &
                            "'" & UCase(txtInsured_name_last_sec.Text).Replace("'", "''") & "', " &
                            "'" & UCase(txtInsured_name_first_sec.Text).Replace("'", "''") & "', " &
                            "'" & UCase(txtInsured_name_mid_sec.Text).Replace("'", "''") & "', " &
                            "'" & UCase(txtInsured_name_suffx_sec.Text).Replace("'", "''") & "', " &
                            "'" & UCase(txtInsured_addr_street_sec.Text).Replace("'", "''") & "', " &
                            "'" & UCase(txtInsured_addr_city_sec.Text).Replace("'", "''") & "', " &
                            "'" & UCase(txtInsured_addr_state_sec.Text) & "', " &
                            "'" & txtInsured_addr_zip_sec.Text & "', " &
                            IIf(IsDate(dteInsured_Date_Birth_sec.Text), "'" & dteInsured_Date_Birth_sec.Text & "'", "NULL") & ", " &
                            "'" & ddlInsured_Gender_sec.SelectedValue & "', " &
                            "'" & ddlRelationship_sec.SelectedValue & "', " &
                            "'2', " &
                            "'" & tblInsurance.Rows(0)("plan_id") & "', " &
                            IIf(IsDate(dteDate_Effective_sec.Text), "'" & dteDate_Effective_sec.Text & "'", "NULL") & ", " &
                            IIf(IsDate(dteDate_Canceled_sec.Text), "'" & dteDate_Canceled_sec.Text & "'", "NULL") & ", " &
                            "'" & UCase(txtEmployer_Name_sec.Text).Replace("'", "''") & "', " &
                            "'" & UCase(txtGroup_Number_sec.Text) & "', " &
                            "'" & UCase(txtPolicy_Number_sec.Text) & "', " &
                            "'" & dolLifetimeMax_sec.Text & "')"
                    g_IO_Execute_SQL(strSQL, False)
                Else
                    strSQL = "update Patient_Insurance set " &
                        "chart_number = '" & txtChartNumber.Text & "', " &
                        "insured_name_last = '" & UCase(txtInsured_name_last_sec.Text).Replace("'", "''") & "', " &
                        "insured_name_first = '" & UCase(txtInsured_name_first_sec.Text).Replace("'", "''") & "', " &
                        "insured_name_mid = '" & UCase(txtInsured_name_mid_sec.Text).Replace("'", "''") & "', " &
                        "insured_name_suffx = '" & UCase(txtInsured_name_suffx_sec.Text).Replace("'", "''") & "', " &
                        "insured_addr_street = '" & UCase(txtInsured_addr_street_sec.Text).Replace("'", "''") & "', " &
                        "insured_addr_city = '" & UCase(txtInsured_addr_city_sec.Text).Replace("'", "''") & "', " &
                        "insured_addr_state = '" & UCase(txtInsured_addr_state_sec.Text) & "', " &
                        "insured_addr_zip = '" & txtInsured_addr_zip_sec.Text & "', " &
                        "insured_date_birth = '" & IIf(IsDate(dteInsured_Date_Birth_sec.Text), "'" & dteInsured_Date_Birth_sec.Text & "'", "NULL") & ", " &
                        "insured_gender = '" & ddlInsured_Gender_sec.SelectedValue & "', " &
                        "relationship = '" & ddlRelationship_sec.SelectedValue & "', " &
                        "coverage_type = '2', " &
                        "plan_id = '" & strPlanID & "', " &
                        "date_effective = '" & IIf(IsDate(dteDate_Effective_sec.Text), "'" & dteDate_Effective_sec.Text & "'", "NULL") & ", " &
                        "date_canceled = '" & IIf(IsDate(dteDate_Canceled_sec.Text), "'" & dteDate_Canceled_sec.Text & "'", "NULL") & ", " &
                        "employer_name = '" & UCase(txtEmployer_Name_sec.Text).Replace("'", "''") & "', " &
                        "group_number = '" & UCase(txtGroup_Number_sec.Text) & "', " &
                        "policy_number = '" & UCase(txtPolicy_Number_sec.Text) & "', " &
                        "lifetimeMax = '" & dolLifetimeMax_sec.Text & "'" &
                        " where chart_number = '" & txtChartNumber.Text & "' coverage_type = '2'"
                    g_IO_Execute_SQL(strSQL, False)
                End If
            End If
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
End Class