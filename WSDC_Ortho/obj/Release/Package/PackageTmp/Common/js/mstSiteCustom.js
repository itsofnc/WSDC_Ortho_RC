// 5/10/16 T3
// your site custom javascripts for SiteMaster
function genWhereClause(WhereSessionName) {
    jQuery.post("ajaxFunctionsCommonCode.aspx?action=buildWhereDevNotes&sn=" + WhereSessionName,
        function () {
        });
}