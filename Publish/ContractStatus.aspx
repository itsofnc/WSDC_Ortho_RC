<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="ContractStatus.aspx.vb" Inherits="WSDC_Ortho.ContractStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form runat="server">        
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />
        <asp:HiddenField ID="txtSessions" runat="server" />
        <h3 class="col-md-12 text-center">Contract Status</h3>

        <h4 class="col-md-12 text-center">
            <asp:Literal ID="litPatientHeader" runat="server"></asp:Literal>
        </h4>
        
        <div class="row">
            <div class="col-lg-12" style="text-align: center;">
                <div class="form-inline">
                    
                    <div class="form-group">
                        <label class="sr-only" for="exampleInputAmount">Last Name</label>
                        <div class="input-group">
                            <div class="input-group-addon" style="background-color:#d1d1d1">Last Name</div>
                            <asp:TextBox ID="txtLastNameSearch" CssClass="form-control"  runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="sr-only" for="exampleInputAmount">Account #</label>
                        <div class="input-group">
                            <div class="input-group-addon" style="background-color:#d1d1d1">Account #</div>
                            <asp:TextBox ID="txtAccountNumSearch" CssClass="form-control" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="sr-only" for="exampleInputAmount">Chart #</label>
                        <div class="input-group">
                            <div class="input-group-addon" style="background-color:#d1d1d1">Chart #</div>
                            <asp:TextBox ID="txtChartNumSearch" CssClass="form-control"  runat="server" />
                        </div>
                    </div>
                    <asp:Button ID="btnSearch" CssClass="btn btn-link" runat="server" Text="Search" />
                    <asp:Button ID="btnClear" CssClass="btn btn-link" runat="server" Text="Clear" />
                   
                </div>
            </div>
        </div>
        <div id="divLinks" class="row hidden">
        <%--<asp:Button ID="btnContract" CssClass="btn btn-link" runat="server" Text="Go To Contract" />
            <asp:Button ID="btnInvoices" CssClass="btn btn-link" runat="server" Text="View Invoices" />
            <asp:Button ID="btnClaims" CssClass="btn btn-link" runat="server" Text="View Claims" />
            <asp:Button ID="btnPayments" CssClass="btn btn-link" runat="server" Text="View Payments" />--%>
        </div>
        <div class="container">
            <div class="col-lg-12">
                <br />
            </div>
            <div class="col-lg-12 table-responsive">
                <table class="table">
                    <thead>
                        <tr style="background-color: #3e3e3e; color: white;">
                            <th>&nbsp;</th>
                            <th>Patient</th>
                            <th>&nbsp;</th>
                            <th>Primary</th>
                            <th>&nbsp;</th>
                            <th>Secondary</th>
                            <th>&nbsp;</th>
                        </tr>
                        <tr style="background-color: #4e4e4e; color: white;">
                            <th>&nbsp;</th>
                            <th><span style="text-decoration: underline">MO's</span></th>
                            <th><span style="text-decoration: underline">Amount</span></th>
                            <th><span style="text-decoration: underline">MO's</span></th>
                            <th><span style="text-decoration: underline">Amount</span></th>
                            <th><span style="text-decoration: underline">MO's</span></th>
                            <th><span style="text-decoration: underline">Amount</span></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th>Inital:</th>
                            <td></td>
                            <td><asp:Label ID="lblInitialPatAmount" runat="server" Text=""></asp:Label></td>
                            
                            <td></td>
                            <td>
                                <asp:Label ID="lblInitialPrimaryAmount" runat="server" Text=""></asp:Label></td>
                            <td></td>
                            <td>
                                <asp:Label ID="lblInitialSecondaryAmount" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr style="background-color: #d1d1d1">
                            <th>Contract:</th>
                            <td>
                                <asp:Label ID="lblContractMonths" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblContractPatAmount" runat="server" Text=""></asp:Label></td>
                            <td></td>
                            <td>
                                <asp:Label ID="lblContractPrimaryAmount" runat="server" Text=""></asp:Label></td>
                            <td></td>
                            <td>
                                <asp:Label ID="lblContractSecondaryAmount" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <th>Billed:</th>
                            <td>
                                <asp:Label ID="lblBilledMonthsPatient" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblBilledPatAmount" runat="server" Text=""></asp:Label></td>
                            
                            <td>
                                <asp:Label ID="lblBilledMonthsPrimary" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblBilledPrimaryAmount" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblBilledMonthsSecondary" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblBilledSecondaryAmount" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr style="background-color: #d1d1d1">
                            <th>Remaining:</th>
                            <td>
                                <asp:Label ID="lblRemainingMonths" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblRemainingPatAmount" runat="server" Text=""></asp:Label></td>
                           
                            <td></td>
                            <td>
                                <asp:Label ID="lblRemainingPrimaryAmount" runat="server" Text=""></asp:Label></td>
                            <td></td>
                            <td>
                                <asp:Label ID="lblRemainingSecondaryAmount" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <th>Credit:</th>
                            <td>
                                <asp:Label ID="lblCreditMonths" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblCreditPatAmount" runat="server" Text="" ForeColor="Red"></asp:Label></td>
                           
                            <td></td>
                            <td>
                                <asp:Label ID="lblCreditPrimaryAmount" runat="server" Text="" ForeColor="Red"></asp:Label></td>
                            <td></td>
                            <td>
                                <asp:Label ID="lblCreditSecondaryAmount" runat="server" Text="" ForeColor="Red"></asp:Label></td>
                        </tr>
                        <tr style="background-color: #d1d1d1">
                            <th>Last Post:</th>
                            <td><asp:Label ID="lblPatientLastPostDate" runat="server" Text=""></asp:Label></td>
                            <td></td>
                            <td></td>
                            <td><asp:Label ID="lblPrimaryLastPostDate" runat="server" Text=""></asp:Label></td>
                            <td></td>
                            <td><asp:Label ID="lblSecondaryLastPostDate" runat="server" Text=""></asp:Label></td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="col-lg-12">
                <br />
            </div>
            <div class="col-lg-12 table-responsive">
                <table class="table">
                    <thead>
                        <tr style="background-color: #4e4e4e; color: white;">
                            <th>&nbsp;</th>
                            <th><span style="text-decoration: underline">Balance</span></th>
                            <th><span style="text-decoration: underline">0-30</span></th>
                            <th><span style="text-decoration: underline">31-60</span></th>
                            <th><span style="text-decoration: underline">61-90</span></th>
                            <th><span style="text-decoration: underline">Over 90</span></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th>Account:</th>
                            <td><asp:Label ID="lblAccountBalance" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblAccount0to30" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblAccount31to60" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblAccount61to90" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblAccountOver90" runat="server" Text="Label"></asp:Label></td>
                        </tr>
                        <tr style="background-color: #d1d1d1">
                            <th>Insurance:</th>
                            <td>
                                <asp:Label ID="lblInsuranceBalance" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblInsurance0to30" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblInsurance31to60" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblInsurance61to90" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblInsuranceOver90" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr>
                            <th>Pat. Resp:</th>
                            <td>
                                <asp:Label ID="lblPatientBalance" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblPatient0to30" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblPatient31to60" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblPatient61to90" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblPatientOver90" runat="server" Text=""></asp:Label></td>
                        </tr>
                        <tr style="background-color: #d1d1d1">
                            <th>Undist:</th>
                            <td>
                                <asp:Label ID="lblUndistBalance" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblUndist0to30" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblUndist31to60" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblUndist61to90" runat="server" Text=""></asp:Label></td>
                            <td>
                                <asp:Label ID="lblUndistOver90" runat="server" Text=""></asp:Label></td>
                        </tr>
                    </tbody>
                </table>
                <h4><asp:Label ID="lblPayoffBalance" runat="server" Text=""></asp:Label></h4>
            </div>
        </div>
        <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>
    </form>

    <!--Modal Contract Selection Popup-->
    <div class="modal fade" id="divModalContractSelection" tabindex="-1" role="dialog" aria-labelledby="divModalContractSelectionLabel" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <h5>Please select a patient.</h5>
                    <h5 class="sr-only"> Please select a patient.</h5>
                    <select id="intAccountId" onchange="submitChart()" class="form-control" style="width:150px">
                        <option value="-1">Choose one</option>
                        <asp:Literal ID="litOptions" runat="server"></asp:Literal>
                    </select>
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
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
        <asp:Literal ID="litScripts" runat="server"></asp:Literal>

    <script>
        jQuery(document).ready(function () {
            jQuery('.nav li.active').removeClass('active');
            document.getElementById("#ContractStatus").className += " active";
        });
        function submitChart() {
            var intAccountNumber = jQuery("#intAccountId").val();
            jQuery("#<%=txtChartNumSearch.ClientID%>").val('');
            jQuery("#<%=txtAccountNumSearch.ClientID%>").val(intAccountNumber);
            jQuery("#<%=txtLastNameSearch.ClientID%>").val('');
            jQuery("#<%=btnSearch.ClientID%>").click();
            jQuery('#divModalContractSelection').modal('hide');
        }
    </script>
</asp:Content>
