Public Class frmClaimsRemove
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If
    End Sub
    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        For Each strClaimNumber As String In Split(txtClaimNumbers.Text, ",")
            ReverseClaimPayments(strClaimNumber)
        Next

    End Sub

    Private Sub ReverseClaimPayments(ByVal ClaimNumber As String)
        Dim tblClaims As DataTable = g_IO_Execute_SQL("Select * from claims where claimNumber in ('" & ClaimNumber.Replace(",", "','") & "')", False)

        ' is there a claim available to post a payment to?
        For Each rowClaims As DataRow In tblClaims.Rows

            Dim tblPayments As DataTable = g_IO_Execute_SQL("Select * from payments where chartnumber = '" & rowClaims("ChartNumber") & "'" & _
                                                          " and claimnumber = '" & rowClaims("claimnumber") & "'" & _
                                                            " order by recid", False)

            ' loop open payment records for primary (type = 0) or secondary (type=1) claims and process them until the claim is paid or until the payments available are exhausted

            For Each rowPayment As DataRow In tblPayments.Rows

                g_IO_Execute_SQL("Update payments set claimnumber = '-1' where recid = " & rowPayment("recid"), False)

            Next

            ' reset amountpaid on the claim and delete it
            g_IO_Execute_SQL("delete from claims where claimnumber = '" & rowClaims("ClaimNumber") & "'", False)

            If rowClaims("Type") = 0 Then
                g_IO_Execute_SQL("update contracts set PrimaryRemainingBalance = PrimaryRemainingBalance + " & rowClaims("procedure_amount") & _
                                 ",PrimaryAmountBilled = PrimaryAmountBilled - " & rowClaims("procedure_amount") & _
                                 " where recid = " & rowClaims("contracts_recid"), False)
            Else
                g_IO_Execute_SQL("update contracts set secondaryRemainingBalance = SecondaryRemainingBalance + " & rowClaims("procedure_amount") & _
                                 ",SecondaryAmountBilled = SecondaryAmountBilled - " & rowClaims("procedure_amount") & _
                                 " where recid = " & rowClaims("contracts_recid"), False)
            End If

        Next
    End Sub

End Class