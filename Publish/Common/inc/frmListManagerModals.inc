<!--Modal Popup-->
    <div class="modal fade" id="divModalAddUpdate" tabindex="-1" role="dialog" aria-labelledby="divModalAddUpdateLabel" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content" style="padding:5px;">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="divModalAddUpdateTitle" class="modal-title"></h4>
                </div>
                <div class="modal-body-addUpdate modal-body-touch">
                </div>
                <div class="modal-footer">
                    <div class="btn-group pull-left">
                        <button type="button" class="btn btn-danger" id="btnDelete" tabindex="-1" onclick="DeleteData();">Delete</button>
                    </div>
                    <div class="btn-group">
                        <!-- 3/9/15 T3 Check for unique fields -->
                        <button type="button" class="btn btn-success" id="btnSave" onclick="revalidateCustom(); jQuery('#btnAjaxSubmit').click(); return false;">Save</button>
                    </div>
                    <div class="btn-group">
                        <button type="button" class="btn btn-default" id="btnCancel" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
<!-- /.modal -->

<!--Notification Modal Popup-->
    <div class="modal fade" id="divModalNotify" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-labelledby="divModalNotify" aria-hidden="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
                    <h4 class="modal-title"></h4>
                </div>
                <form id="modalForm" method="post" class="form-horizontal" >    <%--5/14/15 took out was preventing modal to popup message style="display: none;"--%>
                    <div class="modal-body-notify modal-body-touch">
                    </div>
                    <div class="modal-footer">
                        <div class="btn-group">
                            <button type="submit" class="btn btn-success" data-dismiss="modal" onclick="FLMNotifyReload();" id="btnOk">OK</button>
                        </div>
                    </div>
                </form>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
<!-- /.modal -->