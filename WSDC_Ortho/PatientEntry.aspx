<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="PatientEntry.aspx.vb" Inherits="WSDC_Ortho.PatientEntry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<form id='frmPatientEntry' class="form-horizontal" role="form" action="PatientEntry.aspx" runat="server" defaultbutton="btnHidden">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" class="hidden" runat="server" OnClientClick="return false;" />
    <asp:HiddenField ID="txtSessions" runat="server" />
        <asp:TextBox ID="txtRecID" CssClass="DB form-control hidden" runat="server"></asp:TextBox>

        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        <div class="container">
                
            <asp:Literal ID="litPatientID" runat="server"></asp:Literal>
            <h4 style="color: #2d6ca2;" class=" text-center"><asp:Label ID="lblPatientName" runat="server"></asp:Label></h4>
            <div id="divEditOptions" class="col-sm-12 hidden">
                <div class=" col-sm-offset-1 col-sm-9">
                <div class="pull-left">
                    <ul>
                        <li><a href="#">
                            <asp:Button ID="btnSave" OnClientClick="saveData(false);" class="btn btn-sm btn-primary" runat="server" Text="Save Changes" /></a></li>
                        <li><a href="#">
                            <asp:Button ID="btnCancel" class="btn btn-sm btn-primary" runat="server" Text="Cancel Edit" OnClientClick="CancelVal();"/></a></li>
                        <li><a href="#">
                            <asp:Button ID="btnDelete" OnClientClick="CancelVal(); return confirm('Are you sure you want to delete this patient?')" class="btn btn-sm btn-danger" runat="server" Text="Delete" /></a></li>
                    </ul>
                </div>
                </div>
            </div>
            <div id="divViewOptions" class="col-sm-12">
                <div class=" col-sm-offset-1 col-sm-9">
                <div class="pull-left">
                    <span class="glyphicon glyphicon-eye-open"></span>View Only Mode <a href="#" onclick="showEditOptions()">Edit Patient (Click Here)</a>
                </div>
                <br /><br />
                <div id="divPatientOptions" class="pull-left">
                    <ul>
                        <li><a href="#">
                            <asp:Button ID="btnPaymentPosting" class="btn btn-sm btn-success" runat="server" Text="Post Payment" OnClientClick="CancelVal();"/></a></li>
                        <li><a href="#">
                            <asp:Button ID="btnContractStatus" class="btn btn-sm btn-primary" runat="server" Text="Contract Status" OnClientClick="CancelVal();"/></a></li>
                        <li><a href="#">
                            <asp:Button ID="btnPaymentHistory" class="btn btn-sm btn-primary" runat="server" Text="Payment History" OnClientClick="CancelVal();"/></a></li>
                    </ul>
                </div>
                </div>
            </div>
            <%--<div class="col-sm-12">
                <hr />
            </div>--%>
            <div class="col-sm-12">
                <div class="form-group" style="color: #2d6ca2;">
                    <div class=" col-sm-offset-1 col-sm-9">
                    <label for="txtPatient_Id" class="col-sm-3"><span class="requiredFieldIndicator">*</span> Patient #:
                            <asp:TextBox ID="txtPatient_Id" value="" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" runat="server" 
                                onkeypress="return IsNumeric(event);" onpaste="return IsNumeric(event);" onChange="getPatName('pat')" ></asp:TextBox>                       
                            <asp:RequiredFieldValidator ID="rfvPatientNumber" runat="server" ControlToValidate="txtPatient_Id" Display="Dynamic" ErrorMessage="Required" class="requiredFieldValidator" />                   
                    </label>
                    <label for="txtChart_Number" class="col-sm-3"><span class="requiredFieldIndicator">*</span> Chart #:
                    <span style="padding-left: 0px;">
                        <asp:TextBox ID="txtChart_Number" CssClass="DB form-control" MaxLength="20" Style="max-width: 130px;" runat="server" 
                            onkeypress="return IsNumeric(event);" onpaste="return IsNumeric(event);" onChange="getPatName('cht')" ></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvChartNumber" runat="server" ControlToValidate="txtChart_Number" Display="Dynamic" ErrorMessage="Required" class="requiredFieldValidator" />
                    </span>
                    </label>
                </div>
            </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class=" col-sm-offset-1 col-sm-9">
                        <label for="txtQSIPatNum" class="col-sm-3">QSI Patient #:
                        <span class="col-sm-8" style="padding-left: 0px;">
                            <asp:TextBox ID="txtQSIPatNum" value="" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                        <label for="ddlPlan_Pri" class="col-sm-3">Primary Plan:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlPlan_Pri" CssClass="DB form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                            </span>
                        </label>
                        <label for="ddlPlan_Sec" class="col-sm-3">Secondary Plan:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlPlan_Sec" CssClass="DB form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                            </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class=" col-sm-offset-1 col-sm-9">
                        <label for="txtname_first" class="col-sm-3">First Name:
                        <span class="col-sm-8" style="padding-left: 0px;">
                            <asp:TextBox ID="txtname_first" value="" CssClass="DB form-control" MaxLength="24" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                        <label for="txtname_mid" class="col-sm-3">Middle Name:
                            <span class="col-sm-8" style="padding-left: 0px;">
                                <asp:TextBox ID="txtname_mid" value="" CssClass="DB form-control" MaxLength="16" Style="max-width: 130px;" runat="server"></asp:TextBox>
                            </span>
                        </label>
                        <label for="txtname_last" class="col-sm-3">Last Name:
                        <span class="col-sm-8" style="padding-left: 0px;">
                            <asp:TextBox ID="txtname_last" value="" CssClass="DB form-control" MaxLength="24" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class=" col-sm-offset-1 col-sm-9">
                        <label for="txtaddr_other" class="col-sm-12">C/O:
                        <span class="col-sm-12" style="padding-left: 0px;">
                            <asp:TextBox ID="txtaddr_other" value="" CssClass="DB form-control" MaxLength="24" Style="max-width: 230px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class=" col-sm-offset-1 col-sm-9">
                        <label for="txtaddr_street" class="col-sm-12">Street/PO Box:
                        <span class="col-sm-12" style="padding-left: 0px;">
                            <asp:TextBox ID="txtaddr_street" value="" CssClass="DB form-control" MaxLength="24" Style="max-width: 230px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class=" col-sm-offset-1 col-sm-9">
                        <label for="txtaddr_city" class="col-sm-3">City:
                        <span class="col-sm-12" style="padding-left: 0px;">
                            <asp:TextBox ID="txtaddr_city" value="" CssClass="DB form-control" MaxLength="24" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                        <label for="txtaddr_state" class="col-sm-3">State:
                        <span class="col-sm-12" style="padding-left: 0px;">
                            <asp:TextBox ID="txtaddr_state" value="" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                        <label for="txtaddr_zip" class="col-sm-3">Zip:
                        <span class="col-sm-12" style="padding-left: 0px;">
                            <asp:TextBox ID="txtaddr_zip" value="" CssClass="DB form-control" MaxLength="10" Style="max-width: 130px;" runat="server"></asp:TextBox>
                        </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class=" col-sm-offset-1 col-sm-9">
                        <label for="ddlGender" class="col-sm-3">Gender:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlGender" CssClass="DB form-control PromptSelection" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                            </span>
                        </label>
                        <label for="dteDOB" class="col-sm-3">Initial Payment Due:
                        <span style="padding-left: 0px;">
                            <span class="input-group date" id="dtpPatientFirstPay">
                                <asp:TextBox ID="dteDOB" CssClass="DB form-control  datePicker" MaxLength="10" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteDOB.ClientID%>').focus()"></span>
                                </span>
                            </span>
                        </span>
                        </label> 
                    </div>
                </div>          
            </div>        
                    
            <div class="col-sm-12 text-center">
                <br />
            </div>
            <div class="col-sm-12 text-center">
                <asp:Button ID="btnSubmit" class="btn btn-sm btn-primary" runat="server" Text="Add New Patient" />
                <asp:Button ID="btnCancelNew" class="btn btn-default" runat="server" Text="Cancel" OnClientClick="CancelVal();"/>
            </div>
            <div class="col-sm-12 text-center">
                <br />
            </div>
        </div>
    </form>

    <!--Modal Patient Add New User Popup-->
    <div class="modal fade" id="divModalAddNew" tabindex="-1" role="dialog" aria-labelledby="divModalContractSelectionLabel" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title"></h4>
                </div>
                <div class="modal-body">
                </div>
                <div class="modal-footer">
                    <div class="btn-group">
                        <button type="button" class="btn btn-primary" id="btnGoToContract" onclick="goToExistingPatient();" data-dismiss="modal">Go To Existing Patient Record</button>
                    </div>
                    <%--<div class="btn-group">
                        <button type="button" class="btn btn-success" id="btnOkay" onclick="keepdata('yes');" data-dismiss="modal">Add New Patient</button>
                    </div>--%>
                    <div class="btn-group">
                        <button type="button" class="btn btn-default" id="btnPopupCancel" onclick="keepdata('no');" data-dismiss="modal">Start Over</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- /.modal -->

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <script type="text/javascript">
        jQuery('.datePicker').datepicker({ format: 'mm/dd/yyyy', autoclose: true, viewMode: 'days' })
            .on('changeDate', function (ev) {
                (ev.viewMode == 'days') ? $(this).datepicker('hide') : '';
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

        function CancelVal() {
            ValidatorEnable(document.getElementById("<%=rfvChartNumber.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=rfvPatientNumber.ClientID%>"), false);
        }

        function saveData(leavePage) {
            if (leavePage == false) {
                setWarningPrompt(false);
            } else {
                setWarningPrompt(true);
            }
        };

        function showEditOptions() {
            jQuery("#divEditOptions").removeClass("hidden");
            jQuery("#divViewOptions").addClass("hidden");
            jQuery("#divViewOnly").addClass("hidden");
        }

        function getPatName(txtBox) {
            var strVal;
            if (txtBox == 'pat') {
                //searching on txtPatientNumber
                strVal = 'pat' + jQuery('#' + '<%=txtPatient_Id.ClientID%>').val();
            } else {
                //searching on txtChartNumber
                strVal = 'cht' + jQuery('#' + '<%=txtChart_Number.ClientID%>').val();
            }
            jQuery.get("ajaxOrtho.aspx?action=getPatName", { id: strVal, frm: "Patients" },
                 function (data) {
                     var strData = data;
                     if (strData.indexOf("~~") == -1) {
                         if (strData.indexOf("<h4>") == 0) {
                             //Display modal for contract selection
                             jQuery('#divModalAddNew').modal('show');
                             jQuery('#loading_indicator').show();
                             jQuery('.modal-body').html(data);
                             jQuery('#loading_indicator').hide();
                         } else {
                             document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData;
                        }
                    } else {
                         //strData = PatientNumber, PatientName, ChartNumber, FirstName, MiddleName, LastName, Address1, Address2, City, State, ZipCode, Gender, DOB
                         document.getElementById('<%=txtPatient_Id.ClientID%>').value = strData.split("~~")[0];
                         document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strData.split("~~")[1];
                         document.getElementById('<%=txtChart_Number.ClientID%>').value = strData.split("~~")[2];
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
                    }
                });
        }

        function goToExistingPatient() {
            strID = jQuery('#intCID')[0].innerHTML;
            window.open("PatientEntry.aspx?cid=" + strID + "&mid=view", "_self");
        }

        function keepdata(obj) {
            strVal = obj;
            var strChtID = jQuery('#intChtID')[0].innerHTML;
            var strPatId = jQuery('#intPatID')[0].innerHTML;
            var strName = jQuery('#strName')[0].innerHTML;

            if (strVal == "yes") {
                document.getElementById('<%=txtPatient_Id.ClientID%>').value = strPatId;
                document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = strName;
                document.getElementById('<%=txtChart_Number.ClientID%>').value = strChtID;
            } else {
                document.getElementById('<%=txtPatient_Id.ClientID%>').value = "";
                document.getElementById('<%=lblPatientName.ClientID%>').innerHTML = "";
                document.getElementById('<%=txtChart_Number.ClientID%>').value = "";
            }
        }

    </script>

    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
</asp:Content>
