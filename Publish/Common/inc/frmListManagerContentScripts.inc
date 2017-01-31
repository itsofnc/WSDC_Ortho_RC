<script>
  $(document).ready(function () {


  // 2/13/15 t3 update active menu entry based on menu id, not table id
  linkID = jQuery('#' + '<%= hidMenuID.ClientID%>').val();
          if (linkID.length == 0) {
          //no table id was sent in, viewing all frmListManager lists
          } else {
          $('.nav li.active').removeClass('active');
          if (document.getElementById(linkID)) {
          document.getElementById(linkID).className += " active";
          }
          }

          //function needed for frmListManager iFrame
          // signal any parent form that this page is loaded
          // 2/9/15 T3 - disabled session timeout timer if called within an iframe (parent will handle timeout)
          if (self == top) {
          // do nothing
          } else {
          clearInterval(sessionTestTimer);
          try {
          parent.childLoaded();
          }
          catch (err) { }
          }
          });

          jQuery(window).on('resize', function () { paginationShowHide() });
          jQuery('#divPagination').load(function () { paginationShowHide() })

          function showSearch(blnShow) {
          if (blnShow == true) {
          jQuery("#divSearch").removeClass("hidden");
          jQuery("#<%=btnSearch.ClientID%>").prop('value', 'Hide Search');
          jQuery("#btnSearchLink").html('Hide Search');
          jQuery("#<%=btnSearch.ClientID%>").removeAttr('onclick');
          jQuery("#<%=btnSearch.ClientID%>").attr('onclick', 'showSearch(false); return false');
          for (i = 1; i <= 5; i++) {
          ASPClientID = "<%=txtColumnAdvSearch1.ClientID%>".replace("txtColumnAdvSearch1", "");
          textbox = jQuery("#" + ASPClientID + "txtColumnAdvSearch" + i)[0];
          //checkClearShow(textbox, i)
          }
          } else {
          jQuery("#divSearch").addClass("hidden");
          jQuery("#<%=btnSearch.ClientID%>").prop('value', 'Search');
          jQuery("#btnSearchLink").html('Search');
          jQuery("#<%=btnSearch.ClientID%>").removeAttr('onclick');
          jQuery("#<%=btnSearch.ClientID%>").attr('onclick', 'showSearch(true); return false');
            }
            }

            function applyQuickSearch(txtBoxValue) {
            //remove all session variables from search criteria.  Form will be resubmitted.
            arrSearchText = jQuery('.searchText');
            waitOnItIn = 0;
            waitOnItOut = 0;
            //for (i = 0; i < arrSearchText.length; i++) {
            //if (arrSearchText[i].value == '') {
            //} else {
              
                jQuery.post('ajaxGetData.aspx', {
                    action: 'removeSearchSessions',
                    blnAll: 'true',
                    sesPrefix: setSessPrefix
                }, function () {
                  
                    if (txtBoxValue == ""){
                         urlParent = window.location.href;
                         if (urlParent.indexOf('frmListManager.aspx') > -1) {
                             document.getElementById('frmListManager').submit();
                         } else {
                             jQuery.get('frmListManager.aspx?pre=1&sesPrefix=' + setSessPrefix,
            function (data) {
            loadDivAreas(data);
            resize();   // 10/14/15 - cpb add resize
            });
            }
            } else {
            arrSearchText[0].value = txtBoxValue;
            jQuery("#<%= btnSearchGo.Clientid %>").click();
            }


            });
            //}

            //}
            }


            function checkClearShow(txtBoxId, searchId) {
            //no longer used
            };

            function clearAdvSearch(searchId) {
            //no longer used
            }




          </script>
