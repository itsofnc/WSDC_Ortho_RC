<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPrintOpenReceivables.aspx.vb" Inherits="WSDC_Ortho.frmPrintOpenReceivables" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<form id='frmPrintOpenReceivables' class="form-horizontal" role="form" action="frmPrintOpenReceivables.aspx" runat="server" defaultbutton="btnHidden">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" class="hidden" runat="server" OnClientClick="return false;" />
        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12"><asp:Label ID="lblTitle" runat="server" Text="Open Receivables by Patient Report"></asp:Label></h4>
            
             <div class="col-sm-9">
                <div class="form-inline">
                    <div class="form-group col-sm-3">
                        <label class="sr-only" for="txtChartNo">Chart #</label>
                        <div class="input-group">
                            <div class="input-group-addon" style="background-color:#d1d1d1">Chart #</div>
                            <asp:TextBox ID="txtChartNo" CssClass="form-control" runat="server" onChange="getPatName();" placeholder="Chart #"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group col-sm-6">
                        <label class="sr-only" for="ddlPatients">Patient Name</label>
                        <div class="input-group">
                            <div class="input-group-addon" style="background-color:#d1d1d1">Patient:</div>
                            <asp:DropDownList ID="ddlPatients" CssClass="DB form-control PromptSelection" onFocus="clearLbl()" runat="server"></asp:DropDownList>
                        </div>
                    </div>  
                </div>
            </div>

            <div class="col-sm-9">
                <br />
                <div class="form-inline">                    
                    <div class="form-group col-sm-6">
                        <label class="sr-only" for="ddlPatients">Provider</label>
                        <div class="input-group">
                            <div class="input-group-addon" style="background-color:#d1d1d1">Provider:</div>
                            <asp:DropDownList ID="ddlProviders" CssClass="DB form-control PromptSelection" onFocus="clearLbl()" runat="server"></asp:DropDownList>
                        </div>
                    </div>  
                </div>
            </div>
            <div class="col-sm-12">
                <asp:Button ID="btnSearch" CssClass="btn btn-sm btn-primary col-sm-2 hidden" runat="server" Text="Search"  />
            </div>
            <div class="col-sm-12">
                <br />
            </div>
            <div class="col-sm-12">
                <asp:Label ID="lblMessage" runat="server" style="color: #f00;"></asp:Label><br />
            </div>
            <div class="col-sm-12">
                    <span style="padding-left: 0px;">
                        <asp:Button ID="btnPrint" class="btn btn-primary" runat="server" Text="Print" />
                        <asp:Button ID="btnCancel" class="btn btn-default" runat="server" Text="Cancel"/>
                    </span>
            </div>
            <div class="col-sm-12">
                <br />
            </div>
        </div>
    </form>
    <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.nav li.active').removeClass('active');
            document.getElementById("#OpenRecvRpt").className += " active";
        });

        function clearLbl() {
            document.getElementById("<%=lblMessage.ClientID%>").innerHTML = "";
        }
        
        //function to add ddl selection to txtbox
        function addChangeToTxt() {
            var selectedItem = $('#<%=ddlPatients.ClientID%> option:selected').text();
            var selectedValue = $('#<%=ddlPatients.ClientID%> option:selected').val();
            if (selectedItem == 'Choose an option') {
                document.getElementById('<%=txtChartNo.ClientID%>').value = '';
            } else {
                document.getElementById('<%=txtChartNo.ClientID%>').value = selectedValue;
            }
        }
    </script>

</asp:Content>
