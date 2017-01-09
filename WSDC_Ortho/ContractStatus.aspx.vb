Public Class ContractStatus
    Inherits System.Web.UI.Page

    Dim m_strTableName As String = ""
    Dim m_strFormCode As String = ""
    Dim m_strPrimaryKey As String = " recid "
    Dim m_strPreviousFormCode As String = ""
    Dim m_strContentAreaName As String = "MainContent"
    Dim m_cphContentHolder As ContentPlaceHolder = Nothing

    Dim m_nvcColumnType As New NameValueCollection
    Dim m_nvcColumnLength As New NameValueCollection
    Dim m_nvcColumnDescription As New NameValueCollection
    Dim m_nvcColumnDDLTableName As New NameValueCollection
    Dim m_nvcColumnDDLValue As New NameValueCollection
    Dim m_nvcColumnDDLText As New NameValueCollection
    Dim m_nvcColumnPwd As New NameValueCollection
    Dim m_nvcColumnShowInGrid As New NameValueCollection
    Dim m_nvcHidden As New NameValueCollection
    Dim m_nvcDisabled As New NameValueCollection


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        'Always needs to be called first
        Call g_RetrieveSessions(txtSessions)

        ' default tabs on -- if table sent via 'id' then not using tabs
        Dim blnShowTabs As Boolean = True

        If IsNothing(Request.QueryString("cid")) Then
            System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = -1
            litPatientHeader.Text = "Please use search options below to view contract status"
        Else
            System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = Request.QueryString("cid")
            txtChartNumSearch.Text = Request.QueryString("cid")
            litScripts.Text = "<script>jQuery(document).ready(function () {jQuery('#" & btnSearch.ClientID & "').click();});</script>"
            litPatientHeader.Text = ""
        End If

        If IsPostBack Then
        Else
            'Initial
            lblInitialPatAmount.Text = ""
            lblInitialPrimaryAmount.Text = ""
            lblInitialSecondaryAmount.Text = ""
            'Contract
            lblContractMonths.Text = ""
            lblContractPatAmount.Text = ""
            lblContractPrimaryAmount.Text = ""
            lblContractSecondaryAmount.Text = ""
            'Billed
            lblBilledMonthsPatient.Text = ""
            lblBilledPatAmount.Text = ""
            lblBilledMonthsPrimary.Text = ""
            lblBilledPrimaryAmount.Text = ""
            lblBilledMonthsSecondary.Text = ""
            lblBilledSecondaryAmount.Text = ""
            'Remaining
            lblRemainingMonths.Text = ""
            lblRemainingPatAmount.Text = ""
            lblRemainingPrimaryAmount.Text = ""
            lblRemainingSecondaryAmount.Text = ""
            'Credit
            lblCreditMonths.Text = ""
            lblCreditPatAmount.Text = ""
            lblCreditPrimaryAmount.Text = ""
            lblCreditSecondaryAmount.Text = ""
            'Last Post
            lblPatientLastPostDate.Text = ""
            lblPrimaryLastPostDate.Text = ""
            lblSecondaryLastPostDate.Text = ""

            'Aging Balances
            lblAccountBalance.Text = ""
            lblAccount0to30.Text = ""
            lblAccount31to60.Text = ""
            lblAccount61to90.Text = ""
            lblAccountOver90.Text = ""

            lblInsuranceBalance.Text = ""
            lblInsurance0to30.Text = ""
            lblInsurance31to60.Text = ""
            lblInsurance61to90.Text = ""
            lblInsuranceOver90.Text = ""

            lblPatientBalance.Text = ""
            lblPatient0to30.Text = ""
            lblPatient31to60.Text = ""
            lblPatient61to90.Text = ""
            lblPatientOver90.Text = ""

            lblUndistBalance.Text = ""
            lblUndist0to30.Text = ""
            lblUndist31to60.Text = ""
            lblUndist61to90.Text = ""
            lblUndistOver90.Text = ""

            lblPayoffBalance.Text = ""
        End If

        Call g_SendSessions(txtSessions)


    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        ' clear existing labels
        'Initial
        lblInitialPatAmount.Text = ""
        lblInitialPrimaryAmount.Text = ""
        lblInitialSecondaryAmount.Text = ""
        'Contract
        lblContractMonths.Text = ""
        lblContractPatAmount.Text = ""
        lblContractPrimaryAmount.Text = ""
        lblContractSecondaryAmount.Text = ""
        'Billed
        lblBilledMonthsPatient.Text = ""
        lblBilledPatAmount.Text = ""
        lblBilledMonthsPrimary.Text = ""
        lblBilledPrimaryAmount.Text = ""
        lblBilledMonthsSecondary.Text = ""
        lblBilledSecondaryAmount.Text = ""
        'Remaining
        lblRemainingMonths.Text = ""
        lblRemainingPatAmount.Text = ""
        lblRemainingPrimaryAmount.Text = ""
        lblRemainingSecondaryAmount.Text = ""
        'Credit
        lblCreditMonths.Text = ""
        lblCreditPatAmount.Text = ""
        lblCreditPrimaryAmount.Text = ""
        lblCreditSecondaryAmount.Text = ""
        'Last Post
        lblPatientLastPostDate.Text = ""
        lblPrimaryLastPostDate.Text = ""
        lblSecondaryLastPostDate.Text = ""

        'Aging Balances
        lblAccountBalance.Text = ""
        lblAccount0to30.Text = ""
        lblAccount31to60.Text = ""
        lblAccount61to90.Text = ""
        lblAccountOver90.Text = ""

        lblInsuranceBalance.Text = ""
        lblInsurance0to30.Text = ""
        lblInsurance31to60.Text = ""
        lblInsurance61to90.Text = ""
        lblInsuranceOver90.Text = ""

        lblPatientBalance.Text = ""
        lblPatient0to30.Text = ""
        lblPatient31to60.Text = ""
        lblPatient61to90.Text = ""
        lblPatientOver90.Text = ""

        lblUndistBalance.Text = ""
        lblUndist0to30.Text = ""
        lblUndist31to60.Text = ""
        lblUndist61to90.Text = ""
        lblUndistOver90.Text = ""

        lblPayoffBalance.Text = ""

        ' search for name or chart # entered
        Dim strSearch As String = ""
        If txtAccountNumSearch.Text <> "" Then
            strSearch = "select *, (FirstName + ' ' + LastName) as PatientName From ContractStatus_vw where Account_Id = '" & txtAccountNumSearch.Text & "'"
        ElseIf txtChartNumSearch.Text <> "" Then
            strSearch = "select *, (FirstName + ' ' + LastName) as PatientName From ContractStatus_vw where ChartNumber = '" & txtChartNumSearch.Text & "'"
        ElseIf txtLastNameSearch.Text <> "" Then
            strSearch = "select *, (FirstName + ' ' + LastName) as PatientName From ContractStatus_vw where LastName like '" & txtLastNameSearch.Text & "%'"
        Else
        End If

        Dim tblCurrentData As DataTable = g_IO_Execute_SQL(strSearch, False)
        If IsNothing(tblCurrentData) Then
            litPatientHeader.Text = "<span style=""color:red;"">Contract not found. Please review search criteria.</span> "
            'Need to clear litScripts so modal does not keep popping up everytime the page is read
            litScripts.Text = ""
            litOptions.Text = ""
            'clear textboxes
            txtChartNumSearch.Text = ""
            txtAccountNumSearch.Text = ""
            txtLastNameSearch.Text = ""
        Else
            If tblCurrentData.Rows.Count = 0 Then
                litPatientHeader.Text = "<span style=""color:red;""> Contract not found. Please review search criteria. </span> "
                'Need to clear litScripts so modal does not keep popping up everytime the page is read
                litScripts.Text = ""
                litOptions.Text = ""
                'clear textboxes
                txtChartNumSearch.Text = ""
                txtAccountNumSearch.Text = ""
                txtLastNameSearch.Text = ""
            ElseIf tblCurrentData.Rows.Count > 1 Then
                litOptions.Text = ""
                For Each row In tblCurrentData.Rows
                    litOptions.Text &= "<option value = """ & row("Account_Id") & """>" & row("FirstName") & " " & row("LastName") & " - Chart # " & row("ChartNumber") & " | Account # " & row("Account_Id") & " | " & row("ContractDate") & "</option>"
                Next
                'More than one patient was found, prompt user to select correct patient
                litScripts.Text = "<script>" & _
                    "jQuery("".modal-title"")[0].innerHTML = ""More than 1 patient found"";" & _
                        "jQuery('#divModalContractSelection').modal('show');" & _
                        "jQuery('#loading_indicator').show();" & _
                        "jQuery('#loading_indicator').hide();</script>"
                Exit Sub
            ElseIf tblCurrentData.Rows.Count = 1 Then
                'Need to clear litScripts so modal does not keep popping up everytime the page is read
                litScripts.Text = ""
                litOptions.Text = ""
                'load data to labels
                ' 11/18/16 CS Only need to show name in this label, as the textboxes below it get filled with additional info (it was repetitive w/ chart # up here too).
                litPatientHeader.Text = " <span style=""color:#2d6ca2;""> " & tblCurrentData.Rows(0)("PatientName") & "</span>"
                'load textboxes
                txtChartNumSearch.Text = tblCurrentData.Rows(0)("ChartNumber")
                txtAccountNumSearch.Text = tblCurrentData.Rows(0)("Account_Id")
                txtLastNameSearch.Text = tblCurrentData.Rows(0)("LastName")

                'Initial
                lblInitialPatAmount.Text = Math.Round(tblCurrentData.Rows(0)("PatientInitialPayment"), 2)
                lblInitialPrimaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("PrimaryInitialPayment"), 2)
                lblInitialSecondaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("SecondaryInitialPayment"), 2)
                'Contract
                lblContractMonths.Text = tblCurrentData.Rows(0)("PatientContractMonths")
                lblContractPatAmount.Text = Math.Round(tblCurrentData.Rows(0)("PatientInitialBalance"), 2)
                lblContractPrimaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("PrimaryCoverageAmt"), 2)
                lblContractSecondaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("SecondaryCoverageAmt"), 2)
                'Billed
                lblBilledMonthsPatient.Text = tblCurrentData.Rows(0)("PatientBilledMonths")
                lblBilledPatAmount.Text = Math.Round(tblCurrentData.Rows(0)("PatientBilledAmt"), 2)
                lblBilledMonthsPrimary.Text = tblCurrentData.Rows(0)("PrimaryBilledMonths")
                lblBilledPrimaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("PrimaryBilledAmt"), 2)
                lblBilledMonthsSecondary.Text = tblCurrentData.Rows(0)("SecondaryBilledMonths")
                lblBilledSecondaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("SecondaryBilledAmt"), 2)
                'Remaining
                lblRemainingMonths.Text = tblCurrentData.Rows(0)("PatientRemainingMonths")
                lblRemainingPatAmount.Text = Math.Round(tblCurrentData.Rows(0)("PatientRemainingAmt"), 2)
                lblRemainingPrimaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("PrimaryRemainingAmt"), 2)
                lblRemainingSecondaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("SecondaryRemainingAmt"), 2)
                'Credit
                lblCreditMonths.Text = tblCurrentData.Rows(0)("PatientCreditMonths")
                lblCreditPatAmount.Text = Math.Round(tblCurrentData.Rows(0)("PatientCreditAmt"), 2)
                lblCreditPrimaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("PrimaryCreditAmt"), 2)
                lblCreditSecondaryAmount.Text = Math.Round(tblCurrentData.Rows(0)("SecondaryCreditAmt"), 2)
                If lblCreditPatAmount.Text = "0.00" Then
                    lblCreditPatAmount.Text = ""
                End If
                If lblCreditPrimaryAmount.Text = "0.00" Then
                    lblCreditPrimaryAmount.Text = ""
                End If
                If lblCreditSecondaryAmount.Text = "0.00" Then
                    lblCreditSecondaryAmount.Text = ""
                End If
                'Last Post
                lblPatientLastPostDate.Text = tblCurrentData.Rows(0)("PatientLastPostDate")
                lblPrimaryLastPostDate.Text = tblCurrentData.Rows(0)("PrimaryLastPostDate")
                lblSecondaryLastPostDate.Text = tblCurrentData.Rows(0)("SecondaryLastPostDate")

            End If
        End If

        'Aging Balances
        If txtChartNumSearch.Text <> "" Then

            strSearch = "select * From ContractAging_vw where ChartNumber = '" & txtChartNumSearch.Text & "'"

            Dim tblAgingData As DataTable = g_IO_Execute_SQL(strSearch, False)

            If tblAgingData.Rows.Count = 0 Then

            Else
                'load data to labels
                lblAccountBalance.Text = ""
                lblAccount0to30.Text = ""
                lblAccount31to60.Text = ""
                lblAccount61to90.Text = ""
                lblAccountOver90.Text = ""

                lblInsuranceBalance.Text = tblAgingData.Rows(0)("InsuranceOpenBalance")
                lblInsurance0to30.Text = tblAgingData.Rows(0)("Insurance0to30")
                lblInsurance31to60.Text = tblAgingData.Rows(0)("InsurancePastDue30")
                lblInsurance61to90.Text = tblAgingData.Rows(0)("InsurancePastDue60")
                lblInsuranceOver90.Text = tblAgingData.Rows(0)("InsurancePastDue90")

                lblPatientBalance.Text = Math.Round(tblAgingData.Rows(0)("PatientBalance"), 2)
                lblPatient0to30.Text = Math.Round(tblAgingData.Rows(0)("Patient0to30"), 2)
                lblPatient31to60.Text = Math.Round(tblAgingData.Rows(0)("PatientPastDue30"), 2)
                lblPatient61to90.Text = Math.Round(tblAgingData.Rows(0)("PatientPastDue60"), 2)
                lblPatientOver90.Text = Math.Round(tblAgingData.Rows(0)("PatientPastDue90"), 2)

                lblUndistBalance.Text = ""
                lblUndist0to30.Text = ""
                lblUndist31to60.Text = ""
                lblUndist61to90.Text = ""
                lblUndistOver90.Text = ""

                Dim strPayoffPatient As String = CType(Math.Round(tblCurrentData.Rows(0)("PatientRemainingAmt"), 2) + Math.Round(tblAgingData.Rows(0)("PatientBalance"), 2) - Math.Round(tblCurrentData.Rows(0)("PatientCreditAmt"), 2), String)
                Dim strPayoffInsurance As String = CType(Math.Round(tblCurrentData.Rows(0)("PrimaryRemainingAmt"), 2) + Math.Round(tblCurrentData.Rows(0)("SecondaryRemainingAmt"), 2) + Math.Round(tblAgingData.Rows(0)("InsuranceOpenBalance"), 2) - Math.Round(tblCurrentData.Rows(0)("PrimaryCreditAmt"), 2) - Math.Round(tblCurrentData.Rows(0)("SecondaryCreditAmt"), 2), String)
                Dim strPayoffTotal As String = CType(Math.Round(tblCurrentData.Rows(0)("PatientRemainingAmt"), 2) + Math.Round(tblAgingData.Rows(0)("PatientBalance"), 2) - Math.Round(tblCurrentData.Rows(0)("PatientCreditAmt"), 2) + Math.Round(tblCurrentData.Rows(0)("PrimaryRemainingAmt"), 2) + Math.Round(tblCurrentData.Rows(0)("SecondaryRemainingAmt"), 2) + Math.Round(tblAgingData.Rows(0)("InsuranceOpenBalance"), 2) - Math.Round(tblCurrentData.Rows(0)("PrimaryCreditAmt"), 2) - Math.Round(tblCurrentData.Rows(0)("SecondaryCreditAmt"), 2), String)
                lblPayoffBalance.Text = "Pay-Off: $ " & strPayoffPatient & " (Patient) + $ " & strPayoffInsurance & " (Insurance) = $ " & strPayoffTotal & " (Total)"
            End If
        End If

    End Sub
End Class