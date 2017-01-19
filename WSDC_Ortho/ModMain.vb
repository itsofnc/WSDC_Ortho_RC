'
' *** please leave imports for bacward compatible with vs2012
'
Imports System.Net.Mail
Imports System.Net.Mail.SmtpClient
Imports System.Web.HttpContext
Imports System.Configuration
Imports System.Web.HttpServerUtilityBase

' 2/16/15 t3 Imports for encryption functions
Imports System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Security.Cryptography
Imports System.Net

'added 09/17/15 for HTML email stream reader CP


Module ModMain
    '***********************************Last Edit**************************************************
    'Last Edit Date: 10/20/15
    'Last Edit By: CPB
    'Last Edit Proj: ACA_Reporting
    '-----------------------------------------------------
    'Change Log: 
    ' 10/20/15 cpb default items per page to 10 if not defined in webconfig and/or not > 0 to prevent divide by zero error in flm
    ' 06/26/15 t3 check to see if we have a key to auto login when debugging
    ' 03/06/15 t3 email timeout per install
    ' 02/16/15 t3 add encrypt/decrpt functions
    ' 02/13/15 changed to use default email address instead of user name
    ' 02/04/15 cpb match up to web.config -- validated to patient portal needs
    ' 01/22/15 cpb get project title for generic forms use
    ' 01/19/15 T3 Set home links for Admin or Default users
    ' 01/09/15 CPB The replace caused issues when address not defined in web.config
    ' 11/12/14 - cpb
    '   add g_loadStateDropDown to load states dropdown list
    ' 11/11/14 - cpb
    '   -add user recid
    ' 09/29/14 -t3- Added newInstall flag and getFieldTypes variables (see litScripts in Admin Master)
    ' 09/17/14 - cpb
    '   Added site display name b/c of error pulling common code in.
    ' 7/11/14 - t3
    '   clean to all common code/ remove patient/mod auto coder specifics.
    '    
    '**********************************************************************************************
    ' session variables that will be available by default
    ' Company Data------------------
    '   Session("CompanyContactName") 
    '   Session("CompanyPhone")
    '   Session("CompanyName") 
    '   Session("CompanyShortName") 
    '   Session("CompanyAddress")
    '   Session("CompanyEmailAddress")
    '   Session("CompanyURL")
    '   Session("CompanyRecid")
    ' Session Preservation ----------
    '   Session("preserveSession")
    ' Demo Variables - Only in apps w/ demo mode available
    '   Session("DemoMode")
    '   Session("DemoSchema")

    ' ---Default global variables
    Public g_Debug As Boolean = False
    Public g_strFormObjectTypes = "input+select+textarea"
    Public g_StringArrayOuterSplitParameter = "||"
    Public g_StringArrayValueSplitParameter = "~~"
    Public g_UserRecId As Integer = -1                   ' 11/11/15 cpb needed for sql_update modio

    '-------------------------------------------------Build Web Config Info----------------------------------------------------------------------------
    ' Build global variables from web config
    '---New Install
    ' 09/29/14 T3
    Public g_newInstall As String = ConfigurationManager.AppSettings("newInstall")

    '---Site Keys
    Public g_SiteUrl As String = ConfigurationManager.AppSettings("siteUrl")
    Public g_SiteDisplayName As String = ConfigurationManager.AppSettings("SiteDisplayName")    ' 9/17/14

    '---Application Keys-->
    'moduleCode---used in autocode    
    'Public g_projectTitle As String = ConfigurationManager.AppSettings("ProjectTitle")          ' 01/22/15 cpb get project title for generic forms use
    Public g_loginPage As String = ConfigurationManager.AppSettings("loginPage")
    Public g_LoginIdStyle As String = IIf(IsNothing(ConfigurationManager.AppSettings("LoginIdStyle")), "UserId", ConfigurationManager.AppSettings("LoginIdStyle"))
    Public g_LoginIdVerifyColumn As String = IIf(IsNothing(ConfigurationManager.AppSettings("LoginIdVerifyColumn")), "UserId", ConfigurationManager.AppSettings("LoginIdVerifyColumn"))
    Public g_DefaultStateRecid As Integer = IIf(ConfigurationManager.AppSettings("defaultStateRecid") = "", ConfigurationManager.AppSettings("T3defaultStateRecid"), ConfigurationManager.AppSettings("defaultStateRecid")) ' 91  North Carolina

    '---Company Keys-->
    Public g_CompanyLongName As String = ConfigurationManager.AppSettings("CompanyName")
    Public g_CompanyShortName As String = ConfigurationManager.AppSettings("CompanyShortName")
    Public g_CompanyLogo As String = ConfigurationManager.AppSettings("LogoImageFullPath")
    Public g_CompanyMainPhone As String = ConfigurationManager.AppSettings("MainPhoneContact")
    ' 01/09/15 CPB The replace caused issues when address not defined in web.config
    ' will need to do the .Replace wherever you use this variable in your app
    'Public g_CompanyAddress As String = ConfigurationManager.AppSettings("Address").Replace("||", "<br>")
    Public g_CompanyAddress As String = ConfigurationManager.AppSettings("CompanyAddress")
    Public g_siteLogo As String = getSiteLogo()

    '---Email Key-->
    Public g_EmailEnabled As Boolean = IIf(UCase(ConfigurationManager.AppSettings("emailEnabled")) = "TRUE", True, False)
    Public g_EmailHostIPAddress As String = ConfigurationManager.AppSettings("emailIPAddress")
    Public g_EmailTimeout As Integer = ConfigurationManager.AppSettings("emailTimeout")     ' 03/06/15 email timeout control per install
    Public g_EmailPort As String = ConfigurationManager.AppSettings("emailPort")
    Public g_EmailUserName As String = ConfigurationManager.AppSettings("emailUserName")
    Public g_EmailPassword As String = ConfigurationManager.AppSettings("emailPassword")
    Public g_emailEnableSSL As String = ConfigurationManager.AppSettings("emailEnableSSL")
    Public g_defaultEmail As String = ConfigurationManager.AppSettings("defaultEmail")
    Public g_EmailErrors As Boolean = IIf(IsNothing(ConfigurationManager.AppSettings("emailErrors")), False, IIf(UCase(ConfigurationManager.AppSettings("emailErrors")) = "TRUE", True, False))
    Public g_EmailInserts As Boolean = IIf(IsNothing(ConfigurationManager.AppSettings("emailInserts")), False, IIf(UCase(ConfigurationManager.AppSettings("emailInserts")) = "TRUE", True, False))
    Public g_EmailUpdates As Boolean = IIf(IsNothing(ConfigurationManager.AppSettings("emailUpdates")), False, IIf(UCase(ConfigurationManager.AppSettings("emailUpdates")) = "TRUE", True, False))
    Public g_EmailDeletes As Boolean = IIf(IsNothing(ConfigurationManager.AppSettings("emailDeletes")), False, IIf(UCase(ConfigurationManager.AppSettings("emailDeletes")) = "TRUE", True, False))
    Public g_EmailIOAddress As String = IIf(IsNothing(ConfigurationManager.AppSettings("emailIOAddress")), "", ConfigurationManager.AppSettings("emailIOAddress"))

    '--------Audit Keys----08/06/15 T3-->
    Public g_errorLog As Boolean = IIf(UCase(ConfigurationManager.AppSettings("errorLog")) = "TRUE", True, False)
    Public g_auditUpdates As Boolean = IIf(UCase(ConfigurationManager.AppSettings("auditUpdates")) = "TRUE", True, False)
    Public g_logEmail As Boolean = IIf(UCase(ConfigurationManager.AppSettings("emailLog")) = "TRUE", True, False)
    Public g_itemsPerPage As Integer = IIf(IsNumeric(ConfigurationManager.AppSettings("itemsPerPage")), ConfigurationManager.AppSettings("itemsPerPage"), 10) ' 10/20/15 cpb default to 10 if not in web config

    '---User keys--- 1/12/16 T3-->
    Public g_newUser As String = IIf(IsNothing(ConfigurationManager.AppSettings("newUser")), "NONE", UCase(ConfigurationManager.AppSettings("newUser")))
    Public g_allowNewUser As Boolean = IIf(UCase(ConfigurationManager.AppSettings("allowNewUser")) = "TRUE", True, False)
    Public g_confirmNewUser As Boolean = IIf(UCase(ConfigurationManager.AppSettings("confirmNewUser")) = "TRUE", True, False)
    Public g_userIdField As String = UCase(ConfigurationManager.AppSettings("userIdField"))
    Public g_userEmailField As String = UCase(ConfigurationManager.AppSettings("userEmailField"))
    Public g_allowGuestUser As Boolean = IIf(UCase(ConfigurationManager.AppSettings("guestLogin")) = "TRUE", True, False)
    Public g_blnRequestLogin As Boolean = IIf(UCase(ConfigurationManager.AppSettings("requestLogin")) = "TRUE", True, False)

    '-------------------------------------------------Build Web Config Info----------------------------------------------------------------------------




    '-----------------These are being worked on for new login
    ' 01/19/15 T3 Set home links for Admin or Default users
    ' Public g_AdminHome As String = ConfigurationManager.AppSettings("AdminHome")
    ' Public g_DefaultHome As String = ConfigurationManager.AppSettings("DefaultHome")
    ' Public g_UnconfirmedAdminHome As String = ConfigurationManager.AppSettings("unconfirmedAdminHome")
    ' Public g_UnconfirmedDefaultHome As String = ConfigurationManager.AppSettings("unconfirmedDefaultHome")
    ' 01/19/15 T3 Setting to determine whether or not to show new user div & confirm new users

    Private Function getSiteLogo() As String
        Dim strSiteLogo As String = IIf(IsNothing(ConfigurationManager.AppSettings("siteLogo")), "", ConfigurationManager.AppSettings("siteLogo"))
        ' site logo file key should return the folder and image name (ie Images/logo.gif)
        If strSiteLogo = "" Then
        Else
            Dim strFileName As String = HttpContext.Current.Server.MapPath(strSiteLogo)
            If File.Exists(strFileName) Then
            Else
                strSiteLogo = ""
            End If
        End If
        Return strSiteLogo
    End Function

    Public Sub g_resetSessionVariables()
        System.Web.HttpContext.Current.Session("preserveSession") = ""
    End Sub

    Public Sub g_RetrieveSessions(ByRef txtSessions As TextBox)
        g_RetrieveSessions(txtSessions.Text)
    End Sub
    Public Sub g_RetrieveSessions(ByRef txtSessions As HiddenField)
        g_RetrieveSessions(txtSessions.Value)
    End Sub
    Public Sub g_RetrieveSessions(ByRef txtSessions As String)

        If Trim(txtSessions = "") Then
            ' no Sessions string area sent to client form
        Else
            Dim arrStrSessionVariables() As String = Split(txtSessions, "^^")


            ' restore session variables
            For Each strSessionVariable As String In arrStrSessionVariables
                Dim strSessionVariablePair() As String = Split(strSessionVariable, "||")
                System.Web.HttpContext.Current.Session(strSessionVariablePair(0)) = strSessionVariablePair(1)
            Next
        End If

    End Sub
    Public Sub g_SendSessions(ByRef txtSessions As TextBox)
        txtSessions.Text = g_SendSessions(txtSessions.Text)
    End Sub
    Public Sub g_SendSessions(ByRef txtSessions As HiddenField)
        txtSessions.Value = g_SendSessions(txtSessions.Value)
    End Sub
    Public Function g_SendSessions(ByRef txtSessions As String) As String

        '  Only do this if user is not signed on
        Dim strSessionsName As String = ""
        Dim strDelimiter As String = ""

        For Each txtFieldName As String In System.Web.HttpContext.Current.Session.Keys
            If txtFieldName.ToUpper = "RELAYMESSAGE" Then
            Else
                strSessionsName = strSessionsName & strDelimiter & txtFieldName & "||" & System.Web.HttpContext.Current.Session(txtFieldName)
                strDelimiter = "^^"
            End If
        Next

        Return strSessionsName

    End Function

    Public Sub g_ValidatePhone(ByRef PhoneTextBox As TextBox)
        PhoneTextBox.Attributes.Add("onfocus", "PhoneFocus(this);")
        PhoneTextBox.Attributes.Add("onblur", "PhoneBlur(this);")
        PhoneTextBox.Attributes.Add("onkeyup", "PhoneChange(this);")
        PhoneTextBox.Attributes.Add("onkeypress", "PhoneKey(this,event);")
        PhoneTextBox.Attributes.Add("onchange", "PhoneChange(this);")
        'PhoneTextBox.Attributes.Add("onpropertychange", "PhoneFocus(this);")   ' covers autofill

    End Sub
    Public Sub g_ValidateDate(ByRef DateTextBox As TextBox)
        DateTextBox.Attributes.Add("onfocus", "DateFocus(this);")
        DateTextBox.Attributes.Add("onblur", "DateBlur(this);")
        DateTextBox.Attributes.Add("onkeyup", "DateChange(this);")
        DateTextBox.Attributes.Add("onkeypress", "DateKey(this,event);")
        DateTextBox.Attributes.Add("onchange", "DateChange(this);")

    End Sub
    Public Sub g_TextMaxLengthEntry(ByRef TextBoxUsed As TextBox, ByVal MaxLength As Integer)
        TextBoxUsed.Attributes.Add("onkeyup", "TextLimitMessage(this," & MaxLength & ");")
        TextBoxUsed.Attributes.Add("onblur", "TextLimitMessageRemove(this);")

    End Sub

    Public Sub g_sendEmail(ByVal ToAddress As String, ByVal Subject As String, ByVal Message As String)
        g_sendEmail(ToAddress, Subject, Message, "", "", "", "")
    End Sub
    Public Sub g_sendEmail(ByVal ToAddress As String, ByVal Subject As String, ByVal Message As String, ByVal CCAddress As String)
        g_sendEmail(ToAddress, Subject, Message, CCAddress, "", "", "")
    End Sub
    Public Sub g_sendEmail(ByVal ToAddress As String, ByVal Subject As String, ByVal Message As String, ByVal CCAddress As String, ByVal emailTemplate As String)
        g_sendEmail(ToAddress, Subject, Message, CCAddress, emailTemplate, "", "")
    End Sub
    Public Sub g_sendEmail(ByVal ToAddress As String, ByVal Subject As String, ByVal Message As String, ByVal CCAddress As String, ByVal emailTemplate As String, ByVal calledFrom As String)
        g_sendEmail(ToAddress, Subject, Message, CCAddress, emailTemplate, calledFrom, "")
    End Sub
    Public Sub g_sendEmail(ByVal ToAddress As String, ByVal Subject As String, ByVal Message As String, ByVal CCAddress As String,
                           ByVal emailTemplate As String, ByVal calledFrom As String, ByVal fromAddress As String)
        'System.Web.HttpContext.Current.Session("Email_Flag") = "Beginning"

        'Dim strFromAddress As String = "admin@asga.fosterandbillups.com"  'ConfigurationManager.AppSettings("emailUserName")
        'Dim strFromPassword As String = "1Asga1!"  'ConfigurationManager.AppSettings("emailPassword")
        'Dim strToAddress As String = ToAddress
        'Dim strSubject As String = Subject
        'Dim strBody As String = Message
        'Dim smtp = New System.Net.Mail.SmtpClient()
        'smtp.Host = ConfigurationManager.AppSettings("EmailIPAddress")
        'If ConfigurationManager.AppSettings("emailPort") = "" Then
        'Else
        '    smtp.Port = ConfigurationManager.AppSettings("emailPort")
        'End If
        'If UCase(ConfigurationManager.AppSettings("emailEnableSSL")) = "TRUE" Then
        '    smtp.EnableSsl = True
        'End If
        'smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network
        'smtp.Credentials = New NetworkCredential(strFromAddress, strFromPassword)
        'smtp.Timeout = ConfigurationManager.AppSettings("emailTimeout")
        'smtp.Send(strFromAddress, strToAddress, strSubject, strBody)

        Debug.WriteLine("Email Module -- To: " & ToAddress & "  Subject: " & Subject & "   Message: " & Message)
        If ConfigurationManager.AppSettings("EmailEnabled") = True Then
            Dim Mail As New System.Net.Mail.MailMessage
            Mail.Subject = Subject
            If ToAddress = "" Then
                Debug.Print("ModMain (g_SendEmail): No email address provided. Can't send it.")
            Else
                ToAddress = ToAddress.Replace(",", ";")
                For Each strEmailAddress As String In Split(ToAddress, ";")
                    Mail.To.Add(strEmailAddress)
                Next

                ' 02/26/16 cpb add ability to send from email address
                If fromAddress = "" Then
                    fromAddress = g_defaultEmail
                End If
                Mail.From = New System.Net.Mail.MailAddress(fromAddress)

                'Prepare Body
                'Check to see if email Template was sent in 
                If emailTemplate = "" Then
                Else
                    Dim rdrEmailTemplate As StreamReader = New StreamReader(HttpContext.Current.Server.MapPath("/" & emailTemplate))
                    Dim htmlBody As String = rdrEmailTemplate.ReadToEnd
                    Message = htmlBody.Replace("{Body}", Message)
                End If

                Mail.Body = Message

                Dim strHTMLCheck As String = UCase(Message)
                Mail.IsBodyHtml = strHTMLCheck.ToUpper.Contains("<BODY") Or strHTMLCheck.ToUpper.Contains("<TABLE") Or strHTMLCheck.ToUpper.Contains("<DIV") Or strHTMLCheck.ToUpper.Contains("<BR")

                Dim SMTPServer As New System.Net.Mail.SmtpClient()
                SMTPServer.Timeout = g_EmailTimeout
                SMTPServer.Host = g_EmailHostIPAddress
                ' 4/8/16 cpb -- check for no port specified. - may not be needed (ref: 1and1)
                If g_EmailPort = "" Then
                Else
                    SMTPServer.Port = g_EmailPort
                End If
                If IsNothing(g_emailEnableSSL) Then
                    g_emailEnableSSL = False
                End If
                SMTPServer.EnableSsl = g_emailEnableSSL

                SMTPServer.Credentials = New System.Net.NetworkCredential(g_EmailUserName, g_EmailPassword)
                Debug.Print(ToAddress & " - " & Subject)
                If g_EmailEnabled Then
                    Try
                        SMTPServer.Send(Mail)
                        g_log_email(ToAddress, calledFrom, "Email sent succesfully.")
                    Catch ex As Exception
                        Dim errMessage As String = ex.Message
                        g_log_email(ToAddress, calledFrom, errMessage)
                    End Try

                End If
            End If
        End If
    End Sub
    Public Sub g_log_email(ByVal ToAddress As String, ByVal calledFrom As String, ByVal message As String)
        If g_logEmail Then
            HttpContext.Current.Session("EMAILLOGSELECT") = "1"
            Dim strSQL As String = "select count(*) as cnt from sys.Tables where name = 'log_email'"
            Dim tblLogEmailVerification As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblLogEmailVerification.Rows(0)("cnt") = 0 Then
                ' create the table...
                Dim strLogEmailTbl As String =
                "CREATE Table [dbo].[log_email](              " &
                "       [recid] [Int] IDENTITY(1, 1) Not NULL," &
                "       [ToAddress] [varchar](75) NULL,       " &
                "       [DateTime] [DateTime] NULL,           " &
                "       [calledFrom] [varchar](75) NULL,      " &
                "       [message] [varchar](3000) NULL        " &
                ") ON [PRIMARY]"
                g_IO_Execute_SQL(strLogEmailTbl, False)
            End If

            Dim strInsert As String = "insert into log_email (toAddress, calledFrom, message) values ('" & ToAddress.Replace("'", "''") & "','" & calledFrom & "','" & message.Replace("'", "''") & "')"
            g_IO_Execute_SQL(strInsert, False)
            HttpContext.Current.Session.Remove("EMAILLOGSELECT")
        End If

    End Sub

    'Public Sub g_sendEmail(ByVal ToAddress As String, ByVal Subject As String, ByVal Message As String)
    '    g_sendEmail(ToAddress, Subject, Message, "")
    'End Sub

    'Public Sub g_sendEmail(ByVal ToAddress As String, ByVal Subject As String, ByVal Message As String, ByVal CCAddress As String)

    '    'Debug.WriteLine("Email Module -- To: " & ToAddress & "  Subject: " & Subject & "   Message: " & Message)

    '    If g_EmailEnabled = True Then
    '        Dim Mail As New System.Net.Mail.MailMessage
    '        Mail.Subject = Subject
    '        If ToAddress = "" Then
    '            'Debug.Print("ModMain (g_SendEmail): No email address provided. Can't send it.")
    '        Else
    '            ToAddress = ToAddress.Replace(",", ";")
    '            For Each strEmailAddress As String In Split(ToAddress, ";")
    '                Mail.To.Add(strEmailAddress)
    '            Next
    '            If CCAddress = "" Then
    '            Else
    '                CCAddress = CCAddress.Replace(",", ";")
    '                For Each strEmailAddress As String In Split(CCAddress, ";")
    '                    Mail.CC.Add(strEmailAddress)
    '                Next
    '            End If

    '            'Mail.From = New System.Net.Mail.MailAddress(g_EmailUserName)
    '            ' 2/13/15 changed to use default email address instead of user name
    '            Mail.From = New System.Net.Mail.MailAddress(g_defaultEmail)
    '            Mail.Body = Message
    '            Dim strHTMLCheck As String = UCase(Message)
    '            Mail.IsBodyHtml = strHTMLCheck.ToUpper.Contains("<BODY") Or strHTMLCheck.ToUpper.Contains("<TABLE") Or strHTMLCheck.ToUpper.Contains("<DIV") Or strHTMLCheck.ToUpper.Contains("<BR") Or strHTMLCheck.ToUpper.Contains("<P")
    '            Dim SMTPServer As New System.Net.Mail.SmtpClient()
    '            SMTPServer.Timeout = g_EmailTimeout    ' 03/06/15 control per install     '20000 '5000
    '            SMTPServer.Host = g_EmailHostIPAddress
    '            If g_emailEnableSSL = True Then
    '                SMTPServer.EnableSsl = True
    '            End If
    '            If g_EmailPort <> "" Then
    '                SMTPServer.Port = g_EmailPort
    '            End If
    '            SMTPServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network
    '            SMTPServer.Credentials = New System.Net.NetworkCredential(g_EmailUserName, g_EmailPassword)
    '            'Debug.Print(ToAddress & " - " & Subject)
    '            If g_EmailEnabled Then
    '                SMTPServer.Send(Mail)
    '            End If
    '        End If
    '    End If
    'End Sub
    '09/29/14 T3- Created
    Public Sub g_changeAppSettings(ByVal key As String, ByVal NewValue As String)
        Dim cfg As Configuration
        cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~")
        Dim setting As KeyValueConfigurationElement = CType(cfg.AppSettings.Settings(key), KeyValueConfigurationElement)
        If setting Is Nothing Then
        Else
            setting.Value = NewValue
            cfg.Save()
        End If
    End Sub
    Public Sub g_loadStateDropDown(ByRef ddlState As DropDownList, ByVal blnAddHeaderRow As Boolean)
        ' 11/12/14 cpb common routine that can be used on all forms to load states dropdown
        Dim strSQL As String = ""
        strSQL = " Select RECID, Abbr, state FROM states "

        Dim tblStates As DataTable = g_IO_Execute_SQL(strSQL, False)

        If blnAddHeaderRow Then
            Dim rowState As DataRow = tblStates.NewRow
            rowState("State") = "State"
            rowState("RECID") = -1
            tblStates.Rows.InsertAt(rowState, 0)
        End If

        ddlState.DataSource = tblStates
        ddlState.DataValueField = "RECID"
        ddlState.DataTextField = "State"
        ddlState.DataBind()
        ddlState.SelectedValue = g_DefaultStateRecid
    End Sub

    ' 02/16/15 t3 add encryption/decription to string routines
    Private key() As Byte = {}
    Private IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
    Private Const EncryptionKey As String = "abcdefgh"
    Public Function g_Decrypt(ByVal stringToDecrypt As String) As String
        Try
            Dim inputByteArray(stringToDecrypt.Length) As Byte
            key = System.Text.Encoding.UTF8.GetBytes(Left(EncryptionKey, 8))
            Dim des As New DESCryptoServiceProvider
            inputByteArray = Convert.FromBase64String(stringToDecrypt)
            Dim ms As New MemoryStream
            Dim cs As New CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
            Return ConvertHexToString(encoding.GetString(ms.ToArray()))
        Catch ex As Exception
            'oops - add your exception logic
            Return stringToDecrypt
        End Try
    End Function

    Public Function g_Encrypt(ByVal stringToEncrypt As String) As String
        Try
            stringToEncrypt = ConvertStringToHex(stringToEncrypt)

            key = System.Text.Encoding.UTF8.GetBytes(Left(EncryptionKey, 8))
            Dim des As New DESCryptoServiceProvider
            Dim inputByteArray() As Byte = Encoding.UTF8.GetBytes(stringToEncrypt)
            Dim ms As New MemoryStream
            Dim cs As New CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Return Convert.ToBase64String(ms.ToArray())
        Catch ex As Exception
            'oops - add your exception logic
            Return stringToEncrypt
        End Try
    End Function
    Public Function ConvertStringToHex(ByVal strToConvert As String) As String
        Dim arrBytes() = strToConvert.ToCharArray
        Dim strHexStringReturn As String = ""
        For Each strChar As Char In arrBytes
            strHexStringReturn &= Hex(Asc(strChar))
        Next
        Return strHexStringReturn
    End Function
    Public Function ConvertHexToString(ByVal HexToConvert As String) As String
        Dim numberChars As Integer = HexToConvert.Length
        Dim strStringToReturn As String = ""
        For i = 1 To numberChars Step 2
            strStringToReturn &= Chr(Convert.ToInt32(Mid(HexToConvert, i, 2), 16))
        Next
        Return strStringToReturn

    End Function

    Public Sub g_flmSaveFunction(ByVal functionToCall As String, ByVal strTable As String, ByVal intRecid As Integer)
        ' Called from frmListManager: Table has ext prop to run a save function
        Dim blnSaveFunction As Boolean = functionToCall & "(" & strTable & ", " & intRecid & ")"
    End Sub

    Public Sub g_DebugAutoLogin()
        ' 6/26/15 this will be launched from frmLogin Load event
        ' required key in web.config ConfigurationManager.AppSettings("debugAutoLogin") = "true"
        If Debugger.IsAttached Then
            System.Web.HttpContext.Current.Session("user_link_id") = "1"
            System.Web.HttpContext.Current.Session("user_role") = "ADMINISTRATOR"
            System.Web.HttpContext.Current.Session("user_name") = "Admin"
            System.Web.HttpContext.Current.Response.Redirect(g_LoginRedirect("Login"))
            System.Web.HttpContext.Current.Session("AccountPasswords") = "1"
            System.Web.HttpContext.Current.Session("AccountRoles") = "1"
            System.Web.HttpContext.Current.Session("AccountView") = "1"
            System.Web.HttpContext.Current.Session("AccountAdd") = "1"
            System.Web.HttpContext.Current.Session("AccountEdit") = "1"
            System.Web.HttpContext.Current.Session("AccountDelete") = "1"
        End If
    End Sub

    ' 2/2/16 T3 set user login param based
    Public Function g_UserLogin(ByRef tblUser As DataTable) As String
        '091515 User Active
        Dim strMessage As String = ""
        Dim blnActive As Boolean = True
        If tblUser.Rows.Count = 0 Then
            blnActive = False
        End If
        If IsNothing(tblUser.Columns("active")) Then
        Else
            If tblUser.Rows(0)("active") = 0 Then
                blnActive = False
            End If
        End If
        If blnActive Then
            System.Web.HttpContext.Current.Session("user_link_id") = tblUser.Rows(0)("recid")
            g_UserRecId = tblUser.Rows(0)("recid")
            Try
                System.Web.HttpContext.Current.Session("user_role") = tblUser.Rows(0)("userRole")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("user_role") = tblUser.Rows(0)("user_role")
            End Try
            Try
                ' 1/4/17 CS added check for dbNull b/c the session variable does get set to null without errroring out
                If IsDBNull(tblUser.Rows(0)("firstName")) Then
                    System.Web.HttpContext.Current.Session("user_name") = tblUser.Rows(0)(g_userIdField)
                Else
                    System.Web.HttpContext.Current.Session("user_name") = tblUser.Rows(0)("firstName")
                End If
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("user_name") = tblUser.Rows(0)(g_userIdField)
            End Try

            Try
                System.Web.HttpContext.Current.Session("user_name") = tblUser.Rows(0)("firstName")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("user_name") = tblUser.Rows(0)(g_userIdField)
            End Try

            ' 1/20/16 cpb put these in try for backward compatible with databaess
            Try
                System.Web.HttpContext.Current.Session("AccountPasswords") = tblUser.Rows(0)("AccountPasswords")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("AccountPasswords") = False
            End Try
            Try
                System.Web.HttpContext.Current.Session("AccountRoles") = tblUser.Rows(0)("AccountRoles")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("AccountRoles") = False
            End Try
            Try
                System.Web.HttpContext.Current.Session("AccountView") = tblUser.Rows(0)("AccountView")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("AccountView") = False
            End Try
            Try
                System.Web.HttpContext.Current.Session("AccountAdd") = tblUser.Rows(0)("AccountAdd")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("AccountAdd") = False
            End Try
            Try
                System.Web.HttpContext.Current.Session("AccountEdit") = tblUser.Rows(0)("AccountEdit")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("AccountEdit") = False
            End Try
            Try
                System.Web.HttpContext.Current.Session("AccountDelete") = tblUser.Rows(0)("AccountDelete")
            Catch ex As Exception
                System.Web.HttpContext.Current.Session("AccountDelete") = False
            End Try

            ' 09/17/15 cpb set any Sessions for quick access from users account to prevent having to always read for them
            g_setSiteUserSessions(tblUser.Rows(0))

            ' 9/14/15 cpb add capture of last login for this user
            Dim nvcSysUsersUpdate As New NameValueCollection
            nvcSysUsersUpdate("lastLogin") = Date.Now
            g_IO_SQLUpdate("sys_Users", nvcSysUsersUpdate, "frmLogin", "recid = '" & tblUser(0)("recid") & "'")
            If ConfigurationManager.AppSettings("logLogins") = "true" Then
                Dim strSQL As String = "Update sys_Users Set lastLogin = getdate() Where recid = '" & tblUser(0)("recid") & "'"
                g_IO_Execute_SQL(strSQL, False)
            End If
        Else
            strMessage = "<h4>Sorry, your account is currently inactive.</h4>"
        End If
        Return strMessage
    End Function

    Public Sub g_getFLMCustomFormParameters(ByRef intCid As Integer, ByRef strMid As String, ByRef strSesPrefix As String, ByVal strUrl As String)
        '1/7/16 T3 For custom forms let's extract the cid (selected row recid) and mode ('view', 'edit') sent over from FLM

        If Left(strUrl, 2) = "E=" Then
            strUrl = g_Decrypt(Mid(strUrl, 3))
        End If

        ' Loop through url string and set parameters/session variables
        Dim arrUrl() = Split(strUrl, "&")
        For Each strParam In arrUrl
            Dim arrParam() As String = Split(strParam, "=")
            If arrParam(0) = "cid" Then
                intCid = arrParam(1)
            End If
            If arrParam(0) = "mid" Then
                strMid = UCase(arrParam(1))
            End If
            If arrParam(0) = "sesPrefix" Then
                strSesPrefix = UCase(arrParam(1))
            End If
        Next


    End Sub

    Public Function g_GetUrlParameterValue(ByRef strUrl As String, ByVal strAction As String) As String
        Dim strValue As String = ""

        If strUrl.Contains("?") Then
            strUrl = Split(strUrl, "?")(1)

            If Left(strUrl, 2) = "E=" Then
                strUrl = g_Decrypt(Mid(strUrl, 3))
            End If

            If strUrl.Contains(strAction) Then
                strValue = Split(Split(strUrl, strAction & "=")(1), "&")(0)
            End If
        End If

        Return strValue
    End Function

    Public Function g_resizeImage(ByVal fileToResize As String, ByVal maxHeight As Double, ByVal maxWidth As Double, ByVal newFileName As String)
        'following code resizes picture to fit
        Dim bm As New System.Drawing.Bitmap(fileToResize)
        Dim Width As Integer = bm.Width
        Dim Height As Integer = bm.Height
        If bm.Height <= maxHeight And bm.Width <= maxWidth Then
        Else
            Dim percentHeightResize As Double = maxHeight / bm.Height
            Dim percentWidthResize As Double = maxWidth / bm.Width
            If percentHeightResize < percentWidthResize Then
                Width = bm.Width * percentHeightResize 'image width. 
                Height = bm.Height * percentHeightResize 'image height
            Else
                Width = bm.Width * percentWidthResize 'image width. 
                Height = bm.Height * percentWidthResize  'image height
            End If
        End If

        Dim thumb As New System.Drawing.Bitmap(Width, Height)
        Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(thumb)
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
        g.DrawImage(bm, New System.Drawing.Rectangle(0, 0, Width, Height), New System.Drawing.Rectangle(0, 0, bm.Width, bm.Height), System.Drawing.GraphicsUnit.Pixel)
        g.Dispose()
        bm.Dispose()
        'image path.
        Dim intLength As Integer = newFileName.Length
        Dim strExtn As String = "PNG"
        For x = intLength To 1 Step -1
            If Mid(newFileName, x, 1) = "." Then
                strExtn = UCase(Mid(newFileName, x + 1))
                Exit For
            End If
        Next
        Select Case strExtn
            ' find out what file type to save thumbnail as...otherwise save as png
            Case "JPG"
                thumb.Save(newFileName,
                System.Drawing.Imaging.ImageFormat.Jpeg)
            Case "JPEG"
                thumb.Save(newFileName,
                System.Drawing.Imaging.ImageFormat.Jpeg)
            Case "PNG"
                thumb.Save(newFileName,
                System.Drawing.Imaging.ImageFormat.Png)
            Case "GIF"
                thumb.Save(newFileName,
                System.Drawing.Imaging.ImageFormat.Gif)
            Case "ICO"
                thumb.Save(newFileName,
                System.Drawing.Imaging.ImageFormat.Icon)
            Case "TIFF"
                thumb.Save(newFileName,
                System.Drawing.Imaging.ImageFormat.Tiff)
            Case Else
                thumb.Save(newFileName,
                System.Drawing.Imaging.ImageFormat.Png)
        End Select

        Return thumb
    End Function


End Module
