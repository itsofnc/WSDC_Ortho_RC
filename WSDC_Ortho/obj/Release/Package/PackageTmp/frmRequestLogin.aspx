<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmRequestLogin.aspx.vb" Inherits="T3CommonCode.frmRequestLogin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <form id="frmRequestLogin"  role="form" runat="server">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12 text-center center-block">
                    <h2><asp:Literal ID="litHeaderText" runat="server"></asp:Literal> Request Login</h2>
                </div>
            </div>

            <div class="row">
                <div class="login-container text-center center-block" style="display: table; vertical-align: middle;">
                    <div class="form-group">
                        <label class="pull-left">Name:</label>
                        <asp:TextBox ID="txtRequestName" name="txtRequestName" class="form-control required" runat="server" placeHolder="Your Name"
                            required data-fv-notempty-message="Your name is required">
                        </asp:TextBox>
                    </div>  
                    <div class="form-group">
                        <label class="pull-left">Email:</label>
                        <asp:TextBox ID="txtRequestEmail" name="txtRequestEmail" class="form-control required" runat="server" placeHolder="Your Email"
                            required data-fv-notempty-message="Your name is required" data-fv-emailaddress="true"
                            data-fv-emailaddress-message="The value is not a valid email address">
                        </asp:TextBox>
                    </div>
                </div>
                <span class="col-xs-12">
                    <button type="button" class="btn btn-default center-block"  onclick="requestLogin()">Send Request</button>
                </span>
            </div>
            <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
                <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
            </div>
        </form>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <script src="Common/formValidation/js/formValidation.js"></script>
    <script src="Common/formValidation/js/framework/bootstrap.min.js"></script>
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script>
        jQuery('#frmRequestLogin')
        .formValidation({
            framework: 'bootstrap',                             
            message: 'This value is not valid'
            })

        function requestLogin() {
                jQuery('#loading_indicator').show();
                rqName = jQuery('#<%=txtRequestName.ClientID%>').val();
                rqEmail = jQuery('#<%=txtRequestEmail.ClientID%>').val();
                jQuery.post('ajaxFunctions.aspx', { action: 'requestLogin', name: rqName, email: rqEmail },
                    function (data) {
                        if (data.indexOf("ERROR") == -1) {
                            alert('Thank You for your interest.  Your request has been sent');
                            window.open('Default.aspx', '_Self')
                        } ;
                        jQuery('#loading_indicator').hide();
                    });
        }
    </script>
</asp:Content>
