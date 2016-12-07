<%@ Page Title="" EnableEventValidation="false" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="PaymentPosting.aspx.vb" Inherits="WSDC_Ortho.PaymentPosting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPayments' class="form-horizontal" role="form" defaultbutton="btnHidden" action="PaymentPosting.aspx" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" cssclass="hidden" OnClientClick="return false" runat="server" />

        <asp:HiddenField ID="txtSessions" runat="server" />
        <asp:TextBox ID="txtPaymentID" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtContract_RECID" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtPaymentSelection" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
        <asp:TextBox ID="dolOrig_Payment" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
        <asp:TextBox ID="intCurInvoice" CssClass="hidden" runat="server"></asp:TextBox>
        <asp:Literal ID="litViewOnly" runat="server"></asp:Literal>
        <%--11/29/2016 Added hidden textbox to store doctors_vw (recid) for payment data--%>
        <asp:TextBox ID="txtDoctors_vw" CssClass="DB form-control hidden" runat="server"></asp:TextBox>

        <%--<div class="container">--%>
            <asp:Literal ID="litHeader" runat="server"></asp:Literal>

            <div id="divViewOptions" class="col-sm-12" style="z-index: 1000;">
                <div class="text-center">
                    <span class="glyphicon glyphicon-eye-open"></span>View Only Mode <a href="#" onclick="showEditOptions()">Edit Payment (Click Here)</a>
                </div>
            </div>
            <!--View only end-->
            <div class="col-sm-12">
                <h4 class="text-center"><span style="color: #2d6ca2;">
                    <asp:Label ID="lblPatientName" runat="server"></asp:Label></span></h4>
                <h6 class="text-center"><span style="color: #2d6ca2;">
                    <asp:Label ID="lblInsurance" runat="server"></asp:Label></span></h6>
            </div>
            <div class="col-sm-12" style="color: #2d6ca2;">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">
                        <label for="dtePaymentDate" class="col-xs-12 col-sm-3">
                            Payment Date:
                            <div id="divdtePaymentDate" class="input-group input-append " >
                                <span class="input-group-addon ">
                                    <span class="glyphicon glyphicon-calendar"></span>
                                </span>
                                <asp:TextBox ID="dtePaymentDate" name="dtePaymentDate" CssClass="DB form-control datePicker" onBlur="DateBlur(this);"  placeholder="mm/dd/yyyy" runat="server"></asp:TextBox>                                                       
                            </div>
                        </label>                             
                        <label for="txtFirstName" class="col-xs-3 col-sm-3">
                            First Name:
                            <asp:TextBox ID="txtFirstName" name="txtFirstName" CssClass="DB form-control" onChange="getPatName('fst')" runat="server"></asp:TextBox>
                            <%--                            <asp:RequiredFieldValidator ID="rfvFirstName" InitialValue="" ControlToValidate="txtFirstName" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />--%>
                        </label>
                        <label for="txtLastName" class="col-xs-3 col-sm-3">
                            Last Name:
                            <asp:TextBox ID="txtLastName" name="txtLastName" CssClass="DB form-control" onChange="getPatName('lst')" runat="server"></asp:TextBox>
                            <%--                            <asp:RequiredFieldValidator ID="rfvLastName" InitialValue="" ControlToValidate="txtLastName" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />--%>
                        </label>
                        <label for="txtPatientNumber" class="hidden">
                            Patient #:
                            <asp:TextBox ID="txtPatientNumber" name="txtPatientNumber" CssClass="DB form-control" runat="server"></asp:TextBox>
<%--                            <asp:RequiredFieldValidator ID="rfvContractNumber" InitialValue="" ControlToValidate="txtPatientNumber" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />
                            <asp:RegularExpressionValidator ID="revConNum" ErrorMessage="Please enter a valid amount." ControlToValidate="txtPatientNumber" ValidationExpression="^\d+$" CssClass="requiredFieldValidator" runat="server" />--%>
                        </label>
                        <label for="txtChartNumber" class="col-xs-3">
                            Chart #:
                            <asp:TextBox ID="txtChartNumber" name="txtChartNumber" CssClass="DB form-control" onChange="getPatName('cht')" runat="server"></asp:TextBox>
<%--                            <asp:RequiredFieldValidator ID="rfvChartNumber" InitialValue="" ControlToValidate="txtChartNumber" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />
                            <asp:RegularExpressionValidator ID="revChtNum" ErrorMessage="Please enter a valid amount." ControlToValidate="txtChartNumber" ValidationExpression="^\d+$" CssClass="requiredFieldValidator" runat="server" />--%>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <!-- Amount Radiobuttons -->
                <div class="form-group">
                    <label class="col-sm-1 control-label" for="applyToRadiobuttons">Amount($):</label>
                    <div class="col-sm-9">
                        <label for="dolPatientAmount" class="col-xs-3 col-sm-3">
                            <input type="radio" name="paymentRadiobuttons" id="rdoPatAmt" checked="checked" onclick="rdoChange('patAmt'); enableValidator('rfvPatientAmount');" value="patAmt" />
                            Patient Amount:
                                     <span class="input-group">
                                         <span class="input-group-addon">$</span>
                                         <asp:TextBox ID="dolPatientAmount" name="dolPatientAmount" type="text" CssClass="DB form-control" onBlur="checkNullorZero('dolPatientAmount');" required="required" runat="server"></asp:TextBox>
                                     </span>
<%--                            <asp:RequiredFieldValidator ID="rfvPatientAmount" ControlToValidate="dolPatientAmount" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />
                            <asp:RegularExpressionValidator ID="revPatAmount" ErrorMessage="Please enter a valid amount." ControlToValidate="dolPatientAmount" ValidationExpression="^(-)?\d+(\.\d\d)?$" CssClass="requiredFieldValidator" runat="server" />--%>
                        </label>
                        &nbsp;&nbsp;
                             <label for="dolPrimaryAmount" class="col-xs-3 col-sm-3">
                                 <input type="radio" id="rdoPrimAmt" name="paymentRadiobuttons" onclick="rdoChange('primAmt'); enableValidator('rfvPrimaryAmount');" value="primAmt" />
                                 Primary Amount: 
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolPrimaryAmount" name="dolPrimaryAmount" type="text" CssClass="DB form-control" onBlur="checkNullorZero('dolPrimaryAmount');" runat="server"></asp:TextBox>
                                    </span>
<%--                                 <asp:RequiredFieldValidator ID="rfvPrimaryAmount" ControlToValidate="dolPrimaryAmount" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />
                                 <asp:RegularExpressionValidator ID="revPrimAmount" ErrorMessage="Please enter a valid amount." ControlToValidate="dolPrimaryAmount" ValidationExpression="^(-)?\d+(\.\d\d)?$" CssClass="requiredFieldValidator" runat="server" />--%>
                             </label>
                        <label for="dolSecondaryAmount" class="col-xs-3 col-sm-3">
                            <input type="radio" name="paymentRadiobuttons" id="rdoSecAmt" onclick="rdoChange('secAmt'); enableValidator('rfvSecondaryAmount');" value="secAmt" />
                            Secondary Amount: 
                                    <span class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox ID="dolSecondaryAmount"  name="dolSecondaryAmount"  type="text" CssClass="DB form-control" onBlur="checkNullorZero('dolSecondaryAmount');" runat="server"></asp:TextBox>
                                    </span>
