<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmInvoicesReprint.aspx.vb" Inherits="WSDC_Ortho.frmInvoicesReprint" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server"> 
    <form id='frmPrintInvoicesReprint' class="form-horizontal formName" role="form" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12">
                <asp:Label ID="lblTitle" runat="server" Text="Re-Print Invoices"></asp:Label></h4>
            <h5 class="col-sm-12">
                <asp:Label ID="lblInfo" runat="server" Text="Enter a single invoice number or multiple invoice numbers separated by commas"></asp:Label></h5>
            <div class="col-sm-12">
                <div class="form-group">
                    <asp:TextBox ID="txtInvoiceNumbers" CssClass="form-control" runat="server" placeHolder="Enter Invoice #"></asp:TextBox>     
                    
                    <%--NEED INVOICE LIST DDL & SEARCH BY PATIENT NAME/CHART#--%>
                    <%--OR - DISPLAY IN GRIDLIST AND CHECK WHICH TO PRINT--%>
                    <%--OPTION TO PRINT COMBINED ON 1 STATEMENT OR INDIVIDUALLY--%>
                    <%--NEED STATEMENT REPORT LIKE INVOICE FOR COMBINED OPTION (NOT HISTORY)--%>
                                      
                </div>
            </div>
            <div class="col-sm-12">
                <asp:Label ID="lblMessage" runat="server"></asp:Label><br />
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
    <%--<div style="display: none">--%>
    <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
    <%--</div>--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <script type="text/javascript">
        jQuery(document).ready(function () {
            jQuery("<%= txtInvoiceNumbers.ClientID%>").focus();
        });
    </script>
</asp:Content>
