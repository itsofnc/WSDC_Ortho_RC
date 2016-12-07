<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmEmbeddedFLM.aspx.vb" Inherits="T3CommonCode.frmEmbeddedFLM" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <!--#include file="Common/inc/frmListManagerCSS.inc"-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="frmEmbeddedFLM" runat="server">
    <div style="padding-top: 25px; padding-bottom:25px;">
        <p>Please enter sample Release Note below:</p>
        Date:
        <asp:TextBox ID="txtDate" runat="server"></asp:TextBox>
        Notes:
        <asp:TextBox ID="txtNotes" runat="server"></asp:TextBox>
        <input id="btnSubmit" type="button" value="Submit" class="btn btn-default" onclick="saveData();"/>
    </div>
    
    <div id="divFLM"></div>
    <div id="divJS"></div>
    <div id="divHiddenItems" class="hidden">
        <asp:TextBox ID="txtFLMSesPrefix" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtFLMGetText" runat="server"></asp:TextBox>
    </div>
    <div id="loading_indicator" style="display: none; position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
        <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
    </div>
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
   
    <script>
        jQuery(document).ready(function () {
            
            jQuery('#loading_indicator').show();
            jQuery.get(jQuery('#' + '<%=txtFLMGetText.ClientID%>').val(),
            function(data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();
                resize();
            });
        })

        function loadDivAreas(data) {
            arrFLMContents = data.split("<!--%%% -->");
            jQuery('#divFLM').html(arrFLMContents[1]);
            jQuery('#divJS').html(arrFLMContents[3]);
        }

        function reloadForm() {
            // this reloads the entire form with updated FLM data
             window.open('frmEmbeddedFLM.aspx?sesPrefix=' + jQuery('#' + '<%=txtFLMSesPrefix.ClientID%>').val(), '_self');
        }

        function saveData() {
            // calls custom save data routine...
            dteDate = jQuery("#<%= txtDate.ClientID%>").val();
            strNotes = jQuery("#<%= txtNotes.ClientID%>").val();
            jQuery.post('ajaxFunctionsCommonCode.aspx', { action: 'saveEmbeddedFLM', date: dteDate, notes: strNotes, sesLst: jQuery('#' + '<%=txtFLMSesPrefix.ClientID%>').val() + 'embflmrid' },
            function (data) {
                if (data.indexOf("ERROR") > -1) {
                    displayText = "SQL Error: Please contact your system administrator for assistance. \n \n"
                    displayText += data.split("ERROR")[1];
                    alert(displayText);
                    return false;
                } else {
                    jQuery('#loading_indicator').show();
                    jQuery("#<%= txtNotes.ClientID%>").val('');
                    jQuery.get(jQuery('#' + '<%=txtFLMGetText.ClientID%>').val(),
                    function(data) {
                        loadDivAreas(data);
                        jQuery('#loading_indicator').hide();
                        resize();
                    });
                }
            });
        }

        function saveDataUsingFLMUpdateCC() {
            // uses FLM common code to insert/update data with all validations rules, etc. (slower)
            updateInfo = "release_date||" + jQuery("#<%= txtDate.ClientID%>").val();
            updateInfo += "::notes||" + jQuery("#<%= txtNotes.ClientID%>").val();
            Tbl = 'ReleaseNotes';
            jQuery.post('ajaxGetData.aspx', { action: 'update', id: -1, tb: 'ReleaseNotes', dBack: updateInfo, sesLst: jQuery('#' + '<%=txtFLMSesPrefix.ClientID%>').val() + 'embflmrid' },
            function (data) {
                //08/06/15
                if (data.indexOf("*SQL_ERROR*") > -1) {
                    displayText = "SQL Error: Please contact your system administrator for assistance. \n \n"
                    displayText += data.split("*SQL_ERROR*")[1];
                    alert(displayText);
                    return false;
                }
                //03/13/15 - validate unique field value w/ data 
                if (data.indexOf("%%") > -1) {
                    var displayText = data.split("%%")[1].replace("~~", "\n");
                    alert(displayText);
                    return false
                } else {
                    refreshFLM(Tbl);
                    return true
                }
            });
            window.open('frmEmbeddedFLM.aspx?sesPrefix=' + jQuery('#' + '<%=txtFLMSesPrefix.ClientID%>').val(), '_self');
        }
            

    </script>
</asp:Content>
