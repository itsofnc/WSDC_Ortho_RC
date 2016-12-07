<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="frmMessaging.aspx.vb" Inherits="WSDC_Ortho.frmMessaging" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<!--This form is to be used for messaging back to the user-->
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="Swaim Web Portal Online Message Center" />
    <meta name="author" content="ACC Business Solutions Inc." />
    <link rel="shortcut icon" href="../../assets/ico/favicon.ico" />
    <title>
        <asp:Literal ID="litPageTitle" runat="server"></asp:Literal></title>

    <!--bootstrap-->
    <link href="Common/bootstrap-3.3.2/css/bootstrap.min.css" rel="stylesheet" />

    <!--form validation css-->
    <link href="Common/formValidation/css/formValidation.min.css" rel="stylesheet" />
    <!--t3 common code css-->
    <link href="Common/css/siteCommon.css" rel="stylesheet" />

    <link href="Content/css/site.css" rel="stylesheet" />
    <link href="Content/css/stickyFooterNavbar.css" rel="stylesheet" />

    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <link rel="icon" href="/Images/favicon.png" type="image/x-icon" />
        <link rel="shotcut icon" href="/Images/favicon.png" type="image/x-icon" />
        <script src="<%: ResolveUrl("~/Common/js/modernizr-2.6.2.js<script ")%>"></script>
    </asp:PlaceHolder>
</head>
<body>
    <div class="row container center-block" style="padding-top: 40px; text-align:center">
        <form id="frmMessaging" runat="server">
            <div class="container">
                <div id="HTMLMessage" class="row">
                    <h1 style="text-align:center"><asp:Literal ID="litProjectTitle" runat="server"></asp:Literal></h1>
                    <div class="panel-info">
                        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </form>
    </div>
    <footer id="divFooterMain" class="footer">
        <div id="divFooter" class="container">
            <p class="text-muted text-center">
                <asp:Literal ID="litCompanyName" runat="server"></asp:Literal>
            </p>
        </div>
    </footer>

    <script src="Common/js/jquery-1.11.0.js"></script>
    <!--#include file="Common/inc/getSessionTimeout.inc"-->
    <%-- Session Timeout--%>
    <script src="Common/bootstrap-3.3.2/js/bootstrap.min.js"></script>
    <!--For IE 8--->
    <script src="Common/js/respond.js"></script>
    <!-- IE10 viewport hack for Surface/desktop Windows 8 bug -->
    <script src="Common/bootstrap-3.3.2/js/ie10ViewportButWorkaround.js"></script>

    <!-- scripts built from VB -->
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>

</body>
</html>
