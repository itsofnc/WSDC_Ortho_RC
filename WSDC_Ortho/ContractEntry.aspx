<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="ContractEntry.aspx.vb" Inherits="WSDC_Ortho.ContractEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmContractEntry' class="form-horizontal" role="form" action="ContractEntry.aspx" runat="server" defaultbutton="btnHidden">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" class="hidden" runat="server" OnClientClick="return false;" />

        <asp:HiddenField ID="txtSessions" runat="server" />
        <asp:TextBox ID="txtRecID" CssClass="DB form-control hidden" runat="server"></asp:TextBox>

        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        <div class="container">
            <div class="col-sm-12 text-center">
                <br />
            </div>
            <asp:Literal ID="litContractID" runat="server"></asp:Literal>
            <h4 style="color: #2d6ca2;" class=" text-center">
                <asp:Label ID="lblPatientName" runat="server"></asp:Label></h4>
            <asp:Literal ID="litShowPatientData" runat="server"></asp:Literal>
            <div id="divEditOptions" class="col-sm-12 hidden">
                <div class=" col-sm-offset-1 col-sm-9">
                    <div class="pull-left">
                        <ul>
                            <li><a href="#">
                                <asp:Button ID="btnSave" OnClientClick="saveData(false);" class="btn btn-sm btn-primary" runat="server" Text="Save Changes" /></a></li>
                            <li><a href="#">
                                <asp:Button ID="btnCancel" class="btn btn-sm btn-primary" runat="server" Text="Cancel Edit"  /></a></li>
                            <li><a href="#">
                                <asp:Button ID="btnDelete" OnClientClick="return confirm('Are you sure you want to delete this contract?')" class="btn btn-sm btn-danger" runat="server" Text="Delete" /></a></li>
                        </ul>
                    </div>
                </div>
            </div>
            <div id="divViewOptions" class="col-sm-12" style="z-index:1000;">
                <div class=" col-sm-offset-1 col-sm-9">
                    <div class="pull-left">
                        <span id="editLink"><span class="glyphicon glyphicon-eye-open"></span>View Only Mode <a href="#" onclick="showEditOptions()">Edit Contract (Click Here)</a></span>
                    </div>
                    <br />
                    <br />
                    <div id="divContractOptions" class="pull-left" >
                        <ul>
                            <li><a href="#">
                                <asp:Button ID="btnPaymentPosting" class="btn btn-sm btn-success" runat="server" Text="Post Payment"  /></a></li>
                            <li><span id="generateClaimLink"><a href="#" >
                                <asp:Button ID="btnGenerateClaim" class="btn btn-sm btn-success" runat="server" Text="Generate Invoice / Claim"  /></a></span></li>
                            <li><a href="#" >
                                <asp:Button ID="btnContractStatus" class="btn btn-sm btn-primary" runat="server" Text="Contract Status"  /></a></li>
                            <li><a href="#" >
                                <asp:Button ID="btnPaymentHistory" class="btn btn-sm btn-primary" runat="server" Text="Payment History"  /></a></li>
                            <li><a href="#" >
                                <asp:Button ID="btnViewNotes" class="btn btn-sm btn-primary" runat="server" Text="View Notes" OnClientClick="showViewNotes(); return false;" /></a></li>
                            <li><a href="#" >
                                <asp:Button ID="btnNotes" class="btn btn-sm btn-primary" runat="server" Text="Add Notes" OnClientClick="showAddNotes(); return false;" /></a></li>                            
                        </ul>
                    </div>
                </div>
            </div>

            <div id="divContractSearch" class="col-sm-12">
                <div class="col-sm-12 text-center">
                    <hr />
                </div>
                 <h4 class="col-md-12 text-center">
                    Enter criteria below to Begin a New Contract, or Search for an Existing Contract:
                </h4>
                <div class="form-group" style="color: #2d6ca2;">
                    <div class="col-lg-12" style="text-align: center;">
                        <div class="form-inline">
                            <div class="form-group">
                                <label class="sr-only" for="txtChartNumber">Chart #</label>
                                <div class="input-group">
                                    <div class="input-group-addon txtChartSearch" style="background-color:#d1d1d1">Chart #</div>
                                        <asp:TextBox ID="txtChartNumber" name="txtChartNumber" CssClass="DB form-control" MaxLength="20" Style="max-width: 130px;" runat="server"
                                            onChange="getPatName('cht');"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="sr-only" for="txtAccountNumber">Account #</label>
                                <div class="input-group col-sm-offset-2">
                                    <div class="input-group-addon" style="background-color:#d1d1d1">Account #</div>
                                        <asp:TextBox ID="txtAccountNumber" name="txtAccountNumber" value="" CssClass="DB form-control" MaxLength="10" Style="max-width: 160px;" runat="server"
                                            onChange="getPatName('act'); jQuery('.txtChartSearch:first').focus();"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 text-center">
                    <hr />
                </div>
            </div>
            

            <div id="divEndOfContract" class="col-sm-12 hidden">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">
                        <label for="dteEndContractDate" class="col-sm-3">End Date:
                            <span class="dateContainer" >
                                <span class="input-group input-append date" id="dtpEndContractDate">
                                    <asp:TextBox ID="dteEndContractDate" name="dteEndContractDate" CssClass="DB form-control" placeholder="MM/DD/YYYY" runat="server"></asp:TextBox>
                                    <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                </span>
                            </span>
                        </label>                    
                        <label for="txtEndReason" class="col-sm-6">End Reason:                           
                            <asp:TextBox ID="txtEndReason" CssClass="DB form-control" Style="width: 400px;" runat="server" MaxLength="8000" TextMode="MultiLine" Rows="3" onFocus="jQuery('.datePicker').datepicker('hide');"></asp:TextBox>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">
                        <label for="dteContractDate" class="col-sm-3" >
                            <span class="requiredFieldIndicator">*</span> Banding Date:
                            <span class="dateContainer" >
                                <span class="input-group input-append date" id="dtpContractDate">                                
                                    <asp:TextBox ID="dteContractDate" name="dteContractDate" CssClass="DB form-control" onFocus="verifyContractSelected();"  placeholder="mm/dd/yyyy" runat="server"></asp:TextBox>
                                    <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                </span>
                            </span>                            
                        </label>
                        <label for="dolAmount" class="col-sm-3">
                            <span class="requiredFieldIndicator">*</span> TCF Amount:
                            <span style="padding-left: 0px;">
                                <span class="input-group ">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="dolAmount" name="dolAmount" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                        runat="server"></asp:TextBox>
                                </span>
                            </span>
                        </label>
                        <label for="ddlDoctors_vw" class="col-sm-3"> Doctor:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlDoctors_vw" CssClass="DB form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                            </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">                                  
                        <label for="ddlTransactionCodes_vw" class="col-sm-6">
                            Transaction Code:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlTransactionCodes_vw" CssClass="DB form-control PromptSelection" Style="width: 375px;" runat="server"></asp:DropDownList>
                            </span>
                        </label>
                        <label for="ddlStatus_vw" class="col-sm-3">
                            Status:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlStatus_vw" CssClass="DB form-control PromptSelection" onChange="showEndOfContract()" runat="server"></asp:DropDownList>
                            </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class=" col-sm-offset-1 col-sm-9">
                        <label for="txtComments" class="col-sm-4">
                            Comments:&nbsp;<span class="glyphicon glyphicon-eye-open" onclick="showViewNotes();"></span>
                            <asp:TextBox ID="txtComments" CssClass="DB form-control" Style="width: 490px;" runat="server" MaxLength="8000" TextMode="MultiLine" Rows="5"></asp:TextBox>
                            
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12 hidden">
                <div class="form-group col-sm-4">
                    <label for="txtAccount_ID" class="col-sm-4">Contract #:</label>
                    <div class="col-sm-8 input-group" style="padding-left: 15px;">
                        <asp:TextBox ID="txtAccount_ID" CssClass="DB form-control" runat="server" Enabled="False"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group col-sm-4">
                    <label for="txtPreviousContract" class="col-sm-4">Previous Contract #:</label>
                    <div class="col-sm-8" style="padding-left: 0px;">
                        <asp:TextBox ID="txtPreviousContract" value="" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                    </div>
                </div>
            </div>
                
            <div class="col-sm-12" style="z-index: 1000;">
                <div class=" col-sm-offset-1 col-sm-9">
                    <ul id="myTab" class="nav nav-tabs">
                        <li class="active"><a href="#patient" data-toggle="tab">Patient</a></li>
                        <li><a href="#primary" data-toggle="tab">Primary Insurance</a></li>
                        <li><a href="#secondary" data-toggle="tab">Secondary Insurance</a></li>
                    </ul>
                </div>
            </div>
            <div class="col-sm-12" >
                <div id="myTabContent" class="tab-content">
                    <div class="tab-pane active fade in" id="patient">
                        <div class="col-sm-12">&nbsp;</div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                 
                                <label for="dolPatientInitialBalance" class="col-sm-3">
                                    Initial Balance:
                                <span style="padding-left: 0px;">
                                    <span class="input-group ">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPatientInitialBalance" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                            runat="server"></asp:TextBox>
                                    </span>
                                </span>
                                </label>
                                <label for="dtePatientFirstPay" class="col-sm-3">Initial Payment Due:
                                    <span class="dateContainer">
                                        <span class="input-group input-append date" id="dtpPatientFirstPay">
                                            <asp:TextBox ID="dtePatientFirstPay" name="dtePatientFirstPay" CssClass="DB form-control" placeholder="MM/DD/YYYY" runat="server" ></asp:TextBox>
                                            <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </span>
                                    </span>
                                </label>    
                                <label for="dolPatientInitialPayment" class="col-sm-3">
                                    Initial Payment Expected:
                                <span style="padding-left: 0px;">
                                    <span class="input-group ">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPatientInitialPayment" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                            runat="server"></asp:TextBox>
                                    </span>
                                </span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="dolPatientDiscount" class="col-sm-3">
                                    Discount/Adjustment:
                                    <span style="padding-left: 0px">
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPatientDiscount" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                            runat="server"></asp:TextBox>
                                    </span>
                                </span>
                                </label>
                                <label for="dolPatientMonthlyPayment" class="col-sm-3">
                                    Payment Amount (Recurring):
                                <span style="padding-left: 0px">
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPatientMonthlyPayment" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                            runat="server"></asp:TextBox>
                                    </span>
                                </span>
                                </label>
                                <label for="ddlPatientBillingFrequency_vw" class="col-sm-3">Billing Frequency:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlPatientBillingFrequency_vw" CssClass="DB form-control" runat="server"></asp:DropDownList>
                                    </span>
                                </label>                       
                                
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                
                                <label for="dolPatientAmountBilled" class="col-sm-3">Amount Billed/Expected:
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPatientAmountBilled" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="txtPatientContractMonths" class="col-sm-3">
                                    <span class="requiredFieldIndicator">*</span> Contract Months:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtPatientContractMonths" CssClass="DB form-control" MaxLength="2" Style="max-width: 75px;" 
                                        runat="server"></asp:TextBox>
                                </span>
                                </label>
                                <label for="txtPatientRemainingMonths" class="col-sm-3">
                                    <span class="requiredFieldIndicator">*</span> Remaining Months:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtPatientRemainingMonths" CssClass="DB form-control" MaxLength="2" Style="max-width: 75px;" 
                                        runat="server"></asp:TextBox>
                                </span>
                                </label>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="dolPatientRemainingBalance" class="col-sm-3">Remaining Balance (To Bill):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPatientRemainingBalance" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="chkPrintMonthlyInvoice" class="col-sm-4 hidden">
                                    <asp:CheckBox ID="chkPrintMonthlyInvoice" CssClass="DB checkbox" runat="server" Text="Print Monthly Invoice" />
                                </label>
                            </div>
                        </div>
                        
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <h4>Patient Information:</h4><a name="patinfo"></a>
                                <hr />
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label for="txtname_first" class="col-sm-4">
                                        First Name:
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtname_first" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>
                                    <label for="txtname_mid" class="col-sm-4">
                                        Middle Name:
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtname_mid" value="" CssClass="form-control" MaxLength="25" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>
                                    <label for="txtname_last" class="col-sm-4">
                                        Last Name:
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtname_last" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label for="txtaddr_other" class="col-sm-4">
                                        Address 1:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtaddr_other" value="" CssClass="form-control" MaxLength="50" Style="max-width: 230px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                    <label for="txtaddr_street" class="col-sm-8">
                                        Address 2:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtaddr_street" value="" CssClass="form-control" MaxLength="50" Style="max-width: 315px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label for="txtaddr_city" class="col-sm-4">
                                        City:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtaddr_city" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                    <label for="txtaddr_state" class="col-sm-4">
                                        State:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtaddr_state" value="" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                    <label for="txtaddr_zip" class="col-sm-4">
                                        Zip:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtaddr_zip" value="" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label for="ddlGender" class="col-sm-4">
                                        Gender:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlGender" CssClass="form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                                    </span>
                                    </label>
                                    <label for="dteDOB" class="col-sm-4">DOB:
                                        <span class="dateContainer" style="padding-left: 0px;">
                                            <span class="input-group input-append date" id="dtpDOB">
                                                <asp:TextBox ID="dteDOB" name="dteDOB" CssClass="form-control" placeholder="MM/DD/YYYY" runat="server" ></asp:TextBox>
                                                <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                            </span>
                                        </span>
                                    </label>                                   
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="primary">
                        <div class="col-sm-12">&nbsp;</div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="ddlPrimaryInsurancePlans_vw" class="col-sm-4">
                                    Primary Insurance Plan:
                                <span style="padding-left: 0px;">
                                    <asp:DropDownList ID="ddlPrimaryInsurancePlans_vw" CssClass="DB form-control PromptSelection" runat="server" onchange="checkPatientInsurance(this, '1');"></asp:DropDownList>
                                </span>
                                </label>
                                <label class="col-sm-8">
                                    <span id="spnPrimaryInsuranceModalDisplay">
                                        <span class="glyphicon glyphicon-eye-open"></span><a href="#primarypolicy">View/Enter Insurance Information</a>
                                    </span>
                                </label>
                            </div>

                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="dolPrimaryCoverageAmt" class="col-sm-3">
                                    Coverage Amount:
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPrimaryCoverageAmt" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="dolPrimaryInitialPayment" class="col-sm-3">
                                    Initial Payment (Expected):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPrimaryInitialPayment" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="dolPrimaryInstallmentAmt" class="col-sm-4">
                                    Installment Amount (Expected):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPrimaryInstallmentAmt" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="ddlPrimaryBillingFrequency_vw" class="col-sm-3">
                                    Billing Frequency:
                                <span style="padding-left: 0px;">
                                    <asp:DropDownList ID="ddlPrimaryBillingFrequency_vw" CssClass="DB form-control" runat="server"></asp:DropDownList>
                                </span>
                                </label>
                                <label for="dolPrimaryClaim_Amount_Initial" class="col-sm-3">
                                    Claim Initial (To Print):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPrimaryClaim_Amount_Initial" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="dolPrimaryClaim_Amount_Installment" class="col-sm-4">
                                    Claim Installment (To Print):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPrimaryClaim_Amount_Installment" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                
                                <label for="dolPrimaryRemainingBalance" class="col-sm-3">
                                    Remaining Balance (To Bill):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPrimaryRemainingBalance" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="dolPrimaryAmountBilled" class="col-sm-3">
                                    Amount Billed/Expected:
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolPrimaryAmountBilled" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9 hidden">
                            <div class="form-group">
                                <label for="chkPrintMonthlyClaimPrimary" class="col-sm-3 ">                                
                                    <asp:CheckBox ID="chkPrintMonthlyClaimPrimary" CssClass="DB checkbox" runat="server" Text="Print Monthly Claim" />
                                </label>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <h4>Primary Insurance Information: <asp:Label ID="lblPrimaryInsuranceProvider" runat="server" Text=""></asp:Label></h4>
                                <a href="#" onclick="cancelPolicy('1')"> Cancel Policy</a>
                                <hr />
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label>Insured/Policyholder:</label><a name="primarypolicy"></a></div>
                                <div class="col-sm-12">
                                    <label for="txtInsured_name_first" class="col-sm-4">
                                        First Name:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtInsured_name_first" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                    <label for="txtInsured_name_mid" class="col-sm-4">
                                        Middle Name:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtInsured_name_mid" value="" CssClass="form-control" MaxLength="25" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                    <label for="txtInsured_name_last" class="col-sm-4">
                                        Last Name:
                                <span style="padding-left: 0px;">
                                    <asp:TextBox ID="txtInsured_name_last" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                </span>
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="txtInsured_name_suffx" class="col-sm-4">
                                    Suffix:
                        <span style="padding-left: 0px;">
                            <asp:TextBox ID="txtInsured_name_suffx" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                                </label>
                                <label for="txtInsured_addr_street" class="col-sm-8 ">
                                    Street/PO Box:
                        <span style="padding-left: 0px;">
                            <asp:TextBox ID="txtInsured_addr_street" value="" CssClass="form-control" MaxLength="50" Style="max-width: 300px;" runat="server"></asp:TextBox>
                        </span>
                                </label>
                            </div>
                        </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="txtInsured_addr_city" class="col-sm-4">
                                    City:
                        <span style="padding-left: 0px;">
                            <asp:TextBox ID="txtInsured_addr_city" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                                </label>
                                <label for="txtInsured_addr_state" class="col-sm-4">
                                    State:
                        <span style="padding-left: 0px;">
                            <asp:TextBox ID="txtInsured_addr_state" value="" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                                </label>
                                <label for="txtInsured_addr_zip" class="col-sm-4">
                                    Zip:
                        <span style="padding-left: 0px;">
                            <asp:TextBox ID="txtInsured_addr_zip" value="" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                                </label>
                            </div>
                        </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="ddlInsured_Gender" class="col-sm-4">
                                    Gender:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlInsured_Gender" CssClass="form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                                    </span>
                                </label>
                                <label for="dteInsured_Date_Birth" class="col-sm-4">
                                    DOB:
                                    <span class="dateContainer">
                                        <span class="input-group input-append date " id="dtpInsured_Date_Birth">
                                            <asp:TextBox ID="dteInsured_Date_Birth" CssClass="form-control" placeholder="MM/DD/YYYY" runat="server"></asp:TextBox>
                                            <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </span>                                
                                    </span>
                                </label>
                            </div>
                        </div>
                            </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="ddlRelationship" class="col-sm-4">
                                    Patient Relation:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlRelationship" CssClass="form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                                    </span>
                                </label>
                                <label for="txtEmployer_Name" class="col-sm-4">
                                    Employer:
                                    <span style="padding-left: 0px;">
                                        <asp:TextBox ID="txtEmployer_Name" value="" CssClass="form-control" MaxLength="50" Style="max-width: 300px;" runat="server"></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                        </div>
                            </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="dteDate_Effective" class="col-sm-4">
                                    Effective Date:
                                    <span class="dateContainer">
                                        <span class="input-group input-append date" id="dtpDate_Effective">
                                            <asp:TextBox ID="dteDate_Effective" CssClass="form-control" placeholder="MM/DD/YYYY" runat="server" ></asp:TextBox>
                                            <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </span>
                                    </span>
                                </label>
                                <label for="dteDate_Canceled" class="col-sm-4">
                                    Canceled Date:
                                    <span class="dateContainer">
                                        <span class="input-group input-append date" id="dtpDate_Canceled">
                                            <asp:TextBox ID="dteDate_Canceled" CssClass="form-control" placeholder="MM/DD/YYYY" runat="server"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                            </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label for="txtGroup_Number" class="col-sm-4">
                                        Group #
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtGroup_Number" value="" CssClass="form-control" MaxLength="24" Style="max-width: 130px;" runat="server" onFocus="jQuery('.datePicker').datepicker('hide');"></asp:TextBox>
                                        </span>
                                    </label>
                                    <label for="txtPolicy_Number" class="col-sm-4">
                                        Policy #
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtPolicy_Number" value="" CssClass="form-control" MaxLength="24" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>
                                    <label for="dolLifetimeMax" class="col-sm-4">
                                        Lifetime Max:
                                        <span style="padding-left: 0px">
                                            <span class="input-group">
                                                <span class="input-group-addon">$</span>
                                                <asp:TextBox ID="dolLifetimeMax" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" 
                                                    runat="server"></asp:TextBox>
                                            </span>
                                        </span>
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="secondary">
                        <div class="col-sm-12">&nbsp;</div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="ddlSecondaryInsurancePlans_vw" class="col-sm-4">
                                    Secondary Insurance Plan:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlSecondaryInsurancePlans_vw" CssClass="DB form-control PromptSelection" runat="server" onchange="checkPatientInsurance(this, '2');"></asp:DropDownList>
                                    </span>
                                </label>
                                <label class="col-sm-8">
                                    <span id="Span18"><span class="glyphicon glyphicon-eye-open"></span><a href="#secondpolicy">View/Enter Insurance Information</a></span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="dolSecondaryCoverageAmt" class="col-sm-3">
                                    Coverage Amount:
                            <span style="padding-left: 0px">
                                <span class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="dolSecondaryCoverageAmt" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                        runat="server"></asp:TextBox>
                                </span>
                            </span>
                                </label>
                                <label for="dolSecondaryInitialPayment" class="col-sm-3">
                                    Initial Payment (Expected):
                            <span style="padding-left: 0px">
                                <span class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="dolSecondaryInitialPayment" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                        runat="server"></asp:TextBox>
                                </span>
                            </span>
                                </label>
                                <label for="dolSecondaryInstallmentAmt" class="col-sm-4">
                                    Installment Amount (Expected):
                            <span style="padding-left: 0px">
                                <span class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="dolSecondaryInstallmentAmt" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                        runat="server"></asp:TextBox>
                                </span>
                            </span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <label for="ddlSecondaryBillingFrequency_vw" class="col-sm-3">
                                    Billing Frequency:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlSecondaryBillingFrequency_vw" CssClass="DB form-control" runat="server"></asp:DropDownList>
                                    </span>
                                </label>
                                <label for="dolSecondaryClaim_Amount_Initial" class="col-sm-3">
                                    Claim Initial (To Print):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolSecondaryClaim_Amount_Initial" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="dolSecondaryClaim_Amount_Installment" class="col-sm-4">
                                    Claim Installment (To Print):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolSecondaryClaim_Amount_Installment" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                
                                <label for="dolSecondaryRemainingBalance" class="col-sm-3">
                                    Remaining Balance (To Bill):
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolSecondaryRemainingBalance" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                                <label for="dolSecondaryAmountBilled" class="col-sm-3">
                                    Amount Billed/Expected:
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolSecondaryAmountBilled" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                        <div class=" col-sm-offset-1 col-sm-9 hidden">
                            <div class="form-group">
                                <label for="chkPrintMonthlyClaimSecondary" class="col-sm-3">
                                    <asp:CheckBox ID="chkPrintMonthlyClaimSecondary" CssClass="DB checkbox" runat="server" Text="Print Monthly Claim" />
                                </label>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <h4>Secondary Insurance Information: <asp:Label ID="lblSecondaryInsuranceProvider" runat="server" Text=""></asp:Label></h4>
                                <a href="#" onclick="cancelPolicy('2')"> Cancel Policy</a>
                                <hr />
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label>Insured/Policyholder:</label><a name="secondpolicy"></a></div>
                                <div class="col-sm-12">
                                    <label for="txtInsured_name_first_sec" class="col-sm-4">
                                        First Name:
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtInsured_name_first_sec" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>

                                    <label for="txtInsured_name_mid_sec" class="col-sm-4">
                                        Middle Name:
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtInsured_name_mid_sec" value="" CssClass="form-control" MaxLength="25" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>

                                    <label for="txtInsured_name_last_sec" class="col-sm-4">
                                        Last Name:
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtInsured_name_last_sec" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>


                                </div>
                            </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="txtInsured_name_suffx_sec" class="col-sm-4">
                                    Suffix:
                                    <span style="padding-left: 0px;">
                                        <asp:TextBox ID="txtInsured_name_suffx_sec" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                    </span>
                                </label>
                                <label for="txtInsured_addr_street_sec" class="col-sm-8">
                                    Street/PO Box:
                                    <span style="padding-left: 0px;">
                                        <asp:TextBox ID="txtInsured_addr_street_sec" value="" CssClass="form-control" MaxLength="50" Style="max-width: 230px;" runat="server"></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                        </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="txtInsured_addr_city_sec" class="col-sm-4">
                                    City:
                                    <span style="padding-left: 0px;">
                                        <asp:TextBox ID="txtInsured_addr_city_sec" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                    </span>
                                </label>

                                <label for="txtInsured_addr_state_sec" class="col-sm-4">
                                    State:
                                    <span style="padding-left: 0px;">
                                        <asp:TextBox ID="txtInsured_addr_state_sec" value="" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                    </span>
                                </label>

                                <label for="txtInsured_addr_zip_sec" class="col-sm-4">
                                    Zip:
                                    <span style="padding-left: 0px;">
                                        <asp:TextBox ID="txtInsured_addr_zip_sec" value="" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                        </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="ddlInsured_Gender_sec" class="col-sm-4">
                                    Gender:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlInsured_Gender_sec" CssClass="form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                                    </span>
                                </label>

                                <label for="dteInsured_Date_Birth_sec" class="col-sm-4">
                                    DOB:
                                    <span class="dateContainer">
                                        <span class="input-group input-append date" id="dtpInsured_Date_Birth_sec">
                                            <asp:TextBox ID="dteInsured_Date_Birth_sec" CssClass="form-control" placeholder="MM/DD/YYYY" runat="server"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="ddlRelationship_sec" class="col-sm-4">
                                    Patient Relation:
                                    <span style="padding-left: 0px;">
                                        <asp:DropDownList ID="ddlRelationship_sec" CssClass="form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                                    </span>
                                </label>
                                <label for="txtEmployer_Name_sec" class="col-sm-4">
                                    Employer:
                                    <span style="padding-left: 0px;">
                                        <asp:TextBox ID="txtEmployer_Name_sec" value="" CssClass="form-control" MaxLength="50" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                        </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label for="dteDate_Effective_sec" class="col-sm-4">
                                    Effective Date:
                                    <span class="dateContainer">
                                        <span class="input-group input-append date" id="dtpDate_Effective_sec">
                                            <asp:TextBox ID="dteDate_Effective_sec" CssClass="form-control" placeholder="MM/DD/YYYY" runat="server"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </span>
                                    </span>
                                </label>

                                <label for="dteDate_Canceled_sec" class="col-sm-4">
                                    Canceled Date:
                                    <span class="dateContainer">
                                        <span class="input-group input-append date" id="dtpDate_Canceled_sec">
                                            <asp:TextBox ID="dteDate_Canceled_sec" CssClass="form-control" placeholder="MM/DD/YYYY" runat="server"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </span>
                                    </span>
                                </label>
                            </div>
                        </div>
                        </div>
                        <div class="col-sm-offset-1 col-sm-9">
                            <div class="form-group">
                                <div class="col-sm-12">
                                    <label for="txtGroup_Number_sec" class="col-sm-4">
                                        Group #
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtGroup_Number_sec" value="" CssClass="form-control" MaxLength="24" Style="max-width: 130px;" runat="server" onFocus="jQuery('.datePicker').datepicker('hide');"></asp:TextBox>
                                        </span>
                                    </label>
                                    <label for="txtPolicy_Number_sec" class="col-sm-4">
                                        Policy #
                                        <span style="padding-left: 0px;">
                                            <asp:TextBox ID="txtPolicy_Number_sec" value="" CssClass="form-control" MaxLength="24" Style="max-width: 130px;" runat="server"></asp:TextBox>
                                        </span>
                                    </label>
                                    <label for="dolLifetimeMax_sec" class="col-sm-4">
                                        Lifetime Max:
                                    <span style="padding-left: 0px">
                                        <span class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox ID="dolLifetimeMax_sec" CssClass="form-control" MaxLength="10" Style="max-width: 130px;" 
                                                runat="server"></asp:TextBox>
                                        </span>
                                    </span>
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-12 text-center">
                <br />
            </div>
            <div class="col-sm-12 text-center">
                <asp:Button ID="btnSubmit" class="btn btn-sm btn-primary" runat="server" Text="Submit New Contract" />
                <asp:Button ID="btnCancelNew" class="btn btn-default" runat="server" Text="Cancel" />
            </div>
            
            <div class="col-sm-12 text-center">
                <br />
            </div>
        </div>
        <!-- /.container -->

        
        <!-- /.modal -->
         <!--Modal Add Notes Popup-->
        <div class="modal fade" id="divModalAddNotes" tabindex="-1" role="dialog" aria-labelledby="divModalAddNotesLabel" aria-hidden="false">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Add Notes:</h4>
                    </div>
                    <div class="modal-body-addNotes">
                        <div class="col-md-12">
                            <div class="form-group col-md-12">
                                <div class="col-sm-8">
                                    <br />
                                    <asp:TextBox ID="txtAddNotes" CssClass="DB form-control" Style="width: 550px;" runat="server" TextMode="MultiLine" Rows="5" placeholder="Enter your notes here..."></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="btn-group">
                            <asp:Button ID="btnAddNote" OnClientClick="saveData(false);" class="btn btn-sm btn-success" runat="server" Text="Save Notes"/>
                        </div>
                        <div class="btn-group">
                            <button type="button" class="btn btn-default" id="btnCancelNote" data-dismiss="modal">Cancel</button>
                        </div>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
        <!--Modal Add Notes Popup-->
        <div class="modal fade" id="divModalViewNotes" tabindex="-1" role="dialog" aria-labelledby="divModalViewNotesLabel" aria-hidden="false">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Notes:</h4>
                    </div>
                    <div class="modal-body-viewNotes">
                        <div class="col-md-12">
                            <div class="form-group col-md-12">
                                <div class="col-sm-8">
                                    <br />
                                    <asp:TextBox ID="txtViewNotes" CssClass="DB form-control" ReadOnly="true" Style="width: 550px; background-color: white;" runat="server" TextMode="MultiLine" Rows="15"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default" id="btnCloseNote" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </form>

    <!--Modal Contract Add New User Popup-->
    <div class="modal fade" id="divModalAddNew" tabindex="-1" role="dialog" aria-labelledby="divModalContractSelectionLabel" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title"></h4>
                </div>
                <div class="modal-body-addNew">
                    
                </div>
                <div class="modal-footer">
                    <div class="btn-group">
                        <button type="button" class="btn btn-primary" id="btnGoToContract" onclick="goToExistingContract();" data-dismiss="modal">Go To Contract</button>
                    </div>
                    <div class="btn-group">
                        <button type="button" class="btn btn-success" id="btnOkay" onclick="keepdata('yes');" data-dismiss="modal">Add New Contract</button>
                    </div>
                    <div class="btn-group">
                        <button type="button" class="btn btn-default" id="btnPopupCancel" onclick="keepdata('no');" data-dismiss="modal">Select Another Patient</button>
                    </div>


                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

   
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <script type="text/javascript">
        // capture user leaving the content edit area
        var blnWarningPrompt = false;        
        window.onbeforeunload = function () {
            if (blnWarningPrompt == true) {
                return 'Leaving This Page, You Will Lose Any UnSaved Data.';
            };
        }
        function setWarningPrompt(onOff) {
            blnWarningPrompt = onOff;
        }
        function saveData(leavePage) {
            if (leavePage == false) {
                setWarningPrompt(false);
            } else {
                setWarningPrompt(true);
            }
        };

        function verifyContractSelected() {
            recid = jQuery('#<%=txtRecID.ClientID %>').val();
            if (recid == '') {
                return false;
            }
        }

        // numeric initializations
        jQuery('#<%=dolAmount.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPatientInitialBalance.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPatientInitialPayment.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPatientMonthlyPayment.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPatientDiscount.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPatientRemainingBalance.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPrimaryAmountBilled.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPrimaryClaim_Amount_Initial.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPrimaryClaim_Amount_Installment.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPrimaryCoverageAmt.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPrimaryInitialPayment.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPrimaryInstallmentAmt.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolPrimaryRemainingBalance.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolSecondaryAmountBilled.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolSecondaryClaim_Amount_Initial.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolSecondaryClaim_Amount_Installment.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolSecondaryCoverageAmt.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolSecondaryInitialPayment.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolSecondaryInstallmentAmt.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
        jQuery('#<%=dolSecondaryRemainingBalance.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});

        jQuery(document).ready(function () {
            //jQuery('.nav li.active').removeClass('active');
            //document.getElementById("#ContractEntry").className += " active";

            // date validations
            jQuery('#dtpContractDate')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });
            jQuery('#dtpPatientFirstPay')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });
            jQuery('#dtpDOB')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });
            jQuery('#dtpInsured_Date_Birth')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });
            jQuery('#dtpDate_Effective')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });
            jQuery('#dtpDate_Canceled')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });
            jQuery('#dtpInsured_Date_Birth_sec')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });            
            jQuery('#dtpDate_Effective_sec')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });
            jQuery('#dtpDate_Canceled_sec')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });            
            jQuery('#dtpEndContractDate')
                .datepicker({
                    format: 'mm/dd/yyyy'
                });

        });

        function showEditOptions() {
            jQuery("#divEditOptions").removeClass("hidden");
            jQuery("#divViewOptions").addClass("hidden");
            jQuery("#divViewOnly").addClass("hidden");
        }

        function getPatName(txtBox) {
            var strVal;
            if (txtBox == 'cht') {
                //searching on txtChartNumber
                strVal = 'cht' + jQuery('#' + '<%=txtChartNumber.ClientID%>').val();
            } else {
                //searching on txtAccountNumber
                strVal = 'act' + jQuery('#' + '<%=txtAccountNumber.ClientID%>').val();
            }
            jQuery.get("ajaxOrtho.aspx?action=getPatName", { id: strVal, frm: "Contracts" },
                 function (data) {
                     var strData = data;
                     if (strData.indexOf("~~") == -1) {
                         if (strData.indexOf("<h4>") == 0) {
                             jQuery('#divModalAddNew').modal('show');
                             jQuery('#loading_indicator').show();
                             jQuery('.modal-body-addNew').html(data);
                             jQuery('#loading_indicator').hide();
                         } else {
                             document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData;
                             jQuery('#showPatientData').addClass('hidden');
                        }
                    } else {
                        //strData = PatientNumber, PatientName, ChartNumber, FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB
                        document.getElementById('<%=txtAccountNumber.ClientID%>').value = strData.split("~~")[0];
                        document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData.split("~~")[1] + ' | Chart # ' + strData.split("~~")[2];
                        document.getElementById('<%=txtChartNumber.ClientID%>').value = strData.split("~~")[2];                         
                        document.getElementById('<%=txtname_first.ClientID%>').value = strData.split("~~")[3];
                        document.getElementById('<%=txtname_mid.ClientID%>').value = strData.split("~~")[4];
                        document.getElementById('<%=txtname_last.ClientID%>').value = strData.split("~~")[5];
                        document.getElementById('<%=txtaddr_other.ClientID%>').value = strData.split("~~")[6];
                        document.getElementById('<%=txtaddr_street.ClientID%>').value = strData.split("~~")[7];
                        document.getElementById('<%=txtaddr_city.ClientID%>').value = strData.split("~~")[8];
                        document.getElementById('<%=txtaddr_state.ClientID%>').value = strData.split("~~")[9];
                         document.getElementById('<%=txtaddr_zip.ClientID%>').value = strData.split("~~")[10];
                         if (strData.split("~~")[11] == 'F') {
                             document.getElementById('<%=ddlGender.ClientID%>').value = 1
                         } else {
                             document.getElementById('<%=ddlGender.ClientID%>').value = 2
                         }
                        document.getElementById('<%=dteDOB.ClientID%>').value = strData.split("~~")[12];
                         jQuery('#showPatientData').removeClass('hidden');
                         jQuery('#divContractSearch').addClass('hidden');
                    }
                });

        }

        function getPatNameCID(obj) {          
            cid = obj.value;
            jQuery('#btnPopupCancel').click();
            window.open("ContractEntry.aspx?cid=" + cid + "&mid=view", "_self")
        }

            function goToExistingContract() {
                strID = jQuery('#intCID')[0].innerHTML;
                window.open("ContractEntry.aspx?cid=" + strID + "&mid=view", "_self")
            }

            function keepdata(obj) {
                strVal = obj;
                var strPatId = jQuery('#intActID')[0].innerHTML;
                var strChtID = jQuery('#intChtID')[0].innerHTML;
                var strName = jQuery('#strName')[0].innerHTML;

                if (strVal == "yes") {
                    document.getElementById('<%=txtAccountNumber.ClientID%>').value = strPatId;
                    document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strName;
                    document.getElementById('<%=txtChartNumber.ClientID%>').value = strChtID;

                    strVal = 'cht' + strChtID
                    jQuery.get("ajaxOrtho.aspx?action=getPatName", { id: strVal, frm: "Contracts", addNew: true },
                 function (data) {
                     var strData = data;
                     if (strData.indexOf("~~") == -1) {
                         if (strData.indexOf("<h4>") == 0) {
                             //Display modal for contract selection
                             jQuery('#divModalAddNew').modal('show');
                             jQuery('#loading_indicator').show();
                             jQuery('.modal-body-addNew').html(data);
                             jQuery('#loading_indicator').hide();
                         } else {
                             document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData;
                             jQuery('#showPatientData').addClass('hidden');
                         }
                     } else {
                         //strData = PatientNumber, PatientName, ChartNumber, FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB
                         document.getElementById('<%=txtAccountNumber.ClientID%>').value = strData.split("~~")[0];
                         document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData.split("~~")[1];
                         document.getElementById('<%=txtChartNumber.ClientID%>').value = strData.split("~~")[2];
                         document.getElementById('<%=txtname_first.ClientID%>').value = strData.split("~~")[3];
                         document.getElementById('<%=txtname_mid.ClientID%>').value = strData.split("~~")[4];
                         document.getElementById('<%=txtname_last.ClientID%>').value = strData.split("~~")[5];
                         document.getElementById('<%=txtaddr_other.ClientID%>').value = strData.split("~~")[6];
                         document.getElementById('<%=txtaddr_street.ClientID%>').value = strData.split("~~")[7];
                         document.getElementById('<%=txtaddr_city.ClientID%>').value = strData.split("~~")[8];
                         document.getElementById('<%=txtaddr_state.ClientID%>').value = strData.split("~~")[9];
                         document.getElementById('<%=txtaddr_zip.ClientID%>').value = strData.split("~~")[10];
                         document.getElementById('<%=ddlGender.ClientID%>').value = strData.split("~~")[11];
                         document.getElementById('<%=dteDOB.ClientID%>').value = strData.split("~~")[12];
                         jQuery('#showPatientData').removeClass('hidden');
                     }
                 });

            } else {
                document.getElementById('<%=txtAccountNumber.ClientID%>').value = "";
                document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = "";
                document.getElementById('<%=txtChartNumber.ClientID%>').value = "";
            }
        }

        function showEndOfContract() {
            //Display div for data entry fields related to end of contract
            if (document.getElementById('<%=ddlStatus_vw.ClientID%>').value == -1) {
                alert('Please set Status to Active or Closed.');
                jQuery('#<%=ddlStatus_vw.ClientID%>').focus();
            }
            if (document.getElementById('<%=ddlStatus_vw.ClientID%>').value == 1) { 
                if (document.getElementById('<%=dteEndContractDate.ClientID%>').value != '') {
                    jQuery('#<%=dteEndContractDate.ClientID%>').focus();
                    alert('Be sure to remove the End of Contract Date and Reason if setting status back to Active.');
                } else {
                    jQuery('#divEndOfContract').addClass('hidden');
                }
            }
            if (document.getElementById('<%=ddlStatus_vw.ClientID%>').value == 2) {
                jQuery('#divEndOfContract').removeClass('hidden');
                jQuery('#<%=dteEndContractDate.ClientID%>').focus();
            }
        }

        function showAddNotes() {
            //Display modal for conttract selection
            jQuery('#divModalAddNotes').modal('show');
            jQuery('#loading_indicator').show();
            jQuery('#loading_indicator').hide();
        }

        function showViewNotes() {
            //Display modal for conttract selection
            jQuery('#divModalViewNotes').modal('show');
            document.getElementById('<%=txtViewNotes.ClientID%>').value = document.getElementById('<%=txtComments.ClientID%>').value
            jQuery('#loading_indicator').show();
            jQuery('#loading_indicator').hide();
        }

        function showPatientInfo() {
            //Display modal for primary insurance info
            jQuery('#loading_indicator').show();
            jQuery('#loading_indicator').hide();
        }

        function showInsurance(insType) {
            if (insType == '1') {
                //Display modal for primary insurance info
                jQuery('#loading_indicator').show();
                jQuery('#loading_indicator').hide();
            } else { 
                //Display modal for secondary insurance info
                jQuery('#loading_indicator').show();
                jQuery('#loading_indicator').hide();
            }
        }

        function checkPatientInsurance(obj, covType) {
            if (obj.selectedIndex > 0) {
                strInsCoName = obj.options[obj.selectedIndex].text;
                if (obj.name.indexOf("ddlPrimaryInsurancePlans_vw") > -1) {
                    var strPrimInsurProvider = document.getElementById('<%=lblPrimaryInsuranceProvider.ClientID%>').innerHTML;
                    if (document.getElementById('<%=lblPrimaryInsuranceProvider.ClientID%>').innerHTML != "") {
                        // alert user if patient insurance record provider differs from the one just selected
                        if (strInsCoName != document.getElementById('<%=lblPrimaryInsuranceProvider.ClientID%>').innerHTML) {
                            alert('Primary Coverage onfile does not match this provider. Please update this information as needed.');
                        }
                    } else {
                        // new insurance entry
                        document.getElementById('<%=lblPrimaryInsuranceProvider.ClientID%>').innerHTML = strInsCoName;
                        // default insurance fields to patient data, can be manually changed by user
                        document.getElementById('<%=txtInsured_name_first.ClientID%>').value = document.getElementById('<%=txtname_first.ClientID%>').value
                        document.getElementById('<%=txtInsured_name_mid.ClientID%>').value = document.getElementById('<%=txtname_mid.ClientID%>').value
                        document.getElementById('<%=txtInsured_name_last.ClientID%>').value = document.getElementById('<%=txtname_last.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_street.ClientID%>').value = document.getElementById('<%=txtaddr_other.ClientID%>').value + ' ' + document.getElementById('<%=txtaddr_street.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_city.ClientID%>').value = document.getElementById('<%=txtaddr_city.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_state.ClientID%>').value = document.getElementById('<%=txtaddr_state.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_zip.ClientID%>').value = document.getElementById('<%=txtaddr_zip.ClientID%>').value
                        document.getElementById('<%=ddlInsured_Gender.ClientID%>').value = document.getElementById('<%=ddlGender.ClientID%>').value
                        document.getElementById('<%=dteInsured_Date_Birth.ClientID%>').value = document.getElementById('<%=dteDOB.ClientID%>').value
                        document.getElementById('<%=ddlRelationship.ClientID%>').value = 1
                    }
                } else {
                    if (document.getElementById('<%=lblSecondaryInsuranceProvider.ClientID%>').innerHTML != "") {
                        // alert user if patient insurance record provider differs from the one just selected
                        if (strInsCoName != document.getElementById('<%=lblSecondaryInsuranceProvider.ClientID%>').innerHTML) {
                            alert('Secondary Coverage onfile does not match this provider. Please update this information as needed.');
                        }
                    } else {
                        // new insurance entry
                        document.getElementById('<%=lblSecondaryInsuranceProvider.ClientID%>').innerHTML = strInsCoName;   
                        // default insurance fields to patient data, can be manually changed by user
                        document.getElementById('<%=txtInsured_name_first_sec.ClientID%>').value = document.getElementById('<%=txtname_first.ClientID%>').value
                        document.getElementById('<%=txtInsured_name_mid_sec.ClientID%>').value = document.getElementById('<%=txtname_mid.ClientID%>').value
                        document.getElementById('<%=txtInsured_name_last_sec.ClientID%>').value = document.getElementById('<%=txtname_last.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_street_sec.ClientID%>').value = document.getElementById('<%=txtaddr_other.ClientID%>').value + ' ' + document.getElementById('<%=txtaddr_street.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_city_sec.ClientID%>').value = document.getElementById('<%=txtaddr_city.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_state_sec.ClientID%>').value = document.getElementById('<%=txtaddr_state.ClientID%>').value
                        document.getElementById('<%=txtInsured_addr_zip_sec.ClientID%>').value = document.getElementById('<%=txtaddr_zip.ClientID%>').value
                        document.getElementById('<%=ddlInsured_Gender_sec.ClientID%>').value = document.getElementById('<%=ddlGender.ClientID%>').value
                        document.getElementById('<%=dteInsured_Date_Birth_sec.ClientID%>').value = document.getElementById('<%=dteDOB.ClientID%>').value
                        document.getElementById('<%=ddlRelationship_sec.ClientID%>').value = 1
                    }
                }
            }
        }

        function cancelPolicy(CoverageType) {
            if (CoverageType == '1') {
                // check primary insurance 
                dteCanceled = jQuery('#' + '<%=dteDate_Canceled.ClientID%>').val()
                if (dteCanceled != "") {
                    jQuery.get("ajaxOrtho.aspx?action=moveInsuranceToHistory", { id: jQuery('#' + '<%=txtChartNumber.ClientID%>').val(), covType: CoverageType, frm: "Contracts" },
                    function (data) {
                        // clear out all textboxes so user can enter a new policy immediately
                        document.getElementById('<%=lblPrimaryInsuranceProvider.ClientID%>').innerHTML = '';
                        document.getElementById('<%=txtInsured_name_first.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_name_last.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_name_mid.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_name_suffx.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_street.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_city.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_state.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_zip.ClientID%>').value = '';
                        document.getElementById('<%=dteInsured_Date_Birth.ClientID%>').value = '';
                        document.getElementById('<%=ddlInsured_Gender.ClientID%>').value = '';
                        document.getElementById('<%=ddlRelationship.ClientID%>').value = '';
                        document.getElementById('<%=dteDate_Effective.ClientID%>').value = '';
                        document.getElementById('<%=dteDate_Canceled.ClientID%>').value = '';
                        document.getElementById('<%=txtEmployer_Name.ClientID%>').value = '';
                        document.getElementById('<%=txtGroup_Number.ClientID%>').value = '';
                        document.getElementById('<%=txtPolicy_Number.ClientID%>').value = '';
                        document.getElementById('<%=dolLifetimeMax.ClientID%>').value = '0';
                        document.getElementById('<%=ddlPrimaryInsurancePlans_vw.ClientID%>').selectedIndex = 0;
                        document.getElementById('<%=dolPrimaryCoverageAmt.ClientID%>').value = '0';
                        document.getElementById('<%=dolPrimaryInitialPayment.ClientID%>').value = '0';
                        document.getElementById('<%=dolPrimaryInstallmentAmt.ClientID%>').value = '0';
                        document.getElementById('<%=dolPrimaryRemainingBalance.ClientID%>').value = '0';
                        document.getElementById('<%=dolPrimaryAmountBilled.ClientID%>').value = '0';
                        document.getElementById('<%=dolPrimaryClaim_Amount_Initial.ClientID%>').value = '0';
                        document.getElementById('<%=dolPrimaryClaim_Amount_Installment.ClientID%>').value = '0';

                        alert('Primary Insurance Canceled and Archived. You may enter new insurance information now.');
                    });
                } else {
                    alert('You must enter a cancellation date.');
                }
            } else {
                // check secondary insurance 
                dteCanceled = jQuery('#' + '<%=dteDate_Canceled_sec.ClientID%>').val()
                if (dteCanceled != "") {
                    jQuery.get("ajaxOrtho.aspx?action=moveInsuranceToHistory", { id: jQuery('<%=txtChartNumber.ClientID%>').val(), covType: CoverageType, frm: "Contracts" },
                    function (data) {
                        // clear out all textboxes so user can enter a new policy immediately
                        document.getElementById('<%=lblSecondaryInsuranceProvider.ClientID%>').innerHTML = '';
                        document.getElementById('<%=txtInsured_name_first_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_name_last_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_name_mid_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_name_suffx_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_street_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_city_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_state_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtInsured_addr_zip_sec.ClientID%>').value = '';
                        document.getElementById('<%=dteInsured_Date_Birth_sec.ClientID%>').value = '';
                        document.getElementById('<%=ddlInsured_Gender_sec.ClientID%>').value = '';
                        document.getElementById('<%=ddlRelationship_sec.ClientID%>').value = '';
                        document.getElementById('<%=dteDate_Effective_sec.ClientID%>').value = '';
                        document.getElementById('<%=dteDate_Canceled_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtEmployer_Name_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtGroup_Number_sec.ClientID%>').value = '';
                        document.getElementById('<%=txtPolicy_Number_sec.ClientID%>').value = '';
                        document.getElementById('<%=dolLifetimeMax_sec.ClientID%>').value = '0';
                        document.getElementById('<%=ddlSecondaryInsurancePlans_vw.ClientID%>').selectedIndex = 0;
                        document.getElementById('<%=dolSecondaryCoverageAmt.ClientID%>').value = '0';
                        document.getElementById('<%=dolSecondaryInitialPayment.ClientID%>').value = '0';
                        document.getElementById('<%=dolSecondaryInstallmentAmt.ClientID%>').value = '0';
                        document.getElementById('<%=dolSecondaryRemainingBalance.ClientID%>').value = '0';
                        document.getElementById('<%=dolSecondaryAmountBilled.ClientID%>').value = '0';
                        document.getElementById('<%=dolSecondaryClaim_Amount_Initial.ClientID%>').value = '0';
                        document.getElementById('<%=dolSecondaryClaim_Amount_Installment.ClientID%>').value = '0';

                        alert('Secondary Insurance Canceled and Archived. You may enter new insurance information now.');
                    });
                } else {
                    alert('You must enter a cancellation date.');
                }
            }
        }

    </script>

    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
</asp:Content>
