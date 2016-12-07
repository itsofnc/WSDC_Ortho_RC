<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintPaymentReceipts.aspx.vb" Inherits="WSDC_Ortho.frmPrintPaymentReceipts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPrintPaymentReceipts' class="form-horizontal formName" role="form" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <asp:Literal ID="litDownload" runat="server"></asp:Literal>
        
        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12">
                <asp:Label ID="lblTitle" runat="server" Text="Print Payment Receipt"></asp:Label></h4>
            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-9">
                        
                        <label for="txtFirstName" class="col-sm-3">First Name:
                            <asp:TextBox ID="txtFirstName" CssClass="DB form-control" onFocus="jQuery('.datePicker').datepicker('hide');"  onChange="getPatName('fst')" Style="max-width: 165px;" runat="server" placeholder="First Name"></asp:TextBox>
                        </label>
                        <label for="txtLastName"  class="col-sm-3">Last Name:
                            <asp:TextBox ID="txtLastName" CssClass="DB form-control" onChange="getPatName('lst')" Style="max-width: 165px;" runat="server" placeholder="Last Name"></asp:TextBox>
                        </label>
                        <label for="txtChartNumber" class="col-sm-3 ">
                            Chart #:
                            <asp:TextBox ID="txtChartNumber" CssClass="form-control"  onChange="getPatName('cht')"  MaxLength="20" onFocus="clearLbl()" Style="max-width: 165px;" runat="server" placeholder="Chart No"></asp:TextBox>
                        </label>
                        <div class="col-sm-12">
                            <br />
                        </div>
                        <label for="txtPayerName" class="col-sm-3">
                            Payer Name:
                        <span style="padding-left: 0px;">
                            <asp:TextBox ID="txtPayerName" CssClass="form-control" MaxLength="10" onFocus="clearLbl()"  runat="server" placeholder="Payer or Insurance"></asp:TextBox>
                        </span>
                        </label>
                        <div class="col-sm-12">
                            <br />
                        </div>
                        <label for="dteBeginDate" class="col-sm-3">
                            Begin Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group input-append date" id="dtpBeginDate">
                                <asp:TextBox ID="dteBeginDate" name="dteBeginDate" CssClass="DB form-control  " MaxLength="200" onFocus="clearLbl()"  runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                <%--<span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteBeginDate.ClientID%>').focus()"></span>
                                </span>--%>
                            </span>
                        </span>
                        </label>
                        <label for="dteEndDate" class="col-sm-3">
                            End Date:
                        <span style="padding-left: 0px;">
                            <span class="input-group date" id="dtpEndDate">
                                <asp:TextBox ID="dteEndDate" name="dteEndDate"  CssClass="DB form-control" MaxLength="10" onFocus="clearLbl()"  runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                                <%--<span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar" onclick="jQuery('#<%= dteEndDate.ClientID%>').focus()"></span>
                                </span>--%>
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
                    <asp:Button ID="btnPrintReceipt" class="btn btn-primary" onFocus="jQuery('.datePicker').datepicker('hide');" runat="server" Text="Print Receipt" />
                    <asp:Button ID="btnCancel" class="btn btn-default" runat="server" Text="Cancel" />
                </span>
            </div>
            <div class="col-sm-12">
                <br />
            </div>
        </div>
        <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="images/loading.gif" style="position:fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>

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
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <script>
        jQuery(document).ready(function () {
            $('.nav li.active').removeClass('active');
            document.getElementById("#Payments").className += " active";
        });
         $('#dtpBeginDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintPaymentReceipts').formValidation('revalidateField', '<%=dteBeginDate.UniqueID%>');
          });

         $('#dtpEndDate')
          .datepicker({
              format: 'mm/dd/yyyy',
              autoclose: true
          })
          .on('changeDate', function (e) {
              // Revalidate the date field
              $('#frmPrintPaymentReceipts').formValidation('revalidateField', '<%=dteEndDate.UniqueID%>');
        });

        $('#frmPrintPaymentReceipts')
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
        //   .on('changeDate', function (ev) {
        //       (ev.viewMode == 'days') ? $(this).datepicker('hide') : '';
        //   });
        function clearLbl() {
            document.getElementById("<%=lblMessage.ClientID%>").innerHTML = "";
        }

        function getPatName(txtBox) {
            var strVal = '';
            if (txtBox == 'fst') {
                //searching on first name
                if (jQuery('#<%=txtFirstName.ClientID%>').val() != '') {
                    strVal = 'fst' + jQuery('#<%=txtFirstName.ClientID%>').val();
                    document.getElementById('<%=txtLastName.ClientID%>').value = "";
                }
            } else if (txtBox == 'lst') {
                //searching on Chart Number
                if (jQuery('#<%=txtLastName.ClientID%>').val() != '') {
                    strVal = 'lst' + jQuery('#<%=txtLastName.ClientID%>').val();
                    document.getElementById('<%=txtFirstName.ClientID%>').value = "";
                }
             } else if (txtBox == 'cht') {
                 //searching on Chart Number
                 if (jQuery('#<%=txtChartNumber.ClientID%>').val() != '') {
                     strVal = 'cht' + jQuery('#<%=txtChartNumber.ClientID%>').val();
                 }
            }
            if (strVal != '') {
                jQuery.post("ajaxOrtho.aspx?action=getPatName", { id: strVal, frm: "PaymentReceipt" },
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
                               }
                           } else {
                               //fill in patient first & last name
                               document.getElementById('<%=txtFirstName.ClientID%>').value = strData.split("~~")[0];
                               document.getElementById('<%=txtLastName.ClientID%>').value = strData.split("~~")[1]
                               document.getElementById('<%=txtChartNumber.ClientID%>').value = strData.split("~~")[2];

                           }
                       });
            }
        }
        function getPatNameCID(obj, txtBox) {
            cid = obj.value;
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
            jQuery('#btnPopupCancel').click();
            jQuery.post("ajaxOrtho.aspx?action=getPatName", { id: strVal, con: cid, frm: "PaymentReceipt" },
                function (data) {
                    var strData = data;
                    if (strData.indexOf("~~") == -1) {
                        if (strData.indexOf("<h4>") == 0) {
                            //Display modal for contract selection
                            jQuery(".modal-title")[0].innerHTML = "Select Contract";
                            jQuery('#divModalContractSelection').modal('show');
                            jQuery('#loading_indicator').show();
                            jQuery('.modal-body').html(data);
                            jQuery('#loading_indicator').hide();
                        } else {
                            //No contract found. Higly unlikely, but just incase

                        }
                    } else {
                        //fill in patient first & last name
                        document.getElementById('<%=txtFirstName.ClientID%>').value = strData.split("~~")[0];
                    document.getElementById('<%=txtLastName.ClientID%>').value = strData.split("~~")[1]
                    document.getElementById('<%=txtChartNumber.ClientID%>').value = strData.split("~~")[2];

                }
            });

        }
    </script>
</asp:Content>
