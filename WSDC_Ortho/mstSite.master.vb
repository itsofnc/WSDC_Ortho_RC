Public Class mstSite
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If IsPostBack Then
        Else
            If IsNothing(Session("user_link_id")) Then
                litNavigation.Text = ""
                litNavHome.Text = "href=""Default.aspx"""
            Else
                litHeaderGreeting.Text = "<p class=""navbar-text navbar-right""><span style=""color:white"">Welcome, " & _
                    IIf(IsNothing(Session("user_name")), "", Session("user_name")) & _
                    "&nbsp;&nbsp;&nbsp;</span><input id=""btnLogout"" onclick=""logOut();"" class=""btn btn-sm btn-warning"" value=""Logout"" type=""button""/></p>"

                litNavigation.Text = g_BuildNavigation(0)
                litNavHome.Text = g_BuildNavigation(1)
            End If

            litPageSource.Text = Page.AppRelativeVirtualPath
            litCompanyName.Text = "&copy; " & Format(Date.Now, "yyyy") & " " & g_CompanyLongName
            litNavbarBrand.Text = Session("CompanyDisplayId") & " " & g_SiteDisplayName

            ' T3 01/26/17 check web config for custom navbar keys 
            Dim strNavCustomStyle As String = IIf(IsNothing(ConfigurationManager.AppSettings("navCustomStyle")), "", ConfigurationManager.AppSettings("navCustomStyle"))
            If strNavCustomStyle = "" Then
            Else
                litNavCustomStyle.Text = strNavCustomStyle
            End If
            Dim strNavCustomBrand As String = IIf(IsNothing(ConfigurationManager.AppSettings("navCustomBrand")), "", ConfigurationManager.AppSettings("navCustomBrand"))
            If strNavCustomBrand = "" Then
            Else
                litNavCustomBrand.Text = strNavCustomBrand
            End If

            litScripts.Text = "<script type=""text/javascript"" language=""javascript"">var g_FormObjectTypes=""" & g_strFormObjectTypes & """;</script>"

        End If
    End Sub

End Class