Module modAutoCode
    'Public Variables required for ModAutoCode
    Public g_ModeCreateDatabase As Boolean = False
    Public g_ModuleCode As String = "**"
    Public g_nvcCreateDatabaseColumns As New NameValueCollection
    Public g_nvcCreateReguiredFieldView As New NameValueCollection

    Public g_blnRequiredFieldsMet As Boolean = False

    'Public Sub g_HideControlsNotUsedByClient(ByRef ContentHolder As ContentPlaceHolder)
    '    Dim ctlControl As WebControl = Nothing

    '    ' hide any controls found in g_strControlsHideList
    '    For Each strControlName As String In Split(g_strControlsHideList, ",")
    '        ctlControl = ContentHolder.FindControl(strControlName)
    '        If IsNothing(ctlControl) Then
    '        Else
    '            ctlControl.Visible = False
    '        End If
    '    Next
    'End Sub

    Public Sub g_BuildOnBlurrEvents(ByVal ContentHolder As ContentPlaceHolder, ByVal FormCode As String, ByVal ModuleCode As String)


        ' get controls from contentholder and subcontainers
        Dim arrControls() As Control = Nothing


        Dim arrContainer(0) As Control

        Dim intContentHolderIndex As Integer = 0
        Dim intControlsIndex As Integer = 0
        Dim intContainerIndex As Integer = 1
        Do
            If ContentHolder.Controls.Item(intContentHolderIndex).Controls.Count = 0 Then
                ReDim Preserve arrControls(intControlsIndex)
                arrControls(intControlsIndex) = ContentHolder.Controls.Item(intContentHolderIndex)
                intControlsIndex += 1
            Else
                ' index of 0 will be left empty 
                ReDim Preserve arrContainer(intContainerIndex)
                arrContainer(intContainerIndex) = ContentHolder.Controls.Item(intContentHolderIndex)
                intContainerIndex += 1
            End If
            intContentHolderIndex += 1
        Loop Until ContentHolder.Controls.Count = intContentHolderIndex


        Dim intSubContainerIndex As Integer = 0
        intContainerIndex -= 1
        Dim ctrlSubContainer As Control = Nothing
        Do Until intContainerIndex = 0
            ctrlSubContainer = arrContainer(intContainerIndex)  '  save off highest entry in container array
            intContainerIndex -= 1
            ReDim Preserve arrContainer(intContainerIndex)  '  drop this entry from the container array

            ' process controls in the current container (just removed from the container array)
            intSubContainerIndex = 0
            Do
                If ctrlSubContainer.Controls.Item(intSubContainerIndex).Controls.Count = 0 Or Left(ctrlSubContainer.Controls.Item(intSubContainerIndex).ID, 3) = "lst" Or Left(ctrlSubContainer.Controls.Item(intSubContainerIndex).ID, 3) = "cyn" Then
                    ReDim Preserve arrControls(intControlsIndex)
                    arrControls(intControlsIndex) = ctrlSubContainer.Controls.Item(intSubContainerIndex)
                    intControlsIndex += 1
                    If Left(ctrlSubContainer.Controls.Item(intSubContainerIndex).ID, 3) = "ryn" Then
                        Dim rrrr = 1
                    End If

                Else
                    ' index of 0 will not be used but will by empty
                    intContainerIndex += 1
                    ReDim Preserve arrContainer(intContainerIndex)
                    arrContainer(intContainerIndex) = ctrlSubContainer.Controls.Item(intSubContainerIndex)
                End If
                intSubContainerIndex += 1
            Loop Until ctrlSubContainer.Controls.Count = intSubContainerIndex
        Loop



        ' now every base control should be represented in arrControls, create table columns from them, if appropriate
        Dim strDelim As String = ""
        Dim strListboxNoItems As String = ""
        For Each ctrControl In arrControls
            strDelim = ";"
            Select Case Left(ctrControl.ID, 3)
                Case "txt"
                    Dim strTypeEvent As String = IIf(UCase(ctrControl.ID).Contains("PHONE") Or UCase(ctrControl.ID).Contains("DATE"), "onChange", "onBlur")
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim txtTextBox As TextBox = ctrControl
                        If UCase(txtTextBox.CssClass).Contains("DB") Then
                            If UCase(txtTextBox.CssClass).Contains("NOAUTO") Then
                                ' can't auto create ONBLUR event because one already exists
                            Else
                                If IsNothing(txtTextBox.Attributes.Item(strTypeEvent)) Then
                                    txtTextBox.Attributes.Add(strTypeEvent, "")
                                    strDelim = ""
                                End If
                                txtTextBox.Attributes.Item(strTypeEvent) = txtTextBox.Attributes.Item(strTypeEvent) & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & txtTextBox.ID & """,this.value)"
                            End If
                        End If
                    End If
                Case "int"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim intTextBox As TextBox = ctrControl
                        If UCase(intTextBox.CssClass).Contains("DB") Then
                            If UCase(intTextBox.CssClass).Contains("NOAUTO") Then
                                ' can't auto create ONBLUR event because one already exists
                            Else
                                If IsNothing(intTextBox.Attributes.Item("onblur")) Then
                                    intTextBox.Attributes.Add("onblur", "")
                                    strDelim = ""
                                End If
                                intTextBox.Attributes.Item("onblur") = intTextBox.Attributes.Item("onblur") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & intTextBox.ID & """,this.value)"
                            End If
                        End If
                    End If
                Case "dol"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim dolTextBox As TextBox = ctrControl
                        If UCase(dolTextBox.CssClass).Contains("DB") Then
                            If UCase(dolTextBox.CssClass).Contains("NOAUTO") Then
                                ' can't auto create ONBLUR event because one already exists
                            Else
                                If IsNothing(dolTextBox.Attributes.Item("onblur")) Then
                                    dolTextBox.Attributes.Add("onblur", "")
                                    strDelim = ""
                                End If
                                dolTextBox.Attributes.Item("onblur") = dolTextBox.Attributes.Item("onblur") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & dolTextBox.ID & """,this.value)"
                            End If
                        End If
                    End If
                Case "num"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim numTextBox As TextBox = ctrControl
                        If UCase(numTextBox.CssClass).Contains("DB") Then
                            If UCase(numTextBox.CssClass).Contains("NOAUTO") Then
                                ' can't auto create ONBLUR event because one already exists
                            Else
                                If IsNothing(numTextBox.Attributes.Item("onblur")) Then
                                    numTextBox.Attributes.Add("onblur", "")
                                    strDelim = ""
                                End If
                                numTextBox.Attributes.Item("onblur") = numTextBox.Attributes.Item("onblur") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & numTextBox.ID & """,this.value)"
                            End If
                        End If
                    End If
                Case "rdo"
                    Dim rdoRadioButton As RadioButton = ctrControl
                    If UCase(rdoRadioButton.CssClass).Contains("DB") Then
                        If UCase(rdoRadioButton.CssClass).Contains("NOAUTO") Then
                            ' can't auto create ONBLUR event because one already exists
                        Else
                            If IsNothing(rdoRadioButton.Attributes.Item("onChange")) Then
                                rdoRadioButton.Attributes.Add("onChange", "")
                                strDelim = ""
                            End If
                            rdoRadioButton.Attributes.Item("onChange") = rdoRadioButton.Attributes.Item("onChange") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & rdoRadioButton.ID & """,(this.checked == true ? """":""X""))"
                        End If
                    End If
                Case "ryn"
                    Dim rynRadioButtonList As RadioButtonList = ctrControl
                    If UCase(rynRadioButtonList.CssClass).Contains("DB") Then
                        If UCase(rynRadioButtonList.CssClass).Contains("NOAUTO") Then
                            ' can't auto create ONBLUR event because one already exists
                        Else
                            If IsNothing(rynRadioButtonList.Attributes.Item("onChange")) Then
                                rynRadioButtonList.Attributes.Add("onChange", "")
                                strDelim = ""
                            End If
                            rynRadioButtonList.Attributes.Item("onChange") = rynRadioButtonList.Attributes.Item("onChange") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & rynRadioButtonList.ID & """,(document.getElementById(this.id + '_0').checked == true ? ""X"":""""))"
                        End If
                    End If
                Case "chk"
                    Dim chkCheckbox As CheckBox = ctrControl
                    If UCase(chkCheckbox.CssClass).Contains("DB") Then
                        If UCase(chkCheckbox.CssClass).Contains("NOAUTO") Then
                            ' can't auto create ONCHANGE event because user flagged it not to
                        Else
                            If IsNothing(chkCheckbox.Attributes.Item("onChange")) Then
                                chkCheckbox.Attributes.Add("onChange", "")
                                strDelim = ""
                            End If
                            chkCheckbox.Attributes.Item("onChange") = chkCheckbox.Attributes.Item("onChange") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & chkCheckbox.ID & """,(this.checked == true ? """":""X""))"
                        End If
                    End If
                Case "sel"
                    Dim ddlDropDownList As DropDownList = ctrControl
                    If UCase(ddlDropDownList.CssClass).Contains("DB") Then
                        If UCase(ddlDropDownList.CssClass).Contains("NOAUTO") Then
                            ' can't auto create onchange event because one already exists
                        Else
                            If IsNothing(ddlDropDownList.Attributes.Item("onchange")) Then
                                ddlDropDownList.Attributes.Add("onchange", "")
                                strDelim = ""
                            End If
                            ddlDropDownList.Attributes.Item("onchange") = ddlDropDownList.Attributes.Item("onchange") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & ddlDropDownList.ID & """,this.value)"
                        End If
                    End If
                Case "ddl"
                    Dim ddlDropDownList As DropDownList = ctrControl
                    If UCase(ddlDropDownList.CssClass).Contains("DB") Then
                        If UCase(ddlDropDownList.CssClass).Contains("NOAUTO") Then
                            ' can't auto create onchange event because one already exists
                        Else
                            If IsNothing(ddlDropDownList.Attributes.Item("onchange")) Then
                                ddlDropDownList.Attributes.Add("onchange", "")
                                strDelim = ""
                            End If
                            ddlDropDownList.Attributes.Item("onchange") = ddlDropDownList.Attributes.Item("onchange") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & ddlDropDownList.ID & """,this.value)"
                        End If
                    End If
                Case "cbo"
                    Dim ddlDropDownList As DropDownList = ctrControl
                    If UCase(ddlDropDownList.CssClass).Contains("DB") Then
                        If UCase(ddlDropDownList.CssClass).Contains("NOAUTO") Then
                            ' can't auto create onchange event because one already exists
                        Else
                            If IsNothing(ddlDropDownList.Attributes.Item("onchange")) Then
                                ddlDropDownList.Attributes.Add("onchange", "")
                                strDelim = ""
                            End If
                            ddlDropDownList.Attributes.Item("onchange") = ddlDropDownList.Attributes.Item("onchange") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & ddlDropDownList.ID & """,this.value)"
                        End If
                    End If
                Case "dte"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim txtTextBox As TextBox = ctrControl
                        If UCase(txtTextBox.CssClass).Contains("DB") Then
                            If UCase(txtTextBox.CssClass).Contains("NOAUTO") Then
                                ' can't auto create ONBLUR event because one already exists
                            Else
                                If IsNothing(txtTextBox.Attributes.Item("onchange")) Then
                                    txtTextBox.Attributes.Add("onchange", "")
                                    strDelim = ""
                                End If
                                txtTextBox.Attributes.Item("onchange") = txtTextBox.Attributes.Item("onchange") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & txtTextBox.ID & """,this.value)"
                            End If
                        End If
                    End If
                Case "lst"
                    Dim lstListCheckBox As CheckBoxList = ctrControl
                    ' the no items on the checkboxes is not flagged for storage in the databased, but still need an event stamped
                    If UCase(lstListCheckBox.CssClass).Contains("DB") Or strListboxNoItems.Contains(UCase(lstListCheckBox.ID)) Then
                        If UCase(lstListCheckBox.CssClass).Contains("NOAUTO") Then
                            ' can't auto create ONBLUR event because one already exists
                        Else
                            Dim blnNoBox As Boolean = Not UCase(lstListCheckBox.CssClass).Contains("DB")
                            If blnNoBox Then
                                Dim r = 1
                            Else
                                strListboxNoItems &= "," & UCase(lstListCheckBox.ID) & "NO"
                            End If
                            Dim intIndex As Integer = 0
                            For Each chkCheckbox As ListItem In lstListCheckBox.Items
                                If IsNothing(chkCheckbox.Attributes.Item("onclick")) Then
                                    chkCheckbox.Attributes.Add("onclick", "")
                                    strDelim = ""
                                End If
                                chkCheckbox.Attributes.Item("onclick") = chkCheckbox.Attributes.Item("onclick") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & lstListCheckBox.ID.Trim("o").Trim("O").Trim("n").Trim("N") & "_" & intIndex & """,(this.checked == true ? """ & IIf(blnNoBox, "", "X") & """:""" & IIf(blnNoBox, "X", "") & """) + ""_"" +  this.value)"
                                intIndex += 1
                            Next
                        End If
                    End If
                Case "cyn"
                    Dim cynListCheckBox As CheckBoxList = ctrControl
                    ' the no items on the checkboxes is not flagged for storage in the databased, but still need an event stamped
                    If UCase(cynListCheckBox.CssClass).Contains("DB") Or strListboxNoItems.Contains(UCase(cynListCheckBox.ID)) Then
                        If UCase(cynListCheckBox.CssClass).Contains("NOAUTO") Then
                            ' can't auto create ONBLUR event because one already exists
                        Else
                            Dim blnNoBox As Boolean = Not UCase(cynListCheckBox.CssClass).Contains("DB")
                            If blnNoBox Then
                                Dim r = 1
                            Else
                                strListboxNoItems &= "," & UCase(cynListCheckBox.ID) & "NO"
                            End If
                            Dim intIndex As Integer = 0
                            For Each chkCheckbox As ListItem In cynListCheckBox.Items
                                If IsNothing(chkCheckbox.Attributes.Item("OnClick")) Then
                                    chkCheckbox.Attributes.Add("OnClick", "")
                                    strDelim = ""
                                End If
                                chkCheckbox.Attributes.Item("OnClick") = chkCheckbox.Attributes.Item("OnClick") & strDelim & "autoPost(""" & ModuleCode & "_" & FormCode & "__" & cynListCheckBox.ID.Trim("o").Trim("O").Trim("n").Trim("N") & "_" & intIndex & """,(this.checked == true ? """ & IIf(blnNoBox, "", "X") & """:""" & IIf(blnNoBox, "X", "") & """) + ""_"" +  this.value)"
                                intIndex += 1
                            Next
                        End If
                    End If
            End Select
            ReDim Preserve arrControls(arrControls.Count - 1)

        Next

    End Sub
    'Public Function Create_Requirement_Side_bar(ByVal FormModuleCode As String, ByVal strTable As String) As String

    '    Dim strSQL As String = "Select * from " & strTable & "_RequiredFields_vw"
    '    Dim tblRequiredFields As DataTable = g_IO_Execute_SQL(strSQL, False)
    '    Dim rowRequiredField As DataRow = tblRequiredFields.Rows(0)
    '    Dim blnAllRequiredFieldsMet As Boolean = True
    '    g_blnRequiredFieldsMet = True

    '    Dim intIndexTabs As Integer = 0
    '    Dim nvcTabHTML As New NameValueCollection
    '    Dim nvcRequirementState As New NameValueCollection

    '    For Each colRequirement As DataColumn In tblRequiredFields.Columns
    '        Dim strAccumKey As String = colRequirement.ColumnName
    '        Dim strTab As String = Split(colRequirement.ColumnName, "__")(0)
    '        Dim strFields As String = ""
    '        Dim strFieldGroup As String = ""
    '        If tblRequiredFields.Rows(0)(colRequirement.ColumnName).Contains("||") Then
    '            strFieldGroup = Split(tblRequiredFields.Rows(0)(colRequirement.ColumnName), "||")(0)
    '            strFields = Split(tblRequiredFields.Rows(0)(colRequirement.ColumnName), "||")(1)
    '        Else
    '            strFields = tblRequiredFields.Rows(0)(colRequirement.ColumnName)
    '        End If



    '        If IsNothing(nvcTabHTML(IIf(strFieldGroup = "", "", strFieldGroup & "||") & colRequirement.ColumnName)) Then
    '            nvcTabHTML(IIf(strFieldGroup = "", "", strFieldGroup & "||") & colRequirement.ColumnName) = ""
    '            nvcRequirementState(IIf(strFieldGroup = "", "", strFieldGroup & "||") & colRequirement.ColumnName) = 0
    '        End If

    '        For Each strFieldName As String In Split(strFields, "|")
    '            Select Case Left(Mid(strFieldName, 7), 3)
    '                Case "txt"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or Not System.Web.HttpContext.Current.Session(strFieldName) = ""
    '                Case "int"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or Not System.Web.HttpContext.Current.Session(strFieldName) = ""
    '                Case "dol"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or Not System.Web.HttpContext.Current.Session(strFieldName) = ""
    '                Case "num"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or Not System.Web.HttpContext.Current.Session(strFieldName) = ""
    '                Case "rdo"
    '                    strAccumKey = strFieldGroup & "||" & colRequirement.ColumnName
    '                    nvcRequirementState(strAccumKey) = CBool(nvcRequirementState(strAccumKey)) Or System.Web.HttpContext.Current.Session(strFieldName) = "X"
    '                Case "ryn"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or System.Web.HttpContext.Current.Session(strFieldName & "Yes") = "X" Or System.Web.HttpContext.Current.Session(strFieldName & "No") = "X"
    '                Case "chk"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or System.Web.HttpContext.Current.Session(strFieldName) = "X"
    '                Case "sel"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or System.Web.HttpContext.Current.Session(strFieldName) > -1
    '                Case "ddl"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or System.Web.HttpContext.Current.Session(strFieldName) > -1
    '                Case "cbo"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or System.Web.HttpContext.Current.Session(strFieldName) > -1
    '                Case "dte"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or IsDate(System.Web.HttpContext.Current.Session(strFieldName))
    '                Case "lst"
    '                    nvcRequirementState(colRequirement.ColumnName) = CBool(nvcRequirementState(colRequirement.ColumnName)) Or System.Web.HttpContext.Current.Session(strFieldName) > -1
    '                Case "cyn"
    '                    Dim strTableName As String = Split(Split(strFieldName, "cyn")(1), "_col")(0)
    '                    If IsNothing(System.Web.HttpContext.Current.Session(strTableName)) Then
    '                        strSQL = "Select RECID, DESCR from " & strTableName & " order by descr"
    '                        Dim tblDropdown As DataTable = g_IO_Execute_SQL(strSQL, False)
    '                        Dim strDelim As String = ""
    '                        For Each rowID As DataRow In tblDropdown.Rows
    '                            System.Web.HttpContext.Current.Session(strTableName) &= strDelim & rowID("recid")
    '                            strDelim = ","
    '                        Next
    '                    End If

    '                    Dim blnComplete As Boolean = True
    '                    Dim strSessionName As String = Split(tblRequiredFields.Rows(0)(colRequirement.ColumnName), "_col")(0)
    '                    If IsNothing(System.Web.HttpContext.Current.Session(strSessionName)) Then
    '                    Else
    '                        Dim strTestList As String = System.Web.HttpContext.Current.Session(strSessionName).Replace("-", "")
    '                        For Each strCode In Split(CStr(System.Web.HttpContext.Current.Session(strTableName)), ",")
    '                            blnComplete = blnComplete And CStr("," & strTestList & ",").Contains("," & strCode & ",")
    '                            If Not blnComplete Then Exit For
    '                        Next
    '                    End If
    '                    nvcRequirementState(colRequirement.ColumnName) = blnComplete
    '            End Select
    '        Next
    '        If strTab = FormModuleCode Then
    '            g_blnRequiredFieldsMet = g_blnRequiredFieldsMet And CBool(nvcRequirementState(strAccumKey))
    '        End If
    '        blnAllRequiredFieldsMet = blnAllRequiredFieldsMet And CBool(nvcRequirementState(strAccumKey))

    '    Next

    '    For Each strRequirement As String In nvcRequirementState.Keys

    '        Dim strTypes() As String
    '        Dim strFields As String = ""
    '        Dim strField As String = ""

    '        If strRequirement.Contains("||") Then
    '            strFields = Split(strRequirement, "||")(1)
    '            strTypes = Split(strFields, "__")
    '        Else
    '            strTypes = Split(strRequirement, "__")
    '        End If

    '        Dim strTableField As String = strRequirement
    '        If strRequirement.Contains("||") Then
    '            strTableField = Split(strRequirement, "||")(1)
    '        End If


    '        If tblRequiredFields.Rows(0)(strTableField).Contains("||") Then
    '            strFields = Split(tblRequiredFields.Rows(0)(strTableField), "||")(1)
    '        Else
    '            strFields = tblRequiredFields.Rows(0)(strTableField)
    '        End If
    '        strField = Split(strFields, "|")(0).Replace(strTypes(0) & "__", "")

    '        If CBool(nvcRequirementState(strRequirement)) Then
    '            nvcTabHTML(strRequirement) &= "<tr><td style=""margin-left: 10px; cursor: hand; cursor: pointer"" onclick=""ProcessTabRequest('" & g_nvcMenuTabId(strTypes(0)) & "','" & strField & "');""><input type=""checkbox"" checked = ""checked""  disabled=""disabled"" />" & strTypes(1).Replace("_", " ") & "</td></tr>"
    '        Else
    '            nvcTabHTML(strRequirement) &= "<tr><td style=""margin-left: 10px; cursor: hand; cursor: pointer"" onclick=""ProcessTabRequest('" & g_nvcMenuTabId(strTypes(0)) & "','" & strField & "');""><input type=""checkbox"" disabled=""disabled"" />" & strTypes(1).Replace("_", " ") & "</td></tr>"
    '        End If
    '    Next

    '    Dim strSubmitToStaffButton As String = ""
    '    Dim strDummy As String = ""
    '    Dim blnNotLoggedIn As Boolean = IsNothing(System.Web.HttpContext.Current.Session("user_link_id"))
    '    If blnNotLoggedIn Then
    '        strSubmitToStaffButton = "<span @@@><input type=""submit"" class=""button"" onclick=""InitiateSignon();return false;"" value=""Login"" &&& /><br><span style=""font-size:x-small"" ><i>You must login to save your work</i></span></span><br><br>"
    '    ElseIf g_AreThereChanges(strDummy) Then

    '        ' if a task record already exists, no reason to offer a submit to staff button
    '        strSQL = "Select substring(TaskDescription,1,3) as type from sys_tasks where submittedby_recid=" & System.Web.HttpContext.Current.Session("P_SysUserRECID")
    '        Dim tblTasks As DataTable = g_IO_Execute_SQL(strSQL, False)
    '        If tblTasks.Rows.Count = 0 Then
    '            strSubmitToStaffButton = "<span @@@><input type=""submit"" class=""button"" onclick=""SubmitFormToStaff();return false;"" value=""Submit My Information"" &&& /><br><span style=""font-size:x-small"" ><i>All required fields must be completed</i></span></span><br><br>"
    '        Else
    '            If UCase(tblTasks.Rows(0)("type")) = "NEW" Then
    '                strSubmitToStaffButton = "<center><i>Your forms are awaiting processing.<br>If you make changes they will be included.</i></center><br><br>"
    '            Else
    '                strSubmitToStaffButton = "<center><i>You have updates in process.<br>If you make changes they will be included.</i></center><br><br>"
    '            End If
    '        End If
    '    End If

    '    Dim strHTML As String = "<h2><b>Required Fields</b></h2>" & strSubmitToStaffButton
    '    Dim strCurrentTab As String = ""
    '    Dim strNextTab As String = ""
    '    Dim strAfterFirstRound As String = ""
    '    For Each strRequirement As String In nvcTabHTML.Keys

    '        If strRequirement.Contains("||") Then
    '            strNextTab = Split(strRequirement, "||")(1)
    '        Else
    '            strNextTab = strRequirement
    '        End If

    '        strNextTab = Split(strNextTab, "__")(0)

    '        If strCurrentTab = strNextTab Then
    '        Else
    '            strHTML &= strAfterFirstRound & "<table border=""0"" cellpadding=""0"" cellspacing=""0"">" & _
    '                 "<tr><td style=""font-style:oblique;font-weight:bold; cursor: hand; cursor: pointer"" onclick=""ProcessTabRequest('" & g_nvcMenuTabId(strNextTab) & "');"">" & g_strMenuTabName(CInt(Mid(Split(strRequirement, "_")(1), 2))) & "</td></tr>"
    '            strAfterFirstRound = "</table><br><br>"
    '        End If

    '        strCurrentTab = strNextTab

    '        strHTML &= nvcTabHTML(strRequirement)
    '    Next

    '    strHTML &= "</table><br><div id=""divSubmitToStaffButton"">" & strSubmitToStaffButton & "</div>"

    '    strHTML = strHTML.Replace("&&&", IIf(blnAllRequiredFieldsMet Or blnNotLoggedIn, "", " disabled=""disabled"" ")).Replace("@@@", IIf(blnAllRequiredFieldsMet Or blnNotLoggedIn, """", " onclick=""alert('All required fields must be completed for our staff to process your information.')"""))
    '    Return strHTML
    'End Function

    Public Function g_FormSessionsSet(ByVal FormCode As String, ByVal ModuleCode As String) As Boolean
        ' do session variables exist for this form?
        Dim blnSessionsNotSet As Boolean = False
        For Each strSession As String In System.Web.HttpContext.Current.Session.Keys
            If strSession.Contains(ModuleCode & "_" & FormCode) Then
                blnSessionsNotSet = True
                Exit For
            End If
        Next
        Return blnSessionsNotSet
    End Function



    Public Sub g_BuildBDTableFromForm(ByVal TableName As String, ByVal ContentHolder As ContentPlaceHolder, ByVal FormCode As String, ByVal ModuleCode As String)
        g_BuildBDTableFromForm(TableName, ContentHolder, FormCode, ModuleCode, False)
    End Sub
    Public Sub g_BuildBDTableFromForm(ByVal TableName As String, ByVal ContentHolder As ContentPlaceHolder, ByVal FormCode As String, ByVal ModuleCode As String, ByVal CreateTable As Boolean)
        If ModuleCode = "**" Then

            Exit Sub
        End If
        Dim strSessionPrefix As String = ModuleCode & "_" & FormCode & "__"
        Dim intPrefixLength As Integer = Len(strSessionPrefix)
        Dim strBaseColumnList As String = ""

        Dim strSQL As String = ""

        strBaseColumnList = "recid,DatePosted,Sys_Users_RECID"

        ' is table already in place, if so look to add the fields only
        If CreateTable Then
            strSQL =
              "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" & TableName & "'"
            Dim tblColumns As DataTable = g_IO_Execute_SQL(strSQL, False)
            If tblColumns.Rows.Count = 0 Then
            Else
                g_IO_Execute_SQL("drop table " & TableName, False)

            End If

            ' table does not exits, create the base table
            strSQL = "CREATE TABLE [dbo].[" & TableName & "](" & _
                     "[RECID] [int] IDENTITY(1,1) NOT NULL," & _
                     "[DatePosted] [datetime] NULL default getDate()," & _
                     "[Sys_Users_RECID] [int] NULL default -1," & _
                     " CONSTRAINT [PK_" & TableName & "] PRIMARY KEY CLUSTERED ([RECID] Asc)" & _
                     ")"
            g_IO_Execute_SQL(strSQL, False)


            ' build index
            'strSQL = "CREATE UNIQUE NONCLUSTERED INDEX [IX_" & TableName & "] ON [dbo].[" & TableName & "] ([Sys_Users_RECID] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"
            'g_IO_Execute_SQL(strSQL, False)

        End If


        ' get controls from contentholder and subcontainers
        Dim arrControls() As Control = Nothing


        Dim arrContainer(0) As Control

        Dim intContentHolderIndex As Integer = 0
        Dim intControlsIndex As Integer = 0
        Dim intContainerIndex As Integer = 1
        Do
            If IsNothing(ContentHolder.Controls.Item(intContentHolderIndex).ID) Then
            Else
                If ContentHolder.Controls.Item(intContentHolderIndex).Controls.Count = 0 Or _
                    "lstselddlcbolstcynryn".Contains(Left(ContentHolder.Controls.Item(intContentHolderIndex).ID, 3)) Then
                    ReDim Preserve arrControls(intControlsIndex)
                    arrControls(intControlsIndex) = ContentHolder.Controls.Item(intContentHolderIndex)
                    intControlsIndex += 1
                Else
                    ' index of 0 will be left empty 
                    ReDim Preserve arrContainer(intContainerIndex)
                    arrContainer(intContainerIndex) = ContentHolder.Controls.Item(intContentHolderIndex)
                    intContainerIndex += 1
                End If
            End If

            intContentHolderIndex += 1

        Loop Until ContentHolder.Controls.Count = intContentHolderIndex


        Dim intSubContainerIndex As Integer = 0
        intContainerIndex -= 1
        Dim ctrlSubContainer As Control = Nothing
        Do Until intContainerIndex = 0
            ctrlSubContainer = arrContainer(intContainerIndex)  '  save off highest entry in container array
            intContainerIndex -= 1
            ReDim Preserve arrContainer(intContainerIndex)  '  drop this entry from the container array

            ' process controls in the current container (just removed from the container array)
            intSubContainerIndex = 0
            Do
                If ctrlSubContainer.ID.Contains("lst") Then
                    Dim rrr = 1


                    If ctrlSubContainer.Controls.Item(intSubContainerIndex).Controls.Count = 0 Or Left(ctrlSubContainer.Controls.Item(intSubContainerIndex).ID, 3) = "lst" Or Left(ctrlSubContainer.Controls.Item(intSubContainerIndex).ID, 3) = "cyn" Then
                        ReDim Preserve arrControls(intControlsIndex)
                        arrControls(intControlsIndex) = ctrlSubContainer.Controls.Item(intSubContainerIndex)
                        intControlsIndex += 1
                        If Left(ctrlSubContainer.Controls.Item(intSubContainerIndex).ID, 3) = "lst" Then
                            Dim rrrr = 1
                        End If

                    Else
                        ' index of 0 will not be used but will by empty
                        intContainerIndex += 1
                        ReDim Preserve arrContainer(intContainerIndex)
                        arrContainer(intContainerIndex) = ctrlSubContainer.Controls.Item(intSubContainerIndex)
                    End If
                End If
                intSubContainerIndex += 1

            Loop Until ctrlSubContainer.Controls.Count = intSubContainerIndex
        Loop



        ' now every base control should be represented in arrControls, create table columns from them, if appropriate
        For Each ctrControl In arrControls

            Select Case Left(ctrControl.ID, 3)
                Case "txt"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim txtTextBox As TextBox = ctrControl
                        If UCase(txtTextBox.CssClass).Contains("DB") Then
                            strSQL = "alter table " & TableName & " add " & Mid(txtTextBox.ID, 4) & " varChar(" & IIf(txtTextBox.MaxLength = 0, 1, txtTextBox.MaxLength) & ") Default '" & txtTextBox.Text & "'"
                            g_IO_Execute_SQL(strSQL, False)

                            g_nvcCreateDatabaseColumns(Mid(txtTextBox.ID, 4)) = strSessionPrefix & txtTextBox.ID
                            If UCase(txtTextBox.CssClass).Contains("RQ__") Then
                                For Each strClass In Split(txtTextBox.CssClass, " ")
                                    If Left(UCase(strClass), 4) = "RQ__" Then
                                        Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                        g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & txtTextBox.ID
                                    End If
                                Next

                            End If
                        End If
                    End If

                Case "int"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim intTextBox As TextBox = ctrControl
                        If UCase(intTextBox.CssClass).Contains("DB") Then
                            strSQL = "alter table " & TableName & " add " & Mid(intTextBox.ID, 4) & " int Default " & 0
                            g_IO_Execute_SQL(strSQL, False)
                            For Each strClass In Split(intTextBox.CssClass, " ")
                                If Left(UCase(strClass), 4) = "RQ__" Then
                                    Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                    g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & intTextBox.ID
                                End If
                            Next
                        End If
                        g_nvcCreateDatabaseColumns(Mid(intTextBox.ID, 4)) = strSessionPrefix & intTextBox.ID
                    End If
                Case "dol"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim dolTextBox As TextBox = ctrControl
                        If UCase(dolTextBox.CssClass).Contains("DB") Then
                            strSQL = "alter table " & TableName & " add " & Mid(dolTextBox.ID, 4) & " money Default " & 0
                            g_IO_Execute_SQL(strSQL, False)
                            For Each strClass In Split(dolTextBox.CssClass, " ")
                                If Left(UCase(strClass), 4) = "RQ__" Then
                                    Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                    g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & dolTextBox.ID
                                End If
                            Next
                        End If
                        g_nvcCreateDatabaseColumns(Mid(dolTextBox.ID, 4)) = strSessionPrefix & dolTextBox.ID
                    End If
                Case "num"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim strType As String = " numeric (11.2) "
                        Dim numTextBox As TextBox = ctrControl

                        ' this code also used in ModMain

                        If UCase(numTextBox.CssClass).Contains("DB") Then
                            Dim strLength = "18"
                            Dim strDec As String = "0"

                            Try
                                For Each strClass As String In Split(numTextBox.CssClass, " ")
                                    If IsNumeric(strClass) Then
                                        strLength = CInt(strClass)
                                        If CInt(strLength) = CSng(strClass) Then
                                        Else
                                            strDec = CInt(Split(strClass, ".")(1))
                                        End If
                                        strLength = CInt(strLength) + CInt(strDec)
                                        strType = " numeric(" & strLength & "," & strDec & ") "
                                        Exit For
                                    ElseIf UCase(strClass) = "INT" Then
                                        strType = " int "
                                    End If
                                Next
                            Catch ex As Exception
                            End Try

                            strSQL = "alter table " & TableName & " add " & Mid(numTextBox.ID, 4) & strType & " Default " & 0
                            g_IO_Execute_SQL(strSQL, False)
                            For Each strClass In Split(numTextBox.CssClass, " ")
                                If Left(UCase(strClass), 4) = "RQ__" Then
                                    Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                    g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & numTextBox.ID
                                End If
                            Next
                        End If
                        g_nvcCreateDatabaseColumns(Mid(numTextBox.ID, 4)) = strSessionPrefix & numTextBox.ID
                    End If
                Case "rdo"
                    Dim rdoRadioButton As RadioButton = ctrControl
                    If UCase(rdoRadioButton.CssClass).Contains("DB") Then
                        strSQL = "alter table " & TableName & " add " & Mid(rdoRadioButton.ID, 4) & " bit Default " & IIf(rdoRadioButton.Checked, 1, 0)
                        g_IO_Execute_SQL(strSQL, False)
                        g_nvcCreateDatabaseColumns(Mid(rdoRadioButton.ID, 4)) = strSessionPrefix & rdoRadioButton.ID
                        For Each strClass In Split(rdoRadioButton.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", rdoRadioButton.GroupName & "||", "|") & ModuleCode & "_" & FormCode & "__" & rdoRadioButton.ID
                            End If
                        Next
                    End If
                Case "ryn"
                    Dim rynRadioButtonList As RadioButtonList = ctrControl
                    If UCase(rynRadioButtonList.CssClass).Contains("DB") Then
                        strSQL = "alter table " & TableName & " add " & Mid(rynRadioButtonList.ID, 4) & "Yes bit Default " & IIf(rynRadioButtonList.SelectedIndex = 0, 1, 0)
                        g_IO_Execute_SQL(strSQL, False)
                        strSQL = "alter table " & TableName & " add " & Mid(rynRadioButtonList.ID, 4) & "No bit Default " & IIf(rynRadioButtonList.SelectedIndex = 1, 1, 0)
                        g_IO_Execute_SQL(strSQL, False)
                        g_nvcCreateDatabaseColumns(Mid(rynRadioButtonList.ID, 4) & "Yes") = strSessionPrefix & rynRadioButtonList.ID & "Yes"
                        g_nvcCreateDatabaseColumns(Mid(rynRadioButtonList.ID, 4) & "No") = strSessionPrefix & rynRadioButtonList.ID & "No"
                        For Each strClass In Split(rynRadioButtonList.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & rynRadioButtonList.ID
                            End If
                        Next
                    End If
                Case "chk"
                    Dim chkCheckbox As CheckBox = ctrControl
                    If UCase(chkCheckbox.CssClass).Contains("DB") Then
                        strSQL = "alter table " & TableName & " add " & Mid(chkCheckbox.ID, 4) & " bit Default " & IIf(chkCheckbox.Checked, 1, 0)
                        g_IO_Execute_SQL(strSQL, False)
                        g_nvcCreateDatabaseColumns(Mid(chkCheckbox.ID, 4)) = strSessionPrefix & chkCheckbox.ID
                        For Each strClass In Split(chkCheckbox.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & chkCheckbox.ID
                            End If
                        Next
                    End If
                Case "sel"
                    Dim ddlDropDownList As DropDownList = ctrControl
                    Call CreateDropdownListTable(ddlDropDownList)

                    If UCase(ddlDropDownList.CssClass).Contains("DB") Then

                        strSQL = "alter table " & TableName & " add " & Mid(ddlDropDownList.ID, 4) & " int Default -1"
                        g_IO_Execute_SQL(strSQL, False)
                        g_nvcCreateDatabaseColumns(ddlDropDownList.ID) = strSessionPrefix & ddlDropDownList.ID
                        Call CreateDropdownListTable(ddlDropDownList)
                        For Each strClass In Split(ddlDropDownList.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & ddlDropDownList.ID
                            End If
                        Next
                    End If
                Case "ddl"
                    Dim ddlDropDownList As DropDownList = ctrControl
                    Call CreateDropdownListTable(ddlDropDownList)
                    If UCase(ddlDropDownList.CssClass).Contains("DB") Then
                        strSQL = "alter table " & TableName & " add " & Mid(ddlDropDownList.ID, 4) & " varchar(25) Default ''"
                        g_IO_Execute_SQL(strSQL, False)
                        g_nvcCreateDatabaseColumns(Mid(ddlDropDownList.ID, 4)) = strSessionPrefix & ddlDropDownList.ID
                        Call CreateDropdownListTable(ddlDropDownList)
                        For Each strClass In Split(ddlDropDownList.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & ddlDropDownList.ID
                            End If
                        Next
                    End If
                Case "cbo"
                    Dim ddlDropDownList As DropDownList = ctrControl
                    Call CreateDropdownListTable(ddlDropDownList)
                    If UCase(ddlDropDownList.CssClass).Contains("DB") Then
                        strSQL = "alter table " & TableName & " add " & Mid(ddlDropDownList.ID, 4) & " varchar(25) Default ''"
                        g_IO_Execute_SQL(strSQL, False)
                        g_nvcCreateDatabaseColumns(Mid(ddlDropDownList.ID, 4)) = strSessionPrefix & ddlDropDownList.ID
                        Call CreateDropdownListTable(ddlDropDownList)
                        For Each strClass In Split(ddlDropDownList.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & ddlDropDownList.ID
                            End If
                        Next
                    End If
                Case "dte"
                    If TypeOf (ctrControl) Is HiddenField Then
                    Else
                        Dim txtTextBox As TextBox = ctrControl
                        If UCase(txtTextBox.CssClass).Contains("DB") Then
                            strSQL = "alter table " & TableName & " add " & Mid(txtTextBox.ID, 4) & " varChar(" & IIf(txtTextBox.MaxLength = 0, 10, txtTextBox.MaxLength) & ") Default '" & txtTextBox.Text & "'"
                            g_IO_Execute_SQL(strSQL, False)
                            g_nvcCreateDatabaseColumns(Mid(txtTextBox.ID, 4)) = strSessionPrefix & txtTextBox.ID
                            For Each strClass In Split(txtTextBox.CssClass, " ")
                                If Left(UCase(strClass), 4) = "RQ__" Then
                                    Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                    g_nvcCreateReguiredFieldView(strReqKey) &= txtTextBox.ID
                                End If
                            Next
                        End If
                    End If
                Case "lst"
                    Dim lstListCheckBox As CheckBoxList = ctrControl
                    Call CreateDropdownListTable(lstListCheckBox)
                    If UCase(lstListCheckBox.CssClass).Contains("DB") Then
                        Dim strColn As String = Split(lstListCheckBox.ID, "_")(Split(lstListCheckBox.ID, "_").Count - 1)
                        If UCase(strColn) = "COL1" Then
                            ' strSQL = "alter table " & TableName & " add " & Mid(lstListCheckBox.ID, 4).Replace("_" & strColn, "") & "_ColumnCount int Default 1"
                            strSQL = "alter table " & TableName & " add " & Mid(lstListCheckBox.ID, 4).Replace("_" & strColn, "") & " varChar(100) Default '" & lstListCheckBox.Text & "'"
                            g_IO_Execute_SQL(strSQL, False)
                            g_nvcCreateDatabaseColumns(Mid(lstListCheckBox.ID, 4).Replace("_" & strColn, "")) = strSessionPrefix & lstListCheckBox.ID.Replace("_" & strColn, "")
                        End If

                        ' need to store how many columns in the display of selection lists on the form
                        If IsNothing(g_nvcCreateDatabaseColumns("VIEWONLY" & strSessionPrefix & lstListCheckBox.ID.Replace(strColn, "_FormColumnCount"))) OrElse _
                                    g_nvcCreateDatabaseColumns("VIEWONLY" & strSessionPrefix & lstListCheckBox.ID.Replace(strColn, "_FormColumnCount")) < UCase(strColn).Replace("COL", "") Then
                            g_nvcCreateDatabaseColumns("VIEWONLY" & strSessionPrefix & lstListCheckBox.ID.Replace(strColn, "_FormColumnCount")) = UCase(strColn).Replace("COL", "")
                        End If

                        For Each strClass In Split(lstListCheckBox.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & lstListCheckBox.ID
                            End If
                        Next
                    End If
                Case "cyn"
                    Dim lstListCheckBox As CheckBoxList = ctrControl
                    Call CreateDropdownListTable(lstListCheckBox)
                    If UCase(lstListCheckBox.CssClass).Contains("DB") Then
                        Dim strColn As String = Split(lstListCheckBox.ID, "_")(Split(lstListCheckBox.ID, "_").Count - 1)
                        If UCase(strColn) = "COL1" Then
                            '         strSQL = "alter table " & TableName & " add " & Mid(lstListCheckBox.ID, 4).Replace("_" & strColn, "") & "_ColumnCount int Default 1"
                            strSQL = "alter table " & TableName & " add " & Mid(lstListCheckBox.ID, 4).Replace("_" & strColn, "") & " varChar(2000) Default '" & lstListCheckBox.Text & "'"
                            g_IO_Execute_SQL(strSQL, False)
                            g_nvcCreateDatabaseColumns(Mid(lstListCheckBox.ID, 4).Replace("_" & strColn, "")) = strSessionPrefix & lstListCheckBox.ID.Replace("_" & strColn, "")
                        End If

                        ' need to store how many columns in the display of selection lists on the form
                        If IsNothing(g_nvcCreateDatabaseColumns("VIEWONLY" & strSessionPrefix & lstListCheckBox.ID.Replace(strColn, "_FormColumnCount"))) OrElse _
                                    g_nvcCreateDatabaseColumns("VIEWONLY" & strSessionPrefix & lstListCheckBox.ID.Replace(strColn, "_FormColumnCount")) < UCase(strColn).Replace("COL", "") Then
                            g_nvcCreateDatabaseColumns("VIEWONLY" & strSessionPrefix & lstListCheckBox.ID.Replace(strColn, "_FormColumnCount")) = UCase(strColn).Replace("COL", "")
                        End If

                        For Each strClass In Split(lstListCheckBox.CssClass, " ")
                            If Left(UCase(strClass), 4) = "RQ__" Then
                                Dim strReqKey As String = ModuleCode & "_" & FormCode & "__" & strClass.Replace("rq__", "")
                                g_nvcCreateReguiredFieldView(strReqKey) &= IIf(g_nvcCreateReguiredFieldView(strReqKey) = "", "", "|") & ModuleCode & "_" & FormCode & "__" & lstListCheckBox.ID
                            End If
                        Next
                    End If
                    'Case "ryn"
                    '    Dim rynRadionList As RadioButtonList = ctrControl
                    '    If UCase(rynRadionList.CssClass).Contains("DB") Then
                    '        strSQL = "alter table " & TableName & " add " & Mid(rynRadionList.ID, 4).Replace("_" & strColn, "") & "_ColumnCount int Default 1"
                    '        strSQL = "alter table " & TableName & " add " & Mid(rynRadionList.ID, 4).Replace("_" & strColn, "") & " varChar(2000) Default '" & rynRadionList.Text & "'"
                    '        g_IO_Execute_SQL(strSQL, False)
                    '        g_IO_Execute_SQL(strSQL.Replace(TableName, TableName & "_pending"), False)
                    '        g_nvcCreateDatabaseColumns(Mid(rynRadionList.ID, 4).Replace("_" & strColn, "")) = strSessionPrefix & rynRadionList.ID.Replace("_" & strColn, "")
                    '    Else
                    '        ' need to store how many columns in the display of selection lists on the form
                    '        g_nvcCreateDatabaseColumns("VIEWONLY" & strSessionPrefix & rynRadionList.ID.Replace(strColn, "_FormColumnCount")) = UCase(strColn).Replace("COL", "")
                    '    End If
                    '    End If
            End Select
            ReDim Preserve arrControls(arrControls.Count - 1)

        Next

        ' build views 
        Dim strViewName As String = TableName & "_FormMap_vw"
        Dim strRequiredFieldsViewName As String = TableName & "_RequiredFields_vw"

        If g_IO_Execute_SQL("SELECT name FROM sys.views WHERE object_id = OBJECT_ID('" & strViewName & "')", False).Rows.Count = 0 Then
        Else
            g_IO_Execute_SQL("drop view " & strViewName, False)
        End If


        If g_IO_Execute_SQL("SELECT name FROM sys.views WHERE object_id = OBJECT_ID('" & strRequiredFieldsViewName & "')", False).Rows.Count = 0 Then
        Else
            g_IO_Execute_SQL("drop view " & strRequiredFieldsViewName, False)
        End If


        Dim strViewColumns As String = ""

        For Each strColumnName As String In g_nvcCreateDatabaseColumns.Keys
            If Left(strColumnName, 8) = "VIEWONLY" Then
                ' create a field in the view that does not exist in the database
                strViewColumns &= "," & g_nvcCreateDatabaseColumns(strColumnName) & " as " & strColumnName.Replace("VIEWONLY", "")
            Else
                strViewColumns &= "," & strColumnName & " as " & g_nvcCreateDatabaseColumns(strColumnName)
            End If

        Next
        g_IO_Execute_SQL("Create view " & strViewName & " as select " & strBaseColumnList & strViewColumns & " from " & TableName, True)


        ' build VIEW that contains the names and locations of required fields
        strViewColumns = ""
        Dim strViewColumnsDelim As String = ""
        For Each strColumnName As String In g_nvcCreateReguiredFieldView.Keys
            strViewColumns &= strViewColumnsDelim & "'" & g_nvcCreateReguiredFieldView(strColumnName) & "' as " & strColumnName
            strViewColumnsDelim = ","
        Next
        g_IO_Execute_SQL("Create view " & strRequiredFieldsViewName & " as select " & strViewColumns, True)



    End Sub
    Private Sub CreateDropdownListTable(ByVal selectionControl As Control)
        Dim blnIOCheck As Boolean = False

        Dim strTableType As String = ""
        If TypeOf (selectionControl) Is CheckBoxList Then
            strTableType = "CheckBoxList_"
        Else
            strTableType = "DropDownList_"
        End If


        Dim strTableName As String = strTableType & "_" & Mid(selectionControl.ID, 4)

        If selectionControl.ID.Contains("Col1") Or Not selectionControl.ID.Contains("_Col") Then
            strTableName = strTableName.Replace("_Col1", "")
            Dim strSQL As String = "Select top 1 * from " & strTableName
            g_IO_Execute_SQL(strSQL, blnIOCheck)
            If blnIOCheck Then
            Else
                g_IO_Execute_SQL("CREATE TABLE " & strTableName & " ([RECID] [int] IDENTITY(1,1) NOT NULL,[DESCR] [varchar](45) NULL default ''," & _
                                 "CONSTRAINT [PK_" & strTableName & "] PRIMARY KEY CLUSTERED ([RECID] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF," & _
                                 " IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]", _
                                 False)

            End If
        End If
    End Sub
    Public Sub g_loadSessionsToPage(ByRef ContentHolder As ContentPlaceHolder, ByVal FormCode As String, ByVal ModuleCode As String)
        Call SessionToFormAndFormToSessions(ContentHolder, FormCode, ModuleCode, "SF")
    End Sub
    Public Sub g_loadPageToSessions(ByRef ContentHolder As ContentPlaceHolder, ByVal FormCode As String, ByVal ModuleCode As String)
        Call SessionToFormAndFormToSessions(ContentHolder, FormCode, ModuleCode, "FS")
    End Sub

    Private Sub SessionToFormAndFormToSessions(ByRef ContentHolder As ContentPlaceHolder, ByVal FormCode As String, ByVal ModuleCode As String, ByVal TypeMove_FS_or_SF As String)

        Dim ctlControl As WebControl = Nothing

        '        Dim litAutoCoderLiteral As Literal = ContentHolder.FindControl("litAutoCoder")
        '        Dim strCommonScripts As String = _
        '         " <script type=""text/javascript"">" & _
        '"                function CheckboxLikeRadio(chkBxLst) {" & _
        '"        chkBxLstPart = chkBxLst.id.substr(0, chkBxLst.id.indexOf(""_Col""));" & _
        '"                            jQuery('[id*="" ' + chkBxLstPart + '""]').each(function (i, obj) {" & _
        '"                                        obj.checked = """";" & _
        '"                        });}" & _
        '"                    </script>"

        'litAutoCoderLiteral.Text &= strCommonScripts

        Dim strSessionPrefix As String = ModuleCode & "_" & FormCode & "__"
        Dim intPrefixLength As Integer = Len(strSessionPrefix)

        Dim strFormObjectName As String = ""
        Dim arrSessionKeys(System.Web.HttpContext.Current.Session.Keys.Count - 1) As String

        For i = 0 To System.Web.HttpContext.Current.Session.Keys.Count - 1
            arrSessionKeys(i) = System.Web.HttpContext.Current.Session.Keys(i)
        Next

        Dim blnViewingUpdatedInformation As Boolean = False
        Dim strColorToHighlightChangesOnForm As String = "yellow"
        If IsNothing(System.Web.HttpContext.Current.Session(g_ModuleCode & "_data__Status")) OrElse System.Web.HttpContext.Current.Session(g_ModuleCode & "_data__Status") = "N" OrElse System.Web.HttpContext.Current.Session(g_ModuleCode & "_data__Status") = "A" Then
        Else
            ' the information being displayed contains pending data, flag any pending changes
            blnViewingUpdatedInformation = True
        End If


        For Each strSessionKey In arrSessionKeys
            If Left(strSessionKey, intPrefixLength) = strSessionPrefix Then
                strFormObjectName = Mid(strSessionKey, intPrefixLength + 1)
                ctlControl = Nothing
                Dim strControlName As String = ""
                If strFormObjectName.Contains("ryn") Then
                    If Right(strFormObjectName, 3) = "Yes" Then
                        strControlName = strFormObjectName.Substring(0, strFormObjectName.Length - 3)
                    ElseIf Right(strFormObjectName, 2) = "No" Then
                        strControlName = strFormObjectName.Substring(0, strFormObjectName.Length - 2)
                    End If
                    ctlControl = ContentHolder.FindControl(strControlName)
                ElseIf Not strFormObjectName.Contains("_COL") Then
                    ctlControl = ContentHolder.FindControl(strFormObjectName.Replace("__FormColumnCount", "_col1"))
                End If

                If IsNothing(ctlControl) Then
                Else

                    Dim blnFlagControlsThatHavePendingChanges As Boolean = System.Web.HttpContext.Current.Session(strSessionKey.Replace("_" & FormCode, "") & "__Status") And _
                                                                    System.Web.HttpContext.Current.Session(g_ModuleCode & "_Data__Status") = "P"

                    ' populate form controls with users settings
                    Select Case Left(strFormObjectName, 3)
                        Case "txt"
                            Dim txtTextBox As TextBox = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = txtTextBox.Text
                            Else
                                txtTextBox.Text = System.Web.HttpContext.Current.Session(strSessionKey)
                                If blnFlagControlsThatHavePendingChanges Then txtTextBox.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "int"
                            Dim intTextBox As TextBox = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = intTextBox.Text
                            Else
                                intTextBox.Text = System.Web.HttpContext.Current.Session(strSessionKey)
                                If blnFlagControlsThatHavePendingChanges Then intTextBox.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "num"
                            Dim numTextBox As TextBox = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = numTextBox.Text
                            Else
                                numTextBox.Text = System.Web.HttpContext.Current.Session(strSessionKey)
                                If blnFlagControlsThatHavePendingChanges Then numTextBox.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "dol"
                            Dim numTextBox As TextBox = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = numTextBox.Text
                            Else
                                If IsNumeric(System.Web.HttpContext.Current.Session(strSessionKey)) Then
                                    numTextBox.Text = Format(CSng(System.Web.HttpContext.Current.Session(strSessionKey)), "currency").Replace("$", "").Replace(",", "")
                                End If
                                If blnFlagControlsThatHavePendingChanges Then numTextBox.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "rdo"
                            Dim rdoRadioButton As RadioButton = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = IIf(rdoRadioButton.Checked = True, "X", " ")
                            Else
                                rdoRadioButton.Checked = IIf(System.Web.HttpContext.Current.Session(strSessionKey) = "X", True, False)
                                If blnFlagControlsThatHavePendingChanges Then rdoRadioButton.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "ryn"
                            Dim rynRadioButtonList As RadioButtonList = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                If strSessionKey.Contains("Yes") Then
                                    System.Web.HttpContext.Current.Session(strSessionKey) = IIf(rynRadioButtonList.Items(0).Selected, "X", " ")
                                Else
                                    System.Web.HttpContext.Current.Session(strSessionKey) = IIf(rynRadioButtonList.Items(1).Selected, "X", " ")
                                End If

                            Else
                                If rynRadioButtonList.Items.Count = 0 Then
                                    rynRadioButtonList.RepeatDirection = RepeatDirection.Horizontal
                                    Dim lstItemYes As New ListItem
                                    lstItemYes.Text = "Yes"
                                    lstItemYes.Value = 0
                                    rynRadioButtonList.Items.Add(lstItemYes)
                                    Dim lstItemNo As New ListItem
                                    lstItemNo.Text = "No"
                                    lstItemNo.Value = 1
                                    rynRadioButtonList.Items.Add(lstItemNo)
                                End If
                                If strSessionKey.Contains("Yes") Then
                                    rynRadioButtonList.Items(0).Selected = IIf(System.Web.HttpContext.Current.Session(strSessionKey) = "X", True, rynRadioButtonList.Items(0).Selected)
                                Else
                                    rynRadioButtonList.Items(1).Selected = IIf(System.Web.HttpContext.Current.Session(strSessionKey) = "X", True, rynRadioButtonList.Items(1).Selected)
                                End If
                                If blnFlagControlsThatHavePendingChanges Then rynRadioButtonList.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "chk"
                            Dim chkCheckbox As CheckBox = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = IIf(chkCheckbox.Checked = True, "X", " ")
                            Else
                                chkCheckbox.Checked = IIf(System.Web.HttpContext.Current.Session(strSessionKey) = "X", True, False)
                                If blnFlagControlsThatHavePendingChanges Then chkCheckbox.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "sel"
                            Dim selDropDownList As DropDownList = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = selDropDownList.SelectedValue
                            Else
                                selDropDownList.SelectedValue = System.Web.HttpContext.Current.Session(strSessionKey)
                                If blnFlagControlsThatHavePendingChanges Then selDropDownList.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "ddl"
                            Dim selDropDownList As DropDownList = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = selDropDownList.SelectedValue
                            Else
                                selDropDownList.SelectedValue = System.Web.HttpContext.Current.Session(strSessionKey)
                                If blnFlagControlsThatHavePendingChanges Then selDropDownList.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "cbo"
                            Dim selDropDownList As DropDownList = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = selDropDownList.SelectedValue
                            Else
                                selDropDownList.SelectedValue = System.Web.HttpContext.Current.Session(strSessionKey)
                                If blnFlagControlsThatHavePendingChanges Then selDropDownList.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "dte"
                            Dim txtTextBox As TextBox = ctlControl
                            If TypeMove_FS_or_SF = "FS" Then
                                System.Web.HttpContext.Current.Session(strSessionKey) = txtTextBox.Text
                            Else
                                txtTextBox.Text = System.Web.HttpContext.Current.Session(strSessionKey)
                                If blnFlagControlsThatHavePendingChanges Then txtTextBox.Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                            End If
                        Case "lst"
                            ''Singular check box list
                            Dim strFormControlBaseName As String = strSessionKey.Replace("__FormColumnCount", "") & "_col"
                            Dim strSubTableName As String = Mid(strFormControlBaseName.Replace(strSessionPrefix, ""), 4).Replace("_col", "")

                            If TypeMove_FS_or_SF = "FS" Then

                                If strSessionKey.Contains("__FormColumnCount") Then

                                    Dim intDisplayColumnsCount As Integer = System.Web.HttpContext.Current.Session(strSessionKey)
                                    System.Web.HttpContext.Current.Session(strSessionPrefix & "lst" & strSubTableName) = ""
                                    Dim strLstDelimiter As String = ""
                                    For j = 1 To intDisplayColumnsCount
                                        Dim lstControl As CheckBoxList = ContentHolder.FindControl(strFormControlBaseName.Replace(strSessionPrefix, "") & j)
                                        For i = 0 To lstControl.Items.Count - 1
                                            If lstControl.Items(i).Selected Then
                                                System.Web.HttpContext.Current.Session(g_ModuleCode & "_lst" & strSubTableName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_SysUserRECID") & "__" & lstControl.Items(i).Value) = True
                                                System.Web.HttpContext.Current.Session(strSessionPrefix & "lst" & strSubTableName) &= strLstDelimiter & lstControl.Items(i).Value
                                                strLstDelimiter = ","
                                            Else
                                                System.Web.HttpContext.Current.Session(g_ModuleCode & "_lst" & strSubTableName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_SysUserRECID") & "__" & lstControl.Items(i).Value) = False
                                            End If
                                        Next
                                    Next
                                End If
                            Else
                                If strSessionKey.Contains("__FormColumnCount") Then

                                    blnFlagControlsThatHavePendingChanges = System.Web.HttpContext.Current.Session(strSessionKey.Replace("_" & FormCode, "").Replace("__FormColumnCount", "") & "__Status") And _
                                                                    System.Web.HttpContext.Current.Session(g_ModuleCode & "_Data__Status") = "P"

                                    ' read data from database
                                    Dim intDisplayColumnsCount As Integer = System.Web.HttpContext.Current.Session(strSessionKey)
                                    Dim tblUserDefined As DataTable = Nothing

                                    Dim tblMultipleChoiceBoxes = g_LoadReportTableFromSessionVariablesConditions(strSubTableName, "", intDisplayColumnsCount)

                                    Dim arrCheckboxLists(intDisplayColumnsCount - 1) As CheckBoxList
                                    For i = 0 To intDisplayColumnsCount - 1
                                        arrCheckboxLists(i) = ContentHolder.FindControl(strFormControlBaseName.Replace(strSessionPrefix, "") & i + 1)
                                    Next

                                    For Each rowMultipleChoiceBoxes In tblMultipleChoiceBoxes.Rows

                                        Dim arrCheckboxListItems(intDisplayColumnsCount - 1) As ListItem
                                        For i = 0 To intDisplayColumnsCount - 1
                                            If rowMultipleChoiceBoxes("recid" & i + 1) = "-1" Then
                                            Else
                                                arrCheckboxListItems(i) = New ListItem
                                                arrCheckboxListItems(i).Text = "&nbsp;&nbsp;" & rowMultipleChoiceBoxes("desc" & i + 1).replace("(", "<br />(")
                                                arrCheckboxListItems(i).Selected = IIf(rowMultipleChoiceBoxes("Yes" & i + 1) = "X", True, False)
                                                arrCheckboxListItems(i).Value = rowMultipleChoiceBoxes("recid" & i + 1)
                                                arrCheckboxLists(i).Items.Add(arrCheckboxListItems(i))


                                                'does this box need to be highlighted as changed?
                                                If ("," & System.Web.HttpContext.Current.Session(g_ModuleCode & "_lst" & strSubTableName & "__Original") & ",").contains("," & rowMultipleChoiceBoxes("recid" & i + 1) & ",") Then
                                                    ' this entry was selected before user started the updates
                                                    If arrCheckboxListItems(i).Selected Then
                                                        ' this entry is currently selected
                                                    Else
                                                        ' this entry is currently not selected (that's a change)
                                                        If blnFlagControlsThatHavePendingChanges Then
                                                            arrCheckboxListItems(i).Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                                                        End If
                                                    End If
                                                Else
                                                    ' this entry was not selected before
                                                    If arrCheckboxListItems(i).Selected Then
                                                        ' this entry is currently selected
                                                        If blnFlagControlsThatHavePendingChanges Then
                                                            arrCheckboxListItems(i).Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                                                        End If
                                                    Else
                                                        ' this entry is currently not selected (that's a change)
                                                    End If
                                                End If

                                            End If
                                        Next
                                    Next
                                End If

                            End If
                        Case "cyn"
                            ''Force a YES / NO check box list
                            Dim strFormControlBaseName As String = strSessionKey.Replace("__FormColumnCount", "") & "_col"   ' extract the form objects base name from the session key
                            Dim strSubTableName As String = Mid(strFormControlBaseName.Replace(strSessionPrefix, ""), 4).Replace("_col", "") ' extract table name that contains options from the field name

                            If TypeMove_FS_or_SF = "FS" Then
                                ''FS - Form to Sessions
                                ''SF - Sessions to Form
                                If strSessionKey.Contains("__FormColumnCount") Then

                                    Dim intDisplayColumnsCount As Integer = System.Web.HttpContext.Current.Session(strSessionKey)
                                    System.Web.HttpContext.Current.Session(strSessionPrefix & "cyn" & strSubTableName) = ""
                                    Dim strcynDelimiter As String = ""
                                    For j = 1 To intDisplayColumnsCount
                                        Dim cynControl As CheckBoxList = ContentHolder.FindControl(strFormControlBaseName.Replace(strSessionPrefix, "") & j)
                                        For i = 0 To cynControl.Items.Count - 1
                                            If cynControl.Items(i).Selected Then
                                                System.Web.HttpContext.Current.Session(g_ModuleCode & "_cyn" & strSubTableName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_SysUserRECID") & "__" & cynControl.Items(i).Value) = True
                                                System.Web.HttpContext.Current.Session(strSessionPrefix & "cyn" & strSubTableName) &= strcynDelimiter & cynControl.Items(i).Value
                                            Else
                                                Dim cynNoControl As CheckBoxList = ContentHolder.FindControl(strFormControlBaseName.Replace(strSessionPrefix, "") & j & "No")
                                                If cynNoControl.Items(i).Selected Then
                                                    System.Web.HttpContext.Current.Session(g_ModuleCode & "_cyn" & strSubTableName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_SysUserRECID") & "__" & cynControl.Items(i).Value) = False
                                                    System.Web.HttpContext.Current.Session(strSessionPrefix & "cyn" & strSubTableName) &= strcynDelimiter & "-" & cynControl.Items(i).Value
                                                End If
                                            End If
                                            strcynDelimiter = ","
                                        Next
                                    Next
                                End If
                            Else
                                If strSessionKey.Contains("__FormColumnCount") Then

                                    ' set a flag that indicates there are pending changes on this record and that there is a pending change on this field
                                    blnFlagControlsThatHavePendingChanges = System.Web.HttpContext.Current.Session(strSessionKey.Replace("_" & FormCode, "").Replace("__FormColumnCount", "") & "__Status") And _
                                                                    System.Web.HttpContext.Current.Session(g_ModuleCode & "_Data__Status") = "P"

                                    ' read data from database

                                    ' how many columns on the form are used to display these checkboxes?
                                    Dim intDisplayColumnsCount As Integer = System.Web.HttpContext.Current.Session(strSessionKey)

                                    ' read the data from the options table that will populate these checkbox choices
                                    Dim tblChkBoxChoicesFromDatabase = g_LoadReportTableFromSessionVariablesConditions(strSubTableName, "", intDisplayColumnsCount, "cyn")

                                    ' extract from the form each checkboxlist (column on the form) by building the names from the base name
                                    '  and store a reference to each in an array
                                    Dim arrCheckboxLists(intDisplayColumnsCount - 1) As CheckBoxList
                                    Dim arrCheckboxListsNo(intDisplayColumnsCount - 1) As CheckBoxList
                                    For i = 0 To intDisplayColumnsCount - 1
                                        arrCheckboxLists(i) = ContentHolder.FindControl(strFormControlBaseName.Replace(strSessionPrefix, "") & i + 1)
                                        arrCheckboxListsNo(i) = ContentHolder.FindControl((strFormControlBaseName).Replace(strSessionPrefix, "") & i + 1 & "No")
                                    Next

                                    ' set a flag indicating that this patient is here for the first time (all checkboxes are unselected)
                                    Dim blnFistTimeIn As Boolean = IIf(System.Web.HttpContext.Current.Session(strFormControlBaseName.Replace("_col", "")) = "", True, False)

                                    ' loop through all the options presented from the options table read from the database
                                    For Each rowMultipleChoiceBoxes In tblChkBoxChoicesFromDatabase.Rows

                                        Dim arrCheckboxListItems(intDisplayColumnsCount - 1) As ListItem   ' Each YES checkbox within the checkboxlist to
                                        Dim arrCheckboxListItemsNo(intDisplayColumnsCount - 1) As ListItem   ' Each NO checkbox within the checkboxlist to

                                        For i = 0 To intDisplayColumnsCount - 1
                                            If rowMultipleChoiceBoxes("recid" & i + 1) = "-1" Then
                                            Else
                                                ' create the YES box on the form for each option
                                                arrCheckboxListItems(i) = New ListItem
                                                arrCheckboxListItems(i).Text = ""
                                                If blnFistTimeIn Then
                                                Else
                                                    arrCheckboxListItems(i).Selected = IIf(rowMultipleChoiceBoxes("Yes" & i + 1) = "X", True, False)
                                                End If
                                                arrCheckboxListItems(i).Value = rowMultipleChoiceBoxes("recid" & i + 1)

                                                arrCheckboxListItems(i).Attributes.Add("onclick", "cynClicked(this, 'yes', 'cynConditionsHistoryDiv')")

                                                arrCheckboxLists(i).Items.Add(arrCheckboxListItems(i))

                                                ' create the NO box on the form for each option
                                                arrCheckboxListItemsNo(i) = New ListItem
                                                arrCheckboxListItemsNo(i).Text = rowMultipleChoiceBoxes("desc" & i + 1)
                                                If blnFistTimeIn Then
                                                Else
                                                    arrCheckboxListItemsNo(i).Selected = IIf(rowMultipleChoiceBoxes("No" & i + 1) = "X", True, False)
                                                End If
                                                arrCheckboxListItemsNo(i).Value = rowMultipleChoiceBoxes("recid" & i + 1)

                                                arrCheckboxListItemsNo(i).Attributes.Add("onclick", "cynClicked(this, 'no', 'cynConditionsHistoryDiv')")

                                                arrCheckboxListsNo(i).Items.Add(arrCheckboxListItemsNo(i))

                                                'does this box need to be highlighted as changed?
                                                If ("," & System.Web.HttpContext.Current.Session(g_ModuleCode & "_cyn" & strSubTableName & "__Original") & ",").contains("," & rowMultipleChoiceBoxes("recid" & i + 1) & ",") Then
                                                    ' this entry was selected before user started the updates
                                                    If arrCheckboxListItems(i).Selected Then
                                                        ' this entry is currently selected
                                                    Else
                                                        ' this entry is currently not selected (that's a change)
                                                        If blnFlagControlsThatHavePendingChanges Then
                                                            arrCheckboxListItems(i).Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                                                            arrCheckboxListItemsNo(i).Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                                                        End If
                                                    End If
                                                Else
                                                    ' this entry was not selected before
                                                    If arrCheckboxListItems(i).Selected Then
                                                        ' this entry is currently selected
                                                        If blnFlagControlsThatHavePendingChanges Then
                                                            arrCheckboxListItems(i).Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                                                            arrCheckboxListItemsNo(i).Attributes.Add("Style", "background-color: " & strColorToHighlightChangesOnForm)
                                                        End If
                                                    Else
                                                        ' this entry is currently not selected (that's a change)
                                                    End If
                                                End If

                                            End If
                                        Next
                                    Next
                                End If
                            End If

                    End Select
                End If
            End If
        Next
    End Sub

    Public Function g_LoadReportTableFromSessionVariablesConditions(ByVal TableName As String, ByVal SubReportTitle As String, ByVal colCount As Integer)
        Return g_LoadReportTableFromSessionVariablesConditions(TableName, SubReportTitle, colCount, "lst")
    End Function

    Public Function g_LoadReportTableFromSessionVariablesConditions(ByVal TableName As String, ByVal SubReportTitle As String, ByVal colCount As Integer, ByVal objType As String) As DataTable

        Dim tblPatientTableNames As New DataTable
        tblPatientTableNames.Columns.Add("Title", GetType(String))
        tblPatientTableNames.Columns.Add("recid1", GetType(Integer))
        tblPatientTableNames.Columns.Add("Desc1", GetType(String))
        tblPatientTableNames.Columns.Add("Yes1", GetType(String))
        tblPatientTableNames.Columns.Add("No1", GetType(String))
        tblPatientTableNames.Columns.Add("recid2", GetType(Integer))
        tblPatientTableNames.Columns.Add("Desc2", GetType(String))
        tblPatientTableNames.Columns.Add("Yes2", GetType(String))
        tblPatientTableNames.Columns.Add("No2", GetType(String))
        tblPatientTableNames.Columns.Add("recid3", GetType(Integer))
        tblPatientTableNames.Columns.Add("Desc3", GetType(String))
        tblPatientTableNames.Columns.Add("Yes3", GetType(String))
        tblPatientTableNames.Columns.Add("No3", GetType(String))


        Dim strSQL As String = "Select RECID, DESCR from CheckBoxList__" & TableName & " order by descr"

        Dim tblTableNames As DataTable = g_IO_Execute_SQL(strSQL, False)

        Dim rowTableName As DataRow = Nothing
        Dim intTableNameCount As Integer = 0
        Try
            intTableNameCount = tblTableNames.Rows.Count
        Catch ex As Exception

        End Try

        Dim intCalcRows As Integer = Math.Ceiling(intTableNameCount / colCount)
        For intRow = 0 To intCalcRows - 1
            rowTableName = tblPatientTableNames.NewRow
            rowTableName("Title") = SubReportTitle

            For j = 1 To colCount
                rowTableName("Recid" & j) = "-1"
                rowTableName("Desc" & j) = ""
                rowTableName("Yes" & j) = ""
                rowTableName("No" & j) = ""
                Try
                    Dim intIndex = (j - 1) * intCalcRows + intRow
                    rowTableName("recid" & j) = tblTableNames.Rows(intIndex)("recid")
                    rowTableName("Desc" & j) = tblTableNames.Rows(intIndex)("descr")
                    ''Rodney-is this still needed (lines 910-922) 
                    ''''P_T3__cynConditionsHistory
                    If IsNothing(System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & objType & TableName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_SysUserRECID") & "__" & tblTableNames.Rows(intIndex)("RECID"))) Then
                        ' 10222013 - removed to prevent the options of NO from defaulting if patient did not go to form while signed on.
                        'If IsNothing(System.Web.HttpContext.Current.Session("user_link_id")) Or (Not IsNothing(System.Web.HttpContext.Current.Session(g_ModuleCode &  "_" & objType & "cynConditionsHistory")) AndAlso System.Web.HttpContext.Current.Session(g_ModuleCode &  "_" & objType & "cynConditionsHistory") = "") Then
                        'Else
                        '    rowTableName("No" & j) = "X"
                        'End If
                    Else
                        If System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & objType & TableName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_SysUserRECID") & "__" & tblTableNames.Rows(intIndex)("RECID")) Then
                            rowTableName("Yes" & j) = "X"
                        Else
                            rowTableName("No" & j) = "X"
                        End If
                    End If

                Catch
                End Try
            Next

            tblPatientTableNames.Rows.Add(rowTableName)
        Next
        Return tblPatientTableNames
    End Function

    Public Sub g_DefaultAllSessionVariables(ByVal TableName As String, ByVal FormCode As String, ByVal ModuleCode As String, ByVal PrimaryKey As String, ByVal UsersRecid As Integer)

        Dim strSessionPrefix As String = ModuleCode & "_" & FormCode & "__"
        Dim intPrefixLength As Integer = Len(strSessionPrefix)
        Dim strSQL = ""

        ' if available, default some items from the appointment form session("A_...")


        ' is table already in place, if so look to add the fields only
        Dim tblColumnDefaults = g_getData(TableName & "_formmap_vw", "recid=0")
        g_IO_Execute_SQL("delete " & TableName & " where recid=0", False)

        If tblColumnDefaults.Rows.Count = 0 Then
            strSQL = "set identity_insert " & TableName & " on; Insert into " & TableName & " (recid) values(0);"
            tblColumnDefaults = g_IO_Execute_SQL(strSQL, False)  '  create a row with all defaults set
            tblColumnDefaults = g_getData(TableName & "_formmap_vw", "recid=0")
            g_IO_Execute_SQL("delete " & TableName & " where recid=0", False)
        End If

        ' remove old session variables for individual list items
        Dim intIndex As Integer = System.Web.HttpContext.Current.Session.Keys.Count - 1
        For intIndex = intIndex To 0 Step -1
            If System.Web.HttpContext.Current.Session.Keys(intIndex).IndexOf(ModuleCode & "_lst") = 0 Then
                System.Web.HttpContext.Current.Session.Remove(System.Web.HttpContext.Current.Session.Keys(intIndex))
            End If
        Next

        Call g_LoadTableInfoFromDatabase(TableName, tblColumnDefaults, FormCode, ModuleCode, UsersRecid)

    End Sub


    Public Sub g_LoadDropDowns(ByVal TableName As String, ByRef ContentHolder As ContentPlaceHolder)

        ' load tables that are set up for auto loading
        Dim strSQL =
            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME like '%list__%' ORDER BY TABLE_NAME"

        Dim tblLists As DataTable = g_IO_Execute_SQL(strSQL, False)
        strSQL = "SELECT Column_NAME FROM INFORMATION_SCHEMA.columns where table_name = '" & TableName & "_FormMap_vw' and (column_name like '%[__]cbo%' or  column_name like '%[__]ddl%' or  column_name like '%[__]sel%')"
        Dim tblWebObjects As DataTable = g_IO_Execute_SQL(strSQL, False)

        If tblWebObjects.Rows.Count = 0 Then
        Else

            For Each rowColumnName As DataRow In tblWebObjects.Rows
                Dim strObjectName As String = rowColumnName("Column_name").Replace(Split(rowColumnName("Column_name"), "__")(0) & "__", "")
                Dim ctlControl As Control = ContentHolder.FindControl(strObjectName)
                Dim strTableName As String = ""
                If TypeOf (ctlControl) Is CheckBoxList Then
                    strTableName = "CheckBoxList__" & strObjectName.Substring(3)
                ElseIf TypeOf (ctlControl) Is DropDownList Then
                    strTableName = "DropDownList__" & strObjectName.Substring(3)
                End If

                ' does this table exist?
                For Each rowTableList As DataRow In tblLists.Rows
                    If rowTableList("Table_name") = strTableName Then '

                        ' load table to drop down
                        Dim blnFoundTheTable As Boolean = False
                        Dim tblSelections As DataTable = g_IO_Execute_SQL("select * from " & strTableName & " order by Descr", blnFoundTheTable)
                        If blnFoundTheTable Then
                            ' is this an auto built table or one that is expected to auto load, must have a recid and Desc column

                            If TypeOf (ctlControl) Is CheckBoxList Then
                                CType(ctlControl, CheckBoxList).DataSource = tblSelections
                                CType(ctlControl, CheckBoxList).DataValueField = "recid"
                                CType(ctlControl, CheckBoxList).DataTextField = "descr"
                                CType(ctlControl, CheckBoxList).DataBind()
                            ElseIf TypeOf (ctlControl) Is DropDownList Then

                                If UCase(CType(ctlControl, DropDownList).CssClass).Contains("PROMPTSELECTION") Then
                                    'add option to dropdown
                                    Dim rowSelection As DataRow = tblSelections.NewRow
                                    rowSelection("recid") = -1
                                    rowSelection("descr") = "Choose an option"
                                    tblSelections.Rows.InsertAt(rowSelection, 0)
                                End If

                                CType(ctlControl, DropDownList).DataSource = tblSelections
                                CType(ctlControl, DropDownList).DataValueField = "recid"
                                CType(ctlControl, DropDownList).DataTextField = "descr"
                                CType(ctlControl, DropDownList).DataBind()
                            End If

                            Exit For
                        End If
                    End If
                Next

            Next

        End If

    End Sub

    Public Function g_getData(ByVal TableName As String, ByVal WhereClause As String) As DataTable
        ' build sql to extract fields from table.  Each row will contain the SQL entry for each column and the column updated status


        Dim strSQL As String = "Select * from " & TableName & " where " & WhereClause

        Dim tblData = g_IO_Execute_SQL(strSQL, False)

        Return tblData
    End Function
    Public Sub g_LoadTableInfoFromDatabase(ByVal TableName As String, ByVal WhereClause As String, ByVal FormCode As String, ByVal ModuleCode As String, ByVal UsersRecid As Integer)
        g_LoadTableInfoFromDatabase(TableName, WhereClause, Nothing, FormCode, ModuleCode, UsersRecid)
    End Sub
    Public Sub g_LoadTableInfoFromDatabase(ByVal TableName As String, ByRef PreLoadedTable As DataTable, ByVal FormCode As String, ByVal ModuleCode As String, ByVal UsersRecid As Integer)
        g_LoadTableInfoFromDatabase(TableName, "", PreLoadedTable, FormCode, ModuleCode, UsersRecid)
    End Sub

    Private Sub g_LoadTableInfoFromDatabase(ByVal TableName As String, ByVal WhereClause As String, ByRef PreLoadedTable As DataTable, ByVal FormCode As String, ByVal ModuleCode As String, ByVal UsersRecid As Integer)
        ''''''Rodney-Add overload for preloaded table''''''

        Dim strSessionPrefix As String = ModuleCode & "_" & FormCode
        Dim intPrefixLength As Integer = Len(strSessionPrefix)
        Dim tblTable As DataTable = Nothing

        Dim rowPatient As DataRow = Nothing
        Dim strSQL As String = ""
        If IsNothing(PreLoadedTable) Then
            tblTable = g_getData(TableName & "_formmap_vw", WhereClause)
        Else
            tblTable = PreLoadedTable ' programmer sent a preread table in
            If tblTable.Rows.Count = 0 Then
                WhereClause = "recid=-1"
            Else
                WhereClause = "recid=" & tblTable.Rows(0)("recid")
            End If
        End If


        If tblTable.Rows.Count = 0 Then
            ' whatever has been keyed, everything else takes defaults set in database
            Call g_PostTablePageToDatabase(TableName, WhereClause, FormCode, ModuleCode, UsersRecid)


        Else
            ' this user has signed on and has already filled out data in the past, probably returning to print forms or make changes.

            Dim strDate As String = ""
            rowPatient = tblTable.Rows(0)

            Dim strCurrentControlName As String = ""

            For Each strColumn As DataColumn In tblTable.Columns

                strCurrentControlName = Mid(strColumn.ColumnName, strColumn.ColumnName.IndexOf("__") + 3)

                'If strColumn.ColumnName.Contains("__Status") Then
                '    System.Web.HttpContext.Current.Session(strColumn.ColumnName.Replace("_" & FormCode, "")) = rowPatient(strColumn.ColumnName)
                'Else
                Select Case Left(strCurrentControlName, 3)
                    Case "dte"
                        Try : System.Web.HttpContext.Current.Session(strColumn.ColumnName) = IIf(IsDate(rowPatient(strColumn.ColumnName)), Format(CType(rowPatient(strColumn.ColumnName), Date), "MM/dd/yyyy"), "") : Catch : System.Web.HttpContext.Current.Session(strColumn.ColumnName) = "" : End Try
                    Case "rdo"
                        System.Web.HttpContext.Current.Session(strColumn.ColumnName) = IIf(rowPatient(strColumn.ColumnName), "X", " ")
                    Case "ryn"
                        System.Web.HttpContext.Current.Session(strColumn.ColumnName) = IIf(rowPatient(strColumn.ColumnName), "X", " ")
                    Case "chk"
                        System.Web.HttpContext.Current.Session(strColumn.ColumnName) = IIf(rowPatient(strColumn.ColumnName), "X", " ")
                    Case "lst"
                        If strColumn.ColumnName.Contains("__FormColumnCount") Then
                            System.Web.HttpContext.Current.Session(strColumn.ColumnName) = rowPatient(strColumn.ColumnName)  ' tell form to load data from database even if no selections yet
                        Else
                            System.Web.HttpContext.Current.Session(strColumn.ColumnName) = rowPatient(strColumn.ColumnName)
                            'If rowPatient(strColumn.ColumnName & "__Status") = 0 Then
                            '    System.Web.HttpContext.Current.Session("P_" & strCurrentControlName & "__Original") = rowPatient(strColumn.ColumnName)
                            'Else
                            '    ' have changes, need original from active record
                            Dim strOriginal = ""
                            Try
                                strOriginal = g_IO_Execute_SQL("select " & strColumn.ColumnName & " from " & TableName & "_formmap_vw where " & WhereClause, False).Rows(0)(strColumn.ColumnName)
                            Catch ex As Exception
                            End Try
                            System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & strCurrentControlName & "__Original") = strOriginal
                            'End If

                            If rowPatient(strColumn) = "" Then
                            Else
                                For Each intRecidOfReferenceTable As Integer In Split(rowPatient(strColumn.ColumnName), ",")
                                    ' load each selection in the lst into an individual session variable tagged with this user's user_id
                                    ' be sure to strip off the prefix (Mid) or the system will think each selection stored is a control on the form and look for it
                                    System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & strCurrentControlName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & "SysUserRECID") & "__" & intRecidOfReferenceTable) = True
                                Next
                            End If
                        End If
                    Case "cyn"
                        If strColumn.ColumnName.Contains("__FormColumnCount") Then
                            System.Web.HttpContext.Current.Session(strColumn.ColumnName) = rowPatient(strColumn.ColumnName)  ' tell form to load data from database even if no selections yet
                        Else
                            System.Web.HttpContext.Current.Session(strColumn.ColumnName) = rowPatient(strColumn.ColumnName)

                            'If rowPatient(strColumn.ColumnName & "__Status") = 0 Then
                            '    System.Web.HttpContext.Current.Session("P_" & strCurrentControlName & "__Original") = rowPatient(strColumn.ColumnName)
                            'Else
                            ' have changes, need original from active record
                            Dim strOriginal = ""
                            Try
                                strOriginal = g_IO_Execute_SQL("select " & strColumn.ColumnName & " from " & TableName & "_formmap_vw where " & WhereClause, False).Rows(0)(strColumn.ColumnName)
                            Catch ex As Exception
                            End Try
                            System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & strCurrentControlName & "__Original") = strOriginal
                            ' End If

                            If rowPatient(strColumn) = "" Then
                            Else
                                For Each intRecidOfReferenceTable As Integer In Split(rowPatient(strColumn.ColumnName), ",")
                                    ' load each selection in the cyn into an individual session variable tagged with this user's user_id
                                    ' be sure to strip off the prefix (Mid) or the system will think each selection stored is a control on the form and look for it
                                    If intRecidOfReferenceTable < 0 Then
                                        System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & strCurrentControlName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & "SysUserRECID") & "__" & -intRecidOfReferenceTable) = False
                                    Else
                                        System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & strCurrentControlName & System.Web.HttpContext.Current.Session(g_ModuleCode & "_" & "SysUserRECID") & "__" & intRecidOfReferenceTable) = True
                                    End If
                                Next
                            End If
                        End If
                    Case Else
                        System.Web.HttpContext.Current.Session(strColumn.ColumnName) = rowPatient(strColumn.ColumnName)
                End Select
                '  End If
            Next
        End If

    End Sub

    Public Sub g_PostTablePageToDatabase(ByVal TableName As String, ByVal WhereClause As String, ByVal FormCode As String, ByVal ModuleCode As String, ByVal UsersRecid As Integer)

        Dim strSessionPrefix As String = ModuleCode & "_" & FormCode & "__"
        Dim intPrefixLength As Integer = Len(strSessionPrefix)


        If g_FormSessionsSet(FormCode, ModuleCode) Then

            Dim strChanges As String = ""

            Dim tblCurrentData = g_getData(TableName & "_formmap_vw", WhereClause)

            If tblCurrentData.Rows.Count = 0 Then

                Dim strFields As String = ""
                Dim strValues As String = ""
                For Each strSession As String In System.Web.HttpContext.Current.Session.Keys

                    If strSession.Contains("__Status") Then
                        ' each field has a generated-at-read (see ModMain.g_getPendingData) status field associated that indicates it has a pending change.  Don't let system try to save it.
                    Else
                        If strSession.Contains(ModuleCode & "_" & FormCode.Substring(0, 1)) Then
                            Dim arr_FieldParts() = Split(strSession, "__")
                            If arr_FieldParts.Count > 1 Then

                                Dim strFieldName = arr_FieldParts(1)
                                Select Case Left(strFieldName, 3)
                                    Case "txt"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= ",'" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                    Case "int"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & System.Web.HttpContext.Current.Session(strSession)
                                    Case "num"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & CType(System.Web.HttpContext.Current.Session(strSession).Replace(",", ""), String)
                                    Case "dol"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & CType(System.Web.HttpContext.Current.Session(strSession).Replace(",", ""), String)
                                    Case "rdo"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & IIf(System.Web.HttpContext.Current.Session(strSession) = "X", 1, 0)
                                    Case "ryn"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & IIf(System.Web.HttpContext.Current.Session(strSession) = "X", 1, 0)
                                    Case "chk"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & IIf(System.Web.HttpContext.Current.Session(strSession) = "X", 1, 0)
                                    Case "cbo"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & IIf(CType(System.Web.HttpContext.Current.Session(strSession), String) = "", -1, System.Web.HttpContext.Current.Session(strSession))
                                    Case "ddl"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & IIf(CType(System.Web.HttpContext.Current.Session(strSession), String) = "", -1, System.Web.HttpContext.Current.Session(strSession))
                                    Case "sel"
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & IIf(CType(System.Web.HttpContext.Current.Session(strSession), String) = "", -1, System.Web.HttpContext.Current.Session(strSession))
                                    Case "dte"
                                        Dim strDate As String = "NULL"
                                        If IsDate(System.Web.HttpContext.Current.Session(strSession)) Then
                                            Dim dteDate As Date = System.Web.HttpContext.Current.Session(strSession)
                                            strDate = "'" & Format(dteDate, "yyyy/MM/dd") & "'"
                                        End If
                                        strFields &= "," & Mid(strFieldName, 4)
                                        strValues &= "," & strDate
                                    Case "lst"
                                        If strSession.Contains("__FormColumnCount") Then
                                        Else
                                            strFields &= "," & Mid(strFieldName, 4)
                                            strValues &= ",'" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                        End If
                                    Case "cyn"
                                        If strSession.Contains("__FormColumnCount") Then
                                        Else
                                            strFields &= "," & Mid(strFieldName, 4)
                                            strValues &= ",'" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                        End If
                                    Case Else
                                        ' don't know what this is, do something with it.
                                        If IsNumeric(System.Web.HttpContext.Current.Session(strSession)) Then
                                            strFields &= "," & Mid(strFieldName, 4)
                                            strValues &= "," & System.Web.HttpContext.Current.Session(strSession)
                                        ElseIf IsDate(System.Web.HttpContext.Current.Session(strSession)) Then
                                            Dim strDate As String = "NULL"
                                            Dim dteDate As Date = System.Web.HttpContext.Current.Session(strSession)
                                            strDate = "'" & Format(dteDate, "yyyy/MM/dd") & "'"
                                            strFields &= "," & Mid(strFieldName, 4)
                                            strValues &= "," & strDate
                                        Else
                                            strFields &= "," & Mid(strFieldName, 4)
                                            strValues &= ",'" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                        End If

                                End Select
                            End If

                        End If
                    End If
                Next

                Dim strAddSQL = _
                        "INSERT INTO " & TableName & " (" & _
                            "Sys_Users_RECID" & _
                            strFields & _
                                 ") " & _
                        " VALUES(" & _
                        UsersRecid &
                        strValues & _
                             ")"

                g_IO_Execute_SQL(strAddSQL, False)
            Else
                ' user has returned to the screen to correct something, update it

                Dim strFields As String = ""
                Dim strDelim As String = ""

                For Each strSession As String In System.Web.HttpContext.Current.Session.Keys
                    If strSession.Contains("__Status") Then
                        ' each field has a generated-at-read (see ModMain.g_getPendingData) status field associated that indicates it has a pending change.  Don't let system try to save it.
                    Else
                        If strSession.Contains(ModuleCode & "_" & FormCode) Then
                            Dim strFieldName = Split(strSession, "__")(1)
                            Select Case Left(strFieldName, 3)
                                Case "txt"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = '" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                    strDelim = ","
                                Case "num"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & System.Web.HttpContext.Current.Session(strSession).Replace(",", "")
                                    strDelim = ","
                                Case "dol"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & System.Web.HttpContext.Current.Session(strSession).Replace(",", "")
                                    strDelim = ","
                                Case "int"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & CType(System.Web.HttpContext.Current.Session(strSession), String)
                                    strDelim = ","
                                Case "rdo"
                                    strFields &= strDelim & Mid(strFieldName, 4) & "= " & IIf(System.Web.HttpContext.Current.Session(strSession) = "X", 1, 0)
                                    strDelim = ","
                                Case "ryn"
                                    strFields &= strDelim & Mid(strFieldName, 4) & "= " & IIf(System.Web.HttpContext.Current.Session(strSession) = "X", 1, 0)
                                    strDelim = ","
                                Case "chk"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & IIf(System.Web.HttpContext.Current.Session(strSession) = "X", 1, 0)
                                    strDelim = ","
                                Case "sel"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & IIf(CType(System.Web.HttpContext.Current.Session(strSession), String) = "", -1, System.Web.HttpContext.Current.Session(strSession))
                                    strDelim = ","
                                Case "ddl"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & IIf(CType(System.Web.HttpContext.Current.Session(strSession), String) = "", -1, System.Web.HttpContext.Current.Session(strSession))
                                    strDelim = ","
                                Case "cbo"
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & IIf(CType(System.Web.HttpContext.Current.Session(strSession), String) = "", -1, System.Web.HttpContext.Current.Session(strSession))
                                    strDelim = ","
                                Case "dte"
                                    Dim strDate As String = "NULL"
                                    If IsDate(System.Web.HttpContext.Current.Session(strSession)) Then
                                        Dim dteDate As Date = System.Web.HttpContext.Current.Session(strSession)
                                        strDate = "'" & Format(dteDate, "yyyy/MM/dd") & "'"
                                    End If
                                    strFields &= strDelim & Mid(strFieldName, 4) & " = " & strDate
                                    strDelim = ","
                                Case "lst"
                                    If strSession.Contains("__FormColumnCount") Then
                                    Else
                                        strFields &= strDelim & Mid(strFieldName, 4) & " = '" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                        strDelim = ","
                                    End If
                                Case "cyn"
                                    If strSession.Contains("__FormColumnCount") Then
                                    Else
                                        strFields &= strDelim & Mid(strFieldName, 4) & " = '" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                        strDelim = ","
                                    End If
                                Case Else
                                    ' don't know what this is, do something with it.
                                    If IsNumeric(System.Web.HttpContext.Current.Session(strSession)) Then
                                        strFields &= strDelim & Mid(strFieldName, 4) & " = " & System.Web.HttpContext.Current.Session(strSession)
                                        strDelim = ","
                                    ElseIf IsDate(System.Web.HttpContext.Current.Session(strSession)) Then
                                        Dim strDate As String = "NULL"
                                        If IsDate(System.Web.HttpContext.Current.Session(strSession)) Then
                                            Dim dteDate As Date = System.Web.HttpContext.Current.Session(strSession)
                                            strDate = "'" & Format(dteDate, "yyyy/MM/dd") & "'"
                                        End If
                                        strFields &= strDelim & Mid(strFieldName, 4) & " = " & strDate
                                        strDelim = ","
                                    Else
                                        strFields &= strDelim & Mid(strFieldName, 4) & " = '" & System.Web.HttpContext.Current.Session(strSession).Replace("'", "''") & "'"
                                        strDelim = ","
                                    End If

                            End Select

                        End If
                    End If
                Next
                Dim strUpdateSQL = "update " & TableName & " set " & strFields & " Where " & WhereClause
                g_IO_Execute_SQL(strUpdateSQL, False)

            End If

        End If

    End Sub


End Module
