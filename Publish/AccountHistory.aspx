<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="AccountHistory.aspx.vb" Inherits="WSDC_Ortho.AccountHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form class="form-horizontal" role="form" defaultbutton="btnHidden" action="AccountHistory.aspx" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
</asp:Content>
