<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmCloseClaims.aspx.vb" Inherits="WSDC_Ortho.frmCloseClaims" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmCloseClaims' class="form-horizontal" role="form" defaultbutton="btnHidden" action="frmCloseClaims.aspx" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <asp:HiddenField ID="txtClaimID" runat="server" />
        <asp:HiddenField ID="txtClaimNumber" runat="server" />
        <asp:HiddenField ID="txtClaimBalance" runat="server" />
        <asp:HiddenField ID="txtChartNumber" runat="server" />
        <asp:HiddenField ID="txtPatientNumber" runat="server" />
        <asp:HiddenField ID="txtInsType" runat="server" />
        <asp:HiddenField ID="txtClaimAmount" runat="server" />
        <asp:TextBox ID="txtContract_RECID" CssClass="DB form-control hidden" runat="server"></asp:TextBox>

           <div class="container">
                <asp:Literal ID="litHeader" runat="server"></asp:Literal>
               <div id="closeClaimInfo" class="col-sm-12">
                   <h4>Are you sure you want to close this Claim?</h4>
                   <div class="col-xs-12">
                        <label>Date: <asp:Literal ID="litDate" runat="server"></asp:Literal> </label>
                   </div>
                   <div class="col-xs-12">
                        <label>Patient: <asp:Literal ID="litPatient" runat="server"></asp:Literal>  </label>      
                   </div>
                   <div class="col-xs-12">
                        <label>Insurance: <asp:Literal ID="litInsurance" runat="server"></asp:Literal>  </label>      
                   </div>
                   <div class="col-xs-12">
                        <label>Claim #: <asp:Literal ID="litClaimNumber" runat="server"></asp:Literal>  </label>      
                   </div>
                   <div class="col-xs-12">
                        <label>Amount: <asp:Literal ID="litAmount" runat="server"></asp:Literal>  </label>       
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                   <div class="col-xs-12">
                        Amount To Charge Patient: <asp:TextBox ID="txtCharge" class="form-control" width="200px" placeholder="0.00" onBlur="verifyAmount();" runat="server"></asp:TextBox> (an entry will automatically produce an Invoice)<br />
                       <asp:RegularExpressionValidator ID="rfvCharge" ErrorMessage="Please enter a valid dollar amount." ControlToValidate="txtCharge" ValidationExpression="^(-)?\d+(\.\d\d)?$" CssClass="requiredFieldValidator" runat="server" />
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                   <div class="col-xs-12">
                        Reason For Closing Claim: 
                       <asp:TextBox ID="txtReason" CssClass="DB form-control" Style="width: 490px;" runat="server" MaxLength="8000" TextMode="MultiLine" Rows="5" placeholder="Enter a reason for closing this claim..."></asp:TextBox>                   
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                   <div class="col-xs-12">
                       <asp:Button ID="btnConfirm" CssClass="btn btn-md btn-warning" OnClientClick="showLoadingGif(); cancelVal();" runat="server" Text="Yes" />&nbsp;<asp:Button ID="btnCancel" CssClass="btn btn-md btn-primary" OnClientClick="cancelVal(); closeForm();" runat="server" Text="No" />
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                   <div class="col-xs-12">
                        <br />                    
                   </div>
                </div>
                <div id="divMessage" class="hidden">
                    <div class="col-xs-12">
                        <br /><br />
                        <h4><asp:Literal ID="litMessage" runat="server"></asp:Literal> </h4>
                        <br />   
                        <asp:Button ID="btnClose" CssClass="btn btn-md btn-primary" OnClientClick="cancelVal(); closeForm();" runat="server" Text="OK (Close Page)" />         
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
            ValidatorEnable(document.getElementById("<%=rfvCharge.ClientID%>"), false);
        }
        function verifyAmount() {
            var patAmount = parseFloat(jQuery('#<%=txtCharge.ClientID%>').val());
            var openAmount = parseFloat(jQuery('#<%=txtClaimBalance.ClientID%>').val());
            if (patAmount == 0 || isNaN(patAmount)) {
                document.getElementById('<%=txtCharge.ClientID%>').value = ""            
            } else {
                if (patAmount > openAmount) {
                    document.getElementById('<%=txtCharge.ClientID%>').value = ""
                    alert('WARNING: Amount charged to patient $ ' + patAmount + ' cannot exceed the open balance $ ' + openAmount + 'on the claim. Please re-enter charge amount.');
                    document.getElementById('<%=txtCharge.ClientID%>').focus();
                }
            }
        }
        function closeForm() {
            window.close();
        }
    </script>
</asp:Content>
