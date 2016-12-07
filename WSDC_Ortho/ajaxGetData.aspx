<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ajaxGetData.aspx.vb" Inherits="WSDC_Ortho.ajaxGetData" %>

<body>
    <form id="Form1" class="form-horizontal" role="form" runat="server">
        <div style="width: 100%">
            <asp:Literal ID="litConstants" runat="server"></asp:Literal>
            <asp:Literal ID="litUpdateArea" runat="server"></asp:Literal>
            <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
        </div>
        <input id="btnAjaxSubmit" type="submit" value="" style="visibility: hidden"  />
    </form>

    <asp:Literal ID="litFormValidation" runat="server"></asp:Literal>

    <script>
        
        //jQuery(document).ready(function () {
            //setTimeout("revalidateCustom()", 3000);
        //    var curVal = $('#Money').val();            
        //    $('#Money').val($('#Money').val().replace('$ ', '').replace(/,/g, ''));
        //    $('#Form1').formValidation('revalidateField', 'Money');
        //    $('#Money').val(curVal);
        //    var curVal = $('#NumberField').val();
        //    $('#NumberField').val($('#NumberField').val().replace('$ ', '').replace(/,/g, ''));
        //    $('#Form1').formValidation('revalidateField', 'NumberField');
        //    $('#NumberField').val(curVal);
        //});
        //jQuery(document).ready(function () {
        //    jQuery('#Form1')
        //         .find('.datePicker')
        //        .datepicker({
        //            changeMonth: true,
        //            changeYear: true,
        //            yearRange: "-100:+10"
        //        })
        //        .mask('99/99/9999')
        //});

        function processForm(e) {
            if (e.preventDefault) e.preventDefault();
            /* do what you want with the form */
            // You must return false to prevent the default form behavior
            return false;
        }

        var form = document.getElementById('Form1');
        if (form.attachEvent) {
            form.attachEvent("submit", processForm);
        } else {
            form.addEventListener("submit", processForm);
        }
    </script>
</body>
