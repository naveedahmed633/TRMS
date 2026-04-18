

function initDeleteModal(parentDivIdentifier, deleteURL, capital) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital + "'>\
       <p>Are you sure you want to delete the record with ID: <span id='delete-dialog-field-id'></span>?</p>\
        <form>\
            <fieldset>\
           <!-- <label>"+ capital + " Name: <span id='delete-dialog-field-name'></span></label>\
            <label>"+ capital + " Description: <span id='delete-dialog-field-description'></span></label> -->\
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    function deleteItem() {

        $.ajax({
            type: "POST",
            url: deleteURL,
            data: {
                id: rowToRemove,
                name: "",
                description: ""
            },
            success: function (data) {

                $('*[data-row="' + rowToRemove + '"]').parent().parent().remove();

                deleteDialog.dialog("close");
                $('#toast2').html('Record deleted successfully');
                $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);

                console.log(data);
            },
            error: function (event) {

                deleteDialog.dialog("close");
                $('#toast2').html('There was an error. The database server might be down.');
                $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
            },
            complete: function (data) {


            },
            dataType: 'text json'
        });
    }

    deleteDialog = $("#delete-dialog-form").dialog({
        autoOpen: false,
        height: 200,
        width: 350,
        modal: true,
        buttons: {
            "Delete": deleteItem,
            Cancel: function () {
                deleteDialog.dialog("close");
            }
        },
        close: function () {

            //form = dialog.find("form");
            //form[0].reset();
            //allFields.removeClass("ui-state-error");
        }
    });

    toReturn = function (id) {
        rowToRemove = id;

        //// get the name and the description of the item that we are removing.
        //var name = $('*[data-row="' + rowToRemove + '"] > td:eq(0)').html();
        //var description = $('*[data-row="' + rowToRemove + '"] > td:eq(1)').html();

        //$('#delete-dialog-field-name').html(name);
        //$('#delete-dialog-field-description').html(description);
        $('#delete-dialog-field-id').html(rowToRemove);

        deleteDialog.dialog("open");
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    deleteDialog.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        deleteItem();


    });


    return toReturn;


}


function generateSelectOptions(mcount) {
    var managed_string = "";

    if (mcount > 0) {
        for (var i = 0; i <= mcount; i++) {
            managed_string += "<option value='" + i + "'>" + i + "</option>";
        }
    }
    else {
        "<option value='0'>0</option>";
    }

    return managed_string;
}

