<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="frmExportFLM.aspx.vb" Inherits="WSDC_Ortho.frmExportFLM" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
        <div id="divExportFLM" runat="server">
            <h2>Please wait...your request is being processed.</h2>  
        </div>
    <div id="loading_indicator" style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">          
        <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>

</body>
</html>
