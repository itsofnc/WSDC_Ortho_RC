<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="WSDC_Ortho._Default" %>

<!DOCTYPE html>

<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">
    <link rel="shortcut icon" href="../../assets/ico/favicon.ico">
    <title>WSDC-Login</title>

    <!-- Styles -->

    <!-- Bootstrap core CSS -->
    <link href="Common/bootstrap-3.3.2/css/bootstrap.min.css" rel="stylesheet" />
    <%--<link href="Bootstrap/css/bootstrap.min.css" rel="stylesheet" /> CCC--%>

    <!--Custom Style Sheets-->
    <link href="Content/Default.css" rel="stylesheet" />
    <script>
           window.location.hash = "no-back-button";
           window.location.hash = "Again-No-back-button";//again because google chrome don't insert first hash into history
           window.onhashchange = function () { window.location.hash = "no-back-button"; }
    </script>
  </head>
  <body>
    <div class="site-wrapper">
        <div class="site-wrapper-inner">
            <div class="Login login-cover-container">
                <!-- Two columns  -->
                <form id="frmLogin" role="form" runat="server">
                     <div id="divLOGIN"></div>
                    <div id="divJS"></div>
                    <div id="divHiddenItems" class="hidden">
                        <asp:TextBox ID="txtLoginSesPrefix" runat="server"></asp:TextBox>
                        <asp:TextBox ID="txtLoginGetText" runat="server"></asp:TextBox>
                    </div>
                    <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
                        <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
                    </div>
                </form>
            </div>

            <!--Error Message -->
            <asp:Literal ID="litCouldnot" runat="server"></asp:Literal>
        </div>
    </div>

    <script src="Content/js/jquery-1.11.1.js"></script>
    <script src="Common/bootstrap-3.3.2/js/bootstrap.min.js"></script>
    <script type="text/javascript">
        // set blnTimeoutActive = false to prevent session timeout from site master
        jQuery(document).ready(function () {
            blnTimeoutActive = false;

            // load frmLogin embedded
            jQuery('#loading_indicator').show();
            jQuery.get(jQuery('#' + '<%=txtLoginGetText.ClientID%>').val(),
            function(data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();
            });
        })
        function loadDivAreas(data) {
            arrLoginContents = data.split("<!--%%% -->");
            jQuery('#divLOGIN').html(arrLoginContents[1]);
            jQuery('#divJS').html(arrLoginContents[3]);
        }

        function reloadForm() {
            window.open('Default.aspx?sesPrefix=' + jQuery('#' + '<%=txtLoginSesPrefix.ClientID%>').val(), '_self');
        }
    </script>
   
  </body>
</html>