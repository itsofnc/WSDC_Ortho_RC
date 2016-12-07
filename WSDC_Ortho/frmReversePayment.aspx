<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmReversePayment.aspx.vb" Inherits="WSDC_Ortho.frmReversePayment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmReversePayment' class="form-horizontal" role="form" defaultbutton="btnHidden" action="frmReversePayment.aspx" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <asp:HiddenField ID="txtPaymentID" runat="server" />
        <asp:HiddenField ID="txtPaymentType" runat="server" />
        <asp:HiddenField ID="txtPaymentSelection" runat="server" />
        <asp:HiddenField ID="txtNSFMax" runat="server" Value="50" />       
        
        <asp:TextBox ID="txtContract_RECID" CssClass="DB form-control hidden" runat="server"></asp:TextBox>

           <div class="container">
                <asp:Literal ID="litHeader" runat="server"></asp:Literal>
               <div id="reversePaymentInfo" class="col-sm-12">
                   <h4>Are you sure you want to reverse this Payment?</h4>
                   <div class="col-xs-12">
                        <label>Date: <asp:Literal ID="litDate" runat="server"></asp:Literal> </label>
                   </div>
                   <div class="col-xs-12">
                        <label>Patient: <asp:Literal ID="litPatient" runat="server"></asp:Literal> </label>
                   </div>
                   <div class="col-xs-12">
                        <label>Amount: <asp:Literal ID="litAmount" runat="server"></asp:Literal> </label>
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                   <div id="NSFOption" class="col-xs-12">
                        NSF Charge to Patient: <asp:TextBox ID="txtNSF" class="form-control" placeholder="0.00" Width="200px" onBlur="verifyAmount();" runat="server"></asp:TextBox> 
                       <br />($50 max. An entry will automatically produce an Invoice)<br />                    
                       <asp:RegularExpressionValidator ID="rfvNSF" ErrorMessage="Please enter a valid dollar amount." ControlToValidate="txtNSF" ValidationExpression="^(-)?\d+(\.\d\d)?$" CssClass="requiredFieldValidator" runat="server" />
                   </div>
<%--                   <div class="col-xs-12">
                       <asp:CheckBox ID="chkInvoice" runat="server"  Text="Create an Invoice"/>
                   </div>--%>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                    <div class="col-xs-12">
                        Reason For Reversal/Adjustment: 
                       <asp:TextBox ID="txtReason" CssClass="DB form-control" Style="width: 490px;" runat="server" MaxLength="8000" TextMode="MultiLine" Rows="5" placeholder="Enter a reason for reversing or adjusting off this payment..."></asp:TextBox>                   
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                   <div class="col-xs-12">
                       <asp:Button ID="btnConfirm" CssClass="btn btn-md btn-warning" OnClientClick="showLoadingGif(); cancelVal();" runat="server" Text="Yes" />&nbsp;<asp:Button ID="btnCancel" CssClass="btn btn-md btn-primary" OnClientClick="cancelVal();" runat="server" Text="No" />
                    </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                </div>
                <div id="divMessage">
                    <div class="col-xs-12">
                        <asp:Literal ID="litMessage" runat="server"></asp:Literal> <br />                    
                    </div>                    
               </div>
           </div>
            <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
                <img src="images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
            </div>
        </form>
        <%--<div style="display: none">--%>
        <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
    <%--</div>--%>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script>
        function showLoadingGif() {
            jQuery('#loading_indicator').show();
        }
        function cancelVal() {
            ValidatorEnable(document.getElementById("<%=rfvNSF.ClientID%>"), false);
        }
        function verifyAmount() {
            var patAmount = jQuery('#' + '<%=txtNSF.ClientID%>').val();
            var nsfMaxAmount = jQuery('#' + '<%=txtNSFMax.ClientID%>').val();
            if (patAmount == 0 || isNaN(patAmount) || patAmount == "") {
                document.getElementById('<%=txtNSF.ClientID%>').value = ""
                document.getElementById('<%=txtNSF.ClientID%>').focus();
            } else {
                if (patAmount > nsfMaxAmount) {
                    document.getElementById('<%=txtNSF.ClientID%>').value = ""
                    alert('WARNING: Amount charged to patient cannot exceed the max NSF Fee allowed ($50.00). Please re-enter charge amount.');
                    document.getElementById('<%=txtNSF.ClientID%>').focus();
                }
            }
        }
    </script>
</asp:Content>
