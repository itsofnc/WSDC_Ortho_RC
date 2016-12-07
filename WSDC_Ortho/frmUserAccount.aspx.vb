Imports System.Drawing

Public Class frmUserAccount
    Inherits System.Web.UI.Page

    Dim m_nvcUrlParams As New NameValueCollection
    Dim m_blnNewRegistration As Boolean = False
    Dim m_imgLoc As String = "Images/"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' see if we have a logo to display, otherwise show site display name
        If g_siteLogo = "" Then
            litSiteLogo.Text = "<h2><a href=""" & g_SiteUrl & """ target=""_self"" >" & g_SiteDisplayName & "</a></h2>"
        Else
            Dim strThumbName As String = "Thumbnail_" & g_siteLogo.Replace(m_imgLoc, "")
            If System.IO.File.Exists(MapPath(m_imgLoc & strThumbName)) Then
                ' is this thumbnail newer/same date as main logo file (in case user uploads new image with same name)
                ' get main logo date and thumbnail date to compare
                Dim strMainLogoDate As Date = System.IO.File.GetLastWriteTime(MapPath(g_siteLogo))
                Dim strThumbLogoDate As Date = System.IO.File.GetLastWriteTime(MapPath(m_imgLoc & strThumbName.Replace("Images/", "")))
                If strMainLogoDate >= strThumbLogoDate Then
                    g_resizeImage(MapPath(g_siteLogo), 75, 1000, MapPath(m_imgLoc & strThumbName))
                End If
            Else
                g_resizeImage(MapPath(g_siteLogo), 75, 1000, MapPath(m_imgLoc & strThumbName))
            End If

            litSiteLogo.Text = "<a href=""" & g_SiteUrl & """ target=""_self"" >" &
                "<img src=""" & m_imgLoc & strThumbName & """ alt=""" & g_SiteDisplayName & """/></a>"
        End If

        Try
            m_blnNewRegistration = CBool(g_GetUrlParameterValue(Request.Url.AbsoluteUri, "newregistration"))
        Catch ex As Exception
        End Try

        If m_blnNewRegistration Then
        Else
            'verify user logged on
            If IsNothing(Session("user_link_id")) Then
                Session("LoginReturnURL") = Request.Url.AbsoluteUri
                Session("MessageTitle") = "Session Expired"
                Session("RelayMessage") = "<h3>Your session has expired. Please login to continue.</h3> <br /><br /> " &
                "<a href=""" & g_loginPage & """ target=""_self"" class=""btn btn-primary"">Login</a> "
                Session("PageRelay") = ""
                Response.Redirect("frmMessaging.aspx")
            End If

        End If

        ' set user login title
        txtLoginStyle.Text = LCase(g_LoginIdStyle.Replace("_", " "))
        litUserTitle.Text = g_LoginIdStyle.Replace("_", " ")
        If LCase(g_LoginIdStyle).Contains("email") Then
            txtUserId.Attributes.Add("data-fv-emailaddress", "true")
            txtUserId.Attributes.Add("data-fv-emailaddress-message", "The value is not a valid email address")
            buildShowHideScripts("rowEmail")
        End If

        If IsPostBack Then
        Else
            ' call sub routine to extract the cid (selected row recid) and mode ('view', 'edit') sent over from FLM
            Dim intEditUserRecId As Integer = -1
            Dim strAddEdit As String = ""
            Dim strSesPrefix As String = ""
            g_getFLMCustomFormParameters(intEditUserRecId, strAddEdit, strSesPrefix, Request.RawUrl.Replace("/frmUserAccount.aspx?", ""))

            ' store off session prefix from FLM (sent in via Url)
            txtReturnUrl.Text = "frmListManager.aspx?E=" & g_Encrypt("id=users_vw&pre=1&sesPrefix=" & strSesPrefix)

            ' determine if we are in a new tab for closing/returning to user list (button on form)
            txtEditTarget.Text = LCase(g_GetTableExtProperty("users_vw", "EditTarget"))
            If txtEditTarget.Text = "" Then
                txtEditTarget.Text = "_self"
            End If

            ' build user role ddl
            buildUserRoleDDL()

            ' do we hide this all together if user does not have permission to maintain roles or new regitration
            If Session("AccountRoles") = 0 Or m_blnNewRegistration Then
                'divLevel
                litScripts.Text &= "<script>jQuery(document).ready(function () { jQuery('#divLevel').addClass('hidden'); jQuery('#btnReturnUserList').addClass('hidden'); jQuery('#rowLastLogin').addClass('hidden'); })</script>"
            End If

            If intEditUserRecId = -1 Then
                litTitle.Text = "Create Account"
                strAddEdit = "ADD"
                txtLastLogin.Visible = False
            Else
                If strAddEdit = "VIEW" Then
                    litTitle.Text = "User Account (View Only)"
                    txtUserId.ReadOnly = True
                    txtFirstName.ReadOnly = True
                    txtLastName.ReadOnly = True
                    txtEmail.ReadOnly = True
                    btnSaveUser.Visible = False
                Else
                    litTitle.Text = "Update User Account"
                End If
                populateUserData(intEditUserRecId, strAddEdit)
            End If

            txtRecid.Text = intEditUserRecId
        End If
    End Sub

    Private Sub populateUserData(ByRef intEditUserRecId As Integer, ByVal strAddEdit As String)
        If intEditUserRecId > -1 Then
            Dim tblSysUser As DataTable = g_IO_Execute_SQL("Select * From sys_users Where recid = '" & intEditUserRecId & "'", False)
            For Each sysUser As DataRow In tblSysUser.Rows
                txtUserId.Text = sysUser("userId")
                If IsDBNull(sysUser("firstName")) Then
                    txtFirstName.Text = ""
                Else
                    txtFirstName.Text = sysUser("firstName")
                End If
                If IsDBNull(sysUser("lastName")) Then
                    txtLastName.Text = ""
                Else
                    txtLastName.Text = sysUser("lastName")
                End If
                If IsDBNull(sysUser("user_email")) Then
                    txtEmail.Text = ""
                Else
                    txtEmail.Text = sysUser("user_email")
                End If

                ddlLevel.SelectedValue = sysUser("user_role")
                If UCase(Session("user_role")) = "ADMINISTRATOR" And strAddEdit <> "VIEW" Then
                    ddlLevel.Enabled = True
                Else
                    ddlLevel.Enabled = False
                    litScripts.Text = "<script>jQuery(document).ready( function () { jQuery('#" & ddlLevel.ClientID & "').addClass(""form-control"") } ) </script>"
                End If
                If IsDBNull(sysUser("lastLogin")) Then
                    txtLastLogin.Text = ""
                Else
                    txtLastLogin.Text = sysUser("lastLogin")
                End If
            Next
        End If
    End Sub

    Private Sub buildUserRoleDDL()
        Dim intUserRole As Integer = -1
        If m_blnNewRegistration Then
        Else
            If UCase(Session("user_role")) = "ADMINISTRATOR" Or Session("user_role") = "1" Then
                intUserRole = 1
            End If
        End If

        ' level dropdown could be limited via ModAppCustoms
        Dim tblUserRoles As DataTable = g_getUserRoleOptions(intUserRole)
        Dim rowRole As DataRow = tblUserRoles.NewRow
        If tblUserRoles.Rows.Count > 1 Then
            rowRole("recid") = "-1"
            rowRole("descr") = "Select User Role"
            tblUserRoles.Rows.InsertAt(rowRole, 0)
        End If
        ddlLevel.DataSource = tblUserRoles
        ddlLevel.DataTextField = "descr"
        ddlLevel.DataValueField = "recid"
        ddlLevel.DataBind()
    End Sub

    Private Sub buildShowHideScripts(ByVal strHideDivs As String)

        Dim strDivHideScript As String = ""
        If Trim(strHideDivs) = "" Then
        Else
            strDivHideScript = "<script>jQuery(document).ready(function () {"
            For Each strDivHide In strHideDivs.Split(",")
                'strDivHideScript &= "document.getElementById(""" & strDivHide & """).style.display = ""none"";"
                strDivHideScript &= "jQuery('#" & strDivHide & "').hide();"
            Next
            strDivHideScript &= "});</script>"
        End If
        litHideDivs.Text = strDivHideScript

    End Sub

End Class