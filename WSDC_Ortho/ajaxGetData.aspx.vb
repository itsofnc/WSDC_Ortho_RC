Public Class ajaxGetData
    Inherits System.Web.UI.Page
    '***********************************Last Edit**************************************************

    'Last Edit Date: Last Edit Date: 11/25/2015
    'Last Edit By: cpb
    'Last Edit Proj: ASGA
    '-----------------------------------------------------
    'Change Log: 
    ' 11/25/15 cpb if date is SQL default, then set to "" for display
    ' 03/18/15 cpb ext property confirmDeleteView can blow up if database not properly setup-fix; multiple 'choose' an option in dropdowns on new record fix
    '               change phone format validation to onblur in phone number is also unique b/c unique tests onchange- moved phone number validation to frmListManager like unique
    '               change email validation on onblur in case the email is also unique as this runs on onchange'
    '               fix for confirmDelete property set, but no corresponding view setup blow up if database not properly setup.
    '               add password format validation
    ' 3/13/15 T3 Check for unique data validation
    ' 3/9/15 T3 - added call to g_GetIndexes to look at table indexes (for unique field validation)
    ' *** Need to code how to validate unique fields at Save or onChange... ***

    ' 3/6/15 T3 Need to add validation on a field w/ unique index on column (ie UserID)
    ' 02/02/15 cpb add ext propery for column display location left,right,center
    ' 1/9/15 - cpb - send over url vm for view mode for 'read' action.  If ='view' all fields are disabled this is view only
    ' 01/06/15 T3 - remove columnback class if field is disabled via ext prop
    '10/13/14 - call g_GetTableExtProperty now to get extended properties (was calling g_GetTableDescrip...)
    '10/6/14 - t3 - Added m_nvcColumnEmail&m_nvcColumnPhone & validators
    '10/3/14 - t3 - Added m_nvcColumnPassword/validation
    '09/29/14 - t3 - Added required field indicator and reqMsg  attributes (ext. property (required = true/false) on column
    '09/19/14-t3 - Auto disable fields when master file referenced in ext properites for a view;
    '               -- make insert/add - update master table
    '               -- make update only update fields from the master table
    '09/18/14-cpb - Handle Time field
    '09/15/14-t3 - Delete now checks history before deleting a record from a table

    '**********************************************************************************************
    Dim m_nvcColumnType As New NameValueCollection
    Dim m_nvcColumnLength As New NameValueCollection
    Dim m_nvcColumnDescription As New NameValueCollection
    Dim m_nvcColumnDDLTableName As New NameValueCollection
    Dim m_nvcColumnDDLValue As New NameValueCollection
    Dim m_nvcColumnDDLText As New NameValueCollection
    Dim m_nvcColumnDDLFilter As New NameValueCollection
    Dim m_nvcColumnPwd As New NameValueCollection
    Dim m_strColumnsLE20 As String = ""
    Dim m_strColumnsLE45 As String = ""
    Dim m_strColumnsGT45 As String = ""
    Dim m_intMaxColumns As Integer = 1
    Dim m_nvcColumnShowInGrid As New NameValueCollection
    Dim m_nvcColumnShowInPopup As New NameValueCollection ' 09/01/15 T3
    Dim m_nvcHidden As New NameValueCollection
    Dim m_nvcDisabled As New NameValueCollection
    Dim m_nvcColumnIndex As New NameValueCollection
    Dim m_nvcColumnRequired As New NameValueCollection
    Dim m_nvcColumnPassword As New NameValueCollection
    Dim m_nvcColumnEmail As New NameValueCollection
    Dim m_nvcColumnPhone As New NameValueCollection
    Dim m_nvcColumnTotal As New NameValueCollection
    Dim m_nvcColumnDisplayLocn As New NameValueCollection
    Dim m_nvcColumnRegExpPattern As New NameValueCollection
    Dim m_nvcColumnRegExpMessage As New NameValueCollection
    Dim m_nvcMinValue As New NameValueCollection
    Dim m_nvcMaxValue As New NameValueCollection
    Dim m_nvcColumnDefaultValue As New NameValueCollection
    Dim m_nvcaSign As New NameValueCollection
    Dim m_nvcpSign As New NameValueCollection
    Dim m_nvcImageMinMax As New NameValueCollection
    Dim m_nvcImageHover As New NameValueCollection
    Dim m_nvcPercentage As New NameValueCollection
    Dim m_nvcShowSeconds As New NameValueCollection ' 07/23/15 T3

    ' 3/13/15 T3
    Dim m_nvcColumnUnique As New NameValueCollection
    Dim m_nvcColumnNames As New NameValueCollection '04/30/15 T3
    Dim m_strAction As String = ""
    Dim m_intRecId As Integer = -1
    Dim m_strViewOnly As String = ""    ' 1/9/15 cpb for view only mode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strTable As String = ""

        If IsNothing(Request.Form("action")) Then
        Else
            m_strAction = Request.Form("action")
        End If

        If IsNothing(Request.Form("id")) Then
        Else
            m_intRecId = Request.Form("id")
        End If

        If IsNothing(Request.Form("tb")) Then
        Else
            strTable = Request.Form("tb")
            'strTable = strTable.Replace("_vw", "")
        End If

        litConstants.Text = "<input type=""hidden"" id=""postid"" value=""" & m_intRecId & """/><input type=""hidden"" id=""posttb"" value=""" & strTable & """/>"

        Select Case m_strAction
            Case "read"
                If IsNothing(Request.Form("vm")) Then
                Else
                    m_strViewOnly = Request.Form("vm")
                End If
                LoadUpdateArea(strTable, m_intRecId)
                'Dim strSQL As String = "select recid, descr from " & strTable & " where recid = " & intRecid
                'Dim tblDivisions As DataTable = g_IO_Execute_SQL(strSQL, False)
                'If tblDivisions.Rows.Count > 0 Then
                '    txtRecid.Text = tblDivisions.Rows(0)("Recid")
                '    txtDescr.Text = tblDivisions.Rows(0)("Descr")
                '    txtTable.Text = Session("SelectedList")
                'End If

            Case "update"
                '****here*** we should switch to the common update code using a name value collection -- need to pull this in
                ' 3/16/15 T3 get list of columns and values to compare to index before insert/update
                Dim nvcColumns As New NameValueCollection

                g_GetColumns(strTable, m_nvcColumnLength, m_nvcColumnType, m_nvcColumnDescription, m_nvcColumnDDLTableName,
                     m_nvcColumnDDLValue, m_nvcColumnDDLText, m_nvcColumnPwd, m_nvcColumnShowInGrid, m_nvcColumnShowInPopup, m_nvcHidden,
                     m_nvcDisabled, m_nvcColumnIndex, m_nvcColumnRequired, m_nvcColumnEmail, m_nvcColumnPhone,
                     m_nvcColumnTotal, m_nvcColumnDisplayLocn, m_nvcColumnUnique, m_nvcColumnNames,
                     m_nvcColumnRegExpPattern, m_nvcColumnRegExpMessage, m_nvcMinValue, m_nvcMaxValue, m_nvcColumnDefaultValue,
                     m_nvcaSign, m_nvcpSign, m_nvcPercentage, m_nvcShowSeconds, m_nvcColumnDDLFilter, m_nvcImageMinMax, m_nvcImageHover)

                ' 6/9/2015 T3 Need to be able to run a function when saving a record
                ' CS will finish this :)
                Dim strSaveFunction As String = g_GetTableExtProperty(strTable, "saveFunction")

                If m_intRecId = -1 Then
                    ' new entry
                    Dim strInsertColumnNames As String = ""
                    Dim strInsertValues As String = ""
                    Dim strInsertValuesDelim As String = ""
                    For Each strUpdateInfo As String In Split(Request.Form("dback"), "::")
                        Dim strColumnName As String = Split(strUpdateInfo, "||")(0)
                        strInsertColumnNames &= strInsertValuesDelim & strColumnName


                        If UCase(strColumnName).Contains("PASSWORD") Then
                            strInsertValues &= strInsertValuesDelim & _
                                "CONVERT(VARCHAR(32), HashBytes('MD5', '" & Split(strUpdateInfo, "||")(1) & "'), 2)"
                        Else
                            Dim strValues() As String = Split(strUpdateInfo, "||")
                            '060215 T3
                            'If ddl and value is "" we are setting value to -1
                            If IsNothing(m_nvcColumnDDLTableName(strColumnName)) Then
                            Else
                                If Trim(strValues(1)) = "" Then
                                    strValues(1) = "-1"
                                End If
                            End If
                            strInsertValues &= strInsertValuesDelim & "'" & strValues(1).Replace("'", "''") & "'"
                            nvcColumns(strColumnName) = strValues(1).Replace("'", "''")
                        End If

                        strInsertValuesDelim = ","
                    Next

                    Dim strDescr As String = ""
                    If IsNothing(Request.Form("descr")) Then
                    Else
                        strDescr = Request.Form("descr")
                    End If
                    Dim strInsertTable As String = g_GetTableExtProperty(strTable, "MasterTable")
                    If IsNothing(strInsertTable) OrElse strInsertTable = "" Then
                        strInsertTable = strTable
                    End If

                    ' 3/16/15 T3 Look at unique indexes and validate against existing data records

                    Dim nvcUniqueIndexes As NameValueCollection = g_GetIndexes(strInsertTable, True)
                    For Each key In nvcUniqueIndexes
                        Dim strSQLUnique As String = "Select count (*) as recCount From " & strInsertTable
                        Dim strWhere As String = " Where "
                        Dim strDelim As String = ""
                        Dim strDelimComma As String = ""
                        Dim strReturnMsg As String = ""
                        Dim intColCount As Integer = 0
                        ' get what field is in the index & is this in our list of columns
                        Dim arrFieldID() As String = Split(nvcUniqueIndexes(key), "||")
                        For Each column In arrFieldID
                            ' is this field in our list of fields user has entered
                            strWhere &= strDelim & column & " = '" & nvcColumns(column) & "'"
                            strDelim = " and "
                            Dim strColumnDescr As String = g_GetColumnExtPropertyValue(strTable, "MS_Description", column)
                            strReturnMsg &= strDelimComma & IIf(strColumnDescr = "", column, strColumnDescr)
                            strDelimComma = ","
                            intColCount += 1
                        Next
                        strSQLUnique &= strWhere
                        Dim tblIndexCheck As DataTable = g_IO_Execute_SQL(strSQLUnique, False)
                        If tblIndexCheck.Rows(0)("recCount") > 0 Then
                            ' key already on file - not unique can not save it
                            lblMessage.Text = "%% The " & strReturnMsg & IIf(intColCount > 1, " combination", "") & " is already on file.~~This record could not be saved.%%"
                        End If
                    Next

                    Dim strSQL As String = "insert into " & strInsertTable & " ( " & strInsertColumnNames & ") values (" & strInsertValues & ")"
                    g_IO_Execute_SQL(strSQL, False)
                    ' 4/19/16 T3 need to update FLM sesLst with new recid
                    If IsNothing(Session(Request.Form("sesLst"))) Then
                    Else
                        If Session(Request.Form("sesLst")) = "" Then
                            Session(Request.Form("sesLst")) &= g_IO_GetLastRecId()
                        Else
                            Session(Request.Form("sesLst")) &= "," & g_IO_GetLastRecId()
                        End If
                    End If
                    If strSaveFunction <> "" Then
                        ' table has ext prop to run a save function
                        g_flmSaveFunction(strSaveFunction, strTable, g_IO_GetLastRecId())
                    End If
                Else
                    Dim strUpdate As String = ""
                    Dim strUpdateDelim As String = ""
                    For Each strUpdateInfo As String In Split(Request.Form("dback"), "::")
                        Dim strColumnName As String = Split(strUpdateInfo, "||")(0)
                        Dim strValues() As String = Split(strUpdateInfo, "||")
                        If IsNothing(m_nvcColumnPwd(strColumnName)) Then
                            '060215 T3
                            'If ddl and value is "" we are setting value to -1
                            If IsNothing(m_nvcColumnDDLTableName(strColumnName)) Then
                            Else
                                If Trim(strValues(1)) = "" Then
                                    strValues(1) = "-1"
                                End If
                            End If
                            strUpdate &= strUpdateDelim & strColumnName & "='" & strValues(1).Replace("'", "''") & "'"
                        Else
                            If Trim(strValues(1)) = "" Then
                            Else
                                strUpdate &= strUpdateDelim & strColumnName & "=" & _
                                    "CONVERT(VARCHAR(32), HashBytes('" & m_nvcColumnPwd(strColumnName) & "', '" & strValues(1) & "'), 2)"
                            End If

                        End If
                        strUpdateDelim = ","
                    Next

                    Dim strDescr As String = ""
                    If IsNothing(Request.Form("descr")) Then
                    Else
                        strDescr = Request.Form("descr")
                    End If

                    Dim strUpdateTable As String = g_GetTableExtProperty(strTable, "MasterTable")
                    If IsNothing(strUpdateTable) OrElse strUpdateTable = "" Then
                        strUpdateTable = strTable
                    End If

                    Dim strSQL As String = "update " & strUpdateTable & " set " & strUpdate & " where recid = " & m_intRecId
                    g_IO_Execute_SQL(strSQL, False)
                    ' check for error processing strSQL
                    If IsNothing(Session("SQLERROR")) Then
                    Else
                        lblMessage.Text = "*SQL_ERROR*" & Session("SQLERROR") & "*SQL_ERROR*"
                    End If

                    If strSaveFunction <> "" Then
                        ' table has ext prop to run a save function
                        g_flmSaveFunction(strSaveFunction, strTable, m_intRecId)
                    End If
                End If
            Case "delete"
                Dim blnHistory As Boolean = False
                Dim strReturnMsg As String = ""
                Dim strHistoryTable As String = g_GetTableExtProperty(strTable, "confirmDeleteView")
                If IsNothing(strHistoryTable) OrElse strHistoryTable = "" Then
                Else
                    Dim strCheckSql As String = "select * from " & strHistoryTable & " where recid = '" & m_intRecId & "'"
                    Dim tblCheckResults As DataTable = g_IO_Execute_SQL(strCheckSql, False)

                    ' 03/18/15 cpb can blow up if database not properly setup.
                    If IsNothing(tblCheckResults) Then
                        strReturnMsg = "<h4 style=""text-indent:5px;"">This record cannot be deleted.<br />&nbsp;&nbsp;Where Used data not defined.<br />&nbsp;&nbsp;Please Contact Support.</h4> <br />"
                        blnHistory = True
                    Else
                        For Each column In tblCheckResults.Columns
                            If (column.columnname).toUpper() <> "RECID" And tblCheckResults.Rows(0)(column.columnname) > 0 Then
                                blnHistory = True
                                If strReturnMsg = "" Then
                                    strReturnMsg = " <h4 style=""text-indent:5px;"">This record cannot be deleted, in use:</h4> <br /> "
                                End If
                                strReturnMsg &= "<h4 style=""text-indent:5px;"">" & tblCheckResults.Rows(0)(column.columnname) & " " & column.columnname & " found.</h4> <br/>"
                            End If
                        Next
                    End If
                End If

                If blnHistory = False Then
                    lblMessage.Text = "%%%%"

                    'get Mast Table from ext. property of view
                    Dim strDeleteTable As String = g_GetTableExtProperty(strTable, "MasterTable")
                    If IsNothing(strDeleteTable) OrElse strDeleteTable = "" Then
                        strDeleteTable = strTable
                    End If
                    Dim strSql As String = ""
                    'delete
                    strSql = "delete " & strDeleteTable & " where recid = " & m_intRecId
                    g_IO_Execute_SQL(strSql, False)
                Else
                    lblMessage.Text = "%%" & strReturnMsg & "%%"
                End If
            Case "validateUniqueField"
                '3/13/15 T3 Check for unique data validation
                Dim strTableId As String = IIf(IsNothing(Request.QueryString("tbl")), Request.Form("tbl"), Request.QueryString("tbl"))
                Dim strFieldId As String = IIf(IsNothing(Request.QueryString("fld")), Request.Form("fld"), Request.QueryString("fld"))
                Dim strValue As String = IIf(IsNothing(Request.QueryString("val")), Request.Form("val"), Request.QueryString("val")).ToString.Replace("'", "''")
                Dim strDescr As String = IIf(IsNothing(Request.QueryString("descr")), Request.Form("descr"), Request.QueryString("descr")).ToString
                Dim strSQL As String = "select " & strFieldId & " from " & strTableId & " where " & strFieldId & " = '" & strValue & "'"
                Dim tblCheck As DataTable = g_IO_Execute_SQL(strSQL, False)
                If tblCheck.Rows.Count > 0 Then
                    ' need to inform user that validation failed
                    lblMessage.Text = "%%" & strDescr & ": " & strValue & " is already in use. Please try again.%%"
                Else
                    ' not found, good to go!
                    lblMessage.Text = ""
                End If
            Case "clearSearchFilterSession"
                ' this code was never getting hit?!?!
                Dim strSessPrefix As String = IIf(IsNothing(Request.QueryString("sesPrefix")), Request.Form("sesPrefix"), Request.QueryString("sesPrefix"))
                Session.Remove(strSessPrefix & "searchFilter")
            Case "SetSearchSessions"
                SetSearchSessions()
            Case "removeSearchSessions"
                removeSearchSessions()
        End Select
    End Sub

    Private Sub SetSearchSessions()
        'andOr:          arrSearchAndOr[i].value,
        '                            field: arrSearchField[i].value,
        '                            logic: arrSearchLogic[i].value,
        '                            Text: arrSearchText[i].value,
        '                            sid:  (i + 1)
        Dim strAndOr As String = IIf(IsNothing(Request.QueryString("andOr")), Request.Form("andOr"), Request.QueryString("andOr"))
        Dim strField As String = IIf(IsNothing(Request.QueryString("Field")), Request.Form("Field"), Request.QueryString("Field"))
        Dim strLogic As String = IIf(IsNothing(Request.QueryString("Logic")), Request.Form("Logic"), Request.QueryString("Logic"))
        Dim strText As String = IIf(IsNothing(Request.QueryString("Text")), Request.Form("Text"), Request.QueryString("Text"))
        Dim strSessPrefix As String = IIf(IsNothing(Request.QueryString("sesPrefix")), Request.Form("sesPrefix"), Request.QueryString("sesPrefix"))
        Dim strSearchCritIndex As String = IIf(IsNothing(Request.QueryString("sid")), Request.Form("sid"), Request.QueryString("sid"))

        Session.Remove(strSessPrefix & "searchFilter")
        Session(strSessPrefix & "SearchAndOr" & strSearchCritIndex) = strAndOr
        Session(strSessPrefix & "SearchField" & strSearchCritIndex) = strField
        Session(strSessPrefix & "SearchLogic" & strSearchCritIndex) = strLogic
        Session(strSessPrefix & "SearchText" & strSearchCritIndex) = strText

    End Sub

    Private Sub removeSearchSessions()
        Dim strSessPrefix As String = IIf(IsNothing(Request.QueryString("sesPrefix")), Request.Form("sesPrefix"), Request.QueryString("sesPrefix"))
        Dim strSearchCritIndex As String = IIf(IsNothing(Request.QueryString("sid")), Request.Form("sid"), Request.QueryString("sid"))
        Dim blnAll As Boolean = IIf(IsNothing(Request.QueryString("blnAll")), Request.Form("blnAll"), Request.QueryString("blnAll"))
        If blnAll Then
            Session.Remove(strSessPrefix & "searchFilter")
            For i = 0 To 4
                If i > 0 Then
                    Session.Remove(strSessPrefix & "SearchAndOr" & i)
                End If
                Session.Remove(strSessPrefix & "SearchField" & i)
                Session.Remove(strSessPrefix & "SearchLogic" & i)
                Session.Remove(strSessPrefix & "SearchText" & i)
            Next
        Else
            Session.Remove(strSessPrefix & "searchFilter")
            Session.Remove(strSessPrefix & "SearchAndOr" & strSearchCritIndex)
            Session.Remove(strSessPrefix & "SearchField" & strSearchCritIndex)
            Session.Remove(strSessPrefix & "SearchLogic" & strSearchCritIndex)
            Session.Remove(strSessPrefix & "SearchText" & strSearchCritIndex)
        End If


    End Sub


    Private Sub LoadUpdateArea(ByVal TableName As String, ByVal RECID As String)

        'Dim tblColumns = GetColumns(TableName)
        g_GetColumns(TableName, m_nvcColumnLength, m_nvcColumnType, m_nvcColumnDescription, m_nvcColumnDDLTableName,
                     m_nvcColumnDDLValue, m_nvcColumnDDLText, m_nvcColumnPwd, m_nvcColumnShowInGrid, m_nvcColumnShowInPopup, m_nvcHidden,
                     m_nvcDisabled, m_nvcColumnIndex, m_nvcColumnRequired, m_nvcColumnEmail, m_nvcColumnPhone,
                     m_nvcColumnTotal, m_nvcColumnDisplayLocn, m_nvcColumnUnique, m_nvcColumnNames,
                     m_nvcColumnRegExpPattern, m_nvcColumnRegExpMessage, m_nvcMinValue, m_nvcMaxValue, m_nvcColumnDefaultValue,
                     m_nvcaSign, m_nvcpSign, m_nvcPercentage, m_nvcShowSeconds, m_nvcColumnDDLFilter, m_nvcImageMinMax, m_nvcImageHover)

        Dim strSQL As String = "Select * from " & TableName & " where recid= '" & RECID & "'"
        Dim tblListing = g_IO_Execute_SQL(strSQL, False)
        Dim rowListing As DataRow

        If tblListing.Rows.Count = 0 Then
            rowListing = tblListing.NewRow
            ' 6/30/15 T3 set default values from db on new record
            For Each strColumn As String In m_nvcColumnDefaultValue.Keys
                rowListing(strColumn) = m_nvcColumnDefaultValue(strColumn)
            Next
        Else
            rowListing = tblListing.Rows(0)
        End If

        For Each strColumn As String In m_nvcHidden.Keys
            If UCase(m_nvcHidden(strColumn)) = "TRUE" Then
                tblListing.Columns.Remove(strColumn)
            End If
        Next


        litUpdateArea.Text = BuildHTMLObject(rowListing, TableName, m_intMaxColumns)

        ' '' '' setup auto finding of column names for ajax call when coming back here, place div's on the site
        ' '' '' one for each column name that will be in client side javascript to find the fields available for update.  Use them to build the postback parameters and values
        ' '' ''   see javascript "SaveData" on the frmListManagement
        '' ''For Each strColumnName As String In m_nvcColumnDescription.Keys
        '' ''    litUpdateArea.Text &= "<div id=""div" & strColumnName & """ class=""columnback"" style=""display:none""></div>"
        '' ''Next

    End Sub
    Private Function BuildHTMLObject(ByVal RowIn As DataRow, ByVal TableName As String, ByVal ObjsDesiredPerRow_Max3 As Int16) As String

        ' form validation completed & tested
        '   - boolean: 05/14/15
        '   - text: 05/19/15
        '   - email
        '   - time: 05/26/15 
        '   - number: numeric, decimal, and integer OK
        '   - money
        ' Under development
        '   - number: finish building regexp in ModIO (left side w/ commas to fit length allowed) 
        '   - date (add calendar picker) 
        '   - datetime (add calendar picker, use formValidation.io)

        '   - try mask on phone# (mask seemed to mess up date try http://formvalidation.io/examples/mask/)

        '7/16/15 T3 Build literal for formvalidation programmatic js
        litFormValidation.Text = _
        "<script>" & _
        "   jQuery(document).ready(function () {" & _
        "       jQuery('#Form1')                " & _
        "          .formValidation({            " & _
        "              framework: 'bootstrap',  " & _
        "           #FVFIELDS# " & _
        "       })" & _
        "       #FVONFIELDS# " & _
        "       .on('success.form.fv', function (e) {" & _
        "            e.preventDefault();" & _
        "            if (checkRequiredFields('divModalAddUpdate') == false) {" & _
        "                if (SaveData()) { jQuery('#btnCancel').click(); }" & _
        "            }  " & _
        "       })   " & _
        "   }); " & _
        "   function revalidateCustom() { #FVREVALIDATE# }; " & _
        "</script>"
        Dim strFVFields As String = ""
        Dim strFVOnFields As String = ""
        Dim strFVFieldsDelim As String = ""
        Dim strFVRevalidateFields As String = ""
        'function revalidateCustom() {
        '    var curVal = $('#Money').val();
        '    $('#Money').val($('#Money').val().replace('$ ', '').replace(/,/g, ''));
        '    $('#Form1').formValidation('revalidateField', 'Money');
        '    $('#Money').val(curVal);
        '    var curVal = $('#NumberField').val();
        '    $('#NumberField').val($('#NumberField').val().replace('$ ', '').replace(/,/g, ''));
        '    $('#Form1').formValidation('revalidateField', 'NumberField');
        '    $('#NumberField').val(curVal);
        '}

        Dim strBootstrapOut As String = "<div id=""divModalContent"">"
        Dim strJavascriptFieldDirectory As String = ""

        Dim strBootStrapBoolean As String = _
                                        "<div class=""form-group"">" & _
                                        "   <label class=""col-xs-3 col-sm-3 control-label hidden-xs"">#FieldLabel#</label>" & _
                                        "   <div class="" col-xs-9 col-sm-9"">" & _
                                        "   <div class=""checkbox"">" & _
                                        "       <label> " & _
                                        "       <input type=""checkbox"" id=""#FieldName#"" name=""#FieldName#"" #checked#  value=""#Value#""" & _
                                        "       class=""#required#"" #validate# #disabled#  reqMsg=""Invalid #FieldName#""  />" & _
                                        "       <span class=""visible-xs T3bootstrapBoldLabel"">#FieldLabel#</span>" & _
                                        "       </label>" & _
                                        "   </div>" & _
                                        "   </div>" & _
                                        "</div>"
        ' need to look into way to determine if we need a textarea or input (based on field length or ext prop??)
        Dim strBootStrapText As String = _
                                        "<div class=""form-group"">" & _
                                        "   <label for=""#FieldName#"" class=""control-label col-sm-3"">&nbsp;#FieldLabel#</label>" & _
                                        "   <div class=""col-sm-9"">" & _
                                        "       <input type=""text"" id=""#FieldName#"" name=""#FieldName#"" class=""form-control #required#"" #unique# #validate#  value=""#Value#"" reqMsg=""Invalid #FieldName#"" maxlength=""#MAX#"" style=""max-width:#WIDTH#"" #disabled# />" & _
                                        "   </div>" & _
                                        "</div>"
        '<textarea id="TextArea1" cols="20" rows="2"></textarea>
        ' need to look into way to determine if we need a textarea or input (based on field length or ext prop??)
        Dim strBootStrapTextArea As String = _
                                        "<div class=""form-group"">" & _
                                        "   <label for=""#FieldName#"" class=""control-label col-sm-3"">&nbsp;#FieldLabel#</label>" & _
                                        "   <div class=""col-sm-9"">" & _
                                        "       <textarea id=""#FieldName#""  name=""#FieldName#"" cols=""20"" rows=""4"" class=""form-control #required#"" #unique# #validate#   reqMsg=""Invalid #FieldName#""  maxlength=""#MAX#""  style=""max-width:#WIDTH#""  #disabled# >#Value#</textarea>" & _
                                        "   </div>" & _
                                        "</div>"
        Dim strBootStrapNum As String = _
                                         "<div class=""form-group"">" & _
                                        "   <label for=""#FieldName#"" class=""control-label col-sm-3"">&nbsp;#FieldLabel#</label>" & _
                                        "   <div class=""col-sm-9"">" & _
                                        "        <input type=""text""  id=""#FieldName#"" name=""#FieldName#"" #validate# class=""form-control #required#"" #unique#   value=""#Value#"" reqMsg=""Invalid #FieldName#"" maxlength=""#MAX#""  style=""max-width:#WIDTH#"" #disabled# " & _
                                        "        />" & _
                                        "   </div>" & _
                                        "</div>" & _
                                        "<script> " & _
                                        "       jQuery('##FieldName#').autoNumeric('init', {mDec: #FieldmDec#}); " & _
                                        "</script>"

        Dim strBootStrapMoney As String = _
                                         "<div class=""form-group"">" & _
                                        "   <label for=""#FieldName#"" class=""control-label col-sm-3"">&nbsp;#FieldLabel#</label>" & _
                                        "   <div class=""col-sm-9"">" & _
                                        "        <input type=""text"" id=""#FieldName#"" name=""#FieldName#"" #validate# class=""form-control #required#"" #unique# #validate#  value=""#Value#"" reqMsg=""Invalid #FieldName#"" maxlength=""#MAX#"" style=""max-width:#WIDTH#"" #disabled# " & _
                                        "        />" & _
                                        "   </div>" & _
                                        "</div>" & _
                                        "<script> " & _
                                        "       jQuery('##FieldName#').autoNumeric('init', {aSep: ',', aDec: '.', aSign: '$ '}); " & _
                                        "</script>"
        'id=""#FieldName#"" maxlength=""#MAX#"" style=""max-width:#WIDTH#"" was on input field - using FV.IO we moved ID to surrounding DIV
        Dim strBootStrapDate As String =
                                        "   <div class=""form-group"">" &
                                        "        <label for=""#FieldName#"" class=""col-xs-3 control-label"">&nbsp;#FieldLabel#</label>" &
                                        "       <div class=""col-xs-5 date"">" &
                                        "       <div class=""input-group input-append date"" id=""div#FieldName#"">" &
                                        "            <input type=""text"" id=""#FieldName#"" name=""#FieldName#"" class=""form-control #required# #disabled#"" #unique# #validate# value=""#Value#"" reqMsg=""Invalid #FieldName#"" #disabled# placeholder=""MM/DD/YYYY"" " &
                                        "            />" &
                                        "           <span id=""spn#FieldName#"" class=""input-group-addon add-on ""><span class=""glyphicon glyphicon-calendar ""></span></span>" &
                                        "       </div>" &
                                        "       </div>" &
                                        "   </div>" &
                                        "<script> " &
                                        "jQuery(document).ready(function() {" &
                                        "   if (jQuery('##FieldName#').hasClass('disabled')) { " &
                                        "       jQuery('#spn#FieldName#').addClass('hidden');" &
                                        "   } else { " &
                                        "       jQuery('#div#FieldName#').datepicker({ format: 'mm/dd/yyyy', autoclose: true})" &
                                                ".on('changeDate', function (e) {" &
                                                    "    jQuery('#Form1').formValidation('revalidateField', '#FieldName#');" &
                                                "});" &
                                        "   }" &
                                        "});" &
                                        "</script>"

        Dim strBootStrapDateTime As String =
                                        "   <div class=""form-group"">" &
                                        "        <label for=""#FieldName#"" class=""col-xs-3 control-label"">&nbsp;#FieldLabel#</label>" &
                                        "       <div class=""col-xs-5 date"">" &
                                        "       <div class=""input-group input-append date"" id=""div#FieldName#"">" &
                                        "            <input type=""text"" id=""#FieldName#"" name=""#FieldName#"" class=""form-control #required# #disabled#"" #unique# #validate# value=""#Value#"" reqMsg=""Invalid #FieldName#"" #disabled# placeholder=""MM/DD/YYYY HH:MM AM"" " &
                                        "            />" &
                                        "           <span id=""spn#FieldName#"" class=""input-group-addon add-on ""><span class=""glyphicon glyphicon-calendar ""></span></span>" &
                                        "       </div>" &
                                        "       </div>" &
                                        "   </div>" &
                                        "<script> " &
                                        "jQuery(document).ready(function() {" &
                                        "   if (jQuery('##FieldName#').hasClass('disabled')) { " &
                                        "       jQuery('#spn#FieldName#').addClass('hidden');" &
                                        "   } else { " &
                                        "       jQuery('#div#FieldName#').datetimepicker();" &
                                        "       $('#div#FieldName#').on('dp.change dp.show', function(e) {" &
                                        "           $('#Form1').formValidation('revalidateField', '#div#FieldName#'); " &
                                        "       });" &
                                        "   }" &
                                        "});" &
                                        "</script>"
        ' 9/18/14 cp/cpb
        Dim strBootStrapTime As String =
                                        "   <div Class=""form-group"">" &
                                        "        <label For=""#FieldName#"" Class="" col-sm-3 control-label"">&nbsp;#FieldLabel#</label>" &
                                        "       <div Class=""col-sm-9"">" &
                                        "           <div Class=""form-inline"">" &
                                        "               <div Class=""form-group"">" &
                                        "                   <div Class=""col-sm-2"">" &
                                        "                      <label Class=""sr-only"" For=""#FieldName#__hr"">Choose Hour</label>" &
                                        "                      <Select id=""#FieldName#__hr"" name=""#FieldName#__hr"" width=""75px"" onchange=""setHiddenTime('#FieldName#');"" class=""form-control #required#"" reqMsg=""Invalid #FieldName# Hour"" style=""padding-left:5px;padding-right:5px"" #validate#  #disabled# >" &
                                        "                       #HourOptions#" &
                                        "                      </select>" &
                                        "                   </div>" &
                                        "               </div>" &
                                        "               <div class=""form-group"">" &
                                        "                   <div class=""col-sm-1"">" &
                                        "                      <label class=""sr-only""></label>" &
                                        "                      <span width=""10px"" class=""hidden-xs"">:</span>" &
                                        "                   </div>" &
                                        "               </div>" &
                                        "              <div class=""form-group"">" &
                                        "                   <div class=""col-sm-2"">" &
                                        "                      <label class=""sr-only"" for=""#FieldName#__min"">Choose Minute</label>" &
                                        "                      <select id=""#FieldName#__min"" name=""#FieldName#__min""  width=""75px"" onchange=""setHiddenTime('#FieldName#');"" class=""form-control #required#"" reqMsg=""Invalid #FieldName# Minutes"" style=""padding-left:5px;padding-right:5px""  #validate#  #disabled# >" &
                                        "                       #MinOptions#" &
                                        "                      </select>" &
                                        "                   </div>" &
                                        "               </div>" &
                                        "              <div class=""form-group #ShowHideSeconds# "">" &
                                        "                   <div class=""col-sm-2"">" &
                                        "                      <label class=""sr-only"" for=""#FieldName#__sec"">Choose Seconds</label>" &
                                        "                      <select id=""#FieldName#__sec"" name=""#FieldName#__sec""  width=""75px"" onchange=""setHiddenTime('#FieldName#');"" class=""form-control #required#"" reqMsg=""Invalid #FieldName# Seconds"" style=""padding-left:5px;padding-right:5px""  #validate#  #disabled# >" &
                                        "                       #SecOptions#" &
                                        "                      </select>" &
                                        "                   </div>" &
                                        "               </div>" &
                                        "              <div class=""form-group"">" &
                                        "                    <div class=""col-sm-2 "">" &
                                        "                      <label class=""sr-only"" for=""#FieldName#__ampm"">Choose AM/PM</label>" &
                                        "                       <select id=""#FieldName#__ampm""  name=""#FieldName#__ampm""  width=""75px"" onchange=""setHiddenTime('#FieldName#');"" class=""form-control"" style=""padding-left:5px;padding-right:5px""  #validate# #disabled# >" &
                                        "                        #AmPmOptions#" &
                                        "                       </select>" &
                                        "                    </div>" &
                                        "               </div>" &
                                        "           </div>" &
                                        "       </div>" &
                                        "</div>" &
                                        "<div class=""hidden"">" &
                                        "       <input type=""text"" id=""#FieldName#"" value=""#Value#"" />" &
                                        "</div>"

        Dim strBootStrapDDL As String = _
                                        "   <div class=""form-group"">" & _
                                        "        <label for=""#FieldName#"" class="" col-sm-3 control-label"">&nbsp;#FieldLabel#</label>" & _
                                        "       <div class=""col-sm-9"">" & _
                                        "        <select id=""#FieldName#"" name=""#FieldName#"" class=""form-control #required#"" #validate# style=""width:#WIDTH#""  reqMsg=""Invalid #FieldName#""  #disabled# >" & _
                                        "         #Options#" & _
                                        "       </select>" & _
                                        "    </div>" & _
                                        "</div>"
        ' 03/18/15 cpb add password format validation
        Dim strBootStrapPassword As String = _
                                        "   <div class=""form-group"">" & _
                                        "        <label for=""#FieldName#"" class="" col-sm-3 control-label"">&nbsp;#FieldLabel#</label>" & _
                                        "       <div class=""col-sm-9"">" & _
                                        "        <input type=""password"" id=""#FieldName#"" name=""#FieldName#"" class=""form-control #required#"" #validate# reqMsg=""#FieldName# Required"" value=""""  maxlength=""#MAX#"" style=""max-width:#WIDTH#""  #disabled# " & _
                                        "           data-fv-field=""#FieldName#""" & _
                                        "           data-fv-regexp=""true""" & _
                                        "           data-fv-regexp-regexp=""^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$""" & _
                                        "           data-fv-regexp-message=""Password must be between 6 to 20 characters and contain at least one numeric digit, one uppercase and one lowercase letter.""" & _
                                        "       />" & _
                                        "    </div>" & _
                                        "</div>" & _
                                        "   <div class=""form-group"">" & _
                                        "        <label for=""verifyPassword"" class="" col-sm-3 control-label""><span class=""requiredFieldIndicator"">*</span>&nbsp;Re-Enter #FieldLabel#</label>" & _
                                        "       <div class=""col-sm-9"">" & _
                                        "        <input type=""password"" id=""verifyPassword"" name=""verifyPassword"" class=""form-control required"" reqMsg=""#FieldName# Required"" value="""" maxlength=""#MAX#"" style=""max-width:#WIDTH#""  #disabled# " & _
                                        "           data-fv-field=""verifyPassword""" & _
                                        "           data-fv-identical=""true""" & _
                                        "           data-fv-identical-field = ""#FieldName#""" & _
                                        "           data-fv-identical-message=""The password and its confirm are not the same.""" & _
                                        "        />" & _
                                        "    </div>" & _
                                        "</div>"
        ' 10/6/14 T3
        ' 4/28/15 T3 removed onblur="validateEmail(this) now that we are using formvalidation.io"
        Dim strBootStrapEmail As String = _
                                        "   <div class=""form-group"">" & _
                                        "        <label for=""#FieldName#"" class="" col-sm-3 control-label"">&nbsp;#FieldLabel#</label>" & _
                                        "       <div class=""col-sm-9"">" & _
                                        "        <input type=""email"" id=""#FieldName#"" name=""#FieldName#"" class=""form-control #required#"" reqMsg=""Invalid #FieldName#""   #validate# #unique# value=""#Value#""  maxlength=""#MAX#"" style=""max-width:#WIDTH#""  #disabled# " & _
                                        "         data-fv-emailaddress=""true""" & _
                                        "         data-fv-emailaddress-message=""The value is not a valid email address format.""" & _
                                        "        </input>" & _
                                        "       </div>" & _
                                        "    </div>"

        Dim strBootStrapPhone As String = _
                                        "   <div class=""form-group"">" & _
                                        "        <label for=""#FieldName#"" class="" col-sm-3 control-label"">&nbsp;#FieldLabel#</label>" & _
                                        "       <div class=""col-sm-9"" > " & _
                                        "        <input type=""phone"" id=""#FieldName#"" name=""#FieldName#"" type=""text"" placeholder=""(###)###-####"" class=""form-control #required#"" #validate# #unique# value=""#Value#""  maxlength=""#MAX#"" style=""max-width:#WIDTH#""  #disabled# " & _
                                         "         data-fv-phone=""true"" data-fv-phone-message=""The value is not valid phone number."" " & _
                                         "         data-fv-phone-country=""US"" />" & _
                                        "       </div>" & _
                                        "    </div>" & _
                                        " <script> " & _
                                        "       jQuery(""##FieldName#"").mask(""(999)999-9999"");" & _
                                        "    </script>"

        Dim strBootstrapRow As String = ""
        Dim intPadding As Integer = 0
        'Dim i As Integer = 0

        '09/19/14 - T3
        'Check to see if a Master Table exists for possible vw 
        'Will disable non master table fields
        'MasterTable ext. prop will be used on Views 
        Dim strMasterTable As String = g_GetTableExtProperty(TableName, "MasterTable")
        Dim strMasterTblFields As String = ""
        If IsNothing(strMasterTable) OrElse strMasterTable = "" Then
            strMasterTable = ""
        Else
            Dim strSql As String = "select isnull(cols.referenced_database_name,'') as dbName, cols.referenced_entity_name as tableName, " & _
                "cols.referenced_minor_name as columnName from sys.sql_expression_dependencies objs outer apply sys.dm_sql_referenced_entities " & _
                "( OBJECT_SCHEMA_NAME(objs.referencing_id) + N'.' + object_name(objs.referencing_id), N'OBJECT' ) as cols " & _
                "where objs.referencing_id = object_id('" & TableName & "') and cols.referenced_minor_name is not null"

            Dim tblViewColumns As DataTable = g_IO_Execute_SQL(strSql, False)
            If tblViewColumns.Rows.Count = 0 Then
                Dim tblAltColumns As DataTable = g_IO_Execute_SQL("select TOP 1 * from " & strMasterTable, False)
                For Each col As DataColumn In tblAltColumns.Columns
                    strMasterTblFields &= "," & Trim(col.ColumnName)
                Next
            Else
                For Each rowColumnName As DataRow In tblViewColumns.Rows
                    'Create list of fields in Master Table from current database only (reference: WSDC- Ortho linked with Improvis)
                    If rowColumnName("dbName") = "" And UCase(rowColumnName("TableName")) = UCase(strMasterTable) Then
                        strMasterTblFields &= "," & Trim(rowColumnName("columnName"))
                    End If
                Next
            End If

            strMasterTblFields &= ","
        End If
        For Each strColumnName As String In m_nvcColumnDescription.Keys
            If UCase(m_nvcHidden(strColumnName)) = "TRUE" Or UCase(m_nvcColumnShowInPopup(strColumnName)) = "FALSE" Then
            Else

                'If i Mod ObjsDesiredPerRow_Max3 + 1 = 1 Then
                '    strBootstrapOut &= "<div class=""row pull-left"" style=""width: 95%"" >"
                '    strBootStrapRowEndTag = "</div><br><br>"
                'End If

                Dim strLabel As String = ""
                Dim strBootstrapCurrent As String = ""
                If m_nvcColumnDescription(strColumnName) <> "" Then
                    strLabel = m_nvcColumnDescription(strColumnName).Replace("_", " ")
                Else
                    strLabel = strColumnName.Replace("_", " ")
                End If

                '09/29/14 - check required
                If UCase(m_nvcColumnRequired(strColumnName)) = "TRUE" Then
                    strLabel = "<span class=""requiredFieldIndicator"">*</span>" & "&nbsp;" & strLabel
                End If



                '09/19/14 -T3 If column is not in master table of view, disable it
                Dim blnMasterField As Boolean = True
                If strMasterTblFields = "" Then
                Else
                    If strMasterTblFields.Contains("," & Trim(strColumnName) & ",") Then
                    Else
                        m_nvcDisabled(strColumnName) = "TRUE"
                        blnMasterField = False
                    End If
                End If

                ' 01/06/15 remove columnback class if field is disabled via ext prop
                Dim strColumnBack As String = ""
                If m_nvcDisabled(strColumnName) = "TRUE" Then
                Else
                    strColumnBack = "columnback "
                End If

                ' 4/10/15 T3 declare validation variable
                Dim strValidation As String = ""

                If m_nvcColumnType(strColumnName).Contains("tinyint") Then

                    ' checkbox or radio
                    If IsDBNull(RowIn(strColumnName)) Then
                        RowIn(strColumnName) = 0
                    End If

                    strBootstrapCurrent = strBootStrapBoolean.Replace("#checked#", IIf(RowIn(strColumnName) = 0, "", " checked=""checked"" "))
                    strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "boo"" style=""display:none""></div>"
                ElseIf m_nvcColumnType(strColumnName).Contains("mon") Then

                    'money
                    If IsDBNull(RowIn(strColumnName)) Then
                        RowIn(strColumnName) = 0
                    End If

                    If m_nvcMinValue(strColumnName) <> "" Then
                        strFVFields &= strFVFieldsDelim &
                        " '" & strColumnName & "': {  " &
                        "        validators: {  " &
                        "            between: { " &
                        "                min: " & m_nvcMinValue(strColumnName) & ", " &
                        "                max: " & m_nvcMaxValue(strColumnName) & ", " &
                        "                message: 'The amount must be between " & m_nvcMinValue(strColumnName) & " and " & m_nvcMaxValue(strColumnName) & "'" &
                        "           } " &
                        "       }     " &
                        "   }     "
                        strFVFieldsDelim = ","
                        strFVOnFields &=
                        ".on('keyup', '[name=""" & strColumnName & """]', function (e) {" &
                        "    curVal = $('#" & strColumnName & "').val();" &
                        "    $('#" & strColumnName & "').val($('#" & strColumnName & "').val().replace('$ ', '').replace(/,/g, ''));" &
                        "    $('#Form1').formValidation('revalidateField', '" & strColumnName & "');" &
                        "    $('#" & strColumnName & "').val(curVal);" &
                        "})"
                        strFVRevalidateFields &=
                        "    curVal = $('#" & strColumnName & "').val();" &
                        "    $('#" & strColumnName & "').val($('#" & strColumnName & "').val().replace('$ ', '').replace(/,/g, ''));" &
                        "    $('#Form1').formValidation('revalidateField', '" & strColumnName & "');" &
                        "    $('#" & strColumnName & "').val(curVal);"
                    End If

                    strBootstrapCurrent = strBootStrapMoney.Replace("#Value#", FormatCurrency(RowIn(strColumnName))).Replace("$", "$ ")
                    'strBootstrapCurrent = strBootStrapMoney '.Replace("#Value#", RowIn(strColumnName))
                    If blnMasterField = True Then
                        strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "num"" style=""display:none""></div>"
                    End If

                ElseIf m_nvcColumnType(strColumnName).Contains("int") Or m_nvcColumnType(strColumnName).Contains("num") Or m_nvcColumnType(strColumnName).Contains("dec") _
                    Or m_nvcColumnType(strColumnName).Contains("flo") Then

                    ' number
                    If IsNothing(m_nvcColumnDDLTableName(strColumnName)) Then
                        If IsDBNull(RowIn(strColumnName)) Then
                            RowIn(strColumnName) = 0
                        End If
                        If m_nvcMinValue(strColumnName) <> "" Then
                            strFVFields &= strFVFieldsDelim &
                            " '" & strColumnName & "': {  " &
                            "        validators: {  " &
                            "            between: { " &
                            "                min: " & m_nvcMinValue(strColumnName) & ", " &
                            "                max: " & m_nvcMaxValue(strColumnName) & ", " &
                            "                message: 'The amount must be between " & m_nvcMinValue(strColumnName) & " and " & m_nvcMaxValue(strColumnName) & "'" &
                            "           } " &
                            "       }     " &
                            "   }     "
                            strFVFieldsDelim = ","
                            strFVOnFields &=
                            ".on('keyup', '[name=""" & strColumnName & """]', function (e) {" &
                            "    var curVal = $('#" & strColumnName & "').val();" &
                            "    $('#" & strColumnName & "').val($('#" & strColumnName & "').val().replace(/,/g, ''));" &
                            "    $('#Form1').formValidation('revalidateField', '" & strColumnName & "');" &
                            "    $('#" & strColumnName & "').val(curVal);" &
                            "})"
                            strFVRevalidateFields &=
                            "    curVal = $('#" & strColumnName & "').val();" &
                            "    $('#" & strColumnName & "').val($('#" & strColumnName & "').val().replace(/,/g, ''));" &
                            "    $('#Form1').formValidation('revalidateField', '" & strColumnName & "');" &
                            "    $('#" & strColumnName & "').val(curVal);"
                        End If
                        Dim intDec As Integer = 0
                        Dim arrDec() As String = Split(m_nvcMaxValue(strColumnName), ".")
                        If arrDec.Count > 1 Then
                            intDec = arrDec(1).Length
                        End If

                        strBootstrapCurrent = strBootStrapNum.Replace("#FieldmDec#", intDec)
                        If blnMasterField = True Then
                            strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "num"" style=""display:none""></div>"
                        End If

                    Else
                        'DDL
                        ' 05/13/15 cpb - added ability to show multiple fields from DDL table
                        Dim strDDLOptions As String = ""
                        If IsDBNull(RowIn(strColumnName)) Then
                            RowIn(strColumnName) = -1
                            ' 03/18/15 cpb this was causing selection(s) and choose an option to be in the dropdown on a new record
                            'strDDLOptions &= "<option value>Selection(s)</option>"
                        End If
                        Dim strWhere As String = ""
                        Dim arrFieldFilter() = Split(m_nvcColumnDDLFilter(strColumnName), "~~")
                        If arrFieldFilter.Length > 1 Then
                            Dim strFieldFromMasterTable As String = arrFieldFilter(0)
                            Dim strLogic As String = arrFieldFilter(1)
                            Dim strFieldFromChildTable As String = arrFieldFilter(2)
                            If strFieldFromChildTable.IndexOf("'") = 0 Or IsNumeric(strFieldFromChildTable) Then
                                ' Value was sent in
                                strWhere = " where " & strFieldFromChildTable & " " & strLogic & " " & strFieldFromMasterTable
                            Else
                                If IsDBNull(RowIn(strFieldFromMasterTable)) Then
                                Else
                                    If RowIn(strFieldFromMasterTable) = "" Or RowIn(strFieldFromMasterTable) = "-1" Then
                                    Else
                                        strWhere = " where  " & strFieldFromChildTable & strLogic & "'" & RowIn(strFieldFromMasterTable) & "'"
                                    End If
                                End If
                            End If
                        End If

                        Dim strSQL As String = " Select " & m_nvcColumnDDLValue(strColumnName) & " , " & m_nvcColumnDDLText(strColumnName) &
                            " FROM " & m_nvcColumnDDLTableName(strColumnName) & strWhere & " order by " & m_nvcColumnDDLText(strColumnName)
                        Dim tblDDLList As DataTable = g_IO_Execute_SQL(strSQL, False)
                        Dim intMaxWidth As Integer = 20

                        '''' & row(m_nvcColumnDDLText(strColumnName)) & """" & _
                        ''''IIf(CStr(RowIn(strColumnName)) = CStr(row(m_nvcColumnDDLValue(strColumnName))), " selected=""selected"" ", "") & _
                        strDDLOptions &= "<option value>Choose an Option</option>"
                        Dim arrDDLText As String() = Split(m_nvcColumnDDLText(strColumnName), ",")
                        Dim strDDLDelim As String = ""
                        Dim blnSelected As Boolean = False
                        For Each row In tblDDLList.Rows
                            blnSelected = False
                            strDDLOptions &= "<option value = """ & row(m_nvcColumnDDLValue(strColumnName)) & """ text="""
                            For x = 0 To arrDDLText.Length - 1
                                strDDLOptions &= strDDLDelim & row(arrDDLText(x))
                                strDDLDelim = ","
                                '05/26/15 replaced arrDDLText(x) with row(m_nvcColumnDDLValue(strColumnName))
                                If CStr(RowIn(strColumnName)) = row(m_nvcColumnDDLValue(strColumnName)) Then 'arrDDLText(x)
                                    blnSelected = True
                                End If
                            Next
                            strDDLOptions &= """" &
                            IIf(blnSelected, " selected=""selected"" ", "") &
                                                ">"         ' & row(m_nvcColumnDDLText(strColumnName)) & "</option>"
                            strDDLDelim = ""
                            For x = 0 To arrDDLText.Length - 1
                                strDDLOptions &= strDDLDelim & row(arrDDLText(x))
                                strDDLDelim = ","
                                If Trim(row(arrDDLText(x))).Length > intMaxWidth Then
                                    intMaxWidth = Trim(row(arrDDLText(x))).Length
                                End If
                            Next
                            'If Trim(row(m_nvcColumnDDLText(strColumnName))).Length > intMaxWidth Then
                            '    intMaxWidth = Trim(row(m_nvcColumnDDLText(strColumnName))).Length
                            'End If
                        Next
                        m_nvcColumnLength(strColumnName) = intMaxWidth
                        intPadding = 25
                        strBootstrapCurrent = strBootStrapDDL.Replace("#Options#", strDDLOptions)
                        If blnMasterField = True Then
                            strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "sel"" style=""display:none""></div>"
                        End If
                    End If
                ElseIf m_nvcColumnType(strColumnName).Contains("datetime") Then
                    ' datetime
                    Dim strValue As String = ""
                    If IsDBNull(RowIn(strColumnName)) Then
                    Else
                        strValue = Format(CDate(RowIn(strColumnName)), "MM/dd/yyyy hh:mm tt")
                        ' 11/25/15 cpb if date is SQL default, then set to "" for display
                        If strValue = "01/01/1900 12:00 AM" Then
                            strValue = ""
                        End If
                    End If
                    strBootstrapCurrent = strBootStrapDateTime.Replace("#Value#", strValue)
                    If blnMasterField = True Then
                        strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "dte"" style=""display:none""></div>"
                    End If
                    m_nvcColumnLength(strColumnName) = 22

                    'Form Validation IO Date
                    strFVFields &= strFVFieldsDelim &
                            " '" & strColumnName & "': {  " &
                             "        validators: {  "
                    ' 11/10/15 cpb - only add not empty message if the date is required.
                    If UCase(m_nvcColumnRequired(strColumnName)) = "TRUE" Then
                        strFVFields &=
                            "            notEmpty: { " &
                            "                message: 'The date is required.'" &
                           "           }, "
                    End If
                    strFVFields &=
                            "            date: { " &
                            "                format: 'MM/DD/YYYY h:m A'," &
                            "                message: 'Enter date like MM/DD/YYYY HH:MM AM'" &
                            "           } " &
                            "       }     " &
                            "   }     "
                    strFVFieldsDelim = ","

                ElseIf m_nvcColumnType(strColumnName).Contains("date") Then
                    ' date
                    Dim strValue As String = ""
                    If IsDBNull(RowIn(strColumnName)) Then
                    Else
                        strValue = Format(CDate(RowIn(strColumnName)), "MM/dd/yyyy")
                        ' 11/25/15 cpb if date is SQL default, then set to "" for display
                        If strValue = "01/01/1900" Then
                            strValue = ""
                        End If
                    End If
                    strBootstrapCurrent = strBootStrapDate.Replace("#Value#", strValue)
                    If blnMasterField = True Then
                        strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "dte"" style=""display:none""></div>"
                    End If
                    m_nvcColumnLength(strColumnName) = 10

                    'Form Validation IO Date
                    strFVFields &= strFVFieldsDelim &
                            " '" & strColumnName & "': {  " &
                             "        validators: {  "
                    ' 11/10/15 cpb - only add not empty message if the date is required.
                    If UCase(m_nvcColumnRequired(strColumnName)) = "TRUE" Then
                        strFVFields &=
                            "            notEmpty: { " &
                            "                message: 'The date is required.'" &
                           "           }, "
                    End If
                    strFVFields &=
                            "            date: { " &
                            "                format: 'MM/DD/YYYY'," &
                            "                message: 'The date is not a valid date.'" &
                            "           } " &
                            "       }     " &
                            "   }     "
                    strFVFieldsDelim = ","
                    'strFVOnFields &=
                    '".on('changeDate', function (e) {" &
                    '"    $('#Form1').formValidation('revalidateField', '" & strColumnName & "');" &
                    '"})"
                ElseIf m_nvcColumnType(strColumnName).Contains("time") Then
                    ' time - added 9/18/14 cp/cpb
                    Dim arrTime() As String
                    Dim strHour As String = "00"
                    Dim strMinute As String = "00"
                    Dim strSecond As String = "00"
                    Dim strAMPM As String = "AM"
                    If IsDBNull(RowIn(strColumnName)) Then
                        strAMPM &= "<option value = ""AM"" selected=""selected"">AM</option>"
                        strAMPM &= "<option value = ""PM"">PM</option>"
                    Else
                        arrTime = Split(RowIn(strColumnName).ToString, ":")

                        If arrTime(0) > 12 Then
                            strHour = arrTime(0) - 12
                            strAMPM &= "<option value = ""AM"">AM</option>"
                            strAMPM &= "<option value = ""PM"" selected=""selected"">PM</option>"
                        Else
                            strHour = arrTime(0)
                            strAMPM &= "<option value = ""AM"" selected=""selected"">AM</option>"
                            strAMPM &= "<option value = ""PM"">PM</option>"
                        End If
                        strMinute = arrTime(1)
                        strSecond = arrTime(2)
                    End If

                    '05/26/15 Hour T3
                    Dim strHours As String = ""
                    For hr = 0 To 12
                        If hr = 0 Then
                            strHours &= "<option value>&nbsp;&nbsp;</option>"
                        Else
                            strHours &= "<option value = """ & CStr(hr).PadLeft(2, "0") & """"
                            If hr = CInt(strHour) Then
                                strHours &= " selected=""selected"" "
                            End If
                            strHours &= ">" & CStr(hr).PadLeft(2, "0") & "</option>"
                        End If
                    Next
                    '05/26/15 Minute T3
                    Dim strMin As String = ""
                    strMin &= "<option value"
                    If strHour = "00" Then
                        strMin &= " selected=""selected"" "
                    End If
                    strMin &= ">&nbsp;&nbsp;</option>"

                    For min = 0 To 59
                        strMin &= "<option value = """ & CStr(min).PadLeft(2, "0") & """"
                        If min = CInt(strMinute) And CInt(strHour) > 0 Then
                            strMin &= " selected=""selected"" "
                        End If
                        strMin &= ">" & CStr(min).PadLeft(2, "0") & "</option>"
                    Next

                    Dim strSec As String = ""
                    strSec &= "<option value"
                    If strHour = "00" Then
                        strSec &= " selected=""selected"" "
                    End If
                    strSec &= ">&nbsp;&nbsp;</option>"
                    For sec = 0 To 59
                        strSec &= "<option value = """ & CStr(sec).PadLeft(2, "0") & """"
                        If sec = CInt(strSecond) And CInt(strHour) > 0 Then
                            strSec &= " selected=""selected"" "
                        End If
                        strSec &= ">" & CStr(sec).PadLeft(2, "0") & "</option>"
                    Next

                    Dim strShowHideSeconds As String = ""
                    If IsNothing(m_nvcShowSeconds(strColumnName)) Then
                        strShowHideSeconds = " hidden "
                    Else
                        If UCase(m_nvcShowSeconds(strColumnName)) = "TRUE" Then
                        Else
                            strShowHideSeconds = " hidden "
                        End If
                    End If

                    ' 4/10/15 T3 set validation declaratives
                    If UCase(m_nvcColumnRequired(strColumnName)) = "TRUE" Then
                        strValidation = "data-fv-notempty=""true"" data-fv-notempty-message=""Req"" "
                    End If
                    strBootstrapCurrent = strBootStrapTime.Replace("#HourOptions#", strHours).Replace("#MinOptions#", strMin).Replace("#SecOptions#", strSec).Replace("#AmPmOptions#", strAMPM).Replace("#ShowHideSeconds#", strShowHideSeconds)
                    strBootstrapCurrent = strBootstrapCurrent.Replace("#Value#", RowIn(strColumnName).ToString).Replace("#validate#", strValidation)
                    'Do not remove, when you submit form, gives list of fields that are going to be updated
                    strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "tme"" style=""display:none""></div>"
                Else
                    ' everything else is char (right or wrong)
                    If IsNothing(m_nvcColumnDDLTableName(strColumnName)) Then
                        If IsDBNull(RowIn(strColumnName)) Then
                            RowIn(strColumnName) = ""
                        End If

                        Dim blnEmail As Boolean = Not IsNothing(m_nvcColumnEmail(strColumnName))
                        Dim blnPhone As Boolean = Not IsNothing(m_nvcColumnPhone(strColumnName))
                        Dim blnPwd As Boolean = Not IsNothing(m_nvcColumnPwd(strColumnName))

                        Select Case True
                            Case blnEmail
                                ' 10/6/14 email field
                                strBootstrapCurrent = strBootStrapEmail
                                If blnMasterField = True Then
                                    strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "txt"" style=""display:none""></div>"
                                End If
                            Case blnPhone
                                ' 10/6/14 phone field
                                strBootstrapCurrent = strBootStrapPhone
                                If blnMasterField = True Then
                                    strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "txt"" style=""display:none""></div>"
                                End If
                            Case blnPwd
                                ' 10/03/14 - include password on new record entry only
                                If m_intRecId = -1 Then
                                    strBootstrapCurrent = strBootStrapPassword
                                    If blnMasterField = True Then
                                        strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "txt"" style=""display:none""></div>"
                                    End If
                                End If
                            Case Else
                                '--we know this is a text box
                                'Determine if textarea
                                If m_nvcColumnLength(strColumnName) > 50 Then
                                    'Needs to be a textarea
                                    strBootstrapCurrent = strBootStrapTextArea
                                Else
                                    'Textbox
                                    strBootstrapCurrent = strBootStrapText
                                End If
                                If blnMasterField = True Then
                                    strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "txt"" style=""display:none""></div>"
                                End If
                        End Select
                    Else
                        'DDL
                        Dim strDDLOptions As String = ""
                        If IsDBNull(RowIn(strColumnName)) Then
                            RowIn(strColumnName) = -1
                            strDDLOptions &= "<option value>Selection(s)</option>"
                        End If
                        Dim strSQL As String = " Select " & m_nvcColumnDDLValue(strColumnName) & " , " & m_nvcColumnDDLText(strColumnName) & _
                            " FROM " & m_nvcColumnDDLTableName(strColumnName) & " order by " & m_nvcColumnDDLText(strColumnName)
                        Dim tblDDLList As DataTable = g_IO_Execute_SQL(strSQL, False)
                        Dim intMaxWidth As Integer = 20
                        strDDLOptions &= "<option value>Choose an Option</option>"
                        For Each row In tblDDLList.Rows
                            strDDLOptions &= "<option value = """ & row(m_nvcColumnDDLValue(strColumnName)) & """ text=""" & row(m_nvcColumnDDLText(strColumnName)) & """" & _
                                               IIf(CStr(RowIn(strColumnName)) = CStr(row(m_nvcColumnDDLValue(strColumnName))), " selected=""selected"" ", "") & _
                                                ">" & row(m_nvcColumnDDLText(strColumnName)) & "</option>"
                            If Trim(row(m_nvcColumnDDLText(strColumnName))).Length > intMaxWidth Then
                                intMaxWidth = Trim(row(m_nvcColumnDDLText(strColumnName))).Length
                            End If
                        Next
                        m_nvcColumnLength(strColumnName) = intMaxWidth
                        intPadding = 25
                        strBootstrapCurrent = strBootStrapDDL.Replace("#Options#", strDDLOptions)
                        If blnMasterField = True Then
                            strJavascriptFieldDirectory &= "<div id=""div" & strColumnName & """ class=""" & strColumnBack & "sel"" style=""display:none""></div>"
                        End If
                    End If

                End If

                Dim strWidth As String = CStr(m_nvcColumnLength(strColumnName) * 10 + 30 + intPadding)

                If CInt(strWidth) > 400 Then
                    strWidth = "400"
                End If
                strWidth &= "px"

                '09/29/14 T3 If required field, add attribute reqmsg for popup column display
                Dim strRequiredMsg As String = ""
                If m_nvcColumnDescription(strColumnName) <> "" Then
                    strRequiredMsg = m_nvcColumnDescription(strColumnName).Replace("_", " ")
                Else
                    strRequiredMsg = strColumnName.Replace("_", " ")
                End If

                If UCase(m_nvcColumnRequired(strColumnName)) = "TRUE" Then
                    ' 05/13/15 cpb fix setting of required field message for form validation
                    strRequiredMsg = strRequiredMsg & " Required"
                End If
                ' 1/9/15 cpb for view only mode--all fields are disabled.
                If m_strViewOnly = "view" Then
                    strBootstrapCurrent = strBootstrapCurrent.Replace("#disabled#", " disabled ")
                End If
                ' 4/10/15 T3 set validation declaratives
                If UCase(m_nvcColumnRequired(strColumnName)) = "TRUE" Then
                    strValidation = "data-fv-notempty=""true"" data-fv-notempty-message=""" & strRequiredMsg & """"
                End If

                ' 5/19/15 T3 Check for ext prop for RegEx defined
                If m_nvcColumnRegExpPattern(strColumnName) <> "" Then
                    strValidation &= " data-fv-regexp=""true"" data-fv-regexp-regexp=""" & m_nvcColumnRegExpPattern(strColumnName) & """"
                    If m_nvcColumnRegExpMessage(strColumnName) <> "" Then
                        strValidation &= " data-fv-regexp-message=""" & m_nvcColumnRegExpMessage(strColumnName) & """"
                    Else
                        strValidation &= " data-fv-regexp-message=""Invalid Entry."""
                    End If
                End If

                ' 7/14/15 Commented out bc will need to build up the formvalidation javascript 
                ' hard-coded now on ajaxGetData.aspx line 16


                'If m_nvcMinValue(strColumnName) <> "" Then
                '    If m_nvcColumnType(strColumnName).Contains("mon") Then
                '        'strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldMax#", "max = ""0""  data-fv-between-max=""" & m_nvcMaxValue(strColumnName) & """")
                '        'If m_nvcMinValue(strColumnName) < 0 Then
                '        '    strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldMin#", " min=""" & m_nvcMinValue(strColumnName) & """ data-fv-between-min=""" & m_nvcMinValue(strColumnName) & """")
                '        'Else
                '        '    strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldMin#", "min = ""0""  data-fv-between-min=""0""")
                '        'End If
                '        'strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldBetweenMsg#", " data-fv-between=""true"" data-fv-between-message=""The value must be " & _
                '        '    "between " & m_nvcMinValue(strColumnName) & " and " & m_nvcMaxValue(strColumnName) & ".""")
                '    Else
                '        strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldBetweenMsg#", " data-fv-between=""true"" data-fv-between-message=""The value must be " & _
                '            "between " & m_nvcMinValue(strColumnName) & " and " & m_nvcMaxValue(strColumnName) & ".""") _
                '            .Replace("#FieldMax#", " data-fv-between-max=""" & m_nvcMaxValue(strColumnName) & """ data-v-max=""" & m_nvcMaxValue(strColumnName) & """ max = """ & m_nvcMaxValue(strColumnName) & """")
                '        If m_nvcMinValue(strColumnName) < 0 Then
                '            strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldMin#", " data-fv-between-min=""" & m_nvcMinValue(strColumnName) & """ data-v-min=""" & m_nvcMinValue(strColumnName) & """ min=""" & m_nvcMinValue(strColumnName) & """")
                '        Else
                '            strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldMin#", " data-fv-between-min=""" & m_nvcMinValue(strColumnName) & """ data-v-min=""0"" min=""" & m_nvcMinValue(strColumnName) & """")
                '        End If

                '    End If

                'End If


                If UCase(m_nvcPercentage(strColumnName)) = "TRUE" Then
                    strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldPercentage#", "data-a-sign="" %"" data-p-sign=""s"" ")
                End If

                strBootstrapCurrent = strBootstrapCurrent.Replace("#FieldName#", strColumnName) _
                    .Replace("#FieldLabel#", strLabel) _
                    .Replace("#MAX#", m_nvcColumnLength(strColumnName)) _
                    .Replace("#WIDTH#", strWidth) _
                    .Replace("#disabled#", IIf(UCase(m_nvcDisabled(strColumnName)) = "FALSE", "", " disabled ")) _
                    .Replace("#required#", IIf(UCase(m_nvcColumnRequired(strColumnName)) = "TRUE", " required ", "")) _
                    .Replace("#unique#", IIf(UCase(m_nvcColumnUnique(strColumnName)) = "TRUE", "onchange='validateUnique(""" & TableName & """, """ & strColumnName & """, this, """ & m_nvcColumnDescription(strColumnName).Replace("""", "/""") & """)' ", "")) _
                    .Replace("#validate#", strValidation) _
                    .Replace("#FieldASign#", " data-a-sign="" " & m_nvcaSign(strColumnName) & """") _
                    .Replace("#FieldPSign#", " data-p-sign=""" & m_nvcpSign(strColumnName) & """")

                ' 9/18/14 cpb/cp - had to pull this out b/c the replace blowes on converting the time field therfore,
                '           set code if value has already been replaced, don't try to do it again.
                If strBootstrapCurrent.Contains("#Value#") Then
                    strBootstrapCurrent = strBootstrapCurrent.Replace("#Value#", RowIn(strColumnName))
                End If
                strBootstrapOut &= strBootstrapCurrent
                ' 3/20/15 T3 testing why mastertable messes up popup add/edit
                'strBootstrapOut &= "<div> Master Fields: " & strMasterTblFields & "</div>"

                'If i Mod ObjsDesiredPerRow_Max3 + 1 = ObjsDesiredPerRow_Max3 Then
                '    strBootstrapOut &= strBootStrapRowEndTag
                '    strBootStrapRowEndTag = ""

                '    Select Case ObjsDesiredPerRow_Max3
                '        Case 1
                '            strBootstrapOut = strBootstrapOut.Replace("#mdLabel#", "1").Replace("#mdField#", "11")
                '        Case 2
                '            strBootstrapOut = strBootstrapOut.Replace("#mdLabel#", "1").Replace("#mdField#", "5")
                '        Case Else
                '            strBootstrapOut = strBootstrapOut.Replace("#mdLabel#", "2").Replace("#mdField#", "2")
                '    End Select

                'End If
                'i += 1
            End If
        Next

        ' 7/16/15 T3 finish formvalidation js
        If strFVFields <> "" Then
            litFormValidation.Text = litFormValidation.Text.Replace("#FVFIELDS#", "fields: {" & strFVFields & "}").Replace("#FVONFIELDS#", strFVOnFields).Replace("#FVREVALIDATE#", strFVRevalidateFields)
        Else
            litFormValidation.Text = litFormValidation.Text.Replace("#FVFIELDS#", "").Replace("#FVONFIELDS#", "").Replace("#FVREVALIDATE#", "")

        End If

        strBootstrapOut &= strJavascriptFieldDirectory & "</div>" ' & "</form> "
        Return strBootstrapOut
    End Function

End Class