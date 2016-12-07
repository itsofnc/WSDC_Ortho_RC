Public Class frmClaimsReprint
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' send user back to login if session expired or they haven't successfully logged in
        If IsNothing(Session("user_link_id")) Then
            Response.Redirect("Default.aspx")
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        Dim rptReport As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim strPOFileBase As String = Format(Date.Now, "yyyyMMdd") & "__TYPE__" & System.Web.HttpContext.Current.Session("user_link_userid") & ".PDF"
        Dim strPOFilePath As String = HttpContext.Current.Server.MapPath("downloads\")
        Dim strMinDocumentNumber As String = ""
        Dim strMaxDocumentNumber As String = ""

        If cbCombinedPrint.Checked Then
            rptReport = New rptClaimMultiLineItems
        Else
            rptReport = New rptClaim
        End If

        '   code to reprint a claim
        Dim strSQL As String = "Select * from claims where claimnumber in ('" & txtClaimNumbers.Text.Replace(",", "','").Replace(" ", "") & "') order by insurance_name, ClaimNumber"
        Dim tblClaims As DataTable = g_IO_Execute_SQL(strSQL, False)


        If tblClaims.Rows.Count > 0 Then
            '7/10/15 CS reverse insurance info for secondary claim printing
            For index = tblClaims.Rows.Count - 1 To 0 Step -1

                Dim rowClaims As DataRow = tblClaims.Rows(index)
                If rowClaims("type") = "1" Then

                    Dim rowOtherCoverage As DataRow = tblClaims.NewRow  ' make a copy of the current record

                    For Each colColumn As DataColumn In tblClaims.Columns
                        ' move the info from the live to the copy
                        rowOtherCoverage(colColumn.ColumnName) = rowClaims(colColumn.ColumnName)
                    Next

                    ' move primary info to secondary and visa-versa
                    rowClaims("insurance_name") = rowOtherCoverage("other_policyholder_company")
                    rowClaims("other_policyholder_company") = rowOtherCoverage("insurance_name")

                    rowClaims("insurance_address1") = rowOtherCoverage("other_policyholder_address1")
                    rowClaims("other_policyholder_address1") = rowOtherCoverage("insurance_address1")

                    rowClaims("insurance_address2") = rowOtherCoverage("other_policyholder_address2")
                    rowClaims("other_policyholder_address2") = rowOtherCoverage("insurance_address2")

                    rowClaims("insurance_city") = rowOtherCoverage("other_policyholder_city")
                    rowClaims("other_policyholder_city") = rowOtherCoverage("insurance_city")

                    rowClaims("insurance_state") = rowOtherCoverage("other_policyholder_state")
                    rowClaims("other_policyholder_state") = rowOtherCoverage("insurance_state")

                    rowClaims("insurance_zip") = rowOtherCoverage("other_policyholder_zip")
                    rowClaims("other_policyholder_zip") = rowOtherCoverage("insurance_zip")

                    rowClaims("other_policyholder_Name") = Trim(rowOtherCoverage("Policyholder_name_first")) & " " & Trim(rowOtherCoverage("Policyholder_name_mid")) & " " & Trim(rowOtherCoverage("Policyholder_name_last"))

                    Try
                        Dim strName As String = rowOtherCoverage("other_policyholder_name")
                        rowClaims("Policyholder_name_first") = Split(strName, " ")(0)
                        strName = Trim(Mid(strName, strName.IndexOf(" ") + 2))
                        rowClaims("Policyholder_name_mid") = Split(strName, " ")(0)
                        If strName.IndexOf(" ") = -1 Then
                            rowClaims("Policyholder_name_last") = rowClaims("Policyholder_name_mid")
                            rowClaims("Policyholder_name_mid") = ""
                        Else
                            rowClaims("Policyholder_name_last") = Split(strName, " ")(1)

                        End If
                    Catch ex As Exception
                        rowClaims("Policyholder_name_last") = rowOtherCoverage("other_policyholder_name")
                        rowClaims("Policyholder_name_mid") = ""
                        rowClaims("Policyholder_name_first") = ""
                    End Try

                    rowClaims("policyholder_dob") = rowOtherCoverage("other_policyholder_dob")
                    rowClaims("other_policyholder_dob") = rowOtherCoverage("policyholder_dob")

                    rowClaims("Policyholder_gender") = rowOtherCoverage("other_policyholder_gender")
                    rowClaims("other_policyholder_gender") = rowOtherCoverage("policyholder_gender")

                    rowClaims("Policyholder_gender") = rowOtherCoverage("other_policyholder_gender")
                    rowClaims("other_policyholder_gender") = rowOtherCoverage("policyholder_gender")

                    rowClaims("Policyholder_subscriberID") = rowOtherCoverage("other_policyholder_subscriberID")
                    rowClaims("other_policyholder_subscriberID") = rowOtherCoverage("policyholder_subscriberID")

                    rowClaims("Policyholder_plan") = rowOtherCoverage("other_policyholder_plan")
                    rowClaims("other_policyholder_plan") = rowOtherCoverage("policyholder_plan")

                    rowClaims("Policyholder_employer") = rowOtherCoverage("other_policyholder_employer")
                    rowClaims("other_policyholder_employer") = rowOtherCoverage("policyholder_employer")
                End If
            Next

            rptReport.SetDataSource(tblClaims)
            ' end of mod 7/10/15 CS 

            If tblClaims.Rows.Count > 1 Then
                strMinDocumentNumber = tblClaims.Rows(0)("ClaimNumber")
                strMaxDocumentNumber = tblClaims.Rows(tblClaims.Rows.Count - 1)("ClaimNumber")
                strPOFileBase = strPOFileBase.Replace("__TYPE__", "_ClaimsReprint_" & strMinDocumentNumber & "-" & strMaxDocumentNumber)
            Else
                strMinDocumentNumber = tblClaims.Rows(0)("ClaimNumber")
                strPOFileBase = strPOFileBase.Replace("__TYPE__", "_ClaimReprint_" & strMinDocumentNumber)
            End If

            Dim strReturnName As String = strPOFileBase
            Try
                rptReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPOFilePath & strPOFileBase)
            Catch ex As Exception
                Dim r = 1
            End Try

            litFrameCall.Text = "DownloadFile.aspx?pdf=" & strPOFileBase

            Exit Sub
        End If


    End Sub
End Class