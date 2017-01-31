// history--


// 10/1/15 t3 - add session prefix to header sorting
// Change the selector if needed
//var $table = $('table.scroll'),
//    $headerCells = $table.find('thead tr:first').children(),
//    $bodyCells = $table.find('tbody tr:first').children(),
//    colHeaderWidth,
//    colBodyWidth;

/* 07/28/15 T3*/
var backdropHeight;

// Adjust the width of thead cells when window resizes
jQuery(window).resize(tableReset()); // Trigger resize handler
// end of table scroll script
jQuery(window).resize(setPageHeight());
        
/*8/27/15 testing */
function validate(uid) {
    alert(uid);
}

function setPageHeight() {
    backdropHeight = jQuery('.modal-backdrop').height() - 200;
    jQuery('.modal-body-addUpdate').attr('style', 'max-height:' + backdropHeight + 'px;overflow-y: auto;overflow-x: hidden;');
}

function resizeFunctions() {
    tableReset();
    setPageHeight();
}

var previousTR = "";
var previousTRColor = "";
var highlightColor = jQuery("#highlightColor").css("background-color");

function highlightRow(rowID) {
    if (previousTR == "") {
    } else {
        jQuery(".td" + previousTR).each(function () { this.style.backgroundColor = previousTRColor; })
    }
    previousTR = rowID;
    previousTRColor = jQuery("#tr" + rowID)[0].style.backgroundColor;
    jQuery(".td" + rowID).each(function () { this.style.backgroundColor = highlightColor; })
}




       
jQuery(document).ready(function () {
    // turn off sessionTimeout within iFrame, b/c sessionTimeout is running in parent form... 
    try {
        if (self != top) {
            blnTimeoutActive = false;
        }
    } catch (e) { }

    if (jQuery('#NoNavDDL').length) {
        jQuery("#navDDL").addClass("sr-only");
    }

    jQuery('#divModalAddUpdate').modal({
        keyboard: false,
        backdrop: 'static',
        show: false
    })

    if (jQuery(this).width() <= 768) {
        //tablet or smaller
        jQuery("#collapseOne").collapse("hide");
        jQuery("#chevronAccordian").removeClass("fa-chevron-up");
        jQuery("#chevronAccordian").addClass("fa-chevron-down");

    } else {
        //larger than a tablet
        jQuery("#chevronAccordian").removeClass("fa-chevron-down");
        jQuery("#chevronAccordian").addClass("fa-chevron-up");

    }

    // 10/14/15 cpb reset horizontal scroll bar on opening of pagination on small/xsmall display
    jQuery('#accPagination').on('shown.bs.collapse', function () {
        resize()
    });
    jQuery('#accPagination').on('hidden.bs.collapse', function () {
        resize()
    });
});

//3/13/15 T3 validation at onChange event
function validateUnique(tblID, fldID, obj, fldDescr) {
    fldVal = obj.value;
    jQuery.post('ajaxGetData.aspx', { action: 'validateUniqueField', tbl: tblID, fld: fldID, val: fldVal, descr: fldDescr },
        function (data) {
            //03/13/15 - validate unique field value w/ data 
            if (data.indexOf("%%") > -1) {
                var displayText = data.split("%%")[1];
                confirm(displayText);
                obj.select();
                obj.focus();
            }
        });
}

//09/29/14 T3
// verify required fields
function checkRequiredFields(objName) {
    var error = false;
    var arrTypes = g_FormObjectTypes.split('+');
    var strErrs = "";
    var delim = "";
    var firstErrFound = "";
    jQuery.each(arrTypes, function () {
        jQuery('#' + objName + ' ' + this + '.required').each(function () {
            if (((GetFieldType(this) == 'checkbox' || GetFieldType(this) == 'radio') && this.checked == 'unchecked') ||
                            (GetFieldType(this) == 'select' && this.selectedIndex == 0) ||
                            (this.value.replace(' ', '') == '')) {
                error = true;
                jQuery(this).addClass("requirederror");
                strErrs += delim + jQuery(this).attr('reqmsg');
                delim = "\n";
                if (firstErrFound == "") {
                    firstErrFound = this.id;
                }
            }
            else {
                jQuery(this).removeClass("requirederror");
            }
        });
    });
    if (strErrs == "") {
    } else {
        alert('Unable to Save Data for the following Reason(s):\n\n' + strErrs);
        jQuery('#' + firstErrFound).focus();
    }
    return error;
}

