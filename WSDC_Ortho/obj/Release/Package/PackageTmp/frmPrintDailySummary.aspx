<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintDailySummary.aspx.vb" Inherits="WSDC_Ortho.frmPrintDailySummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<form id='frmPrintDailySummary' class="form-horizontal" role="form" method="post" runat="server" >
      <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12"><asp:Label ID="lblTitle" runat="server" Text="Summary of Daily Totals"></asp:Label></h4>
            <div class="form-group">
                <label class="col-xs-2 control-label">Begin Date</label>
                <div class="col-xs-3 dateContainer">
                    <div class="input-group input-append date" id="divBeginDate">
                        <asp:TextBox ID="dteBeginDate" name="dteBeginDate" CssClass="form-control" runat="server" ></asp:TextBox>
                        <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                    </div>
                </div>
            </div>
            <div class="form-group">                   
                <label class="col-xs-2 control-label">End Date</label>
                <div class="col-xs-3 dateContainer">
                    <div class="input-group input-append date" id="divEndDate">
                        <asp:TextBox ID="dteEndDate" name="dteEndDate" CssClass="form-control" runat="server" ></asp:TextBox>
                        <span class="input-group-addon add-on"><span class="glyphicon glyphicon-calendar"></span></span>
                    </div>
                </div>
            </div>
            <div class="col-sm-12 form-group">
                <div class="col-xs-5 col-xs-offset-2">
                    <asp:Label ID="lblMessage" class="control-label" runat="server"></asp:Label><br />
                </div>
            </div>
            <div class="col-sm-12 form-group">
                <div class="col-xs-3 col-xs-offset-2">
                    <asp:Button ID="btnSubmit" class="btn btn-primary" runat="server" Text="Print" />
                </div>
                <div class="col-sm-12">
                    <br />
                </div>
            </div>
        </div>
    </form>
    <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <script type="text/javascript">
        
        jQuery(document).ready(function () {
            jQuery('.nav li.active').removeClass('active');
            document.getElementById("#DailyTotals").className += " active";

            jQuery('#divBeginDate')
                .datepicker({
                    format: 'mm/dd/yyyy'
                })
                .on('changeDate', function (e) {
                    // Revalidate the start date field
                    jQuery('#frmPrintDailySummary').formValidation('revalidateField', '<%=dteBeginDate.UniqueID%>');
                });

            jQuery('#divEndDate')
                .datepicker({
                    format: 'mm/dd/yyyy'
                })
                .on('changeDate', function (e) {
                    jQuery('#frmPrintDailySummary').formValidation('revalidateField', '<%=dteEndDate.UniqueID%>');
                });

            jQuery('#frmPrintDailySummary')
                .formValidation({
                    framework: 'bootstrap',
                    icon: {
                        valid: 'glyphicon glyphicon-ok',
                        invalid: 'glyphicon glyphicon-remove',
                        validating: 'glyphicon glyphicon-refresh'
                    },
                    fields: {                        
                        <%=dteBeginDate.UniqueID%>: {
                            validators: {
                                notEmpty: {
                                    message: 'The begin date is required'
                                },
                                date: {
                                    format: 'MM/DD/YYYY',
                                    max: '<%=dteEndDate.UniqueID%>',
                                    message: 'The begin date is not a valid'
                                }
                            }
                        },
                        <%=dteEndDate.UniqueID%>: {
                            validators: {
                                notEmpty: {
                                    message: 'The end date is required'
                                },
                                date: {
                                    format: 'MM/DD/YYYY',
                                    min: '<%=dteBeginDate.UniqueID%>',
                                    message: 'The end date is not a valid'
                                }
                            }
                        }
                    }
                })
                .on('success.field.fv', function (e, data) {
                    if (data.field === '<%=dteBeginDate.UniqueID%>' && !data.fv.isValidField('<%=dteEndDate.UniqueID%>')) {
                        // We need to revalidate the end date
                        data.fv.revalidateField('<%=dteEndDate.UniqueID%>');
                    }

                    if (data.field === '<%=dteEndDate.UniqueID%>' && !data.fv.isValidField('<%=dteBeginDate.UniqueID%>')) {
                        // We need to revalidate the start date
                        data.fv.revalidateField('<%=dteBeginDate.UniqueID%>');
                    }
                });
        });

        function clearLbl() {
            document.getElementById('<%=lblMessage.ClientID%>').innerText= "";                
        }

    </script>

</asp:Content>
