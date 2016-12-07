
Public Class WebForm2
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If IsNothing(Request.QueryString("pdf")) OrElse Request.QueryString("pdf") = "" Then
        Else
            ' Download a pdf
            Dim strFileName As String = Request.QueryString("pdf")
            Response.ContentType = "application/asp-unknown"
            Response.AddHeader("content-disposition", "attachment; filename=" & strFileName)
            Dim FStream = CreateObject("ADODB.Stream")
            FStream.Open()
            FStream.Type = 1
            FStream.LoadFromFile(Server.MapPath("downloads\") & strFileName)
            Response.BinaryWrite(FStream.Read())
            FStream.Close()
            FStream = Nothing
            If IsNothing(Request.QueryString("del")) Then
            Else
                System.IO.File.Delete(Server.MapPath("downloads\") & strFileName)
            End If
            Response.End()

        End If

    End Sub

End Class