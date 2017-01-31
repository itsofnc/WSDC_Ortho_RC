<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmIFrame.aspx.vb" Inherits="WSDC_Ortho.frmIFrame" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <!--#include file="Common/inc/frmListManagerCSS.inc"-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Your parent form must have a form tag with id=frmListManager--%>
    <form id='frmListManager' class="form-horizontal" role="form" runat="server" >
        <!--#include file="Common/inc/frmListManagerModals.inc"-->   <%-- pulls in Modal divModalAddUpdate--%>
        </form>
    <%--in FLM url string, divHide=... hides elements based on your project site master layout--%>
    <iframe id="ifrTest" style="height: 700px; border:solid; border-color:red; z-index: 100; width: 90%;" src="frmListManager.aspx?id=DevelopmentNotes&pre=1&divHide=divHeader,divFooter" ></iframe> <%--&sesPrefix=--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
<!--must include frmListManager to use edit/view/delete in frmListManager-->
    <!--#include file="Common/inc/frmListManagerScripts.inc"-->
    <script>
        function childLoaded() {
        //possibly use childLoaded for multiple iFrames on a page and keeping unique sessions for each of them
        }
    </script>
</asp:Content>