<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmPasswordReset.aspx.vb" Inherits="WSDC_Ortho.frmPasswordReset" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <!--<link href="Common/css/passwordValidation_bootsnip.css" rel="stylesheet" />--!>
    <!--03/12/15 cpb add standard form validation-->
    <link href="Common/formValidation/css/formValidation.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <form id="frmPasswordReset" runat="server">
            <div class="container">
                <div id="passwordReset" style="padding-top: 20px;">
                    <asp:Literal ID="litUserInfo" runat="server"></asp:Literal>
                    <p>
                        <label>Password must be between 6 to 20 characters and contain at least one numeric digit, one uppercase and one lowercase letter</label></p>
                    <div class="row">
                        <div class="form-group col-sm-4">
                            <label for="txtValidate_password">New Password</label>
                            <div class="input-group" data-validate="password">
                                <asp:TextBox ID="txtValidate_password" TextMode="Password" type="password" class="form-control" name="validate_password" 
                                    placeholder="Enter password" required="true" runat="server"></asp:TextBox>
                                <%--<span class="input-group-addon danger"><span class="glyphicon glyphicon-remove"></span></span>--%>
                            </div>
                        </div>
                        <div class="col-sm-12">
                            <br />
                        </div>
                        <div class="form-group col-sm-4">
                            <label for="txtValidate_password2">Re-enter Password</label>
                            <div class="input-group" data-validate="password">
                                <asp:TextBox ID="txtValidate_password2" TextMode="Password" type="password" class="form-control" name="validate_password2" 
                                    placeholder="Re-enter password" required="true" runat="server"></asp:TextBox>
                                <%--<span class="input-group-addon danger"><span class="glyphicon glyphicon-remove"></span></span>--%>
                            </div>
                        </div>
                        <div class="col-sm-12">
                            <br />
                        </div>
                        <div class="col-sm-12">
                            <asp:Label ID="lblMessage" Style="color: red;" runat="server"></asp:Label>
                        </div>
                        <div class="col-sm-12">
                            <br />
                        </div>
                        <div class="col-sm-4">
                            <asp:Button ID="btnUpdatePassword" CssClass="btn btn-large btn-success disabled" runat="server" Text="Update Password" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <p style="padding-top: 20px; font-size: medium">
                        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                    </p>
                </div>
            </div>
        </form>
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
            jQuery('#frmPasswordReset')
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
                    message: 'This value is not valid',
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
        });
    </script>
    <script>

        var m_success = false
        function validatePassword() {
            if (jQuery("#<%=txtValidate_password.ClientID%>").val() != jQuery("#<%=txtValidate_password2.ClientID%>").val()) {
                jQuery("#<%= lblMessage.ClientID%>").text('Passwords do not match');
                jQuery("#<%= btnUpdatePassword.ClientID%>").addClass('disabled');
                return false;
            } else {
                jQuery("#<%= lblMessage.ClientID%>").text('');
                jQuery("#<%= btnUpdatePassword.ClientID%>").removeClass('disabled');
                return true;
            }
        }

        //jQuery(document).ready(function () {
        //    jQuery('.input-group input[required], .input-group textarea[required], .input-group select[required]').on('keyup change', function () {
        //        var $form = jQuery(this).closest('form'),
        //            $group = jQuery(this).closest('.input-group'),
        //            $addon = $group.find('.input-group-addon'),
        //            $icon = $addon.find('span'),
        //            state = false;

        //        if (!$group.data('validate')) {
        //            state = jQuery(this).val() ? true : false;
        //        } else if ($group.data('validate') == "email") {
        //            state = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(jQuery(this).val())
        //        } else if ($group.data('validate') == 'phone') {
        //            state = /^[(]{0,1}[0-9]{3}[)]{0,1}[-\s\.]{0,1}[0-9]{3}[-\s\.]{0,1}[0-9]{4}$/.test(jQuery(this).val())
        //        } else if ($group.data('validate') == "length") {
        //            state = jQuery(this).val().length >= $group.data('length') ? true : false;
        //        } else if ($group.data('validate') == "number") {
        //            state = !isNaN(parseFloat(jQuery(this).val())) && isFinite(jQuery(this).val());
        //        } else if ($group.data('validate') == "password") {
        //            state = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/.test(jQuery(this).val());
        //            //validatePassword();
        //            //if (m_success == true) {
        //            //} else {
        //            //    state = false;
        //            //}
        //        }

        //        if (state) {
        //            $addon.removeClass('danger');
        //            $addon.addClass('success');
        //            $icon.attr('class', 'glyphicon glyphicon-ok');
        //        } else {
        //            $addon.removeClass('success');
        //            $addon.addClass('danger');
        //            $icon.attr('class', 'glyphicon glyphicon-remove');
        //        }

        //        if ($form.find('.input-group-addon.danger').length == 0) {
        //            $form.find('[type="submit"]').prop('disabled', false);
        //        } else {
        //            $form.find('[type="submit"]').prop('disabled', true);
        //        }
        //    });
        //    jQuery('.input-group input[required], .input-group textarea[required], .input-group select[required]').trigger('change');
        //});

    </script>
</asp:Content>

