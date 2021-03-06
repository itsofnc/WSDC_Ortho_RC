﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmClaimsProcessing.aspx.vb" Inherits="WSDC_Ortho.frmClaimsProcessing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
 </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPrintProcessClaims' class="form-horizontal formName" role="form" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <asp:HiddenField ID="hidInitialcontract" runat="server" />
        <asp:HiddenField ID="hidClaimTypeSelected" runat="server" />
        <div class="row">
            <div class="col-md-12 center-block">
                <asp:Label ID="lblMessage" runat="server" class="form-control" Text="Print Invoices & Claims"></asp:Label>
                <br />
            </div>
        </div>
        <div class="row">
            <div class="col-sm-9">
                <div class="col-sm-3">
                    <asp:Button ID="btnPrint" runat="server" Text="Print Patient Invoices"  CssClass="btn btn-sm btn-success" OnClientClick="if (confirm('Warning: processing invoices is irreversible. \nClick OK to continue')) {jQuery('#loading_indicator').show(); queryIframe();} else {return false;}" />
                </div>
                <div class="col-sm-3">
                    <asp:Button ID="btnPreviewInvoice" runat="server" Text="Preview Patient Invoices"  CssClass="btn btn-sm btn-primary" OnClientClick="jQuery('#loading_indicator').show(); queryIframe();" />
                </div>
                <div class="col-sm-3">
                    <asp:Button ID="btnClaimPrimary" runat="server" Text="Print Primary Claims"  CssClass="btn btn-sm btn-success" OnClientClick="if (confirm('Warning: processing claims is irreversible. \nClick OK to continue')) {jQuery('#loading_indicator').show(); queryIframe();} else {return false;}" />
                </div>
                <div class="col-sm-3">
                    <asp:Button ID="btnPreviewClaimPrimary" runat="server" Text="Preview Primary Claims"  CssClass="btn btn-sm btn-primary" OnClientClick="jQuery('#loading_indicator').show(); queryIframe();" />
                </div>
            </div>
        </div>
        <div class="row" style="margin-top: 3px; margin-bottom: 3px">
            <div class="col-sm-9">
                <div class="col-sm-3">
                    <asp:Button ID="btnClaimSecondary" runat="server" Text="Print Secondary Claims"  CssClass="btn btn-sm btn-success" OnClientClick="if (confirm('Warning: processing claims is irreversible. \nClick OK to continue')) {jQuery('#loading_indicator').show(); queryIframe();} else {return false;}" />
                </div>
                <div class="col-sm-3">
                    <asp:Button ID="btnPreviewClaimSecondary" runat="server" Text="Preview Secondary Claims"  CssClass="btn btn-sm btn-primary" OnClientClick="jQuery('#loading_indicator').show(); queryIframe();" />
                </div>
                <div class="col-sm-6" >
                    <asp:Button ID="btnManualClaim" runat="server" Text="Override Billing Date"  CssClass="btn btn-sm btn-success" OnClientClick="getProcessDate();return false" />
                    <span id="spnManDate" style="display: none"></span>
                </div>
            </div>
        </div>
        <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>
    </form>

    <div id="divIframe" style ="width: 100%">
    <iframe id="ifmClaims" src="#" width="900" height="900" style="border: 0px"></iframe>
        </div>
    <%--<div style="display: none">--%>
        <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
    <%--</div>--%>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script type="text/javascript">
        window.onresize = fixIframeSize;
        
        function fixIframeSize() {
            document.getElementById("ifmClaims").width = document.getElementById("divIframe").offsetWidth - 20;
        }

        jQuery(document).ready(fixIframeSize());

        function queryIframe() {
            ifmObj = document.frames.ifmClaims.getAspElement("txtSearch");
        }

        function childLoaded() {
            document.frames.ifmClaims.getAspElement("divHeader").style.display = "none";
        }

        function getProcessDate() {
                pDate = prompt('Enter a claim date (mm/dd/yyyy)', "");
                showProcessDate(pDate);
                jQuery('#loading_indicator').show();
                window.open(document.forms[0].action,"_self");
            } 

        function showProcessDate(pDate) {

            spnDateOvrd = document.getElementById("spnManDate");

            strAction = document.forms[0].action;
            prmLoc = strAction.indexOf("?od=") + 1;

            if (prmLoc > 0) {
                // strip out any previous setting of an override date this session
                strInitAction = strAction.substr(0, prmLoc);
                strEndAction = strAction.substr(strAction.indexOf("&") + 1);
                document.forms[0].action = strInitAction + strEndAction;
            }

            if (pDate === null || pDate.trim == "") {
                spnDateOvrd.innerHTML = '';
                spnDateOvrd.style.display = 'none';
            } else {
                document.forms[0].action = document.forms[0].action.replace(".aspx?", ".aspx?od=" + pDate + "&");
                spnDateOvrd.innerHTML = '<b>Claim/Invoice Date Setting: ' + pDate + '</b>';
                spnDateOvrd.style.display = 'block';
            }
        }

    </script>

    <script>
        jQuery(document).ready(function () {
            jQuery('.nav li.active').removeClass('active');
            try {
                document.getElementById("#" + InvoiceClaim).className += " active";
            }
            catch (e) {
            }
            jQuery(function () {
                jQuery("#myTab a:first").tab('show');
            });
        });

    </script>
</asp:Content>
