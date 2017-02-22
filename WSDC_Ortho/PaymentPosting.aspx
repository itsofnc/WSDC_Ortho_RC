<%@ Page Title="" EnableEventValidation="false" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="PaymentPosting.aspx.vb" Inherits="WSDC_Ortho.PaymentPosting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <link href="Common/fonts/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPayments' class="form-horizontal" role="form" defaultbutton="btnHidden" action="PaymentPosting.aspx" runat="server">
        <!--hidden default button so enter key doesn't submit the form-->
        <asp:Button ID="btnHidden" cssclass="hidden" OnClientClick="return false" runat="server" />
        <!--11/29/2016 Added hidden textbox to store doctors_vw (recid) for payment data-->
        <asp:TextBox ID="txtDoctors_vw" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtPaymentSelection" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtPatientContractRecId" CssClass ="hidden" runat="server"></asp:TextBox>

        <div class="pull-left">   <!--class="container"-->
            <!--View only-->
            <div id="divViewOptions" class="col-sm-12" style="z-index: 1000;" runat="server">
                <div class="text-center">
                    <span class="glyphicon glyphicon-eye-open"></span>View Only Mode.  To Edit Payment <a href="#" onclick="showEditOptions()">(Click Here)</a>
                </div>
            </div>
            <!--View only end-->

            <div class="col-sm-12">
                <h4 class="text-center" style="color: #2d6ca2;">
                    <asp:Literal ID="litHeader" runat="server"></asp:Literal>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblPatientHeader" runat="server"></asp:Label>
                </h4>
                <h6 id="h6PatientSearch" class="text-center" style="color:red" runat="server">No patient found. Please review search criteria.</h6>
            </div>

            <!--02/23/16 - from contractStatus - select patient first-->
            <div class="col-sm-12 form-inline" style="text-align: center;">
                <label class="sr-only" for="txtFirstName">First Name</label>
                <div class="input-group">
                    <div class="input-group-addon" style="background-color:#d1d1d1">First Name</div>
                    <asp:TextBox ID="txtFirstName" CssClass="form-control" runat="server" onkeypress="checkKeyPressEnter(this.id, event)" />
                </div>
                <label class="sr-only" for="txtLastName">Last Name</label>
                <div class="input-group">
                    <div class="input-group-addon" style="background-color:#d1d1d1">Last Name</div>
                    <asp:TextBox ID="txtLastName" CssClass="form-control"  runat="server" onkeypress="checkKeyPressEnter(this.id, event)" />
                </div>
                <label class="sr-only" for="txtChartNumber">Chart #</label>
                <div class="input-group">
                    <div class="input-group-addon" style="background-color:#d1d1d1">Chart #</div>
                    <asp:TextBox ID="txtChartNumber" CssClass="form-control"  runat="server" onkeypress="if(checkKeyPressEnter(this.id, event)) {gotoNextFieldAfterEnter('btnSearch');}" />
                </div>
                <div class="input-group">
                   <asp:Button ID="btnSearch" CssClass="btn btn-default" runat="server" Text="Search" /> 
                   <asp:Button ID="btnClear" CssClass="btn btn-default" runat="server" Text="Clear" />                                        
                </div>
                <hr />
                <!-- hidden field just to contain the patient number for filling in the database-->
                <label for="txtPatientNumber" class="hidden">
                    Patient #:
                    <asp:TextBox ID="txtPatientNumber" name="txtPatientNumber" CssClass="DB form-control" runat="server"></asp:TextBox>
                </label>
            </div>
            <!--02/23/16 eom-->

            <!--Payment Collection Area-->
            <div id="divPaymentCollectionArea" runat="server">
                <!--Payer Info-->
                <div id="divPatientInfo" class="col-sm-12" style="color: #2d6ca2;" runat="server">
                    <div class="form-group">
                        <div class="col-sm-10"> <!--col-sm-offset-1-->
                            <div class="col-xs-12 col-sm-3">
                                <label for="ddlPaymentFrom">
                                    <span style="white-space:nowrap">*Payment From:</span>
                                    <asp:DropDownList ID="ddlPaymentFrom" name="ddlPaymentFrom" CssClass="DB  form-control PromptSelection required" runat="server"
                                        onchange="pmtFromChange('');" onkeypress="checkKeyPressEnter(this.id, event)" >
                                    </asp:DropDownList>
                                </label>
                            </div>
                             <div id="divPayerName" class="col-xs-12 col-sm-3">
                                <label for="txtPayerName">
                                    *Payer Name:
                                    <asp:TextBox ID="txtPayerName" name="txtPayerName" CssClass="DB form-control required" ToolTip="" onkeypress="checkKeyPressEnter(this.id, event)"  runat="server"></asp:TextBox> <%--onchange="setPayerName(this.value)"--%>
                                </label>
                            </div>
                            <div class="col-xs-12 col-sm-3">
                                <label for="ddlPaymentType">
                                    *Payment Type:
                                    <asp:DropDownList ID="ddlPatPaymentType" name="ddlPatPaymentType" CssClass="DB  form-control  PromptSelection required" runat="server" onchange="setPmtTypeDDL('patient');" ></asp:DropDownList>                                                     
                                    <asp:DropDownList ID="ddlInsPaymentType" name="ddlInsPaymentType" CssClass="DB  form-control  PromptSelection required hidden" runat="server" onchange="setPmtTypeDDL('insurnace');"></asp:DropDownList>    
                                </label>
                            </div>
                            <div class="col-xs-12 col-sm-3">
                                <label for="txtPaymentReference">
                                    Reference:
                                    <asp:TextBox ID="txtPaymentReference" name="txtPaymentReference" CssClass="DB form-control" ToolTip="" onkeypress="checkKeyPressEnter(this.id, event)" runat="server"></asp:TextBox>
                                </label>
                            </div>         
                        </div>
                    </div>
                </div>

                <!--Payment Amount Info-->
                <div id="divPaymentInfo" class="col-xs-12 col-sm-12" runat="server">
                    <div class="form-group">
                        <div class=" col-sm-10"> <!--col-sm-offset-1-->
                            <div class="col-xs-12 col-sm-3">
                                <label for="dolPaymentAmount">
                                    <span style="white-space:nowrap">*Payment Amount:</span>
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPaymentAmount" name="dolPaymentAmount" type="text" CssClass="DB form-control required" Style="text-align:right;" 
                                            onkeypress="checkKeyPressEnter(this.id, event)" onchange="autoApplyPayment(this.value, '');" runat="server"></asp:TextBox>  
                                    </span>
                                </label>
                            </div>
                            <div id="divApplyBalance" class="col-xs-12 col-sm-3">
                                <label for="dolApplyBalance">
                                    <span style="white-space:nowrap">Remaining:</span>
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolApplyRemaining" name="dolApplyRemaining" type="text" CssClass="DB form-control" runat="server" Enabled="false" Style="text-align:right;"></asp:TextBox>  
                                    </span>
                                </label>
                            </div>  
                            <!--01.04.17 cpb add display of payments to be applied to invoices-->
                            <div id="divOverPayments" class="col-xs-12 col-sm-3">
                                <label for="dolOverPayment">
                                    <span id="spnPatientTip" style="white-space:nowrap">Credit On Account:
                                        <a href="#" data-toggle="tooltip" title="This amount will be automatically applied to the next invoice when it is processed." tabindex="-1">
                                            <span class="glyphicon glyphicon-question-sign"></span>
                                        </a>
                                    </span>
                                    <span id="spnPrimaryTip" style="white-space:nowrap" class="hidden">Primary Credit:
                                        <a href="#" data-toggle="tooltip" title="This amount will be automatically applied to the next claim when it is processed." tabindex="-1">
                                            <span class="glyphicon glyphicon-question-sign"></span>
                                        </a>
                                    </span>
                                    <span id="spnSecondaryTip" style="white-space:nowrap" class="hidden">Secondary Credit:
                                        <a href="#" data-toggle="tooltip" title="This amount will be automatically applied to the next claim when it is processed." tabindex="-1">
                                            <span class="glyphicon glyphicon-question-sign"></span>
                                        </a>
                                    </span>
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolOverPayment" name="dolOverPayment" type="text"  CssClass="DB form-control" runat="server" Enabled="false" Style="text-align:right;"></asp:TextBox>  
                                        <asp:TextBox ID="dolOverPaymentPrimary" name="dolOverPaymentPrimary" type="text"  CssClass="DB form-control hidden" runat="server" Enabled="false" Style="text-align:right;"></asp:TextBox>  
                                        <asp:TextBox ID="dolOverPaymentSecondary" name="dolOverPaymentSecondary" type="text"  CssClass="DB form-control hidden" runat="server" Enabled="false" Style="text-align:right;"></asp:TextBox>  
                                    </span>
                                </label>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Patient Apply To Info-->
                <div id="divPatientPaymentsApply" class="col-xs-12 col-sm-12" runat="server">
                    <div class="form-group">
                        <div class="col-sm-10"> <!--col-sm-offset-1 -->
                            <div id="divApplyCurrent" class="col-xs-12 col-sm-3">
                                <label for="dolPatientCurrent">
                                    <span style="white-space: nowrap">Current:
                                        <asp:Label ID="lblPatientCurrent" runat="server" Text="" style="color:green"></asp:Label>
                                        <asp:Label ID="lblPatientCurrentPend" runat="server" Text="" CssClass="pull-right alert-info" ></asp:Label>
                                    </span>
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <!--01.31.17 cpb removed required as part of the cssclass -->
                                        <asp:TextBox ID="dolPatientCurrent" name="dolPatientCurrent" type="text" CssClass="DB form-control" Style="text-align:right;" runat="server"
                                            onblur ="checkFieldPaymentAmount(this.id, this.value, 'hid');calculateTotalPayment();"></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                            <div id="divApplyPrior" class="col-xs-12 col-sm-3">
                                <label for="dolPatientPastDue">
                                    <span style="white-space: nowrap">Past Due:
                                        <asp:Label ID="lblPatientPastDue" runat="server" Text="" style="color:red"></asp:Label>
                                        <asp:Label ID="lblPatientPastDuePend" runat="server" Text="" CssClass="pull-right alert-info" ></asp:Label>
                                    </span>
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPatientPastDue" name="dolPatientPastDue" type="text" CssClass="DB form-control" Style="text-align:right;" runat="server" 
                                            onblur ="checkFieldPaymentAmount(this.id, this.value, 'hid');calculateTotalPayment();"
                                            ></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                            <div id="divApplyNextInvoice" class="col-xs-12 col-sm-3">
                                <label for="dolPatientNext">
                                    <span style="white-space: nowrap;">Next Inv/Credit:
                                        <asp:Label ID="lblPatientNext" runat="server" Text="" style="color:green"></asp:Label>
                                        <asp:Label ID="lblPatientNextPend" runat="server" Text="" CssClass="pull-right alert-info" ></asp:Label>
                                    </span>
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPatientNext" name="dolPatientNext" type="text" CssClass="DB form-control" Style="text-align:right;" runat="server" 
                                             onblur ="checkFieldPaymentAmount(this.id, this.value, 'hid');calculateTotalPayment();"
                                            ></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                            <div id="divApplyPatienceBalance" class="col-xs-12 col-sm-3">
                                <label for="dolPatientBalance">
                                    <span style="white-space: nowrap">Balance:
                                        <asp:Label ID="lblPatientBalance" runat="server" Text="" style="color:green"></asp:Label>
                                        <asp:Label ID="lblPatientBalancePend" runat="server" Text="" CssClass="pull-right alert-info" ></asp:Label>
                                    </span>
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPatientBalance" name="dolPatientBalance" type="text" CssClass="DB form-control" Style="text-align:right;" runat="server" 
                                            onblur ="checkFieldPaymentAmount(this.id, this.value, 'hid');calculateTotalPayment();"
                                            ></asp:TextBox>
                                    </span>
                                </label>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Insurance Apply To Info  current/secondary -->
                <div id="divInsurancePaymentsApply" class="col-xs-12 col-sm-12 hidden" runat="server">
                    <div id="divClaimNumber" class="form-group">
                        <div class=" col-sm-10"> <!--col-sm-offset-1-->
                            <!--use table instead of text boxes-->
                            <table id="tblClaimInvoiceData" class="table table-bordered">
                                <thead>
                                    <tr>
                                        <th style="text-align:center; white-space:nowrap; min-width:110px;">Insurance</th>
                                        <th style="text-align: center; white-space:nowrap; min-width:100px;">Policy Holder</th>
                                        <th style="text-align:center; white-space:nowrap; min-width:60px;">Claim #</th>
                                        <th style="text-align:center; white-space:nowrap; min-width:65px;">Date</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Expected $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Due $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Pend $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Apply $&nbsp;&nbsp;&nbsp;&nbsp;</th>
                                    </tr>
                                </thead> 
                                <tbody>
                                    <asp:Literal ID="litInsuranceClaimsTable" runat="server"></asp:Literal>
                                </tbody>
                            </table>                            
                        </div>
                    </div>
                </div>
                
                <!--Insurance Apply To Info secondary -->
                <div id="divInsurancePaymentApplySecondary" class="col-xs-12 col-sm-12 hidden" runat="server">
                    <div id="divClaimNumberSecondary" class="form-group">
                        <div class=" col-sm-10"> <!--col-sm-offset-1-->
                            <!--use table instead of text boxes-->
                            <table id="tblClaimInvoiceDataSecondary" class="table table-bordered">
                                <thead>
                                    <tr>
                                        <th style="text-align:center; white-space:nowrap; min-width:110px;">Insurance</th>
                                        <th style="text-align: center; white-space:nowrap; min-width:100px;">Policy Holder</th>
                                        <th style="text-align:center; white-space:nowrap; min-width:60px;">Claim #</th>
                                        <th style="text-align:center; white-space:nowrap; min-width:65px;">Date</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Expected $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Due $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Pend $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:120px;">Apply $&nbsp;&nbsp;&nbsp;&nbsp;</th>
                                    </tr>
                                </thead> 
                                <tbody>
                                    <asp:Literal ID="litInsuranceClaimsTableSecondary" runat="server"></asp:Literal>
                                </tbody>
                            </table>                            
                        </div>
                    </div>
                </div>

                <!--Patient Invoice Detail Table 01.11.17 cpb-->
                <div id="divPatientInvoiceDetail" class="col-xs-12 col-sm-12 hidden" runat="server">
                    <div id="divInvoiceNumber" class="form-group">
                        <div class=" col-sm-10"> <!--col-sm-offset-1-->
                            <table id="tblPatientInvoiceData" class="table table-bordered">
                                <thead>
                                    <tr>
                                        <th style="text-align:center; white-space:nowrap; min-width:150px;">Status</th>
                                        <th style="text-align: center; white-space:nowrap; min-width:150px;">Name</th>
                                        <th style="text-align:center; white-space:nowrap; min-width:75px;">Invoice #</th>
                                        <th style="text-align:center; white-space:nowrap; min-width:75px;">Date</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Balance $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:75px;">Pend $</th>
                                        <th style="text-align:right; white-space:nowrap; min-width:120px;">Apply $&nbsp;&nbsp;&nbsp;&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Literal ID="litPatientInvoiceTable" runat="server"></asp:Literal>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <div id="divPaymentForComment" class="col-sm-12">
                    <div class="form-group">
                        <div class=" col-sm-10"> <!--col-sm-offset-1-->
                            <div id="divDDLPaymentFor" class="col-xs-12 col-sm-6">
                                <label for="ddlPaymentFor">
                                    Payment For:
                                    <asp:DropDownList ID="ddlPaymentFor" name="ddlPaymentFor" CssClass="DB  form-control  PromptSelection" runat="server"></asp:DropDownList>
                                </label>
                            </div>
                            <div id="divComments" class="col-xs-12 col-sm-6">
                                <label for="txtComments">
                                    Comments:
                                    <asp:TextBox ID="txtComments" name="txtComments" CssClass="DB form-control" runat="server" TextMode="MultiLine" Rows="2" Columns="50"></asp:TextBox>
                                </label>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="divPaymentButtons" class="col-xs-12 col-sm-12" runat="server">
                    <div class="form-group">
                        <div class="col-sm-10"> <!--col-sm-offset-1-->
                            <div id="divButtons" class="col-xs-12 col-sm-4">
                                <div id="divBtnOptions" runat="server">
                                    <asp:Button ID="btnAdd" cssClass="btn btn-large btn-success"  runat="server" Text="Add to Queue" OnClientClick="jQuery('#divWait').show(); setWarningPrompt(false); blnOkToAdd = addToQueue(); if (blnOkToAdd == false) { return false;};" />
                                    <asp:Button ID="btnCancel" cssclass="btn btn-large btn-primary" runat="server" Text="Cancel" OnClientClick="jQuery('#divWait').show(); setWarningPrompt(false); blnOkToCancel = confirmCancel('');if (blnOkToCancel == false) { return false;};" />
                                </div>
                                <div id="divBtnEditOptions"  runat="server">
                                    <asp:Button ID="btnSave" cssclass="btn btn-large btn-success " OnClientClick="jQuery('#divWait').show(); setWarningPrompt(false); blnOkToUpdate = addToQueue(); if (blnOkToUpdate == false) { return false;}; " runat="server" Text="Save Changes" />  
                                    <asp:Button ID="btnDelete" cssclass="btn btn-large btn-warning" OnClientClick="jQuery('#divWait').show(); setWarningPrompt(false); blnDelete = deletePayment(); if (blnDelete == false) { return false;};" runat="server" Text="Delete" />
                                    <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" cssclass="btn btn-large btn-danger" OnClientClick="jQuery('#divWait').show(); setWarningPrompt(false); blnOkToCancel = confirmCancel('edit');if (blnOkToCancel == false) { return false;};" />
                                </div>
                                <div id="divBtnViewOptions"  runat="server">
                                    <asp:Button ID="btnClose" runat="server" Text="Close View" cssclass="btn btn-large btn-primary" />
                                </div>
                            </div>
                            <div class="col-xs-12 col-sm-6">
                                <label id="lblPaymentMessage" class="alert-danger"></label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--End Payment Details Collection Area-->
            <div class="col-sm-12">
                <hr />
                <h4>Payments Queue&nbsp;
                    <a id="lnkHidePending" href="#" onclick="showHidePending('hide')")>(Hide)</a>
                    <a id="lnkShowPending" href="#" onclick="showHidePending('show')" style="display: none;")>(Show)</a>
                </h4>
            </div>
            <div id="divPendingPayments" runat="server">
                <!--page body-->
                <asp:Literal ID="litNoData" runat="server"></asp:Literal>
                <!--iframe Begin-->
                <div id="divIframe">
                    <iframe id="ifmPayment" src="frmListManager.aspx?id=PaymentsTemp_vw&usr=True&divHide=divHeader,divPaginationHeading,divPagination,navPage,divFooter&perm=11000" 
                        style="border: 0px; min-height: 250px; width: 100%; z-index: 1000; position: relative;"></iframe>
                </div>
                <!--iframe End-->
                <div class="col-sm-12">
                    <div class="form-group col-sm-12">
                        <asp:Button ID="btnPost" cssclass="btn btn-large btn-primary center-block" UseSubmitBehavior="false" Style="z-index: 1000; position: relative;" runat="server" Text="Post Payments" />
                    </div>
                </div>
            </div>
        </div>
        <!--Container end-->

        <!--Hidden Elements-->
        <asp:HiddenField ID="txtSessions" runat="server" />
        <asp:HiddenField ID="hidFocusField" runat="server" />
        <asp:HiddenField ID="hidPrimaryInsurance" runat="server" />
        <asp:HiddenField ID="hidPrimaryInsuranceBalance" runat="server" />
        <asp:HiddenField ID="hidPrimaryInsurancePlanId" runat="server" />
        <asp:HiddenField ID="hidSecondaryInsurance" runat="server" />
        <asp:HiddenField ID="hidSecondaryInsuranceBalance" runat="server" />
        <asp:HiddenField ID="hidSecondaryInsurancePlanId" runat="server" />
        <asp:HiddenField ID="hidInsCount" runat="server" />
        <asp:HiddenField ID="hidInsRefList" runat="server" />
        <asp:HiddenField ID="hidDefaultPaymentFor" runat="server" />
        <asp:HiddenField ID="hidMode" runat="server" />
        <asp:HiddenField ID="hidUnloadWarning" runat="server" />
        <asp:HiddenField ID="hidPatientCurrent" runat="server" />
        <asp:HiddenField ID="hidPatientCurrentPend" runat="server" />
        <asp:HiddenField ID="hidPatientPastDue" runat="server" />
        <asp:HiddenField ID="hidPatientPastDuePend" runat="server" />
        <asp:HiddenField ID="hidPatientNext" runat="server" />
        <asp:HiddenField ID="hidPatientNextPend" runat="server" />
        <asp:HiddenField ID="hidPatientBalance" runat="server" />
        <asp:HiddenField ID="hidPatientBalancePend" runat="server" />
        <div id="divHiddenElements" hidden="hidden">
            <!--These elements need to exist for the autocoder, but are being handled manually with other code.-->
            <asp:TextBox ID="dtePaymentDate" name="dtePaymentDate" CssClass="DB"  runat="server"></asp:TextBox>  
            <asp:DropDownList ID="ddlPaymentType" name="ddlPaymentType" CssClass="DB" runat="server"></asp:DropDownList>  
            <asp:TextBox ID="txtContract_RECID" CssClass="DB" runat="server"></asp:TextBox>
            <asp:TextBox ID="dolOrig_Payment" name="dolOrig_Payment" CssClass="DB" runat="server"></asp:TextBox>
            <!--patient payment-->
            <asp:TextBox ID="dolPatientAmount" name="dolPatientAmount" CssClass="DB" runat="server"></asp:TextBox>
            <asp:TextBox ID="dolApplyToCurrentInvoice" name="dolApplyToCurrentInvoice" CssClass="DB" runat="server"></asp:TextBox>                                                                                                
            <asp:TextBox ID="dolApplyToPastDue" name="dolApplyToPastDue" CssClass="DB" runat="server"></asp:TextBox>
            <asp:TextBox ID="dolApplyToNextInvoice" name="dolApplyToNextInvoice" CssClass="DB" runat="server"></asp:TextBox>                                                                                                
            <asp:TextBox ID="dolApplyToPrinciple" name="dolApplyToPrinciple" CssClass="DB" runat="server"></asp:TextBox>                                                                                                
            <!--insurance payment-->
            <asp:TextBox ID="dolPrimaryAmount" name="dolPrimaryAmount" CssClass="DB" runat="server"></asp:TextBox>
            <asp:TextBox ID="dolSecondaryAmount" name="dolSecondaryAmount" CssClass="DB" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtClaimNumber" name="txtClaimNumber" runat="server"></asp:TextBox>
        </div>
        <!--Hidden Elements End-->
        <div id="divWait" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>
    </form>

    <!--Modal Contract Selection Popup-->
    <div class="modal fade" id="divModalContractSelection" tabindex="-1" role="dialog" aria-labelledby="divModalContractSelectionLabel" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">More than One Patient</h4>
                </div>
                <div class="modal-body modal-contract">
                    <h5>Please select a patient.</h5>
                    <h5 class="sr-only"> Please select a patient.</h5>
                    <select id="selSelectPatient" onchange="patientSelectedFromDDL()" class="form-control" style="max-width:150px">
                        <option value="-1">Choose one</option>
                        <asp:Literal ID="litOptions" runat="server"></asp:Literal>
                    </select>
                </div>
            </div>
        </div>
    </div>
    <!-- /.modal -->    
    <!--Modal Multiple Contracts Selection Popup-->
    <div class="modal fade" id="divModalContractSelectionMultiples" tabindex="-1" role="dialog" aria-labelledby="divModalContractSelectionMultiplesLabel" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">More than One Contract Found</h4>
                </div>
                <div class="modal-body modal-contract">
                    <h5>Please select a contract.</h5>
                    <h5 class="sr-only"> Please select a contract.</h5>
                    <select id="selSelectContract" onchange="contractSelectedFromDDL()" class="form-control" style="max-width:150px">
                        <option value="-1">Choose one</option>
                        <asp:Literal ID="litContractOptions" runat="server"></asp:Literal>
                    </select>
                </div>
            </div>
        </div>
    </div>
    <!-- /.modal -->   

    <!--Modal Payments already being entered Popup-->
    <div class="modal fade" id="divModalPaymentsBeingEntered" tabindex="-1" role="dialog" aria-labelledby="divModalPaymentsBeingEnteredLabel" aria-hidden="false" style="z-index:99999">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title"><br />Payments are already being entered.</h4>
                </div>
                <div class="modal-body modal-contract">
                    <h4>Sorry, payments are already being entered for this patient by:<br /><br />
                        <asp:Literal ID="litPaymentUsers" runat="server"></asp:Literal>
                        <br /><br />Please wait for current payments to be posted and try again.
                    </h4>
                </div>
            </div>
        </div>
    </div>
    <!-- /.modal -->  

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <asp:Literal ID="litInsurnaceScript" runat="server"></asp:Literal>
    <script>
     //Scripts for frmListManager iFrame
        window.onresize = fixIframeSize;
        function fixIframeSize() {
            document.getElementById("ifmPayment").width = document.getElementById("divIframe").offsetWidth - 20;
        }

        jQuery(document).ready(fixIframeSize());

        function queryIframe() {
            ifmObj = document.frames.ifmPayment.getAspElement("txtSearch");
            alert(ifmObj.value);
        }
        //identify elements you want to hide in the iFrame's  frmListManager's (elements are in master page )
        function childLoaded() {
            ifrm = document.getElementById("ifmPayment").contentWindow;
            ifrm.document.body.style.paddingTop = '0px';
        }
    </script>

    <script type="text/javascript">

        var decFocusedPaymentAmount = 0.00;
        var blnProcessOnBlur = true;

        jQuery(document).ready(function () {
            //update menu
            $('.nav li.active').removeClass('active');
            document.getElementById("#PaymentPosting").className += " active";

            jQuery('[data-toggle="tooltip"]').tooltip();

            //Add focus to Chart Number
            if (jQuery('#<%=hidFocusField.ClientID%>').val() != '') {
                focusField = jQuery('#<%=hidFocusField.ClientID%>').val();
                jQuery('#' + focusField).select();
                jQuery('#' +focusField).focus();
            };

            //show/hide patient dollars as necessary -- balance never disabled - overpayment goes in balance
            if (jQuery('#<%=hidMode.ClientID%>').val() == 'view') {
                jQuery('#<%=dolPatientCurrent.ClientID%>').prop('disabled', true);
                jQuery('#<%=dolPatientNext.ClientID%>').prop('disabled', true);
                jQuery('#<%=dolPatientPastDue.ClientID%>').prop('disabled', true);
                jQuery('#<%=dolPatientBalance.ClientID%>').prop('disabled', true);
                jQuery('#<%=divBtnEditOptions.ClientID%>').addClass("hidden");
                jQuery('#<%=divBtnOptions.ClientID%>').addClass('hidden');
            } else {
                enableDisableDollarsByBalance();
                if (jQuery('#<%=hidMode.ClientID%>').val() == 'edit') {
                    jQuery('#<%=divBtnViewOptions.ClientID%>').addClass('hidden');
                    jQuery('#<%=divBtnOptions.ClientID%>').addClass('hidden');
                } else {
                    jQuery('#<%=divBtnEditOptions.ClientID%>').addClass('hidden');
                    jQuery('#<%=divBtnViewOptions.ClientID%>').addClass('hidden');
                }
            };
            jQuery('#<%=dolPaymentAmount.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
            jQuery('#<%=dolPatientCurrent.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
            jQuery('#<%=dolPatientPastDue.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
            jQuery('#<%=dolPatientNext.ClientID%>').autoNumeric('init', {aSep: ',', aDec: '.'});
            jQuery('#<%=dolPatientBalance.ClientID%>').autoNumeric('init', { aSep: ',', aDec: '.' });

            // if in edit mode - set previous payment amounts
            loadInsArrayPaymentEdit();
            pmtFromChange('load');

            jQuery('#frmPayments').formValidation({
                fields: {
                }
            })
            .on('err.field.fv', function(e, data) {
                if (data.fv.getSubmitButton()) {
                    data.fv.disableSubmitButtons(false);
                }
            })
            .on('success.field.fv', function(e, data) {
                if (data.fv.getSubmitButton()) {
                    data.fv.disableSubmitButtons(false);
                };
            })
            .on('success.form.fv', function(e) {
                //functions go here
            });

        });

        // capture user leaving the content edit area
        var blnWarningPrompt = jQuery('#<%=hidUnloadWarning.ClientID%>').val();
        window.onbeforeunload = function () {
            if (blnWarningPrompt == true || blnWarningPrompt == 'True') {
                return 'Leaving Payment Posting, You Will Lose Any UnSaved Data!';
            };
        }
        function setWarningPrompt(onOff) {
            blnWarningPrompt = onOff;
        }

        function showHidePending(type) {
            if (type == 'show') {
                jQuery('#<%=divPendingPayments.ClientID%>').show();
                jQuery('#lnkHidePending').show();
                jQuery('#lnkShowPending').hide();
            } else {
                jQuery('#<%=divPendingPayments.ClientID%>').hide();
                jQuery('#lnkHidePending').hide();
                jQuery('#lnkShowPending').show();
            };
        }

        function enableDisableDollarsByBalance() {
            decPatientCurrent = parseFloat(jQuery('#<%=hidPatientCurrent.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            decPatientCurrentPend = parseFloat(jQuery('#<%=hidPatientCurrentPend.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            decPatientPastDue = parseFloat(jQuery('#<%=hidPatientPastDue.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            decPatientPastDuePend = parseFloat(jQuery('#<%=hidPatientPastDuePend.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            decPatientNext = parseFloat(jQuery('#<%=hidPatientNext.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            decPatientNextPend = parseFloat(jQuery('#<%=hidPatientNextPend.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            decPatientBalance = parseFloat(jQuery('#<%=hidPatientBalance.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            decPatientBalancePend = parseFloat(jQuery('#<%=hidPatientBalancePend.ClientID%>').val().replace(',', '').replace('$', '')).toFixed(2);
            if (+decPatientCurrent - +decPatientBalancePend > 0.00) {
                jQuery('#<%=dolPatientCurrent.ClientID%>').prop('disabled', false);
            } else {
                jQuery('#<%=dolPatientCurrent.ClientID%>').prop('disabled', true);
            };
            if (+decPatientPastDue - +decPatientPastDuePend > 0.00) {
                jQuery('#<%=dolPatientPastDue.ClientID%>').prop('disabled', false);
            } else {
                jQuery('#<%=dolPatientPastDue.ClientID%>').prop('disabled', true);
            };
            if (+decPatientNext - +decPatientNextPend > 0.00) {
                jQuery('#<%=dolPatientNext.ClientID%>').prop('disabled', false);
            } else {
                jQuery('#<%=dolPatientNext.ClientID%>').prop('disabled', true);
            };
            if (+decPatientBalance - +decPatientBalancePend > 0.00) {
                jQuery('#<%=dolPatientBalance.ClientID%>').prop('disabled', false);
            } else {
                jQuery('#<%=dolPatientBalance.ClientID%>').prop('disabled', true);
            };

            jQuery('#<%=dolPatientNext.ClientID%>').prop('disabled', false);
        }

        function patientSelectedFromDDL() {
            jQuery("#<%=txtPatientContractRecId.ClientID%>").val('-1');
            jQuery("#<%=txtChartNumber.ClientID%>").val(jQuery("#selSelectPatient").val());
            jQuery("#<%=txtFirstName.ClientID%>").val('');
            jQuery("#<%=txtLastName.ClientID%>").val('');
            jQuery('#divModalContractSelection').modal('hide');
            jQuery("#<%=btnSearch.ClientID%>").click();
        }

        function contractSelectedFromDDL() {
            // 01.09.17 added to select specific contract for payments posting.
            jQuery("#<%=txtPatientContractRecId.ClientID%>").val(jQuery("#selSelectContract").val());
            jQuery("#<%=txtChartNumber.ClientID%>").val(jQuery("#selSelectPatient").val());
            jQuery("#<%=txtFirstName.ClientID%>").val('');
            jQuery("#<%=txtLastName.ClientID%>").val('');
            jQuery('#divModalContractSelection').modal('hide');
            jQuery("#<%=btnSearch.ClientID%>").click();
        }

        function pmtFromChange(mode) {
            strPmtFrom = jQuery('#<%=ddlPaymentFrom.ClientID%>').val();
            if (strPmtFrom == null) { } else {
                blnContinue = true;
                if (strPmtFrom.indexOf('Patient') == 0) {
                    if (mode == 'load') { } else {
                        if (jQuery('#<%=dolPaymentAmount.ClientID%>').val() == jQuery('#<%=dolApplyRemaining.ClientID%>').val()) { } else {
                            blnContinue = confirm('Insurance Distributions will be removed.  Ok to Continue?');
                        };
                    };
                    if (blnContinue) {
                        clearApplyFields('insurance');
                        jQuery('#<%=ddlPatPaymentType.ClientID%>').removeClass('hidden');
                        jQuery('#<%=ddlInsPaymentType.ClientID%>').addClass('hidden');
                        setPmtTypeDDL('patient');
                        if (mode == 'load') { } else {
                            jQuery('#<%=txtPayerName.ClientID%>').val('');
                        };
                        jQuery('#<%=divPatientPaymentsApply.ClientID%>').removeClass('hidden');
                        jQuery('#<%=divPatientInvoiceDetail.ClientID%>').removeClass('hidden');
                        jQuery('#<%=divInsurancePaymentsApply.ClientID%>').addClass('hidden');
                        jQuery('#<%=divInsurancePaymentApplySecondary.ClientID%>').addClass('hidden');
                        jQuery('#<%=txtPaymentSelection.ClientID%>').val('PatientAmount');
                        jQuery('#spnPatientTip').removeClass('hidden');
                        jQuery('#spnPrimaryTip').addClass('hidden');
                        jQuery('#spnSecondaryTip').addClass('hidden');
                        jQuery('#<%=dolOverPayment.ClientID%>').removeClass('hidden');
                        jQuery('#<%=dolOverPaymentPrimary.ClientID%>').addClass('hidden');
                        jQuery('#<%=dolOverPaymentSecondary.ClientID%>').addClass('hidden');
                    };
                } else {
                    clearApplyFields('patient');
                    jQuery('#<%=ddlInsPaymentType.ClientID%>').removeClass('hidden');
                    jQuery('#<%=ddlPatPaymentType.ClientID%>').addClass('hidden');
                    jQuery('#<%=dolOverPayment.ClientID%>').addClass('hidden');
                    jQuery('#spnPatientTip').addClass('hidden');
                    setPmtTypeDDL('insurance');
                    if (mode == 'load') { } else {
                        if (strPmtFrom.indexOf('Primary') == 0) {
                            jQuery('#<%=txtPayerName.ClientID%>').val(jQuery('#<%=hidPrimaryInsurance.ClientID%>').val());
                            jQuery('#<%=txtPaymentSelection.ClientID%>').val('PrimaryAmount');
                        } else {
                            if (strPmtFrom.indexOf('Secondary') == 0) {
                                jQuery('#<%=txtPayerName.ClientID%>').val(jQuery('#<%=hidSecondaryInsurance.ClientID%>').val());
                                jQuery('#<%=txtPaymentSelection.ClientID%>').val('SecondaryAmount');
                            } else {
                                jQuery('#<%=txtPayerName.ClientID%>').val(strPmtFrom.replace('Other-', ''));
                                jQuery('#<%=txtPaymentSelection.ClientID%>').val('PrimaryAmount');
                            }
                        };
                    };
                    jQuery('#<%=divPatientPaymentsApply.ClientID%>').addClass('hidden');
                    jQuery('#<%=divPatientInvoiceDetail.ClientID%>').addClass('hidden');
                    if (strPmtFrom.indexOf('Primary') == 0) {
                        jQuery('#<%=divInsurancePaymentsApply.ClientID%>').removeClass('hidden');
                        jQuery('#<%=dolOverPaymentPrimary.ClientID%>').removeClass('hidden');
                        jQuery('#<%=divInsurancePaymentApplySecondary.ClientID%>').addClass('hidden');
                        jQuery('#<%=dolOverPaymentSecondary.ClientID%>').addClass('hidden');
                        jQuery('#spnPrimaryTip').removeClass('hidden');
                        jQuery('#spnSecondaryTip').addClass('hidden');
                    } else {
                        jQuery('#<%=divInsurancePaymentApplySecondary.ClientID%>').removeClass('hidden');
                        jQuery('#<%=dolOverPaymentSecondary.ClientID%>').removeClass('hidden');
                        jQuery('#<%=divInsurancePaymentsApply.ClientID%>').addClass('hidden');
                        jQuery('#<%=dolOverPaymentPrimary.ClientID%>').addClass('hidden');
                        jQuery('#spnPrimaryTip').addClass('hidden');
                        jQuery('#spnSecondaryTip').removeClass('hidden');
                    }
                    
                };
                autoApplyPayment(jQuery('#<%=dolPaymentAmount.ClientID%>').val(), mode);
            };
        };

        function setPmtTypeDDL(type) {
            if (type == 'patient') {
                jQuery('#<%=ddlPaymentType.ClientID%>').val(jQuery('#<%=ddlPatPaymentType.ClientID%>').val());
            } else {
                jQuery('#<%=ddlPaymentType.ClientID%>').val(jQuery('#<%=ddlInsPaymentType.ClientID%>').val());
            }
        }

        function checkKeyPressEnter(fld, e) {
            if (e.keyCode === 13) {
                return true;
            } else {
                return false;
            }
        }

        function gotoNextFieldAfterEnter(fld) {
            if (fld == 'btnSearch') {
                jQuery('#<%=btnSearch.ClientId%>').click();
            } else {
                alert(fld);
            }
            
        }

        //function checkEnterPaymentAmount(fld, event, val) {
        //    if (checkKeyPressEnter(fld, event)) {
        //        blnProcessOnBlur = false;
        //        checkFieldPaymentAmount(fld, val);
        //        index = fld.indexOf("Patient");
        //        if (index == -1) { } else {
        //            switch (fld) {
        //                case "</%=dolPatientCurrent.ClientID%>":
        //                    jQuery('#</%=dolPatientPastDue.ClientID%>').focus();
        //                    break;
        //                case "</%=dolPatientPastDue.ClientID%>":
        //                    jQuery('#</%=dolPatientNext.ClientID%>').focus();
        //                    break;
        //                case "</%=dolPatientNext.ClientID%>":
        //                    jQuery('#</%=dolPatientBalance.ClientID%>').focus();
        //                    break;
        //                case "</%=dolPatientBalance.ClientID%>":
        //                    jQuery('#</%=btnAdd.ClientID%>').focus();
        //                    break;
        //            }
        //        };
        //    }
        // }

        function autoApplyPayment(pmtAmount, mode) {
            pmtAmount = parseFloat(pmtAmount.replace(',', '')).toFixed(2);
            if (isNaN(pmtAmount)) {
                pmtAmount = 0.00;
            };
            pmtRemaining = pmtAmount;
            strPmtFrom = jQuery('#<%=ddlPaymentFrom.ClientID%>').val();
            if (strPmtFrom.indexOf('Patient') == 0) {
                // pmt from patient
                if (isNaN(parseFloat(jQuery('#<%=dolPatientCurrent.ClientID%>').val()))) {
                    decCurVal = 0.00
                }
                else {
                    decCurVal = parseFloat(jQuery('#<%=dolPatientCurrent.ClientID%>').val(), 2)
                };
                if (isNaN(parseFloat(jQuery('#<%=dolPatientPastDue.ClientID%>').val()))) {
                    decPastDueVal = 0.00
                }
                else {
                    decPastDueVal = parseFloat(jQuery('#<%=dolPatientPastDue.ClientID%>').val(), 2)
                };
                if (isNaN(parseFloat(jQuery('#<%=dolPatientNext.ClientID%>').val()))) {
                    decNextVal = 0.00
                }
                else {
                    decNextVal = parseFloat(jQuery('#<%=dolPatientNext.ClientID%>').val(), 2)
                };
                if (isNaN(parseFloat(jQuery('#<%=dolPatientBalance.ClientID%>').val()))) {
                    decBalanceVal = 0.00
                }
                else {
                    decBalanceVal = parseFloat(jQuery('#<%=dolPatientBalance.ClientID%>').val(), 2)
                };
                blnAutoApply = true
                if (decCurVal > 0 || decPastDueVal > 0 || decNextVal > 0 || decBalanceVal > 0) {
                    if (mode == 'load') {
                        blnAutoApply = false;
                    } else {
                        blnAutoApply = confirm('Automatically Re-Distribute the Payment?');
                    };
                    if (blnAutoApply == true) {
                        clearApplyFields('patient');
                    } else {
                        ttlApplied = 0;
                        ttlApplied = decCurVal + decPastDueVal + decNextVal + decBalanceVal;
                        if (ttlApplied > pmtAmount) {
                            alert('Payments must be Automatically Restributed.\nCurrent applied amount of $' + ttlApplied + ' is more than the current payment amount of $' + pmtAmount + '.');
                            clearApplyFields('patient');
                            blnAutoApply = true
                        } else {
                            pmtRemaining = pmtAmount - ttlApplied;
                        }
                    }
                }
                if (blnAutoApply) {
                    // first try to apply to current invoice if there is one.
                    itmAmt = jQuery('#<%=hidPatientCurrent.ClientID%>').val().replace(',', '').replace('$', '');
                    itmPend = jQuery('#<%=hidPatientCurrentPend.ClientID%>').val().replace(',', '').replace('$', '');
                    curMax = +itmAmt - +itmPend;
                    if (+curMax > 0) {
                        if (+pmtRemaining >= +curMax) {
                            jQuery('#<%=dolPatientCurrent.ClientID%>').val(curMax);
                            pmtRemaining = pmtRemaining - curMax;
                        } else {
                            jQuery('#<%=dolPatientCurrent.ClientID%>').val(pmtRemaining);
                            pmtRemaining = 0;
                        }
                    }
                    // second apply to past due
                    if (+pmtRemaining > 0) {
                        itmAmt = jQuery('#<%=hidPatientPastDue.ClientID%>').val().replace(',', '').replace('$', '');
                        itmPend = jQuery('#<%=hidPatientPastDuePend.ClientID%>').val().replace(',', '').replace('$', '');
                        curMax = +itmAmt - +itmPend;
                        if (+curMax > 0) {
                            if (+pmtRemaining >= +curMax) {
                                jQuery('#<%=dolPatientPastDue.ClientID%>').val(curMax);
                                pmtRemaining = pmtRemaining - curMax;
                            } else {
                                jQuery('#<%=dolPatientPastDue.ClientID%>').val(pmtRemaining);
                                pmtRemaining = 0;
                            }
                        }
                    }

                    //2.22.17 change next iv back to 3rd, and balance last.
                    // 3rd apply to next inv
                    if (+pmtRemaining > 0) {

                        itmPmt = pmtRemaining;
                        jQuery('#<%=dolPatientNext.ClientID%>').val(pmtRemaining);
                        pmtRemaining = 0;
                        itmAmt = jQuery('#<%=hidPatientNext.ClientID%>').val().replace(',', '').replace('$', '');
                        itmPend = jQuery('#<%=hidPatientNextPend.ClientID%>').val().replace(',', '').replace('$', '');

                        itmAmt = jQuery('#<%=hidPatientNext.ClientID%>').val().replace(',', '').replace('$', '');
                        itmPend = jQuery('#<%=hidPatientNextPend.ClientID%>').val().replace(',', '').replace('$', '');
                        curMax = +itmAmt - +itmPend;
                        if (+curMax > 0) {
                            if (+pmtRemaining >= +curMax) {
                                jQuery('#<%=dolPatientNext.ClientID%>').val(curMax);
                                pmtRemaining = pmtRemaining - curMax;
                            } else {
                                jQuery('#<%=dolPatientNext.ClientID%>').val(pmtRemaining);
                                pmtRemaining = 0;
                            }
                        }
                    }

                    // last apply to contract balance
                    // 2.22.17 commented this out reversed next inv and balance to move balance back to last
                    <%--if (+pmtRemaining > 0) {
                        itmAmt = jQuery('#<%=hidPatientBalance.ClientID%>').val().replace(',', '').replace('$', '');
                        itmPend = jQuery('#<%=hidPatientBalancePend.ClientID%>').val().replace(',', '').replace('$', '');
                        curMax = +itmAmt - +itmPend;
                        if (+curMax > 0) {
                            if (+pmtRemaining >= +curMax) {
                                jQuery('#<%=dolPatientBalance.ClientID%>').val(curMax);
                                pmtRemaining = pmtRemaining - curMax;
                            } else {
                                jQuery('#<%=dolPatientBalance.ClientID%>').val(pmtRemaining);
                                pmtRemaining = 0;
                            }
                        }
                    };--%>

                    // last apply to contract balance
                    if (+pmtRemaining > 0) {
                        itmPmt = pmtRemaining;
                        jQuery('#<%=dolPatientBalance.ClientID%>').val(pmtRemaining);
                        pmtRemaining = 0;
                        itmAmt = jQuery('#<%=hidPatientBalance.ClientID%>').val().replace(',', '').replace('$', '');
                        itmPend = jQuery('#<%=hidPatientBalancePend.ClientID%>').val().replace(',', '').replace('$', '');
                        overPayment =  +itmPend + +itmPmt - +itmAmt;
                        if (+overPayment > 0) {
                            alert('Notice: Patient has an overpayment of $' + overPayment + ' to the Balance.')
                        }
                    };
                }
            } else {
                // pmt from insurance -- no auto apply -- jsut put amount into remaining          
            };
            // update remaining to apply on the screen
            calculateTotalPayment();
        }

        function clearApplyFields(clearType) {
            if (clearType == 'patient') {
                jQuery('#<%=dolPatientCurrent.ClientID%>').val(0);
                jQuery('#<%=dolPatientPastDue.ClientID%>').val(0);
                jQuery('#<%=dolPatientNext.ClientID%>').val(0);
                jQuery('#<%=dolPatientBalance.ClientID%>').val(0);
            } else {
                insCount = parseInt(jQuery('#<%=hidInsCount.ClientID%>').val());
                 insLoop = 0;
                 while (insLoop <= insCount) {
                     jQuery('#dolInsPayment' + insLoop).val(0.00);
                     insLoop += 1;
                 };
            }
        }

        function calculateTotalPayment() {
            pmtAmount = parseFloat(jQuery('#<%=dolPaymentAmount.ClientID%>').val().replace(',', '')).toFixed(2);
            if (pmtAmount == 'NaN') { pmtAmount = 0.00 };
            paymentFrom = jQuery('#<%=ddlPaymentFrom.ClientID%>').val();
            if (paymentFrom == 'Patient') {
                pmtCurrent = parseFloat(jQuery('#<%=dolPatientCurrent.ClientID%>').val().replace(',', '')).toFixed(2);
                if (pmtCurrent == 'NaN') { pmtCurrent = 0.00 };
                pmtPastDue = parseFloat(jQuery('#<%=dolPatientPastDue.ClientID%>').val().replace(',', '')).toFixed(2);
                if (pmtPastDue == 'NaN') { pmtPastDue = 0.00 };
                pmtNext = parseFloat(jQuery('#<%=dolPatientNext.ClientID%>').val().replace(',', '')).toFixed(2);
                if (pmtNext == 'NaN') { pmtNext = 0.00 };
                pmtBalance = parseFloat(jQuery('#<%=dolPatientBalance.ClientID%>').val().replace(',', '')).toFixed(2);
                if (pmtBalance == 'NaN') { pmtBalance = 0.00 };
                totalPayment = +pmtCurrent + +pmtPastDue + +pmtNext + +pmtBalance;
            } else {
                totalPayment = 0;
                insCount = parseInt(jQuery('#<%=hidInsCount.ClientID%>').val());
                insLoop = 0;
                while (insLoop <= insCount) {
                    if (jQuery('#dolInsPayment' + insLoop).length > 0) {
                        insPayment = parseFloat(jQuery('#dolInsPayment' + insLoop).val().replace(',', '')).toFixed(2);
                        if (isNaN(insPayment)) { insPayment = 0.00 };
                        totalPayment += +insPayment;
                    }
                    insLoop += 1;
                };
            };
            remainingBalance = parseFloat(+pmtAmount - +totalPayment).toFixed(2);
            jQuery('#<%=dolApplyRemaining.ClientID%>').val(remainingBalance);
            if (remainingBalance < 0) {
                jQuery('#lblPaymentMessage').html('ERROR: Payment has been over-applied');
                jQuery('#divApplyBalance').addClass('has-error');
            } else {
                jQuery('#lblPaymentMessage').html('');
                jQuery('#divApplyBalance').removeClass('has-error');
            }
        }

        function checkFieldPaymentAmount(fld, val, max) {
            applyPaymentMessage = '';
            val = parseFloat(val.replace(',', '')).toFixed(2);
            if (val == 'NaN') { val = 0.00; jQuery('#' + fld).val(parseFloat(val).toFixed(2)) };
            if (+val < 0) { val = 0.00 }; // this should never happen, but just in case.
            if (max == 'hid') {
                hiddenMaxField = fld.replace('dol', 'hid');
                max = parseFloat(jQuery('#' + hiddenMaxField).val().replace(',', '').replace('$', '')).toFixed(2);
            };            
            if (val == 0) {
                updateInsArray(fld, val);
            } else {
                if (+val > +max) {
                    blnOverPayOk = false;
                    if (fld.indexOf('dolInsPayment') == 0) {
                        idx = fld.replace('dolInsPayment', '');
                        arrInsList = jQuery('#<%=hidInsRefList.ClientID%>').val().split('||');
                        arrInsEntry = arrInsList[idx].split('~~');
                        if (arrInsEntry[2] == 'PrimaryWait' || arrInsEntry[2] == 'SecondaryWait') {
                            overPay = +val - +max;
                            overPay = parseFloat(overPay).toFixed(2);
                            applyPaymentMessage += 'Warning: Overpayment of $' + overPay + ' to Waiting on Claim.  This transaction will create a Credit on Account.';
                            blnOverPayOk = true;
                        }
                    };
                    // 01.31.17 cpb changed to check patient next invoice instead of patient balance
                    if (fld == '<%=dolPatientNext.ClientID%>') {
                        overPay = +val - +max;
                        applyPaymentMessage += 'Warning: Overpayment of $' + overPay + ' to Patient Next Invoice.  This transaction will create a Credit on Account.';
                        blnOverPayOk = true;
                    };
                    if (blnOverPayOk) {
                    } else {
                        applyPaymentMessage += 'Max value is: $' + max;
                        val = max;
                        jQuery('#' + fld).select();
                        jQuery('#' + fld).focus();
                    };
                };
                updateInsArray(fld, val);
            };
            if (applyPaymentMessage != '') {
                alert(applyPaymentMessage)
            };
        }

        function checkNullZero(checkAmount) {
            checkAmount = parseFloat(checkAmount);
            if (isNaN(checkAmount)) {
                return 'nullZero';
            } else {
                if (checkAmount == 0) {
                    return 'nullZero';
                } else {
                    if (checkAmount < 0) {
                        return 'lessZero';
                    } else {
                        return true;
                    }
                };
            }
        }

        function applyInsAmt(id, val) {
            // this function is called from links created in vb--you will not see direct link in this code.
            // verify there is enough in remaining and if so, apply the ins amt
           remaining = parseFloat(jQuery('#<%=dolApplyRemaining.ClientID%>').val().replace(',', '')).toFixed(2);
           val = parseFloat(val.replace(',', '')).toFixed(2);
           if (isNaN(remaining)) {remaining = 0.00};
           if (isNaN(val)) { val = 0.00 };
           if (+remaining <= 0) {
               alert('Sorry, there is no amount remaining to apply.');
           } else {
               if (+val > +remaining) {
                   alert('The full amount can not be applied.\nThe remaining amount of ' + remaining + ' will be applied.');
                   val = remaining;
               };
               jQuery('#' + id).val(val);
               updateInsArray(id,val);
               calculateTotalPayment();
           }
       }

        function loadInsArrayPaymentEdit() {
            arrInsList = jQuery('#<%=hidInsRefList.ClientID%>').val().split('||');
            for (insIdx = 0; insIdx < arrInsList.length; insIdx++) {
                arrInsEntry = arrInsList[insIdx].split('~~');
                editPmtAmount = arrInsEntry[5];
                if (editPmtAmount == 0) { } else {
                    jQuery('#dolInsPayment' + insIdx).val(editPmtAmount);
                }
            }
        }

        function updateInsArray(fld, val) {
            if (fld.indexOf('dolInsPayment') == 0) {
                idx = fld.replace('dolInsPayment', '');
                strUpdatedInsList = '';
                strUpdatedInsListDelim = '';
                arrInsList = jQuery('#<%=hidInsRefList.ClientID%>').val().split('||');
                    for (insIdx = 0; insIdx < arrInsList.length; insIdx++) {
                        if (idx == insIdx) {
                            arrInsEntry = arrInsList[insIdx].split('~~');
                            arrInsEntry[4] = val;
                            strUpdatedEntry = '';
                            strUpdatedEntryDelim = '';
                            for (entryIdx = 0; entryIdx < arrInsEntry.length; entryIdx++) {
                                strUpdatedEntry += strUpdatedEntryDelim + arrInsEntry[entryIdx];
                                strUpdatedEntryDelim = '~~';
                            };
                        } else {
                            strUpdatedEntry = arrInsList[insIdx];
                        };
                        strUpdatedInsList += strUpdatedInsListDelim + strUpdatedEntry;
                        strUpdatedInsListDelim = "||";
                    };
                    jQuery('#<%=hidInsRefList.ClientID%>').val(strUpdatedInsList);
                };
        }

        function deletePayment() {
            blnDeletePayment = confirm("Are you sure you want to delete this payment?")
            if (blnDeletePayment == false) {
                jQuery('#divWait').hide();
                return false;
            }
        }

        function addToQueue() {
            alertMsg = '';
            // verify required fields are completed
            if (jQuery('#<%=txtPayerName.ClientID%>').val() == '') {
                alertMsg += 'Please enter a Payer Name.\n';
            };
            strPmtFrom = jQuery('#<%=ddlPaymentFrom.ClientID%>').val();
            if (strPmtFrom.indexOf('Patient') == 0) {
                jQuery('#<%=ddlPaymentType.ClientID%>').val(jQuery('#<%=ddlPatPaymentType.ClientID%>').val())
                if (jQuery('#<%=ddlPatPaymentType.ClientID%>').val() == '-1') {
                    alertMsg += 'Please select a Payment Type.\n';
                };
            } else {
                jQuery('#<%=ddlPaymentType.ClientID%>').val(jQuery('#<%=ddlInsPaymentType.ClientID%>').val())
                if (jQuery('#<%=ddlInsPaymentType.ClientID%>').val() == '-1') {
                    alertMsg += 'Please select a Payment Type.\n';
                };
            }
            pmtAmount = checkNullZero(jQuery('#<%=dolPaymentAmount.ClientID%>').val());
            if (pmtAmount == 'nullZero') {
                alertMsg += 'Please enter a Payment Amount.\n'
            };
            // verify payment distributed correctly
            remaining = checkNullZero(jQuery('#<%=dolApplyRemaining.ClientID%>').val());
            if (remaining == 'nullZero') {} else {
                alertMsg += 'Allocation amount(s) do not match payment amount.\n';
            };
            if (alertMsg == '') {
                // payment looks good add to payments queue
                pmtAmount = parseFloat(jQuery('#<%=dolPaymentAmount.ClientID%>').val().replace(',', '')).toFixed(2);
                if (pmtAmount == 'NaN') { pmtAmount = 0.00 };
                jQuery('#<%=dolOrig_Payment.ClientID%>').val(pmtAmount);
                strPmtFrom = jQuery('#<%=ddlPaymentFrom.ClientID%>').val();
                if (strPmtFrom.indexOf('Patient') == 0) {
                    jQuery('#<%=dolPrimaryAmount.ClientID%>').val(0.00);
                    jQuery('#<%=dolSecondaryAmount.ClientID%>').val(0.00);
                    // patient amount
                    jQuery('#<%=dolPatientAmount.ClientID%>').val(pmtAmount);
                    // current
                    pmtCurrent = parseFloat(jQuery('#<%=dolPatientCurrent.ClientID%>').val().replace(',', '')).toFixed(2);
                    if (pmtCurrent == 'NaN') { pmtCurrent = 0.00 };
                    jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').val(pmtCurrent);
                    // past due
                    pmtPastDue = parseFloat(jQuery('#<%=dolPatientPastDue.ClientID%>').val().replace(',', '')).toFixed(2);
                    if (pmtPastDue == 'NaN') { pmtPastDue = 0.00 };
                    jQuery('#<%=dolApplyToPastDue.ClientID%>').val(pmtPastDue);
                    // next
                    pmtNext = parseFloat(jQuery('#<%=dolPatientNext.ClientID%>').val().replace(',', '')).toFixed(2);
                    if (pmtNext == 'NaN') { pmtNext = 0.00 };
                    jQuery('#<%=dolApplyToNextInvoice.ClientID%>').val(pmtNext);
                    // balance
                    pmtBalance = parseFloat(jQuery('#<%=dolPatientBalance.ClientID%>').val().replace(',', '')).toFixed(2);
                    if (pmtBalance == 'NaN') { pmtBalance = 0.00 };
                    jQuery('#<%=dolApplyToPrinciple.ClientID%>').val(pmtBalance);
                } else {
                    if (strPmtFrom.indexOf('Secondary') == 0 ) {
                        jQuery('#<%=dolPrimaryAmount.ClientID%>').val(0.00);
                        jQuery('#<%=dolSecondaryAmount.ClientID%>').val(pmtAmount);
                        jQuery('#<%=dolPatientAmount.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToPastDue.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToPrinciple.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToNextInvoice.ClientID%>').val(0.00);
                    } else {
                        // if not secondary must be primary or other -- so other must post into primary amount
                        jQuery('#<%=dolPrimaryAmount.ClientID%>').val(pmtAmount);
                        jQuery('#<%=dolSecondaryAmount.ClientID%>').val(0.00);
                        jQuery('#<%=dolPatientAmount.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToPastDue.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToPrinciple.ClientID%>').val(0.00);
                        jQuery('#<%=dolApplyToNextInvoice.ClientID%>').val(0.00);
                    }
                };
                return true;
            } else {
                jQuery('#divWait').hide();
                alert(alertMsg); return false;
            }
        };

        function confirmCancel(mode) {
            if (mode == 'edit') {
                msg = 'Edits will not be Saved!';
            } else {
                msg = 'Payemnt Data will not be Saved!'
            }
            blnCancel = confirm(msg);
            if (blnCancel) {
                clearApplyFields();
                return true;
            } else {
                return false;
            }
        };

        function showEditOptions() {
            jQuery('#<%=divViewOptions.ClientID%>').addClass("hidden");
            jQuery('#<%=divBtnEditOptions.ClientID%>').removeClass("hidden");
            jQuery('#<%=divBtnViewOptions.ClientID%>').addClass('hidden');
            jQuery('#<%=ddlPaymentFrom.ClientID%>').prop('disabled', false);
            jQuery('#<%=txtPaymentReference.ClientID%>').prop('disabled', false);
            strPmtFrom = jQuery('#<%=ddlPaymentFrom.ClientID%>').val();
            if (strPmtFrom.indexOf('Patient') == 0) {
                jQuery('#<%=hidPatientBalance.ClientID%>').val(newMax);
                jQuery('#<%=dolPaymentAmount.ClientID%>').prop('disabled', false);
                jQuery('#<%=ddlPatPaymentType.ClientID%>').prop('disabled', false);
                enableDisableDollarsByBalance();
                jQuery('#<%=ddlPaymentFor.ClientID%>').prop('disabled', false);
                jQuery('#<%=txtComments.ClientID%>').prop('disabled', false);
                jQuery('#<%=dolPaymentAmount.ClientID%>').focus();
            }
        }

        function postData() {
            blnPostPayments = confirm("Are you sure you want to post all payments in the Payments Queue?")
            if (blnPostPayments == false) { }
            else {
                Tbl = 'PaymentsTemp';
                //get chart number 08/27/14 CP
                jQuery.post('ajaxOrtho.aspx', { action: 'postPending', tb: Tbl },
                        function () {
                            document.getElementById('frmPayments').submit()
                        });
            };
            
        }

    </script>

</asp:Content>
