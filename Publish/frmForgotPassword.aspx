<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmForgotPassword.aspx.vb" Inherits="WSDC_Ortho.frmForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <!--03/12/15 cpb add standard form validation-->
    <link href="Common/formValidation/css/formValidation.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <!-- Two columns  -->
        <form id="frmForgotPassword" role="form" runat="server">
            <div class="row">
                <h4 id="forgotPassword" class="headerText">Password Reset</h4>
                <br />
            </div>
            <div id="emailSent"><asp:Label ID="lblEmailSent" runat="server" style="color:blue; font-size:small;" Text=""></asp:Label></div>                
            <div id="divResetOptions">
            <div class="row">
                <p style="font-size: medium">
                    Please enter a valid <asp:Literal ID="litLoginType" runat="server"></asp:Literal> in the field below.<br />
                    A reset link will be sent to the email address on-file.
                </p>
            </div>
            <div class="row">
                <div id="email" class="form-group">
                    <label id="lblEmailError" for="txtEmail" style="color:red;"></label>
                    <%--<asp:RegularExpressionValidator ID="revEmail" runat="server"
                        ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage=" Email is an incorrect format." ForeColor="red" 
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" EnableClientScript="True" SetFocusOnError="True" />--%>
                    <asp:TextBox ID="txtEmail" class="form-control" runat="server" placeHolder="Email" Style="max-width: 300px"
                            data-fv-notempty="true" data-fv-notempty-message="The Email address is required and cannot be empty" 
                            data-fv-emailAddress="true" data-fv-emailAddress-message="Invalid Email Address Format"></asp:TextBox>
                </div>
                <div id="userId" class="form-group col-sm-4 col-md-4">                    
                    <asp:TextBox ID="txtUserID" class="form-control" runat="server" placeHolder="UserID" 
                        data-fv-notempty="true" data-fv-notempty-message="The User ID is required and cannot be empty"></asp:TextBox>
                    <label id="lblUserIdError" for="txtUserID" style="color:red;"></label>
                </div>
            </div>
            <div class="row">
                <div class="form-group">
                    <a href="#" onclick="forgotPassword();" class=" btn btn-sm btn-primary">Reset Password</a><br />
                    <script>
                        function forgotPassword() {
                            jQuery('#loading_indicator').show();
                            var email = jQuery("#<%=txtEmail.ClientID%>").val();
                            var userID = jQuery("#<%=txtUserID.ClientID%>").val();
                            if (email == "" && userId == "") {
                                alert("A valid entry is required for password recovery.");
                                jQuery('#' + '<%=txtEmail.ClientID%>').focus();
                            } else {
                                if (email != "") {
                                    //using email 
                                    jQuery.post("ajaxFunctions.aspx", { email: email, action: "forgotPassword" },
                                    function (data) {
                                        jQuery('#loading_indicator').hide();
                                        if (data.indexOf("Sorry,") == 0) {
                                            alert(data);
                                        } else {                                            
                                            jQuery("#emailSent").removeClass("hidden");
                                            jQuery("#divResetOptions").addClass("hidden");
                                            jQuery("#<%=lblEmailSent.ClientId%>")[0].innerHTML = data;
                                        }
                                    });
                                } else {
                                    //using UserID
                                    jQuery.post("ajaxFunctions.aspx", { userID: userID, action: "forgotPassword" },
                                   function (data) {
                                       jQuery('#loading_indicator').hide();
                                       if (data.indexOf("Sorry,") == 0) {
                                           alert(data);
                                       } else {
                                           jQuery("#emailSent").removeClass("hidden");
                                           jQuery("#divResetOptions").addClass("hidden");
                                           jQuery("#<%=lblEmailSent.ClientId%>")[0].innerHTML = data;
                                       }
                                   });
                                }
                            }
                        }
                    </script>
                </div>
                </div>
            </div>
            <!-- /.row -->
        </form>
    </div>
    <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <!--03/12/15 cpb add standard formvalidation-->
    <script src="Common/formValidation/js/formValidation.js"></script>
    <script src="Common/formValidation/js/framework/bootstrap.min.js"></script>
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script>
        // set blnTimeoutActive = false to prevent session timeout from site master
        jQuery(document).ready(function () { blnTimeoutActive = false })

        jQuery(document).ready(function() {
            jQuery('#frmForgotPassword')
                .formValidation({
                    framework: 'bootstrap',
                    //do not use icons on this page
                    //icon: {
                    //    valid: 'glyphicon glyphicon-ok',
                    //    invalid: 'glyphicon glyphicon-remove',
                    //    validating: 'glyphicon glyphicon-refresh'
                    //},
                    //err: {
                    //    container: 'tooltip'
                    //},
                    message: 'This value is not valid'
                });
        });

        function validateEmail() {
            var email = jQuery("#<%=txtEmail.ClientId%>").val();
            if (email == "") {
            } else {
                jQuery.post("ajaxFunctions.aspx", { email: email, action: "validateEmail", reset: true },
                       function (data) {
                           if (data.indexOf("Error:") == 0) {
                               if (jQuery('#email').hasClass('has-error')) {
                               }
                               else {
                                   jQuery('#email').addClass('has-error');
                                   jQuery('#lblEmailError').html(data.replace('Error:',''));
                                   jQuery('#lblEmailError').show();
                               }
                           }
                           else {
                               if (jQuery('#email').hasClass('has-error')) {
                                   jQuery('#email').removeClass('has-error');
                                   jQuery('#lblEmailError').html('');
                               }
                           }
                       });
            }
        }
        function validateUserId() {
            var user = jQuery("#<%=txtUserID.ClientID%>").val();
            if (user == "") {
            } else {
                jQuery.post("ajaxFunctions.aspx", { user: user, action: "validateUserId", reset: true },
                       function (data) {
                           if (data.indexOf("Error:") == 0) {
                               if (jQuery('#userId').hasClass('has-error')) {
                               }
                               else {
                                   jQuery('#userId').addClass('has-error');                                   
                                   jQuery('#lblUserIdError').html(data.replace('Error:', ''));
                                   jQuery('#lblUserIdError').show();
                               }
                           }
                           else {
                               //if (jQuery('#userId').hasClass('has-error')) {
                                   jQuery('#userId').removeClass('has-error');
                                   jQuery('#lblUserIdError').html('');
                              // }
                           }
                       });
            }
        }
    </script>
</asp:Content>
