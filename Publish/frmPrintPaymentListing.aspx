<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.master" CodeBehind="frmPrintPaymentListing.aspx.vb" Inherits="WSDC_Ortho.frmPrintPaymentListing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPrintPaymentListing' class="form-horizontal" role="form" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12">
                <asp:Label ID="lblTitle" runat="server" Text="Payment Listing Report"></asp:Label></h4>

            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-12">
                        <label for="dteBeginDate" class="col-xs-4">
                            Begin Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group date" id="dtpBeginDate">
                                <asp:TextBox ID="dteBeginDate" CssClass="DB form-control datePicker" MaxLength="10" onFocus="clearLbl()" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteBeginDate.ClientID%>').focus()"></span>
                                </span>
                            </span>
                            <p>
                                <asp:RequiredFieldValidator ID="rfvBeginDate" runat="server" ControlToValidate="dteBeginDate" Display="Dynamic" ErrorMessage="Required" class="requiredFieldValidator" />
                            </p>
                        </span>
                        </label>
                        <label for="dteEndDate" class="col-xs-4">
                            End Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group date" id="dtpEndDate">
                                <asp:TextBox ID="dteEndDate" CssClass="DB form-control  datePicker" MaxLength="10" onFocus="clearLbl()" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteEndDate.ClientID%>').focus()"></span>
                                </span>
                            </span>
                            <p>
                                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" Display="Dynamic" ErrorMessage="Required" class="requiredFieldValidator" />
                            </p>
                        </span>
                        </label>
                        <label for="txtChartNo" class="col-xs-4">
                            Chart No:
                            <span style="padding-left: 0px;">
                                <span class="input-group">
                                    <span class="input-group-addon" style="background-color:#d1d1d1">Chart #</span>
                                    <asp:TextBox ID="txtChartNo" CssClass="form-control" runat="server" placeholder="Chart #"></asp:TextBox>
                                </span>
                            </span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <asp:Label ID="lblMessage" runat="server"></asp:Label><br />
            </div>
            <div class="col-sm-12">
                <span style="padding-left: 0px;">
                    <asp:Button ID="btnSubmit" class="btn btn-primary" onFocus="jQuery('.datePicker').datepicker('hide');" runat="server" Text="Print" />
                    <asp:Button ID="btnCancel" class="btn btn-default" runat="server" Text="Cancel" OnClientClick="CancelVal();" />
                </span>
            </div>
            <div class="col-sm-12">
                <br />
            </div>
        </div>
        <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
     <script type="text/javascript">
        $(document).ready(function () {
            $('.nav li.active').removeClass('active');
            document.getElementById("#PaymentListing").className += " active";
        });

        jQuery('.datePicker').datepicker({ format: 'mm/dd/yyyy', viewMode: 'days' })
            .on('changeDate', function (ev) {
                (ev.viewMode == 'days') ? $(this).datepicker('hide') : '';
            });
        function clearLbl() {
            document.getElementById("<%=lblMessage.ClientID%>").innerHTML = "";
        }
        function CancelVal() {
            ValidatorEnable(document.getElementById("<%=rfvBeginDate.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=rfvEndDate.ClientID%>"), false);
        }
    </script>
</asp:Content>
