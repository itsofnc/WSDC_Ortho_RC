<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintContractPosting.aspx.vb" Inherits="WSDC_Ortho.frmPrintContractPosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPrintContractPosting' class="form-horizontal" role="form" action="frmPrintContractPosting.aspx" runat="server" defaultbutton="btnHidden">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" class="hidden" runat="server" OnClientClick="return false;" />
        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12"><asp:Label ID="lblTitle" runat="server" Text="Contract Posting Report"></asp:Label></h4>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-9">
                        <label for="dteBeginDate" class="col-sm-3">Begin Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group input-append date" id="dtpBeginDate">
                                <asp:TextBox ID="dteBeginDate"  name="dteBeginDate" CssClass="DB form-control" MaxLength="10" onFocus="clearLbl()"  runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                <%--<span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteBeginDate.ClientID%>').focus()"></span>
                                </span>--%>
                            </span>
                        </span>
                        </label> 
                        <label for="dteEndDate" class="col-sm-3">End Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group input-append date" id="dtpEndDate">
                                <asp:TextBox ID="dteEndDate"  name="dteEndDate" CssClass="DB form-control " MaxLength="10" onFocus="clearLbl()"  runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                               <%-- <span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteEndDate.ClientID%>').focus()"></span>
                                </span>--%>
                            </span>
                        </span>
                        </label> 
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-9">
                        <label for="ddlDoctor" class="col-sm-3">Doctor:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlDoctor" CssClass="DB form-control PromptSelection" onFocus="clearLbl()"  Style="max-width: 165px;" runat="server"></asp:DropDownList>
                            </span>
                        </label>
                        <label for="ddlCpt_Code" class="col-sm-3">Code:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlCpt_Code" CssClass="DB form-control PromptSelection disabled" onFocus="clearLbl()"  Style="max-width: 165px;" runat="server"></asp:DropDownList>
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
                        <asp:Button ID="btnCancel" class="btn btn-default" runat="server" Text="Cancel" OnClientClick="CancelVal();"/>
                    </span>
            </div>
            <div class="col-sm-12">
                <br />
            </div>
        </div>
    </form>
    <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.nav li.active').removeClass('active');
            document.getElementById("#ContractPostingReport").className += " active";
        });

        //jQuery('.datePicker').datepicker({ format: 'mm/dd/yyyy', viewMode: 'days' })
        //    .on('changeDate', function (ev) {
        //        (ev.viewMode == 'days') ? $(this).datepicker('hide') : '';
        //    });

        $('#dtpBeginDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintContractPosting').formValidation('revalidateField', '<%=dteBeginDate.UniqueID%>');
          });

         $('#dtpEndDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintContractPosting').formValidation('revalidateField', '<%=dteEndDate.UniqueID%>');
        });

        $('#frmPrintContractPosting')
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

        function clearLbl() {
            document.getElementById("<%=lblMessage.ClientID%>").innerHTML = "";
        }
        function CancelVal() {
            <%--ValidatorEnable(document.getElementById("<%=rfvBeginDate.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=rfvEndDate.ClientID%>"), false);--%>
        }
    </script>

</asp:Content>
