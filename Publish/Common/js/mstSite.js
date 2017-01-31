// history--
// 10/14/15 cpb - add 1 px to resize just to get horizontal scroll bar always on top

jQuery(window).resize(function () {
    resize();
});

jQuery(window).load(function () {
    resize();
});

function resize() {
    if (jQuery('#divNavbarTop')) {
        if (jQuery('#divNavbarTop').hasClass('navbar-fixed-top')) {
            jQuery('body').css('padding-top', parseInt(jQuery('#divNavbarTop').css("height")));
        }
    }

    //resize Gridlist div 09/08/15 T3
    // 10/14/15 cpb added -1px to ensure entrire bottom scroll always seen.
    try {
        intWinHeight = parseInt(jQuery(window).height()) - 1;
        intDivFooterTop = parseInt(jQuery('#divFooter').offset().top);
        intDivFooterBottomTop = parseInt(jQuery('#divFooterBottom').offset().top);
        if (document.getElementById("divGridContainer") !== null) {
            intGridContainerTop = parseInt(jQuery('#divGridContainer').offset().top);
            jQuery('#divGridContainer').css('height', intWinHeight - (intDivFooterBottomTop - intDivFooterTop) - intGridContainerTop);
        }
    } catch (ex) {

    }
}

function setListSession(list) {
    if (list == '') {
        //No specific table, clear session variable(s)
        jQuery.post("ajaxFunctions.aspx?action=clearSessions&Sessions=selectedList||remove||~~selectedId||remove||",
                function () {
                });

    } else {
        //specific table requested, set/clear session variables
        jQuery.post("ajaxFunctions.aspx?action=clearSessions&Sessions=selectedList||remove||~~selectedId||set||" + list,
                function () {
                });
    }
}
function logOut() {
    window.open("frmLogout.aspx", "_self");
}

//-------------------------------------------------------------
//2/4/15-this does work together with session timeout include
//  realized this is extra precaution if users is smart enough to bypass single click of back button
//  however need to make sure it does not run if we have returend to the login page
//  added a span with literal embedded that is set on vb side to the maincontent source page 
// 5/21/15 check flag for whether to allow timeout (this mod is part of modifications to getSessionTimeout.inc)
//          blnTimeoutActive defaulted to true
//          each page that does not require timeout session check will override blnTimeoutActive (true/false)

window.addEventListener("focus", function (event) {
    if (blnTimeoutActive) {        
        jQuery.post("ajaxFunctions.aspx?action=checkSessions",
            function (data) {
                if (data.indexOf("expired") == -1) {
                } else {
                    window.open("frmSessionTimeout.aspx", "_self");
                }
            });
    } else {
        clearInterval(sessionTestTimer);
    }
})