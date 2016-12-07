Public Class frmForgotPassword
    Inherits System.Web.UI.Page

    '***********************************Last Edit**************************************************
    '
    'Last Edit Date: 03/12/15
    '  Last Edit By: cpb
    'Last Edit Proj: msCaseFiles
    '-----------------------------------------------------
    'Change Log:
    ' 03/12/15 add replace to handle old usage of 'user_id'
    ' 02/04/15 cpb use global variables instead of configuration manager
    '**********************************************************************************************

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Create pwdRecovery table if needed 
        createPwdRecoveryTbl()

        ' 02/04/15 cpb use global variables instead of configuration manager
        Dim LoginIdStyle As String = "email"
        If IsNothing(g_LoginIdStyle) Then
        Else
            If g_LoginIdStyle = "" Then
            Else
                LoginIdStyle = g_LoginIdStyle
            End If
        End If

        Dim strUserBeingReset As String = ""
        If IsNothing(Request.QueryString("un")) Then
        Else
            strUserBeingReset = Request.QueryString("un")
        End If
        If UCase(LoginIdStyle.Replace("_", "")) = "USERID" Then     ' 03/12/15 add replace to handle old usage of 'user_id'
            'Focus on User ID Textbox
            If strUserBeingReset <> "" Then
                txtUserID.Text = strUserBeingReset
            End If
            litLoginType.Text = "User ID"
            txtUserID.Focus()
            litScripts.Text = "<script>jQuery(document).ready ( function () {jQuery(""#email"").addClass(""hidden"");} )</script>"
        ElseIf UCase(g_LoginIdStyle).Contains("EMAIL") Then
            'Focus on Email Textbox
            If strUserBeingReset <> "" Then
                txtEmail.Text = strUserBeingReset
            End If
            litLoginType.Text = "Email ID"
            txtEmail.Focus()
            litScripts.Text = "<script>jQuery(document).ready ( function () {jQuery(""#userId"").addClass(""hidden"");} )</script>"
        End If

        'Validation
        txtEmail.Attributes("onBlur") = "validateEmail()"
        txtUserID.Attributes("onBlur") = "validateUserId()"
    End Sub
    Private Sub createPwdRecoveryTbl()
        g_createPwdRecoveryTbl()
    End Sub
End Class