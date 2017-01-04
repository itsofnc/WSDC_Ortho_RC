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
                ' 1/4/17 CS Now sending in payments.BaseRecid so that we can loop through the split of the entire payment and get all invoices, etc associated with it.
                ' make sure contract exists, and gather data associated with it
                txtPaymentID.Value = System.Web.HttpContext.Current.Session(Trim(m_strPrimaryKey))
                ' enable NSF fee to charge back to patient
                txtNSF.Enabled = True
                ' pull the base payment record to get summary data
                Dim strSql As String = "select * from Payments_vw where Recid = '" & txtPaymentID.Value & "'"
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
                        Dim strDetailDelim As String = " Invoices - "
                        strSql = "Select i.invoiceno, p.PatientAmount, p.invoices_recid from payments p LEFT OUTER JOIN Invoices i ON p.Invoices_RECID = i.recid where baserecid  = " & tblPayment.Rows(0)("BaseRecid") & " order by invoices_recid desc"
                        Dim tblPaymentsDistribution As DataTable = g_IO_Execute_SQL(strSql, False)

                        For Each rowPayment In tblPaymentsDistribution.Rows
                            If rowPayment("invoices_recid") = "-1" Then
                                strInvoiceDetail &= strDetailDelim & "Waiting To Apply To Next Invoice: " & FormatCurrency(rowPayment("PatientAmount"))
                            ElseIf rowPayment("invoices_recid") = "-99" Then
                                strInvoiceDetail &= strDetailDelim & "Applied To Account Balance: " & FormatCurrency(rowPayment("PatientAmount"))
                            Else
                                strInvoiceDetail &= strDetailDelim & rowPayment("invoiceno") & ": " & FormatCurrency(rowPayment("PatientAmount"))
                            End If
                            strDetailDelim = ", "
                        Next

                        litAmount.Text = "$ " & tblPayment.Rows(0)("orig_payment") & " (" & strInvoiceDetail.Replace("Invoices", IIf(strInvoiceDetail.Contains(","), "Invoices", "Invoice")) & " )"
                    ElseIf tblPayment.Rows(0)("PaymentSelection") = "PrimaryAmount" Then
                        litScripts.Text &= "<script>jQuery(""#NSFOption"").addClass(""hidden"");</script>"
                        Dim strClaimDetail As String = ""
                        Dim strDetailDelim As String = " Claims - "
                        Dim tblPaymentsDistribution As DataTable = g_IO_Execute_SQL("Select ClaimNumber, PrimaryAmount from payments p where PaymentSelection = 'PrimaryAmount' and baserecid = " & tblPayment.Rows(0)("BaseRecid") & " order by ClaimNumber desc", False)
                        For Each rowPayment In tblPaymentsDistribution.Rows
                            If rowPayment("ClaimNumber") = "-1" Or Trim(rowPayment("ClaimNumber")) = "" Then
                                strClaimDetail &= strDetailDelim & "Waiting To Apply To Next Claim: " & FormatCurrency(rowPayment("PrimaryAmount"))
                            ElseIf rowPayment("ClaimNumber") = "-99" Then
                                strClaimDetail &= strDetailDelim & "Applied To Account Balance: " & FormatCurrency(rowPayment("PrimaryAmount"))
                            Else
                                strClaimDetail &= strDetailDelim & rowPayment("ClaimNumber") & " - " & FormatCurrency(rowPayment("PrimaryAmount"))
                            End If
                            strDetailDelim = ", "
                        Next
                        litAmount.Text = "$ " & tblPayment.Rows(0)("orig_payment") & " (" & strClaimDetail.Replace("Claims", IIf(strClaimDetail.Contains(","), "Claims", "Claim")) & " )"
                    ElseIf tblPayment.Rows(0)("PaymentSelection") = "SecondaryAmount" Then
                        litScripts.Text &= "<script>jQuery(""#NSFOption"").addClass(""hidden"");</script>"
                        Dim strClaimDetail As String = ""
                        Dim strDetailDelim As String = " Claims - "
                        Dim tblPaymentsDistribution As DataTable = g_IO_Execute_SQL("Select ClaimNumber, SecondaryAmount from payments p where PaymentSelection = 'SecondaryAmount' and baserecid = " & tblPayment.Rows(0)("BaseRecid") & " order by ClaimNumber desc", False)
                        For Each rowPayment In tblPaymentsDistribution.Rows
                            If rowPayment("ClaimNumber") = "-1" Or Trim(rowPayment("ClaimNumber")) = "" Then
                                strClaimDetail &= strDetailDelim & "Waiting To Apply To Next Claim: " & FormatCurrency(rowPayment("SecondaryAmount"))
                            ElseIf rowPayment("ClaimNumber") = "-99" Then
                                strClaimDetail &= strDetailDelim & "Applied To Account Balance: " & FormatCurrency(rowPayment("SecondaryAmount"))
                            Else
                                strClaimDetail &= strDetailDelim & rowPayment("ClaimNumber") & " - " & FormatCurrency(rowPayment("SecondaryAmount"))
                            End If
                            strDetailDelim = ", "
                        Next
                        litAmount.Text = "$ " & tblPayment.Rows(0)("orig_payment") & " (" & strClaimDetail.Replace("Claims", IIf(strClaimDetail.Contains(","), "Claims", "Claim")) & " )"
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

            ' set a default comment, in case user didn't enter anything...goes to payment reversal record(s)
            If Trim(txtReason.Text) = "" Then
                txtReason.Text = "Reversed Payment"
            End If

            Dim intNewPaymentsBaseRecid As Integer = -1
            If txtPaymentSelection.Value = "PatientAmount" Then
                g_ReverseInvoicePayment(txtPaymentID.Value, strPaymentType, txtReason.Text, intNewPaymentsBaseRecid)
            Else
                g_ReverseClaimPayment(txtPaymentID.Value, strPaymentType, txtReason.Text, intNewPaymentsBaseRecid, txtPaymentType.Value)
            End If

            ' 1/4/17 CS The comments the user entered are now being sent over to g_ReverseInvoicePayment now and part of the insert routine there...
            '' update comment on payment reversal to reflect what was entered on this screen
            'If Trim(txtReason.Text) = "" Then
            '    txtReason.Text = "Reversed Payment"
            'End If
            'Dim strSQL As String = "update payments set Comments = '" & txtReason.Text.Replace("'", "''") & "' where recid = " & intNewPaymentsBaseRecid

            If (IsNumeric(txtNSF.Text) AndAlso CDec(txtNSF.Text) > 0) Then
                ' 6/8/15 CS Per Rebecca: Update contract with note that NSF charged to patient
                Dim strSQL As String = "Select Comments from contracts where recid = " & txtContract_RECID.Text
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

                litMessage.Text &= "<h4>Invoice Generated (downloaded to your browser...Open/Save when prompted).</h4><br /><br />" &
                    "<h4><a href=""frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&vo=true&perm=1000&add=PaymentPosting.aspx"">Click here to return to Payments Posted Listing</a></h4>"
                litScripts.Text &= "<script>jQuery(""#reversePaymentInfo"").addClass(""hidden""); jQuery(""#divMessage"").removeClass(""hidden""); jQuery('#loading_indicator').hide();</script>"
            Else
                litMessage.Text &= "<h4>Payment Reversal Complete.</h4><br /><br />" &
                    "<h4><a href=""frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&vo=true&perm=1000&add=PaymentPosting.aspx"">Click here to return to Payments Posted Listing</a></h4>"
                litScripts.Text &= "<script>jQuery(""#reversePaymentInfo"").addClass(""hidden""); jQuery(""#divMessage"").removeClass(""hidden""); jQuery('#loading_indicator').hide();</script>"
            End If
        Else
            Response.Redirect("frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&vo=true&perm=1001&add=PaymentPosting.aspx")
        End If

    End Sub

End Class