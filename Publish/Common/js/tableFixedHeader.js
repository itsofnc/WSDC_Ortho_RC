; (function ($) {
    $.fn.fixMe = function () {
        return this.each(function () {
            var $this = $(this),
               $t_fixed,
               $headerCells, // will be set below after fixed header row has been built.
               $bodyCells = $this.find('tbody tr:first').children(),
               colHeaderWidth,
               colBodyWidth;
            function init() {
                
                resizeFixed();
            }
            function resizeFixed() {
                $headerCells = $('#tblListHDR').find('thead tr:first').children(),
                tableReset();
                //$t_fixed.find("th").each(function (index) {
                //    $(this).css("width", $this.find("th").eq(index).outerWidth() + "px");
                //});
            }
            function scrollFixed() {
                var offset = $(this).scrollTop(),
                tableOffsetTop = $this.offset().top,
                tableOffsetBottom = tableOffsetTop + $this.height() - $this.find("thead").height();
                if (offset < tableOffsetTop || offset > tableOffsetBottom)
                    $t_fixed.show();
                else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && $t_fixed.is(":hidden"))
                    $t_fixed.show();
            }
            function tableReset() {

                $headerCells = $("#tblListHDR").find('thead th:first').children() // will be set below after fixed header row has been built.
                $bodyCells = $("#tblListDTL").find('tbody tr:first').children(),
                colHeaderWidth,
                colBodyWidth;

                // Get the tbody columns width array
                colBodyWidth = $bodyCells.map(function () {
                    return $(this).outerWidth();
                }).get();
                colHeaderWidth = $headerCells.map(function () {
                    return $(this).outerWidth();
                }).get();
                // Set the width of thead columns
                //var sortCol = -1;
                $t_fixed.find('thead th').children().each(function (i, v) {
                    //if ($(v).hasClass('fa-angle-down')) {
                    //    sortCol = i
                    //}
                    //alert('hdr '+ colHeaderWidth[i] + ', ' + colBodyWidth[i]);
                    if (colHeaderWidth[i] < colBodyWidth[i]) {
                        //$(v).width(colBodyWidth[i]);
                        $(v).css("min-width", colBodyWidth[i] + 'px')
                    }
                });
                $($this.find('tbody tr')[0]).children().each(function (i, v) {

                    if (colBodyWidth[i] < colHeaderWidth[i]) {
                        //alert('body' + colHeaderWidth[i] + ', ' + colBodyWidth[i]);
                        //$(v).width(colHeaderWidth[i]);
                        $(v).css("min-width", colHeaderWidth[i] + 'px')
                        //alert($(v)[0].style.width);
                        //if (i == sortCol) {
                        //    $(v).width(colHeaderWidth[i] + 10);
                        //} else {
                        //$(v).width(colHeaderWidth[i]);
                        //}
                    }
                });

                // set table body to fixed position
                //$table.find('tbody')[0].style.position = "fixed"
            }

            $(window).resize(resizeFixed);
            $('#divGridContainer').scroll(scrollFixed);
            init();
        });
    };
})(jQuery);

$(document).ready(function () {
    $("table").fixMe();
    $(".up").click(function () {
        $('html, body').animate({
            scrollTop: 0
        }, 2000);
    });
});