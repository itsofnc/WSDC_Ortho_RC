Public Class ajaxFunctions
    Inherits System.Web.UI.Page
    '***********************************Last Edit**************************************************

    'Last Edit Date: 02/25/15
    '  Last Edit By: cpb
    'Last Edit Proj: HMIWebPortal
    '-----------------------------------------------------
    'Change Log: 
    '02/25/15 cpb determine field in database for users email.
    '02/04/15 cpb use global variable for site url
    '01/22/15 cpb verify database--called from default(or startup page) to make sure the initial database is built as necessary
    '               create new database if necessary
    ' 11/17/14 t3
    '  merge into patient portal -- 
    ' checkEmailAvailability - added this routine
    ' validateEmail() & validateUserId - send in table id to check and field id to check.  If not sent in, default to sys_users & user_email
    ' 9/19/14 cs
    ' test array count for session name, action and value (where applicable)
    '7/14/14 - t3
    'added password reset call forgotPassword()
    '
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim action As String = IIf(IsNothing(Request.QueryString("action")), Request.Form("action"), Request.QueryString("action"))
        '''''''''''''''''''''''''''''''''''''''
        Select Case True
            Case action.Contains("Sessions")
                handleSessions()
            Case action.Contains("encryptTab")
                encryptTab(action)
            Case Else
                Select Case action
                    Case "validateEmail"
                        validateEmail()
                    Case "validateUserId"
                        validateUserId()
                    Case "checkEmailAvailability"
                        checkEmailAvailability()
                    Case "forgotPassword"
                        forgotPassword()
                    Case "checkSessions"
                        checkSessions()
                    Case "timeout"
                        Session.RemoveAll()
                    Case "databaseVerify"
                        databaseVerify()
                    Case "verifySysUsers"
                        verifySysUsers()
                    Case "saveUser"
                        saveUser()
                    Case "verifyUserEmail"
                        verifyUserEmail()
                    Case "verifyUserId"
                        verifyUserId()
                    Case "guestLogin"
                        guestLogin()
                    Case "execLogin"
                        execLogin()
                    Case "requestLogin"
                        requestLogin()
                End Select
        End Select

    End Sub

    Private Sub execLogin()
        Dim strUserId As String = IIf(IsNothing(Request.QueryString("uid")), Request.Form("uid"), Request.QueryString("uid"))
        Dim strPw As String = IIf(IsNothing(Request.QueryString("pw")), Request.Form("pw"), Request.QueryString("pw"))
        ' get user record id via user id & password
        ' 03/12/15 cpb use login id style as defined in web config to define the user id field
        '               --note--the key defined in web.config  key="LoginIdStyle" value should match the field id in sys_users & users_vw

        Dim strSQL As String = "Select * From users_vw Where " & g_LoginIdStyle & " = '" & strUserId & "' And Password = CONVERT(VARCHAR(32), HashBytes('MD5', '" & strPw & "'), 2)"
        Dim m_tblUsers As DataTable = g_IO_Execute_SQL(strSQL, False)
        If IsNothing(m_tblUsers) Then
            litMessage.Text = "ERROR: Error Communicating with Database ERROR:"
        Else
            If m_tblUsers.Rows.Count = 0 Then
                litMessage.Text = "ERROR: Login Failed...Please Try Again ERROR:"
            Else
                Dim strMessage As String = g_UserLogin(m_tblUsers)
                If strMessage = "" Then
                Else
                    litMessage.Text = "ERROR:" & strMessage & "ERROR:"
                End If

            End If
        End If
    End Sub
    Private Sub guestLogin()
        ' 2/2/16 call application custom guest login
        Dim tblUser As DataTable = g_ReadGuestUser()
        Dim strMessage As String = g_UserLogin(tblUser)
        If strMessage <> "" Then
            litMessage.Text = "ERROR" & strMessage & "ERROR"
        End If
    End Sub
    Private Sub verifyUserEmail()
        Dim strSQL As String = ""
        Dim strEmail As String = IIf(IsNothing(Request.QueryString("email")), Request.Form("email"), Request.QueryString("email"))
        Dim intUserRecId As Integer = IIf(IsNothing(Request.QueryString("recid")), Request.Form("recid"), Request.QueryString("recid"))

        If intUserRecId = -1 Then
            strSQL = "Select recid From sys_users Where user_email = '" & strEmail & "'"
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblUser.Rows.Count > 0 Then
                litMessage.Text = "Sorry, This Email is already on file."
            End If
        Else
            'checking existing user
            strSQL = "Select recid From sys_users Where user_email = '" & strEmail & "' and recid <> " & intUserRecId
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblUser.Rows.Count > 0 Then
                litMessage.Text = "Sorry, This Email is already on file."
            End If
        End If

    End Sub

    Private Sub verifyUserId()
        Dim strSQL As String = ""
        Dim strUserId As String = IIf(IsNothing(Request.QueryString("userid")), Request.Form("userid"), Request.QueryString("userid"))
        Dim intUserRecId As Integer = IIf(IsNothing(Request.QueryString("recid")), Request.Form("recid"), Request.QueryString("recid"))

        If intUserRecId = -1 Then
            strSQL = "Select recid From sys_users Where " & g_LoginIdVerifyColumn & " = '" & strUserId & "'"
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblUser.Rows.Count > 0 Then
                litMessage.Text = "Sorry, This " & g_LoginIdStyle & " is already on file."
            End If
        Else
            'checking existing user
            strSQL = "Select recid From sys_users Where " & g_LoginIdVerifyColumn & " = '" & strUserId & "' and recid <> " & intUserRecId
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblUser.Rows.Count > 0 Then
                litMessage.Text = "Sorry, This " & g_LoginIdStyle & " is already on file."
            End If
        End If

    End Sub

    Private Sub saveUser()
        Dim strRecId As String = IIf(IsNothing(Request.QueryString("recId")), Request.Form("recId"), Request.QueryString("recId"))
        Dim strUserid As String = IIf(IsNothing(Request.QueryString("userid")), Request.Form("userid"), Request.QueryString("userid"))
        Dim strEmail As String = IIf(IsNothing(Request.QueryString("email")), Request.Form("email"), Request.QueryString("email"))
        Dim strFirstName As String = IIf(IsNothing(Request.QueryString("firstName")), Request.Form("firstName"), Request.QueryString("firstName"))
        Dim strLastName As String = IIf(IsNothing(Request.QueryString("lastName")), Request.Form("lastName"), Request.QueryString("lastName"))
        Dim strUserRole As String = IIf(IsNothing(Request.QueryString("userRole")), Request.Form("userRole"), Request.QueryString("userRole"))
        Dim strPassword As String = IIf(IsNothing(Request.QueryString("password")), Request.Form("password"), Request.QueryString("password"))
        Dim nvcUser As New NameValueCollection
        nvcUser(g_LoginIdVerifyColumn) = strUserid
        nvcUser("user_email") = strEmail
        nvcUser("firstName") = strFirstName
        nvcUser("lastName") = strLastName
        nvcUser("user_role") = strUserRole
        'nvcUser("inviteAccessCode") = ""
        'nvcUser("activatedDate") = Date.Now
        'nvcUser("active") = "1"
        litMessage.Text = ""
        If strRecId = "-1" Then
            ' insert new user
            g_IO_SQLInsert("sys_users", nvcUser, "frmUserAccount")
            strRecId = g_IO_GetLastRecId()
            Dim strSQLPassword As String = " update sys_users set password = CONVERT(VARCHAR(32), HashBytes('MD5', '" & strPassword & "'), 2) where recid = " & strRecId
            g_IO_Execute_SQL(strSQLPassword, False)
            ' what is this user allowed to do now...
            litMessage.Text = g_SaveNewUser(strRecId)

        Else
            Dim strSQL As String = "Select recid From sys_Users Where recid = '" & strRecId & "'"
            Dim tblUser As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblUser.Rows.Count > 0 Then
                g_IO_SQLUpdate("sys_users", nvcUser, "frmUserAccount", "recid = '" & strRecId & "'")
                Dim strSQLPassword As String = " update sys_users set password = CONVERT(VARCHAR(32), HashBytes('MD5', '" & strPassword & "'), 2) where recid = " & strRecId
                g_IO_Execute_SQL(strSQLPassword, False)
                'If editing self edit user role session
                If strRecId = Session("user_link_id") Then
                    Session("user_role") = UCase(strUserRole)
                End If
            Else
                litMessage.Text = "ERROR"
            End If
        End If
    End Sub

    Private Sub handleSessions()
        Dim arrSessions() As String = Split(IIf(IsNothing(Request.QueryString("Sessions")), Request.Form("Sessions"), Request.QueryString("Sessions")), "~~")
        For Each sessionRequest In arrSessions
            Dim arrSessionRequest() As String = Split(sessionRequest, "||")
            Dim strSessionName As String = ""
            Dim strSessionAction As String = ""
            Dim strSessionValue As String = ""
            ' 9/19/14 cs
            ' test array count for session name, action and value (where applicable)
            If arrSessionRequest.Count > 0 Then
                If IsNothing(arrSessionRequest(0)) Then
                Else
                    strSessionName = arrSessionRequest(0)
                End If
            End If
            If arrSessionRequest.Count > 1 Then
                If IsNothing(arrSessionRequest(1)) Then
                Else
                    strSessionAction = arrSessionRequest(1)
                End If
            End If
            If arrSessionRequest.Count > 2 Then
                If IsNothing(arrSessionRequest(2)) Then
                Else
                    strSessionValue = arrSessionRequest(2)
                End If
            End If


            If strSessionAction = "set" Then
                setSessions(strSessionName, strSessionValue)
            End If
            If strSessionAction = "clear" Then
                clearSessions(strSessionName)
            End If
            If strSessionAction = "remove" Then
                removeSessions(strSessionName)
            End If
            If strSessionAction = "preserve" Then
                preserveSession()
            End If

        Next
    End Sub
    Private Sub encryptTab(ByRef action As String)
        ' 9/22/15 T3 code above, action = "encryptTab" not getting hit re-loading FLM after Edit...
        ' action = encryptTab||tabId (table name)
        Dim strTabId As String = ""
        If action = "encryptTab" Then
            strTabId = IIf(IsNothing(Request.Form("tabId")), Request.QueryString("tabId"), Request.Form("tabId"))
        Else
            strTabId = Split(action, " Then||")(1)
        End If
        litMessage.Text = "E=" & g_Encrypt("tab=" & strTabId)
    End Sub
    Private Sub databaseVerify()
        '01/22/15 cpb verify database--called from default(or startup page) to make sure the initial database is built as necessary
        ' This relies on a config flag regarding "Demo"...this is specific to our Patient Portal
        ' 11/24/14 cpb moved to modPortal because this is not patient specific

        Dim strConnectionString As String = ConfigurationManager.ConnectionStrings("ConnectionString").ToString
        Dim strTempString As String = ""
            Dim strFront As String = Left(strConnectionString, UCase(strConnectionString).IndexOf("INITIAL CATALOG"))  ' ex: 
            Dim strBack As String = ""

            strTempString = strConnectionString.Replace(strFront, "")
        Try
            strBack = Mid(strTempString, UCase(strTempString).IndexOf(";") + 1)
        Catch ex As Exception

        End Try
        strConnectionString = strFront & "Initial Catalog=master" & strBack
        System.Web.HttpContext.Current.Session("connectionString") = strConnectionString

        If g_verifyDatabaseExists() Then
        Else
            litMessage.Text = "Error: Database does not exist."
            Session("MessageTitle") = "Database Not Defined"
            Session("PageRelay") = "Default.aspx"
            Session("RelayMessage") = "We were unable to verfiy your database schema: " & g_ConnectionSchema & ". This database must be defined before the application can run.  Please contact your system administrator for assistance."
        End If
        System.Web.HttpContext.Current.Session.Remove("connectionString")
    End Sub

    Private Sub verifySysUsers()
        '01/22/15 cpb create new database if necessary
        If g_verifySys_Users() Then
        Else
            litMessage.Text = "Error: Trouble creating/accessing user tables/views"
            Session("MessageTitle") = "Error Creating Users Table"
            Session("PageRelay") = "Default.aspx"
            Session("RelayMessage") = "We were unable to verify your user setup. Please contact your system administrator for assistance."
        End If
    End Sub
    Private Sub setSessions(ByVal sessionName As String, ByVal sessionValue As String)
        Session(sessionName) = sessionValue

    End Sub
    Private Sub clearSessions(ByVal sessionName As String)
        Session(sessionName) = ""
    End Sub
    Private Sub removeSessions(ByVal sessionName As String)
        Session.Remove(sessionName)
    End Sub
    Private Sub preserveSession()
        g_resetSessionVariables()
    End Sub
    Private Sub checkSessions()
        If Session.Count = 0 Then
            litMessage.Text = "expired"
        End If
    End Sub
    Private Sub forgotPassword()
        Dim strWhere As String = " where "
        Dim strType As String = " email "
        If IsNothing(Request.Form("email")) Then
        Else
            strWhere &= " user_email = '" & Request.Form("email") & "'"
                End If
                If IsNothing(Request.Form("userId")) Then
        Else
            strType = " user ID "
            'strWhere &= " user_id = '" & Request.Form("userID") & "'"
            strWhere &= g_LoginIdStyle & " = '" & Request.Form("userID") & "'"
        End If

        Dim blnRecordFound As Boolean = False
        Dim strCheck As String = "Select * from sys_Users " & strWhere
        Dim tblCheck As DataTable
        tblCheck = g_IO_Execute_SQL(strCheck, blnRecordFound)
        If IsNothing(tblCheck) And strType = " user ID " Then
            strType = " userId "
            strCheck = "Select * from sys_Users Where userId = '" & Request.Form("userID") & "'"
            tblCheck = g_IO_Execute_SQL(strCheck, blnRecordFound)
        End If
        If blnRecordFound Then
            If tblCheck.Rows.Count > 0 Then
                Dim strSql As String = "SELECT HashBytes('MD5', CONVERT(nvarchar,'" & tblCheck.Rows(0)("recid") & "')) as hash"
                'Dim strSql As String = " select CONVERT(VARCHAR(32), HashBytes('MD5', '" & tblCheck.Rows(0)("recid") & "'), 2) as hash"
                Dim tblHash As DataTable = g_IO_Execute_SQL(strSql, False)
                Dim strHash As String = Bytes_To_String2(tblHash.Rows(0)("hash"))
                'insert recovery record
                Dim nvcRecovery As New NameValueCollection
                nvcRecovery("resetKey") = strHash
                nvcRecovery("userRecid") = tblCheck.Rows(0)("recid")
                g_IO_SQLInsert("pwdRecovery", nvcRecovery)

                ' 02/25/15 cpb test for what email field is in users table
                Dim strEmail As String = ""
                Try
                    strEmail = tblCheck.Rows(0)("user_email")
                Catch ex As Exception
                    strEmail = tblCheck.Rows(0)("userEmail")
                End Try

                'send user email & return notification 
                ' 02/04/15 cpb should use global variable as instead of straight to webconfig
                Dim strLink As String = g_SiteUrl & "/frmPasswordReset.aspx?pwrid=" & strHash
                Dim strMessage As String = "You have indicated that you have forgotten your " & g_SiteDisplayName & " Account password." & vbCrLf & _
                    "To securely reset your password and sign into your account, click the link below and follow the instructions." & vbCrLf & vbCrLf & strLink & _
                    vbCrLf & vbCrLf & "For security reasons, this link will expire in 48 hours." & _
                     vbCrLf & vbCrLf & "If you did not request a new password, don't worry, your password will not be changed unless you take action. You can just ignore this message." & _
                    vbCrLf & vbCrLf & "Thank you for choosing " & g_SiteDisplayName & "." & _
                    vbCrLf & vbCrLf & "Please do not reply to this message. This email address is not monitored so we are unable to respond to any messages sent to this address."

                ' 02/25/15 cpb use email variable b/c not sure how it is stored in database
                g_sendEmail(strEmail, "Reset Your Password", strMessage)
                litMessage.Text = "An email has been sent to your email address with reset instructions.<br /><br />Please check your Junk Mail if you do not receive the email."
            Else
                'return email not found 
                litMessage.Text = "Sorry," & strType & "not found. "
            End If
        Else
            litMessage.Text = "Error:Unable to communicate with Database. (Error 101)"
        End If

    End Sub
    Private Sub checkEmailAvailability()
        '11/17/14
        Dim email As String = Request.Form("email")
        Dim strTableId As String = IIf(IsNothing(Request.Form("tableId")), "sys_users", Request.Form("tableId"))
        Dim strEmailId As String = IIf(IsNothing(Request.Form("emailId")), "user_email", Request.Form("emailId"))
        Dim strCheck As String = "Select * from " & strTableId & " where " & strEmailId & " = '" & email & "'"
        Dim tblCheck As DataTable = g_IO_Execute_SQL(strCheck, False)
        If tblCheck.Rows.Count > 0 Then
            litMessage.Text = "Error:Email Not Available"
        Else
            litMessage.Text = ""
        End If
    End Sub
    Private Sub validateEmail()
        '11/17/14 cb/cs
        ' send in table id to check and field id to check.  If not sent in, default to sys_users & user_email
        Dim email As String = Request.Form("email")
        Dim strTableId As String = IIf(IsNothing(Request.Form("tableId")), "sys_users", Request.Form("tableId"))
        'Dim strEmailId As String = IIf(IsNothing(Request.Form("emailId")), "user_email", Request.Form("emailId"))
        ' 8/20/15 cpb - use loginstyle from webconfig
        Dim strEmailId As String = IIf(IsNothing(Request.Form("emailId")), g_LoginIdVerifyColumn, Request.Form("emailId"))
        Dim blnReset As Boolean = False
        If IsNothing(Request.Form("reset")) Then
        Else
            blnReset = Request.Form("reset")
        End If
        Dim strCheck As String = "Select * from " & strTableId & " where " & strEmailId & " = '" & email & "'"
        Dim tblCheck As DataTable = g_IO_Execute_SQL(strCheck, False)
        If tblCheck.Rows.Count > 0 Then
            If blnReset Then
                litMessage.Text = "Email found"
            Else
                litMessage.Text = "Error:Email Not Available"
            End If
        Else
            If blnReset Then
                litMessage.Text = "Error:Email Not Found"
            Else
                litMessage.Text = "Email Available"
            End If
        End If
    End Sub
    Private Sub validateUserId()
        '11/17/14 cb/cs
        ' send in table id to check and field id to check.  If not sent in, default to sys_users & user_email
        Dim strTableId As String = IIf(IsNothing(Request.Form("tableId")), "sys_users", Request.Form("tableId"))
        'Dim strUserId As String = IIf(IsNothing(Request.Form("userId")), "user_Id", Request.Form("userId"))
        ' 8/20/15 cpb - use loginstyle from webconfig
        Dim strUserId As String = IIf(IsNothing(Request.Form("userId")), g_LoginIdVerifyColumn, Request.Form("userId"))
        Dim userId As String = Request.Form("user")
        Dim blnReset As Boolean = False
        If IsNothing(Request.Form("reset")) Then
        Else
            blnReset = Request.Form("reset")
        End If
        Dim strCheck As String = "Select * from " & strTableId & " where " & strUserId & " = '" & userId & "'"
        Dim tblCheck As DataTable
        tblCheck = g_IO_Execute_SQL(strCheck, False)
        If IsNothing(tblCheck) And strUserId = "user_Id" Then
            strUserId = "userId"
            strCheck = "Select * from " & strTableId & " where " & strUserId & " = '" & userId & "'"
            tblCheck = g_IO_Execute_SQL(strCheck, False)
        End If
        If tblCheck.Rows.Count > 0 Then
            If blnReset Then
                litMessage.Text = "User ID found"
            Else
                litMessage.Text = "Error:User ID Not Available"
            End If
        Else
            If blnReset Then
                litMessage.Text = "Error:User ID Not Found"
            Else
                litMessage.Text = "User ID Available"
            End If
        End If
    End Sub

    Private Sub requestLogin()
        ' call custom routine to request login
        Dim strRqName As String = IIf(IsNothing(Request.QueryString("name")), Request.Form("name"), Request.QueryString("name"))
        Dim strRqEmail As String = IIf(IsNothing(Request.QueryString("email")), Request.Form("email"), Request.QueryString("email"))
        g_requestLogin(strRqName, strRqEmail)
    End Sub

    Private Function Bytes_To_String2(ByVal bytes_Input As Byte()) As String
        Dim strTemp As New StringBuilder(bytes_Input.Length * 2)
        For Each b As Byte In bytes_Input
            strTemp.Append(Conversion.Hex(b))
        Next
        Return strTemp.ToString()
    End Function
End Class