// determine the field type
function GetFieldType(objField) {
    if (jQuery(objField).is('select')) {
        return 'select';
    }
    else if (jQuery(objField).is('textarea')) {
        return 'textarea';
    }
    else {
        return jQuery(objField).attr('type');
    }
}
//060415 T3 if frmname contains ? we are sending in our own parameters
function redirectDataRow(id, frmName, target, mode, iframe, sesPrefix) {
    if (iframe == "true") {
        if (frmName.indexOf("?") >= 0) {
            parent.window.open(frmName + "&cid=" + id + "&rl=" + id + "&sesPrefix=" + sesPrefix, target)
        } else {
            parent.window.open(frmName + "?cid=" + id + "&rl=" + id + "&sesPrefix=" + sesPrefix + "&mid=" + mode, target)
        }
    } else {
        // 1/7/16 T3 not sure why rl is being sent in this url...could be used in a specific project/app
        if (frmName.indexOf("?") >= 0) {
            window.open(frmName + "&cid=" + id + "&rl=" + id + "&sesPrefix=" + sesPrefix, target)
        } else {
            window.open(frmName + "?cid=" + id + "&rl=" + id + "&sesPrefix=" + sesPrefix + "&mid=" + mode, target)
        }
    }
}

// 7/10/14 cpb
function deleteDataRow(recId, tblId, delTitle, delValue) {
    var displayText = "Are you sure you want to remove";
    if (delTitle == '') {
        displayText += " this record?";
    } else {
        displayText += '\n\n' + delTitle + ":  " + delValue + "  ?";
    };
    if (confirm(displayText)) {
        
        jQuery.post('ajaxGetData.aspx', { action: 'delete', id: recId, tb: tblId },
            function (data) {
                strDeleteReturn = data.split("%%")[1];
                if (strDeleteReturn == "") { FLMNotifyReload(); } else {
                    jQuery('#divModalNotify').modal('show');
                    jQuery('.modal-body-notify').html(strDeleteReturn);
                };
                jQuery('#loading_indicator').hide();
            });
        try {
            parent.childLoaded();            
        }
        catch (err) {
        }

        
    }
}

