Module ModDataImport
    Public Sub g_ImportContracts()
        ' pull data from DataMiner tables
        Dim strSQL As String = "SELECT ortho_contracts.*, patients.patient_id, patients.chart_number " & _
                               "FROM ortho_contracts left outer join patients on patients.unique_id = ortho_contracts.unique_id;"
        Dim tblOrthoContracts As DataTable = g_IO_Execute_SQL("select * from ortho_contracts", False, "MYSQL") ' need to create view in MySQL to pull child data references too
        'Dim tblPatientTransactions As DataTable = g_IO_Execute_SQL("select * from patient_transactions", False, "MYSQL")
        'Dim tblProviders As DataTable = g_IO_Execute_SQL("select * from providers", False, "MYSQL")

        ' insert into WSDC_Ortho tables
        strSQL = ""
        For Each row In tblOrthoContracts.Rows
            strSQL = "INSERT INTO Contracts " & _
               "(DatePosted" & _
               ", Sys_Users_RECID" & _
               ", PatientNumber" & _
               ", ChartNumber" & _
               ", ContractDate" & _
               ", Amount" & _
               ", Doctors_recid" & _
               ", TransactionCodes_recid" & _
               ", Comments" & _
               ", PatientInitialBalance" & _
               ", PatientInitialPayment" & _
               ", PatientContractMonths" & _
               ", PatientRemainingMonths" & _
               ", PatientMonthlyPayment] " & _
               ", PatientFirstPay]       " & _
               ", PrimaryCoverageAmt]    " & _
               ", PrimaryInitialPayment] " & _
               ", PrimaryInstallmentAmt] " & _
               ", PrimaryFirstPay" & _
               ", PrimaryBillingFrequency_recid" & _
               ", SecondaryCoverageAmt" & _
               ", SecondaryInitialPayment" & _
               ", SecondaryInstallmentAmt" & _
               ", SecondaryFirstPay" & _
               ", SecondaryBillingFrequency_recid" & _
               ", UCFInitialPayment" & _
               ", UCFMonthlyPayment" & _
               ", PatientBillingFrequency_recid)" & _
            " VALUES(" & _
               "'" & Today() & "', " & _
               "1, " & _
               "'" & row("patient_id") & "', " & _
               "'" & row("chart_number") & "', " & _
               "'" & row("date_contract") & "', " & _
               "'" & row("amount_tot") & "', " & _
               "'" & row("provider_id") & "', " & _
               "'" & row("trans_code") & "', " & _
               "'" & row("comment_1") & vbCrLf & row("comment_2") & "', " & _
               "'" & row("amount_pat") & "', " & _
               "'" & row("amount_down_pat") & "', " & _
               "'" & row("months_tot") & "', " & _
               "'" & row("months_remaining") & "', " & _
               "'" & row("amount_monthly_pat") & "', " & _
               "'" & row("month_first_pay") & "', " & _
               "'" & row("amount_pri") & "', " & _
               "'" & row("amount_down_pri") & "', " & _
               "'" & row("amount_monthly_pri") & "', " & _
               "'" & row("month_first_pay") & "', " & _
               "1, " & _
               "'" & row("amount_sec") & "', " & _
               "'" & row("amount_down_sec") & "', " & _
               "'" & row("amount_monthly_sec") & "', " & _
               "'" & row("month_first_pay") & "', " & _
               "1, " & _
               "0, " & _
               "0, " & _
               "1')"
            '

            g_IO_Execute_SQL(strSQL, False, "MSSQL")

        Next

    End Sub


End Module
