Public Class frmClaimsProcessing
    Inherits System.Web.UI.Page


    Protected Sub endit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim r = 1
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        ' This routine will build a list of recid's of appropriate contracts to feed to the IFrame on the form.  
        ' The IFrame will call a routine that will further filter the contracts based on the date selected


        'RLO 10/25/16 - let user override the date processed on the claims selected
        Dim dteProcedureDate As Date = Date.Now
        If IsNothing(Request.QueryString("od")) OrElse Trim(CStr(Request.QueryString("od"))) = "" Then
        Else
            If IsDate(Request.QueryString("od")) Then
                dteProcedureDate = Request.QueryString("od")
            Else
                lblMessage.Text = "Invalid altered claim date entered.  Please try again."
                Exit Sub
            End If
        End If
        litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){showProcessDate('" & Format(dteProcedureDate, "MM/dd/yyyy") & "')});</script>"

        Dim strIframeDestination As String = ""

        If IsPostBack Then

        Else
            If IsNothing(Request.QueryString("rl")) Then
            Else
                ' If a specific contract is requested then "rl" will contain the contract number(s)
                ' strIframeDestination = "frmListManager.aspx?id=[UnprocessedClaimsInvoicesCurrentMonth_vw]&vo=1&divHide=divHeader,divFooter" ' prepare the iframe load
                strIframeDestination = "frmListManager.aspx?id=UnprocessedClaimsInvoicesCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "')&vo=1&divHide=divHeader,divFooter"    ' prepare the iframe load
                hidInitialcontract.Value = "UnprocessedClaimsInvoicesCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "')" & "&&" & Request.QueryString("rl")

                lblMessage.Text = "Print Selected Claims and Invoices"

                ' is there insurance to be processed?
                Dim strSQL As String = "Select PrimaryInsurancePlans_vw,SecondaryInsurancePlans_vw,SecondaryCoverageAmt,PrimaryRemainingBalance,SecondaryRemainingBalance from contracts where recid = " & Request.QueryString("rl")
                Dim tblContracts As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblContracts.Rows.Count = 0 Then
                Else
                    If tblContracts.Rows(0)("PrimaryInsurancePlans_vw") > 0 Then
                        '  there is insurance coverage here

                        ' if there is insurance has there already been a primary claim processed for selected month?
                        strSQL = "Select count(*) as ClaimCount from claims where contracts_recid = " & Request.QueryString("rl") & " and type = 0 " &
                            " and month(procedure_date) = " & Month(dteProcedureDate) & "and year(procedure_date) = " & Year(dteProcedureDate)

                        Dim tblClaims As DataTable = g_IO_Execute_SQL(strSQL, False)
                        If tblClaims.Rows(0)("ClaimCount") > 0 Then
                            ' if a primary has been processed and there is secondary insurance has that been processed yet?
                            btnClaimPrimary.Enabled = False
                        Else
                            btnClaimPrimary.Enabled = True  ' need to process the primary and/or secondary claim
                        End If

                        '  is there a secondary claim to be printed
                        If tblContracts.Rows(0)("SecondaryInsurancePlans_vw") > 0 Then
                            ' there is secondary insurance, is there an amount indicated?
                            If tblContracts.Rows(0)("SecondaryCoverageAmt") = 0 Then
                                ' not ready to process yet, user is waiting to indicate the secondary coverage amt

                                btnClaimSecondary.Enabled = False

                            Else
                                strSQL = "Select count(*) as ClaimCount from claims where contracts_recid = " & Request.QueryString("rl") & " and type = 1 and plan_id = (select plan_id from DropDownList__InsurancePlans where RECID =" & tblContracts.Rows(0)("PrimaryInsurancePlans_vw") & ")" &
                                 " and month(procedure_date) = " & Month(dteProcedureDate) & " and year(procedure_date) = " & Year(dteProcedureDate)
                                tblClaims = g_IO_Execute_SQL(strSQL, False)
                                If tblClaims.Rows(0)("ClaimCount") > 0 Then
                                    ' secondary has already been processed
                                    btnClaimSecondary.Enabled = False
                                Else
                                    btnClaimSecondary.Enabled = True
                                End If

                            End If
                        Else
                            ' there is no secondary 
                            btnClaimSecondary.Enabled = False
                        End If

                    Else
                        ' disable Claims button, there is no coverage
                        btnClaimPrimary.Enabled = False
                        btnClaimSecondary.Enabled = False
                    End If

                    btnPreviewClaimPrimary.Enabled = btnClaimPrimary.Enabled
                    btnPreviewClaimSecondary.Enabled = btnClaimSecondary.Enabled

                    ' has there already been an invoice printed?
                    Dim tblInvoices As DataTable = g_IO_Execute_SQL("select count(*) as InvCount from Invoices inv where InvoiceType = 'I' and inv.contracts_recid = " & Request.QueryString("rl") &
                                                                    " and month(inv.postdate) = " & Month(dteProcedureDate) & " and year(inv.postdate) = " & Year(dteProcedureDate), False)

                    ' was the last Invoice processed this month?  If so, it should not be processed again
                    If tblInvoices.Rows(0)("InvCount") > 0 Then

                        ' invoice has already been processed fpr selected month
                        btnPrint.Enabled = False
                        btnPreviewInvoice.Enabled = False
                    Else
                        ' process this invoice
                    End If

                End If

                litScripts.Text &= "<script type=""text/javascript"">var InvoiceClaim = 'Processing';</script>" ' load page label
            End If

            If IsNothing(Request.QueryString("c")) Then
            Else

                ' run claims only
                btnPrint.Style.Add("display", "none")
                btnPreviewInvoice.Style.Add("display", "none")

                litScripts.Text &= "<script type=""text/javascript"">var InvoiceClaim = 'Claims';</script>" ' load page label



                '  create a list of contract recid's to display and store them in hidInitialcontract 

                ' query to pull contracts that are active 
                Dim strSQL As String = "Select recid,PatientFirstPay,PrimaryBillingFrequency_vw,SecondaryBillingFrequency_vw,contractdate,PrimaryCoverageAmt,SecondaryCoverageAmt,PrimaryInsurancePlans_vw,SecondaryInsurancePlans_vw from contracts where status_vw = 1 and ContractDate <= '" & Format(dteProcedureDate, "yyyy/MM/dd") & " 23:59:59'"

                '  pull only contracts that have primary or secondary insurance flags set
                strSQL &= " and (PrimaryInsurancePlans_vw > -1 or SecondaryInsurancePlans_vw > -1)"
                Dim tblClaimContracts As DataTable = g_IO_Execute_SQL(strSQL, False)

                btnClaimPrimary.Enabled = False
                btnClaimSecondary.Enabled = False
                If Request.QueryString("c") = "1" Then
                    strIframeDestination = "frmListManager.aspx?id=UnprocessedPrimaryInsuranceClaimsCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "')&vo=1&divHide=divHeader,divFooter"    ' prepare the iframe load
                    hidInitialcontract.Value = "frmListManager.aspx?id=UnprocessedPrimaryInsuranceClaimsCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "')" & "&&-1"                               '  "-1"  ' primer -- in case there aren't any below
                Else
                    strIframeDestination = "frmListManager.aspx?id=UnprocessedSecondaryInsuranceClaimsCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "',0,0)&vo=1&divHide=divHeader,divFooter"    ' prepare the iframe load
                    hidInitialcontract.Value = "frmListManager.aspx?id=UnprocessedSecondaryInsuranceClaimsCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "',0,0)" & "&&-1"
                End If

                For Each rowClaimContract In tblClaimContracts.Rows
                    Dim blnProcessPrimaryClaim As Boolean = False
                    Dim blnProcessSecondaryClaim As Boolean = False

                    If Request.QueryString("c") = "1" Then
                        lblMessage.Text = "Create/Print Primary Monthly Claims"

                        '  what is the frequency this claim is supposed to be processed?    (determine based on start of contract date, not the last time this claim was processed)
                        Select Case rowClaimContract("PrimaryBillingFrequency_vw")
                            Case 1
                                ' monthly  - then flag it to be processed
                                blnProcessPrimaryClaim = True
                            Case 2
                                ' Quarterly
                                Dim intMonthsSinceContractStarted As Integer = DateDiff(DateInterval.Month, rowClaimContract("contractdate"), dteProcedureDate)

                                If intMonthsSinceContractStarted Mod 3 = 0 Then
                                    '  Every third month will generate a Mod 3 value of 0 -  flag it to process every third month since the start of the contract
                                    blnProcessPrimaryClaim = True
                                End If

                            Case 3
                                ' Yearly
                                Dim intMonthsSinceContractStarted As Integer = DateDiff(DateInterval.Month, rowClaimContract("contractdate"), dteProcedureDate)
                                If intMonthsSinceContractStarted Mod 12 = 0 Then
                                    '  Every twelth month will generate a Mod 12 value of 0 -  flag it to process every twelth month since the start of the contract
                                    blnProcessPrimaryClaim = True
                                End If
                            Case Else
                                blnProcessPrimaryClaim = False
                        End Select
                    Else

                        blnProcessPrimaryClaim = False

                        lblMessage.Text = "Create/Print Secondary Monthly Claims"

                        If rowClaimContract("SecondaryCoverageAmt") = 0 Then
                            blnProcessSecondaryClaim = False
                        Else


                            Select Case rowClaimContract("SecondaryBillingFrequency_vw")
                                Case 1
                                    ' monthly
                                    blnProcessSecondaryClaim = True
                                Case 2
                                    ' Quarterly
                                    If DateDiff(DateInterval.Month, rowClaimContract("contractdate"), dteProcedureDate) Mod 3 = 0 Then
                                        blnProcessSecondaryClaim = True
                                    End If
                                Case 3
                                    ' Yearly
                                    If DateDiff(DateInterval.Month, rowClaimContract("contractdate"), dteProcedureDate) Mod 12 = 0 Then
                                        blnProcessSecondaryClaim = True
                                    End If
                                Case Else
                                    blnProcessSecondaryClaim = False

                            End Select
                        End If
                    End If

                    Dim blnProcessClaim As Boolean = blnProcessPrimaryClaim Or blnProcessSecondaryClaim

                    If blnProcessClaim Then

                        Dim chrType As Char = IIf(blnProcessPrimaryClaim, "0", "1")  ' determine type of claim to process (0 = Primary, 1 = Secondary)

                        ' it was determined above to process this claim for the primary insurance company

                        ' if a primary claim has been processed it will be saved on a single record on the CLAIMS table,  see if such a record exists before processing this claim 

                        ' is there a history of claims already processed?  Find the most recent claim processed date (claims already processed are in the CLAIMS table)
                        Dim tblClaims As DataTable = g_IO_Execute_SQL("select  clm.procedure_date procedure_date from Claims clm where clm.contracts_recid = " & rowClaimContract("recid") &
                                                                      " and type = " & chrType & " and plan_id = (select plan_id from DropDownList__InsurancePlans where RECID =" & tblClaimContracts.Rows(0)("PrimaryInsurancePlans_vw") & ")" & " order by clm.procedure_date desc", False)

                        ' primary  or secondary claim is due this month - has it already been processed for this month?

                        'RLO 10/25/2016
                        ' Is there a claim already processed for the selected month?  If so, it should not be processed again
                        Dim intSelectedMonthIndex As Integer = 0
                        For Each rowClaim As DataRow In tblClaims.Rows

                            If IsDate(rowClaim("procedure_date")) Then

                                ' it should never not be a date since the contract starts out issuing an initial claim but check anyway
                                If Month(rowClaim("procedure_date")) = Month(dteProcedureDate) Then
                                    ' claim has already been processed this month
                                    blnProcessClaim = False
                                    Exit For
                                Else
                                    If Month(rowClaim("procedure_date")) < Month(dteProcedureDate) Then
                                        ' yes, process this claim

                                        If blnProcessSecondaryClaim Then
                                            ' there is no secondary claim already processed, but if there is already a primary it must be already paid before the secondary is produced
                                            Dim tblPriClaim As DataTable = g_IO_Execute_SQL("select  count(*) FoundOne procedure_date from Claims clm where clm.contracts_recid = " & rowClaimContract("recid") &
                                                                      " and type = 0 and plan_id = (select plan_id from DropDownList__InsurancePlans where RECID =" & tblClaimContracts.Rows(0)("PrimaryInsurancePlans_vw") & ")" &
                                                                     " and month(clms.procedure_date) = " & Month(dteProcedureDate) & " and year(clms.procedure_date) = " & Year(dteProcedureDate) &
                                                                      " and clms.status = 'C'", False)

                                            If tblPriClaim.Rows(0)("FoundOne") = 1 Then
                                                blnProcessClaim = True
                                            Else
                                                blnProcessClaim = False
                                            End If
                                            Exit For
                                        Else
                                            Exit For
                                        End If
                                    End If
                                End If

                            End If

                        Next

                    End If

                    If blnProcessSecondaryClaim Then

                        ' is there a history of claims already processed?  Find the most recent claim processed date (claims already processed are in the CLAIMS table)
                        Dim tblClaims As DataTable = g_IO_Execute_SQL("select  MAX(clm.procedure_date) as DateLastClaimPosted from Claims clm where clm.contracts_recid = " & rowClaimContract("recid") &
                                                                      " and type = 1 and plan_id = " & tblClaimContracts.Rows(0)("SecondaryInsurancePlans_vw"), False)

                        ' claim is due this month - has it already been processed for this month?

                        ' was the last claim processed this month?  If so, it should not be processed again
                        If IsDate(tblClaims.Rows(0)("DateLastClaimPosted")) Then

                            ' it should never not be a date since the contract starts out issuing an initial claim but check anyway
                            If Month(tblClaims.Rows(0)("DateLastClaimPosted")) = Month(dteProcedureDate) Then
                                ' claim has already been processed this month
                                blnProcessSecondaryClaim = False
                            Else
                                ' yes, process this claim
                            End If

                        End If

                    End If

                    btnClaimPrimary.Enabled = blnProcessPrimaryClaim Or btnClaimPrimary.Enabled
                    btnClaimSecondary.Enabled = blnProcessSecondaryClaim Or btnClaimSecondary.Enabled
                    btnClaimPrimary.Style.Add("display", "blank")

                    btnPreviewClaimPrimary.Enabled = btnClaimPrimary.Enabled
                    btnPreviewClaimSecondary.Enabled = btnClaimSecondary.Enabled

                    If blnProcessPrimaryClaim Or blnProcessSecondaryClaim Then
                        ' Add this contract recid to the pull list to be viewed in the grid
                        hidInitialcontract.Value &= "," & rowClaimContract("recid")
                    End If
                Next
            End If

            If IsNothing(Request.QueryString("i")) Then
            Else
                lblMessage.Text = "Create/Print Monthly Invoices"

                ' this is a request to process invoices only
                strIframeDestination = "frmListManager.aspx?id=UnprocessedInvoiceCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "')&vo=1&divHide=divHeader,divFooter"
                hidInitialcontract.Value = "UnprocessedInvoiceCurrentMonth_fn('" & Format(dteProcedureDate, "yyyy/MM/dd") & "')" & "&&-1"       '  "-1"  ' primer -- in case there aren't any below

                btnClaimPrimary.Style.Add("display", "none")
                btnPreviewClaimPrimary.Style.Add("display", "none")
                btnClaimSecondary.Style.Add("display", "none")
                btnPreviewClaimSecondary.Style.Add("display", "none")

                litScripts.Text &= "<script type=""text/javascript"">var InvoiceClaim = 'Invoices';</script>"

                ' pull contracts that are active and eliminate any that have invoices already processed this month
                Dim tblPendingInvoices As DataTable = g_IO_Execute_SQL("Select contracts.recid,patientfirstpay, PatientBillingFrequency_vw,contractdate" &
                    " from Contracts " &
                    "left outer join Invoices inv on  inv.InvoiceType = 'I' and inv.contracts_recid = contracts.recid and DATEPART(month, inv.PostDate) = " & Format(dteProcedureDate, "MM") &
                    " where PatientRemainingBalance > 0 and contractdate <= '" & Format(dteProcedureDate, "yyyy/MM/dd") & " 23:59:59' and inv.recid is null", False)

                For Each rowPendingInvoice In tblPendingInvoices.Rows
                    Dim blnProcessInvoice As Boolean = False


                    Select Case rowPendingInvoice("PatientBillingFrequency_vw")
                        Case 1
                            ' monthly  - then flag it to be processed
                            blnProcessInvoice = True
                        Case 2
                            ' Quarterly
                            If DateDiff(DateInterval.Month, rowPendingInvoice("PatientFirstPay"), dteProcedureDate) Mod 3 = 0 Then
                                blnProcessInvoice = True
                            End If

                        Case 3
                            ' Yearly
                            If DateDiff(DateInterval.Month, rowPendingInvoice("PatientFirstPay"), dteProcedureDate) Mod 12 = 0 Then
                                blnProcessInvoice = True
                            End If

                    End Select

                    ' add this contract recid to the list to be pulled for viewing in the grid
                    hidInitialcontract.Value &= ", " & rowPendingInvoice("recid")

                Next
            End If

            Dim arrNextActionValues() = Split(hidInitialcontract.Value, "&&")
            If arrNextActionValues(1) = "" Then
            Else
                ' RLO - too dangerous to send the recid list in the URL, create a session variable instead and send the session variable name to the list manager
                ' if this is fed list of recid's or a postback and the form received a recid list then keep them active
                '  this script will feed the list of recid's to the frmListManager in the iFrame
                Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
                Session(strSessionWhereName) = arrNextActionValues(1)   ' put the list in the SESSION variable

                ' past the name of the session variable to the form list manager via the URL list in the IFRAME
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function() {document.getElementById('ifmClaims').src = """ & strIframeDestination & "&seslst=" & strSessionWhereName & """});</script>"

            End If
        End If


    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) _
         Handles btnPrint.Click,
                 btnPreviewInvoice.Click,
                 btnClaimPrimary.Click,
                 btnPreviewClaimPrimary.Click,
                 btnClaimSecondary.Click,
                 btnPreviewClaimSecondary.Click,
                 btnManualClaim.Click

        Static Generator As System.Random = New System.Random()
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim btnPressed As Button = sender

        If btnPressed Is btnPrint Or btnPressed Is btnPreviewInvoice Then
            ' printing invoices

            'RLO 10/25/16 - let user override the date processed on the claims selected
            Dim strProcedureDate As String = ""   ' defaults to today i left blank
            If IsNothing(Request.QueryString("od")) OrElse Trim(CStr(Request.QueryString("od"))) = "" Then
                strProcedureDate = Format(Date.Now, "MM/dd/yyyy")
            Else
                If IsDate(Request.QueryString("od")) Then
                    strProcedureDate = Format(CDate(Request.QueryString("od")), "MM/dd/yyyy")
                Else
                    lblMessage.Text = "Invalid altered invoice date entered.  Please try again."
                    Exit Sub
                End If
            End If

            ' print the invoice(s)
            ' pull invoices as filtered by the user from frmListManager
            Dim strSQL_base As String = Split(Session("LM_SQL"), "order by")(0)

            Dim blnPreviewInvoices As Boolean = btnPressed Is btnPreviewInvoice

            Dim strSQL As String = ""
            Dim strTableName As String = ""

            ' strip off everything in the front of the query (leave table/view and where clause only --  most likely a view)
            strSQL = strSQL_base.Substring(UCase(strSQL_base).IndexOf(" FROM ") + 6)

            ' now extract the table of the query
            strTableName = Split(Trim(strSQL), " ")(0)

            ' now convert this query/view to a straight read of the contracts table using the where clause extracted from the user's filter
            strSQL = "select * from " & strSQL.Replace(strTableName, "contracts")

            Dim strPOFileBase = g_createInvoice(strSQL, "I", 0, blnPreviewInvoices, CDate(strProcedureDate))

            litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase
            Dim arrNextActionValues() = Split(hidInitialcontract.Value, "&&")
            Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
            Session(strSessionWhereName) = arrNextActionValues(1)

            If blnPreviewInvoices Then
            Else
                btnPreviewInvoice.Enabled = False
                btnPrint.Enabled = False
            End If


            ' paste the name of the session variable to the form list manager via the URL list in the IFRAME
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('ifmClaims').src = """ &
                "frmListManager.aspx?id=" & arrNextActionValues(0) & "&vo=1&divHide=divHeader,divFooter" &
                "&seslst=" & strSessionWhereName & """});</script>"

        Else
            ' printing claims

            Dim strProcedureDate As String = ""   ' defaults to today i left blank
            Dim blnClaimTypeIsPrimary As Boolean = btnPressed Is btnClaimPrimary Or btnPressed Is btnPreviewClaimPrimary
            Dim blnPreviewClaims As Boolean = btnPressed Is btnPreviewClaimPrimary Or btnPressed Is btnPreviewClaimSecondary

            'RLO 10/25/16 - let user override the date processed on the claims selected
            If IsNothing(Request.QueryString("od")) OrElse Trim(CStr(Request.QueryString("od"))) = "" Then
            Else
                If IsDate(Request.QueryString("od")) Then
                    strProcedureDate = Format(CDate(Request.QueryString("od")), "MM/dd/yyyy")
                Else
                    lblMessage.Text = "Invalid altered claim date entered.  Please try again."
                    Exit Sub
                End If
            End If

            Dim strClaimType As String = ""
            If blnClaimTypeIsPrimary Then
                strClaimType = "Primary"
            Else
                strClaimType = "Secondary"
            End If

            Dim tblClaims As DataTable = CreateInsuranceClaims("", blnPreviewClaims, strClaimType, strProcedureDate)

            ' add the sort (groupby) field
            tblClaims.Columns.Add("GroupBy", GetType(String))
            rptReport = New rptClaim
            rptReport.SetDataSource(tblClaims)

            Dim strMinDocumentNumber As String = ""
            Dim strMaxDocumentNumber As String = ""
            If tblClaims.Rows.Count > 0 Then
                strMinDocumentNumber = tblClaims.Rows(0)("ClaimNumber")
                strMaxDocumentNumber = tblClaims.Rows(tblClaims.Rows.Count - 1)("ClaimNumber")
                Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "__TYPE__" & Session("user_link_userid") & ".PDF"
                Dim strPOFilePath As String = Server.MapPath("downloads\")
                strPOFileBase = strPOFileBase.Replace("__TYPE__", "_claims_" & strMinDocumentNumber & "-" & strMaxDocumentNumber & "_")
                rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFilePath & strPOFileBase)
                litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase
            Else
                lblMessage.Text = "There were no claims processed for " & strProcedureDate & ".  The contracts selected likely have a claim already processed for the date specified."
            End If

            If blnPreviewClaims Then
            Else
                If blnClaimTypeIsPrimary Then
                    btnClaimPrimary.Enabled = False
                    btnPreviewClaimPrimary.Enabled = False
                Else
                    btnClaimSecondary.Enabled = False
                    btnPreviewClaimSecondary.Enabled = False
                End If
            End If


            ' past the name of the session variable to the form list manager via the URL list in the IFRAME and clear the grid
            ' strClaimType will be "Primary" or "Secondary"
            Dim arrNextActionValues() = Split(hidInitialcontract.Value, "&&")

            Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
            Session(strSessionWhereName) = arrNextActionValues(1)

            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('ifmClaims').src = """ &
                    "frmListManager.aspx?id=" & arrNextActionValues(0) & "&vo=1&divHide=divHeader,divFooter" &
                        "&seslst=" & strSessionWhereName & """});</script>"

        End If

    End Sub

End Class



