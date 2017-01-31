<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="Dashboard.aspx.vb" Inherits="WSDC_Ortho.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <!--#include file="Common/inc/frmListManagerCSS.inc"-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="frmEmbeddedFLM" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />
        <div style="padding-top: 25px; padding-bottom:25px;">            
            <div id="dashAdmin" >
                <br />
                <div class="row">
                    <div class="col-xs-2">
                        <img src="images/patient-portal.jpg" class="img-responsive" />
                    </div>
                    <div class="col-xs-9">
                        <a href="frmListManager.aspx?id=Contracts_vw&add=ContractEntry.aspx&perm=1101">
                            <img src="images/viewContracts.png" /></a>
                        &nbsp;&nbsp;&nbsp;
                        <a href="frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&add=PaymentPosting.aspx&vo=true&perm=1001">
                            <img src="images/receivePayments.png"  /></a>
                        &nbsp;&nbsp;&nbsp;
                        <a href="frmClaimsProcessing.aspx?i=1">
                            <img src="images/processInvoices.png" /></a>                    
                        &nbsp;&nbsp;&nbsp;
                        <a href="frmClaimsProcessing.aspx?c=1">
                            <img src="images/PrimaryClaims.png"/></a>
                        &nbsp;&nbsp;&nbsp;
                        <a href="frmClaimsProcessing.aspx?c=2">
                            <img src="images/SecondaryClaims.png"/></a>
                    </div>
                </div>
            </div>
            <div id="dashUser" >
                <div class="col-xs-2">
                        <img src="images/patient-portal.jpg" class="img-responsive"/>
                    </div>
                <div class="col-xs-10">
                    <a href="frmListManager.aspx?id=Contracts_vw&vo=true&perm=1001">
                        <img src="images/viewContracts.png"/></a>
                    &nbsp;&nbsp;&nbsp;
                    <a href="frmListManager.aspx?id=PaymentsPostedWithLinkToReverse_vw&add=PaymentPosting.aspx&vo=true&perm=1001">
                        <img src="images/receivePayments.png"/></a>
                </div>
            </div>             
        </div>    
        <div id="divFLM"></div>
        <div id="divJS"></div>
        <div id="divHiddenItems" class="hidden">
            <asp:TextBox ID="txtFLMSesPrefix" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtFLMGetText" runat="server"></asp:TextBox>
        </div>
        <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script>
        jQuery(document).ready(function () {

            jQuery('#loading_indicator').show();
            jQuery.get(jQuery('#' + '<%=txtFLMGetText.ClientID%>').val(),
            function(data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();
                resize();
            });
        })

        function loadDivAreas(data) {
            arrFLMContents = data.split("<!--%%% -->");
            jQuery('#divFLM').html(arrFLMContents[1]);
            jQuery('#divJS').html(arrFLMContents[3]);
        }

        function reloadForm() {
            window.open('frmEmbeddedFLM.aspx?sesPrefix=' + jQuery('#' + '<%=txtFLMSesPrefix.ClientID%>').val(), '_self');
        }
    </script>
</asp:Content>
