Public Class frmSessionTimeout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        litProjectTitle.Text = g_SiteDisplayName
        litMessage.Text = "<br /><br /> Your session has ended."
        Session.RemoveAll()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Response.Redirect("Default.aspx")
    End Sub

End Class