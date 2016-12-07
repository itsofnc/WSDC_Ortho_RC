
'***********************************Last Edit**************************************************
'Developer's notes:****************PLEASE READ************************************************** 
'*First time Setup: set webconfig key "newInstall" = true, then you can change the app settings key value using g_changeAppSettings in ModMain

'Last Edit Date: 01/22/15
'  Last Edit By: cpb
'Last Edit Proj: HMIWebPortal
'-----------------------------------------------------
'Change Log:
'01/22/15 cpb verify sys users exists
' 09/29/14 - T3 -Created 
'**********************************************************************************************
Module ModSetup

    '02/23/16 cpb this routine has been replaced no longer needed
    '01/22/15 cpb verify sys users exists
    'Public Function g_verifySysUsersTableExists() As Boolean
    '    Dim blnSysUsersTable As Boolean = False
    '    Dim strCheck As String = " select count(*) as tblCount from sys.Tables where name = 'sys_Users' "
    '    If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") = 0 Then
    '    Else
    '        blnSysUsersTable = True
    '    End If
    '    Return blnSysUsersTable
    'End Function

    Public Function g_verifyDatabaseExists() As Boolean
        Dim blnDBExists As Boolean = False
        ' verify that our database does exist
        Dim strSQL As String = "Select * From master.dbo.sysdatabases where Name = '" & g_ConnectionSchema & "'"
        Dim tblDB As DataTable = g_IO_Execute_SQL(strSQL, False)
        If tblDB.Rows.Count = 0 Then
            strSQL = "CREATE DATABASE [" & g_ConnectionSchema & "]"
            Try
                g_IO_Execute_SQL(strSQL, False)
                blnDBExists = True
            Catch ex As Exception

            End Try
        Else
            blnDBExists = True
        End If
        Return blnDBExists

    End Function

    '09/29/14 T3 Create common code required system table
    Public Function g_verifySys_Users() As Boolean
        Dim blnUserSetupOK As Boolean = False

        If g_verifyDatabaseExists() Then
            Try
                CreateUserRolesTbl()
                CreateSysUsersTable()
                CreateUserRoleWhereUsed()
                blnUserSetupOK = True
            Catch ex As Exception
            End Try
        End If

        Return blnUserSetupOK
    End Function

    Private Sub CreateUserRolesTbl()
        ' 01/22/15 cpb
        Dim strSQL As String = ""
        Dim strCheck As String = "Select count(*) As TblCount From sys.Tables Where name = 'DropDownList__UserRoles'"
        If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") = 0 Then
            strSQL = "CREATE TABLE [dbo].[DropDownList__UserRoles](" &
                "[RECID] [int] IDENTITY(1,1) NOT NULL," &
                "[DESCR] [varchar](45) NULL," &
                "CONSTRAINT [PK_DropDownList__UserRoles_vw] PRIMARY KEY CLUSTERED " &
                "( [RECID] Asc" &
                ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]" &
                ") ON [PRIMARY]; " &
                "ALTER TABLE [dbo].[DropDownList__UserRoles] ADD  DEFAULT ('') FOR [DESCR]; "
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'User Role' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles', @level2type=N'COLUMN',@level2name=N'DESCR'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'required', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles', @level2type=N'COLUMN',@level2name=N'DESCR'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'addDescription', @value=N'New User Role' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'confirmDeleteView', @value=N'frmListManager_whereUsed_UserRoles_vw' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'deleteDisplayColumn', @value=N'DESCR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'deleteDisplayText', @value=N'User Role' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'displayDescription', @value=N'User Roles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'editDescription', @value=N'Edit User Role' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'sortDefaultCol', @value=N'Descr' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "EXEC sys.sp_addextendedproperty @name=N'unique', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DropDownList__UserRoles', @level2type=N'COLUMN',@level2name=N'DESCR'"
            g_IO_Execute_SQL(strSQL, False)
            strSQL = "INSERT INTO [DropDownList__UserRoles] ([DESCR]) VALUES('Administrator');"
            strSQL &= "INSERT INTO [DropDownList__UserRoles] ([DESCR]) VALUES('User')"
            g_IO_Execute_SQL(strSQL, False)
            ' see if we have any custom roles to define
            g_UserRoles_AppFields()
        End If

    End Sub

    Private Sub CreateUserRoleWhereUsed()
        ' 2/23/16 cpb moved to separate routine -- cant build this view until after sysusers table has been created.
        Dim strCheck As String = "Select count(*) As TblCount From sys.Views Where name = 'frmListManager_whereUsed_UserRoles_vw'"
        If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") = 0 Then
            Dim strSQL As String = "CREATE VIEW [dbo].[frmListManager_whereUsed_UserRoles_vw] " &
            "As Select RECID, " &
            "(Select     COUNT(*) As Expr1" &
            "      FROM  dbo.sys_Users" &
            "      WHERE (dbo.DropDownList__UserRoles.RECID = user_role)) As Users " &
            "FROM dbo.DropDownList__UserRoles"
            g_IO_Execute_SQL(strSQL, False)
        End If
    End Sub

    Private Sub CreateSysUsersTable()
        Dim blnSysUsersCreated As Boolean = False
        Dim strSQL As String = ""
        Dim strCheck As String = " select count(*) as tblCount from sys.Tables where name = 'sys_Users' "
        If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") = 0 Then
            strSQL = "CREATE TABLE [sys_Users](" &
                      "[recid] [int] IDENTITY(1,1) NOT NULL," &
                      "[" & g_userIdField & "] [varchar](45) NOT NULL, " &
                      "[firstName] [varchar](50) NULL," &
                      "[lastName] [varchar](50) NULL," &
                      "[password] [char](35) NOT NULL,   " &
                      "[user_role] [int] NOT NULL, " &
                      "[" & g_userEmailField & "] [varchar](50) NOT NULL, " &
                      "[lastLogin] [datetime] NULL " &
                      ") ON [PRIMARY]; " &
                      "ALTER TABLE [dbo].[sys_Users] ADD  CONSTRAINT [DF_sys_Users_firstName]  DEFAULT ('') FOR [firstName]" &
                      "ALTER TABLE [dbo].[sys_Users] ADD  CONSTRAINT [DF_sys_Users_lastName]  DEFAULT ('') FOR [lastName]" &
                      "ALTER TABLE [dbo].[sys_Users] ADD  CONSTRAINT [DF_sys_Users_user_role]  DEFAULT ((-1)) FOR [user_role]" &
                      "ALTER TABLE [dbo].[sys_Users] ADD  CONSTRAINT [DF_sys_Users_" & g_userEmailField & "]  DEFAULT ('') FOR [" & g_userEmailField & "] "
            g_IO_Execute_SQL(strSQL, False)

            If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") = 0 Then
            Else
                blnSysUsersCreated = True
                ' 2/23/16 cpb -- verify the table got created -- if not this creates a large number of error emails
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'User Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users', @level2type=N'COLUMN',@level2name=N'" & g_userIdField & "'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'required', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users', @level2type=N'COLUMN',@level2name=N'" & g_userIdField & "'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'hidden', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users', @level2type=N'COLUMN',@level2name=N'password'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'required', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users', @level2type=N'COLUMN',@level2name=N'password'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'showInGrid', @value=N'false' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users', @level2type=N'COLUMN',@level2name=N'password'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'addDescription', @value=N'New User' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'deleteDisplayColumn', @value=N'" & g_userIdField & "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'deleteDisplayText', @value=N'User' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users'"
                g_IO_Execute_SQL(strSQL, False)
                strSQL = "EXEC sys.sp_addextendedproperty @name=N'editDescription', @value=N'Edit User' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users'"
                g_IO_Execute_SQL(strSQL, False)

                ' 01/22/15  cpb brand new users table should have at least one startup login.
                Dim tblUserRoles As DataTable = g_IO_Execute_SQL("Select recid From DropDownList__UserRoles Where Descr = 'Administrator'", False)
                Dim intAdminRecId As Integer = -1
                If tblUserRoles.Rows.Count > 0 Then
                    intAdminRecId = tblUserRoles.Rows(0)("recid")
                End If
                '02/23/16 cpb set user email address in case used for login type.  Also set admin first name for welcome prompt.
                Dim strAdminEmail As String = g_EmailIOAddress
                If LCase(g_LoginIdStyle).Contains("email") Then
                    strAdminEmail = "admin@" & g_SiteUrl.Replace("www.", "").Replace("http:\\", "").Replace("https:\\", "")
                End If
                strSQL = "INSERT INTO [sys_Users] ([" & g_userIdField & "],[password],[firstName],[user_role],[" & g_userEmailField & "]) " &
                    "VALUES ('admin',CONVERT(VARCHAR(32), HashBytes('MD5', 'Admin1'), 2),'Admin'," & intAdminRecId & ",'" & strAdminEmail & "')"
                g_IO_Execute_SQL(strSQL, False)
                '01/22/15 cpb see if there are any custom fields the users table/view needs for the app
                g_Sys_Users_AppFields()
            End If
        End If

        '01/22/15 cpb setup view for sys_users table
        '02/23/16 cpb only create view if table was created.
        If blnSysUsersCreated Then
            strCheck = " select count(*) as tblCount from sys.Views where name = 'Users_vw' "
            If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") = 0 Then
                strSQL = "CREATE VIEW[Users_vw] AS " &
                    "SELECT sys_Users.recid, sys_Users." & g_userIdField & ", sys_Users.password, sys_Users.firstName, sys_Users.lastName, sys_Users." & g_userEmailField & ", " &
                    "sys_Users.user_role , DropDownList__UserRoles.DESCR AS userRole " &
                    "FROM sys_Users LEFT OUTER JOIN " &
                    "DropDownList__UserRoles ON sys_Users.user_role = DropDownList__UserRoles.RECID; "
                g_IO_Execute_SQL(strSQL, False)
                '02/23/16 cpb verify view got created before adding ext properties
                If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") > 0 Then
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'sortDefaultCol', @value=N'" & g_userIdField & "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sys_Users'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'User Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'" & g_userIdField & "'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'required', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'" & g_userIdField & "'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'unique', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'" & g_userIdField & "'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Password' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'password'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'pwd', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'password'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'required', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'password'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'First Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'firstName'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'required', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'firstName'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Last Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'lastName'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'email', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'" & g_userEmailField & "'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Email' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'" & g_userEmailField & "'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'required', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'" & g_userEmailField & "'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'ddlTable', @value=N'DropDownList__UserRoles' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'user_role'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'ddlText', @value=N'Descr' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'user_role'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'ddlValue', @value=N'recid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'user_role'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'User Role' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'user_role'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'hidden', @value=N'true' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw', @level2type=N'COLUMN',@level2name=N'userRole'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'addDescription', @value=N'Add User' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = " EXEC sys.sp_addextendedproperty @name=N'deleteDisplayColumn', @value=N'" & g_userIdField & "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'deleteDisplayText', @value=N'User ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'displayDescription', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'editDescription', @value=N'Edit User' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'editForm', @value=N'frmUserAccount.aspx' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw'"
                    g_IO_Execute_SQL(strSQL, False)
                    strSQL = "EXEC sys.sp_addextendedproperty @name=N'MasterTable', @value=N'sys_Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'users_vw'"
                End If
            End If
        End If

    End Sub


    Public Sub g_createPwdRecoveryTbl()
        Dim strCheck As String = " select count(*) as tblCount from sys.Tables where name = 'pwdRecovery' "
        If g_IO_Execute_SQL(strCheck, False).Rows(0)("tblCount") = 0 Then
            Dim strSql As String = "CREATE TABLE pwdRecovery( recid   int  IDENTITY(1,1) NOT NULL, resetKey   varchar (35) NULL, " &
                " userRecid   int  NULL, addDate   datetime  NULL, CONSTRAINT  PK_pwdRecovery  PRIMARY KEY CLUSTERED (recid Asc ) " &
                " WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON  [PRIMARY] ) ON  [PRIMARY] "
            g_IO_Execute_SQL(strSql, False)
        End If
    End Sub


End Module