function FLMNotifyReload() {
    jQuery('#loading_indicator').show();
    urlParent = window.location.href;
    if (urlParent.indexOf('frmListManager.aspx') > -1) {
        window.open('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix, '_self');
    } else {
        jQuery.get('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix, '_self',
            function (data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();
                resize();   // 10/14/15 - cpb add resize
            });
        jQuery('#divModalAddUpdate').modal('hide');
    }
}

function flmPagingReload(flmUrl) {
    jQuery('#loading_indicator').show();
    urlParent = window.location.href;
    if (urlParent.indexOf('frmListManager.aspx') > -1) {
        window.open(flmUrl, '_self');
    } else {
        jQuery.get(flmUrl,
            function (data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();
                resize();   // 10/14/15 - resize after paging
            });
        jQuery('#divModalAddUpdate').modal('hide');
    }
}

function getDataRow(id, TableName, TableDescription, viewMode, perm) {
    //Both Script and Modal div must exist in Parent form
    if (self == top) {

        if (TableDescription == "") {
            if (TableName.indexOf("__") > 0) {
                jQuery("#divModalAddUpdateTitle")[0].innerHTML = TableName.split("__")[1];
            } else {
                jQuery("#divModalAddUpdateTitle")[0].innerHTML = TableName;
            }
        } else {
            if (TableDescription.indexOf("__") > 0) {
                jQuery("#divModalAddUpdateTitle")[0].innerHTML = TableDescription.split("__")[1];
            } else {
                jQuery("#divModalAddUpdateTitle")[0].innerHTML = TableDescription;
            }
        }
        var Recid = id;
        //default button visibility
        jQuery("#btnSave")[0].style.display = "block";
        jQuery("#btnCancel")[0].innerHTML = "Cancel";
        //check for delete permission in perm
        var strPerm = perm.toString().substring(2, 3);
        if (strPerm == '0') {
            jQuery("#btnDelete")[0].style.display = "none";
        } else {
            //if in add mode
            if (Recid == -1) {
                jQuery("#btnDelete")[0].style.display = "none";
            } else {
                jQuery("#btnDelete")[0].style.display = "block";
            }
        }
        var view = viewMode;
        if (view == "view") {
            jQuery("#btnSave")[0].style.display = "none";
            jQuery("#btnCancel")[0].innerHTML = "Close";
            jQuery("#btnDelete")[0].style.display = "none";
        }
              
        jQuery('#divModalAddUpdate').modal('show');
        jQuery('#loading_indicator').show();
        jQuery.post('ajaxGetData.aspx', { action: 'read', id: Recid, tb: TableName, vm: viewMode },
                function (html) {
                    jQuery('.modal-body-addUpdate').html(html);
                    //07/28/15 T3
                    setPageHeight();
                    jQuery('#loading_indicator').hide();
                });
    } else {
        try {
            parent.getDataRow(id, TableName, TableDescription, viewMode, perm);
        } catch (ex) {
            alert('Error T3-911: Unable to load data... Contact system administrator for assistance.');
        }
    }
}

var setSessPrefix = ''; //set in vb on load of frmListmanager in litscripts

function SaveData() {
    updateInfo = "";
    delim = "";
    jQuery(".columnback").each(
        function () {
            if (this.className.split(" ")[1] == "boo") {
                if (jQuery("#" + this.id.substr(3))[0].checked) {
                    updateInfo += delim + this.id.substr(3) + "||1";

                } else {
                    updateInfo += delim + this.id.substr(3) + "||0";
                }
            } else {
                if (this.className.split(" ")[1] == "num") {
                    updateInfo += delim + this.id.substr(3) + "||" + jQuery("#" + this.id.substr(3)).val().replace(/,/g, '').replace('$', '');
                } else {
                    updateInfo += delim + this.id.substr(3) + "||" + jQuery("#" + this.id.substr(3)).val();
                }
            }
            delim = "::";
        })
    Recid = jQuery('#postid').val();
    Tbl = jQuery('#posttb').val();
    jQuery.post('ajaxGetData.aspx', { action: 'update', id: Recid, tb: Tbl, dBack: updateInfo },
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
                var displayText = data.split("%%")[1].replace("~~","\n");
                alert(displayText);
                return false
            } else {             
                refreshFLM(Tbl);
                return true
            }        
        });

}

function DeleteData() {
    if (confirm("Are you sure you want to delete this record?")) {
        Recid = jQuery('#postid').val();
        Tbl = jQuery('#posttb').val();
        jQuery.post('ajaxGetData.aspx', { action: 'delete', id: Recid, tb: Tbl },
            function () {
               document.getElementById('frmListManager').submit()
               
            });
    }
}

