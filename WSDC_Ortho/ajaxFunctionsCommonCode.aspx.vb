Public Class ajaxFunctionsCommonCode
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim action As String = IIf(IsNothing(Request.QueryString("action")), Request.Form("action"), Request.QueryString("action"))
        '''''''''''''''''''''''''''''''''''''''

        Select Case action
            Case "saveEmbeddedFLM"
                saveEmbeddedFLM()
            Case "buildWhereDevNotes"
                buildWhereDevNotes()
        End Select

    End Sub

    Private Sub buildWhereDevNotes()
        ' build custom session variable for flm called WhereSessionName
        ' strWhere clubId = g_ClubId  (g_ClubId saved when user logs in)--example
        Dim strSessionName As String = IIf(IsNothing(Request.QueryString("sn")), Request.Form("sn"), Request.QueryString("sn"))
        Dim strWhere As String = "notes like '%users%'"
        Session(strSessionName) = strWhere
    End Sub

    Private Sub saveEmbeddedFLM()
        Dim strDate As String = IIf(IsNothing(Request.QueryString("date")), Request.Form("date"), Request.QueryString("date"))
        Dim strNotes As String = IIf(IsNothing(Request.QueryString("notes")), Request.Form("notes"), Request.QueryString("notes"))
        Dim strSesLst As String = IIf(IsNothing(Request.QueryString("sesLst")), Request.Form("sesLst"), Request.QueryString("sesLst"))

        Dim nvcReleaseNotes As New NameValueCollection
        nvcReleaseNotes("Release_Date") = strDate
        nvcReleaseNotes("Notes") = strNotes
        Dim blnInsertSuccess As Boolean = True
        g_IO_SQLInsert("ReleaseNotes", nvcReleaseNotes, "ajax", blnInsertSuccess)
        If blnInsertSuccess Then
            ' 4/19/16 T3 need to update FLM sesLst with new recid
            If IsNothing(Session(strSesLst)) Then
            Else
                If Session(strSesLst) = "" Then
                    Session(strSesLst) &= g_IO_GetLastRecId()
                Else
                    Session(strSesLst) &= "," & g_IO_GetLastRecId()
                End If
            End If
        End If
    End Sub

End Class