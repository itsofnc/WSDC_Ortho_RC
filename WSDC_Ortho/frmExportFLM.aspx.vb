Public Class frmExportFLM
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strContentType As String = "application/vnd.ms-excel"
        Dim strFileName As String = "Export"
        Dim strFileExtn As String = ".xls"

        If IsNothing(Request.QueryString("expType")) Then
        Else
            Dim strExpType = Request.QueryString("expType")
            Select Case strExpType
                Case "EXCEL"
                    ' defaulted above
                Case "WORD"
                    strContentType = "application/msword"
                    strFileExtn = ".doc"
                Case "CSV"
                    ' would need to clean up html tags, etc.
                    strContentType = "application/CSV"
                    strFileExtn = ".csv"
                Case "TXT"
                    ' would need to clean up html tags, etc.
                    strContentType = "application/txt"
                    strFileExtn = ".txt"
                Case "PDF"
                    ' would need to clean up html tags, etc. and convert to PDF somehow
                    strContentType = "application/pdf"
                    strFileExtn = ".pdf"
            End Select

        End If
        If IsNothing(Request.QueryString("fileName")) Then
        Else
            strFileName = Request.QueryString("fileName")
        End If

        ' retrieve FLM table html from session variable we created in FLM (at end of sub LoadListTableToGrid)
        If strFileExtn = ".csv" Or strFileExtn = ".txt" Then
            'divExportFLM.InnerHtml = stripTags(Session("FLMGridHTML"))
            downloadFile(stripTags(Session("FLMGridHTML")), strFileName & strFileExtn)
        Else
            divExportFLM.InnerHtml = Session("FLMGridHTML")
            ' set header tags to let the page know it is exporting...
            Response.ContentType = strContentType
            Response.AddHeader("Content-Disposition", "attachment;filename=" & strFileName & strFileExtn)
        End If

    End Sub

    Public Function stripTags(ByVal htmlToParse As String) As String
        htmlToParse = htmlToParse.Replace("<th", ",""<th").Replace("</th", """</th")
        htmlToParse = htmlToParse.Replace("<td", ",""<td").Replace("</td", """</td")
        htmlToParse = htmlToParse.Replace(vbCrLf, " ")
        htmlToParse = htmlToParse.Replace("<tr", "&&&&<tr")
        htmlToParse = Text.RegularExpressions.Regex.Replace(htmlToParse, "<[^>]*>", "").Trim(",")
        htmlToParse = htmlToParse.Replace("&&&&,", vbCrLf)
        htmlToParse = htmlToParse.Trim(vbCr).Trim(vbLf)
        Return htmlToParse
    End Function

    Private Sub downloadFile(ByVal htmlString As String, ByVal strFileName As String)
        ' Download a pdf
        Response.ContentType = "application/asp-unknown"
        Response.AddHeader("content-disposition", "attachment; filename=" & strFileName)
        Dim htmlBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(htmlString)
        Response.BinaryWrite(htmlBytes)
        Response.End()
    End Sub

End Class