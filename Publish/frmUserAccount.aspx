<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmUserAccount.aspx.vb" Inherits="WSDC_Ortho.frmUserAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        
        <form id="frmInviteUser" class="form-horizontal" role="form" runat="server">           
            <div class="text text-center">
                <asp:Literal ID="litSiteLogo" runat="server"></asp:Literal>
            </div>
            <div class="row text-center center-block">
                <label class="control-label h3"><asp:Literal ID="litTitle" runat="server"></asp:Literal></label>
                    <a id="btnReturnUserList" href="#" class="btn btn-default" style="margin-left:10px; margin-bottom: 10px;" onclick="returnUserList();">Return to Users List</a>
            </div>
            <div id="rowUserId" class="row">
                <div class="form-group">
                    <label class="control-label col-sm-3"><span class="requiredFieldIndicator">*</span><asp:Literal ID="litUserTitle" runat="server"></asp:Literal></label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="txtUserId" runat="server" class="form-control required" MaxLength="45" 
                            data-fv-notempty="true" data-fv-notempty-message="User ID Required" onchange="verifyUserId(); setUserEmail(); "></asp:TextBox>
                    </div>
                </div>
            </div>
            <div id="rowPassword" class="row">
                <div class="form-group">
                    <label class="control-label col-sm-3"><span class="requiredFieldIndicator">*</span>Password</label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="txtValidate_password" TextMode="Password" type="password" class="form-control required" name="validate_password" 
                            placeholder="Enter password" required="true" runat="server"></asp:TextBox>
                    </div>
                </div>
            </div>     
            <div id="rowPassword2" class="row">
                <div class="form-group ">
                    <label class="control-label col-sm-3"><span class="requiredFieldIndicator">*</span>Re-enter Password</label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="txtValidate_password2" TextMode="Password" type="password" class="form-control required" name="validate_password2" 
                            placeholder="Re-enter password" required="true" runat="server"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div id="rowFirstName" class="row">
                <div class="form-group">
                    <label class="control-label col-sm-3"><span class="requiredFieldIndicator">*</span>First Name:</label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="txtFirstName" runat="server" class="form-control required" MaxLength="50"
                            data-fv-notempty="true" data-fv-notempty-message="First Name Required"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div id="rowLastName" class="row">
                <div class="form-group">
                    <label class="control-label col-sm-3"><span class="requiredFieldIndicator">*</span>Last Name:</label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="txtLastName" runat="server" class="form-control required" MaxLength="50"
                            data-fv-notempty="true" data-fv-notempty-message="Last Name Required"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div id="rowEmail" class="row">
                <div class="form-group">
                    <label class="control-label col-sm-3"><span class="requiredFieldIndicator">*</span>Email:</label>
                    <div class="col-sm-6">
                        <asp:TextBox ID="txtEmail" runat="server" class="form-control required" MaxLength="50" onchange="verifyEmail();"
                            data-fv-emailaddress="true" data-fv-emailaddress-message="The value is not a valid email address format."
                            data-fv-notempty="true" data-fv-notempty-message="Email Required"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div id="divLevel" class="row">
                <div id="levelSelect" class="form-group">
                    <div class="input-group-sm">
                        <label class="control-label col-sm-3"><span class="requiredFieldIndicator">*</span>Select Level:</label>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="ddlLevel" name="ddlLevel" runat="server" class="form-control" 
                                data-fv-notempty="true" data-fv-notempty-message="User Role Required">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div id="rowLastLogin" class="row">
                <div id="lastLogin" class="form-group">
                    <div class="input-group-sm">
                        <label class="control-label col-sm-3">Last Login:</label>
                        <div class="col-sm-6">
                            <asp:TextBox ID="txtLastLogin" runat="server" CssClass="form-control" Enabled="True" ReadOnly="True"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

            <div id="rowSaveUser" class="row center-block" style="padding-bottom: 75px;">
                <div class="col-sm-12 center-block text-center">
                     <asp:Button ID="btnSaveUser" runat="server" Text="Save User" CssClass="btn btn-primary" />
                </div>
            </div>
            <div style="display: none">
                <asp:TextBox ID="txtLoginStyle" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtRecid" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtReturnUrl" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtEditTarget" runat="server"></asp:TextBox>
            </div>
        </form>
    </div>

    <!--Loading Indicator  needs to be here this is shown from uploadFile.aspx-->
    <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.55;">
        <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -128px; margin-top: -17px;" alt="" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    <asp:Literal ID="litHideDivs" runat="server"></asp:Literal>
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <script>
        // set blnTimeoutActive = false to prevent session timeout from site master
        jQuery(document).ready(function () { blnTimeoutActive = false })

        jQuery('#frmInviteUser').formValidation({
            fields: {
                <%=txtValidate_password.UniqueID%>: {
                            validators: {
                                identical: {
                                    field: '<%=txtValidate_password2.UniqueID%>',
                                    message: 'The password and its confirm are not the same'
                                },
                                notEmpty:{
                                    message: 'The password is required and may not be left blank'
                                },
                                regexp:{
                                    regexp: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/,
                                    message: 'The password must be between 6 to 20 characters and contain at least one numeric digit, one uppercase and one lowercase letter'
                                }
                            }
                        },
                        <%=txtValidate_password2.UniqueID%>: {
                            validators: {
                                identical: {
                                    field: '<%=txtValidate_password.UniqueID%>',
                                    message: 'The password and its confirm are not the same'
                                },
                                notEmpty:{
                                    message: 'The confirm password is required and may not be left blank'
                                },
                                regexp:{
                                    regexp: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/,
                                    message: 'The confirm password must be between 6 to 20 characters and contain at least one numeric digit, one uppercase and one lowercase letter'
                                }
                            }
                        }
            }
        })
        .on('err.field.fv', function (e, data) {
            {
                if (data.fv.getSubmitButton()) {
                    data.fv.disableSubmitButtons(false);
                };
            }
        })
        .on('success.field.fv', function (e, data) {
            if (data.fv.getSubmitButton()) {
                data.fv.disableSubmitButtons(false);
            };
        })
        .on('success.form.fv', function (e) {
            e.preventDefault();               
                jQuery('#loading_indicator').show();
                // save registration data
                jQuery.post("ajaxFunctions.aspx", {
                    action:"saveUser",
                    recId: jQuery('#' + '<%=txtRecId.ClientID%>').val(),
                    userid:jQuery('#' + '<%=txtUserId.ClientID%>').val(),
                    email:jQuery('#' + '<%=txtEmail.ClientID%>').val(),
                    firstName:jQuery('#' + '<%=txtFirstName.ClientID%>').val(),
                    lastName: jQuery('#' + '<%=txtLastName.ClientID%>').val(),
                    userRole: jQuery('#' + '<%=ddlLevel.ClientID%>').val(),
                    password: jQuery('#' + '<%=txtValidate_password.ClientID%>').val()
                },
                function(data) {
                    if (data.indexOf("ERROR") > -1) {
                        alert('Sorry, We were unable to save this user information.\nPlease contact support.');
                    } else {                        
                        if (data.indexOf("REDIRECT") > -1) {
                            strRedirectForm = data.split("||")[1];
                            window.location(strRedirectForm);
                        } else {
                            alert('Update completed successfully.');  
                            jQuery('#' + '<%=txtRecid.ClientID%>').val('-1');
                            jQuery('#' + '<%=txtUserId.ClientID%>').val('');
                            jQuery('#' + '<%=txtEmail.ClientID%>').val('');
                            jQuery('#' + '<%=txtFirstName.ClientID%>').val('');
                            jQuery('#' + '<%=txtLastName.ClientID%>').val('');
                            jQuery('#' + '<%=ddlLevel.ClientID%>').val('-1');
                            jQuery('#' + '<%=txtValidate_password.ClientID%>').val('');
                            jQuery('#' + '<%=txtValidate_password2.ClientID%>').val('');
                            jQuery('#<%=txtUserId.ClientID%>').focus();
                            jQuery('#frmInviteUser').data('formValidation').resetForm();
                        }
                    }
                    jQuery('#loading_indicator').hide();
                });
        })

        function returnUserList() {
            if (jQuery('#' + '<%= txtEditTarget.ClientID%>').val() == '_self') {
                 window.open(jQuery('#' + '<%= txtReturnUrl.ClientID%>').val(), '_self')
            } else {
                window.close();
            }
           
        }

        function setUserEmail() {
            txtLoginStyle = jQuery('#' + '<%=txtLoginStyle.ClientID%>').val();
            if (txtLoginStyle.indexOf("email") > -1) {
                jQuery('#' + '<%=txtEmail.ClientID%>').val(jQuery('#' + '<%=txtUserId.ClientID%>').val());
                alert('user email address auto set');
            }
        }

        //verify email does not already exist
        function verifyEmail() {
            txtLoginStyle = jQuery('#' + '<%=txtLoginStyle.ClientID%>').val();
            if (txtLoginStyle.indexOf("email") > -1) {
            } else {
                userEmail = jQuery('#<%=txtLoginStyle.ClientID%>').val();
                userRecid = jQuery('#<%=txtRecid.ClientID%>').val();
                if (userEmail == '') {
                    jQuery('#<%=txtUserId.ClientID%>').focus();
                };
                jQuery('#loading_indicator').show();
                jQuery.post("ajaxFunctions.aspx", {email:userEmail,recid:userRecid,action:"verifyUserEmail"},
                    function(data) {
                        jQuery('#loading_indicator').hide();
                        if (data.indexOf("Sorry,") == 0) {
                            alert(data);
                            jQuery('#<%=txtLoginStyle.ClientID%>').select();
                            jQuery('#<%=txtEmail.ClientID%>').focus();
                        } 
                    });
            }           
        };

        //verify userid does not already exist
        function verifyUserId() {
            userID = jQuery('#<%=txtUserId.ClientID%>').val();            
            userRecid = jQuery('#<%=txtRecid.ClientID%>').val();
            if (userID == '') {
                jQuery('#<%=txtUserId.ClientID%>').focus();
            };
            jQuery('#loading_indicator').show();
            jQuery.post("ajaxFunctions.aspx", {userid:userID,recid:userRecid,action:"verifyUserId"},
                function(data) {
                    jQuery('#loading_indicator').hide();
                    if (data.indexOf("Sorry,") == 0) {
                        alert(data);
                        jQuery('#<%=txtUserId.ClientID%>').select();
                        jQuery('#<%=txtUserId.ClientID%>').focus();
                    } 
                });
        };
        
    </script>
</asp:Content>
