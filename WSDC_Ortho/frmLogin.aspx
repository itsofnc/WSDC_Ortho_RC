<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="mstSite.Master" CodeBehind="frmLogin.aspx.vb" Inherits="WSDC_Ortho.frmLogin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <!--03/12/15 cpb add standard form validation-->
    <link href="Common/formValidation/css/formValidation.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
         <!--%%% --> <!-- DO NOT DELETE THE %%% USED TO FIND BODY OF frmLogin IN EMBEDDED DIVS-->
        <form id="frmLogin" defaultbutton="btnLogin" role="form" runat="server">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12 text-center center-block">
                    <h2><asp:Literal ID="litHeaderText" runat="server"></asp:Literal> <span class="text-muted">Login</span></h2>
                </div>
            </div>
            <div id="divRequestInvitation" class="row text-center center-block" runat="server" >
                <%--<p>Not Registered?  Please contact<button type="button" class="btn btn-link" data-toggle="modal" data-target="#divRequestLogin">Administration</button>for an invitation.</p>--%>
                <p>Not Registered?  <a href="frmRequestLogin.aspx" class="alert-link" style="font:bold">Request an invitation.</a></p>
            </div>

            <div class="row">
                <div class="login-container text-center center-block" style="display: table; vertical-align: middle;">
                    <div class="form-group">
                        <label class="pull-left"><asp:Literal ID="litLoginStyle" runat="server"></asp:Literal>:</label>
                        <asp:TextBox ID="txtUserID" name="txtUserID" class="form-control" runat="server" placeHolder="test" onBlur="goToPassword()"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="pull-left">Password:</label>
                        <a href="frmForgotPassword.aspx" class="pull-right" onmouseover="blnFrmValidate=true;" onmouseout="blnFrmValidate=false; ">Forgot Password?</a>
                        <asp:TextBox ID="txtPassword" name="txtPassword" type="password" class="form-control" runat="server" placeHolder="Password"></asp:TextBox>
                    </div>       

                    <span id="spnErrorMessage" class="requiredFieldValidator h4"></span>
                                 
                    <asp:Button ID="btnLogin" type="submit" class="btn btn-primary center-block col-xs-12" runat="server" Text="Login" onClientClick="execLogin(); return false;" />
                    <div id="divGuestLogin">
                        <br /><hr />
                        <button id="btnGuestLogin" name="btnGuestLogin" class="btn btn-default center-block col-xs-12" onmouseover="blnFrmValidate=true;" onmouseout="blnFrmValidate=false; " onclick="guestLogin()">Continue as Guest</button>
                    </div>
                    <div id="divNewUser">
                    <hr />
                        <button id="btnNewUser" name="btnNewUser" class="btn btn-default center-block col-xs-12" onmouseover="blnFrmValidate=true;" onmouseout="blnFrmValidate=false; " onclick="<asp:Literal ID="litNewUser" runat="server"></asp:Literal>">Create New Account</button>
                    </div>
                    <asp:HiddenField ID="txtNewUserAllowed" runat="server" />
                    <asp:HiddenField ID="txtGuestUserAllowed" runat="server" />
                    <asp:HiddenField ID="txtGuestRedirect" runat="server" />
                    <asp:HiddenField ID="txtLoginRedirect" runat="server" />
                </div>
            </div>
           
            <!-- /.row -->
        </form>
         <!--%%% --> <!-- DO NOT DELETE THE %%% USED TO FIND BODY OF frmLogin IN EMBEDDED DIVS-->
        <!--Error Message -->
        <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
        </div>

        <!--request registration-->

        <asp:Literal ID="litCouldnot" runat="server"></asp:Literal>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
     <!--%%% --> <!-- DO NOT DELETE THE %%% USED TO FIND BODY OF frmLogin IN EMBEDDED DIVS-->
    <!--03/12/15 cpb add standard formvalidation-->
    <script src="Common/formValidation/js/formValidation.js"></script>
    <script src="Common/formValidation/js/framework/bootstrap.min.js"></script>
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script>
        var blnFrmValidate = false;
        // set blnTimeoutActive = false to prevent session timeout from site master
        
        jQuery(document).ready(function() {

            blnTimeoutActive = false

            jQuery('#loading_indicator').show();
            // call ajax to make sure database has been setup.  if not go set it up then send user on to the login page.
            jQuery.post("ajaxfunctions.aspx", { action: "databaseVerify" },
                function (data) {
                    if (data.indexOf("Error") == -1) {
                        jQuery.post("ajaxFunctions.aspx", { action: "verifySysUsers" },
                            function (data) {
                                if (data.indexOf("Error") == -1) {
                                    jQuery('#loading_indicator').hide();
                                } else {
                                    window.location.href("frmMessaging.aspx")
                                }
                            });
                    } else {
                        window.location.href("frmMessaging.aspx")
                    }
                })

            if (jQuery('#<%=txtNewUserAllowed.ClientId%>').val() == 'false'){
                jQuery('#divNewUser').addClass("hidden");
            }
            if (jQuery('#<%=txtGuestUserAllowed.ClientId%>').val() == 'false'){
                jQuery('#divGuestLogin').addClass("hidden");
            }
            jQuery('#frmLogin')
                .formValidation({
                    framework: 'bootstrap',                             
                    message: 'This value is not valid',
                    fields: {
                        <%=txtUserID.UniqueID%>: {
                            validators: {
                                notEmpty: {
                                    message: 'User Id is required and cannot be empty'
                                }
                            }
                        },
                        <%=txtUserID.UniqueID%>: {
                            validators: {
                                callback: {
                                    message: 'User Id is required and cannot be empty',
                                    callback: function (value, validator, $field) { 
                                        // If blnFrmValidate is set, this disables txtBox by setting the field to valid, 
                                        // otherwise validation turned back on
                                        // see http://formvalidation.io/examples/ignoring-validation/ for examples
                                        return (blnFrmValidate) ? true : (value !== '');
                                    }
                                }
                            }
                        },
                        <%=txtPassword.UniqueID%>: {
                            validators: {
                                notEmpty: {
                                    message: 'Password is required and cannot be empty'
                                }
                            }
                        },
                        <%=txtPassword.UniqueID%>: {
                            validators: {
                                callback: {
                                    message: 'Password is required and cannot be empty',
                                    callback: function (value, validator, $field) {  
                                        // If blnFrmValidate is set, this disables txtBox by setting the field to valid, 
                                        // otherwise validation turned back on
                                        //// see http://formvalidation.io/examples/ignoring-validation/ for examples
                                        return (blnFrmValidate) ? true : (value !== '');
                                                
                                                
                                    }
                                }
                            }
                        }
                    }
                })
             .on('click', '[name="btnNewUser"]', function(e) {
                 $('#frmLogin').formValidation('revalidateField', '<%=txtUserID.UniqueID%>');
                 $('#frmLogin').formValidation('revalidateField', '<%=txtPassword.UniqueID%>');
             })
            ; 
        });

        function goToPassword() {
            jQuery('#' + '<%=txtPassword.ClientID %>').focus();
        }

        function guestLogin() {
            jQuery('#loading_indicator').show();
            jQuery.post('ajaxFunctions.aspx', { action: 'guestLogin' },
                function (data) {
                    // data will return as message if inactive
                    if (data.indexOf("ERROR") == -1) {
                        
                        window.location.href=jQuery('#<%=txtGuestRedirect.ClientId%>').val();
                    } else {
                        jQuery('#loading_indicator').hide();
                        alert("Sorry, the Guest account is currently not active.");

                    };
                });
        }

        function execLogin() {
            jQuery('#loading_indicator').show();
            uid = jQuery('#<%=txtUserID.ClientId%>').val();
            pw = jQuery('#<%=txtPassword.ClientId%>').val();
            jQuery.post('ajaxFunctions.aspx', { action: 'execLogin', uid: uid, pw: pw },
                function (data) {
                    // data will return as message if inactive
                    if (data.indexOf("ERROR") == -1) {                        
                        window.location.href=jQuery('#<%=txtLoginRedirect.ClientId%>').val();
                    } else {
                        jQuery('#spnErrorMessage').html(data.split("ERROR:")[1])
                        jQuery('#loading_indicator').hide();
                    };
                });
        }


    </script>    
     <!--%%% --> <!-- DO NOT DELETE THE %%% USED TO FIND BODY OF frmLogin IN EMBEDDED DIVS-->
</asp:Content>
