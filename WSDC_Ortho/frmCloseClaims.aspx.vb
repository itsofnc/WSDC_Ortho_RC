Public Class frmCloseClaims

    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        If IsPostBack Then
        Else
            Dim m_strPrimaryKey As String = " recid "

            If IsNothing(Request.QueryString("cid")) Then
            Else
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = Request.QueryString("cid") 'cid is actually the claim recid

                ' make sure contract exists, and gather data associated with it
                txtClaimID.Value = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))

                Dim strSql As String = "select c.DateProcessed, c.insurance_name, c.ClaimNumber, c.patient_name_last, " & _
                    "c.patient_name_first, c.patient_name_mid, c.contracts_recid, c.ClaimNumber, c.ChartNumber, " & _
                    "c.Type, c.procedure_amount, c.AmountPaid, con.PatientNumber " & _
                    " from claims c left outer join contracts con on con.ChartNumber = c.ChartNumber " & _
                    " where c.recid = '" & txtClaimID.Value & "'"
                Dim tblClaims As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblClaims.Rows.Count > 0 Then
                    litDate.Text = tblClaims.Rows(0)("DateProcessed")
                    litInsurance.Text = tblClaims.Rows(0)("insurance_name")
                    litClaimNumber.Text = tblClaims.Rows(0)("ClaimNumber")
                    litPatient.Text = Trim(tblClaims.Rows(0)("patient_name_last")) & ", " & _
                        Trim(tblClaims.Rows(0)("patient_name_first")) & " " & _
                        Trim(tblClaims.Rows(0)("patient_name_mid"))
                    txtContract_RECID.Text = tblClaims.Rows(0)("contracts_recid")
                    txtClaimNumber.Value = tblClaims.Rows(0)("ClaimNumber")
                    txtChartNumber.Value = tblClaims.Rows(0)("ChartNumber")
                    txtInsType.Value = tblClaims.Rows(0)("Type")
                    txtClaimBalance.Value = CType(Math.Round(tblClaims.Rows(0)("procedure_amount"), 2) - Math.Round(tblClaims.Rows(0)("AmountPaid"), 2), String)
                    txtClaimAmount.Value = CType(Math.Round(tblClaims.Rows(0)("procedure_amount"), 2), String)
                    litAmount.Text = "$" & CType(Math.Round(tblClaims.Rows(0)("procedure_amount"), 2), String) & " (Expected)" & _
                        ",  $" & CType(Math.Round(tblClaims.Rows(0)("AmountPaid"), 2), String) & " (Paid)" & _
                        ",  $" & CType(Math.Round(tblClaims.Rows(0)("procedure_amount"), 2) - Math.Round(tblClaims.Rows(0)("AmountPaid"), 2), String) & " (Open Balance)"
                    txtPatientNumber.Value = tblClaims.Rows(0)("PatientNumber")
                End If
            End If
        End If

    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) _
        Handles btnConfirm.Click

        ' close claim
        Dim strSQL As String = "update claims set status='C'" & _
            ", AmountPassedToPatient=" & IIf(Trim(txtCharge.Text) = "", 0.0, txtCharge.Text) & _
            ", ClosedDate='" & Date.Now() & "'" & _
            ", ClosedReason='" & txtReason.Text.Replace("'", "''") & "'" & _
            " where recid='" & txtClaimID.Value & "'"
        g_IO_Execute_SQL(strSQL, False)

        ' create a payment record for an adjustment to close out the claim
        Dim intLastPaymentRecid As Integer = -1
        strSQL = "insert into payments (DatePosted, sys_users_recid, patientNumber, ChartNumber, PrimaryAmount, SecondaryAmount, ApplyToClaim, " & _
                            "PaymentType, PaymentReference, Contract_recid, Invoices_recid, claimNumber, baserecid, PaymentSelection, orig_Payment, " & _
                            "Comments, PayerName,PaymentFor)" & _
                            " values (" & _
                            "'" & Format(Date.Now, "MM/dd/yyyy") & "'," & _
                            Session("user_link_id") & "," & _
                            "'" & Left(txtPatientNumber.Value, 6) & "'," & _
                            "'" & txtChartNumber.Value & "'," & _
                            IIf(txtInsType.Value = "0", txtClaimBalance.Value, 0.0) & "," & _
                            IIf(txtInsType.Value = "1", txtClaimBalance.Value, 0.0) & "," & _
                             txtClaimBalance.Value & "," & _
                            "'14'," & _
                            "'ADJ-Closed Claim'," & _
                            "'" & txtContract_RECID.Text & "'," & _
                            "'-1'," & _
                            "'" & txtClaimNumber.Value & "'," & _
                            intLastPaymentRecid & "," & _
                            "'" & IIf(txtInsType.Value = "0", "PrimaryAmount", "SecondaryAmount") & "'," & _
                            txtClaimBalance.Value & "," & _
                            "'" & txtReason.Text.Replace("'", "''") & "'," & _
                            "'" & litInsurance.Text.Replace("'", "''") & "'," & _
                            "'1'" & _
                            ")"
        g_IO_Execute_SQL(strSQL, False)

        ' update BaseRECID on payment record just created
        intLastPaymentRecid = g_IO_GetLastRecId()
        strSQL = "update Payments set BaseRecid = " & intLastPaymentRecid & " where recid=" & intLastPaymentRecid

        g_IO_Execute_SQL(strSQL, False)

        If (IsNumeric(txtCharge.Text) AndAlso CDec(txtCharge.Text) > 0) Then
            ' Update contract with note that remaining claim amount charged to patient
            strSQL = "Select Comments from contracts where recid = " & txtContract_RECID.Text
            Dim tblContract As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblContract.Rows.Count > 0 Then
                Dim strComments As String = Format(Date.Now, "MM/dd/yyyy") & " " & Session("user_name") & ": $ " & txtCharge.Text & " invoiced to patient. Claim # " & Trim(txtClaimNumber.Value) & " was closed w/ balance due." & vbCrLf & vbCrLf & tblContract.Rows(0)("Comments")
                strSQL = "UPDATE Contracts SET Comments = '" & strComments & "' WHERE recid = " & txtContract_RECID.Text
                g_IO_Execute_SQL(strSQL, False)
            End If

            ' create session variable to use during CreateInvoice processing                
            System.Web.HttpContext.Current.Session("InvoiceDescr") = "Amount Not Covered By Insurance (Claim # " & Trim(txtClaimNumber.Value) & ") "
            Dim strPOFileBase As String = g_createInvoice("Select * from contracts where recid = " & txtContract_RECID.Text, "M", txtCharge.Text, Date.Now)

            Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
            Session(strSessionWhereName) = "-1"   ' put the list in the SESSION variable

            litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase

            litMessage.Text &= "Claim was successfully closed & Invoice Generated to Patient for $ " & CType(txtCharge.Text, String) & "."
            litScripts.Text &= "<script>jQuery(""#closeClaimInfo"").addClass(""hidden""); jQuery(""#divMessage"").removeClass(""hidden""); jQuery('#loading_indicator').hide();</script>"
        Else
            litMessage.Text &= "Claim was successfully closed."
            litScripts.Text &= "<script>jQuery(""#closeClaimInfo"").addClass(""hidden""); jQuery(""#divMessage"").removeClass(""hidden""); jQuery('#loading_indicator').hide();</script>"
        End If


    End Sub
End Class