Public Class frmListManager
    Inherits System.Web.UI.Page

    '***********************************Last Edit**************************************************

    'Last Edit Date: 11/25/15 
    'Last Edit By: cpb 
    'Last Edit Proj: ASGA
    '-----------------------------------------------------
    'Change Log:
    '11/25/15 cpb check for SQL default date of 01/01/1900 and handle date display in data grid
    '09/30/15 - add allow to send session prefix for iframe
    '9/29/15 T3 Added Session Variable Prefix for multi tab frmListManager Use
    '6/9/15 T3 Coded to handle encrypted URL string vs QueryString - decrypt and create session variables
    '06/05/2015 RLO removing options column did not cover the TOTALS line at the end of the grid
    '06/04/15 T3 View Edit Delete Add Options via url perm=0000, link with our own parameters via editForm, Encrypt/Decrypt URL
    '3/13/15 T3 Check for unique data validation
    '02/20/15 t3- we added email links and started on adding custom links. Look at ext. prop "link" to see how
    '02/13/15 t3 rename hidtableid to hidmenuid, code to use menu for setting active instead of table id
    '02/03/15 cpb protect if user gets into this form with invalid table name.
    '02/02/15 cpb add ext propery for column display location left,right,center
    '01/09/15 - cpb - add viewDescription paramater
    '12/30/14 - cp - added m_sessionUsr/ Session(hidSessionsPrefix.value & "usr") to filter grid by user ' cp
    '10/13/14 - t3 - control pagination show/hide by screen size (added accordian around divPag..)
    '10/6/14 - t3 - added  m_nvcColumnEmail, & m_nvcColumnPhone & validators--
    '10/6/14 - t3 - changed view only test to look at user access level in sys_Security
    '10/3/14 - t3 - added required field and validation for password
    '09/29/14 - t3 - added GetFieldType & checkRequiredFields js functions 
    '09/19/14  - t3 - name each modal unique for date updates
    '09/18/14 - cpb - handle date
    '09/17/14 - rlo - fix a fix???
    '09/15/14-t3 - moved function GetTableDescription (moved to ModIO)
    '09/08/14 cs/cb - determine if ext property comes from view or table
    '8/29/14 - t3
    'Added m_nvcColumnIndex
    '8/28/14 - CP
    'On ASP side: Added style 'min-width:80px;' to the grid's table header(th)  to fix wrapping issue
    'On VB side: In LoadListTableToGrid, checking column type of (money & time) to display formatted data in grid i.e. $10.00 
    'Note-828 : Added the 'add' parameter for url. Allows us to have control on whether we want a popup or redirect when we click Add Btn
    '           in the pagination area. It defaults to a popup but if you want to redirect you have to specify the page you want to send them to in the parameter.
    '           (i.e frmListManager.aspx?add=ContractEntry.aspx) Being used in WSDC Ortho 
    '           Added 'pre' parameter for URL. Allows us to preserve sessions. Used in Pagination (i.e.) pre=1 
    '8/27/14 - CP
    'Added HiddenField called 'hidTableName' on asp side. Using the hidden field to set links active in the navbar on pages that use frmListmanager: See Note-827 
    'Removed "cursor: hand; cursor: pointer;" from the columns being built in the gridlist
    '8/26/14 - t3
    '-View Only Mode
    '-Send list of restricted recids
    '8/22/14 - t3
    '   add prompt for delete indicating value being deleted used ext property from sql; updated excel sheet
    '8/20/14 - CS
    'added parameters to both search panels... onclick="setPaginationChevron('collapseOne','chevronAccordian')"
    '8/15/14 - cpb/rlo/cfs
    'add force of goto page number to numeric
    'verify user logged on within this form
    '7/18/14 - cp/cpb/rlo
    'added ext. properties(hidden(DO NOT SHOW COLUMNS ANYWHERE), disabled, showInGrid for columns)
    'fixed DDL in popup to show for text field
    'control buttons in Popup based on viewMode(view/edit/delete)
    'litScripts is being set via session variable
    '    Deleted         
    'Dim litLinkDropdownManager As Literal = Master.FindControl("litDropdownManager")
    'Dim litWelcome As Literal = Master.FindControl("litWelcome")
    'Dim litSignOn As Literal = Master.FindControl("litSignOn")
    '
    '7/9/14 - rlo
    '   Call the form with the query id "lists" equal to anything will reset the form
    '7/9/14 - cpb
    '   add overload for get table description; add ability to return default to table name or return default as ''
    '   Build Edit/View/Delete column 7/9/14 cpb build view/edit/delete based on how edit form loads
    '6/30/14-
    '   added target location to Onclick of grid row (added extended properties "editTarget & editForm")
    '   added mid(mode) to url to check view/edit/delete mode
    '   added divViewOnly to disable all inputs in view only mode and used z-index to show tabs in view mode
    '   also added an options column for view/edit/delete on gridlist
    '6/27/14-
    '   Hide Tabs if viewing a specific table
    '   Created ajaxFunctioins(set current id/tab sessions instead of sending in url
    '   Onload -verified existance of site master literals before setting text 
    '   Removed pagination if no records found
    '
    '**********************************************************************************************


    ''onclick="getDataRow(-1,'<%= Session(hidSessionsPrefix.value & "SelectedList")%>','<%# Session(hidSessionsPrefix.value & "editDescription")%>')"
    Dim m_intItemsPerPage As Integer = g_itemsPerPage
    Dim m_intPageNo As Integer = 1
    Dim m_introwcount As Integer = 0
    Dim m_intPageCount As Integer = 0
    Dim m_nvcColumnType As New NameValueCollection
    Dim m_nvcColumnLength As New NameValueCollection
    Dim m_nvcColumnDescription As New NameValueCollection
    Dim m_nvcColumnDDLTableName As New NameValueCollection
    Dim m_nvcColumnDDLValue As New NameValueCollection
    Dim m_nvcColumnDDLText As New NameValueCollection
    Dim m_nvcColumnDDLFilter As New NameValueCollection
    Dim m_nvcColumnPwd As New NameValueCollection
    Dim m_nvcColumnShowInGrid As New NameValueCollection
    Dim m_nvcColumnShowInPopup As New NameValueCollection
    Dim m_nvcHidden As New NameValueCollection
    Dim m_nvcDisabled As New NameValueCollection
    Dim m_nvcColumnDeleteValue As New NameValueCollection
    Dim m_nvcColumnIndex As New NameValueCollection
    Dim m_nvcColumnRequired As New NameValueCollection
    Dim m_nvcColumnEmail As New NameValueCollection
    Dim m_nvcColumnPhone As New NameValueCollection
    Dim m_nvcColumnTotal As New NameValueCollection
    Dim m_nvcColumnDisplayLocn As New NameValueCollection
    Dim m_nvcColumnlink As New NameValueCollection '2/20/15
    Dim m_nvcColumnUnique As New NameValueCollection ' 3/13/15
    Dim m_nvcColumnNames As New NameValueCollection '04/30/15 T3
    Dim m_nvcColumnRegExpPattern As New NameValueCollection ' 5/19/15 T3
    Dim m_nvcColumnRegExpMessage As New NameValueCollection ' 5/19/15 T3
    Dim m_nvcMinValue As New NameValueCollection
    Dim m_nvcMaxValue As New NameValueCollection
    Dim m_nvcImageMinMax As New NameValueCollection
    Dim m_nvcImageHover As New NameValueCollection
    Dim m_nvcColumnDefaultValue As New NameValueCollection
    Dim m_nvcaSign As New NameValueCollection
    Dim m_nvcpSign As New NameValueCollection
    Dim m_nvcPercentage As New NameValueCollection
    Dim m_nvcShowSeconds As New NameValueCollection '07/23/15 T3

    Dim m_sessionSelectedId As String = ""
    Dim m_sessionSelectedMenu As String = ""    ' 02/13/15
    Dim m_sessionSelectedList As String = ""
    Dim m_sessiontab As String = ""
    Dim m_sessionrl As String = ""
    Dim m_sessionwsn As String = ""     ' 5/10/16 T3 session where clause
    Dim m_sessionvo As String = ""
    Dim m_sessionadd As String = ""
    Dim m_sessionsrt As String = "" 'index of column we are sorting on 
    Dim m_sessionseq As String = "" ' asc or desc
    Dim m_sessionDivHide As String = ""
    Dim m_sessionUsr As String = ""
    Dim m_sessionSearchFilter As String = "" '04/30/15 T3
    Dim m_sessionPerm As String = "" '06/04/15 T3
    Dim m_nvcUrlParams As New NameValueCollection ' 6/9/15 T3

    '02/16/16 t3 forces FLM to retrieve all records, and will export the table contents to frmExportFLM (at end of sub LoadListTableToGrid)
    Dim m_blnExportMode As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.ExpiresAbsolute = DateTime.Now.AddDays(-1D)
        Response.Expires = -1500
        Response.CacheControl = "no-cache"

        'verify user logged on
        If IsNothing(Session("user_link_id")) Then
            Session("LoginReturnURL") = Request.Url.AbsoluteUri
            Session("MessageTitle") = "Session Expired"
            Session("RelayMessage") = "<h3>Your session has expired. Please login to continue.</h3> <br /><br /> " &
                "<a href=""" & g_loginPage & """ target=""_self"" class=""btn btn-primary"">Login</a> "
            Session("PageRelay") = ""
            Response.Redirect("frmMessaging.aspx")
        End If

        ' 02/16/16 t3 check to see if we are in an export tab
        If IsNothing(Request.QueryString("expMode")) Then
        Else
            m_blnExportMode = Request.QueryString("expMode")
        End If
        If m_blnExportMode Then
            ' force all records, get select from session and extract table name from sesseion
            ' set page type based on export type, default to excel
            'Dim strContentType As String = "ms-excel"
            'Dim strFileName As String = "Export.xls"
            'If IsNothing(Request.QueryString("contentType")) Then
            'Else
            '    strContentType = Request.QueryString("contentType")
            'End If
            'If IsNothing(Request.QueryString("fileName")) Then
            'Else
            '    strFileName = Request.QueryString("fileName")
            'End If
            'Response.ContentType = "application/vnd." & strContentType
            'Response.AddHeader("Content-Disposition", "attachment;filename=" & strFileName)

            ' get previous select to get table name
            Dim strSession As String = Request.QueryString("sesPrefix") & "FLM_SQL"
            Dim strSessionSQL As String = Session(strSession)
            Dim strTableName As String = Split(Split(LCase(strSessionSQL), " from ")(1), " ")(0)

        End If


        ' default tabs on -- if table sent via 'id' then not using tabs
        Dim blnShowTabs As Boolean = False
        Dim strTableSelected As String = ""
        Dim strSelectedMenu As String = "" ' 02/13/15
        Dim blnFilterByUser As Boolean = False

        If IsNothing(Session(hidSessionsPrefix.Value & "litScripts")) Then
        Else
            litScripts.Text = Session(hidSessionsPrefix.Value & "litScripts")
        End If

        ' -----
        '  selected data table may be sent directly into the form via 'id' or 
        '  it may be retreived from the generated tab list
        ' ----
        '--7/9/14 rlo

        '6/9/15 T3
        Session("frmUrl") = Request.RawUrl

        Dim strUrl As String = Request.RawUrl.Replace("/frmListManager.aspx", "")
        If Left(strUrl, 3) = "?E=" Then
            'Call decrypt
            'Dim strNewUrl As String = "frmListManager.aspx?" & g_Decrypt(Mid(strUrl, 4))
            'Response.Redirect(strNewUrl)
            ' Loop through url string and set parameters/session variables
            Dim arrUrl() = Split(g_Decrypt(Mid(strUrl, 4)), "&")

            For Each strParam In arrUrl
                Dim arrParam() As String = Split(strParam, "=")
                m_nvcUrlParams(arrParam(0)) = arrParam(1)
            Next
        Else
            '9/24/15 T3 Need to save URL into nvc 

        End If


        If IsPostBack Then
        Else


            '09/29/15 - add session prefix
            '09/30/15 - add allow to send session prefix for iframe
            'This will allow a form (or iFrame) to supply the unique prefix if you want to, either in the encrypted or with an unencrypted param (sysPrefix).  
            'In the iFrame you will have to generate this prefix on the parent and supply it to the iFrame along with the other parameters.
            If IsNothing(Request.QueryString("sesPrefix")) Then
                If IsNothing(m_nvcUrlParams("sesPrefix")) Then
                    hidSessionsPrefix.Value = Format(Date.Now, "ssffff") ' time in milliseconds 
                Else
                    hidSessionsPrefix.Value = m_nvcUrlParams("sesPrefix") ' sesPrefix was sent in  
                End If
            Else
                hidSessionsPrefix.Value = Request.QueryString("sesPrefix")   ' 9/30/15 if an application needs to supply the prefix value
            End If
            litScripts.Text &= "<script>jQuery(document).ready(function () { setSessPrefix = '" & hidSessionsPrefix.Value & "'})</script>"


            If IsNothing(Request.QueryString("pre")) AndAlso IsNothing(m_nvcUrlParams("pre")) Then
                'If pre is anything, then session variables will not be cleared
                Session.Remove(hidSessionsPrefix.Value & "rl") 'rlo
                Session.Remove(hidSessionsPrefix.Value & "vo") ' rlo
                Session.Remove(hidSessionsPrefix.Value & "perm") ' t3
                Session.Remove(hidSessionsPrefix.Value & "add") ' cp
                Session.Remove(hidSessionsPrefix.Value & "srt") ' cp
                Session.Remove(hidSessionsPrefix.Value & "seq") ' t3
                Session.Remove(hidSessionsPrefix.Value & "divHide") 't3
                Session.Remove(hidSessionsPrefix.Value & "usr") 'cp
                Session.Remove(hidSessionsPrefix.Value & "SelectedMenu") ' t3
                Session.Remove(hidSessionsPrefix.Value & "SelectedId")

                Session.Remove(hidSessionsPrefix.Value & "SelectedList")
                Session.Remove(hidSessionsPrefix.Value & "tab")
                Session.Remove(hidSessionsPrefix.Value & "searchFilter") 't3
                Session.Remove(hidSessionsPrefix.Value & "pageId") 't3
                Session.Remove(hidSessionsPrefix.Value & "itemsPerPage") 't3

                For i = 1 To 5
                    If IsNothing(Session(hidSessionsPrefix.Value & "SearchText" & i)) Then
                    Else
                        Session.Remove(hidSessionsPrefix.Value & "SearchAndOr" & i)
                        Session.Remove(hidSessionsPrefix.Value & "SearchField" & i)
                        Session.Remove(hidSessionsPrefix.Value & "SearchLogic" & i)
                        Session.Remove(hidSessionsPrefix.Value & "SearchText" & i)
                    End If
                Next
            End If
        End If

        ' 5/10/16 T3 - check for where clase session variable
        If IsNothing(m_nvcUrlParams("wsn")) Then
            Dim strSessionVariableNameContainingWhere As String = Request.QueryString("wsn")
            If IsNothing(Request.QueryString("wsn")) Then
                Session.Remove(strSessionVariableNameContainingWhere)
            Else
                Session(hidSessionsPrefix.Value & "wsn") = Session(strSessionVariableNameContainingWhere)
            End If
        Else
            Session(hidSessionsPrefix.Value & "wsn") = Request.QueryString("wsn")
        End If
        ' rlo
        ' is there a list of recid's supplied?
        If IsNothing(m_nvcUrlParams("rl")) Then
            If IsNothing(Request.QueryString("rl")) Then
                ' RLO 02/05/2015- if there is a possibility of a list being too long for a URL transmit put it in a session variable and use the param "seslst" to 
                ' send the session varible name with the list
                'not by "rl" but is there a session varible provided with the list?
                If IsNothing(m_nvcUrlParams("seslst")) Then
                    If IsNothing(Request.QueryString("seslst")) Then
                    Else
                        Dim strSessionVariableNameContainingList As String = Request.QueryString("seslst")
                        Session(hidSessionsPrefix.Value & "rl") = Session(strSessionVariableNameContainingList)
                        ' 4/19/16 T3 do not remove embedded FLM session variable that contains a record id list
                        If IsNothing(m_nvcUrlParams("eFLM")) Then
                            If IsNothing(Request.QueryString("eFLM")) Then
                                Session.Remove(strSessionVariableNameContainingList)
                            Else
                                If Request.QueryString("eFLM") = "1" Then
                                Else
                                    Session.Remove(strSessionVariableNameContainingList)
                                End If
                            End If
                        Else
                            If m_nvcUrlParams("eFLM") = "1" Then
                            Else
                                Session.Remove(strSessionVariableNameContainingList)
                            End If
                        End If
                    End If
                Else
                    Dim strSessionVariableNameContainingList As String = m_nvcUrlParams("seslst")
                    Session(hidSessionsPrefix.Value & "rl") = Session(strSessionVariableNameContainingList)
                    ' 4/19/16 T3 do not remove embedded FLM session variable that contains a record id list
                    If IsNothing(m_nvcUrlParams("eFLM")) Then
                        If IsNothing(Request.QueryString("eFLM")) Then
                            Session.Remove(strSessionVariableNameContainingList)
                        Else
                            If Request.QueryString("eFLM") = "1" Then
                            Else
                                Session.Remove(strSessionVariableNameContainingList)
                            End If
                        End If
                    Else
                        If m_nvcUrlParams("eFLM") = "1" Then
                        Else
                            Session.Remove(strSessionVariableNameContainingList)
                        End If
                    End If

                    'Session.Remove(strSessionVariableNameContainingList)
                End If
            Else
                Session(hidSessionsPrefix.Value & "rl") = Request.QueryString("rl")
            End If
        Else
            Session(hidSessionsPrefix.Value & "rl") = m_nvcUrlParams("rl")
        End If


        ' show the view option only
        If IsNothing(m_nvcUrlParams("vo")) Then
            If IsNothing(Request.QueryString("vo")) Then
            Else
                Session(hidSessionsPrefix.Value & "vo") = Request.QueryString("vo")
            End If
        Else
            Session(hidSessionsPrefix.Value & "vo") = m_nvcUrlParams("vo")
        End If

        ' permissions 06/04/15 T3
        If IsNothing(m_nvcUrlParams("perm")) And IsNothing(Request.QueryString("perm")) And IsNothing(Session(hidSessionsPrefix.Value & "perm")) Then
            Session(hidSessionsPrefix.Value & "perm") = "1111"
        Else
            If IsNothing(m_nvcUrlParams("perm")) Then
                If IsNothing(Request.QueryString("perm")) Then
                Else
                    Session(hidSessionsPrefix.Value & "perm") = Request.QueryString("perm")
                End If
            Else
                Session(hidSessionsPrefix.Value & "perm") = m_nvcUrlParams("perm")
            End If
        End If

        'cp Pagination Add Button Redirect to Another Page, Defaults to popup
        If IsNothing(m_nvcUrlParams("add")) Then
            If IsNothing(Request.QueryString("add")) Then
            Else
                Session(hidSessionsPrefix.Value & "add") = Request.QueryString("add")
            End If
        Else
            Session(hidSessionsPrefix.Value & "add") = m_nvcUrlParams("add")
        End If

        'cp Session to store the index of the column being sorted 
        If IsNothing(m_nvcUrlParams("srt")) Then
            If IsNothing(Request.QueryString("srt")) Then
            Else
                Session(hidSessionsPrefix.Value & "srt") = Request.QueryString("srt")
            End If
        Else
            Session(hidSessionsPrefix.Value & "srt") = m_nvcUrlParams("srt")
        End If

        If IsNothing(m_nvcUrlParams("seq")) Then
            If IsNothing(Request.QueryString("seq")) Then
            Else
                Session(hidSessionsPrefix.Value & "seq") = Request.QueryString("seq")
            End If
        Else
            Session(hidSessionsPrefix.Value & "seq") = m_nvcUrlParams("seq")
        End If

        If IsNothing(m_nvcUrlParams("divHide")) Then
            If IsNothing(Request.QueryString("divHide")) Then
            Else
                Session(hidSessionsPrefix.Value & "divHide") = Request.QueryString("divHide")
            End If
        Else
            Session(hidSessionsPrefix.Value & "divHide") = m_nvcUrlParams("divHide")
        End If

        If IsNothing(m_nvcUrlParams("usr")) Then
            If IsNothing(Request.QueryString("usr")) Then
            Else
                Session(hidSessionsPrefix.Value & "usr") = Request.QueryString("usr")
            End If
        Else
            Session(hidSessionsPrefix.Value & "usr") = m_nvcUrlParams("usr")
        End If

        If IsNothing(m_nvcUrlParams("pageId")) Then
            If IsNothing(Request.QueryString("pageId")) Then
            Else
                Session(hidSessionsPrefix.Value & "pageId") = Request.QueryString("pageId")
            End If
        Else
            Session(hidSessionsPrefix.Value & "pageId") = m_nvcUrlParams("pageId")
        End If

        If IsNothing(m_nvcUrlParams("itemsPerPage")) Then
            If IsNothing(Request.QueryString("itemsPerPage")) Then
            Else
                Session(hidSessionsPrefix.Value & "itemsPerPage") = Request.QueryString("itemsPerPage")
            End If
        Else
            Session(hidSessionsPrefix.Value & "itemsPerPage") = m_nvcUrlParams("itemsPerPage")
        End If

        ' if feeding search criteria through sessions then load values into textbox/ddl/etc
        If IsNothing(m_nvcUrlParams("scriteria")) Then
        Else
            For i = 1 To 5

            Next

        End If

        'Resetting modular variables from save sessions
        If IsNothing(Session(hidSessionsPrefix.Value & "SelectedID")) Then
        Else
            m_sessionSelectedId = Session(hidSessionsPrefix.Value & "SelectedId")
        End If
        ' 02/13/15
        If IsNothing(Session(hidSessionsPrefix.Value & "SelectedMenu")) Then '###
        Else
            m_sessionSelectedMenu = Session(hidSessionsPrefix.Value & "SelectedMenu")
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "SelectedList")) Then
        Else
            m_sessionSelectedList = Session(hidSessionsPrefix.Value & "SelectedList")
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "tab")) Then
        Else
            m_sessiontab = Session(hidSessionsPrefix.Value & "tab")
        End If

        If IsNothing(Session(hidSessionsPrefix.Value & "rl")) Then
        Else
            m_sessionrl = Session(hidSessionsPrefix.Value & "rl") ' rlo
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "vo")) Then
        Else
            m_sessionvo = Session(hidSessionsPrefix.Value & "vo") ' rlo
        End If

        ' 5/10/16 T3 handle session where clause
        If IsNothing(Session(hidSessionsPrefix.Value & "wsn")) Then
        Else
            m_sessionwsn = Session(hidSessionsPrefix.Value & "wsn")
        End If

        If m_sessionvo = "" Then
        Else
            m_sessionPerm = "1000"
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('linkAdd').style.display='none';});</script>"
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('btnAddSmallVw').style.display='none';});</script>"
        End If

        If IsNothing(Session(hidSessionsPrefix.Value & "perm")) Then
        Else
            m_sessionPerm = Session(hidSessionsPrefix.Value & "perm") ' t3
            ' 02/16/16 t3 check to see if we are in export mode, if so remove all permissions
            If m_blnExportMode Then
                m_sessionPerm = "0000"
            End If
            If Mid(m_sessionPerm, 4, 1) = "1" Then
            Else
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('linkAdd').style.display='none';});</script>"
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('btnAddSmallVw').style.display='none';});</script>"
            End If
            If Mid(m_sessionPerm, 5, 1) = "" Then
            Else
                If Mid(m_sessionPerm, 5, 1) = "1" Then
                Else
                    litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('linkSearch').style.display='none';});</script>"
                    litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('btnSearchLink').style.display='none';});</script>"
                End If
            End If
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "add")) Then
        Else
            m_sessionadd = Session(hidSessionsPrefix.Value & "add") ' cp
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "srt")) Then
        Else
            m_sessionsrt = Session(hidSessionsPrefix.Value & "srt") ' cp
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "seq")) Then
        Else
            m_sessionseq = Session(hidSessionsPrefix.Value & "seq") ' t3
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "divHide")) Then
        Else
            m_sessionDivHide = Session(hidSessionsPrefix.Value & "divHide") ' t3
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "usr")) Then
        Else
            m_sessionUsr = Session(hidSessionsPrefix.Value & "usr") ' cp
            blnFilterByUser = True
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "pageId")) Then
        Else
            m_intPageNo = Session(hidSessionsPrefix.Value & "pageId") ' cp
            ' 02/16/16 t3 if in export mode, always start at page 1
            If m_blnExportMode Then
                m_intPageNo = 1
            End If
        End If

        '04/30/15 T3 
        If IsNothing(Session(hidSessionsPrefix.Value & "searchFilter")) Then
        Else
            m_sessionSearchFilter = Session(hidSessionsPrefix.Value & "searchFilter") ' t3
        End If

        Dim strSQL As String = ""
        'strSql = " select * from sys_Security where frmName = 'frmListManager' and userRole = '" & Session("user_type") & "'"
        'Dim tblSysSecurity As DataTable = g_IO_Execute_SQL(strSql, False)

        'If tblSysSecurity.Rows.Count > 0 Then
        '    If tblSysSecurity.Rows(0)("accessLevel") = 1 Then
        '        'Set View Only
        '        Session(hidSessionsPrefix.value & "vo") = "True"
        '        m_sessionvo = Session(hidSessionsPrefix.value & "vo")
        '    End If
        'End If

        If m_sessionDivHide = "" Then
        Else
            Dim strDivHideScript As String = "<script>jQuery(document).ready(function () {"
            For Each strDivHide In m_sessionDivHide.Split(",")
                strDivHideScript &= "document.getElementById(""" & strDivHide & """).style.display = ""none"";"
            Next
            strDivHideScript &= "});</script>"
            litTopScripts.Text &= strDivHideScript
        End If

        ' -check for tab selection (always sent if being used)

        If IsNothing(Request.QueryString("tab")) AndAlso IsNothing(Session(hidSessionsPrefix.Value & "tab")) AndAlso IsNothing(m_nvcUrlParams("tab")) Then
            ' if table id not sent see if stored in session from previous selection
            If IsNothing(Session(hidSessionsPrefix.Value & "SelectedList")) Then
                ' tab not sent nor stored in session, look for specific table id
                Dim strID As String = ""
                If IsNothing(m_nvcUrlParams("id")) Then
                    If IsNothing(Request.QueryString("id")) Then
                    Else
                        strID = Request.QueryString("id")
                    End If
                Else
                    strID = m_nvcUrlParams("id")
                End If

                If strID = "" Then
                    ' if table id not sent see if stored in session from previous selection
                    If IsNothing(Session(hidSessionsPrefix.Value & "SelectedId")) Then
                        ' requested table not known
                        blnShowTabs = True
                    Else
                        ' set table from previous sesssion via id
                        strSelectedMenu = Session(hidSessionsPrefix.Value & "SelectedMenu") ' 02/13/15
                        strTableSelected = Session(hidSessionsPrefix.Value & "SelectedId")
                        hidMenuID.Value = strSelectedMenu  ' 02/13/15  9/24/15 chng'd strTableSelected to strSelectedMenu
                        blnShowTabs = False
                    End If
                Else
                    ' specific table sent, get it and save session
                    strTableSelected = strID
                    blnShowTabs = False
                    ' 02/13/15
                    If IsNothing(m_nvcUrlParams("mid")) Then
                        If IsNothing(Request.QueryString("mid")) Then
                            ' specific table sent, get it and save session
                            strSelectedMenu = strTableSelected
                        Else
                            ' specific table sent, get it and save session
                            strSelectedMenu = Request.QueryString("mid")
                        End If
                    Else
                        strSelectedMenu = m_nvcUrlParams("mid")
                    End If

                    Session(hidSessionsPrefix.Value & "SelectedMenu") = strSelectedMenu
                    Session(hidSessionsPrefix.Value & "SelectedId") = strTableSelected
                    'Created hidden field with the name of the table being sent in so we can change active link on the javascript side
                    'Note-827: The value for the parameter "id" needs to be the same for the list ID and URL parameter id being sent-i.e. <li id="#Contracts" class="active"><a href="frmListManager.aspx?id=Contracts_vw"
                    Session(hidSessionsPrefix.Value & "SelectedId") = strTableSelected
                    hidMenuID.Value = strSelectedMenu ' 02/13/15 9/24/15 chng'd strTableSelected to strSelectedMenu
                End If

            Else
                ' table in tab selection session
                strTableSelected = Session(hidSessionsPrefix.Value & "SelectedList")
                blnShowTabs = True
            End If
        Else
            blnShowTabs = True
            ' tab selected, set table
            ' 7/9/14 rlo
            If IsNothing(m_nvcUrlParams("tab")) Then
                If IsNothing(Request.QueryString("tab")) Then
                    strTableSelected = Session(hidSessionsPrefix.Value & "tab")
                Else
                    strTableSelected = Request.QueryString("tab")
                    Session(hidSessionsPrefix.Value & "tab") = strTableSelected
                End If
            Else
                strTableSelected = m_nvcUrlParams("tab")
                Session(hidSessionsPrefix.Value & "tab") = strTableSelected
            End If
        End If

        If IsNothing(Session(hidSessionsPrefix.Value & "itemsPerPage")) Then
        Else
            m_intItemsPerPage = Session(hidSessionsPrefix.Value & "itemsPerPage")
        End If
        ' 02/16/16 t3 if in export mode, always all pages, setting to zero, should force ttl rows in table
        If m_blnExportMode Then
            m_intItemsPerPage = 0
        End If

        '-- build tabbed navigation
        Session(hidSessionsPrefix.Value & "SelectedList") = "__"
        m_sessionSelectedList = "__"
        Dim strTabList As String = ""
        Dim strNavTabs As String = ""
        Dim strTableDescription As String = ""

        '-- determine if table is tabbed list or sent directly
        If blnShowTabs Then
            'blnShowTabs
            strSql = "Select TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME Like '%list__%' " &
                " AND TABLE_NAME NOT IN (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS) ORDER BY TABLE_NAME"
            '05/13/15 cpb took out to include views. AND TABLE_NAME NOT IN (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS)--

            Dim tblLists As DataTable = g_IO_Execute_SQL(strSql, False)
            If tblLists.Rows.Count = 0 Then
            Else
                tblLists.Columns.Add("Table_Desc", GetType(String))
                Try
                    For Each rowLists As DataRow In tblLists.Rows
                        rowLists("Table_desc") = "&nbsp;&nbsp;" & Split(CStr(rowLists("Table_Name")), "__")(1)
                    Next
                Catch ex As Exception
                End Try

                ' build an li for each list table found in the database
                If tblLists.Rows.Count > 0 Then
                    Dim blnFirstTime As Boolean = True
                    For Each listTable In tblLists.Rows
                        Dim strListTableName = listTable("Table_Name")
                        ' make sure it is an auto tab gen table
                        If strListTableName.contains("__") Then
                            strTabList = "<li"
                            If blnFirstTime = True And strTableSelected = "" Then
                                strTabList &= " class = ""active"""
                                blnFirstTime = False
                                strTableSelected = strListTableName
                            Else
                                If strListTableName = strTableSelected Then
                                    strTabList &= " class = ""active"""
                                End If
                            End If
                            strTabList &= "><a href=""#divGridContainer"" data-toggle=""tab"" onclick=""tabSelect('" &
                                strListTableName & "');"">"
                            Dim strTableDescr As String = g_GetTableDescription(strListTableName, "displayDescription")
                            strTabList &= strTableDescr
                            strTabList &= "</a></li>"
                            ' add this selection to the tab pages
                            strNavTabs &= strTabList
                        End If
                    Next
                End If
            End If

            ' see if we have a table to load
            If strTableSelected = "" Then
                litNavDDL.Text = "<li class=""active""><a href=""Default.aspx"">No items found. Go Back.</a></li>"
                litNavTabs.Text = "<li class=""active""><a href=""Default.aspx"">No items found. Go Back.</a></li>"
                litDDLSelectedList.Text = ""
                litSelectedList.Text = ""
                litSelectedListDisplay.Text = "style=""display:none"""
            Else
                Session(hidSessionsPrefix.Value & "SelectedList") = strTableSelected
                m_sessionSelectedId = strTableSelected
                m_sessionSelectedMenu = strSelectedMenu ' 02/13/15
                m_sessionUsr = blnFilterByUser
                'session.remove(hidSessionsPrefix.value & "SelectedList")
                litNavDDL.Text = strNavTabs
                litNavTabs.Text = strNavTabs
                strTableDescription = g_GetTableDescription(strTableSelected, "displayDescription")
                litDDLSelectedList.Text = strTableDescription
                litSelectedList.Text = ""
                litSelectedListDisplay.Text = "style=""display:none"""
                LoadListTableToGrid(strTableSelected, m_intPageNo, strTableDescription, blnFilterByUser)
            End If
        Else
            ' not showing tabs
            If strTableSelected = "" Then
                litNavDDL.Text = "<li>Unable to Get List Table Request</li>"
                litNavTabs.Text = "<li>Unable to Get List Table Request</li>"
                litDDLSelectedList.Text = ""
                litSelectedList.Text = ""
                litSelectedListDisplay.Text = "style=""display:none"""
            Else
                Session(hidSessionsPrefix.Value & "SelectedId") = strTableSelected
                Session(hidSessionsPrefix.Value & "SelectedMenu") = strSelectedMenu ' 02/13/15
                m_sessionSelectedId = strTableSelected
                m_sessionSelectedMenu = strSelectedMenu ' 02/13/15
                m_sessionUsr = blnFilterByUser
                Session.Remove(hidSessionsPrefix.Value & "SelectedList")
                strTableDescription = g_GetTableDescription(strTableSelected, "displayDescription")
                litNavDDL.Text = "<li id=""NoNavDDL""></li>"
                litNavTabs.Text = ""
                litDDLSelectedList.Text = strTableDescription
                litSelectedList.Text = strTableDescription
                litSelectedListDisplay.Text = ""
                LoadListTableToGrid(strTableSelected, m_intPageNo, strTableDescription, blnFilterByUser)
            End If
        End If
        Dim strTableAddTitle = g_GetTableDescription(strTableSelected, "addDescription")
        Dim strEditForm = m_sessionadd
        If strEditForm = "" Then
            strEditForm = g_GetTableExtProperty(strTableSelected, "editForm")
        End If
        Dim strEditTarget = g_GetTableExtProperty(strTableSelected, "editTarget")
        If strEditTarget = "" Then
            strEditTarget = "_self"
        End If
        'Note-828 : Add Button in Pagination (Pop up Window or send to another page)
        If strEditForm = "" Then
            'Popup window
            'btnAdd.Attributes.Add("onclick", "getDataRow(-1,'" & strTableSelected & "','" & strTableAddTitle & "', 'add', '" & m_sessionPerm.ToString & "'); return false")
            litBtnAdd.Text = "onclick=""getDataRow(-1,'" & strTableSelected.Replace("'", "\'") & "','" & strTableAddTitle & "', 'add', '" & m_sessionPerm.ToString & "'); return false"" "
        Else
            'Send to another form, name of form is sent via url in the parameter "add"
            ' 2/25/16 T3 Changed to literal on btnAdd to use window.open to control the target location from ext properties
            ' window.open known in the past to not work in certain browsers, maybe Safari on Apple devices??
            'btnAdd.Attributes.Add("onclick", "window.location.assign(""" & strEditForm & """); return false;")
            litBtnAdd.Text = "onclick=""window.open('" & strEditForm & "','" & strEditTarget & "'); return false;"" "

        End If
        btnSearch.Attributes.Add("onclick", "showSearch(True); return false")
        btnSearchGo.Attributes.Add("onclick", "jQuery('#loading_indicator').show();setSearchFilterSession(); return false")
        btnSearchClear.Attributes.Add("onclick", "jQuery('#loading_indicator').show();clearSearch(); return false")
        btnClearQuickSearch.Attributes.Add("onclick", "jQuery('#loading_indicator').show();clearSearch(); return false")

    End Sub
    Private Sub LoadListTableToGrid(ByVal TableName As String, ByVal PageToShow As Integer, ByVal TableDescription As String, ByVal FilterByUser As Boolean)

        Dim strSQL As String = "Select * from " & TableName & " where recid= -1"
        Dim tblList = g_IO_Execute_SQL(strSQL, False)

        ' 2/3/15 cpb protect if user gets into this form with invalid table name.
        If IsNothing(tblList) Then
            litCurrentPageInfo.Text = "Sorry, Requested File Not Found"
        Else
            LoadListTableToGrid(TableName, tblList.Columns(1).ColumnName, PageToShow, FilterByUser)
        End If


    End Sub

    Private Sub LoadListTableToGrid(ByVal TableName As String, ByVal OrderBy As String, ByVal PageToShow As Integer, ByVal FilterByUser As Boolean)

        g_GetColumns(TableName, m_nvcColumnLength, m_nvcColumnType, m_nvcColumnDescription, m_nvcColumnDDLTableName, m_nvcColumnDDLValue, m_nvcColumnDDLText,
                     m_nvcColumnPwd, m_nvcColumnShowInGrid, m_nvcColumnShowInPopup, m_nvcHidden, m_nvcDisabled, m_nvcColumnIndex, m_nvcColumnRequired, m_nvcColumnEmail,
                     m_nvcColumnPhone, m_nvcColumnTotal, m_nvcColumnDisplayLocn, m_nvcColumnUnique, m_nvcColumnNames,
                     m_nvcColumnRegExpPattern, m_nvcColumnRegExpMessage, m_nvcMinValue, m_nvcMaxValue, m_nvcColumnDefaultValue,
                     m_nvcaSign, m_nvcpSign, m_nvcPercentage, m_nvcShowSeconds, m_nvcColumnDDLFilter, m_nvcImageMinMax, m_nvcImageHover)

        ' 02/20/15 T3 add ability to specify links to show in the grid.
        ' defined in the extended properties
        '# = i.e (link1,link2,link3) can have unlimited amount of custom links
        '   Text to Display ~~NameOfFunction~~fieldsToSend separated by || (if you want to send a constant put it in single quotes)~~FieldName to follow(blank to show at beginning)
        '   ex: Click Me~~fncName~~recid||'T3'~~fieldName
        Dim strTableLinks As String = g_GetTableExtProperty(TableName, "link")
        If strTableLinks = "" Then
        Else
            Dim intIndex As Integer = 0
            For Each rowLink In Split(strTableLinks, "##")
                ' extract which link we are working with (ie link1, link2, etc.)
                Dim arrCurrLink() As String = Split(rowLink, "++")

                'Breaks up link name and list of parameters
                'Dim arrLinks() As String = Split(arrCurrLink(1), "~~")
                m_nvcColumnlink(arrCurrLink(0)) = arrCurrLink(1)

                'For intCount = 1 To arrLinks.Count - 1
                '    m_nvcColumnlink(arrCurrLink(0)) &= "~~" & arrLinks(intCount)
                'Next
                'Entry 1 is text to display
                'Entry 2 is function
                'Entry 3 is fields/parameters
                'Entry 4 is field name to display after to control location
            Next
        End If
        Dim strEditTitle As String = g_GetTableDescription(TableName, "editDescription").Replace("'", "\'")
        Dim strViewTitle As String = g_GetTableDescription(TableName, "viewDescription").Replace("'", "\'") & " (VIEW ONLY)"  ' 1/9/15 cpb
        Dim strEditForm As String = g_GetTableExtProperty(TableName, "editForm")
        Dim strEditTarget As String = g_GetTableExtProperty(TableName, "editTarget")
        If strEditTarget = "" Then
            strEditTarget = "_self"
        End If
        Dim strEditIframe As String = g_GetTableExtProperty(TableName, "editIframe")
        Dim strDeleteDisplayColumn As String = g_GetTableExtProperty(TableName, "deleteDisplayColumn")
        Dim strDeleteDisplayText As String = g_GetTableExtProperty(TableName, "deleteDisplayText").Replace("'", "\'")

        If m_sessionsrt = "" Then
            m_sessionsrt = g_GetTableExtProperty(TableName, "sortDefaultCol")
            If m_sessionsrt = "" Then
            Else
                m_sessionsrt = m_nvcColumnIndex(m_sessionsrt)
            End If
        End If

        Session(hidSessionsPrefix.Value & "editDescription") = strEditTitle
        Session(hidSessionsPrefix.Value & "viewDescription") = strViewTitle   '1/9/15 cpb
        Session(hidSessionsPrefix.Value & "editForm") = strEditForm
        Session(hidSessionsPrefix.Value & "editTarget") = strEditTarget
        Session(hidSessionsPrefix.Value & "editIframe") = strEditIframe
        Session(hidSessionsPrefix.Value & "deleteDescription") = strDeleteDisplayColumn
        Session(hidSessionsPrefix.Value & "deleteDisplayText") = strDeleteDisplayText

        ' 7/7/15 T3 STOPPED HERE if items.count = 0, build them, otherwise, skip to line 848... need to test
        'If ddlColumnAdvSearch1.Items.Count = 0 Then

        ddlColumnAdvSearch1.Items.Clear()
        ddlColumnAdvSearch2.Items.Clear()
        ddlColumnAdvSearch3.Items.Clear()
        ddlColumnAdvSearch4.Items.Clear()
        ddlColumnAdvSearch5.Items.Clear()
        ddlColumnAdvSearch1.Items.Add("Any Field")
        ddlColumnAdvSearch2.Items.Add("Any Field")
        ddlColumnAdvSearch3.Items.Add("Any Field")
        ddlColumnAdvSearch4.Items.Add("Any Field")
        ddlColumnAdvSearch5.Items.Add("Any Field")

        Dim rowIndex As Integer = 1 'starts at 1 since we have any field at position 0
        For Each colColumn In m_nvcColumnNames
            If ",RECID,TDS,".Contains("," & UCase(colColumn) & ",") Then
            Else
                ' load ddl for advanced searching section
                ' 2/2/15 use column description if available
                If IsNothing(m_nvcColumnDescription(colColumn)) Then
                    ddlColumnAdvSearch1.Items.Add(colColumn)
                    ddlColumnAdvSearch1.Items(rowIndex).Value = colColumn
                Else
                    ddlColumnAdvSearch1.Items.Add(m_nvcColumnDescription(colColumn))
                    ddlColumnAdvSearch1.Items(rowIndex).Value = colColumn
                End If
                If IsNothing(m_nvcColumnDescription(colColumn)) Then
                    ddlColumnAdvSearch2.Items.Add(colColumn)
                    ddlColumnAdvSearch2.Items(rowIndex).Value = colColumn
                Else
                    ddlColumnAdvSearch2.Items.Add(m_nvcColumnDescription(colColumn))
                    ddlColumnAdvSearch2.Items(rowIndex).Value = colColumn
                End If
                If IsNothing(m_nvcColumnDescription(colColumn)) Then
                    ddlColumnAdvSearch3.Items.Add(colColumn)
                    ddlColumnAdvSearch3.Items(rowIndex).Value = colColumn
                Else
                    ddlColumnAdvSearch3.Items.Add(m_nvcColumnDescription(colColumn))
                    ddlColumnAdvSearch3.Items(rowIndex).Value = colColumn
                End If
                If IsNothing(m_nvcColumnDescription(colColumn)) Then
                    ddlColumnAdvSearch4.Items.Add(colColumn)
                    ddlColumnAdvSearch4.Items(rowIndex).Value = colColumn
                Else
                    ddlColumnAdvSearch4.Items.Add(m_nvcColumnDescription(colColumn))
                    ddlColumnAdvSearch4.Items(rowIndex).Value = colColumn
                End If
                If IsNothing(m_nvcColumnDescription(colColumn)) Then
                    ddlColumnAdvSearch5.Items.Add(colColumn)
                    ddlColumnAdvSearch5.Items(rowIndex).Value = colColumn
                Else
                    ddlColumnAdvSearch5.Items.Add(m_nvcColumnDescription(colColumn))
                    ddlColumnAdvSearch5.Items(rowIndex).Value = colColumn
                End If
            End If
            rowIndex += 1
        Next

        ' Load ddl's for advanced searching section

        ' Note: ddlColumnId is loaded w/in LoadListTableToGrid while scanning through column names
        ddlAndOrAdvSearch2.Items.Clear()
        ddlAndOrAdvSearch3.Items.Clear()
        ddlAndOrAdvSearch4.Items.Clear()
        ddlAndOrAdvSearch5.Items.Clear()
        ddlAndOrAdvSearch2.Items.Add("And")
        ddlAndOrAdvSearch2.Items.Add("Or")
        ddlAndOrAdvSearch3.Items.Add("And")
        ddlAndOrAdvSearch3.Items.Add("Or")
        ddlAndOrAdvSearch4.Items.Add("And")
        ddlAndOrAdvSearch4.Items.Add("Or")
        ddlAndOrAdvSearch5.Items.Add("And")
        ddlAndOrAdvSearch5.Items.Add("Or")

        ddlLogicAdvSearch1.Items.Clear()
        ddlLogicAdvSearch2.Items.Clear()
        ddlLogicAdvSearch3.Items.Clear()
        ddlLogicAdvSearch4.Items.Clear()
        ddlLogicAdvSearch5.Items.Clear()
        ddlLogicAdvSearch1.Items.Add("Contains")
        ddlLogicAdvSearch1.Items.Add("=")
        ddlLogicAdvSearch1.Items.Add("<")
        ddlLogicAdvSearch1.Items.Add("<=")
        ddlLogicAdvSearch1.Items.Add(">")
        ddlLogicAdvSearch1.Items.Add(">=")
        ddlLogicAdvSearch1.Items.Add("Not Equal")
        ddlLogicAdvSearch1.Items.Add("Starting With")

        ddlLogicAdvSearch2.Items.Add("Contains")
        ddlLogicAdvSearch2.Items.Add("=")
        ddlLogicAdvSearch2.Items.Add("<")
        ddlLogicAdvSearch2.Items.Add("<=")
        ddlLogicAdvSearch2.Items.Add(">")
        ddlLogicAdvSearch2.Items.Add(">=")
        ddlLogicAdvSearch2.Items.Add("Not Equal")
        ddlLogicAdvSearch2.Items.Add("Starting With")

        ddlLogicAdvSearch3.Items.Add("Contains")
        ddlLogicAdvSearch3.Items.Add("=")
        ddlLogicAdvSearch3.Items.Add("<")
        ddlLogicAdvSearch3.Items.Add("<=")
        ddlLogicAdvSearch3.Items.Add(">")
        ddlLogicAdvSearch3.Items.Add(">=")
        ddlLogicAdvSearch3.Items.Add("Not Equal")
        ddlLogicAdvSearch3.Items.Add("Starting With")

        ddlLogicAdvSearch4.Items.Add("Contains")
        ddlLogicAdvSearch4.Items.Add("=")
        ddlLogicAdvSearch4.Items.Add("<")
        ddlLogicAdvSearch4.Items.Add("<=")
        ddlLogicAdvSearch4.Items.Add(">")
        ddlLogicAdvSearch4.Items.Add(">=")
        ddlLogicAdvSearch4.Items.Add("Not Equal")
        ddlLogicAdvSearch4.Items.Add("Starting With")

        ddlLogicAdvSearch5.Items.Add("Contains")
        ddlLogicAdvSearch5.Items.Add("=")
        ddlLogicAdvSearch5.Items.Add("<")
        ddlLogicAdvSearch5.Items.Add("<=")
        ddlLogicAdvSearch5.Items.Add(">")
        ddlLogicAdvSearch5.Items.Add(">=")
        ddlLogicAdvSearch5.Items.Add("Not Equal")
        ddlLogicAdvSearch5.Items.Add("Starting With")

        ' 10/12/15 CS Clear textboxes to repopulate from sessions variables
        txtColumnAdvSearch1.Text = ""
        txtColumnAdvSearch2.Text = ""
        txtColumnAdvSearch3.Text = ""
        txtColumnAdvSearch4.Text = ""
        txtColumnAdvSearch5.Text = ""
        txtQuickSearch.Text = ""


        'End If

        'Dim strSCriteria As String = ""
        'If IsNothing(m_nvcUrlParams("scriteria")) Then
        'Else
        '    strSCriteria = m_nvcUrlParams("scriteria")
        'End If
        'If IsNothing(Request.QueryString("scriteria")) Then
        'Else
        '    strSCriteria = Request.QueryString("scriteria")
        'End If
        'If strSCriteria = "" Then
        'Else
        Dim intSearchActive As Integer = 0
        If IsNothing(Session(hidSessionsPrefix.Value & "SearchText1")) Then
        Else
            ddlColumnAdvSearch1.Text = Session(hidSessionsPrefix.Value & "SearchField1")
            ddlLogicAdvSearch1.Text = Session(hidSessionsPrefix.Value & "SearchLogic1")
            txtColumnAdvSearch1.Text = Session(hidSessionsPrefix.Value & "SearchText1")
            txtQuickSearch.Text = Session(hidSessionsPrefix.Value & "SearchText1")
            If Trim(Session(hidSessionsPrefix.Value & "SearchText1")) = "" Then
            Else
                intSearchActive = 1
            End If
        End If

            If IsNothing(Session(hidSessionsPrefix.Value & "SearchText2")) Then
            Else
                ddlAndOrAdvSearch2.Text = Session(hidSessionsPrefix.Value & "SearchAndOr2")
                ddlColumnAdvSearch2.Text = Session(hidSessionsPrefix.Value & "SearchField2")
                ddlLogicAdvSearch2.Text = Session(hidSessionsPrefix.Value & "SearchLogic2")
            txtColumnAdvSearch2.Text = Session(hidSessionsPrefix.Value & "SearchText2")
            If Trim(Session(hidSessionsPrefix.Value & "SearchText2")) = "" Then
            Else
                intSearchActive += 1
            End If

        End If
            If IsNothing(Session(hidSessionsPrefix.Value & "SearchText3")) Then
            Else
                ddlAndOrAdvSearch3.Text = Session(hidSessionsPrefix.Value & "SearchAndOr3")
                ddlColumnAdvSearch3.Text = Session(hidSessionsPrefix.Value & "SearchField3")
                ddlLogicAdvSearch3.Text = Session(hidSessionsPrefix.Value & "SearchLogic3")
                txtColumnAdvSearch3.Text = Session(hidSessionsPrefix.Value & "SearchText3")
            If Trim(Session(hidSessionsPrefix.Value & "SearchText3")) = "" Then
            Else
                intSearchActive += 1
            End If
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "SearchText4")) Then
            Else
                ddlAndOrAdvSearch4.Text = Session(hidSessionsPrefix.Value & "SearchAndOr4")
                ddlColumnAdvSearch4.Text = Session(hidSessionsPrefix.Value & "SearchField4")
                ddlLogicAdvSearch4.Text = Session(hidSessionsPrefix.Value & "SearchLogic4")
                txtColumnAdvSearch4.Text = Session(hidSessionsPrefix.Value & "SearchText4")
            If Trim(Session(hidSessionsPrefix.Value & "SearchText4")) = "" Then
            Else
                intSearchActive += 1
            End If
        End If
        If IsNothing(Session(hidSessionsPrefix.Value & "SearchText5")) Then
            Else
                ddlAndOrAdvSearch5.Text = Session(hidSessionsPrefix.Value & "SearchAndOr5")
                ddlColumnAdvSearch5.Text = Session(hidSessionsPrefix.Value & "SearchField5")
                ddlLogicAdvSearch5.Text = Session(hidSessionsPrefix.Value & "SearchLogic5")
                txtColumnAdvSearch5.Text = Session(hidSessionsPrefix.Value & "SearchText5")
            If Trim(Session(hidSessionsPrefix.Value & "SearchText5")) = "" Then
            Else
                intSearchActive += 1
            End If
        End If

        'End If

        If intSearchActive > 1 Then
            txtQuickSearch.Style.Add("display", "none")
            btnClearQuickSearch.Style.Add("display", "block")
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(jQuery('#divQuickSearchAddon').addClass('hidden'))</script>"
        Else
            txtQuickSearch.Style.Add("display", "block")
            btnClearQuickSearch.Style.Add("display", "none")
        End If


        '04/30/15 T3 Populate Search Filter Criteria
        If m_sessionSearchFilter = "" Then
        Else
            Dim arrSearchSections() As String = Split(m_sessionSearchFilter, "~~")
            Dim arrSearchColumnSession() As String = Split(arrSearchSections(0), "||")
            Dim arrSearchAndOrSession() As String = Split(arrSearchSections(1), "||")
            Dim arrSearchLogicSession() As String = Split(arrSearchSections(2), "||")
            Dim arrSearchBoxSession() As String = Split(arrSearchSections(3), "||")
            For searchIndex = 0 To 4
                If searchIndex > arrSearchBoxSession.Length - 1 Then
                Else
                    Select Case searchIndex
                        Case 0
                            ddlColumnAdvSearch1.Text = arrSearchColumnSession(searchIndex)
                            ddlLogicAdvSearch1.Text = arrSearchLogicSession(searchIndex)
                            txtColumnAdvSearch1.Text = arrSearchBoxSession(searchIndex)
                        Case 1
                            ddlColumnAdvSearch2.Text = arrSearchColumnSession(searchIndex)
                            ddlAndOrAdvSearch2.Text = arrSearchAndOrSession(searchIndex)
                            ddlLogicAdvSearch2.Text = arrSearchLogicSession(searchIndex)
                            txtColumnAdvSearch2.Text = arrSearchBoxSession(searchIndex)
                        Case 2
                            ddlColumnAdvSearch3.Text = arrSearchColumnSession(searchIndex)
                            ddlAndOrAdvSearch3.Text = arrSearchAndOrSession(searchIndex)
                            ddlLogicAdvSearch3.Text = arrSearchLogicSession(searchIndex)
                            txtColumnAdvSearch3.Text = arrSearchBoxSession(searchIndex)
                        Case 3
                            ddlColumnAdvSearch4.Text = arrSearchColumnSession(searchIndex)
                            ddlAndOrAdvSearch4.Text = arrSearchAndOrSession(searchIndex)
                            ddlLogicAdvSearch4.Text = arrSearchLogicSession(searchIndex)
                            txtColumnAdvSearch4.Text = arrSearchBoxSession(searchIndex)
                        Case 4
                            ddlColumnAdvSearch5.Text = arrSearchColumnSession(searchIndex)
                            ddlAndOrAdvSearch5.Text = arrSearchAndOrSession(searchIndex)
                            ddlLogicAdvSearch5.Text = arrSearchLogicSession(searchIndex)
                            txtColumnAdvSearch5.Text = arrSearchBoxSession(searchIndex)
                    End Select
                End If
            Next
        End If



        Dim strWhere As String = ""
        Dim strWhereDelim As String = " where "
        'Building an array for search options
        Dim ddlSearchColumn() As DropDownList = {ddlColumnAdvSearch1, ddlColumnAdvSearch2, ddlColumnAdvSearch3, ddlColumnAdvSearch4, ddlColumnAdvSearch5}
        Dim ddlSearchAndOr() As DropDownList = {New DropDownList, ddlAndOrAdvSearch2, ddlAndOrAdvSearch3, ddlAndOrAdvSearch4, ddlAndOrAdvSearch5}
        Dim ddlSearchLogic() As DropDownList = {ddlLogicAdvSearch1, ddlLogicAdvSearch2, ddlLogicAdvSearch3, ddlLogicAdvSearch4, ddlLogicAdvSearch5}
        Dim txtSearchBox() As TextBox = {txtColumnAdvSearch1, txtColumnAdvSearch2, txtColumnAdvSearch3, txtColumnAdvSearch4, txtColumnAdvSearch5}

        Dim strSearchColumnSession As String = ""
        Dim strSearchAndOrSession As String = ""
        Dim strSearchLogicSession As String = ""
        Dim strSearchBoxSession As String = ""
        Dim strSessionDelim As String = ""
        Dim blnSearchFilter As Boolean = False
        Dim intcounter As Integer = 0
        Dim blnLogicStart As Boolean = False
        For Each txtBox In txtSearchBox
            '04/30/15 T3 Build string to store search criteria to session variable
            strSearchColumnSession &= strSessionDelim & ddlSearchColumn(intcounter).Text
            strSearchAndOrSession &= strSessionDelim & ddlSearchAndOr(intcounter).Text
            strSearchLogicSession &= strSessionDelim & ddlSearchLogic(intcounter).Text
            strSearchBoxSession &= strSessionDelim & txtBox.Text
            strSessionDelim = "||"

            Dim strSearchBox As String = txtBox.Text

            Dim strSearchColumn As String = ""
            Dim strSearchAndOr As String = ""
            Dim strSearchLogic As String = ""

            If strSearchBox <> "" Then
                blnSearchFilter = True
                strSearchColumn = ddlSearchColumn(intcounter).Text

                If blnLogicStart = False Then
                    blnLogicStart = True
                Else
                    strSearchAndOr = " " & ddlSearchAndOr(intcounter).Text & " "
                End If
                strSearchLogic = " " & ddlSearchLogic(intcounter).Text & " "

                'Did they select any field?
                '("Contains")
                '("=")
                '("<")
                '("<=")
                '(">")
                '(">=")
                '("Not Equal")
                '("Starting With")
                Select Case Trim(UCase(strSearchLogic))
                    Case "CONTAINS"
                        strSearchBox = "%" & strSearchBox & "%"
                        strSearchLogic = " LIKE "
                    Case "="
                        strSearchLogic = " = "
                    Case "<"
                        strSearchLogic = " < "
                    Case "<="
                        strSearchLogic = " <= "
                    Case ">"
                        strSearchLogic = " > "
                    Case ">="
                        strSearchLogic = " >= "
                    Case "NOT EQUAL"
                        strSearchLogic = " <> "
                    Case "STARTING WITH"
                        strSearchBox &= "%"
                        strSearchLogic = " LIKE "
                End Select

                If UCase(strSearchColumn) = "ANY FIELD" Then
                    Dim strRecids As String = BuildSearchRecidsList(TableName, strSearchBox, strSearchLogic)
                    If strRecids = "" Then
                        strWhere &= strWhereDelim & strSearchAndOr & " recid = -1 "
                    Else
                        strWhere &= strWhereDelim & strSearchAndOr & " recid in (" & strRecids & ")"
                    End If
                Else
                    strWhere &= strWhereDelim & strSearchAndOr & strSearchColumn & strSearchLogic & "'" & strSearchBox & "'"
                End If

                strWhereDelim = ""

            End If
            intcounter += 1
        Next

        'Store search criteria in session
        If blnSearchFilter Then
            Session(hidSessionsPrefix.Value & "searchFilter") = strSearchColumnSession & "~~" & strSearchAndOrSession & "~~" & strSearchLogicSession & "~~" & strSearchBoxSession
        Else
            Session.Remove(hidSessionsPrefix.Value & "searchFilter")
        End If

        ' CFS 09/23/14 Added label to display search criteria now that we are toggling the search area to gain space on form
        'If Trim(strWhere) = "" Then
        '    litSearchFilter.Text = ""
        'Else
        '    litSearchFilter.Text = "Records filtered per search criteria." &
        '        "&nbsp;&nbsp;<a href=""#"" onclick=""clearSearch();"">Clear Search</a>"
        'End If

        ' rlo
        ' handle any master selection items
        If m_sessionrl = "" Then
        Else
            If strWhere = "" Then
                strWhere = " where recid in (" & m_sessionrl & ")"
            Else
                ' replace exisiing where completely enclosed with (), then add on the recids sent in thru url
                strWhere = strWhere.Replace("where", "where (") & ") and recid in (" & m_sessionrl & ")"
            End If
        End If
        ' 5/10/16 T3 handle session where clause
        If m_sessionwsn = "" Then
        Else
            If strWhere = "" Then
                strWhere = " where " & m_sessionwsn
            Else
                strWhere = strWhere.Replace("where", "where (") & ") and " & m_sessionwsn & ")"
            End If
        End If

        If Not IsNumeric(m_sessionsrt) Then
            ' set sort to the first visible column or column specified via url
            Dim intColIndex As Integer = 1
            For Each colKeys In m_nvcColumnIndex.Keys
                If UCase(colKeys) = "RECID" Then
                Else
                    If m_sessionsrt = "" Then
                        If IsNothing(m_nvcColumnShowInGrid(colKeys)) And IsNothing(m_nvcHidden(colKeys)) Then
                        Else
                            m_sessionsrt = intColIndex
                            Exit For
                        End If
                    Else
                        ' a column name was sent in via url srt=ColName
                        If colKeys = m_sessionsrt Then
                            m_sessionsrt = intColIndex
                            Exit For
                        End If
                    End If

                    intColIndex += 1
                End If
            Next
        End If
        For Each colKeys In m_nvcColumnIndex.Keys
            If m_nvcColumnIndex(colKeys) = m_sessionsrt Then
                OrderBy = colKeys
                Exit For
            End If
        Next

        Dim seqOrder As String = "asc"
        If m_sessionseq = "" Then
            m_sessionseq = g_GetTableExtProperty(TableName, "sortDefaultSeq")
            If m_sessionseq = "" Then
            Else
                seqOrder = m_sessionseq
            End If
        Else
            If m_sessionseq = "d" Then
                seqOrder = "desc"
            End If
        End If

        Dim strSQL As String = "Select * from " & TableName & IIf(FilterByUser, " where Sys_Users_RECID = '" & Session("user_link_id") & "'", "")
        If strWhere = "" Then
        Else
            strSQL &= strWhere
        End If
        strSQL &= " order by " & OrderBy & " " & seqOrder

        LoadPagination(TableName, strWhere)

        'Store the sql search 
        Session("LM_SQL") = strSQL

        Session(hidSessionsPrefix.Value & "FLM_SQL") = strSQL

        Dim tblList = g_IO_ReadPageOfRecords(strSQL, (PageToShow - 1) * m_intItemsPerPage, m_intItemsPerPage, False)

        '07/21/15 T3
        m_sessionseq = LCase(Left(m_sessionseq, 1))

        ' 05/13/15 cpb move to after build of edit/view/delete in case some fields referenced in these options, just before display of columns
        'For Each strColumn As String In m_nvcColumnShowInGrid.Keys
        '    If UCase(m_nvcColumnShowInGrid(strColumn)) = "FALSE" Then
        '        tblList.Columns.Remove(strColumn)
        '    End If
        'Next
        'For Each strColumn As String In m_nvcHidden.Keys
        '    If UCase(m_nvcHidden(strColumn)) = "TRUE" Then
        '        tblList.Columns.Remove(strColumn)
        '    End If
        'Next
        tblList.Columns.Add("TDs", GetType(String))

        Dim strTDs As String = ""
        Dim strOnclick As String = ""
        Dim strOnclickView As String = ""
        Dim strOnclickEdit As String = ""
        Dim strOnclickDelete As String = ""
        Dim intLinkTDHeader As Integer = 0              ' 2/23/15 T3 number of header links at front of gridrow
        Dim nvcLinkTDs As New NameValueCollection       ' 02/20/15 T3 - show links
        Dim nvcLinkTDHeaders As New NameValueCollection '02/20/15 T3 - links ability to have column header
        Dim nvcLinkTDHeadersFront As New NameValueCollection  '02/23/15 T3 - Headers for links shown at front of grid
        Dim nvcColumnStyle As New NameValueCollection

        If tblList.Rows.Count = 0 Then
            Dim row As DataRow = tblList.NewRow
            row("TDs") = "<td colspan=""" & tblList.Columns.Count & """ style=""text-align:center; font-size:12px;"">No results were found.</td>"
            tblList.Rows.Add(row)
            ' 03/07/16 cpb only show add button if user has permissions.
            If Mid(m_sessionPerm, 4, 1) = "0" Then
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('linkAddNoRows').style.display='none';});</script>"
            End If

            'hide pagination and data grid   
            If blnSearchFilter Then
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('divPagingOptions').style.display='none';});</script>"
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('divNoResults').style.display='none';});</script>"
            Else
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('accPagination').style.display='none';});</script>"
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('divTblListHdr').style.display='none';});</script>"
                litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('divGridContainer').style.display='none';});</script>"

            End If
        Else
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('divNoResults').style.display='none';});</script>"
            litScripts.Text &= "<script type=""text/javascript"">jQuery(document).ready(function(){document.getElementById('divPagingOptions').style.display='block';});</script>"

            ' 12/12/14 T3 - add totals to last record in data table
            If m_intPageCount = PageToShow Then
                If m_nvcColumnTotal.Count > 0 Then
                    Dim rowTotals As DataRow = tblList.NewRow
                    If IsNothing(rowTotals("recid")) Then
                    Else
                        rowTotals("recid") = -99
                    End If
                    Dim strFields As String = ""
                    Dim strDelim As String = ""
                    strSQL = Session("LM_SQL").replace("Select * from ", "")
                    strSQL = Split(strSQL, " order by ")(0)
                    For Each strColumn In m_nvcColumnTotal.Keys
                        strFields &= strDelim & "SUM(" & strColumn & ") as " & strColumn
                        strDelim = ", "
                    Next
                    strSQL = "select " & strFields & " FROM " & strSQL
                    Dim tblTotals As DataTable = g_IO_Execute_SQL(strSQL, False)
                    If tblTotals.Rows.Count > 0 Then
                        For Each strColumn In m_nvcColumnTotal.Keys
                            rowTotals(strColumn) = tblTotals.Rows(0)(strColumn)
                        Next
                        tblList.Rows.Add(rowTotals)
                    End If
                End If
            End If

            ' 05/13/15 cpb need all fields in case referenced in delete prompt -- set this flag so only try to cleanup once
            Dim blnFirstLoop As Boolean = True
            For Each rowList As DataRow In tblList.Rows
                strTDs = ""
                If IsDBNull(rowList("recid")) Then
                    rowList("recid") = -1
                End If
                If rowList("recid") = -99 Then
                    '06/05/2015 RLO
                    If Session(hidSessionsPrefix.Value & "perm") = "0000" Then
                    Else
                        strTDs &= "<td style=""text-align: center; font-weight:600;"" width=""150px;"">" &
                           " Totals"
                        strTDs &= "</td>"
                    End If
                Else
                    If strEditForm = "" Then
                        ' 1/9/15 cpb 
                        strOnclickView = "getDataRow(" & rowList("recid") & ",'" & TableName.Replace("'", "\'") & "','" & strViewTitle & "', 'view', '" & m_sessionPerm.ToString & "');"
                        strOnclickEdit = "getDataRow(" & rowList("recid") & ",'" & TableName.Replace("'", "\'") & "','" & strEditTitle & "', 'edit', '" & m_sessionPerm.ToString & "');"

                    Else
                        strOnclickView = "redirectDataRow(" & rowList("recid") & ",'" & strEditForm & "','" & strEditTarget & "', 'view','" & strEditIframe & "', '" & hidSessionsPrefix.Value & "');"
                        strOnclickEdit = "redirectDataRow(" & rowList("recid") & ",'" & strEditForm & "','" & strEditTarget & "', 'edit','" & strEditIframe & "', '" & hidSessionsPrefix.Value & "');"
                        'strOnclickDelete = "deleteDataRow(" & rowList("recid") & ");"
                        'strOnclickDelete = "deleteDataRow(" & rowList("recid") & ",'" & TableName.Replace("'", "\'")  & "');"
                    End If
                    Dim strDeleteShowColumn As String = ""
                    If strDeleteDisplayColumn = "" Then
                    Else
                        ' 05/13/15 cpb only replace on character or text fields
                        ' 05/13/15 cpb ability to display multiple fields for deletion confirmation.
                        Dim arrDisplayColumns() As String = Split(strDeleteDisplayColumn, ",")
                        Dim strDisplayColumnDelim As String = ""
                        For x = 0 To arrDisplayColumns.Length - 1
                            strDeleteShowColumn &= strDisplayColumnDelim & CStr(rowList(arrDisplayColumns(x))).Replace("'", "\'")
                        Next
                    End If

                    strOnclickDelete = "deleteDataRow(" & rowList("recid") & ",'" & TableName.Replace("'", "\'") & "','" & strDeleteDisplayText & "','" & strDeleteShowColumn & "');"

                    'Build Edit/View/Delete column 7/9/14 cpb build view/edit/delete based on how edit form loads
                    ' 2/2/15 cpb Build size of column based on options avail 50 for just view, 150 for all 3
                    '  --- also seems we no longer need to test streditform here..done above
                    Dim strViewEditDelete As String = ""
                    Dim strDelim As String = ""
                    Dim intWidth As Integer = 55
                    ' was 0 - 
                    If Mid(m_sessionPerm, 1, 1) = "1" Then
                        'view
                        strViewEditDelete = "<a href=""#"" onclick=""" & strOnclickView & "" & """><i class=""fa fa-search""  title=""View""> </i></a> "
                        'strDelim = " | "
                        'intWidth += 5
                    End If
                    If Mid(m_sessionPerm, 2, 1) = "1" Then
                        'edit
                        strViewEditDelete &= strDelim & "<a href=""#"" onclick=""" & strOnclickEdit & """> <i class=""fa fa-pencil"" title=""Edit""></i> </a> "
                        'strDelim = " | "
                        'intWidth += 5
                    End If
                    If Mid(m_sessionPerm, 3, 1) = "1" Then
                        'delete
                        strViewEditDelete &= strDelim & "<a href=""#"" onclick=""" & strOnclickDelete & """> <i class=""fa fa-times"" title=""Delete""></i> </a> "
                        'strDelim = " | "
                        'intWidth += 5
                    End If
                    'strViewEditDelete = "<a href=""#"" onclick=""" & strOnclickView & "" & """> View </a> "

                    'If m_sessionvo = "" OrElse UCase(m_sessionvo) = "FALSE" Then
                    '    strViewEditDelete &= " | " & _
                    '    "<a href=""#"" onclick=""" & strOnclickEdit & """> Edit </a> " & _
                    '    " | " & _
                    '    "<a href=""#"" onclick=""" & strOnclickDelete & """> Delete </a> "
                    '    intWidth = 150
                    'End If
                    'If strEditForm = "" Then
                    '    strTDs &= "<td style=""text-align:center; width:150px;"">" & _
                    '    "<a href=""#"" onclick=""" & strOnclickView & "" & """> View </a> "
                    '    If m_sessionvo = "" OrElse UCase(m_sessionvo) = "FALSE" Then
                    '        strViewEditDelete &= " | " & _
                    '        "<a href=""#"" onclick=""" & strOnclickEdit & """> Edit </a> " & _
                    '        " | " & _
                    '        "<a href=""#"" onclick=""" & strOnclickDelete & """> Delete </a> "
                    '        intWidth = 150
                    '    End If
                    'Else
                    '    strTDs &= "<td style=""text-align:center; width:150px;"">" & _
                    '    "<a href=""#"" onclick=""" & strOnclickView & "" & """> View </a> "
                    '    If m_sessionvo = "" OrElse UCase(m_sessionvo) = "FALSE" Then
                    '        strViewEditDelete &= " | " & _
                    '        "<a href=""#"" onclick=""" & strOnclickEdit & """> Edit </a> " & _
                    '        " | " & _
                    '        "<a href=""#"" onclick=""" & strOnclickDelete & """> Delete </a> "
                    '        intWidth = 150
                    '    End If
                    'End If
                    '060415 T3 only add column if showing options 
                    If Left(m_sessionPerm, 3) = "000" Then
                    Else
                        strTDs &= "<td style=""text-align:center; white-space: nowrap; max-width:" & intWidth & "px;"">" & strViewEditDelete & "</td>"
                    End If


                End If

                intLinkTDHeader = 0      ' 2/23/15 T3 reset this number because will get over counted b/c we are within another loop at this point
                For Each strLink In m_nvcColumnlink.Keys

                    If rowList("recid") = -99 Then
                        '06/05/2015 RLO
                        If Session(hidSessionsPrefix.Value & "perm") = "0000" Then
                            ' there will not be an OPTIONS column to put put the TOTALS label into
                        Else
                            strTDs &= "<td style=""text-align:center; font-weight:600;"" width=""150px;"">" &
                               " Totals"
                            strTDs &= "</td>"
                        End If

                    End If

                    Dim arrOnclickandParam() As String = Split(m_nvcColumnlink(strLink), "~~")
                    Dim arrParameters() As String = Split(arrOnclickandParam(2), "||")
                    Dim strDelim As String = ""
                    ' Link function needs to be in the site master associated with frmListManager
                    Dim strRowLink As String = "<a href= ""#"" onclick=""" & arrOnclickandParam(1) & "("
                    For Each strPar In arrParameters
                        If strPar.StartsWith("'") Then
                            strRowLink &= strDelim & strPar
                        Else
                            strRowLink &= strDelim & "'" & CStr(rowList(strPar)).Replace("'", "''") & "'"
                        End If
                        strDelim = ","
                    Next
                    strRowLink &= ")"
                    strRowLink &= """>" & arrOnclickandParam(0) & "</a>"
                    'strTDs &= "<td style=""text-align:center;"">" & strRowLink & "</td>"
                    strRowLink = "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; text-align:center;"" >" & strRowLink & "</td>"
                    'if a specific column handle here otherwise just add the td
                    ' the first original entry has been removed by this point (~~) entry.
                    If arrOnclickandParam.Length > 3 Then
                        Dim arrLinkRowLink() As String = Split(arrOnclickandParam(3), "||")     '~~firstName||ClickMeHeader  or  ~~||ClickMeTooHeader
                        If arrLinkRowLink.Count > 1 And arrLinkRowLink(0) <> "" Then
                            ' fieldname & header
                            nvcLinkTDHeaders(arrLinkRowLink(0)) = arrLinkRowLink(1)
                            nvcLinkTDs(arrLinkRowLink(0)) = strRowLink
                        Else
                            ' we got a fieldname or header for leading column
                            If arrLinkRowLink(0) = "" Then
                                ' header only, no after field name
                                nvcLinkTDHeadersFront(CStr(intLinkTDHeader)) = arrLinkRowLink(1)
                                intLinkTDHeader += 1
                                strTDs &= strRowLink
                            Else
                                ' after field name but no header
                                nvcLinkTDHeaders(arrLinkRowLink(0)) = ""
                                nvcLinkTDs(arrLinkRowLink(0)) = strRowLink
                            End If
                        End If
                        'onclick event of the link
                        'nvcLinkTDs(arrLinkRowLink(0)) = strRowLink
                    Else
                        ' leading link with no header 
                        strTDs &= strRowLink
                        nvcLinkTDHeadersFront(CStr(intLinkTDHeader)) = ""
                        intLinkTDHeader += 1
                    End If
                Next

                ' 05/13/15 cpb moved to here
                If blnFirstLoop Then
                    For Each strColumn As String In m_nvcColumnShowInGrid.Keys
                        If UCase(m_nvcColumnShowInGrid(strColumn)) = "FALSE" Then
                            tblList.Columns.Remove(strColumn)
                        End If
                    Next
                    For Each strColumn As String In m_nvcHidden.Keys
                        If UCase(m_nvcHidden(strColumn)) = "TRUE" Then
                            tblList.Columns.Remove(strColumn)
                        End If
                    Next
                    blnFirstLoop = False
                End If


                Dim blnTotalsLabelNotSet As Boolean = True
                For Each colColumn As DataColumn In tblList.Columns

                    strOnclick = "highlightRow(" & rowList("RECID") & ")"
                    Dim strDisplayLocn As String = ""       ' 2/2/15 cpb control display position in grid
                    'Or UCase(colColumn.ColumnName).Contains("PASSWORD")
                    If "RECID,TDS".Contains(UCase(colColumn.ColumnName)) Then
                    Else


                        '06/05/2015 RLO
                        If rowList("recid") = -99 AndAlso blnTotalsLabelNotSet Then
                            ' since no OPTIONS column put the Totals label in the first column available
                            strTDs &= "<td style=""overflow-x: hidden; text-align:center; font-weight:600;"" min-width=""150px;"">Totals</td>"
                            nvcColumnStyle(colColumn.ColumnName) = "text-align:center;"
                            blnTotalsLabelNotSet = False
                        Else


                            '10/3/14 handle password(set as change password link)
                            ' 01/16/15 T3 removed userid ref from href bc may not be used in your app
                            ' "&un=" & rowList("user_id")
                            ' sending param over to indicate coming from this page & will display link to close pw reset page
                            If m_nvcColumnPwd(colColumn.ColumnName) = "true" Then
                                ' 1/12/16 Does this user have access to change user passwords
                                If System.Web.HttpContext.Current.Session("AccountPasswords") Then

                                    If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                        ' 2/2/15 cpb test display position from database
                                        strDisplayLocn = "center"
                                    Else
                                        strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                    End If
                                    strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; text-align:" & strDisplayLocn & """>" &
                                        "<a href=""frmPasswordReset.aspx?rl=" & rowList("recid") & "&flm=1"" target=""_blank""> Change Password </a> " &
                                        "</td>"
                                    nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"
                                End If

                            ElseIf m_nvcColumnEmail(colColumn.ColumnName) = "true" Then
                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    ' 2/2/15 cpb test display position from database
                                    strDisplayLocn = "left"
                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                '02/20/2015 T3 Email link
                                strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden;  white-space: nowrap; text-align:" & strDisplayLocn & ";"">" &
                                    "<a href=""mailto:" & rowList(colColumn.ColumnName) & """> " & rowList(colColumn.ColumnName) & " </a> " &
                                    "</td>"
                                nvcColumnStyle(colColumn.ColumnName) = "white-space: nowrap; text-align:" & strDisplayLocn & ";"
                            ElseIf m_nvcColumnType(colColumn.ColumnName) = "tinyint" Then
                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    strDisplayLocn = "center"
                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; text-align:" & strDisplayLocn & ";""" &
                                "onclick=""" & strOnclick & """>"
                                nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"
                                ' checkbox
                                If IsDBNull(rowList(colColumn.ColumnName)) Then
                                Else
                                    If rowList(colColumn.ColumnName) = 1 Then
                                        ' true
                                        strTDs &= "<i class=""fa fa-check t3-check-color"" ></i>"
                                    Else
                                        strTDs &= "<i class=""fa fa-times t3-times-color"" ></i>"
                                    End If
                                End If
                                strTDs &= "</td>"
                                'Time & Money Else If's Added 8/27/14 - cp    'ToString("MM-dd-yyyy")
                                ' 9/18/14 cpb/ro handle 'time' field
                            ElseIf m_nvcColumnType(colColumn.ColumnName) = "image" Then
                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    strDisplayLocn = "center"
                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; text-align:" & strDisplayLocn & ";""" &
                                "onclick=""" & strOnclick & """>"
                                nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"
                                ' image
                                If IsDBNull(rowList(colColumn.ColumnName)) Then
                                Else
                                    Dim strThumbNameDelim As String = ""
                                    Dim strImageName As String = ""
                                    Dim strImageSrc As String = ""
                                    Dim strThumb_Delim As String = "\"
                                    Dim arrImageName As String() = Split(rowList(colColumn.ColumnName), "\")
                                    If arrImageName.Length <= 1 Then
                                        arrImageName = Split(rowList(colColumn.ColumnName), "/")
                                        strThumb_Delim = "/"
                                    End If
                                    If arrImageName.Length <= 1 Then
                                        strImageName = rowList(colColumn.ColumnName)
                                        strImageSrc = "thumb_" & strImageName
                                    Else
                                        strImageName = arrImageName(arrImageName.Length - 1)

                                        For y = 0 To arrImageName.Length - 1
                                            If y = arrImageName.Length - 1 Then
                                                strImageSrc &= strThumbNameDelim & "thumb_" & arrImageName(y)
                                            Else
                                                strImageSrc &= strThumbNameDelim & arrImageName(y)
                                            End If
                                            strThumbNameDelim = strThumb_Delim
                                        Next
                                        'strImageSrc = ""
                                    End If

                                    Dim strThumbName As String = ""
                                    strThumbNameDelim = ""
                                    Dim x As Integer = arrImageName.Length
                                    For y = 0 To x - 1
                                        If y < x - 1 Then
                                            strThumbName &= strThumbNameDelim & arrImageName(y)
                                        Else
                                            strThumbName &= strThumbNameDelim & "thumb_" & arrImageName(y)
                                        End If
                                        strThumbNameDelim = "\"
                                    Next
                                    g_resizeImage(MapPath(rowList(colColumn.ColumnName)), 20, 20, strThumbName)
                                    strTDs &= "<img alt=""" & arrImageName(arrImageName.Length - 1) & """ src=""" & strImageSrc & """ /> "
                                End If
                                strTDs &= "</td>"
                            ElseIf m_nvcColumnType(colColumn.ColumnName) = "time" Then
                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    strDisplayLocn = "left"
                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                Dim strTimeFormat As String = "hh:mm tt"
                                If IsNothing(m_nvcShowSeconds(colColumn.ColumnName)) Then
                                Else
                                    If UCase(m_nvcShowSeconds(colColumn.ColumnName)) = "TRUE" Then
                                        strTimeFormat = "hh:mm:ss tt"
                                    End If
                                End If

                                strTDs &= "<td nowrap class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; text-align:" & strDisplayLocn & ";""" &
                             "onclick=""" & strOnclick & """>"
                                nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"

                                If IsDBNull(rowList(colColumn.ColumnName)) Then
                                Else
                                    strTDs &= Format(CDate("01/01/01 " & rowList(colColumn.ColumnName).ToString()), strTimeFormat)
                                End If
                                strTDs &= "</td>"
                                ' DATETIME 07/23/15 T3
                            ElseIf m_nvcColumnType(colColumn.ColumnName) = "datetime" Then
                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    strDisplayLocn = "left"
                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                Dim strTimeFormat As String = "MM/dd/yyyy hh:mm tt"
                                If IsNothing(m_nvcShowSeconds(colColumn.ColumnName)) Then
                                Else
                                    If UCase(m_nvcShowSeconds(colColumn.ColumnName)) = "TRUE" Then
                                        strTimeFormat = "MM/dd/yyyy hh:mm:ss tt"
                                    End If
                                End If

                                strTDs &= "<td nowrap class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; text-align:" & strDisplayLocn & ";""" &
                             "onclick=""" & strOnclick & """>"
                                nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"

                                If IsDate(rowList(colColumn.ColumnName)) Then
                                    strTDs &= Format(CDate(rowList(colColumn.ColumnName).ToString()), strTimeFormat)
                                End If
                                strTDs &= "</td>"
                            ElseIf m_nvcColumnType(colColumn.ColumnName) = "date" Then
                                ' 11/25/15 cpb - handle date only field
                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    strDisplayLocn = "left"
                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                Dim strTimeFormat As String = "MM/dd/yyyy"
                                strTDs &= "<td nowrap class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; text-align:" & strDisplayLocn & ";""" &
                             "onclick=""" & strOnclick & """>"
                                nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"
                                If IsDate(rowList(colColumn.ColumnName)) Then
                                    If Format(CDate(rowList(colColumn.ColumnName).ToString()), strTimeFormat) = "01/01/1900" Then
                                    Else
                                        strTDs &= Format(CDate(rowList(colColumn.ColumnName).ToString()), strTimeFormat)
                                    End If

                                End If
                                strTDs &= "</td>"

                            ElseIf m_nvcColumnType(colColumn.ColumnName) = "money" Then
                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    strDisplayLocn = "right"
                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                If IsNumeric(rowList(colColumn.ColumnName)) Then
                                    strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden;  text-align:" & strDisplayLocn & ";""" &
                              "onclick=""" & strOnclick & """>" &
                              FormatCurrency(rowList(colColumn.ColumnName)) &
                              "</td>"
                                    nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"

                                Else
                                    strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden;  text-align:" & strDisplayLocn & ";""" &
                                      "onclick=""" & strOnclick & """>" &
                                      "</td>"
                                    nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"

                                End If

                            ElseIf m_nvcColumnType(colColumn.ColumnName) = "tinyint" Then
                                ' need to go through and determine what to display and/or export
                                'based on extProp "exportValue" which we need to pull and use here

                            ElseIf IsNothing(m_nvcColumnDDLTableName(colColumn.ColumnName)) Then
                                ' all other column types fall into here, if not ddl (see below)

                                If m_nvcColumnDisplayLocn(colColumn.ColumnName) = "" Then
                                    If m_nvcColumnType(colColumn.ColumnName) = "decimal" Or m_nvcColumnType(colColumn.ColumnName) = "int" Then
                                        strDisplayLocn = "right"
                                    Else
                                        strDisplayLocn = "left"
                                    End If

                                Else
                                    strDisplayLocn = m_nvcColumnDisplayLocn(colColumn.ColumnName)
                                End If
                                'text-indent: 5px; 
                                strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden;  text-align:" & strDisplayLocn & ";""" &
                             "onclick=""" & strOnclick & """>" &
                            rowList(colColumn.ColumnName) &
                            "</td>"
                                nvcColumnStyle(colColumn.ColumnName) = "text-align:" & strDisplayLocn & ";"

                            Else
                                ' we are looking up the value to display from ddl
                                Dim strDisplayValue As String = ""
                                '9/19/14 Removed rlo's if isnumeric()
                                ' 9/17/14 rlo added if is numeric
                                ' 9/18/14 cpb -- took out hard codein where for recid -- use field identified in database

                                Dim strWhereRef As String = " recid "
                                If IsNothing(m_nvcColumnDDLValue(colColumn.ColumnName)) Then
                                Else
                                    strWhereRef = m_nvcColumnDDLValue(colColumn.ColumnName)
                                End If

                                strSQL = " Select " & m_nvcColumnDDLValue(colColumn.ColumnName) & " , " & m_nvcColumnDDLText(colColumn.ColumnName) &
                                    " FROM " & m_nvcColumnDDLTableName(colColumn.ColumnName) & " where " & strWhereRef & " = '" & rowList(colColumn.ColumnName) & "'"
                                Dim tblDDLList As DataTable = g_IO_Execute_SQL(strSQL, False)
                                If tblDDLList.Rows.Count = 0 Then
                                Else
                                    ' 05/13/15 cpb handle multiple fields in ddlText
                                    Dim arrDDLText() As String = Split(m_nvcColumnDDLText(colColumn.ColumnName), ",")
                                    Dim strDDLDelim As String = ""
                                    For x = 0 To arrDDLText.Length - 1
                                        strDisplayValue &= strDDLDelim & tblDDLList.Rows(0)(arrDDLText(x))
                                        strDDLDelim = ","
                                    Next
                                    'strDisplayValue = tblDDLList.Rows(0)(m_nvcColumnDDLText(colColumn.ColumnName))
                                End If
                                'text-indent: 5px; 

                                strTDs &= "<td class=""td" & rowList("RECID") & """ style=""overflow-x: hidden; """ &
                                    "onclick=""" & strOnclick & """>" &
                                    strDisplayValue &
                                    "</td>"

                            End If
                            '02/23/15 T3 check for link to display after column just added
                            If IsNothing(nvcLinkTDs(colColumn.ColumnName)) Then
                            Else
                                strTDs &= nvcLinkTDs(colColumn.ColumnName)
                            End If

                        End If
                    End If
                Next
                rowList("TDs") = strTDs
            Next
        End If


        rprGridListing.DataSource = tblList
        rprGridListing.DataBind()


        ' sorting options
        Dim strSeqOrder = "sorta"
        Dim strSortInteger As String = "1"
        If m_sessionsrt = "" Then
        Else
            strSortInteger = m_sessionsrt
        End If

        If m_sessionseq = "" Then
        Else
            If m_sessionseq = "d" Then
                strSeqOrder = "sortd"
            End If
        End If

        ' 12/28/15 T3 Added hidden textbox to store sort column and sequence
        txtSortCol.Text = strSortInteger & "||" & strSeqOrder

        ' row header
        Dim tblColumns As New DataTable
        tblColumns.Columns.Add("Description", GetType(String))
        tblColumns.Columns.Add("Index", GetType(String))
        tblColumns.Columns.Add("align", GetType(String))
        'tblColumns.Columns.Add("max-width", GetType(String))

        ' options add/change/view
        Dim rowColumnOpt As DataRow = tblColumns.NewRow
        rowColumnOpt("Description") = "" ' Options 10/15/15 CP
        rowColumnOpt("Index") = -1
        '060415 T3 only add header if showing options 
        If Left(m_sessionPerm, 3) = "000" Then
        Else
            tblColumns.Rows.Add(rowColumnOpt)
        End If

        ' 02/23/15 T3 add in any link headers at front
        For Each linkHeader As String In nvcLinkTDHeadersFront.Keys
            Dim rowColumn As DataRow = tblColumns.NewRow
            rowColumn("Description") = nvcLinkTDHeadersFront(linkHeader)
            rowColumn("Index") = -1
            rowColumn("align") = "text-align:center;"
            tblColumns.Rows.Add(rowColumn)
        Next
        For i = tblList.Columns.Count - 1 To 0 Step -1
            Dim strColumnName As String = tblList.Columns(i).ColumnName
            If m_nvcColumnPwd(strColumnName) = "true" Then
                ' 1/12/16 Does this user have access to change user passwords
                If System.Web.HttpContext.Current.Session("AccountPasswords") Then
                Else
                    tblList.Columns.Remove(strColumnName)
                End If
            End If
        Next
        For Each colColumn As DataColumn In tblList.Columns
            ' Or UCase(colColumn.ColumnName).Contains("PASSWORD")
            If "RECID,TDS".Contains(UCase(colColumn.ColumnName)) Then
            Else

                Dim rowColumn As DataRow = tblColumns.NewRow

                If IsNothing(nvcColumnStyle(colColumn.ColumnName)) Then
                    rowColumn("align") = ""
                Else
                    rowColumn("align") = nvcColumnStyle(colColumn.ColumnName)

                End If

                'If IsNothing(m_nvcColumnLength(colColumn.ColumnName)) Then
                'Else
                '    rowColumn("max-width") = "max-width: " & m_nvcColumnLength(colColumn.ColumnName) & "px;"

                'End If


                If IsNothing(m_nvcColumnDescription(colColumn.ColumnName)) Then
                            rowColumn("Description") = colColumn.ColumnName
                        Else
                            rowColumn("Description") = m_nvcColumnDescription(colColumn.ColumnName)
                        End If
                        rowColumn("Index") = m_nvcColumnIndex(colColumn.ColumnName)
                        tblColumns.Rows.Add(rowColumn)

                        '02/23/15 T3 check for link to display before a column
                        If IsNothing(nvcLinkTDs(colColumn.ColumnName)) Then
                        Else
                            Dim rowColumnLink As DataRow = tblColumns.NewRow
                            If IsNothing(nvcLinkTDHeaders(colColumn.ColumnName)) Then
                                rowColumnLink("Description") = ""
                            Else
                                rowColumnLink("Description") = nvcLinkTDHeaders(colColumn.ColumnName)
                            End If
                            rowColumnLink("Index") = -1
                            rowColumnLink("align") = "text-align:center;"
                            tblColumns.Rows.Add(rowColumnLink)

                        End If
                    End If
        Next

        rprGridHeader.DataSource = tblColumns
        rprGridHeader.DataBind()

        If m_blnExportMode Then
            ' 2/18/16 We have the table exporting...
            ' but need to remove link columns maybe above while it is building up the data
            ' if it hits a pw or link column skip it or remove it from the table if in Export Mode
            Dim strGridHTML As String = "<table><tr>"
            For Each row As DataRow In tblColumns.Rows
                strGridHTML &= "<th>" & row("Description") & "</th>"
            Next
            strGridHTML &= "</tr>"
            For Each row As DataRow In tblList.Rows
                strGridHTML &= "<tr>" & row("TDs").Replace("<i class=""fa fa-check t3-check-color"" ></i>", "True").Replace("<i class=""fa fa-times t3-times-color"" ></i>", "False") & "</tr>"
            Next
            strGridHTML &= "</table>"
            Session("FLMGridHTML") = strGridHTML
            Response.Redirect("frmExportFLM.aspx?expType=" & Request.QueryString("expType") & "filename=" & Request.QueryString("filename"))
        End If

    End Sub

    Private Sub LoadPagination(ByVal TableName As String, ByVal strWhere As String)



        Dim strSQL As String = "Select count(*) As tblCount from " & TableName & strWhere
        Dim tblRowCount As DataTable = g_IO_Execute_SQL(strSQL, False)
        m_introwcount = tblRowCount.Rows(0)("tblCount")
        Dim intItemsPerPage As Integer
        If m_intItemsPerPage > 0 Then
            intItemsPerPage = m_intItemsPerPage
        Else
            intItemsPerPage = m_introwcount
            m_intItemsPerPage = m_introwcount
        End If
        m_intPageCount = Math.Ceiling(m_introwcount / intItemsPerPage)
        Dim intMaxPageNumToDisplay As Integer = 3
        Dim intFirstPageInList As Integer = 1
        Dim intLastPageInList As Integer = 1
        Dim intCurrentPage As Integer = m_intPageNo


        If m_introwcount = 0 Then
            litPagination.Text = ""
        Else
            Dim intCurrentSection As Integer = Math.Floor((intCurrentPage - 1) / intMaxPageNumToDisplay) + 1
            intFirstPageInList = intMaxPageNumToDisplay * intCurrentSection - (intMaxPageNumToDisplay - 1)
            If intFirstPageInList + intMaxPageNumToDisplay - 1 > m_intPageCount Then
                intLastPageInList = m_intPageCount
            Else
                intLastPageInList = intFirstPageInList + intMaxPageNumToDisplay - 1
            End If

            Dim intLastSection As Integer = Math.Floor((m_intPageCount - 1) / intMaxPageNumToDisplay) + 1

            Dim intNextSection = intCurrentSection
            If intCurrentSection + 1 > intLastPageInList Then
            Else
                intNextSection = intCurrentSection + 1
            End If
            Dim intNextSectionFirstPage As Integer = intMaxPageNumToDisplay * intNextSection - (intMaxPageNumToDisplay - 1)

            Dim intPrevSection = intCurrentSection
            If intCurrentSection - 1 < 1 Then
            Else
                intPrevSection = intCurrentSection - 1
            End If
            Dim intPrevSectionFirstPage As Integer = intMaxPageNumToDisplay * intPrevSection - (intMaxPageNumToDisplay - 1)

            Dim strPagination As String = ""
            If intCurrentPage > 1 Then
                strPagination &= "<li><a href=""#"" onclick=""flmPagingReload('frmListManager.aspx?E=" & g_Encrypt("pageId=1&pre=1&sesPrefix=" & hidSessionsPrefix.Value) & "')""><span class=""glyphicon glyphicon-step-backward""></span></a></li>"
            Else
                strPagination &= "<li class=""disabled""><span class=""glyphicon glyphicon-step-backward""></span></li>"
            End If
            If intCurrentSection > 1 Then
                strPagination &= "<li><a href=""#"" onclick=""flmPagingReload('frmListManager.aspx?E=" & g_Encrypt("pageId=" & intPrevSectionFirstPage & "&pre=1&sesPrefix=" & hidSessionsPrefix.Value) & "')""><span class=""glyphicon glyphicon-fast-backward""></span></a></li>"
            Else
                strPagination &= "<li class=""disabled""><span class=""glyphicon glyphicon-fast-backward""></span></li>"
            End If
            If intCurrentPage > 1 Then
                strPagination &= "<li><a href=""#"" onclick=""flmPagingReload('frmListManager.aspx?E=" & g_Encrypt("pageId=" & intCurrentPage - 1 & "&pre=1&sesPrefix=" & hidSessionsPrefix.Value) & "')""><span class=""glyphicon glyphicon-backward""></span></a></li>"
            Else
                strPagination &= "<li class=""disabled""><span class=""glyphicon glyphicon-backward""></span></li>"
            End If

            For intLoop = intFirstPageInList To intLastPageInList
                strPagination &= "<li"
                If intLoop = intCurrentPage Then
                    strPagination &= " class=""active"""
                End If
                strPagination &= "><a href=""#"" onclick=""flmPagingReload('frmListManager.aspx?E=" & g_Encrypt("pageId=" & intLoop & "&pre=1&sesPrefix=" & hidSessionsPrefix.Value) & "')"">" & intLoop & "</a></li>"
            Next

            ' display forward options
            If intCurrentPage < m_intPageCount Then
                strPagination &= "<li><a href=""#"" onclick=""flmPagingReload('frmListManager.aspx?E=" & g_Encrypt("pageId=" & intCurrentPage + 1 & "&pre=1&sesPrefix=" & hidSessionsPrefix.Value) & "')""><span class=""glyphicon glyphicon-forward""></span></a></li>"
            Else
                strPagination &= "<li class=""disabled""><span class=""glyphicon glyphicon-forward""></span></li>"
            End If
            If intCurrentSection < intLastSection Then
                strPagination &= "<li><a href=""#"" onclick=""flmPagingReload('frmListManager.aspx?E=" & g_Encrypt("pageId=" & intNextSectionFirstPage & "&pre=1&sesPrefix=" & hidSessionsPrefix.Value) & "')""><span class=""glyphicon glyphicon-fast-forward""></span></a></li>"
            Else
                strPagination &= "<li class=""disabled""><span class=""glyphicon glyphicon-fast-forward""></span></li>"
            End If
            If intCurrentPage < m_intPageCount Then
                strPagination &= "<li><a href=""#"" onclick=""flmPagingReload('frmListManager.aspx?E=" & g_Encrypt("pageId=" & m_intPageCount & "&pre=1&sesPrefix=" & hidSessionsPrefix.Value) & "')""><span class=""glyphicon glyphicon-step-forward""></span></a></li>"
            Else
                strPagination &= "<li class=""disabled""><span class=""glyphicon glyphicon-step-forward""></span></li>"
            End If
            strPagination &= "</ul>"
            litPagination.Text = strPagination

            ' current page location
            ' 11/24/15 T3 removed "Page: " & intCurrentPage & " of " & m_intPageCount & ". " &
            litCurrentPageInfo.Text = " of " & m_introwcount & " rows."


            If m_introwcount < m_intItemsPerPage Then
                ddlItemsPerPage.Items.Add("All")
                ddlItemsPerPage.Items.FindByText("All").Value = "-1"
            Else
                Dim blnCustomItemsPerPageAdded As Boolean = False
                If m_intItemsPerPage < 10 Then
                    ddlItemsPerPage.Items.Add(m_intItemsPerPage)
                    blnCustomItemsPerPageAdded = True
                End If
                ddlItemsPerPage.Items.Add("10")
                ddlItemsPerPage.Items.FindByText("10").Selected = True
                If blnCustomItemsPerPageAdded = False AndAlso m_intItemsPerPage < 25 Then
                    ddlItemsPerPage.Items.Add(m_intItemsPerPage)
                    blnCustomItemsPerPageAdded = True
                End If
                ddlItemsPerPage.Items.Add("25")
                If blnCustomItemsPerPageAdded = False AndAlso m_intItemsPerPage < 50 Then
                    ddlItemsPerPage.Items.Add(m_intItemsPerPage)
                    blnCustomItemsPerPageAdded = True
                End If
                ddlItemsPerPage.Items.Add("50")
                If blnCustomItemsPerPageAdded = False AndAlso m_intItemsPerPage < 100 Then
                    ddlItemsPerPage.Items.Add(m_intItemsPerPage)
                    blnCustomItemsPerPageAdded = True
                End If
                ddlItemsPerPage.Items.Add("100")
                If blnCustomItemsPerPageAdded = False AndAlso m_intItemsPerPage > 100 Then
                    ddlItemsPerPage.Items.Add(m_intItemsPerPage)
                    blnCustomItemsPerPageAdded = True
                End If
                ddlItemsPerPage.Items.Add("All")
                ddlItemsPerPage.Items.FindByText("All").Value = "-1"
            End If


            ' display specify page number
            'rvPageNum.MaximumValue = m_intPageCount
            'rvPageNum.ErrorMessage = "Enter a value between 1 And " & m_intPageCount

            ' set validation for # items per page
            strSQL = "Select recid from " & TableName
            Dim tblItemsPerPage As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblItemsPerPage.Rows.Count > 0 Then
            Else
                'rvItemsPerPage.MaximumValue = 1
            End If

        End If
        'Set items per page textbox 10/15/2015 CP
        'txtItemsPerPage.Attributes("placeholder") = m_intItemsPerPage
        If m_intItemsPerPage >= m_introwcount Then
            ddlItemsPerPage.SelectedValue = "-1"  'all
            litPaginationHeader.Text = "All" & m_introwcount & " rows."
        Else
            ddlItemsPerPage.SelectedValue = m_intItemsPerPage
            '10/13/14 - add accorian for pagination
            litPaginationHeader.Text = m_intItemsPerPage & " of " & m_introwcount & " rows."
        End If

    End Sub


    Private Function BuildSearchRecidsList(ByVal TableName As String, ByVal strSearch As String, ByVal strSearchLogic As String) As String
        Dim strSql As String = " Select recid"
        Dim strWhere As String = " where "
        Dim strDelim As String = ""
        Dim strWhereDelim As String = ""

        For Each strColumnName As String In m_nvcColumnType.Keys
            ' 7/7/15 T3 Handle datetime format differently
            If m_nvcColumnType(strColumnName) = "datetime" Then
                If IsDate(strSearch) Then
                    strSearch = Format(CDate(strSearch), "MMM dd yyyy")
                    strWhere &= strWhereDelim & "cast([" & strColumnName & "] As nvarchar(max)) " & strSearchLogic & "'" & strSearch.Replace("'", "''") & "' "
                    strWhereDelim = " or "
                End If
            Else
                If IsDate(strSearch) Then
                    strSearch = Format(CDate(strSearch), "yyyy-MM-dd")
                End If
                If IsNothing(m_nvcColumnDDLTableName(strColumnName)) Then
                    strWhere &= strWhereDelim & "cast([" & strColumnName & "] as nvarchar(max)) " & strSearchLogic & "'" & strSearch.Replace("'", "''") & "' "
                    strWhereDelim = " or "
                Else
                    strWhere &= strWhereDelim & "(select TOP 1 "
                    Dim strDelimCol As String = ""
                    Dim arrDDLColumns() = Split(m_nvcColumnDDLText(strColumnName), ",")
                    For Each col In arrDDLColumns
                        strWhere &= strDelimCol & " CAST(" & col & " AS varchar(5000))"
                        strDelimCol = " + "
                    Next
                    strWhere &= " from " & m_nvcColumnDDLTableName(strColumnName) & " where recid = A." & strColumnName & ") " & strSearchLogic & "'" & strSearch.Replace("'", "''") & "' "
                    strWhereDelim = " or "
                End If
            End If

            'strWhere &= strWhereDelim & "cast([" & strColumnName & "] as nvarchar(max)) like '%" & strSearch.Replace("'", "''") & "%' "
            'strWhereDelim = " or "
        Next

        strSql &= " from " & TableName & " as A  " & strWhere

        Dim tblHits As DataTable = g_IO_Execute_SQL(strSql, False)
        Dim strRecids As String = ""
        strDelim = ""
        For Each row In tblHits.Rows
            strRecids &= strDelim & row("recid")
            strDelim = ","
        Next

        Return strRecids

    End Function

End Class