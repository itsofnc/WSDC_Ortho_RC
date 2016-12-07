<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="frmLogout.aspx.vb" Inherits="WSDC_Ortho.frmLogout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Logout</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    
    <!--bootstrap-->
    <link href="Common/bootstrap-3.3.2/css/bootstrap.min.css" rel="stylesheet" />
    <!--t3 common code css-->
    <link href="Common/css/siteCommon.css" rel="stylesheet" />

    <!--site customization-->
    <link href="Content/css/site.css" rel="stylesheet" />
    <link href="/Images/favicon.ico" rel="shortcut icon" type="image/x-icon" />

</head>
<body id="sessionTimeoutBody">
    <div class="container">
        <div style="padding-top:50px;"></div>
        <form id='frmLogout' class="form-horizontal" role="form" action="frmLogout.aspx" runat="server" defaultbutton="btnOK">
            <h1 style="text-align:center"><asp:Literal ID="litProjectTitle" runat="server"></asp:Literal></h1>
            <h4 style="text-align: center;"><asp:Literal ID="litMessage" runat="server"></asp:Literal></h4>
            <asp:Button ID="btnOK" type="submit" class="btn btn-success btn-lg center-block" runat="server" Text="OK" />
        </form>
    </div>

    <!--Scripts-->
    <!--For IE 8--->
    <script src="Common/js/respond.js"></script>
    <script>
        window.location.hash = "nbb";
        window.location.hash = "anbb";//again because google chrome don't insert first hash into history
        window.onhashchange = function () { window.location.hash = "nbb"; }
    </script>
</body>
</html>