Public Class frmClaimsProcessing
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        Dim strIframeDestination As String = ""
        If IsPostBack Then
        Else
            If IsNothing(Request.QueryString("rl")) Then
            Else
                ' If a specific contract is requested then "rl" will contain the contract number(s)
                strIframeDestination = "frmListManager.aspx?id=[UnprocessedClaimsInvoicesCurrentMonth_vw]&vo=1&divHide=divHeader,divFooter" ' prepare the iframe load
                hidInitialcontract.Value = Request.QueryString("rl")

                lblMessage.Text = "Print Selected Claims and Invoices"

                ' is there insurance to be processed?
                Dim strSQL As String = "Select PrimaryInsurancePlans_vw,SecondaryInsurancePlans_vw,SecondaryCoverageAmt,PrimaryRemainingBalance,SecondaryRemainingBalance from contracts where recid = " & Request.QueryString("rl")
                Dim tblContracts As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblContracts.Rows.Count = 0 Then
                Else
                    If tblContracts.Rows(0)("PrimaryInsurancePlans_vw") > 0 Then
                        '  there is insurance coverage here

                        ' if there is insurance has there already been a primary claim processed
                        strSQL = "Select count(*) as ClaimCount from claims where contracts_recid = " & Request.QueryString("rl") & " and type = 0 and plan_id = " & tblContracts.Rows(0)("PrimaryInsurancePlans_vw")

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
                                strSQL = "Select count(*) as ClaimCount from claims where contracts_recid = " & Request.QueryString("rl") & " and type = 1 and plan_id = " & tblContracts.Rows(0)("SecondaryInsurancePlans_vw")
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
                    Dim tblInvoices As DataTable = g_IO_Execute_SQL("select MAX(inv.postdate) as postdate from Invoices inv where InvoiceType = 'I' and inv.contracts_recid = " & Request.QueryString("rl") _
                                                                 , False)

                    ' was the last Invoice processed this month?  If so, it should not be processed again
                    If IsDate(tblInvoices.Rows(0)("postdate")) Then

                        ' it should never not be a date since the contract starts out issuing an initial invoice
                        If Month(tblInvoices.Rows(0)("postdate")) = Month(Date.Now) Then
                            ' claim has already been processed this month
                            btnPrint.Enabled = False
                            btnPreviewInvoice.Enabled = False
                        Else
                            ' process this invoice
                        End If

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
                hidInitialcontract.Value = "-1"  ' primer -- in case there aren't any below


                '  create a list of contract recid's to display and store them in hidInitialcontract 

                ' query to pull contracts that are active 
                Dim strSQL As String = "Select recid,PatientFirstPay,PrimaryBillingFrequency_vw,SecondaryBillingFrequency_vw,contractdate,PrimaryCoverageAmt,SecondaryCoverageAmt,PrimaryInsurancePlans_vw,SecondaryInsurancePlans_vw from contracts where status_vw = 1 and ContractDate <= GetDate()  "

                '  pull only contracts that have primary or secondary insurance flags set
                strSQL &= " and (PrimaryInsurancePlans_vw > -1 or SecondaryInsurancePlans_vw > -1)"
                Dim tblClaimContracts As DataTable = g_IO_Execute_SQL(strSQL, False)

                btnClaimPrimary.Enabled = False
                btnClaimSecondary.Enabled = False

                For Each rowClaimContract In tblClaimContracts.Rows
                    Dim blnProcessPrimaryClaim As Boolean = False
                    Dim blnProcessSecondaryClaim As Boolean = False

                    If Request.QueryString("c") = "1" Then
                        strIframeDestination = "frmListManager.aspx?id=PrimaryClaimsListingMain_vw&vo=1&divHide=divHeader,divFooter"    ' prepare the iframe load

                        lblMessage.Text = "Create/Print Primary Monthly Claims"

                        '  what is the frequency this claim is supposed to be processed?    (determine based on start of contract date, not the last time this claim was processed)


                        Select Case rowClaimContract("PrimaryBillingFrequency_vw")
                            Case 1
                                ' monthly  - then flag it to be processed
                                blnProcessPrimaryClaim = True
                            Case 2
                                ' Quarterly
                                Dim intMonthsSinceContractStarted As Integer = DateDiff(DateInterval.Month, rowClaimContract("contractdate"), Date.Now)

                                If intMonthsSinceContractStarted Mod 3 = 0 Then
                                    '  Every third month will generate a Mod 3 value of 0 -  flag it to process every third month since the start of the contract
                                    blnProcessPrimaryClaim = True
                                End If

                            Case 3
                                ' Yearly
                                Dim intMonthsSinceContractStarted As Integer = DateDiff(DateInterval.Month, rowClaimContract("contractdate"), Date.Now)
                                If intMonthsSinceContractStarted Mod 12 = 0 Then
                                    '  Every twelth month will generate a Mod 12 value of 0 -  flag it to process every twelth month since the start of the contract
                                    blnProcessPrimaryClaim = True
                                End If
                            Case Else
                                blnProcessPrimaryClaim = False
                        End Select
                    Else
                        blnProcessPrimaryClaim = False
                    End If

                    If Request.QueryString("c") = "2" Then
                        strIframeDestination = "frmListManager.aspx?id=SecondaryClaimsListingMain_vw&vo=1&divHide=divHeader,divFooter"    ' prepare the iframe load

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
                                    If DateDiff(DateInterval.Month, rowClaimContract("contractdate"), Date.Now) Mod 3 = 0 Then
                                        blnProcessSecondaryClaim = True
                                    End If
                                Case 3
                                    ' Yearly
                                    If DateDiff(DateInterval.Month, rowClaimContract("contractdate"), Date.Now) Mod 12 = 0 Then
                                        blnProcessSecondaryClaim = True
                                    End If
                                Case Else
                                    blnProcessSecondaryClaim = False

                            End Select
                        End If
                    End If

                    If blnProcessPrimaryClaim Then
                        ' it was determined above to process this claim for the primary insurance company

                        ' if a primary claim has been processed it will be saved on a single record on the CLAIMS table,  see if such a record exists before processing this claim 

                        ' is there a history of claims already processed?  Find the most recent claim processed date (claims already processed are in the CLAIMS table)
                        Dim tblClaims As DataTable = g_IO_Execute_SQL("select  MAX(clm.procedure_date) as DateLastClaimPosted from Claims clm where clm.contracts_recid = " & rowClaimContract("recid") & _
                                                                      " and type = 0 and plan_id = " & tblClaimContracts.Rows(0)("PrimaryInsurancePlans_vw"), False)

                        ' primary claim is due this month - has it already been processed for this month?

                        ' was the last claim processed this month?  If so, it should not be processed again
                        If IsDate(tblClaims.Rows(0)("DateLastClaimPosted")) Then

                            ' it should never not be a date since the contract starts out issuing an initial claim but check anyway
                            If Month(tblClaims.Rows(0)("DateLastClaimPosted")) = Month(Date.Now) Then
                                ' claim has already been processed this month
                                blnProcessPrimaryClaim = False
                            Else
                                ' yes, process this claim
                            End If

                        End If

                    End If

                    If blnProcessSecondaryClaim Then
                        ' it was determined above to process this secondary claim for the primary or secondary insurance company

                        ' if a primary and/or secondary claim has been processed this month one or both will be saved on a single record on the CLAIMS table,  see if such a record exists before processing this claim or claims

                        ' is there a history of claims already processed?  Find the most recent claim processed date (claims already processed are in the CLAIMS table)
                        Dim tblClaims As DataTable = g_IO_Execute_SQL("select  MAX(clm.procedure_date) as DateLastClaimPosted from Claims clm where clm.contracts_recid = " & rowClaimContract("recid") & _
                                                                      " and type = 1 and plan_id = " & tblClaimContracts.Rows(0)("SecondaryInsurancePlans_vw"), False)

                        ' claim is due this month - has it already been processed for this month?

                        ' was the last claim processed this month?  If so, it should not be processed again
                        If IsDate(tblClaims.Rows(0)("ProcedureDate")) Then

                            ' it should never not be a date since the contract starts out issuing an initial claim but check anyway
                            If Month(tblClaims.Rows(0)("DateLastClaimPosted")) = Month(Date.Now) Then
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
                strIframeDestination = "frmListManager.aspx?id=UnprocessedPrimaryInvoiceCurrentMonth_vw&vo=1&divHide=divHeader,divFooter"
                btnClaimPrimary.Style.Add("display", "none")
                btnPreviewClaimPrimary.Style.Add("display", "none")
                btnClaimSecondary.Style.Add("display", "none")
                btnPreviewClaimSecondary.Style.Add("display", "none")

                litScripts.Text &= "<script type=""text/javascript"">var InvoiceClaim = 'Invoices';</script>"
                hidInitialcontract.Value = "-1"  ' primer -- in case there aren't any below

                Dim tblPendingInvoices As DataTable = g_IO_Execute_SQL("Select recid,patientfirstpay, PatientBillingFrequency_vw,contractdate" & _
                    " from Contracts where PatientRemainingBalance > 0 and contractdate <= getdate()", False)
                For Each rowPendingInvoice In tblPendingInvoices.Rows
                    Dim blnProcessInvoice As Boolean = False


                    Select Case rowPendingInvoice("PatientBillingFrequency_vw")
                        Case 1
                            ' monthly  - then flag it to be processed
                            blnProcessInvoice = True
                        Case 2
                            ' Quarterly
                            If DateDiff(DateInterval.Month, rowPendingInvoice("PatientFirstPay"), Date.Now) Mod 3 = 0 Then
                                blnProcessInvoice = True
                            End If

                        Case 3
                            ' Yearly
                            If DateDiff(DateInterval.Month, rowPendingInvoice("PatientFirstPay"), Date.Now) Mod 12 = 0 Then
                                blnProcessInvoice = True
                            End If

                    End Select


                    If blnProcessInvoice Then

                        ' look for the last invoice processed for this contract
                        Dim tblInvoices As DataTable = g_IO_Execute_SQL("select MAX(inv.postdate) as postdate from Invoices inv where inv.InvoiceType = 'I' and inv.contracts_recid = " & rowPendingInvoice("recid"), False)

                        ' was the last Invoice processed this month?  If so, it should not be processed again
                        If IsDate(tblInvoices.Rows(0)("postdate")) Then

                            ' it should never not be a date since the contract starts out issuing an initial invoice
                            If Month(tblInvoices.Rows(0)("postdate")) = Month(Date.Now) Then
                                ' claim has already been processed this month
                                blnProcessInvoice = False
                            Else
                                ' process this claim
                            End If

                        End If

                    End If


                    If blnProcessInvoice Then
                        ' add this contract recid to the list to be pulled for viewing in the grid
                        hidInitialcontract.Value &= "," & rowPendingInvoice("recid")
                    End If

                Next
            End If

        If hidInitialcontract.Value = "" Then
        Else
            ' RLO - too dangerous to send the recid list in the URL, create a session variable instead and send the session variable name to the list manager
            ' if this is fed list of recid's or a postback and the form received a recid list then keep them active
            '  this script will feed the list of recid's to the frmListManager in the iFrame
            Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
            Session(strSessionWhereName) = hidInitialcontract.Value   ' put the list in the SESSION variable

            ' past the name of the session variable to the form list manager via the URL list in the IFRAME
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('ifmClaims').src = '" & strIframeDestination & "&seslst=" & strSessionWhereName & "'});</script>"

        End If
        End If


    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click, btnPreviewInvoice.Click, btnClaimPrimary.Click, btnPreviewClaimPrimary.Click, btnClaimSecondary.Click, btnPreviewClaimSecondary.Click

        Static Generator As System.Random = New System.Random()
        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim btnPressed As Button = sender

        If btnPressed Is btnPrint Or btnPressed Is btnPreviewInvoice Then

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

            ' 6/9/15 CS Not sure why we were sending over an "M" in some scenarios...normal invoice processing should have "I" as invoice type
            'Dim strPOFileBase = g_createInvoice(strSQL, IIf(hidInitialcontract.Value = "", "M", "I"), 0, blnPreviewInvoices)
            Dim strPOFileBase = g_createInvoice(strSQL, "I", 0, blnPreviewInvoices)

            litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase

            Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
            Session(strSessionWhereName) = "-1"   ' put the list in the SESSION variable

            ' past the name of the session variable to the form list manager via the URL list in the IFRAME
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('ifmClaims').src = '" & _
                "frmListManager.aspx?id=UnprocessedPrimaryInvoiceCurrentMonth_vw&vo=1&divHide=divHeader,divFooter" & _
                "&seslst=" & strSessionWhereName & "'});</script>"

        Else
            ' printing claims
            Dim blnTestRun As Boolean = btnPressed Is btnPreviewClaimPrimary Or btnPressed Is btnPreviewClaimSecondary
            Dim strClaimType As String = ""
            If btnPressed Is btnClaimPrimary Or btnPressed Is btnPreviewClaimPrimary Then
                strClaimType = "Primary"
            Else
                strClaimType = "Secondary"
            End If
            Dim tblClaims As DataTable = CreateInsuranceClaims("", blnTestRun, strClaimType, "")

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
            End If

            Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
            Session(strSessionWhereName) = "-1"   ' put the list in the SESSION variable
            If blnTestRun Then
            Else
                ' past the name of the session variable to the form list manager via the URL list in the IFRAME and clear the grid
                If strClaimType = "Primary" Then
                    litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('ifmClaims').src = '" & _
                        "frmListManager.aspx?id=PrimaryClaimsListingMain_vw&vo=1&divHide=divHeader,divFooter" & _
                        "&seslst=" & strSessionWhereName & "'});</script>"
                Else
                    litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('ifmClaims').src = '" & _
                        "frmListManager.aspx?id=SecondaryClaimsListingMain_vw&vo=1&divHide=divHeader,divFooter" & _
                        "&seslst=" & strSessionWhereName & "'});</script>"
                End If
            End If

        End If

    End Sub

End Class



