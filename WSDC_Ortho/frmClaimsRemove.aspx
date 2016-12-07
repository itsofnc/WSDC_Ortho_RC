<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmClaimsRemove.aspx.vb" Inherits="WSDC_Ortho.frmClaimsRemove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
        <form id='frmPrintClaimsRemove' class="form-horizontal formName" role="form" runat="server">
            <%--hidden default button so enter key doesn't submit the form--%>
            <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

            Enter list of Claims Numbers to remove:
            <asp:TextBox ID="txtClaimNumbers" runat="server"></asp:TextBox>
            <asp:Button ID="btnSubmit" runat="server" Text="Remove" />
            </form>
        <%--<div style="display: none">--%>
        <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
    <%--</div>--%>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
</asp:Content>