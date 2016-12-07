<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintPaymentsRecord.aspx.vb" Inherits="WSDC_Ortho.frmPrintPaymentsRecord" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPrintPaymentsRecord' class="form-horizontal" role="form" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12">
                <asp:Label ID="lblTitle" runat="server" Text="Payments Record Report"></asp:Label></h4>

            <div class="col-sm-12">
                <div class="form-group">
                    <label for="dteBeginDate" class="col-sm-4">
                        Begin Date:
                    <span style="padding-left: 0px;">
                        <span class="input-group input-append date" id="dtpBeginDate">
                            <asp:TextBox ID="dteBeginDate" CssClass="DB form-control" MaxLength="10" onFocus="clearLbl()" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                            <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                           <%-- <span class="input-group-addon add-on">
                                <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteBeginDate.ClientID%>').focus()"></span>
                            </span>--%>
                        </span>
                    </span>
                    </label>
                    <label for="dteEndDate" class="col-sm-4">
                        End Date:
                    <span style="padding-left: 0px;">
                        <span class="input-group input-append date" id="dtpEndDate">
                            <asp:TextBox ID="dteEndDate" CssClass="DB form-control " MaxLength="10" onFocus="clearLbl()" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                            <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                            <%--<span class="input-group-addon">
                                <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteEndDate.ClientID%>').focus()"></span>
                            </span>--%>
                        </span>
                    </span>
                    </label>

                    <label for="ddlUsers" class="col-sm-4">
                        User: 
                        <span style="padding-left: 0px;">
                            <asp:DropDownList ID="ddlUsers" CssClass="DB form-control PromptSelection" onFocus="clearLbl();jQuery('.datePicker').datepicker('hide');" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                        </span>
                    </label>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <label for="ddlPaymentType" class="col-sm-4">
                        Payment Type: 
                        <span style="padding-left: 0px;">
                            <asp:DropDownList ID="ddlPaymentType" CssClass="DB form-control PromptSelection" onFocus="clearLbl();jQuery('.datePicker').datepicker('hide');" Style="max-width: 165px;" runat="server"></asp:DropDownList>
                        </span>
                    </label>
                </div>
            </div>
            <div class="col-sm-12">
                <label for="rdoReportType" class="col-xs-4">
                    Report Type:
                    <asp:RadioButtonList ID="rdoReportType" RepeatDirection="Horizontal" runat="server">
                        <asp:ListItem Text=" Detail " style="margin-left: 12px;" Value="Detail" Selected="True"></asp:ListItem>
                        <asp:ListItem Text=" Summary " style="margin-left: 12px;" Value="Summary"></asp:ListItem>
                    </asp:RadioButtonList>
                </label>
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
            document.getElementById("#PaymentsRecord").className += " active";
        });

          $('#dtpBeginDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintPaymentsRecord').formValidation('revalidateField', '<%=dteBeginDate.UniqueID%>');
          });

         $('#dtpEndDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintPaymentsRecord').formValidation('revalidateField', '<%=dteEndDate.UniqueID%>');
        });

        $('#frmPrintPaymentsRecord')
           .formValidation({
               framework: 'bootstrap',
               icon: {
                   valid: 'glyphicon glyphicon-ok',
                   invalid: 'glyphicon glyphicon-remove',
                   validating: 'glyphicon glyphicon-refresh'
               },
               fields: {
                   <%=dteBeginDate.UniqueID%>: {
                       row: '.col-sm-4',
                       validators: {
                           notEmpty: {
                               message: 'The begin date is required'
                           },
                           date: {
                               format: 'MM/DD/YYYY',
                               message: 'The date is not a valid'
                           }
                       }
                   },
                    <%=dteEndDate.UniqueID%>: {
                        row: '.col-sm-4',
                       validators: {
                           notEmpty: {
                               message: 'The end date is required'
                           },
                           date: {
                               format: 'MM/DD/YYYY',
                               message: 'The date is not a valid'
                           }
                       }
                   }
               }
           });


        //jQuery('.datePicker').datepicker({ format: 'mm/dd/yyyy', viewMode: 'days' })
        //    .on('changeDate', function (ev) {
        //        (ev.viewMode == 'days') ? $(this).datepicker('hide') : '';
        //    });
        function clearLbl() {
            document.getElementById("<%=lblMessage.ClientID%>").innerHTML = "";
        }
        function CancelVal() {
            <%--ValidatorEnable(document.getElementById("<%=rfvBeginDate.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=rfvEndDate.ClientID%>"), false);--%>
        }
    </script>
</asp:Content>
