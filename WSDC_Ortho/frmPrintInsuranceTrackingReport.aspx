<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintInsuranceTrackingReport.aspx.vb" Inherits="WSDC_Ortho.frmPrintInsuranceTrackingReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<form id='frmPrintInsuranceTrackingReport' class="form-horizontal" role="form" action="frmPrintInsuranceTrackingReport.aspx" runat="server" defaultbutton="btnHidden">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" class="hidden" runat="server" OnClientClick="return false;" />
        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12"><asp:Label ID="lblTitle" runat="server" Text="Insurance Tracking Report"></asp:Label></h4>
            
            <div class="col-sm-12">
                <div class="form-group">
                    <div class="col-sm-9">
                        <label for="ddlInsurancePlan" class="col-sm-6">Insurance Plan:
                            <span style="padding-left: 0px;">
                                <asp:DropDownList ID="ddlInsurancePlan" CssClass="DB form-control PromptSelection" onFocus="clearLbl()"  Style="max-width: 165px;" runat="server"></asp:DropDownList>
                            </span>
                        </label>                    
                    </div>
                </div>          
            </div>    
            <div class="col-sm-12">
                <asp:Label ID="lblMessage" runat="server" style="color: #f00;"></asp:Label><br />
            </div>
            <div class="col-sm-12">
                    <span style="padding-left: 0px;">
                        <asp:Button ID="btnPrint" class="btn btn-primary" runat="server" Text="Print" />
                        <asp:Button ID="btnCancel" class="btn btn-default" runat="server" Text="Cancel"/>
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
            document.getElementById("#InsuranceTracking").className += " active";
        });

        function clearLbl() {
            document.getElementById("<%=lblMessage.ClientID%>").innerHTML = "";
        }
        
    </script>

</asp:Content>