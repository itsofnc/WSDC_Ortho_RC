<!--getSessionTimeout Include File Begin-->
<!--Creation: 05/05/2014-->
<!--Last Updated: 02/02/2015-->
<!--02/02/15 cpb direct to frmsessiontimeout instead of default-->
<!--DO NOT MODIFY THIS FILE  Modify main file located in the T3ASPUtilities Directory-->
<!--Modification: mm/dd/yy  your comments here EVERY TIME!!!-->

<div class="modal fade" id="divModalSessionTimeout" tabindex="-1" role="dialog" aria-labelledby="divModalSessionTimeoutLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
        <h4 class="modal-title">Session Timeout</h4>
      </div>
      <div class="modal-body">
            <p>Your session is about to expire&hellip;<br />This window will automatically close in 1 minutes.</p>
            <div class="progress progress-striped active">
              <div id="divProgressBarSessionTimeout" class="progress-bar"  role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 1px">
              </div>
            </div>
      </div>
      <div class="modal-footer">
        <button type="button" id="btnTimedOut" class="btn btn-primary hidden" data-dismiss="modal" onclick="timedOut();">Timed Out</button>
        <button type="button" class="btn btn-primary" data-dismiss="modal" onclick="keepWorking();">Keep Working</button>
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->

<div class="modal fade" id="divModalSessionEnded" tabindex="-1" role="dialog" aria-labelledby="divModalSessionEndedLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
        <h4 class="modal-title">Session Ended</h4>
      </div>
      <div class="modal-body">
        <p>
          Your session has ended.
        </p>
      </div>
      <div class="modal-footer">
        <!--<a href="frmSessionTimeout.aspx" class="btn btn-primary" >OK</a>-->
        <a href="frmLogout.aspx" class="btn btn-primary" >OK</a>
      </div>
    </div>
    <!-- /.modal-content -->
  </div>
  <!-- /.modal-dialog -->
</div>
<!-- /.modal -->

<script type="text/javascript">
  jQuery('document').ready(function () {

  jQuery('#divModalSessionTimeout').modal({
  keyboard: false,
  backdrop: 'static',
  show: false
  })
  });

  var blnTimeoutActive = true;                       // 5/21/15 T3 default variable to check session timeout (allows programmer to turn timeout on/off as needed)
  var timeSessionInterval = 1000 * 60 * 5;           // (1-28-15.cpb.WAS 10) minutes
  var sessionSaveTimes = 1;                          // (1-28-15.cpb.--no preserve-time @ 5min..WAS 3) 30 minutes (10*3) -- auto save before popup -- 1 FOR TESTING
  var cntSessionReset = 0;
  var sessionTestTimer = setInterval(testSession, timeSessionInterval);
  var bars = document.getElementById("divProgressBarSessionTimeout");

  function keepWorking() {
  preserveSession();
  //reset popup session timeout
  bars.style.width = '1px';
  cntPopupReset = 0;
  clearInterval(sessionPopupTimer);
  //restart page timer
  resetPageTimer();
  }


  function resetPageTimer() {
  cntSessionReset = 0;
  sessionTestTimer = setInterval(testSession, timeSessionInterval);
  }

  function testSession() {
  preserveSession();
  if (blnTimeoutActive) {
  // retrun to server to save session variables
  if (cntSessionReset > sessionSaveTimes) {
  clearInterval(sessionTestTimer);
  sessionPopupTimer = setInterval(popupSession, 1000);        // every second test for time out popup close
  $('#divModalSessionTimeout').modal('show')
  } else {
  cntSessionReset += 1;
  var time = new Date();              // alert(time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds());
  //preserveSession();
  };
  } else {
  clearInterval(sessionTestTimer);
  }
  }

  function preserveSession() {
  jQuery.post("ajaxFunctions.aspx?action=preserveSessions&Sessions=||preserve||~~");
  }

  //popup timer
  var cntPopupReset = 0;
  var cntPopupMax = 60;       // was 120 2/2/15 cpb i wanted just 1 minute for my app
  var sessionPopupTimer;

  function popupSession() {
  // retrun to server to save session variables
  if (cntPopupReset < cntPopupMax) {
            //update progress bar
            var width = (cntPopupReset / cntPopupMax) * 100;
            bars.style.width = width + '%';
            cntPopupReset += 1;
        } else {
            cntPopupReset += 1;
            
            // we hit our max - go to session timeout page
            document.getElementById('btnTimedOut').click();
        };
    }
    
          function timedOut() {
                  clearInterval(sessionPopupTimer);
                  jQuery.post("ajaxFunctions.aspx?action=timeout");
                  window.open("frmSessionTimeout.aspx", "_self");
          }
    //end of popup timer

</script>

<!--getSessionTimeout Include File End-->