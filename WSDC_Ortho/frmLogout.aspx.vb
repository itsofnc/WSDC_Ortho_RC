Public Class frmLogout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        litProjectTitle.Text = g_SiteDisplayName
        litMessage.Text = "<br /><br /> You have been logged out."
        Session.RemoveAll()

        ' 2/27/16 cpb add ability to retrun directly to login page after logout
        If IsNothing(ConfigurationManager.AppSettings("logoutAutoDirect")) Then
        Else
            If UCase(ConfigurationManager.AppSettings("logoutAutoDirect")) = "TRUE" Then
                Response.Redirect(g_loginPage)
            End If
        End If

    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Response.Redirect("Default.aspx")
    End Sub

End Class