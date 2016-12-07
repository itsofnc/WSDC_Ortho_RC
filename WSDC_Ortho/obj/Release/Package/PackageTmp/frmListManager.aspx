<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/mstSite.Master" CodeBehind="frmListManager.aspx.vb" Inherits="WSDC_Ortho.frmListManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">

    <!--#include file="Common/inc/frmListManagerCSS.inc"-->
    <script>
        //function needed for frmListManager iFrame
        function getAspElement(objId) {
            return jQuery("[id*=" + objId + "]")[0];
        }
    </script>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--3/22/16 T3 removed style=display: none; because we need it to show while loading a lot of data.  
        Added document.ready div hide below in scripts--%>
    <div id="loading_indicator" style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; 
            z-index: 9999999; background-color: #000000; opacity: 0.7;">
        <img src="Images/loading.gif" style="position: fixed; top: 50%; left: 50%; margin-left: -50px; margin-top: -50px;" alt="" />
    </div>
    <!--%%% --> <!-- DO NOT DELETE THE %%% USED TO FIND BODY OF FLM IN EMBEDDED DIVS-->
    <a href="#top" id="toTop"></a>
    <!--Post back form-->
    <form id='frmListManager' class="form-inline" role="form" runat="server" action="frmListManager.aspx" defaultbutton="btnHidden">
        <!--hidden default button so enter key doesn't submit the form-->
        <asp:Button ID="btnHidden" class="hidden" runat="server" OnClientClick="return false;" />
        <!--<asp:HiddenField ID="hidTableName" runat="server" />  2/13/15 t3 changed to hidmenuid-->
        <asp:HiddenField ID="hidMenuID" runat="server" />
        <asp:HiddenField ID="hidSessionsPrefix" runat="server" />
        <asp:TextBox ID="txtSortCol" class="hidden hidSortCol" runat="server" Text="1||sortd"></asp:TextBox>
        
         <!--hidden div to store our highlight color on an element to reference in highlightRow()-->
        <div id="highlightColor" class="t3-highlightColor" style="display:none;"></div>
        <nav id="navPage" class="navbar navbar-default" role="navigation">
            <div class="container-fluid">
                <!-- dropdown menu for list only shown on xs screen -->
                <div id="navDDL" class="navbar-header visible-xs">
                    <div class="navbar-toggle-list" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
                        <ul class="nav navbar-nav navbar-left">
                            <li class="dropdown">
                                <a href="#" class="dropdown-toggle" data-toggle="dropdown">Select List <b class="caret"></b></a>
                                <ul class="dropdown-menu">
                                    <asp:Literal ID="litNavDDL" runat="server"></asp:Literal>
                                </ul>
                            </li>
                        </ul>
                    </div>
                    <span class="navbar-selectedList visible-xs">
                        <asp:Literal ID="litDDLSelectedList" runat="server"></asp:Literal>
                    </span>
                </div>

                <!-- Collect the nav links, forms, and other content for toggling -->
                <!-- 2/13/15 this was padding-top: 10px-->
                <div style="margin-top: 5px;" class="visible-lg visible-md visible-sm">
                    <ul class="nav nav-pills">
                        <asp:Literal ID="litNavTabs" runat="server"></asp:Literal>
                    </ul>
                </div>
                <span class="navbar-selectedList" <asp:Literal ID="litSelectedListDisplay" runat="server"></asp:Literal>>
                    <asp:Literal ID="litSelectedList" runat="server"></asp:Literal>
                </span>
                <!-- /.navbar-collapse -->
            </div>
            <!-- /.container-fluid -->
        </nav>
        <!--Tabbed Navigation End-->
        <!--Pagination DIV-->
       <%-- 01/02/15 CP Removed visible-xs b/c we want to control this via Url if needs to be hidden--%>
        <!--10/13/14 add collapse of navigation for xs-->
        <!--10/14/15 cpb - add header and text decoration to pagenation to display for user-->
        <div id="divNoResults" class="container">
            <!--02/26/16 cpb added this div to have a button for adding new record if no results are found; updated vb to hide as necessary, was hiding lbl, now will hide the div.-->
            <label id="lblNoResults" class="h4">No results were found.</label>&nbsp;&nbsp;&nbsp;
            <a id="linkAddNoRows" href="#" class="btn btn-default btn-sm" title="Add New" onclick="jQuery('#btnAdd').click(); return false;"><i class="fa fa-plus"></i>Add New</a>
        </div>
        <div id="accPagination" class="container-fluid" style="white-space:nowrap"> <%--width:98%;--%>
                <div id="divPaginationHeading" class="panel-heading visible-xs visible-sm "> <%--hidden-md hidden-lg--%>
                    <a id="lnkPaginationHeader" data-toggle="collapse" data-parent="#accPagination" href="#divPagination" title="Pagination Options" style="text-decoration:underline;">
                        <asp:Literal ID="litPaginationHeader" runat="server"></asp:Literal>
                    </a>
                    &nbsp;&nbsp;
                    <button id="btnSearchLink" title="Click for Search Options" class="btn btn-sm btn-link" onclick="jQuery('#<%=btnSearch.ClientID%>').click();return false;">Search</button>
                    &nbsp;&nbsp;
                    <button id="btnAddSmallVw" class="btn btn-sm btn-link" onclick="jQuery('#btnAdd').click(); return false;">Add</button>
                </div>
                <div id="divPagination" style="margin-bottom: -15px;" class="container-fluid visible-md visible-lg"> <%--hidden-xs hidden-sm  style="margin-left: 15px;"--%>
                        <!--pagination-->
                    <div class="row">
                        <!--pages list-->
                        <div id="divPagingOptions" class="col-md-6 pull-left" >
                            <ul class="pagination pagination-sm" style="vertical-align: middle;">
                                <asp:Literal ID="litPagination" runat="server"></asp:Literal>  
                            </ul>
                            <span class="hidden-xs hidden-sm" >
                            <label>Showing </label>
                            <div class="input-group input-group-sm " style="padding-left: 5px;">                                                                      
                                <asp:DropDownList ID="ddlItemsPerPage" CssClass="form-control" onChange="flmNoItemsPerPageReload(this.value);" runat="server" title="Items per page">
                                </asp:DropDownList>
                            </div>
                            <label ><asp:Literal ID="litCurrentPageInfo" runat="server"></asp:Literal></label>
                            </span>
                        </div>
                        <!-- Search -->
                        <div class="col-md-6 pull-right" >
                            <div class="btn btn-group btn-group-sm pull-right" style="padding: 0px; vertical-align: middle;">
                               <%-- <a href="frmListManager.aspx?sesPrefix=<%= hidSessionsPrefix.Value %>&expMode=1&pre=1" class="btn" target="_blank">Export</a>--%>
                                <a id="linkExport" href="#" class="btn btn-default btn-sm" title="Export" onclick="jQuery('#selExportOptions').removeClass('hidden'); jQuery(this).addClass('hidden'); return false;"><i class="fa fa-download"></i></a>
                                <select id="selExportOptions" class="btn btn-default hidden" onchange="exportFLM()">
                                    <option value="">Select...</option> 
                                    <option value="EXCEL">Excel</option>
                                    <option value="WORD">Word</option>                                      
                                    <option value="CSV">CSV</option>
                                    <option value="TXT">TXT</option>
                                    <option value="Cancel">Cancel</option> 
                                      <%-- <option value="PDF">TXT</option>--%>
                                </select>
                                <asp:Button ID="btnSearch" CssClass="hidden" runat="server" Text="" />
                                <a id="linkSearch" href="#" class="btn btn-default btn-sm" title="Search/Filter Options" onclick="jQuery('#<%=btnSearch.ClientID%>').click(); return false;"><i class="fa fa-filter"></i></a>
                                <button ID="btnAdd" class="hidden" <asp:Literal ID="litBtnAdd" runat="server"></asp:Literal> > </button>
                                <a id="linkAdd" href="#" class="btn btn-default btn-sm" title="Add New" onclick="jQuery('#btnAdd').click(); return false;"><i class="fa fa-plus"></i></a>
                            </div>
                            <div class="form-group has-feedback pull-right">
                                <asp:Button ID="btnClearQuickSearch" runat="server" Text="Clear Search Criteria" CssClass="btn btn-sm btn-default pull-right" />
                                <div class="input-group" style="padding-right: 15px;">
                                <asp:TextBox ID="txtQuickSearch" runat="server" CssClass="form-control input-sm quickSearch " 
                                    placeholder="Search..." style="min-width:75px; "
                                    onblur="applyQuickSearch(this.value);" 
                                    onKeyPress="if (enterKeyPressed(event)) { applyQuickSearch(this.value);} ;"></asp:TextBox> 
                                <div id="divQuickSearchAddon" class="input-group-addon" style="cursor: pointer">
                                    <i class="fa fa-search"></i>
                                </div>
                                    </div>
                            </div>