<%--                            <asp:RequiredFieldValidator ID="rfvSecondaryAmount" ControlToValidate="dolSecondaryAmount" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />
                            <asp:RegularExpressionValidator ID="revSecAmount" ErrorMessage="Please enter a valid amount." ControlToValidate="dolSecondaryAmount" ValidationExpression="^(-)?\d+(\.\d\d)?$" CssClass="requiredFieldValidator" runat="server" />--%>
                        </label>
                    </div>
                </div>
            </div>
            <div id="divPaymentOptions1" class="col-sm-12">
                <!-- Apply to Checkboxes -->
                <div class="form-group">
                    <label class="col-sm-1 control-label" for="applyToCheckboxes">Apply to:</label>
                    <div class="col-sm-9">
                        <label for="applyToCheckboxes-0" class="col-xs-3 col-sm-3">
                            Current Invoice  <span style="color: green">
                                <asp:Label ID="lblCurrentInvoiceBal" runat="server"></asp:Label></span><asp:TextBox ID="intCurrentInvoiceBal" cssclass="hidden" runat="server" />
                            <span class="input-group">
                                <span class="input-group-addon">$</span>
                                <asp:TextBox ID="dolApplyToCurrentInvoice" name="dolApplyToCurrentInvoice" value="0" type="text" CssClass="DB form-control" onkeyup="updateRemaining();checkOverPayment()" onBlur="checkNullorZero('dolApplyToInvoice');" Display="Dynamic" EnableClientScript="True" runat="server"></asp:TextBox>
                            </span>
                        </label>
                        &nbsp;&nbsp;
                            <label id="lblPastDueDiv" for="applyToCheckboxes-1" class="col-xs-3 col-sm-3">
                                Prior Invoices Balance  <span style="color: red">
                                    <asp:Label ID="lblPastDueBal" runat="server"></asp:Label></span><asp:TextBox ID="intPastDueBal" cssclass="hidden" runat="server" />
                                <span class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="dolApplyToPastDue" name="dolApplyToPastDue" value="0" type="text" onkeyup="updateRemaining()" onBlur="checkNullorZero('dolApplyToPastDue');" CssClass="DB form-control" runat="server"></asp:TextBox>
                                </span>
                            </label>
                    </div>
                </div>
            </div>
            <div id="divPaymentOptions2" class="col-xs-12 col-sm-12">
                <!-- Apply To Next Invoice and remaining(hidden) -->
                <div class="form-group">
                    <label class="col-sm-1 control-label" for="applyToCheckboxes2">
                        Remaining
                        <span style="color: green">$<asp:Label ID="lblRemainingAmt" runat="server" /></span></label>:
                        <div class="col-sm-9">
                            <label for="applyToCheckboxes2-0" class="col-xs-3 col-sm-3">
                                Patient Remaining  <span style="color: green">
                                    <asp:Label ID="lblPatRemaniningBal" runat="server"></asp:Label></span><asp:TextBox ID="intPatRemaniningBal" cssclass="hidden" runat="server" />
                                <span class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="dolApplyToPrinciple" name="dolApplyToPrinciple" value="0" type="text" CssClass="DB form-control"  onBlur="checkNullorZero('dolApplyToPrinciple');"  onkeyup="updateRemaining()" runat="server"></asp:TextBox>
                                </span>
                            </label>
                            &nbsp;&nbsp;
                            <label for="applyToCheckboxes2-1" class="col-xs-3 col-sm-3">
                                Next Invoice 
                                <span class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="dolApplyToNextInvoice" name="dolApplyToNextInvoice" value="0" type="text" CssClass="DB form-control"  onBlur="checkNullorZero('dolApplyToNextInvoice');"  onkeyup="updateRemaining()" runat="server"></asp:TextBox>
                                </span>
                            </label>
                        </div>
                </div>
            </div>
            <div id="divPaymentInfo" class="col-xs-12 col-sm-12">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">
                        <div id="divPayerName" class="col-xs-12 col-sm-3">
                            <label for="txtPayerName">
                                Payer Name:
                                <asp:TextBox ID="txtPayerName" name="txtPayerName" CssClass="DB form-control" ToolTip="" runat="server"></asp:TextBox>
<%--                                <asp:RequiredFieldValidator ID="rfvPayer" InitialValue="" ControlToValidate="txtPayerName" ErrorMessage="Required" ValidationGroup="valGroup1" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />--%>
                            </label>
                        </div>
                        <div id="divClaimNumber" class="col-xs-12 col-sm-3">
                            <label for="ddlPaidByInsurance">
                                Service Date:
                            <asp:DropDownList ID="ddlClaimNumber" name="ddlClaimNumber" CssClass=" form-control  PromptSelection" runat="server"></asp:DropDownList>
<%--                            <asp:RequiredFieldValidator ID="rfvClaimNumber" ControlToValidate="ddlClaimNumber" InitialValue="-2" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" runat="server" />--%>
                            </label>
                            <asp:TextBox ID="txtClaimNumber" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
                            <asp:TextBox ID="txtClaimIndex" CssClass="DB form-control hidden" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-xs-12 col-sm-3">
                            <label for="ddlPaymentType">
                                Payment Type:
                            <asp:DropDownList ID="ddlPaymentType" name="ddlPaymentType" CssClass="DB  form-control  PromptSelection" runat="server"></asp:DropDownList>
<%--                                <asp:RequiredFieldValidator ID="rfvPaymentType" InitialValue="-1" ControlToValidate="ddlPaymentType" ValidationGroup="valGroup1" ErrorMessage="Required" ForeColor="Red" runat="server" />--%>
                            </label>
                        </div>
                        <div class="col-xs-12 col-sm-3">
                            <label for="txtPaymentReference">
                                Check #:
                                <asp:TextBox ID="txtPaymentReference" name="txtPaymentReference" CssClass="DB form-control" ToolTip="" runat="server"></asp:TextBox>
