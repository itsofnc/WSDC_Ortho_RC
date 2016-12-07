<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmClaimsReprint.aspx.vb" Inherits="WSDC_Ortho.frmClaimsReprint" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <style>
      .checkbox .btn,
      .checkbox-inline .btn {
        padding-left: 2em;
        min-width: 8em;
      }
 
      .checkbox label,
      .checkbox-inline label {
        text-align: left;
        padding-left: 0.5em;
      }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id='frmPrintClaimsReprint' class="form-horizontal formName" role="form" runat="server">
        <%--hidden default button so enter key doesn't submit the form--%>
        <asp:Button ID="btnHidden" CssClass="hidden" OnClientClick="return false" runat="server" />

        <div class="container">
            <h4 style="color: #2d6ca2;" class="col-sm-12">
                <asp:Label ID="lblTitle" runat="server" Text="Re-Print Claims"></asp:Label></h4>
            <h5 class="col-sm-12">
                <asp:Label ID="lblInfo" runat="server" Text="Enter a single claim number or multiple claim numbers separated by commas"></asp:Label></h5>
            <div class="col-sm-12">
                <div class="form-group col-sm-9">
                    <asp:TextBox ID="txtClaimNumbers" CssClass="form-control" runat="server" placeHolder="Enter Claim #"></asp:TextBox>                    
                </div>
            </div>
            <div class="col-sm-12">
                <div class="form-group col-sm-6">
                    <div class="checkbox">
                        <label class="btn btn-default">
                            <asp:CheckBox ID="cbCombinedPrint" runat="server" Text="Print All Claims On 1 Form" />
                            <%--Enabled="false"--%>
                        </label>
                        <h5 style="color:gray;"> (only select this option if all claims are associated with one patient)</h5>
                    </div>
                </div>
            </div>
            <div class="col-sm-12">
                <asp:Label ID="lblMessage" runat="server"></asp:Label><br />
            </div>
            <div class="col-sm-12">
                <span style="padding-left: 0px;">
                    <asp:Button ID="btnSubmit" class="btn btn-primary" runat="server" Text="Print" />
                </span>
            </div>
            <div class="col-sm-12">
                <br />
            </div>
        </div>
    </form>
    <%--<div style="display: none">--%>
    <iframe src="<asp:Literal ID="litFrameCall" runat="server"></asp:Literal>" id="forcedownload" width="800" height="800" frameborder="0" target="_self"></iframe>
    <%--</div>--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
</asp:Content>
