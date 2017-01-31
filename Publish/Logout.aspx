<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Logout.aspx.vb" Inherits="WSDC_Ortho.Logout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<!--<body onload="noBack();" onpageshow="if (event.persisted) noBack();" onunload="">
        <script type="text/javascript">
            window.history.forward();
            function noBack() { window.history.forward(); }
        </script>-->
<body>
    <!--<script type="text/javascript">
        jQuery(document).ready(function () {
            var blnState = { coo: "var" };
            history.pushState(blnState, "", "try.html");
        })
    </script>-->
    <div class="site-wrapper">
        <div class="site-wrapper-inner">
            <div class="Login login-cover-container">
                <!-- Two columns  -->
                <form id="frmLogin" role="form" runat="server">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <h1 id="logoutHeading" class="headerText">Ortho Claims Logout</h1>
                        </div>
                    </div>
                    <div class="row">
                        <div class="login-container col-xs-offset-3 col-xs-6 col-sm-offset-3 col-sm-6 col-md-offset-3 col-md-6 col-lg-offset-3 col-lg-6 " style="display: table; vertical-align: middle;">
                           You have been logged out.
                        </div>
                    </div>
                    <div class="row">
                        <asp:Button ID="btnLogin" type="submit" class="btn btn-primary btn-lg center-block" runat="server" Text="Login" PostBackUrl="~/Default.aspx" />
                    </div>
                    <!-- /.row -->
                </form>
            </div>
        </div>
    </div>
    <!--<script type="text/javascript">
        window.onload = function () {
            window.open('Default.aspx', '_self');
        }
    </script>-->
</body>
</html>
