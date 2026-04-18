//$(document).ready(function () {

//    $(".monthPicker").datepicker({
//        dateFormat: 'mm-yy',
//        changeMonth: true,
//        changeYear: true,
//        showButtonPanel: true,

//        onClose: function (dateText, inst) {
//            var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
//            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
//            $(this).val($.datepicker.formatDate('yy-mm', new Date(year, month, 1)));
//        }
//    });

//    $(".monthPicker").focus(function () {
//        $(".ui-datepicker-calendar").hide();
//        $("#ui-datepicker-div").position({
//            my: "center top",
//            at: "center bottom",
//            of: $(this)
//        });
//    });

//});


//$('.monthPicker').datepicker({
//    changeMonth: true,
//    changeYear: true,
//    showButtonPanel: true,
//    beforeShow: function (textbox, instance) {
//        instance.dpDiv.css({
//            marginLeft: textbox.offsetWidth + (0) + 'px'
//            //Here -176 is the width of my datepicker div, you can change according to your need.
//        })
//    },
//    dateFormat: 'yy-mm'
//}).focus(function () {
//    var thisCalendar = $(this);
//    $('.ui-datepicker-calendar').detach();
//    $('.ui-datepicker-close').click(function () {
//        var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
//        var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
//        thisCalendar.datepicker('setDate', new Date(year, month, 1));

//    });
//});



$('.monthPicker').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'yy-mm'
    }).focus(function () {
        var thisCalendar = $(this);
        $('.ui-datepicker-calendar').detach();
        $('.ui-datepicker-close').click(function () {
            var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
            thisCalendar.datepicker('setDate', new Date(year, month, 1));
        
        });
    });

