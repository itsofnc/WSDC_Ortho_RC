<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReportViewer.aspx.vb" Inherits="WSDC_Ortho.ReportViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<%@ Register Assembly="CrystalDecisions.Shared, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    
    <form id="form1" runat="server">
    <div><CR:CrystalReportViewer ID="crvReport" runat="server" 
             AutoDataBind="true" DisplayStatusbar="False" 
            DisplayToolbar="True" 
            HasCrystalLogo="False" 
            HasDrilldownTabs="True" 
            HasDrillUpButton="True" 
            HasExportButton="True" 
            HasGotoPageButton="True" 
            HasPrintButton="True" HasRefreshButton="True" 
             HasToggleGroupTreeButton="True" 
            HasToggleParameterPanelButton="False" 
            HasZoomFactorList="False" 
            GroupTreeStyle-ShowLines="True" 
            ToolPanelView="GroupTree"       
            />        
    </div>
    </form>
</body>

</html>