<%--                                <label class="pull-right">
                                    <asp:Literal ID="litSearchFilter" runat="server" ></asp:Literal>
                                </label>--%>
                        </div>
                    </div>
                </div>
            </div>
        

        <div id="divSearch" class="hidden container">
            <div id="SearchGo" class="row">
                    <asp:Button ID="btnSearchClear" runat="server" Text="Clear Search Criteria" CssClass="btn btn-sm btn-default pull-right" />
                    <asp:Button ID="btnSearchGo" runat="server" Text="Apply Search Criteria" CssClass="btn btn-sm btn-default btnSearchGo pull-right" />

            </div>
            <!-- top row field/param selections -->            
            <div id="advSearch1" class="row">
                    <div class="form-group-sm col-xs-12 col-sm-3 col-sm-offset-1">
                        <label for="ddlColumnAdvSearch1">Field</label><br />
                        <asp:DropDownList ID="ddlColumnAdvSearch1" runat="server" CssClass="form-control searchField" style="min-width: 20px;"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12  col-sm-2">
                        <label for="ddlLogicAdvSearch1">Logic</label><br />
                        <asp:DropDownList ID="ddlLogicAdvSearch1" runat="server" CssClass="form-control searchLogic"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12  col-sm-6">
                        <label for="txtColumnAdvSearch1">Value</label><br />
                        <asp:TextBox ID="txtColumnAdvSearch1" runat="server" CssClass="form-control searchText" style="min-width: 200px;" onblur="checkClearShow(this,1);" onKeyup="if (enterKeyPressed(event)) { checkClearShow(this,1); document.getElementById(this.id.replace('1','2')).focus();} else {jQuery('.quickSearch').val(this.value); } ;"></asp:TextBox>                        
                    </div>
                    <%--<div class="form-group-sm col-xs-12  col-sm-1">
                        <label for="btnDeleteAdvSearch1">Clear</label>
                        <asp:Button ID="btnDeleteAdvSearch1" runat="server" CssClass="btn btn-sm btn-link " Text="Clear" OnClientClick="clearAdvSearch(1);" Style="display: none;"></asp:Button>
                    </div>--%>
            </div>
            <div id="advSearch2" class="row">
                    <div class="form-group-sm col-xs-12  col-sm-1">
                        <div class="visible-xs"><hr /></div>
                        <asp:DropDownList ID="ddlAndOrAdvSearch2" runat="server" CssClass="form-control searchAndOr" ></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12  col-sm-3">
                        <div class="visible-xs"><label for="ddlColumnAdvSearch2"><br />Field</label></div>
                        <asp:DropDownList ID="ddlColumnAdvSearch2" runat="server" CssClass="form-control searchField"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-2">
                        <div class="visible-xs"><label for="ddlLogicAdvSearch2"><br />Logic</label></div>
                        <asp:DropDownList ID="ddlLogicAdvSearch2" runat="server" CssClass="form-control searchLogic"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-6">
                        <div class="visible-xs"><label for="txtColumnAdvSearch2"><br />Value</label></div>
                        <asp:TextBox ID="txtColumnAdvSearch2" runat="server" CssClass="form-control searchText" style="min-width: 200px;" onblur="checkClearShow(this,2);" onKeyPress="if (enterKeyPressed(event)) { checkClearShow(this,2); document.getElementById(this.id.replace('2','3')).focus();} ;"></asp:TextBox>
                    </div>
                    <%--<div class="form-group-sm col-xs-1">
                        <asp:Button ID="btnDeleteAdvSearch2" runat="server" CssClass="btn btn-sm btn-link" Text="Clear" OnClientClick="clearAdvSearch(2);" Style="display: none;" />
                    </div>--%>
            </div>
            <div id="advSearch3" class="row">
                   <div class="form-group-sm col-xs-12  col-sm-1">
                        <div class="visible-xs"><hr /></div>
                        <asp:DropDownList ID="ddlAndOrAdvSearch3" runat="server" CssClass="form-control searchAndOr" ></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-3">
                        <div class="visible-xs"><label for="ddlColumnAdvSearch3"><br />Field</label></div>
                        <asp:DropDownList ID="ddlColumnAdvSearch3" runat="server" CssClass="form-control searchField" ></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-2">
                        <div class="visible-xs"><label for="ddlLogicAdvSearch3"><br />Logic</label></div>
                        <asp:DropDownList ID="ddlLogicAdvSearch3" runat="server" CssClass="form-control searchLogic"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-6">
                        <div class="visible-xs"><label for="txtColumnAdvSearch3"><br />Value</label></div>
                        <asp:TextBox ID="txtColumnAdvSearch3" runat="server" CssClass="form-control searchText" style="min-width: 200px;" onblur="checkClearShow(this,3);" onKeyPress="if (enterKeyPressed(event)) { checkClearShow(this,3); document.getElementById(this.id.replace('3','4')).focus();} ;"></asp:TextBox>
                    </div>
                    <%--<div class="form-group-sm col-xs-1">
                        <asp:Button ID="btnDeleteAdvSearch3" runat="server" CssClass="btn btn-sm btn-link" Text="Clear" OnClientClick="clearAdvSearch(3);" Style="display: none;" />
                    </div>--%>
            </div>
            <div id="advSearch4" class="row">
                    <div class="form-group-sm col-xs-12 col-sm-1">
                        <div class="visible-xs"><hr /></div>
                        <asp:DropDownList ID="ddlAndOrAdvSearch4" runat="server" CssClass="form-control searchAndOr"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-3">
                        <div class="visible-xs"><label for="ddlColumnAdvSearch4"><br />Field</label></div>
                        <asp:DropDownList ID="ddlColumnAdvSearch4" runat="server" CssClass="form-control searchField" ></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-2">
                        <div class="visible-xs"><label for="ddlLogicAdvSearch4"><br />Logic</label></div>
                        <asp:DropDownList ID="ddlLogicAdvSearch4" runat="server" CssClass="form-control searchLogic"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-6">
                        <div class="visible-xs"><label for="txtColumnAdvSearch4"><br />Value</label></div>
                        <asp:TextBox ID="txtColumnAdvSearch4" runat="server" CssClass="form-control searchText" style="min-width: 200px;" onblur="checkClearShow(this,4);" onKeyPress="if (enterKeyPressed(event)) { checkClearShow(this,4); document.getElementById(this.id.replace('4','5')).focus();} ;"></asp:TextBox>
                    </div>
                   <%-- <div class="form-group-sm col-xs-12 col-sm-1">
                        <asp:Button ID="btnDeleteAdvSearch4" runat="server" CssClass="btn btn-sm btn-link" Text="Clear" OnClientClick="clearAdvSearch(4);" Style="display: none;" />
                    </div>--%>
            </div>
            <div id="advSearch5" class="row">
                    <div class="form-group-sm col-xs-12 col-sm-1">
                        <div class="visible-xs"><hr /></div>
                        <asp:DropDownList ID="ddlAndOrAdvSearch5" runat="server" CssClass="form-control searchAndOr"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-3">
                        <div class="visible-xs"><label for="ddlColumnAdvSearch5"><br />Field</label></div>
                        <asp:DropDownList ID="ddlColumnAdvSearch5" runat="server" CssClass="form-control searchField" ></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-2">
                        <div class="visible-xs"><label for="ddlLogicAdvSearch5"><br />Logic</label></div>
                        <asp:DropDownList ID="ddlLogicAdvSearch5" runat="server" CssClass="form-control searchLogic"></asp:DropDownList>
                    </div>
                    <div class="form-group-sm col-xs-12 col-sm-6">
                        <div class="visible-xs"><label for="txtColumnAdvSearch5"><br />Value</label></div>
                        <asp:TextBox ID="txtColumnAdvSearch5" runat="server" CssClass="form-control searchText" style="min-width: 200px;" onblur="checkClearShow(this,5);" onKeyPress="if (enterKeyPressed(event)) { checkClearShow(this,5); jQuery('.btnSearchGo').focus();} ;"></asp:TextBox>
                    </div>
                   <%-- <div class="form-group-sm col-xs-12 col-sm-1">
                        <asp:Button ID="btnDeleteAdvSearch5" runat="server" CssClass="btn btn-sm btn-link" Text="Clear" OnClientClick="clearAdvSearch(5);" Style="display: none;" />
                    </div>--%>
            </div>
            
        </div>
        <!--page body-->
                <div id="FLMGridList" runat="server">
                    <div id="divTblListHdr" style="width:98%; overflow: hidden; margin-left: 10px;">
                        <div id="divTblListHdrScrollArea" style=" position:relative">
                        <div class="row">
                        <div class="col-lg-12">
                            <table id="tblListHDR" class="table table-condensed" > <%--table-striped tablesorter scroll"--%>
                                <thead > <%--class="tableheader"--%>
                                    <tr>
                                        <asp:Repeater ID="rprGridHeader" runat="server">
                                            <ItemTemplate>
                                                <%--min-width: 50px; text-align: center; <%# Eval("max-width")%>--%>
                                                <th style="overflow-x: hidden; white-space: nowrap; cursor:pointer;<%# Eval("align")%> " onclick="sortByCol('<%# Eval("Index")%>');">
                                                    <%# Eval("Description")%>&nbsp;<div id='divCol<%# Eval("Index")%>' class="sortCol"></div>
                                                </th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                     </div>
                            </div>
                 </div>

                <div id="divGridContainer" style="width:98%; margin-left: 10px; margin-top: -20px; overflow-y: auto; overflow-x:auto;  white-space: nowrap" onscroll="scrollItH(this)" >  <%--class="tab-pane fade in active"--%>
                    <!--listTable-->
                    <div class="row">
                        <div class="col-lg-12 ">
                                <table id="tblListDTL" class="table table-condensed table-responsive table-striped " > <%--tablesorter scroll"--%>
                                    <tbody>
                                        <asp:Repeater ID="rprGridListing" runat="server">
                                            <ItemTemplate>
                                                <tr id="tr<%# Eval("RECID")%>"><%# Eval("TDs")%> </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                        </div>
                    </div>
                </div>
            </div>
        
    </form>

    <%-- this include pulls in divModalAddUpdate & divModalNotify as they are needed in parent forms--%>
    <!--#include file="Common/inc/frmListManagerModals.inc"-->   
    <!--%%% --><!-- DO NOT DELETE THE %%% USED TO FIND BODY OF FLM IN EMBEDDED DIVS-->
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="pageJavaScripts" runat="server">
    
    <!--%%% --><!-- DO NOT DELETE THE %%% USED TO FIND BODY OF FLM IN EMBEDDED DIVS-->
    <!--this page needs ui for the date picker-use smoothness-->
    <!--#include file="Common/inc/frmListManagerScripts.inc"-->
    <asp:Literal ID="litTopScripts" runat="server"></asp:Literal>   
    <!--#include file="Common/inc/frmListManagerContentScripts.inc"-->
    <asp:Literal ID="litScripts" runat="server"></asp:Literal>
    <%--<script src="Common/js/tableFixedHeader.js"></script>--%>    
    <script>
        jQuery(document).ready(function () {
            jQuery('#loading_indicator').hide();
        });
        function exportFLM() {
            if (jQuery('#selExportOptions').val() != "") {
                if (jQuery('#selExportOptions').val() != "Cancel") {
                    window.open("frmListManager.aspx?sesPrefix=<%= hidSessionsPrefix.Value %>&expMode=1&pre=1&expType=" + jQuery('#selExportOptions').val(), "_blank")
                }
                jQuery('#selExportOptions').val('')
                jQuery('#selExportOptions').addClass('hidden');
                jQuery('#linkExport').removeClass('hidden');
            }
        }
    </script>
    <!--%%% --><!-- DO NOT DELETE THE %%% USED TO FIND BODY OF FLM IN EMBEDDED DIVS-->
</asp:Content>
