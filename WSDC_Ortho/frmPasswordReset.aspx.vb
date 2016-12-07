Public Class frmPasswordReset
    Inherits System.Web.UI.Page

    'Last Edit Date: 09/23/15
    '  Last Edit By: cpb
    'Last Edit Proj: SDIWebPortal
    '-----------------------------------------------------
    'Change Log: 
    '09/23/15 cpb always send email notification even if not from hash; clear sessions if login is required.
    '02/16/15 t3 get login page from encrypted ulr
    '02/04/15 cpb use global variable for site url; use global variable for login page.  Change default login page from Login.aspx to frmLogin.aspx
    '-----------------------------------------------------

    ' 02/16/15 default login page
    Dim m_strLoginPage As String = "frmLogin.aspx"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsPostBack Then
        Else
            'get login default
            ' 2/4/15 use global variable, set logind deafult to frmLogin
            ' 2/16/15 T3 See if we have a session variable for login page 
            ' (ie. used in Patient Portal Demo area)
            If IsNothing(Request.QueryString("lid")) Then
            Else
                ' sent via encrypted string 
                g_loginPage = g_Decrypt(Request.QueryString("lid")) ''ie: frmDemoStart.aspx
            End If
            If IsNothing(g_loginPage) Then
            Else
                If g_loginPage = "" Then
                Else
                    m_strLoginPage = g_loginPage
                End If
            End If
            btnUpdatePassword.Attributes.Add("onCLick", "return validatePassword();")
            'txtValidate_password.Attributes.Add("onKeyUp", " return validatePassword();")
            'txtValidate_password2.Attributes.Add("onKeyUp", " return validatePassword();")

            'Clean up expired reset keys
            removeExpiredKeys()

            ' 10/3/14 see if we came here staight from frmlistmanager
            If IsNothing(Request.QueryString("rl")) Then
                Dim strHashKey As String = Request.QueryString("pwrid")
                Dim strSql As String = "select * from pwdRecovery where resetKey = '" & strHashKey & "'"
                Dim tblPwdReset As DataTable = g_IO_Execute_SQL(strSql, False)
                If tblPwdReset.Rows.Count > 0 Then
                Else
                    litMessage.Text = "We were unable to process your password reset. " & vbCrLf & "You can <a href=""" & m_strLoginPage & """>return to the login page</a> & try again."
                    litScripts.Text = "<script>jQuery(document).ready(function () { jQuery('#passwordReset').addClass('hidden'); })</script>"
                End If
            Else
                If IsNothing(Request.QueryString("un")) Then
                Else
                    litUserInfo.Text = "<h4> Reset password for user:" & Request.QueryString("un") & "</h4>"
                End If
            End If
        End If
    End Sub

    Private Sub btnUpdatePassword_Click(sender As Object, e As EventArgs) Handles btnUpdatePassword.Click
        Dim tblPwdReset As New DataTable
        Dim strHashKey As String = ""        
        If IsNothing(Request.QueryString("rl")) Then
            If IsNothing(Request.QueryString("pwrid")) Then
            Else
                strHashKey = Request.QueryString("pwrid")
            End If
            Dim strSql As String = "select * from pwdRecovery where resetKey = '" & strHashKey & "'"
            tblPwdReset = g_IO_Execute_SQL(strSql, False)
        Else
            tblPwdReset.Columns.Add("userRecId", GetType(System.Int64))
            tblPwdReset.Columns.Add("recid", GetType(System.Int64))
            Dim row As DataRow = tblPwdReset.NewRow
            row("userRecId") = Request.QueryString("rl")
            row("recid") = -1
            tblPwdReset.Rows.Add(row)
        End If

        litScripts.Text = "<script>jQuery(document).ready(function () { jQuery('#passwordReset').addClass('hidden'); })</script>"
        If tblPwdReset.Rows.Count > 0 Then
            '02/04/15 cpb use global variable
            ' 2/16/15 T3 Check url for ref to user table, when different than sys_users
            Dim strUserTable As String = "sys_Users"
            If IsNothing(Request.QueryString("tid")) Then
            Else
                strUserTable = g_Decrypt(Request.QueryString("tid")) ''ie: portalUsers
            End If

            Dim intUserRecid As Integer = tblPwdReset.Rows(0)("userRecid")
            Dim strSQLPassword As String = " update " & strUserTable & " set password = CONVERT(VARCHAR(32), HashBytes('MD5', '" & txtValidate_password.Text & "'), 2) where recid = " & intUserRecid
            g_IO_Execute_SQL(strSQLPassword, False)

            If IsNothing(Request.QueryString("flm")) Then
                litMessage.Text = "Your password has been reset. Click <a href=""" & m_strLoginPage & """>here</a> to login."
                ' 9/23/15 cpb if re-login is required - clear the sessions and let them get re-set on login
                Session.RemoveAll()
            Else
                ' called from frmListManager, diplay option to go back to it
                litMessage.Text &= "The password has been reset. Click <a href=""#"" onclick=""window.close();"">here</a> to close this page."
            End If
            If strHashKey <> "" Then
                Dim strSqlDelete As String = " delete from pwdRecovery where resetKey = '" & strHashKey & "'"
                g_IO_Execute_SQL(strSqlDelete, False)
            End If
            '09/23/15 cpb this was only sending if using a hash key -- moved out of the if should always be sending this.
            '09/29/14 T3 send email that password has successfully updated 
            '02/04/15 cpb use global variable
            Dim strSql As String = " select " & g_userIdField & "," & g_userEmailField & " from " & strUserTable & " where recid = '" & tblPwdReset.Rows(0)("userRecid") & "'"
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSql, False)
            Dim strAccount As String = " account " & tblUser.Rows(0)(g_userIdField)
            Dim strEmail As String = tblUser.Rows(0)(g_userEmailField)
            Dim strMessage As String = "The password for your " & g_SiteDisplayName & strAccount & " has been reset. " & vbCrLf & vbCrLf &
                 "If you did not make this change or if you believe an unauthorized person has accessed your account go to " &
                 g_SiteUrl & "/frmForgotPassword.aspx to reset your password immediately. " &
            vbCrLf & vbCrLf & "If you need addional help, contact " & g_SiteDisplayName & " support."

            g_sendEmail(strEmail, " Your " & g_SiteDisplayName & " account password has been reset", strMessage)
        Else
            litMessage.Text = "We were unable to process your password reset. " & vbCrLf & "You can <a href=""" & m_strLoginPage & """>return to the login page</a> & try again."
        End If
    End Sub

    Private Sub removeExpiredKeys()
        Dim strSql As String = " Delete from pwdRecovery where addDate <= '" & Format(DateAdd(DateInterval.Hour, -48, Date.Now), "MM/dd/yyyy HH:mm:ss") & "'"
        g_IO_Execute_SQL(strSql, False)
    End Sub
   
End Class