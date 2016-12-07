Public Class frmLogin
    Inherits System.Web.UI.Page


    '***********************************Last Edit**************************************************
    'Developer's notes:****************PLEASE READ************************************************** 
    '*First time Setup: set webconfig key "newInstall" = true, then you can change the app settings key value using g_changeAppSettings in ModMain

    'Last Edit Date: 06/26/15
    '  Last Edit By: t3
    'Last Edit Proj: T3CommonCode
    '-----------------------------------------------------
    'Change Log:
    ' 06/26/15 T3 check to see if we have a key to auto login when debugging
    ' 03/12/15 cpb get login field id from webconfg & global variable instead of hard coded to userID
    '       slight changes in display arrangement to match changes cfs had done earlier in case files.
    ' 01/22/15 cpb verify users table exists if so continue if not go back to default.
    ' 09/29/14 - T3 -Created 
    '**********************************************************************************************

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Request.QueryString("l")) Then
        Else
            Session.RemoveAll()
        End If

        ' check to see login required in debug mode only
        If ConfigurationManager.AppSettings("debugAutoLogin") = "true" Then
            g_DebugAutoLogin()
        End If

        ' 2/4/16 cpb add ability to retrun directly to login page after logout
        If IsNothing(ConfigurationManager.AppSettings("logoutAutoDirect")) Then
        Else
            If UCase(ConfigurationManager.AppSettings("logoutAutoDirect")) = "TRUE" Then
                litScripts.Text &= "<script src = ""Common/js/respond.js"" ></script>" &
                    "<script>" &
                    "    window.location.hash = ""nbb"";" &
                    "    window.location.hash = ""anbb"";" &
                    "    window.onhashchange = function() {window.location.hash = ""nbb""; }" &
                    "</script>"
            End If
        End If


        '1/12/15 T3 see if we need to allow new users to create an account
        If g_allowNewUser And (g_newUser = "REGISTER" Or g_newUser = "BOTH") Then
            txtNewUserAllowed.Value = "true"
            litNewUser.Text = "window.location.href='frmUserAccount.aspx?E=" & g_Encrypt("newregistration=true") & "'; return false"
        Else
            txtNewUserAllowed.Value = "false"
        End If

        '2/1/15 cpb check for guest login
        If g_allowGuestUser Then
            txtGuestUserAllowed.Value = "true"
            txtGuestRedirect.Value = redirectLoginLocation()
        Else
            txtGuestUserAllowed.Value = "false"
        End If

        ' 2/4/16 T3 store login redirect path in textbox
        txtLoginRedirect.Value = redirectLoginLocation()

        ' 4/6/16 cpb Ability to Request Login
        If g_blnRequestLogin Then
            divRequestInvitation.Visible = True
        Else
            divRequestInvitation.Visible = False
        End If

        If IsPostBack Then
        Else
            ' 2/18/2016 T3 Moved check for db existing and sys_users existing to document.ready
            '.........
            ' check that we have a sys_users table...in case a new install sends user to this form (1st time in) 
            ' (at least it won't bomb on btnLogin w/out the sys_users table existing)
            '01/22/15 cpb verify users table exists if so continue if not go back to default.
            'If g_verifySysUsersTableExists() Then
            'Else
            '    ' 2/17/16 cpb if sys_users does not exist, try to create.  if cant go back to Default
            '    g_createSys_UsersTbl()
            '    If g_verifySysUsersTableExists() Then
            '    Else
            '        Response.Redirect("Default.aspx")
            '    End If
            'End If
            'eom 2/18/16

            ' 8/20/15 T3 Set form properties based on login style set in web config
            litLoginStyle.Text = g_LoginIdStyle.Replace("_", " ")
            txtUserID.Attributes.Add("Placeholder", g_LoginIdStyle.Replace("_", " "))
            txtUserID.Attributes.Add("data-fv-notempty-message", "The " & g_LoginIdStyle.Replace("_", " ") & " is required and cannot be empty")
            If LCase(g_LoginIdStyle).Contains("email") Then
                txtUserID.Attributes.Add("data-fv-emailaddress", "true")
                txtUserID.Attributes.Add("data-fv-emailaddress-message", "The value is not a valid email address")
            End If
            '- end of mod 8/20/15

            litHeaderText.Text = g_SiteDisplayName
            txtUserID.Focus()
        End If

    End Sub

    Private Function redirectLoginLocation() As String

        Dim strReturnTo As String = ""
        'here will need to determine where to go based on the user role.
        If IsNothing(Session("LoginReturnURL")) Then
            'here will need to determine where to go based on the user role.
            strReturnTo = g_LoginRedirect("Login")
        Else
            Dim strGoBackTo As String = Session("LoginReturnURL")
            Session.Remove("LoginReturnURL")
            strReturnTo = strGoBackTo
        End If

        Return strReturnTo
    End Function

End Class