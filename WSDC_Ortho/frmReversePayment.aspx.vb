Public Class frmReversePayment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        If IsPostBack Then
        Else
            Dim m_strPrimaryKey As String = " recid "

            If IsNothing(Request.QueryString("pid")) Then

            Else
                System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey)) = Request.QueryString("pid") 'cid is actually the payment recid

                ' make sure contract exists, and gather data associated with it
                txtPaymentID.Value = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))
                ' enable NSF fee to charge back to patient
                txtNSF.Enabled = True
                Dim strSql As String = "select * from Payments_vw where recid = '" & txtPaymentID.Value & "'"
                Dim tblPayment As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblPayment.Rows.Count > 0 Then
                    txtPaymentSelection.Value = tblPayment.Rows(0)("PaymentSelection")
                    txtPaymentType.Value = tblPayment.Rows(0)("PaymentType")
                    litDate.Text = tblPayment.Rows(0)("DatePosted")
                    litPatient.Text = Trim(tblPayment.Rows(0)("LastName")) & ", " & Trim(tblPayment.Rows(0)("FirstName"))
                    If tblPayment.Rows(0)("PaymentSelection") = "PatientAmount" Then
                        litScripts.Text &= "<script>jQuery(""#NSFOption"").removeClass(""hidden"");</script>"

                        txtContract_RECID.Text = tblPayment.Rows(0)("contract_recid")

                        Dim strInvoiceDetail As String = ""
                        Dim strDetailDelim As String = " Invoices: "
                        Dim tblPaymentsDistribution As DataTable = g_IO_Execute_SQL("Select i.invoiceno, p.PatientAmount from payments p,invoices i where p.invoices_recid = i.recid and baserecid = " & tblPayment.Rows(0)("recid"), False)

                        For Each rowPayment In tblPaymentsDistribution.Rows
                            strInvoiceDetail &= strDetailDelim & rowPayment("invoiceno") & " - " & FormatCurrency(rowPayment("PatientAmount"))
                            strDetailDelim = ", "
                        Next

                        litAmount.Text = tblPayment.Rows(0)("orig_payment") & strInvoiceDetail.Replace("Invoices", IIf(strInvoiceDetail.Contains(","), "Invoices", "Invoice"))
                    ElseIf tblPayment.Rows(0)("PaymentSelection") = "PrimaryAmount" Then
                        litScripts.Text &= "<script>jQuery(""#NSFOption"").addClass(""hidden"");</script>"
                        Dim strClaimDetail As String = ""
                        Dim strDetailDelim As String = " Claims: "
                        Dim tblPaymentsDistribution As DataTable = g_IO_Execute_SQL("Select ClaimNumber, PrimaryAmount from payments p where PaymentSelection = 'PrimaryAmount' and ClaimNumber <> '' and baserecid = " & tblPayment.Rows(0)("recid"), False)
                        For Each rowPayment In tblPaymentsDistribution.Rows
                            strClaimDetail &= strDetailDelim & rowPayment("ClaimNumber") & " - " & FormatCurrency(rowPayment("PrimaryAmount"))
                            strDetailDelim = ", "
                        Next
                        litAmount.Text = tblPayment.Rows(0)("orig_payment") & strClaimDetail.Replace("Claims", IIf(strClaimDetail.Contains(","), "Claims", "Claim"))
                    ElseIf tblPayment.Rows(0)("PaymentSelection") = "SecondaryAmount" Then
                        litScripts.Text &= "<script>jQuery(""#NSFOption"").addClass(""hidden"");</script>"
                        Dim strClaimDetail As String = ""
                        Dim strDetailDelim As String = " Claims: "
                        Dim tblPaymentsDistribution As DataTable = g_IO_Execute_SQL("Select ClaimNumber, SecondaryAmount from payments p where PaymentSelection = 'SecondaryAmount' and ClaimNumber <> '' and baserecid = " & tblPayment.Rows(0)("recid"), False)
                        For Each rowPayment In tblPaymentsDistribution.Rows
                            strClaimDetail &= strDetailDelim & rowPayment("ClaimNumber") & " - " & FormatCurrency(rowPayment("SecondaryAmount"))
                            strDetailDelim = ", "
                        Next
                        litAmount.Text = tblPayment.Rows(0)("orig_payment") & strClaimDetail.Replace("Claims", IIf(strClaimDetail.Contains(","), "Claims", "Claim"))
                    End If
                End If
            End If
        End If
        
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) _
        Handles btnConfirm.Click, btnCancel.Click

        If sender Is btnConfirm Then
            Dim strPaymentType As String = "20"
            If (IsNumeric(txtNSF.Text) AndAlso CDec(txtNSF.Text) > 0) Then
                strPaymentType = "19"
            End If

            Dim intNewPaymentsBaseRecid As Integer = -1
            If txtPaymentSelection.Value = "PatientAmount" Then
                g_ReverseInvoicePayment(txtPaymentID.Value, strPaymentType, intNewPaymentsBaseRecid)
            Else
                g_ReverseClaimPayment(txtPaymentID.Value, strPaymentType, intNewPaymentsBaseRecid, txtPaymentType.Value)
            End If
            ' update comment on payment reversal to reflect what was entered on this screen
            If Trim(txtReason.Text) = "" Then
                txtReason.Text = "Reversed Payment"
            End If
            Dim strSQL As String = "update payments set Comments = '" & txtReason.Text.Replace("'", "''") & "' where recid = " & intNewPaymentsBaseRecid

            If (IsNumeric(txtNSF.Text) AndAlso CDec(txtNSF.Text) > 0) Then
                ' 6/8/15 CS Per Rebecca: Update contract with note that NSF charged to patient
                strSQL = "Select Comments from contracts where recid = " & txtContract_RECID.Text
                Dim tblContract As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblContract.Rows.Count > 0 Then
                    Dim strComments As String = Format(Date.Now, "MM/dd/yyyy") & " " & Session("user_name") & ": $ " & txtNSF.Text.Replace("'", "''") & " NSF Fee invoiced to patient." & vbCrLf & vbCrLf & tblContract.Rows(0)("Comments")
                    strSQL = "UPDATE Contracts SET Comments = '" & strComments & "' WHERE recid = " & txtContract_RECID.Text
                    g_IO_Execute_SQL(strSQL, False)
                End If
                ' end of mod 6/8/15

                ' 6/9/15 CS create session variable to use during g_CreateInvoice processing
                System.Web.HttpContext.Current.Session("InvoiceDescr") = "NSF Fee"
                ' end of mod 6/9/15

                Dim strPOFileBase As String = g_createInvoice("Select * from contracts where recid = " & txtContract_RECID.Text, "M", txtNSF.Text)

                Dim strSessionWhereName As String = "CPWhere" & Trim(CStr(TimeOfDay.Second))   ' create a unique session variable name used to send the list to the frmListManager
                Session(strSessionWhereName) = "-1"   ' put the list in the SESSION variable

                litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase

                litMessage.Text &= "<h4>Invoice Generated (downloaded to your browser...Open/Save when prompted).</h4><br /><br />" & _
                    "<h4><a href=""frmListManager.aspx?id=PaymentsPosted_vw&vo=true&perm=1000&add=PaymentPosting.aspx"">Click here to return to Payments Posted Listing</a></h4>"
                litScripts.Text &= "<script>jQuery(""#reversePaymentInfo"").addClass(""hidden""); jQuery(""#divMessage"").removeClass(""hidden""); jQuery('#loading_indicator').hide();</script>"
            Else
                litMessage.Text &= "<h4>Payment Reversal Complete.</h4><br /><br />" & _
                    "<h4><a href=""frmListManager.aspx?id=PaymentsPosted_vw&vo=true&perm=1000&add=PaymentPosting.aspx"">Click here to return to Payments Posted Listing</a></h4>"
                litScripts.Text &= "<script>jQuery(""#reversePaymentInfo"").addClass(""hidden""); jQuery(""#divMessage"").removeClass(""hidden""); jQuery('#loading_indicator').hide();</script>"
            End If
        Else
            Response.Redirect("frmListManager.aspx?id=PaymentsPosted_vw&vo=true&perm=1001&add=PaymentPosting.aspx")
        End If

    End Sub

End Class