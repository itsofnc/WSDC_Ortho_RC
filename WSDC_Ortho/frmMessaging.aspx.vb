Public Class frmMessaging
    Inherits System.Web.UI.Page

    '***********************************Use Notes**************************************************
    ' Uses Sessions to control displays
    '   - MessageTitle - controls text in tab at top
    '   - RelayMessage - the main message to be displayed including any links
    '   - PageRelay - page you are allowing user to return to
    '***********************************Last Edit**************************************************
    'Last Edit Date: 09/17/15
    'Last Edit By: cpb
    'Last Edit Proj: SDIWebPortal
    'Created: 11/12/14; cpb
    '-----------------------------------------------------
    'Change Log: 
    ' 09/17/15 cpb Cleanup display; Remove use of Default page css to remove large image in background.  Centered text. Set site name as page title.
    ' 11/12/14 cpb Patient Portal
    '    
    '*********************************************************************************************

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' clear last form session
        Session.Remove("FormPage")

        ' 09/17/15 cpb set site name as page title
        litProjectTitle.Text = g_SiteDisplayName

        If Session("MessageTitle") Is Nothing Then
            litPageTitle.Text = "Message from Online Portal"
        Else
            litPageTitle.Text = Session("MessageTitle")
        End If
        Session.Remove("MessageTitle")

        ' Expects Session Variable RelayMessage and PageRelay
        Dim strRelayMessage As String = ""
        Dim strHomePage As String = g_LoginRedirect("frmMessaging")
        Dim strRelayPage As String = "Return to <a href='" & strHomePage & "'>Home Page</a>."
        If Session("RelayMessage") Is Nothing Then
            strRelayMessage = "If you have experienced an unexpected problem... <br /> <br />" &
                "Please contact us for support."
            Session.Remove("PageRelay")
        Else
            strRelayMessage = Session("RelayMessage")
        End If
        If Session("PageRelay") Is Nothing Then
        Else
            If IsNothing(Session("user_link_id")) Then
            Else
                strRelayPage = "Return to Previous Page <a href='" & Session("PageRelay") & "'>here</a>."
            End If
        End If
        litMessage.Text = strRelayMessage & "<br /><br />" & strRelayPage

        If IsPostBack Then
        Else
            Session.Remove("PageRelay")
        End If

        litCompanyName.Text = g_CompanyLongName

    End Sub

    Private Sub frmMessaging_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        Session("RelayMessage") = ""
    End Sub
End Class