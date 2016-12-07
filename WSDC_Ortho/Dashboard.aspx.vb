Public Class Dashboard
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If

        If Session("user_role") <> "Administrator" Then
            'User
            litScripts.Text = "<script  type=""text/javascript"">"
            litScripts.Text &= "jQuery(""#divDataImport"").addClass(""hidden"");"
            litScripts.Text &= "jQuery(""#dashAdmin"").addClass(""hidden"");"

            litScripts.Text &= "</script>"
        Else
            'Admin
            litScripts.Text = "<script  type=""text/javascript"">"
            litScripts.Text &= "jQuery(""#dashUser"").addClass(""hidden"");"
            litScripts.Text &= "</script>"
        End If

        If IsPostBack Then
        Else
            If IsNothing(Request.QueryString("sesPrefix")) Then
                txtFLMSesPrefix.Text = "RLN1" & Format(Date.Now, "ssffff") ' time in milliseconds
            Else
                txtFLMSesPrefix.Text = Request.QueryString("sesPrefix")
            End If
        End If
        txtFLMGetText.Text = "frmListManager.aspx?id=ReleaseNotes_vw&perm=0000&srt=Release_Date&pre=1&sesPrefix=" & txtFLMSesPrefix.Text
    End Sub
End Class