function initEditLeaveSession(parentDivIdentifier, updateURL, capital, st01, lt01, mx01, st02, lt02, mx02, st03, lt03, mx03, st04, lt04, mx04, st05, lt05, mx05, st06, lt06, mx06, st07, lt07, mx07, st08, lt08, mx08, st09, lt09, mx09, st10, lt10, mx10, st11, lt11, mx11, st12, lt12, mx12, st13, lt13, mx13, st14, lt14, mx14, st15, lt15, mx15) {

    var HTML = "\
        <div id='dialog-form2' class='jqUI-modal' title='Edit " + capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='id' id='id'>\
            <input type='hidden' name='userid' id='userid'>\
            <label class='control-label m-t-10'>User Name</label>\
            <input type='text' class='form-control' name='ls_name' id='ls_name' readOnly />\
            <label class='control-label m-t-10'>Session Start Date</label>\
            <input type='text' class='form-control datepicker' name='ls_startdate' id='ls_startdate' required />\
            <label class='control-label m-t-10'>Session End Date</label>\
            <input type='text' class='form-control datepicker' name='ls_enddate' id='ls_enddate' required />\
            <label class='control-label m-t-10'>" + lt01 + " Leave</label>\
            <select class='form-control' name='ddl_SickLeaves' id='ddl_SickLeaves' required>" + generateSelectOptions(mx01) + "</select>\
            <label class='control-label m-t-10'>" + lt02 + " Leave</label>\
            <select class='form-control' name='ddl_CasualLeaves' id='ddl_CasualLeaves' required>" + generateSelectOptions(mx02) + "</select>\
            <label class='control-label m-t-10'>" + lt03 + " Leave</label>\
            <select class='form-control' name='ddl_AnnualLeaves' id='ddl_AnnualLeaves' required>" + generateSelectOptions(mx03) + "</select>\
            <label class='control-label m-t-10'>" + lt04 + " Leave</label>\
            <select class='form-control' name='ddl_OtherLeaves' id='ddl_OtherLeaves' required>" + generateSelectOptions(mx04) + "</select>\
            <label class='control-label m-t-10'>" + lt05 + " Leave</label>\
            <select class='form-control' name='ddl_LeaveType01' id='ddl_LeaveType01' required>" + generateSelectOptions(mx05) + "</select>\
            <label class='control-label m-t-10'>" + lt06 + " Leave</label>\
            <select class='form-control' name='ddl_LeaveType02' id='ddl_LeaveType02' required>" + generateSelectOptions(mx06) + "</select>\
            <label class='control-label m-t-10'>" + lt07 + " Leave</label>\
            <select class='form-control' name='ddl_LeaveType03' id='ddl_LeaveType03' required>" + generateSelectOptions(mx07) + "</select>\
            <label class='control-label m-t-10'>" + lt08 + " Leave</label>\
            <select class='form-control' name='ddl_LeaveType04' id='ddl_LeaveType04' required>" + generateSelectOptions(mx08) + "</select>\
            <div style='display: " + (st09 == 'True' ? "block" : "none") + "'><label class='control-label m-t-10'>" + lt09 + " Leave</label><select class='form-control' name='ddl_LeaveType05' id='ddl_LeaveType05' required>" + generateSelectOptions(mx09) + "</select></div>\
            <div style='display: " + (st10 == 'True' ? "block" : "none") + "'><label class='control-label m-t-10'>" + lt10 + " Leave</label><select class='form-control' name='ddl_LeaveType06' id='ddl_LeaveType06' required>" + generateSelectOptions(mx10) + "</select></div>\
            <div style='display: " + (st11 == 'True' ? "block" : "none") + "'><label class='control-label m-t-10'>" + lt11 + " Leave</label><select class='form-control' name='ddl_LeaveType07' id='ddl_LeaveType07' required>" + generateSelectOptions(mx11) + "</select></div>\
            <div style='display: " + (st12 == 'True' ? "block" : "none") + "'><label class='control-label m-t-10'>" + lt12 + " Leave</label><select class='form-control' name='ddl_LeaveType08' id='ddl_LeaveType08' required>" + generateSelectOptions(mx12) + "</select></div>\
            <div style='display: " + (st13 == 'True' ? "block" : "none") + "'><label class='control-label m-t-10'>" + lt13 + " Leave</label><select class='form-control' name='ddl_LeaveType09' id='ddl_LeaveType09' required>" + generateSelectOptions(mx13) + "</select></div>\
            <div style='display: " + (st14 == 'True' ? "block" : "none") + "'><label class='control-label m-t-10'>" + lt14 + " Leave</label><select class='form-control' name='ddl_LeaveType10' id='ddl_LeaveType10' required>" + generateSelectOptions(mx14) + "</select></div>\
            <div style='display: " + (st15 == 'True' ? "block" : "none") + "'><label class='control-label m-t-10'>" + lt15 + " Leave</label><select class='form-control' name='ddl_LeaveType11' id='ddl_LeaveType11' required>" + generateSelectOptions(mx15) + "</select></div>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var id = $('#id');
    var userid = $('#userid');
    var name = $("#ls_name");
    var start_date = $("#ls_startdate");
    var end_date = $("#ls_enddate");
    var start_date = $("#ls_startdate");
    var ddl_sleaves = $("#ddl_SickLeaves");
    var ddl_cleaves = $("#ddl_CasualLeaves");
    var ddl_aleaves = $("#ddl_AnnualLeaves");
    var ddl_oleaves = $("#ddl_OtherLeaves");
    var ddl_leavetype01 = $("#ddl_LeaveType01");
    var ddl_leavetype02 = $("#ddl_LeaveType02");
    var ddl_leavetype03 = $("#ddl_LeaveType03");
    var ddl_leavetype04 = $("#ddl_LeaveType04");
    var ddl_leavetype05 = $("#ddl_LeaveType05");
    var ddl_leavetype06 = $("#ddl_LeaveType06");
    var ddl_leavetype07 = $("#ddl_LeaveType07");
    var ddl_leavetype08 = $("#ddl_LeaveType08");
    var ddl_leavetype09 = $("#ddl_LeaveType09");
    var ddl_leavetype10 = $("#ddl_LeaveType10");
    var ddl_leavetype11 = $("#ddl_LeaveType11");

    var allFields = $([]).add(name).add(start_date).add(end_date).add(ddl_sleaves).add(ddl_cleaves).add(ddl_aleaves).add(ddl_oleaves).add(ddl_LeaveType01).add(ddl_LeaveType02).add(ddl_LeaveType03).add(ddl_LeaveType04).add(ddl_LeaveType05).add(ddl_LeaveType06).add(ddl_LeaveType07).add(ddl_LeaveType08).add(ddl_LeaveType09).add(ddl_LeaveType10).add(ddl_LeaveType11);


    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        //valid = valid && checkLength(fieldName, capital + " name", 2, 50);
        // valid = valid && checkLength(fieldDescription, "description", 0, 100);
        //var check2 = $("#isforacademiccalendar2").is(":checked");
        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    id: parseInt(id.val()),
                    userid: parseInt(userid.val()),
                    str_SessionStartDate: start_date.val(),
                    str_SessionEndDate: end_date.val(),
                    SickLeaves: ddl_sleaves.val(),
                    CasualLeaves: ddl_cleaves.val(),
                    AnnualLeaves: ddl_aleaves.val(),
                    OtherLeaves: ddl_oleaves.val(),
                    LeaveType01: ddl_leavetype01.val(),
                    LeaveType02: ddl_leavetype02.val(),
                    LeaveType03: ddl_leavetype03.val(),
                    LeaveType04: ddl_leavetype04.val(),
                    LeaveType05: ddl_leavetype05.val(),
                    LeaveType06: ddl_leavetype06.val(),
                    LeaveType07: ddl_leavetype07.val(),
                    LeaveType08: ddl_leavetype08.val(),
                    LeaveType09: ddl_leavetype09.val(),
                    LeaveType10: ddl_leavetype10.val(),
                    LeaveType11: ddl_leavetype11.val()
            },
                success: function (data) {
                    if (data.status == "already") {
                        editDialog.dialog("close");
                        $('#toast2').html('Record already exists.');
                        $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                    } else {
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(start_date.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(end_date.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html("-");

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html(ddl_sleaves.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(6)').html(ddl_cleaves.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html(ddl_aleaves.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(8)').html(ddl_oleaves.val());

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(9)').html(ddl_leavetype01.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(10)').html(ddl_leavetype02.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(11)').html(ddl_leavetype03.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(12)').html(ddl_leavetype04.val());

                        editDialog.dialog("close");
                        $('#toast2').html('Updated successfully.');
                        $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                    }
                },
                error: function (event) {
                    
                    editDialog.dialog("close");
                    $('#toast2').html('There was an error. The database server might be down.');
                    $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                },
                complete: function (data) {


                },
                dataType: 'text json'
            });

        }
        return valid;
    }

    editDialog = $("#dialog-form2").dialog({
        autoOpen: false,
        height: 420,
        width: 350,
        modal: true,
        buttons: {
            "Update": updateItem,
            Cancel: function () {
                editDialog.dialog("close");
            }
        },
        close: function () {

            form = editDialog.find("form");
            form[0].reset();
            allFields.removeClass("ui-state-error");
        }
    });


    toReturn = function (id, userid, name, start_date, end_date, sick_leaves, casual_leaves, annual_leaves, other_leaves, leave_type_01, leave_type_02, leave_type_03, leave_type_04, leave_type_05, leave_type_06, leave_type_07, leave_type_08, leave_type_09, leave_type_10, leave_type_11) {

        $('#id').val(id);
        $('#userid').val(userid);
        $('#ls_name').val(name);
        $('#ls_startdate').val(start_date);
        $("#ls_enddate").val(end_date);
        $('#ddl_SickLeaves').val(sick_leaves);
        $("#ddl_CasualLeaves").val(casual_leaves);
        $('#ddl_AnnualLeaves').val(annual_leaves);
        $('#ddl_OtherLeaves').val(other_leaves);
        $('#ddl_LeaveType01').val(leave_type_01);
        $('#ddl_LeaveType02').val(leave_type_02);
        $('#ddl_LeaveType03').val(leave_type_03);
        $('#ddl_LeaveType04').val(leave_type_04);
        $('#ddl_LeaveType05').val(leave_type_05);
        $('#ddl_LeaveType06').val(leave_type_06);
        $('#ddl_LeaveType07').val(leave_type_07);
        $('#ddl_LeaveType08').val(leave_type_08);
        $('#ddl_LeaveType09').val(leave_type_09);
        $('#ddl_LeaveType10').val(leave_type_10);
        $('#ddl_LeaveType11').val(leave_type_11);

        rowBeingEdited = id;


        editDialog.dialog("open");


    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        updateItem();


    });


    return toReturn;
}
