Public Class frmRequestLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack Then
        Else
            litHeaderText.Text = g_SiteDisplayName
            txtRequestName.Focus()
        End If
    End Sub

End Class