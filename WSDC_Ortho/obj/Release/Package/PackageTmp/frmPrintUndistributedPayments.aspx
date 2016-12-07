<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintUndistributedPayments.aspx.vb" Inherits="WSDC_Ortho.frmPrintUndistributedPayments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<form id='frmPrintUndistributedPayments' class="form-horizontal" role="form" action="frmPrintUndistributedPayments.aspx" runat="server" defaultbutton="btnHidden">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" class="hidden" runat="server" OnClientClick="return false;" />
        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12"><asp:Label ID="lblTitle" runat="server" Text="Undistributed Payments Report"></asp:Label></h4>
            <div class="col-sm-12">
                <asp:Label ID="lblMessage" runat="server" style="color: #f00;"></asp:Label><br />
            </div>
            <div class="col-sm-12">
                    <span style="padding-left: 0px;">
                        <asp:Button ID="btnSubmit" class="btn btn-primary" runat="server" Text="Print" />
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
            document.getElementById("#UndistributedPayments").className += " active";
        });

        
        function clearLbl() {
            document.getElementById("<%=lblMessage.ClientID%>").innerHTML = "";
        }

    </script>

</asp:Content>