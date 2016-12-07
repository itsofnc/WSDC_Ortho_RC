Public Class frmEmbeddedFLM
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        If IsPostBack Then
        Else
            If IsNothing(Request.QueryString("sesPrefix")) Then
                txtFLMSesPrefix.Text = "REN1" & Format(Date.Now, "ssffff") ' time in milliseconds
            Else
                txtFLMSesPrefix.Text = Request.QueryString("sesPrefix")
            End If
        End If
        ' 4/19 T3 testing calling FLM embedded by using session variable with recids to retrieve
        ' remove after we know this works
        ' send in recid list in session variable
        If IsNothing(Session(txtFLMSesPrefix.Text & "embflmrid")) Then
            Session(txtFLMSesPrefix.Text & "embflmrid") = "1,2,3,4,5"
        End If
        txtFLMGetText.Text = "frmListManager.aspx?id=ReleaseNotes&srt=Notes&pre=1&sesPrefix=" & txtFLMSesPrefix.Text & "&seslst=" & txtFLMSesPrefix.Text & "embflmrid&eFLM=1"
    End Sub

End Class