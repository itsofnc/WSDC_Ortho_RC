Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack Then
        Else
            ' 01/22/15
            ' check for new install and load up configuration settings as necessary
            If g_newInstall = "true" Then
                g_setAppConfigurationKeys()
            End If
            If IsNothing(Request.QueryString("sesPrefix")) Then
                txtLoginSesPrefix.Text = "REN1" & Format(Date.Now, "ssffff") ' time in milliseconds
            Else
                txtLoginSesPrefix.Text = Request.QueryString("sesPrefix")
            End If
        End If

        txtLoginGetText.Text = "frmLogin.aspx?sesPrefix=" & txtLoginSesPrefix.Text

    End Sub

End Class