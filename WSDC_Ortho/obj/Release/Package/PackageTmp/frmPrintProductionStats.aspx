<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintProductionStats.aspx.vb" Inherits="WSDC_Ortho.frmPrintProductionStats" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<form id='frmPrintProductionStats' class="form-horizontal" role="form" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12">
                <asp:Label ID="lblTitle" runat="server" Text="Monthly Group Summary"></asp:Label></h4>

            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-9">
                          <label for="dteBeginDate" class="col-sm-3">Begin Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group date" id="dtpBeginDate">
                                <asp:TextBox ID="dteBeginDate"  name="dteBeginDate"  CssClass="DB form-control" MaxLength="10" onFocus="clearLbl()"  runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                            </span>
                        </span>
                        </label> 
                        <label for="dteEndDate" class="col-sm-3">End Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group date" id="dtpEndDate">
                                <asp:TextBox ID="dteEndDate"  name="dteEndDate"  CssClass="DB form-control" MaxLength="10" onFocus="clearLbl()"  runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                            </span>
                        </span>
                        </label> 
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-9">
                        <label for="ddlDoctors" class="col-sm-6">Doctor:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlDoctors" CssClass="DB form-control PromptSelection" onFocus="clearLbl()"  Style="max-width: 165px;" runat="server"></asp:DropDownList>
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
                    <asp:Button ID="btnSubmit" class="btn btn-primary" runat="server" Text="Print Report" />
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
            document.getElementById("#ProductionStats").className += " active";
        });

         $('#dtpBeginDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintProductionStats').formValidation('revalidateField', '<%=dteBeginDate.UniqueID%>');
          });

         $('#dtpEndDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintProductionStats').formValidation('revalidateField', '<%=dteEndDate.UniqueID%>');
        });

        $('#frmPrintProductionStats')
           .formValidation({
               framework: 'bootstrap',
               icon: {
                   valid: 'glyphicon glyphicon-ok',
                   invalid: 'glyphicon glyphicon-remove',
                   validating: 'glyphicon glyphicon-refresh'
               },
               fields: {
                   <%=dteBeginDate.UniqueID%>: {
                       row: '.col-sm-3',
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
                        row: '.col-sm-3',
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