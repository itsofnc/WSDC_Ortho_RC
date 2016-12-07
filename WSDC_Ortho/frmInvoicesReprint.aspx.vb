Public Class frmInvoicesReprint
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        Dim strPOFileBase As String = g_ReprintInvoiceByInvoiceNumber("", txtInvoiceNumbers.Text)

        If strPOFileBase = "" Then
        Else
            litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase
        End If

    End Sub
End Class