function tabSelect(tabId) {
    jQuery('#loading_indicator').show();
    jQuery.post("ajaxFunctions.aspx?action=setSessions&Sessions=" + setSessPrefix + "selectedList||set||" + tabId + "~~" + setSessPrefix + "selectedId||remove||",
        function () {
            //document.getElementById('frmListManager').submit();
            jQuery('#loading_indicator').show();
            urlParent = window.location.href;
            if (urlParent.indexOf('frmListManager.aspx') > -1) {
                window.open('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix, '_self');
            } else {
                jQuery.get('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix,
                    function (data) {
                        loadDivAreas(data);
                        jQuery('#loading_indicator').hide();
                        resize();   // 10/14/15 - cpb add resize
                    });
                jQuery('#divModalAddUpdate').modal('hide');
            }
        });
    

}

function refreshFLM(tableId) {
   
    jQuery('#loading_indicator').show();
    //document.getElementById('frmListManager').submit();
    //document.forms.frmListManager.action += '?id=' + tableId;
    urlParent = window.location.href;
    if (urlParent.indexOf('frmListManager.aspx') > -1) {
        window.open('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix + '&id=' + tableId, '_self');
    } else {
        jQuery.get('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix + '&id=' + tableId,
            function (data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();
                resize();   // 10/14/15 - cpb add resize
                jQuery('#divModalAddUpdate').modal('hide');
                $('.modal-backdrop').remove();  // 10/23/2015 T3 modal backdrop sometimes would not go away w/ FLM embedded
            });
    }
    
}

function flmGoToPageReload(intPageNo) {    
    jQuery('#loading_indicator').show();
    urlParent = window.location.href;
    if (urlParent.indexOf('frmListManager.aspx') > -1) {
        window.open('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix + '&pageId=' + intPageNo, '_self');
    } else {
        jQuery.get('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix + '&pageId=' + intPageNo,
            function (data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();

            });
        jQuery('#divModalAddUpdate').modal('hide');
    }
}

function flmNoItemsPerPageReload(noItemsPerPage) {
    jQuery('#loading_indicator').show();
    urlParent = window.location.href;
    if (urlParent.indexOf('frmListManager.aspx') > -1) {
        window.open('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix + '&itemsPerPage=' + noItemsPerPage, '_self');
    } else {
        jQuery.get('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix + '&itemsPerPage=' + noItemsPerPage,
            function (data) {
                loadDivAreas(data);
                jQuery('#loading_indicator').hide();
                resize();   // 10/14/15 - cpb add resize
            });
        jQuery('#divModalAddUpdate').modal('hide');
    }
}

//Function to scroll to top 
//Requires import Content/ScrollToTop folder
jQuery(function () {
    jQuery("#toTop").scrollToTop(1000);
});

function setPaginationChevron(collapsePanel, searchArea) {

    if (jQuery("#" + collapsePanel).hasClass("in")) {
        jQuery("#" + searchArea).removeClass("fa-chevron-up");
        jQuery("#" + searchArea).addClass("fa-chevron-down");
    }
    else {
        jQuery("#" + searchArea).removeClass("fa-chevron-down");
        jQuery("#" + searchArea).addClass("fa-chevron-up");
    }
};



function sortByCol(index) {
    colClicked = "divCol" + index;
    sortSeq = "a"
    //if (jQuery("#" + colClicked).hasClass("fa-angle-down")) {
    if (jQuery("#" + colClicked).hasClass("fa-chevron-down")) {
        //WE have clicked on a column that is sorted asc and need to chamge it to desc
        sortSeq = "d";
    };

    // 10/1/15 t3 - add session prefix to header sorting
    urlParent = window.location.href;
    jQuery('#loading_indicator').show();
    if (urlParent.indexOf('frmListManager.aspx') > -1) {
        window.open('frmListManager.aspx?pre=1&srt=' + index + '&seq=' + sortSeq + '&sesPrefix=' + setSessPrefix, '_self');
    } else {
        jQuery.get('frmListManager.aspx?pre=1&srt=' + index + '&seq=' + sortSeq + '&sesPrefix=' + setSessPrefix,
            function (data) {
                loadDivAreas(data);
                resize();   // 10/14/15 - cpb add resize
                jQuery('#loading_indicator').hide();
            });
    }
    
    ///alert('put it back here  frmListManager.js line 329');
    ///alert(setSessPrefix);
    ///jQuery('#divMyUsers').load('frmListManager.aspx?pre=1&srt=' + index + '&seq=' + sortSeq + '&sesPrefix=' + setSessPrefix + " #mainContent");

}
function paginationShowHide() {
    if (jQuery("#divPagination").width() >= 768) {
        jQuery("#divPagination").collapse("show");
    } else {
        jQuery("#divPagination").collapse("hide");
    }
}



        
        
    $(document).ready(function () {
        $('.input-group input[required], .input-group textarea[required], .input-group select[required]').on('keyup change', function () {
            var $form = $(this).closest('form'),
                $group = $(this).closest('.input-group'),
                $addon = $group.find('.input-group-addon'),
                $icon = $addon.find('span'),
                state = false;

            if (!$group.data('validate')) {
                state = $(this).val() ? true : false;
            } else if ($group.data('validate') == "email") {
                state = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test($(this).val())
            } else if ($group.data('validate') == 'phone') {
                state = /^[(]{0,1}[0-9]{3}[)]{0,1}[-\s\.]{0,1}[0-9]{3}[-\s\.]{0,1}[0-9]{4}$/.test($(this).val())
            } else if ($group.data('validate') == "length") {
                state = $(this).val().length >= $group.data('length') ? true : false;
            } else if ($group.data('validate') == "number") {
                state = !isNaN(parseFloat($(this).val())) && isFinite($(this).val());
            }

            if (state) {
                $addon.removeClass('danger');
                $addon.addClass('success');
                $icon.attr('class', 'glyphicon glyphicon-ok');
            } else {
                $addon.removeClass('success');
                $addon.addClass('danger');
                $icon.attr('class', 'glyphicon glyphicon-remove');
            }

            if ($form.find('.input-group-addon.danger').length == 0) {
                $form.find('[type="submit"]').prop('disabled', false);
            } else {
                $form.find('[type="submit"]').prop('disabled', true);
            }
        });
        $('.input-group input[required], .input-group textarea[required], .input-group select[required]').trigger('change');
    });

    $(document).ready(function () {
        $('#' + 'body_txtPhone').on('keyup change', function () {
            var $form = $(this).closest('form'),
                $group = $(this).closest('.input-group'),
                $addon = $group.find('.input-group-addon'),
                $icon = $addon.find('span'),
                state = /^[(]{0,1}[0-9]{3}[)]{0,1}[-\s\.]{0,1}[0-9]{3}[-\s\.]{0,1}[0-9]{4}$/.test($(this).val());
            if ($(this).val() == '') {
                $addon.removeClass('success');
                $addon.removeClass('danger');
                $addon.addClass('info');
                $icon.attr('class', 'glyphicon glyphicon-asterisk');
            } else if (state) {
                $addon.removeClass('danger');
                $addon.removeClass('info');
                $addon.addClass('success');
                $icon.attr('class', 'glyphicon glyphicon-ok');
            } else {
                $addon.removeClass('success');
                $addon.removeClass('info');
                $addon.addClass('danger');
                $icon.attr('class', 'glyphicon glyphicon-remove');
            }

            if ($form.find('.input-group-addon.danger').length == 0) {
                $form.find('[type="submit"]').prop('disabled', false);
            } else {
                $form.find('[type="submit"]').prop('disabled', true);
            }
        });
        $('#' + 'body_txtPhone').trigger('change');
    });

       
    var waitOnItIn = 0;
    var waitOnItOut = 0;
    function setSearchFilterSession() {
        waitOnItIn = 0;
        waitOnItOut = 0;
        
        // 10/8/15 T3 check search textboxes for values and store to session variables via ajax
        arrSearchAndOr = jQuery('.searchAndOr');
        arrSearchField = jQuery('.searchField');
        arrSearchLogic = jQuery('.searchLogic');
        arrSearchText = jQuery('.searchText');
        blnSearchTextFound = false;

        for (i = 0; i < arrSearchText.length; i++) {
         
                blnSearchTextFound = true;
                AndOrVal = '';
                if (i > 0) {
                    AndOrVal = arrSearchAndOr[i - 1].value;   // there is no AndOr on the first line
                }
                waitOnItIn += 1;
                jQuery.post('ajaxGetData.aspx', {
                    action: 'SetSearchSessions',
                    andOr: AndOrVal,
                    field: arrSearchField[i].value,
                    logic: arrSearchLogic[i].value,
                    text: arrSearchText[i].value,
                    sesPrefix: setSessPrefix,
                    sid: (i + 1)
                }, function () {
                    waitOnItOut += 1;
                    if (waitOnItIn == waitOnItOut) {
                        //10/2015 T3 determine if we came from normal FLM window or embedded via div on a form
                        urlParent = window.location.href;
                        if (urlParent.indexOf('frmListManager.aspx') > -1) {
                            window.open('frmListManager.aspx?scriteria=1&pre=1&sesPrefix=' + setSessPrefix + '&pageId=1', '_self')
                            //document.getElementById('frmListManager').submit();
                        } else {
                            jQuery.get('frmListManager.aspx?scriteria=1&pre=1&sesPrefix=' + setSessPrefix + '&pageId=1',
                            function (data) {
                                loadDivAreas(data);
                                jQuery('#loading_indicator').hide();
                                resize();   // 10/14/15 - cpb add resize
                            });
                        }
                    }
                });
           
        }
        if (blnSearchTextFound == false) { alert('Please enter search criteria to continue.'); jQuery('#loading_indicator').hide(); }
    }

 
    function clearSearch() {
       
            //remove all session variables from search criteria.  Form will be resubmitted.
            arrSearchText = jQuery('.searchText');
            waitOnItIn = 0;
            waitOnItOut = 0;
            intCtr = 0;
            for (i = 0; i < arrSearchText.length; i++) {
                if (arrSearchText[i].value == '') {
                    intCtr += 1;
                } else {
                    waitOnItIn += 1;
                    jQuery.post('ajaxGetData.aspx', {
                        action: 'removeSearchSessions',
                        blnAll: 'false',
                        sesPrefix: setSessPrefix,
                        sid: (i + 1)
                    }, function () {
                        waitOnItOut += 1;
                        if (waitOnItIn == waitOnItOut) {
                            urlParent = window.location.href;
                            if (urlParent.indexOf('frmListManager.aspx') > -1) {
                                document.getElementById('frmListManager').submit();
                            } else {
                                jQuery.get('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix,
                                function (data) {
                                    loadDivAreas(data);
                                    resize();   // 10/14/15 - cpb add resize
                                    jQuery('#loading_indicator').hide();

                                });
                            }
                        }
                    });
                }
            }
            if (intCtr == arrSearchText.length) {
                jQuery('#loading_indicator').hide();
                showSearch(false);
            }
    }

    function enterKeyPressed(e) {
        var keycode;
        if (window.event) {
            keycode = window.event.keyCode;
        } else {
            if (e) {
                keycode = e.which;
            }
        }
        if (keycode == 13) {
            return true;
        } else {
            return false;
        }
    }

    //05/26/15 T3 Set hidden time field with selected time fields(hour/minute/AMPM chosen)
    function setHiddenTime(fldName) {
        strHour = jQuery('#' + fldName + '__hr').val();
        strMin = jQuery('#' + fldName + '__min').val();
        strSec = jQuery('#' + fldName + '__sec').val();
        if (strHour == "") {
            strHour = "00";
        }
        if (strMin == "") {
            strMin = "00";
        }
        if (strSec == "") {
            strSec = "00";
        }
        strAmPm = jQuery('#' + fldName + '__ampm').val();
        if (strAmPm == "PM" && strHour > "00" && strHour != "12") {
            strHour = parseInt(strHour) + 12;
        }
        if (strHour == "00") {
            strmin = "00";
            strAmPm = "AM";
            strSec = "00"
            jQuery('#' + fldName + '__min')[0].selectedIndex = 0;
            jQuery('#' + fldName + '__sec')[0].selectedIndex = 0;
            jQuery('#' + fldName + '__ampm').val("AM");
        }
        jQuery('#' + fldName).val(strHour + ':' + strMin + ':' + strSec);
    }
    function tableReset() {
       
        var
        $t_HDR = $("#tblListHDR"),
        $t_DTL = $("#tblListDTL"),
        $headerCells = $("#tblListHDR").find('thead tr:first').children(), // will be set below after fixed header row has been built.
        $bodyCells = $("#tblListDTL").find('tbody tr:first').children(),
        colHeaderWidth,
        colBodyWidth;

        //12/28/15 T3 add chevron to header we are sorting on        
        sortCol = "1||sortd";
        if (jQuery(".hidSortCol").length == 0) {
        } else {
            sortCol = jQuery(".hidSortCol").val();
        }
        arrSortCol = sortCol.split("||");
        jQuery($t_HDR.find('thead tr')[0]).children().each(function (i, v) {
          
            if (i == parseInt(arrSortCol[0])) {
                if (arrSortCol[1] == 'sortd') {
                    jQuery('#divCol' + i).addClass('fa fa-chevron-up')
                } else {
                    jQuery('#divCol' + i).addClass('fa fa-chevron-down')
                }
            }
        });

        // Get the tbody columns width array
        colBodyWidth = $bodyCells.map(function () {
            return $(this).outerWidth();
        }).get();
        colHeaderWidth = $headerCells.map(function () {
            // 12/22/15 T3 added 5 to width to allow for fa-up/down chevron icon
            return $(this).outerWidth();
        }).get();
        // Set the width of thead columns
        //var sortCol = -1;
        $($t_HDR.find('thead tr')[0]).children().each(function (i, v) {
            //if ($(v).hasClass('fa-angle-down')) {
            //    sortCol = i
            //}
            //alert('hdr '+ colHeaderWidth[i] + ', ' + colBodyWidth[i]);

            //if (i % 2 == 0) {                                 //This code will help see header and detail alignments
            //    $(v).css("background-color", "yellow");
            //    $($t_DTL.find('tbody tr:first').children()[i]).css("background-color", "yellow");
            //} else {
            //    $(v).css("background-color", "grey");
            //    $($t_DTL.find('tbody tr:first').children()[i]).css("background-color", "grey");
            //}

            if (colHeaderWidth[i] < colBodyWidth[i]) {
                //adjust header to width of body
                //$(v).width(colBodyWidth[i]);
                $(v).css("width", colBodyWidth[i] + 'px');
                $(v).css("min-width", colBodyWidth[i] + 'px');
                $(v).css("max-width", colBodyWidth[i] + 'px');
                $($t_DTL.find('tbody tr:first').children()[i]).css("width", colBodyWidth[i] + 'px');
                $($t_DTL.find('tbody tr:first').children()[i]).css("min-width", colBodyWidth[i] + 'px');
                $($t_DTL.find('tbody tr:first').children()[i]).css("max-width", colBodyWidth[i] + 'px');
            } else {
                //adjust body to width of header
                $(v).css("width", colHeaderWidth[i] + 'px');
                $(v).css("min-width", colHeaderWidth[i] + 'px');
                $(v).css("max-width", colHeaderWidth[i] + 'px');
                $($t_DTL.find('tbody tr:first').children()[i]).css("width", colHeaderWidth[i] + 'px');
                $($t_DTL.find('tbody tr:first').children()[i]).css("min-width", colHeaderWidth[i] + 'px');
                $($t_DTL.find('tbody tr:first').children()[i]).css("max-width", colHeaderWidth[i] + 'px');
            }
            //  $($t_DTL.find('tbody tr:first').children()[i]).css("width", colBodyWidth[i] + 'px');
        });
        //$($t_DTL.find('tbody tr')[0]).children().each(function (i, v) {

        //    if (colBodyWidth[i] < colHeaderWidth[i]) {
        //        //alert('body' + colHeaderWidth[i] + ', ' + colBodyWidth[i]);
        //        //$(v).width(colHeaderWidth[i]);
        //        $(v).css("width", colHeaderWidth[i] + 'px')
        //        $(v).css("min-width", colHeaderWidth[i] + 'px')
        //        $(v).css("max-width", colHeaderWidth[i] + 'px')
        //        //     $(v).css("width", colHeaderWidth[i] + 'px')
        //        //alert($(v)[0].style.width);
        //        //if (i == sortCol) {
        //        //    $(v).width(colHeaderWidth[i] + 10);
        //        //} else {
        //        //$(v).width(colHeaderWidth[i]);
        //        //}
        //    }
        //    $($t_HDR.find('thead tr:first').children()[i]).css("width", colHeaderWidth[i] + 'px');
        //    $($t_HDR.find('thead tr:first').children()[i]).css("min-width", colHeaderWidth[i] + 'px');
        //    $($t_HDR.find('thead tr:first').children()[i]).css("max-width", colHeaderWidth[i] + 'px');
        //    //    $($t_HDR.find('thead tr:first').children()[i]).css("width", colHeaderWidth[i] + 'px');
        //})


    }

    function scrollItH(divObj) {
        obj = document.getElementById("divTblListHdrScrollArea");
        obj.style.left = -divObj.scrollLeft + 'px';

    }