<%--                                <asp:RequiredFieldValidator ID="rfvPayRef" InitialValue="" ControlToValidate="txtPaymentReference" ValidationGroup="valGrpPayment" ErrorMessage="Required" ForeColor="Red" Display="Dynamic" EnableClientScript="True" runat="server" />--%>
                            </label>
                        </div>                      
                    </div>
                </div>
            </div>
            <div id="divPaymentFor" class="col-xs-12 col-sm-12">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">
                        <div id="divDDLPaymentFor" class="col-xs-12 col-sm-9">
                            <label for="ddlPaymentFor">
                                Payment For:
                                <asp:DropDownList ID="ddlPaymentFor" name="ddlPaymentFor" CssClass="DB  form-control  PromptSelection" runat="server"></asp:DropDownList>
                                <%--<asp:RequiredFieldValidator ID="rfvPaymentFor" InitialValue="-1" ControlToValidate="ddlPaymentFor" ValidationGroup="valGrpPaymentFor" ErrorMessage="Required" ForeColor="Red" runat="server" />--%>
                            </label>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divPaymentComments" class="col-xs-12 col-sm-12">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">
                        <div id="divComments" class="col-xs-12 col-sm-9">
                            <label for="txtComments">
                                Comments:
                                <asp:TextBox ID="txtComments" name="txtComments" CssClass="DB form-control" runat="server" TextMode="MultiLine" Rows="2" Columns="50"></asp:TextBox>
                            </label>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divPaymentButtons" class="col-xs-12 col-sm-12">
                <div class="form-group">
                    <div class="col-sm-offset-1 col-sm-9">
                        <%--<label class="col-sm-1 control-label" for="chkKeepFrmValues"></label>
                        <div class="col-xs-12 col-sm-3">
                            <label for="chkKeepFrmValues">
                                <asp:CheckBox ID="chkKeepFrmValues" Text="&nbsp;Keep form values after add" runat="server" />
                            </label>
                        </div>--%>
                        <div id="divButtons" class="col-xs-12 col-sm-9">
                            <div id="divBtnAdd">
                                <asp:Button ID="btnAdd" cssClass="btn btn-large btn-success" OnClientClick="if (checkAmounts() == true) {  } else { if (checkAmounts() == 'nullZero') { alert('Please enter a payment amount'); return false; } else { alert('Allocation amount(s) do not match payment amount.'); return false; } }" ValidationGroup="valGroup1" runat="server" Text="Add to Queue" />
                            </div>
                            <span id="divEditOptions" class="hidden">
                                <asp:Button ID="btnSave" cssclass="btn btn-large btn-warning " OnClientClick="if (checkAmounts() == true) {  } else { if (checkAmounts() == 'nullZero') { alert('Please enter a payment amount'); return false; } else { alert('Allocation amount(s) do not match payment amount.'); return false; } }; saveChanges();saveData(false);" runat="server" Text="Save Changes" />
                                <asp:Button ID="btnCancel" cssclass="btn btn-large btn-primary" OnClientClick="" runat="server" Text="Cancel Edit" />
                                <asp:Button ID="btnDelete" cssclass="btn btn-large btn-danger" OnClientClick="return confirm('Are you sure you want to delete this payment?')" runat="server" Text="Delete" />
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group col-xs-6 col-sm-6">
                    <h4>Payments Queue</h4>
                </div>
            </div>
            <!--page body-->
            <asp:Literal ID="litNoData" runat="server"></asp:Literal>
            <!--iframe Begin-->
            <div id="divIframe">
                <iframe id="ifmPayment" src="frmListManager.aspx?id=PaymentsTemp_vw&usr=True&divHide=divHeader,divPaginationHeading,divPagination,navPage,divFooter" style="border: 0px; min-height: 250px; z-index: 1000; position: relative;"></iframe>
            </div>
            <!--iframe End-->
            <div class="col-sm-12">
                <div class="form-group col-sm-12">
                    <%--<div class="col-sm-5">
                        <h4>Payments Total:
                            <asp:Label ID="lblPaymentsTempTotal" runat="server"></asp:Label>
                        </h4>
                    </div>--%>
                    <%--                    <div class="col-sm-7">--%>
                    <asp:Button ID="btnPost" cssclass="btn btn-large btn-primary center-block" UseSubmitBehavior="false" Style="z-index: 1000; position: relative;" runat="server" Text="Post Payments" />
                    <%--                    </div>--%>
                </div>
            </div>
            <div class="col-sm-12"><br /><br /></div>
        <%--</div>--%>
        <!--Container end-->
    </form>

    <!--Modal Contract Selection Popup-->
    <div class="modal fade" id="divModalContractSelection" tabindex="-1" role="dialog" aria-labelledby="divModalContractSelectionLabel" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title"></h4>
                </div>
                <div class="modal-body modal-contract">
                </div>
                <div class="modal-footer">
                    <div class="btn-group">
                        <button type="button" class="btn btn-default" id="btnPopupCancel" data-dismiss="modal">Cancel</button>
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

        jQuery(document).ready(function () {

            //Highlights area on click to make data entry easier
            jQuery('#<%=dolPatientAmount.ClientID%>').focus(function () { this.select(); });
            jQuery('#<%=dolPrimaryAmount.ClientID%>').focus(function () { this.select(); });
            jQuery('#<%=dolSecondaryAmount.ClientID%>').focus(function () { this.select(); });
            jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').focus(function () { this.select(); });
            jQuery('#<%=dolApplyToNextInvoice.ClientID%>').focus(function () { this.select(); });
            jQuery('#<%=dolApplyToPastDue.ClientID%>').focus(function () { this.select(); });
            jQuery('#<%=dolApplyToPrinciple.ClientID%>').focus(function () { this.select(); });

            //Add focus to Chart Number
            jQuery('#<%=txtChartNumber.ClientID%>').focus();

            //Disable required field validators  for primary & secondary amount
            <%--jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.ClientIDMode%>', false,'numeric');
            jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.ClientIDMode%>', false,'numeric');--%>
            //ValidatorEnable(document.getElementById('rfvPrimaryAmount.ClientID%>'), false);
            //ValidatorEnable(document.getElementById('rfvSecondaryAmount.ClientID%>'), false);
            jQuery('#<%=dtePaymentDate.ClientID%>').prop('disabled', true);
            jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', true);
            jQuery('#<%=dolApplyToPastDue.ClientID%>').prop('readonly', true);
            jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop('readonly', true);
            jQuery('#<%=dolApplyToNextInvoice.ClientID%>').prop('readonly', true);
            document.getElementById('<%=lblRemainingAmt.ClientID%>').innerText = "0.00";

            //Claim/Patient Info validators
            <%--jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=txtPayerName.UniqueID%>', true,'notEmpty');--%>
            //document.getElementById('rfvPayer.ClientID%>').enabled = true;

            document.getElementById('divClaimNumber').style.display = "none";

            $('#frmPayments')
            .formValidation({
                framework: 'bootstrap',
                icon: {
                    valid: 'glyphicon glyphicon-ok',
                    invalid: 'glyphicon glyphicon-remove',
                    validating: 'glyphicon glyphicon-refresh'
                },
                fields: {
                    <%=txtChartNumber.UniqueID%>: {
                        row: '.col-xs-3',
                        validators: {
                            notEmpty: {
                                message: 'Chart number cannot be empty '
                            },
                             integer: {
                                message: 'The value is not an integer'
                                }
                          }
                    },
                    <%=dolPatientAmount.UniqueID%>: {
                        row: '.col-xs-3',
                        validators: {
                            numeric: {
                                message: 'The value is not a number',
                                // The default separators
                                thousandsSeparator: '',
                                decimalSeparator: '.'
                            }
                          }
                    },
                    <%=dolPrimaryAmount.UniqueID%>: {
                        row: '.col-xs-3',
                        validators: {
                            numeric: {
                                message: 'The value is not a number',
                                // The default separators
                                thousandsSeparator: '',
                                decimalSeparator: '.'
                                    }
                          }
                    },
                    <%=dolSecondaryAmount.UniqueID%>: {
                        row: '.col-xs-3',
                        validators: {
                            numeric: {
                                message: 'The value is not a number',
                                // The default separators
                                thousandsSeparator: '',
                                decimalSeparator: '.'
                            }
                          }
                    },
                     <%=ddlClaimNumber.UniqueID%>: {
                         row: '.col-sm-3',
                         validators: {
                            greaterThan: {
                                value: -1,
                                message: 'Please select an option'
                            }
                          }
                     },
                     <%=txtPayerName.UniqueID%>: {
                         row: '.col-sm-3',
                         validators: {
                            notEmpty: {
                                message: 'Payer name cannot be empty'
                            }
                          }
                     },
                     <%=ddlPaymentType.UniqueID%>: {
                         row: '.col-sm-3',
                         validators: {
                            greaterThan: {
                                value: 0,
                                message: 'Please select an option'
                            }
                          }
                     },
                     <%=txtPaymentReference.UniqueID%>: {
                         row: '.col-sm-3',
                         validators: {
                            notEmpty: {
                                message: 'Please enter a check #'
                            }
                          }
                     }
                }
            })
            .on('input keyup', '[name="<%=dolPatientAmount.UniqueID%>"]', function() {
                       jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', false,'numeric');
                        jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', false,'numeric');
                })
             // keep submit button enabled all the time
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
            // once form is validated
            .on('success.form.fv', function(e) {
                //functions go here
            });

        });

        var blnWarningPrompt = false;

        // capture user leaving the content edit area
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

        function showEditOptions() {
            jQuery("#divViewOptions").addClass("hidden");
            jQuery("#divViewOnly").addClass("hidden");
            jQuery("#divEditOptions").removeClass("hidden");
        }

        //Function- if Check is selected in dropdownlist, Payment Reference textbox is required- users need to enter Check #
        $(function () {
            $('select[id$=ddlPaymentType]').bind("change keyup", function () {
                selIndex = $('#<%=ddlPaymentType.ClientID%>').val();
                if (selIndex == 2 || selIndex == 4 || selIndex == 11){
                    jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=txtPaymentReference.UniqueID%>', true,'notEmpty');  
                    //ValidatorEnable(document.getElementById('rfvPayRef.ClientID%>'), true);
                } else {
                    jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=txtPaymentReference.UniqueID%>', false,'notEmpty');
                    //ValidatorEnable(document.getElementById('rfvPayRef.ClientID%>'), false);
                }
            });
        });

        //function to add ddl selection to txtbox
        function addChangeToTxt() {
            var selectedItem = $('#<%=ddlClaimNumber.ClientID%> option:selected').text();
            var selectedIndex = document.getElementById('<%=ddlClaimNumber.ClientID%>').selectedIndex;
            document.getElementById('<%=txtClaimIndex.ClientID%>').value = selectedIndex;

            if (selectedItem == 'Choose an option') {
                document.getElementById('<%=txtClaimNumber.ClientID%>').value = '-1';
            } else if (selectedItem == 'Waiting for claim to be processed') {
                document.getElementById('<%=txtClaimNumber.ClientID%>').value = '-1';
            }
            else {
                var arrDDLSelectedClaim = selectedItem.split("-");
                document.getElementById('<%=txtClaimNumber.ClientID%>').value = arrDDLSelectedClaim[0];
                document.getElementById('<%=txtPayerName.ClientID%>').value = arrDDLSelectedClaim[4];
            }
    }

    function rdoChange(rdoID) {

        if (rdoID == "patAmt") {
            //Clear out prim & sec payment amounts if amount is greater than 0
            
                //document.getElementById('<%=dolPrimaryAmount.ClientID%>').value = (0).toFixed(2);
                //document.getElementById('<%=dolSecondaryAmount.ClientID%>').value = (0).toFixed(2);
                //Show Payment Options for Patient Amount
                document.getElementById('divPaymentOptions1').style.display = "block";
                document.getElementById('divPaymentOptions2').style.display = "block";
                document.getElementById('rdoPatAmt').checked = true;

                //Show Patient Payment divPayer 1/20/15 
                document.getElementById('divPayerName').style.display = "block";
                document.getElementById('divClaimNumber').style.display = "none";

                // Set PaymentSelection, needed to add this so we know where the payment is being applied when payment amount is $0 -1/29/15
                document.getElementById('<%=txtPaymentSelection.ClientID%>').value = 'PatientAmount';
                
            }
        if (rdoID == "primAmt") {
            //if (document.getElementById('dolPrimaryAmount.ClientID%>').value = 0) {
                //do not reset other payment amounts
            //} else { 
                //document.getElementById('<%=dolPatientAmount.ClientID%>').value = (0).toFixed(2);
                //document.getElementById('<%=dolSecondaryAmount.ClientID%>').value = (0).toFixed(2);
                document.getElementById('divPaymentOptions1').style.display = "none";
                document.getElementById('divPaymentOptions2').style.display = "none";
                document.getElementById('rdoPrimAmt').checked = true;

                //Clear apply to patient amounts
                document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);

                //Show Insurance Payment divPaidByInsurance 1/20/15
                document.getElementById('divClaimNumber').style.display = "block";
                document.getElementById('divPayerName').style.display = "none";

                //Enable Claim Number textbox
                jQuery('#ddlClaimNumber').prop("enabled", true);

                // Set PaymentSelection, needed to add this so we know where the payment is being applied when payment amount is $0 -1/29/15
                document.getElementById('<%=txtPaymentSelection.ClientID%>').value = 'PrimaryAmount';
                //}
            }
        if (rdoID == "secAmt") {
                //document.getElementById('<%=dolPatientAmount.ClientID%>').value = (0).toFixed(2);
                //document.getElementById('<%=dolPrimaryAmount.ClientID%>').value = (0).toFixed(2);
                document.getElementById('divPaymentOptions1').style.display = "none";
                document.getElementById('divPaymentOptions2').style.display = "none";
                document.getElementById('rdoSecAmt').checked = true;

                //Clear apply to patient amounts
                document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);

                //Show Insurance Payment divPaidByInsurance 1/20/15
                document.getElementById('divClaimNumber').style.display = "block";
                document.getElementById('divPayerName').style.display = "none";

                //Enable Claim Number textbox
                jQuery('#ddlClaimNumber').prop("enabled", true);

                // Set PaymentSelection, needed to add this so we know where the payment is being applied when payment amount is $0 -1/29/15
                document.getElementById('<%=txtPaymentSelection.ClientID%>').value = 'SecondaryAmount';
                
            }
        };

        function enableValidator(obj) {
            if (obj == "rfvPatientAmount") {
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPatientAmount.UniqueID%>', true,'numeric'); //enable Patient Amount
                //document.getElementById('rfvPatientAmount.ClientID%>').enabled = true;
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', false,'numeric');
                //ValidatorEnable(document.getElementById('rfvSecondaryAmount.ClientID%>'), false);
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', false,'numeric');
                //ValidatorEnable(document.getElementById('rfvPrimaryAmount.ClientID%>'), false);
                
                //Payment Info Validators
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=txtPayerName.UniqueID%>', true,'notEmpty');
                //ValidatorEnable(document.getElementById('rfvPayer.ClientID%>'), true);
                //Disable Claim validator
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=ddlClaimNumber.UniqueID%>', false,'greaterThan');
                //ValidatorEnable(document.getElementById('rfvClaimNumber.ClientID%>'), false);
            }
            if (obj == "rfvPrimaryAmount") {
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', true,'numeric');
                //document.getElementById('rfvPrimaryAmount.ClientID%>').enabled = true;
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', false,'numeric');
                //ValidatorEnable(document.getElementById('rfvSecondaryAmount.ClientID%>'), false);
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPatientAmount.UniqueID%>', false,'numeric'); //disable Patient Amount
                //ValidatorEnable(document.getElementById('rfvPatientAmount.ClientID%>'), false);

                //Enable Claim validator
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=ddlClaimNumber.UniqueID%>', true,'greaterThan');  
                //ValidatorEnable(document.getElementById('rfvClaimNumber.ClientID%>'), true);

                //Payment Info Validators
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=txtPayerName.UniqueID%>', false,'notEmpty');  
                //ValidatorEnable(document.getElementById('rfvPayer.ClientID%>'), false);
            }
            if (obj == "rfvSecondaryAmount") {
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', true,'numeric'); 
                //document.getElementById('rfvSecondaryAmount.ClientID%>').enabled = true
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPatientAmount.UniqueID%>', false,'numeric'); //disable Patient Amount
                //ValidatorEnable(document.getElementById('rfvPatientAmount.ClientID%>'), false);
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', false,'numeric');
                //ValidatorEnable(document.getElementById('rfvPrimaryAmount.ClientID%>'), false);

                //Enable Claim validator
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=ddlClaimNumber.UniqueID%>', true,'greaterThan');
                //ValidatorEnable(document.getElementById('rfvClaimNumber.ClientID%>'), true);

                //Payment Info Validators
                jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=txtPayerName.UniqueID%>', false,'txtPayerName'); //enable Patient Amount
                //ValidatorEnable(document.getElementById('rfvPayer.ClientID%>'), false);
            }
        }

        function checkNullorZero(obj) {
            if (obj == "dolPatientAmount") {
                var patAmount = jQuery('#' + '<%=dolPatientAmount.ClientID%>').val();
                if (patAmount == 0 || isNaN(patAmount) || patAmount == "") {
                    document.getElementById('<%=dolPatientAmount.ClientID%>').value = (0).toFixed(2);
                }
            }
            if (obj == "dolPrimaryAmount") {
                var primAmount = jQuery('#' + '<%=dolPrimaryAmount.ClientID%>').val();
                if (primAmount == 0 || isNaN(primAmount) || primAmount == "") {
                    document.getElementById('<%=dolPrimaryAmount.ClientID%>').value = (0).toFixed(2);
                }
            }
            if (obj == "dolSecondaryAmount") {
                var secAmount = jQuery('#' + '<%=dolSecondaryAmount.ClientID%>').val();
                if (secAmount == 0 || isNaN(secAmount) || secAmount == "") {
                    document.getElementById('<%=dolSecondaryAmount.ClientID%>').value = (0).toFixed(2);
                }
            }
            if (obj == "dolApplyToInvoice") {
                var dolApplyToInvoice = jQuery('#' + '<%=dolApplyToCurrentInvoice.ClientID%>').val();
                if (dolApplyToInvoice == 0 || isNaN(dolApplyToInvoice) || dolApplyToInvoice == "") {
                    document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                }
            }
            if (obj == "dolApplyToPastDue") {
                var dolApplyToPastDue = jQuery('#' + '<%=dolApplyToPastDue.ClientID%>').val();
                if (dolApplyToPastDue == 0 || isNaN(dolApplyToPastDue) || dolApplyToPastDue == "") {
                    document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                }
            }
            if (obj == "dolApplyToPrinciple") {
                var dolApplyToPrinciple = jQuery('#' + '<%=dolApplyToPrinciple.ClientID%>').val();
                if (dolApplyToPrinciple == 0 || isNaN(dolApplyToPrinciple) || dolApplyToPrinciple == "") {
                    document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                }
            }
            if (obj == "dolApplyToNextInvoice") {
                var dolApplyToNextInvoice = jQuery('#' + '<%=dolApplyToNextInvoice.ClientID%>').val();
                if (dolApplyToNextInvoice == 0 || isNaN(dolApplyToNextInvoice) || dolApplyToNextInvoice == "") {
                    document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                }
            }
        }



        //09/29/14 T3
        // verify required fields
        function checkRequiredFields(objName) {
            var error = false;
            var arrTypes = g_FormObjectTypes.split('+');
            var strErrs = "";
            var delim = "";
            var firstErrFound = "";
            jQuery.each(arrTypes, function () {
                jQuery('#' + objName + ' ' + this + '.required').each(function () {
                    if (((GetFieldType(this) == 'checkbox' || GetFieldType(this) == 'radio') && this.checked == 'unchecked') ||
                                    (GetFieldType(this) == 'select' && this.selectedIndex == 0) ||
                                    (this.value.replace(' ', '') == '')) {
                        error = true;
                        jQuery(this).addClass("requirederror");
                        strErrs += delim + jQuery(this).attr('reqmsg');
                        delim = ",";
                        if (firstErrFound == "") {
                            firstErrFound = this.id;
                        }
                    }
                    else {
                        jQuery(this).removeClass("requirederror");
                    }
                });
            });
            if (strErrs == "") {
            } else {
                alert('The following fields cannot be empty:\n\n' + strErrs);
                jQuery('#' + firstErrFound).focus();
            }
            return error;
        }

        // determine the field type
        function GetFieldType(objField) {
            if (jQuery(objField).is('select')) {
                return 'select';
            }
            else if (jQuery(objField).is('textarea')) {
                return 'textarea';
            }
            else {
                return jQuery(objField).attr('type');
            }
        }

        function SaveData() {
            updateInfo = "";
            delim = "";
            jQuery(".columnback").each(
                function () {
                    if (this.className.split(" ")[1] == "boo") {
                        if (jQuery("#" + this.id.substr(3))[0].checked) {
                            updateInfo += delim + this.id.substr(3) + "||1";

                        } else {
                            updateInfo += delim + this.id.substr(3) + "||0";
                        }
                    } else {
                        updateInfo += delim + this.id.substr(3) + "||" + jQuery("#" + this.id.substr(3)).val();

                    }
                    delim = "::";
                })
            Recid = jQuery('#postid').val();
            Tbl = jQuery('#posttb').val();
            jQuery.post('ajaxGetData.aspx', { action: 'update', id: Recid, tb: Tbl, dBack: updateInfo },
                function () { tabSelect(Tbl); });
        }

        function tabSelect(tabId) {
            document.forms.frmPayments.action += '?tab=' + tabId;
            document.getElementById('frmPayments').submit();
        }

        function checkAmounts() {

            var intPatAmount = parseFloat(jQuery('#<%=dolPatientAmount.ClientID%>').val());
            var intPrimAmount = parseFloat(jQuery('#<%=dolPrimaryAmount.ClientID%>').val());
            var intSecAmount = parseFloat(jQuery('#<%=dolSecondaryAmount.ClientID%>').val());
            if (intPatAmount > 0) {

                var intCurInvBal = jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').val();
                if (intCurInvBal == 0 || isNaN(intCurInvBal) || intCurInvBal == "") {
                    document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                }
                intCurInvBal = parseFloat(jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').val());

                var intPastDue = jQuery('#<%=dolApplyToPastDue.ClientID%>').val();
                if (intPastDue == 0 || isNaN(intPastDue) || intPastDue == "") {
                    document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                }
                intPastDue = parseFloat(jQuery('#<%=dolApplyToPastDue.ClientID%>').val());

                var intPatRemaining = jQuery('#<%=dolApplyToPrinciple.ClientID%>').val();
                if (intPatRemaining == 0 || isNaN(intPatRemaining) || intPatRemaining == "") {
                    document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                }
                intPatRemaining = parseFloat(jQuery('#<%=dolApplyToPrinciple.ClientID%>').val());

                var intNextInvoice = jQuery('#<%=dolApplyToNextInvoice.ClientID%>').val();
                if (intNextInvoice == 0 || isNaN(intNextInvoice) || intNextInvoice == "") {
                    document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                }
                intNextInvoice = parseFloat(jQuery('#<%=dolApplyToNextInvoice.ClientID%>').val());

                var intTotalEntered = (intCurInvBal + intPastDue + intPatRemaining + intNextInvoice).toFixed(2);
                if (intPatAmount == '') {
                    return 'nullZero';
                } else {
                    if (intTotalEntered == intPatAmount) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } else if (intPrimAmount >= 0) {
                return true;
            } else if (intSecAmount >= 0) {
                return true;
            } else {
                return 'nullZero';
            }


        }
        function checkOverPayment() {
            var decCurInvoiceBal = parseFloat(document.getElementById('<%=intCurrentInvoiceBal.ClientID%>').textContent);
            var decCurPatAmtInput = parseFloat(document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value);
            if (decCurPatAmtInput > decCurInvoiceBal) {
                alert('Amount cannot be greater than current invoice balance.');
                document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = decCurInvoiceBal.toFixed(2);
            }
        }

        function setOrig_Payment() {
            var patAmount = document.getElementById('<%=dolPatientAmount.ClientID%>').value;
            var priAmount = document.getElementById('<%=dolPrimaryAmount.ClientID%>').value;
            var secAmount = document.getElementById('<%=dolSecondaryAmount.ClientID%>').value;
            ////convert to integer with 2 decimals
            patAmount = parseFloat(patAmount);
            priAmount = parseFloat(priAmount);
            secAmount = parseFloat(secAmount);
            paymentAmount = patAmount + priAmount + secAmount;
            if (isNaN(paymentAmount) || paymentAmount == "") {
            } else {
                document.getElementById('<%=dolOrig_Payment.ClientID%>').value = paymentAmount.toFixed(2);
            }
        }

        function populateAmounts() {

            var patAmount = document.getElementById('<%=dolPatientAmount.ClientID%>').value;
            var intCurInvoiceBal = document.getElementById('<%=intCurrentInvoiceBal.ClientID%>').textContent;
            var intCurPastDueBal = document.getElementById('<%=intPastDueBal.ClientID%>').textContent;


            ////convert to integer with 2 decimals
            patAmount = parseFloat(patAmount);
            intCurInvoiceBal = parseFloat(intCurInvoiceBal);
            intCurPastDueBal = parseFloat(intCurPastDueBal);

            var intRemainingAmt = patAmount - intCurInvoiceBal;
            if (patAmount == 0 || isNaN(patAmount) || patAmount == "") {

                //Patient Amount is blank or 0 -- disable everything & set = 0
                document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);
            } else {
                //Does Patient Amount cover current invoice/balance?
                if (patAmount >= intCurInvoiceBal) {
                    //check to see if intCurInvoice Bal is already paid
                    if (intCurInvoiceBal == 0 || intCurInvoiceBal == "") {
                    } else {
                        //fill in current invoice bal textbox
                        document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = intCurInvoiceBal;
                        //is there money left over?
                    }
                    if (intRemainingAmt > 0) {
                        //yes
                        //display remaining amount
                        intRemainingAmt = patAmount - intCurInvoiceBal;
                        document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = intRemainingAmt.toFixed(2);
                        //first, add remaining amount to past due amount (if applicable)
                        if (intCurPastDueBal > 0) {
                            if (intRemainingAmt >= intCurPastDueBal) {
                                //Remaining Amount can cover Past Due Balance
                                //fill Past Due text box
                                document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = intCurPastDueBal;
                                //re-calculate Remaining Amount
                                intRemainingAmt = intRemainingAmt - intCurPastDueBal;
                                if (intRemainingAmt == 0) {
                                    //no remaining amount-disable Principal & next invoice options
                                    document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                                    document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                                    document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);

                                } else {
                                    //show Principal & next invoice options for remaining amount & remaining label
                                    document.getElementById('divPaymentOptions2').style.display = "block";
                                    document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = intRemainingAmt.toFixed(2);
                                }
                            } else {
                                //remaining amount won't cover all of the past due amount-just fill past due amount with what is left
                                //hide other payment options since we won't have any let over 
                                document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = intRemainingAmt;
                                document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                                document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                                document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);
                            }
                        } else {
                            //no past due balance-disable/ disable Past Due
                            //display Principal or next invoice options
                            document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                            document.getElementById('divPaymentOptions2').style.display = "block";
                        }
                    } else {
                        //no money left over -hide all other textboxes
                        //activate/deactivate boxes
                        //set values
                        document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);

                    }
                } else {
                    //Patient Amount < current invoice/balance
                    //fill in current invoice bal textbox if invoice is > 0 
                    if (intCurInvoiceBal > 0) {
                        document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = patAmount.toFixed(2);
                    }
                    //DisableOther apply to textboxes
                    document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                    document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                    document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                    document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);
                }
            }
        }

        function updateRemaining() {

                var patAmount = document.getElementById('<%=dolPatientAmount.ClientID%>').value;
            var dolApplyToCurrentInvoice = document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value;
            var dolApplyToPastDue = document.getElementById('<%=dolApplyToPastDue.ClientID%>').value;
            var dolApplyToPrinciple = document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value;
            var dolApplyToNextInvoice = document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value;

            ////convert to integer with 2 decimals
            patAmount = parseFloat(patAmount);
            dolApplyToCurrentInvoice = parseFloat(dolApplyToCurrentInvoice);
            dolApplyToPastDue = parseFloat(dolApplyToPastDue);
            dolApplyToPrinciple = parseFloat(dolApplyToPrinciple);
            dolApplyToNextInvoice = parseFloat(dolApplyToNextInvoice);

            if (patAmount == 0 || isNaN(patAmount) || patAmount == "") {

                //Patient Amount is blank or 0 -- disable everything & set = 0
                document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);
            } else {


                var intRemainingAmt = patAmount - dolApplyToCurrentInvoice - dolApplyToPastDue - dolApplyToPrinciple - dolApplyToNextInvoice;
                if (intRemainingAmt < 0) {
                    document.getElementById('<%=lblRemainingAmt.ClientID%>').style.color = "red";
                    document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (intRemainingAmt).toFixed(2);
                } else {
                    document.getElementById('<%=lblRemainingAmt.ClientID%>').style.color = "green";
                    document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (intRemainingAmt).toFixed(2);
                }
            }
        }

        function getPatName(txtBox) {
            var strVal;
            if (txtBox == 'fst') {
                //searching on first name
                strVal = 'fst' + jQuery('#<%=txtFirstName.ClientID%>').val();
                document.getElementById('<%=txtLastName.ClientID%>').value = "";
            } else if (txtBox == 'lst') {
                //searching on Chart Number
                strVal = 'lst' + jQuery('#<%=txtLastName.ClientID%>').val();
                document.getElementById('<%=txtFirstName.ClientID%>').value = "";
            } else if (txtBox == 'cht') {
                //searching on Chart Number
                strVal = 'cht' + jQuery('#<%=txtChartNumber.ClientID%>').val();
            }
            else {
                //Coming from contracts page with cno sent in 
                strVal = txtBox;
            }
            jQuery.post("ajaxOrtho.aspx?action=getPatName", { id: strVal, frm: "PaymentEntry" },
                       function (data) {
                           var strData = data;
                           if (strData.indexOf("~~") == -1) {
                               if (strData.indexOf("<h4>") == 0) {
                                   //Display modal for contract selection
                                   jQuery(".modal-title")[0].innerHTML = "Select Contract";
                                   jQuery('#divModalContractSelection').modal('show');
                                   jQuery('#loading_indicator').show();
                                   jQuery('.modal-contract').html(data);
                                   jQuery('#loading_indicator').hide();
                               } else {
                                   //No contract found.
                                   document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData;
                                           jQuery('#<%=txtChartNumber.ClientID%>').focus();
                                           document.getElementById('<%=txtChartNumber.ClientID%>').value = "";
                                           document.getElementById('<%=txtFirstName.ClientID%>').value = "";
                                           document.getElementById('<%=txtLastName.ClientID%>').value = "";
                                   //Disable required field validators  for primary & secondary amount
                                           jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', false,'numeric');
                                           jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', false,'numeric');
                                           //ValidatorEnable(document.getElementById('rfvPrimaryAmount.ClientID%>'), false);
                                           //ValidatorEnable(document.getElementById('rfvSecondaryAmount.ClientID%>'), false);
                                           document.getElementById('<%=lblInsurance.ClientID%>').innerHTML = "";
                                           //disable and set apply to values to 0 until user has chosen a patient
                                           jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', true);
                                           jQuery('#<%=dolApplyToPastDue.ClientID%>').prop('readonly', true);
                                           jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop('readonly', true);
                                           jQuery('#<%=dolApplyToNextInvoice.ClientID%>').prop('readonly', true);
                                           document.getElementById('<%=dolPatientAmount.ClientID%>').value = (0).toFixed(2);
                                           document.getElementById('<%=dolPrimaryAmount.ClientID%>').value = (0).toFixed(2);
                                           document.getElementById('<%=dolSecondaryAmount.ClientID%>').value = (0).toFixed(2);
                                           document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                                           document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                                           document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                                           document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                                           document.getElementById('<%=lblCurrentInvoiceBal.ClientID%>').innerHTML = (0).toFixed(2);
                                           document.getElementById('<%=lblPatRemaniningBal.ClientID%>').innerHTML = (0).toFixed(2);
                                           document.getElementById('<%=lblPastDueBal.ClientID%>').innerHTML = (0).toFixed(2);
                                           document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);

                                           document.getElementById('<%=txtContract_RECID.ClientID%>').value = "-1";
                                           //11/29/16 CS getting doctors_vw back from contract data
                                           document.getElementById('<%=txtDoctors_vw.ClientID%>').value = "-1";
                                           document.getElementById('<%=intCurrentInvoiceBal.ClientID%>').innerHTML = (0).toFixed(2);
                                           document.getElementById('<%=intPatRemaniningBal.ClientID%>').innerHTML = (0).toFixed(2);
                                           document.getElementById('<%=intPastDueBal.ClientID%>').innerHTML = (0).toFixed(2);
                                           document.getElementById('<%=intCurInvoice.ClientID%>').innerHTML = (0).toFixed(2);
                                           jQuery('#ddlClaimNumber').prop("enabled", false);

                                       }
                                   } else {
                                       //enable amounts textboxes & radio buttons

                                       jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', false);
                                       jQuery('#<%=dolApplyToPastDue.ClientID%>').prop('readonly', false);
                                       jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop('readonly', false);
                                       jQuery('#<%=dolApplyToNextInvoice.ClientID%>').prop('readonly', false);
                                       //set values
                                       arrData = strData.split("~~");
                                       document.getElementById('<%=txtPatientNumber.ClientID%>').value = arrData[0];
                                       document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = arrData[1];
                                       document.getElementById('<%=txtChartNumber.ClientID%>').value = arrData[2];
                                       document.getElementById('<%=txtContract_RECID.ClientID%>').value = arrData[3];
                                       document.getElementById('<%=lblInsurance.ClientID%>').innerHTML = arrData[4];
                                       document.getElementById('<%=lblInsurance.ClientID%>').innerHTML += arrData[5];
                                       document.getElementById('<%=lblCurrentInvoiceBal.ClientID%>').innerHTML = arrData[6];
                                       document.getElementById('<%=intCurrentInvoiceBal.ClientID%>').innerHTML = arrData[7];
                                       document.getElementById('<%=lblPatRemaniningBal.ClientID%>').innerHTML = arrData[8];
                                       document.getElementById('<%=intPatRemaniningBal.ClientID%>').innerHTML = arrData[9];
                                       document.getElementById('<%=lblPastDueBal.ClientID%>').innerHTML = arrData[10];
                                       document.getElementById('<%=intPastDueBal.ClientID%>').innerHTML = arrData[11];
                                       document.getElementById('<%=intCurInvoice.ClientID%>').innerHTML = arrData[12];
                                       document.getElementById('<%=txtFirstName.ClientID%>').value = arrData[14];
                                       document.getElementById('<%=txtLastName.ClientID%>').value = arrData[15];
                                       //11/29/16 CS getting doctors_vw back from contract data
                                       document.getElementById('<%=txtDoctors_vw.ClientID%>').value = arrData[18];
                                       jQuery('#rdoPatAmt').focus();

                                       if (arrData[3] == "-1") {
                                           //No contract, disable Primary & Secondary payment boxes
                                           //disable radio button & textbox
                                           document.getElementById('rdoPrimAmt').disabled = true;
                                           document.getElementById('rdoSecAmt').disabled = true;
                                           document.getElementById('<%=dolPrimaryAmount.ClientID%>').disabled = true;
                                           document.getElementById('<%=dolSecondaryAmount.ClientID%>').disabled = true;
                                       } else {
                                           //re-enable boxes, there is a contract
                                           document.getElementById('rdoPrimAmt').disabled = false;
                                           document.getElementById('rdoSecAmt').disabled = false;
                                           document.getElementById('<%=dolPrimaryAmount.ClientID%>').disabled = false;
                                           document.getElementById('<%=dolSecondaryAmount.ClientID%>').disabled = false;
                                       }
                                       if (arrData[9] == 0 || arrData[9] == "") {
                                           //No principle amount, user cannot enter a value
                                           jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop("readonly", true);
                                       }
                                       //Past Invoices found?
                                       if (arrData[11] == 0 || arrData[11] == "") {
                                           jQuery('#<%=dolApplyToPastDue.ClientID%>').prop("readonly", true);
                                        }
                                       //Current Invoice $0.00?
                                        if (arrData[7] == "" || arrData[7] == 0) {
                                            //disable current invoice
                                            jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop("readonly", true);

                                        }
                                        strClaimValues = arrData[16];   // reload dropdown
                                        strClaimText = arrData[17];
                                        DDLClaims = document.getElementById('<%= ddlClaimNumber.ClientID%>');
                                       ddlValues = strClaimValues.split(",");
                                       ddlText = strClaimText.split(",");
                                       DDLSelectedIndex = -2
                                       DDLClaims.length = 0;


                                       for (i = 0; i <= ddlValues.length - 1; i++) {
                                           var option = document.createElement("option");
                                           option.value = ddlValues[i];
                                           option.innerHTML = ddlText[i];
                                           DDLClaims.appendChild(option);
                                           if (i == claimIndex) {
                                               option.selected = true;
                                           }
                                       }
                                       // reset Claims array
                                       //arrClaims = eval(arrData[19]);
                                        
                                   }
                               });

                           }

        function getPatNameCID(obj, txtBox) {
            cid = obj.value;
            if (txtBox == 'con') {
                //searching on txtContract_RECID
                strVal = 'con' + jQuery('#' + '<%=txtContract_RECID.ClientID%>').val();
            } else {
                //searching on txtChartNumber
                strVal = 'cht' + jQuery('#' + '<%=txtChartNumber.ClientID%>').val();
            }
            jQuery('#btnPopupCancel').click();
            jQuery.post("ajaxOrtho.aspx?action=getPatName", { id: strVal, con: cid, frm: "PaymentEntry" },
            function (data) {
                var strData = data;
                if (strData.indexOf("~~") == -1) {
                    if (strData.indexOf("<h4>") == 0) {
                        //Display modal for contract selection
                        jQuery(".modal-title")[0].innerHTML = "Select Contract";
                        jQuery('#divModalContractSelection').modal('show');
                        jQuery('#loading_indicator').show();
                        jQuery('.modal-contract').html(data);
                        jQuery('#loading_indicator').hide();
                    } else {
                        //No contract found. Higly unlikely, but just incase
                        document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData;
                        jQuery('#<%=txtChartNumber.ClientID%>').focus();
                        document.getElementById('<%=txtChartNumber.ClientID%>').value = "";
                        document.getElementById('<%=txtFirstName.ClientID%>').value = "";
                        document.getElementById('<%=txtLastName.ClientID%>').value = "";
                        //Disable required field validators  for primary & secondary amount
                        jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', false,'numeric');
                        jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', false,'numeric');
                        //ValidatorEnable(document.getElementById('rfvPrimaryAmount.ClientID%>'), false);
                        //ValidatorEnable(document.getElementById('rfvSecondaryAmount.ClientID%>'), false);
                        //disable and set apply to values to 0 until user has chosen a patient
                        document.getElementById('<%=lblInsurance.ClientID%>').innerHTML = "";
                        jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', true);
                        jQuery('#<%=dolApplyToPastDue.ClientID%>').prop('readonly', true);
                        jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop('readonly', true);
                        jQuery('#<%=dolApplyToNextInvoice.ClientID%>').prop('readonly', true);
                        document.getElementById('<%=dolPatientAmount.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolPrimaryAmount.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolSecondaryAmount.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = (0).toFixed(2);
                        document.getElementById('<%=lblCurrentInvoiceBal.ClientID%>').innerHTML = (0).toFixed(2);
                        document.getElementById('<%=lblPatRemaniningBal.ClientID%>').innerHTML = (0).toFixed(2);
                        document.getElementById('<%=lblPastDueBal.ClientID%>').innerHTML = (0).toFixed(2);
                        document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);

                        document.getElementById('<%=txtContract_RECID.ClientID%>').value = "-1";
                        //11/29/16 CS getting doctors_vw back from contract data
                        document.getElementById('<%=txtDoctors_vw.ClientID%>').value = "-1";
                        document.getElementById('<%=intCurrentInvoiceBal.ClientID%>').innerHTML = (0).toFixed(2);
                        document.getElementById('<%=intPatRemaniningBal.ClientID%>').innerHTML = (0).toFixed(2);
                        document.getElementById('<%=intPastDueBal.ClientID%>').innerHTML = (0).toFixed(2);
                        document.getElementById('<%=intCurInvoice.ClientID%>').innerHTML = (0).toFixed(2);
                        jQuery('#ddlClaimNumber').prop("enabled", false);
                    }
                } else {

                    //enable amounts textboxes & radio buttons
                    jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', false);
                    jQuery('#<%=dolApplyToPastDue.ClientID%>').prop('readonly', false);
                    jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop('readonly', false);
                    jQuery('#<%=dolApplyToNextInvoice.ClientID%>').prop('readonly', false);

                    arrData = strData.split("~~");
                    document.getElementById('<%=txtPatientNumber.ClientID%>').value = arrData[0];
                    document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = arrData[1];
                    document.getElementById('<%=txtChartNumber.ClientID%>').value = arrData[2];
                    document.getElementById('<%=txtContract_RECID.ClientID%>').value = arrData[3];
                    document.getElementById('<%=lblInsurance.ClientID%>').innerHTML = arrData[4];
                    document.getElementById('<%=lblInsurance.ClientID%>').innerHTML += arrData[5];
                    document.getElementById('<%=lblCurrentInvoiceBal.ClientID%>').innerHTML = arrData[6];
                    document.getElementById('<%=intCurrentInvoiceBal.ClientID%>').innerHTML = arrData[7];
                    document.getElementById('<%=lblPatRemaniningBal.ClientID%>').innerHTML = arrData[8];
                    document.getElementById('<%=intPatRemaniningBal.ClientID%>').innerHTML = arrData[9];
                    document.getElementById('<%=lblPastDueBal.ClientID%>').innerHTML = arrData[10];
                    document.getElementById('<%=intPastDueBal.ClientID%>').innerHTML = arrData[11];
                    document.getElementById('<%=intCurInvoice.ClientID%>').innerHTML = arrData[12];
                    document.getElementById('<%=txtFirstName.ClientID%>').value = arrData[14];
                    document.getElementById('<%=txtLastName.ClientID%>').value = arrData[15];
                    //11/29/16 CS getting doctors_vw back from contract data
                    document.getElementById('<%=txtDoctors_vw.ClientID%>').value = arrData[18];
                    jQuery('#rdoPatAmt').focus();

                    if (arrData[3] == "-1") {
                        //No contract, disable Primary & Secondary payment boxes
                        //disable radio button & textbox
                        document.getElementById('rdoPrimAmt').disabled = true;
                        document.getElementById('rdoSecAmt').disabled = true;
                        document.getElementById('<%=dolPrimaryAmount.ClientID%>').disabled = true;
                        document.getElementById('<%=dolSecondaryAmount.ClientID%>').disabled = true;
                    } else {
                        //re-enable boxes, there is a contract
                        document.getElementById('rdoPrimAmt').disabled = false;
                        document.getElementById('rdoSecAmt').disabled = false;
                        document.getElementById('<%=dolPrimaryAmount.ClientID%>').disabled = false;
                        document.getElementById('<%=dolSecondaryAmount.ClientID%>').disabled = false;
                    }

                    if (arrData[9] == 0 || arrData[9] == "") {
                        //No contract data yet
                        //hiding txt box to avoid confusion, there will not be a remaining balance yet
                        jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop("readonly", true);
                    }
                    //Past Invoices found?
                    if (arrData[11] == 0 || arrData[11] == "") {
                        jQuery('#<%=dolApplyToPastDue.ClientID%>').prop("readonly", true);
                    }
                    //Current Invoice $0.00?
                    if (arrData[7] == "" || arrData[7] == 0) {
                        //disable current invoice
                        jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop("readonly", true);

                    }

                    strClaimValues = arrData[16];   // reload dropdown
                    strClaimText = arrData[17];
                    DDLClaims = document.getElementById('<%= ddlClaimNumber.ClientID%>');
                    ddlValues = strClaimValues.split(",");
                    ddlText = strClaimText.split(",");
                    DDLSelectedIndex = -2
                    DDLClaims.length = 0;

                    //rodney -changed ValueText to ddlValues line below
                    for (i = 0; i <= ddlValues.length - 1; i++) {
                        var option = document.createElement("option");
                        option.value = ddlValues[i];
                        option.innerHTML = ddlText[i];
                        DDLClaims.appendChild(option);
                        //if (i == claimIndex) {
                        //    option.selected = true;
                        //}
                    }
                }
            });

        }

        function pullPaymentInfo(pid, viewMode) {
            var blnViewMode = viewMode;
            jQuery.post("ajaxOrtho.aspx?action=pullPaymentInfo", { id: pid, vwMode: blnViewMode },
            function (data) {
                var strData = data;
                var arrData = strData.split("~~");
                
                document.getElementById('<%=txtPatientNumber.ClientID%>').value = arrData[0];
                document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = arrData[1];
                document.getElementById('<%=txtChartNumber.ClientID%>').value = arrData[2];
                document.getElementById('<%=txtContract_RECID.ClientID%>').value = arrData[3];
                document.getElementById('<%=lblInsurance.ClientID%>').innerHTML = arrData[4];
                document.getElementById('<%=lblInsurance.ClientID%>').innerHTML += arrData[5];
                document.getElementById('<%=lblCurrentInvoiceBal.ClientID%>').innerHTML = arrData[6];
                document.getElementById('<%=intCurrentInvoiceBal.ClientID%>').innerHTML = arrData[7];
                document.getElementById('<%=lblPatRemaniningBal.ClientID%>').innerHTML = arrData[8];
                document.getElementById('<%=intPatRemaniningBal.ClientID%>').innerHTML = arrData[9];
                document.getElementById('<%=lblPastDueBal.ClientID%>').innerHTML = arrData[10];
                document.getElementById('<%=intPastDueBal.ClientID%>').innerHTML = arrData[11];
                document.getElementById('<%=intCurInvoice.ClientID%>').innerHTML = arrData[12];
                document.getElementById('<%=lblRemainingAmt.ClientID%>').innerHTML = (0).toFixed(2);
                var intPatAmount = parseFloat(arrData[16]);
                var intApplyToCurrentInvoice = parseFloat(arrData[17]);
                var intApplyToPastDue = parseFloat(arrData[18]);
                var intApplyToPrinciple = parseFloat(arrData[19]);
                var intApplyToNextInvoice = parseFloat(arrData[20]);
                document.getElementById('<%=dtePaymentDate.ClientID%>').value = arrData[21];
                document.getElementById('<%=txtFirstName.ClientID%>').value = arrData[22];
                document.getElementById('<%=txtLastName.ClientID%>').value = arrData[23];
                document.getElementById('<%=txtPayerName.ClientID%>').value = arrData[24];
                
                
                //set ddl index to arrData[25]
                document.getElementById('<%=txtComments.ClientID%>').value = arrData[26];
                document.getElementById('<%=txtPaymentReference.ClientID%>').value = arrData[27];

                //11/29/16 CS claimIndex comes back in arrData[28] - not used anywhere tho??
                //11/29/16 CS getting doctors_vw back from contract data (should be in arrData[29])
                document.getElementById('<%=txtDoctors_vw.ClientID%>').value = arrData[29];
                alert(arrData[29]);

                //Readonly/Disable validation on Patient Number & Chart Number since user cannot edit
                jQuery('#<%=txtChartNumber.ClientID%>').prop('readonly', true);
                jQuery('#<%=txtPatientNumber.ClientID%>').prop('readonly', true);
                
                //jQuery('#frmPayments').formValidation('enableFieldValidators', 'txtPatientNumber.UniqueID%>', false,'notEmpty'); 
                //jQuery('#frmPayments').formValidation('enableFieldValidators', 'txtPatientNumber.UniqueID%>', false,'integer'); 
                //jQuery('#frmPayments').formValidation('enableFieldValidators', 'txtChartNumber.UniqueID%>', false,'notEmpty'); 
                //jQuery('#frmPayments').formValidation('enableFieldValidators', 'txtChartNumber.UniqueID%>', false,'integer'); 
                jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', false);
                jQuery('#<%=dolApplyToPastDue.ClientID%>').prop('readonly', false);
                jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop('readonly', false);
                jQuery('#<%=dolApplyToNextInvoice.ClientID%>').prop('readonly', false);
                
                
                //Check which payment amount was entered 
                var rdoChecked = arrData[15];
                if (arrData[15] == "PatientAmount") {
                    //Enable/Fill/Focus Payment amount
                    rdoChange('patAmt');
                    document.getElementById('<%=dolPatientAmount.ClientID%>').value = intPatAmount.toFixed(2);

                    //Disable required field validators  for primary & secondary amount
                    jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', false,'numeric');
                    jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', false,'numeric');
                    //ValidatorEnable(document.getElementById('rfvPrimaryAmount.ClientID%>'), false);
                    //ValidatorEnable(document.getElementById('rfvSecondaryAmount.ClientID%>'), false);

                    //Enable & Fill Apply To boxes
                    document.getElementById('<%=dolApplyToCurrentInvoice.ClientID%>').value = intApplyToCurrentInvoice.toFixed(2);
                    if (intApplyToCurrentInvoice > 0) {
                        jQuery('#<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', false);

                    }
                    document.getElementById('<%=dolApplyToPastDue.ClientID%>').value = intApplyToPastDue.toFixed(2);
                    if (intApplyToPastDue > 0) {
                        jQuery('#<%=dolApplyToPastDue.ClientID%>').prop('readonly', false);

                    }
                    document.getElementById('<%=dolApplyToPrinciple.ClientID%>').value = intApplyToPrinciple.toFixed(2);
                    if (intApplyToPrinciple > 0) {
                        jQuery('#<%=dolApplyToPrinciple.ClientID%>').prop('readonly', false);

                    }
                    document.getElementById('<%=dolApplyToNextInvoice.ClientID%>').value = intApplyToNextInvoice.toFixed(2);
                    if (intApplyToNextInvoice > 0) {
                        jQuery('#<%=dolApplyToNextInvoice.ClientID%>').prop('readonly', false);
                    }
                } else if (arrData[15] == "PrimaryAmount") {
                    
                    //Disable required field validators  for primary & secondary amount
                    $('#frmPayments').formValidation('enableFieldValidators', '<%=dolPatientAmount.UniqueID%>', false,'numeric'); //disable Patient Amount
                    //ValidatorEnable(document.getElementById('rfvPatientAmount.ClientID%>'), false);
                    jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolSecondaryAmount.UniqueID%>', false,'numeric');
                    //ValidatorEnable(document.getElementById('rfvSecondaryAmount.ClientID%>'), false);
                    //Enable/Fill/Focus Primary amount
                    rdoChange('primAmt');
                    document.getElementById('<%=dolPrimaryAmount.ClientID%>').value = intPatAmount.toFixed(2);
                    if (arrData[14].indexOf("False") >= 0) {
                        //ddlClaimNumber
                        jQuery('#<%=dolPrimaryAmount.ClientID%>').focus();
                    }

                } else if (arrData[15] == "SecondaryAmount") {
                    //Disable required field validators  for primary & patient amount
                    $('#frmPayments').formValidation('enableFieldValidators', '<%=dolPatientAmount.UniqueID%>', false,'numeric'); //disable Patient Amount
                    //ValidatorEnable(document.getElementById('rfvPatientAmount.ClientID%>'), false);
                    jQuery('#frmPayments').formValidation('enableFieldValidators', '<%=dolPrimaryAmount.UniqueID%>', false,'numeric');
                    //ValidatorEnable(document.getElementById('rfvPrimaryAmount.ClientID%>'), false);
                    //Enable/Fill/Focus Secondary amount
                    rdoChange('secAmt');
                    document.getElementById('<%=dolSecondaryAmount.ClientID%>').value = intPatAmount.toFixed(2);
                    if (arrData[14].indexOf("False") >= 0) {
                        jQuery('#<%=dolSecondaryAmount.ClientID%>').focus();
                    }
                }
                
                //Past Invoices found?
                if (arrData[13].indexOf("False") >= 0) {
                    document.getElementById('lblPastDueDiv').style.display = "none";
                } else {
                    document.getElementById('lblPastDueDiv').style.display = "block";
                }
                //Current Invoice $0.00?
                if (arrData[7] == 0) {
                    //disable current invoice
                    jQuery('<%=dolApplyToCurrentInvoice.ClientID%>').prop('readonly', true);
                }


                //possibly send extra parameter (i.e. view or edit sate to show/hide correct buttons, also thought of using christies buttons on contract entry
                //hide/display buttons
                if (arrData[14].indexOf("False") >= 0) {
                    jQuery('#divEditOptions').removeClass('hidden');
                }

                jQuery('#<%=btnAdd.ClientID%>').addClass('hidden');
            });
        }

        function postData() {
            Tbl = 'PaymentsTemp';
            //get chart number 08/27/14 CP
            jQuery.post('ajaxOrtho.aspx', { action: 'postPending', tb: Tbl },
                    function () {
                        document.getElementById('frmPayments').submit()
                    });
        }

        function saveChanges() {
            //hide/display buttons
            jQuery('#divEditOptions').addClass('hidden');
            jQuery('#<%=btnAdd.ClientID%>').removeClass('hidden');
            alert('Payment changes have been saved.');

        }


    </script>


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


    <script>
        $(document).ready(function () {
            $('.nav li.active').removeClass('active');
            document.getElementById("#PaymentPosting").className += " active";
        });
    </script>
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script>

        function ClaimSelected(iIndex) {
            //if (iIndex == 0) {
            //} else {
            //    if (jQuery("#rdoPatAmt").is(':checked')) {
            //        //Should not fall into this, but just in case
            //        alert('Please enter a primary or secondary amount to process a claim.');
            //    }
            //    if (jQuery("#rdoPrimAmt").is(':checked')) {
            //        if (arrClaims[iIndex][0] == 0) {
            //            //Primary dropdown with primary claim selection 
            //        } else if (arrClaims[iIndex][0] == "") {
            //            //do nothing
            //        } else if (arrClaims[iIndex][0] == 1) {
            //            alert('This is a secondary insurance claim, you are entering a payment for a primary insurance claim');
            //        }
            //    }
            //    if (jQuery("#rdoSecAmt").is(':checked')) {
            //        if (arrClaims[iIndex][0] == 1) {
            //            //sec dropdown with sec claim selection 

            //        } else if (arrClaims[iIndex][0] == "") {
            //            //do nothing
            //        } else if (arrClaims[iIndex][0] == 0) {
            //            alert('This is a primary insurance claim, you are entering a payment for a secondary insurance claim');
            //        }
            //    }
            //}
        }
    </script>
</asp:Content>
