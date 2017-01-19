Public Class PaymentPosting
    Inherits System.Web.UI.Page

    Dim m_strTableName As String = "PaymentsTemp"
    Dim m_strFormCode As String = "PaymentsTemp"
    Dim m_strPrimaryKey As String = " recid "
    Dim m_strContentAreaName As String = "MainContent"
    Dim m_cphContentHolder As ContentPlaceHolder = Nothing
    Dim m_strDefaultPatientHeader As String = "Enter Data to Search for Patient (%=wildcard)"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Always needs to be called first
        Call g_RetrieveSessions(txtSessions)

        '-part of auto coder
        g_ModuleCode = "PT"
        m_cphContentHolder = Page.Master.FindControl(m_strContentAreaName)

        Dim strSQL As String = ""
        hidUnloadWarning.Value = False

        If IsPostBack Then
        Else

            ' hide payment elements until patient is chosen
            lblPatientHeader.Text = m_strDefaultPatientHeader
            hidFocusField.Value = txtChartNumber.ClientID
            h6PatientSearch.Visible = False
            divPaymentCollectionArea.Visible = False
            dtePaymentDate.Text = Date.Now.ToShortDateString
            dtePaymentDate.Enabled = False
            hidMode.Value = "new"
            If IsNothing(Request.QueryString("cid")) Then
                'User wants to enter new payment, not coming from anywhere
                divViewOptions.Visible = False
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = -1
                litScripts.Text = "<script  type=""text/javascript"">"
                litScripts.Text &= "jQuery(""#divViewOptions"").addClass(""hidden"");"
                litScripts.Text &= "jQuery(""#divViewOnly"").addClass(""hidden"");"
                litScripts.Text &= "</script>"
            Else
                'We are either coming from the contracts entry screen or viewing a temp payment from grid 
                'viewing/editing temp payment sends parameter "mid", so we can distinguish where we are coming from based on this parameter
                Dim recId As Integer = Request.QueryString("cid")
                loadPaymentByRecId(recId)
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = recId 'cid is actually the payment recid
                ' make sure contract exists, and gather data associated with it
                'txtPaymentID.Text = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))

                'Check to see if we are editing an existing payment--txtPaymentID will be filled
                'Dim strSql As String = "select * from PaymentsTemp_vw where recid = '" & txtPaymentID.Text & "'"
                'Dim tblPayment As DataTable = g_IO_Execute_SQL(strSql, False)
                'If tblPayment.Rows.Count > 0 Then
                '    ' get chart number for getPatName function that we will execute
                '    litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { pullPaymentInfo('" & txtPaymentID.Text & "','" & blnViewMode & "'); });</script>"
                'End If

                If IsNothing(Request.QueryString("mid")) Then
                    ''no view/edit mode
                    'litScripts.Text &= "<script type=""text/javascript"">jQuery(""#divEditOptions"").addClass(""hidden"");jQuery(""#divBtnAdd"").removeClass(""hidden"");</script>"
                    ''coming from contracts page 
                    ''Load patient info if coming from Contracts page - Chart number is sent in 
                    'If IsNothing(Request.QueryString("cno")) Then
                    'Else
                    '    txtChartNumber.Text = Request.QueryString("cno")
                    '    litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { getPatName('cht" & Request.QueryString("cno") & "');jQuery(""#divViewOptions"").addClass(""hidden""); });</script>"
                    'End If
                Else

                    If Request.QueryString("mid") = "edit" Then
                        divViewOptions.Visible = False
                        hidMode.Value = "edit"
                        'Dim strChartNo As String = Request.QueryString("cno")
                        'Dim strFirstName As String = ""
                        'Dim strLastName As String = ""
                        'Dim strDDLOptions As String = ""
                        'GetPatient(strChartNo, strFirstName, strLastName, strDDLOptions)
                        'litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { getPatName('cht" & Request.QueryString("cno") & "'); });</script>"
                        'litScripts.Text &= "<script  type=""text/javascript"">jQuery(""#divViewOptions"").addClass(""hidden"");jQuery(""#divEditOptions"").removeClass(""hidden"");jQuery(""#divBtnAdd"").addClass(""hidden"");</script>"

                    ElseIf Request.QueryString("mid") = "view" Then
                        hidMode.Value = "view"
                        'litScripts.Text &= "<script type=""text/javascript"">$( document ).ready(function() { getPatName('cht" & Request.QueryString("cno") & "'); });</script>"
                        'litViewOnly.Text = "<div id=""divViewOnly"" style=""width: 100%; height:100%; position: absolute; left:0px; z-index:999;"" ></div>"
                        'blnViewMode = True
                        ddlPaymentFrom.Enabled = False
                        txtPayerName.Enabled = False
                        ddlPatPaymentType.Enabled = False
                        ddlInsPaymentType.Enabled = False
                        txtPaymentReference.Enabled = False
                        dolPaymentAmount.Enabled = False
                        dolApplyRemaining.Enabled = False
                        dolPatientCurrent.Enabled = False
                        dolPatientPastDue.Enabled = False
                        dolPatientNext.Enabled = False
                        dolPatientBalance.Enabled = False
                        ddlPaymentFor.Enabled = False
                        txtComments.Enabled = False
                    End If
                End If
            End If
            ' -- auto coder
            Dim strWhere As String = m_strPrimaryKey & " = " & IIf(IsNothing(System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))), -1, System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)))
            Dim tblCurrentData As DataTable = g_getData(m_strTableName & "_formmap_vw", strWhere)
            If tblCurrentData.Rows.Count = 0 Then
                Call g_DefaultAllSessionVariables(m_strTableName, m_strFormCode, g_ModuleCode, m_strPrimaryKey, Session("user_link_id"))
            Else
                ' load database values to session variables
                Call g_LoadTableInfoFromDatabase(m_strTableName, tblCurrentData, m_strFormCode, g_ModuleCode, Session("user_link_id"))
            End If
            g_LoadDropDowns(m_strTableName, m_cphContentHolder)
            g_loadSessionsToPage(m_cphContentHolder, m_strFormCode, g_ModuleCode)
            ' -- end auto coder 

            hidDefaultPaymentFor.Value = ddlPaymentFor.SelectedIndex

            ' load payment types drop down individually by type code from payments type table
            strSQL = "Select * From DropDownList__PaymentType Where Type = 'P' or Type = 'X' Order By Descr"
            Dim tblPaymentPatientTypes As DataTable = g_IO_Execute_SQL(strSQL, False)
            Dim rowSelection As DataRow = tblPaymentPatientTypes.NewRow
            rowSelection("recid") = -1
            rowSelection("descr") = "Choose an option"
            tblPaymentPatientTypes.Rows.InsertAt(rowSelection, 0)
            ddlPatPaymentType.DataSource = tblPaymentPatientTypes
            ddlPatPaymentType.DataValueField = "recid"
            ddlPatPaymentType.DataTextField = "descr"
            ddlPatPaymentType.DataBind()
            ddlPaymentType.SelectedValue = ddlPatPaymentType.SelectedValue

            strSQL = "Select * From DropDownList__PaymentType Where Type = 'I' or Type = 'X' Order By Descr"
            Dim tblInsurnacePaymentTypes As DataTable = g_IO_Execute_SQL(strSQL, False)
            rowSelection = tblInsurnacePaymentTypes.NewRow
            rowSelection("recid") = -1
            rowSelection("descr") = "Choose an option"
            tblInsurnacePaymentTypes.Rows.InsertAt(rowSelection, 0)
            ddlInsPaymentType.DataSource = tblInsurnacePaymentTypes
            ddlInsPaymentType.DataValueField = "recid"
            ddlInsPaymentType.DataTextField = "descr"
            ddlInsPaymentType.DataBind()

            litHeader.Text = "Posting Payments As-Of " & Format(Date.Now, "MM/dd/yyyy")
            btnPost.Attributes.Add("onClick", "postData();")

        End If

        Call g_SendSessions(txtSessions)
    End Sub

    Private Sub loadPaymentByRecId(ByVal recId As Integer)
        ' must be from contracts -- contract recid sent it
        Dim strSQL As String = "Select * From PaymentsTemp Where recid = '" & recId & "'"
        Dim tblPaymentsTemp As DataTable = g_IO_Execute_SQL(strSQL, False)
        Dim strChartNo As String = tblPaymentsTemp.Rows("0")("chartNumber")
        Dim strFirstName As String = ""
        Dim strLastName As String = ""
        Dim strDDLOptions As String = ""
        Dim contractRecId As String = tblPaymentsTemp.Rows("0")("contract_RECID")
        GetPatient(strChartNo, strFirstName, strLastName, strDDLOptions, recId, contractRecId)

        ' these values are the same no matter if payment is from patient or insurance
        txtPayerName.Text = tblPaymentsTemp.Rows("0")("payerName")
        txtPaymentReference.Text = tblPaymentsTemp.Rows("0")("paymentReference")
        txtComments.Text = tblPaymentsTemp.Rows("0")("comments")
        dolApplyRemaining.Text = "0.00"

        If tblPaymentsTemp.Rows("0")("patientAmount") > 0 Then
            ' patient payment
            ddlPaymentFrom.SelectedValue = "Patient"
            ddlPatPaymentType.SelectedValue = tblPaymentsTemp.Rows("0")("paymentType")
            dolPaymentAmount.Text = tblPaymentsTemp.Rows("0")("patientAmount")
            ddlPaymentFor.SelectedValue = tblPaymentsTemp.Rows("0")("paymentFor")
            dolPatientCurrent.Text = tblPaymentsTemp.Rows("0")("applyToCurrentInvoice")
            dolPatientPastDue.Text = tblPaymentsTemp.Rows("0")("applyToPastDue")
            dolPatientNext.Text = tblPaymentsTemp.Rows("0")("applyToNextInvoice")
            dolPatientBalance.Text = tblPaymentsTemp.Rows("0")("applyToPrinciple")

            ' update pending payments by reducing the amount by the current payment
            Dim decPendingCurrent As Decimal = hidPatientCurrentPend.Value - tblPaymentsTemp.Rows("0")("applyToCurrentInvoice")
            hidPatientCurrentPend.Value = FormatCurrency(decPendingCurrent, 2)
            If decPendingCurrent = 0 Then
                lblPatientBalancePend.Text = ""
            Else
                lblPatientBalancePend.Text = decPendingCurrent.ToString("C2")
            End If
            Dim decPendingPastDue As Decimal = hidPatientPastDuePend.Value - tblPaymentsTemp.Rows("0")("applyToPastDue")
            hidPatientPastDuePend.Value = FormatCurrency(decPendingPastDue, 2)
            If decPendingPastDue = 0 Then
                lblPatientPastDuePend.Text = ""
            Else
                lblPatientPastDuePend.Text = decPendingPastDue.ToString("C2")
            End If
            Dim decPendingNextInvoice As Decimal = hidPatientNextPend.Value - tblPaymentsTemp.Rows("0")("applyToNextInvoice")
            hidPatientNextPend.Value = FormatCurrency(decPendingNextInvoice, 2)
            If decPendingNextInvoice = 0 Then
                lblPatientNextPend.Text = ""
            Else
                lblPatientNextPend.Text = decPendingNextInvoice.ToString("C2")
            End If
            Dim decPendingPatRemaining As Decimal = hidPatientBalancePend.Value - tblPaymentsTemp.Rows("0")("applyToPrinciple")
            hidPatientBalancePend.Value = FormatCurrency(decPendingPatRemaining, 2)
            If decPendingPatRemaining = 0 Then
                lblPatientBalancePend.Text = ""
            Else
                lblPatientBalancePend.Text = decPendingPatRemaining.ToString("C2")
            End If

        Else
            ' insurance payment
            Dim strPayerName As String = tblPaymentsTemp.Rows("0")("payerName")
            ddlInsPaymentType.SelectedValue = tblPaymentsTemp.Rows("0")("paymentType")
            Dim strPaymentFrom As String = ""
            For Each pmtFrom In ddlPaymentFrom.Items
                If pmtFrom.ToString.Contains(strPayerName) Then
                    strPaymentFrom = pmtFrom.ToString
                    Exit For
                End If
            Next
            ddlPaymentFrom.SelectedValue = strPaymentFrom
            If tblPaymentsTemp.Rows("0")("primaryAmount") > 0 Then
                dolPaymentAmount.Text = tblPaymentsTemp.Rows("0")("primaryAmount")
                If strPaymentFrom = "" Then
                    ' this will get selected if payment is from current primary or some "other" insurance-no where do we store pmt was from 'other' - would happen if data entry changes payer name
                    ddlPaymentFrom.SelectedIndex = 1
                End If
            Else
                dolPaymentAmount.Text = tblPaymentsTemp.Rows("0")("secondaryAmount")
                ddlPaymentFrom.SelectedIndex = 2
            End If
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        ' clear existing labels
        h6PatientSearch.Visible = False
        litScripts.Text = ""
        litOptions.Text = ""

        ' search for name or chart # entered
        ' 02/23/16 cpb lookup patient from search entry  
        Dim strFirstName As String = txtFirstName.Text
        Dim strLastName As String = txtLastName.Text
        Dim strChartNo As String = txtChartNumber.Text
        Dim strDDLOptions As String = ""
        Dim strContractRecId As String = txtPatientContractRecId.Text
        If strContractRecId = "" Then
            strContractRecId = "-1"
        End If
        GetPatient(strChartNo, strFirstName, strLastName, strDDLOptions, "-1", strContractRecId)


    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        h6PatientSearch.Visible = False
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtChartNumber.Text = ""
        txtPatientNumber.Text = ""
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        clearScreen()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        clearScreen()
    End Sub

    Private Sub btnCancelEdit_Click(sender As Object, e As EventArgs) Handles btnCancelEdit.Click
        clearScreen()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim strWhere As String = m_strPrimaryKey & " = " & System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))
        g_IO_SQLDelete(m_strTableName, strWhere)

        If ddlPaymentFrom.SelectedValue.Contains("Patient") Then
        Else
            ' delete insurance details
            strWhere = "paymentsTempRecId = '" & System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) & "'"
            g_IO_SQLDelete("PaymentsTempDetail", strWhere)
        End If

        clearScreen()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        g_RetrieveSessions(txtSessions)
        g_ModuleCode = "PT"
        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))
        g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, System.Web.HttpContext.Current.Session("user_link_id"))
        g_SendSessions(txtSessions)

        If ddlPaymentFrom.SelectedValue.Contains("Patient") Then
        Else
            ' update insurance details
            ' delete all detail records for this payment, then rebuild as they are currently
            strWhere = "paymentsTempRecId = '" & System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) & "'"
            g_IO_SQLDelete("PaymentsTempDetail", strWhere)
            Dim strClaim As String = ""
            Dim strClaimDelim As String = ""
            Dim arrInsList() As String = Split(hidInsRefList.Value, "||")
            For idx = 0 To arrInsList.Count - 1
                Dim arrInsEntry() As String = Split(arrInsList(idx), "~~")
                If arrInsEntry(4) = 0 Then
                Else
                    Dim nvcInsDetail As New NameValueCollection
                    nvcInsDetail("chartNumber") = txtChartNumber.Text
                    nvcInsDetail("paymentId") = arrInsEntry(2)
                    nvcInsDetail("paymentsTempRecId") = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))
                    nvcInsDetail("paymentAmount") = arrInsEntry(4)
                    g_IO_SQLInsert("PaymentsTempDetail", nvcInsDetail)
                    strClaim &= strClaimDelim & Trim(arrInsEntry(1)) & "-" & Trim(arrInsEntry(2))
                    strClaimDelim = ","
                End If
            Next
            Dim nvcPaymentTempUdate As New NameValueCollection
            nvcPaymentTempUdate("ClaimNumber") = strClaim
            g_IO_SQLUpdate("PaymentsTemp", nvcPaymentTempUdate, "PaymentPosting", "recid = '" & System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) & "'")
        End If
        clearScreen()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        If txtContract_RECID.Text = "-1" Then
            'if txtContract_RECID is -1 then this patient does not have a contract yet, create patient record
            CreatePatientData(txtChartNumber.Text)
        End If

        ' auto coder add
        g_ModuleCode = "PT"
        Dim strValues As String = Nothing
        g_loadPageToSessions(m_cphContentHolder, m_strFormCode, g_ModuleCode)
        Dim strWhere As String = m_strPrimaryKey & " = " & IIf(IsNothing(System.Web.HttpContext.Current.Session(m_strPrimaryKey)), -1, System.Web.HttpContext.Current.Session(m_strPrimaryKey))
        g_PostTablePageToDatabase(m_strTableName, strWhere, m_strFormCode, g_ModuleCode, System.Web.HttpContext.Current.Session("user_link_id"))
        Dim intPaymentsTempRecId As Integer = g_IO_GetLastRecId()
        Dim strClaim As String = ""
        Dim strClaimDelim As String = ""

        If ddlPaymentFrom.SelectedValue.Contains("Patient") Then
        Else
            Dim arrInsList() As String = Split(hidInsRefList.Value, "||")
            For idx = 0 To arrInsList.Count - 1
                Dim arrInsEntry() As String = Split(arrInsList(idx), "~~")
                If arrInsEntry(4) = 0 Then
                Else
                    Dim nvcInsDetail As New NameValueCollection
                    nvcInsDetail("chartNumber") = txtChartNumber.Text
                    nvcInsDetail("paymentId") = arrInsEntry(2)
                    nvcInsDetail("paymentsTempRecId") = intPaymentsTempRecId
                    nvcInsDetail("paymentAmount") = arrInsEntry(4)
                    g_IO_SQLInsert("PaymentsTempDetail", nvcInsDetail)
                    strClaim &= strClaimDelim & Trim(arrInsEntry(1)) & "-" & Trim(arrInsEntry(2))
                    strClaimDelim = ","
                End If
            Next
            Dim nvcPaymentTempUdate As New NameValueCollection
            nvcPaymentTempUdate("ClaimNumber") = strClaim
            g_IO_SQLUpdate("PaymentsTemp", nvcPaymentTempUdate, "PaymentPosting", "recid = '" & intPaymentsTempRecId & "'")
        End If

        clearScreen()
        'Response.Redirect("PaymentPosting_Orig.aspx?values=" & strValues)
    End Sub

    Private Sub clearScreen()
        hidMode.Value = "new"

        litOptions.Text = ""
        litScripts.Text = ""

        lblPatientHeader.Text = m_strDefaultPatientHeader

        ' patient selection
        txtFirstName.Text = ""
        txtFirstName.Enabled = True
        txtLastName.Text = ""
        txtLastName.Enabled = True
        txtChartNumber.Text = ""
        txtChartNumber.Enabled = True
        txtPatientNumber.Text = ""
        btnSearch.Enabled = True
        btnSearch.Visible = True
        btnClear.Enabled = True
        btnClear.Visible = True

        ' payment collection-payer info
        ddlPaymentFrom.SelectedIndex = -1
        txtPayerName.Text = ""
        'txtPayersName.Text = ""
        ddlInsPaymentType.SelectedIndex = -1
        ddlPatPaymentType.SelectedIndex = -1
        ddlPaymentType.SelectedIndex = -1
        txtPaymentReference.Text = ""

        ' payment collection-payment info
        dolPaymentAmount.Text = "0.00"
        dolApplyRemaining.Text = "0.00"
        dolOverPayment.Text = "0.00"

        ' payment collection-payment info-patient
        lblPatientCurrent.Text = ""
        dolPatientCurrent.Text = "0.00"
        lblPatientPastDue.Text = ""
        dolPatientPastDue.Text = "0.00"
        lblPatientNext.Text = ""
        dolPatientNext.Text = "0.00"
        lblPatientBalance.Text = ""
        dolPatientBalance.Text = "0.00"
        dolApplyToCurrentInvoice.Text = "0.00"
        dolApplyToPastDue.Text = "0.00"
        dolApplyToNextInvoice.Text = "0.00"
        dolApplyToPrinciple.Text = "0.00"

        ' payment collection-payment info-insurance
        litInsuranceClaimsTable.Text = ""
        litInsurnaceScript.Text = ""

        ' payment for and comments
        ddlPaymentFor.SelectedIndex = hidDefaultPaymentFor.Value
        txtComments.Text = ""

        ' 11/29/16 doctors_vw
        txtDoctors_vw.Text = "-1"
        txtPatientContractRecId.Text = ""

        h6PatientSearch.Visible = False
        divPaymentCollectionArea.Visible = False
        btnPost.Enabled = True
        hidFocusField.Value = txtChartNumber.ClientID
    End Sub
    Private Sub CreatePatientData(ByVal strChartNum As String)
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
            Dim nvcPatient As New NameValueCollection
            nvcPatient("patient_id") = intPatientID
            nvcPatient("chart_number") = strChartNum
            nvcPatient("name_last") = tblPatient.Rows(0)("LastName")
            nvcPatient("name_first") = tblPatient.Rows(0)("FirstName")
            nvcPatient("name_mid") = IIf(IsDBNull(tblPatient.Rows(0)("MiddleName")), "", tblPatient.Rows(0)("MiddleName"))
            nvcPatient("addr_other") = tblPatient.Rows(0)("Address2")
            nvcPatient("addr_street") = tblPatient.Rows(0)("Address1")
            nvcPatient("addr_city") = tblPatient.Rows(0)("City")
            nvcPatient("addr_state") = tblPatient.Rows(0)("State")
            nvcPatient("addr_zip") = tblPatient.Rows(0)("ZipCode")
            nvcPatient("gender") = IIf(tblPatient.Rows(0)("Gender") = "F", "1", "2")
            nvcPatient("dob") = tblPatient.Rows(0)("DOB")
            g_IO_SQLInsert("Patients", nvcPatient, "paymentPosting")
        End If
    End Sub

    Private Sub GetPatient(ByRef strChartNo As String, ByRef strFirstName As String, ByRef strLastName As String, ByRef strDDLOptions As String,
                           ByRef paymentsTempRecId As Integer, ByRef contractRecId As String)

        Dim strSQL As String = ""
        Dim strLitMessage As String = ""
        Dim strSearchType As String = ""
        Dim strSearchValue As String = ""
        If contractRecId = "-1" Then
            contractRecId = ""
        End If
        Dim tblPatient As DataTable = g_patientSearch(strChartNo, strFirstName, strLastName, contractRecId, strSQL, strLitMessage, strDDLOptions)
        If tblPatient.Rows.Count = 0 Then
            h6PatientSearch.Visible = True
            hidFocusField.Value = txtChartNumber.ClientID

        ElseIf tblPatient.Rows.Count > 1 Then
            ' check to see if all patiets have the same chart number.  If so must just be multiple contracts
            Dim blnSameChart As Boolean = True
            Dim strPatientChart As String = tblPatient.Rows(0)("ChartNumber")
            For Each Patient As DataRow In tblPatient.Rows
                If strPatientChart = Patient("ChartNumber") Then
                Else
                    blnSameChart = False
                    Exit For
                End If
            Next
            Dim strModalTitle As String = "More than 1 patient found"
            If blnSameChart Then
                litContractOptions.Text = ""
                ' all same chart -- build up select for specific contract
                For Each patient As DataRow In tblPatient.Rows
                    litContractOptions.Text &= "<option value = """ & patient("recid") & """>" &
                        patient("ChartNumber").PadLeft(6, "0") & " - " & patient("PatientName").PadRight(50, " ") &
                    IIf(patient("Account_Id") = "", "", " - " & patient("Account_Id")) &
                    " - " & patient("ContractDate") &
                    " Patient:" & patient("PatientRemainingBalance") &
                    " Primary: " & patient("PrimaryRemainingBalance") &
                    " Secondary: " & patient("SecondaryRemainingBalance") &
                    "</option>"
                Next
                litScripts.Text = "<script>" &
                "jQuery("".modal-title"")[0].innerHTML = """ & strModalTitle & """;" &
                    "jQuery('#divModalContractSelectionMultiples').modal('show');" &
                    "jQuery('#loading_indicator').show();" &
                    "jQuery('#loading_indicator').hide();</script>"
            Else
                Dim arrOptions() As String = Split(strDDLOptions, "<option")
                For idx = 2 To arrOptions.Length - 1
                    litOptions.Text &= "<option" & arrOptions(idx)
                Next
                litScripts.Text = "<script>" &
                    "jQuery("".modal-title"")[0].innerHTML = """ & strModalTitle & """;" &
                        "jQuery('#divModalContractSelection').modal('show');" &
                        "jQuery('#loading_indicator').show();" &
                        "jQuery('#loading_indicator').hide();</script>"
            End If


        Else
            ' must have gotten the one we wanted
            ' 01.04.17 cpb check to see if patient has any overpayments not yet applied to invoices
            ' first get patient overpayments
            Dim decOverPayments As Decimal = 0.0
            Dim strSQLOverPayment = "Select * From Payments Where contract_recId = '" & contractRecId & "' and Invoices_RecId = '-1' and PaymentSelection = 'PatientAmount'"
            Dim tblOverPayments As DataTable = g_IO_Execute_SQL(strSQLOverPayment, False)
            For Each payment As DataRow In tblOverPayments.Rows
                decOverPayments += payment("patientAmount")
            Next
            ' now get insurance overpayments
            Dim decOverPaymentsPrimary As Decimal = 0.0
            Dim decOverPaymentsSecondary As Decimal = 0.0
            strSQLOverPayment = "Select * From Payments Where contract_recId = '" & contractRecId & "' and Invoices_RecId = '-1' and ClaimNumber = -1 and (PaymentSelection='PrimaryAmount' or PaymentSelection='SecondaryAmount')"
            tblOverPayments = g_IO_Execute_SQL(strSQLOverPayment, False)
            For Each payment As DataRow In tblOverPayments.Rows
                If payment("paymentSelection") = "PrimaryAmount" Then
                    decOverPaymentsPrimary += payment("PrimaryAmount")
                Else
                    decOverPaymentsSecondary += payment("SecondaryAmount")
                End If

            Next
            dolOverPayment.Text = decOverPayments.ToString("C2")
            dolOverPaymentPrimary.Text = decOverPaymentsPrimary.ToString("C2")
            dolOverPaymentSecondary.Text = decOverPaymentsSecondary.ToString("C2")
            '01.04.17 eom

            ' get pending payments
            Dim decPendingCurrent As Decimal = 0.0
            Dim decPendingPastDue As Decimal = 0.0
            Dim decPendingNextInvoice As Decimal = 0.0
            Dim decPendingPatRemaining As Decimal = 0.0
            Dim strPaymentsFromAnotherUserPending As String = ""
            getPendingPayments(decPendingCurrent, decPendingPastDue, decPendingNextInvoice, decPendingPatRemaining, tblPatient.Rows("0")("chartNumber"), strPaymentsFromAnotherUserPending)                ' current
            If strPaymentsFromAnotherUserPending <> "" Then
                litPaymentUsers.Text = strPaymentsFromAnotherUserPending
                litScripts.Text = "<script>" &
                "jQuery("".modal-title"")[0].innerHTML = ""Payments Already Being Entered"";" &
                    "jQuery('#divModalPaymentsBeingEntered').modal('show');" &
                    "jQuery('#loading_indicator').show();" &
                    "jQuery('#loading_indicator').hide();</script>"
            Else

                hidUnloadWarning.Value = True
                Dim blnInsurance As Boolean = False

                If strSQL.Contains("Contracts") Then
                    'if data is from contracts table, then get dollars info
                    ' get patient dollars info 
                    Dim strInvoicesTableCurrent As String = ""
                    Dim decCurrent As Decimal = g_getPatientCurrentAmount(tblPatient.Rows("0"), strInvoicesTableCurrent)
                    Dim strInvoicesTablePastDue As String = ""
                    Dim decPastDue As Decimal = g_getPatientPastDueAmount(tblPatient.Rows("0"), strInvoicesTablePastDue)
                    Dim decPatRemaining As Decimal = g_getPatientRemainingBalance(tblPatient.Rows("0"))
                    Dim decNextInvoice As Decimal = g_getPatientNextInvoice(tblPatient.Rows("0"))

                    litPatientInvoiceTable.Text = strInvoicesTableCurrent & strInvoicesTablePastDue

                    ' current
                    lblPatientCurrent.Text = decCurrent.ToString("C2")
                    hidPatientCurrent.Value = FormatCurrency(decCurrent, 2)
                    If decCurrent > 0 Then
                        dolPatientCurrent.Text = "0.00"
                    End If
                    hidPatientCurrentPend.Value = FormatCurrency(decPendingCurrent, 2)
                    If decPendingCurrent = 0 Then
                        lblPatientCurrentPend.Text = ""
                    Else
                        lblPatientCurrentPend.Text = decPendingCurrent.ToString("C2")
                    End If

                    ' past due
                    lblPatientPastDue.Text = decPastDue.ToString("C2")
                    hidPatientPastDue.Value = FormatCurrency(decPastDue, 2)
                    If decPastDue > 0 Then
                        dolPatientPastDue.Text = "0.00"
                    End If
                    hidPatientPastDuePend.Value = FormatCurrency(decPendingPastDue, 2)
                    If decPendingPastDue = 0 Then
                        lblPatientPastDuePend.Text = ""
                    Else
                        lblPatientPastDuePend.Text = decPendingPastDue.ToString("C2")
                    End If

                    ' next invoice
                    lblPatientNext.Text = decNextInvoice.ToString("C2")
                    hidPatientNext.Value = FormatCurrency(decNextInvoice, 2)
                    If decNextInvoice > 0 Then
                        dolPatientNext.Text = "0.00"
                    End If
                    hidPatientNextPend.Value = FormatCurrency(decPendingNextInvoice, 2)
                    If decPendingNextInvoice = 0 Then
                        lblPatientNextPend.Text = ""
                    Else
                        lblPatientNextPend.Text = decPendingNextInvoice.ToString("C2")
                    End If

                    ' patient remaining-balance
                    lblPatientBalance.Text = decPatRemaining.ToString("C2")
                    hidPatientBalance.Value = FormatCurrency(decPatRemaining, 2)
                    If decPatRemaining > 0 Then
                        dolPatientBalance.Text = "0.00"
                    End If
                    hidPatientBalancePend.Value = FormatCurrency(decPendingPatRemaining, 2)
                    If decPendingPatRemaining = 0 Then
                        lblPatientBalancePend.Text = ""
                    Else
                        lblPatientBalancePend.Text = decPendingPatRemaining.ToString("C2")
                    End If

                    ' get insurance info
                    Dim tblPrimInsurInfo As DataTable = Nothing
                    Dim tblSecInsurInfo As DataTable = Nothing
                    g_getPatientInsurance(IIf(IsDBNull(tblPatient.Rows("0")("PrimaryInsurancePlans_vw")), "-1", tblPatient.Rows("0")("PrimaryInsurancePlans_vw")),
                                      IIf(IsDBNull(tblPatient.Rows("0")("SecondaryInsurancePlans_vw")), "-1", tblPatient.Rows("0")("SecondaryInsurancePlans_vw")),
                                      tblPrimInsurInfo, tblSecInsurInfo)
                    ddlPaymentFrom.Items.Clear()
                    ddlPaymentFrom.Items.Add("Patient")
                    If tblPrimInsurInfo.Rows.Count > 0 Then
                        hidPrimaryInsurance.Value = tblPrimInsurInfo.Rows("0")("ins_company_name")
                        hidPrimaryInsurancePlanId.Value = tblPrimInsurInfo.Rows("0")("plan_id")
                        ddlPaymentFrom.Items.Add("Primary-" & tblPrimInsurInfo.Rows("0")("ins_company_name"))
                        blnInsurance = True
                        hidPrimaryInsuranceBalance.Value = tblPatient.Rows("0")("PrimaryRemainingBalance")
                    End If
                    If tblSecInsurInfo.Rows.Count > 0 Then
                        hidSecondaryInsurance.Value = tblSecInsurInfo.Rows("0")("ins_company_name")
                        hidSecondaryInsurancePlanId.Value = tblSecInsurInfo.Rows("0")("plan_id")
                        ddlPaymentFrom.Items.Add("Secondary-" & tblSecInsurInfo.Rows("0")("ins_company_name"))
                        blnInsurance = True
                        hidSecondaryInsuranceBalance.Value = tblPatient.Rows("0")("SecondaryRemainingBalance")
                    End If

                    ' build claim table to databind to insurace ddl; build input table for insurance claims.
                    ' 01.11.17 cpb separate insurance table type ins type
                    Dim arrInsuranceTable(3) As String
                    'Dim strClaimsTable As String = ""
                    Dim intClaimCount As Integer = 0
                    Dim strClaimsScript As String = ""
                    Dim strOtherInsList As String = ""
                    Dim strInsRefList As String = ""
                    'Dim tblClaimNumber As DataTable = g_getPatientClaims(True, tblPatient.Rows("0"), paymentsTempRecId, "", "", strClaimsTable, intClaimCount, strClaimsScript, strOtherInsList, strInsRefList)
                    Dim tblClaimNumber As DataTable = g_getPatientClaims(True, tblPatient.Rows("0"), paymentsTempRecId, "", "", arrInsuranceTable, intClaimCount, strClaimsScript, strOtherInsList, strInsRefList)
                    hidInsCount.Value = intClaimCount - 1
                    hidInsRefList.Value = strInsRefList
                    litInsuranceClaimsTable.Text = arrInsuranceTable(0) + arrInsuranceTable(2) 'strClaimsTable
                    litInsuranceClaimsTableSecondary.Text = arrInsuranceTable(1) + arrInsuranceTable(2)
                    litInsurnaceScript.Text = strClaimsScript
                    If strOtherInsList = "" Then
                    Else
                        Dim arrOtherInsList() As String = Split(strOtherInsList, "||")
                        For Each otherIns As String In arrOtherInsList
                            ddlPaymentFrom.Items.Add("Other-" & otherIns)
                        Next
                    End If
                End If

                lblPatientHeader.Text = "Patient#: "
                If tblPatient.Rows("0")("Account_Id") <> "" Then
                    lblPatientHeader.Text &= tblPatient.Rows("0")("Account_Id")
                    txtContract_RECID.Text = tblPatient.Rows("0")("recid")
                Else
                    lblPatientHeader.Text &= "No Contract Found"
                    txtContract_RECID.Text = "-1"
                End If

                txtFirstName.Text = tblPatient.Rows("0")("firstName")
                txtFirstName.Enabled = False
                txtLastName.Text = tblPatient.Rows("0")("lastName")
                txtLastName.Enabled = False
                txtChartNumber.Text = tblPatient.Rows("0")("chartNumber")
                txtChartNumber.Enabled = False
                txtPatientNumber.Text = tblPatient.Rows("0")("patientKey")
                btnSearch.Enabled = False
                btnSearch.Visible = False
                btnClear.Enabled = False
                btnClear.Visible = False

                ' 11/29/16 - add doctors_vw
                If IsDBNull(tblPatient.Rows(0)("doctors_vw")) Then
                    txtDoctors_vw.Text = "-1"
                Else
                    txtDoctors_vw.Text = tblPatient.Rows(0)("doctors_vw")
                End If


                hidFocusField.Value = ddlPaymentFrom.ClientID
                divPaymentCollectionArea.Visible = True
                btnPost.Enabled = False

                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function() {document.getElementById('ifmPayment').src = 'frmListManager.aspx?id=PaymentsTemp_vw&usr=True&divHide=divHeader,divPaginationHeading,divPagination,navPage,divFooter&perm=00000'});</script>"
            End If

        End If
    End Sub

    Private Sub getPendingPayments(ByRef decPendingCurrent As Decimal, ByRef decPendingPastDue As Decimal, ByRef decPendingNextInvoice As Decimal, ByRef decPendingPatRemaining As Decimal,
                                   ByVal strChartNumber As String, ByRef strPaymentsFromAnotherUserPending As String)
        ' 09.21.16 cpb add check for any payments being applied by another user -- limit to one entry for a chart number to a single entry person
        Dim strPaymentsFromAnotherUserPendingList As String = ""
        Dim strDelim As String = ","
        Dim strSQLPaymentsTemp As String = "Select * From PaymentsTemp Where ChartNumber = '" & strChartNumber & "'"
        Dim tblPendingPayments As DataTable = g_IO_Execute_SQL(strSQLPaymentsTemp, False)
        For Each payment As DataRow In tblPendingPayments.Rows
            decPendingCurrent += payment("ApplyToCurrentInvoice")
            decPendingPastDue += payment("ApplyToPastDue")
            decPendingNextInvoice += payment("ApplyToNextInvoice")
            decPendingPatRemaining += payment("ApplyToPrinciple")
            If payment("Sys_Users_RECID") <> g_UserRecId Then
                If strPaymentsFromAnotherUserPendingList.Contains("," & payment("Sys_Users_RECID")) Then
                Else
                    strPaymentsFromAnotherUserPendingList = strDelim & payment("Sys_Users_RECID")
                End If
            End If
        Next
        If strPaymentsFromAnotherUserPendingList = "" Then
        Else
            Dim arrPaymentsFromAnotherUserPendingList() = Split(strPaymentsFromAnotherUserPendingList, ",")
            strDelim = ""
            For Each strUser As String In arrPaymentsFromAnotherUserPendingList
                If strUser = "" Then
                Else
                    Dim strSQL As String = "Select user_id From sys_Users Where recid = '" & strUser & "'"
                    Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
                    strPaymentsFromAnotherUserPending &= strDelim & tblUser.Rows(0)("user_id")
                    strDelim = ","
                End If

            Next
        End If
    End Sub

End Class