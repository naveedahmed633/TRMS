
function updateValidationTips(tipText) {
    $(".validationTips").text(tipText);

}

function checkLength(field, fieldName, minLength, maxLength) {
    if (minLength === maxLength && field.val().length > maxLength || field.val().length < minLength) {
        field.addClass("ui-state-error");
        updateValidationTips("Length of " + fieldName + " must be " +
            minLength + ".");
        return false;
    }
    else if (field.val().length > maxLength || field.val().length < minLength) {
        field.addClass("ui-state-error");
        updateValidationTips("Length of " + fieldName + " must be between " +
            minLength + " and " + maxLength + ".");
        return false;
    } else {
        return true;
    }
}





function initDeleteShiftModal(deleteURL) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete Shift'>\
        <p>Are you sure you want to delete the following Shift?</p>\
        <form>\
            <fieldset>\
            <label>Shift Name: <span id='delete-dialog-field-name'></span></label>\
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $('#modals').append(HTML);

    function deleteItem() {

        $.ajax({
            type: "POST",
            url: deleteURL,
            data: {
                id: rowToRemove,
            },
            success: function (data) {
                // remove the tr element
                $('*[data-row="' + rowToRemove + '"]').remove();


                deleteDialog.dialog("close");
                $('#toast').html('Shift deleted successfully');
                $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);
            },
            error: function (event) {

                deleteDialog.dialog("close");
                $('#toast').html('There was an error. The database server might be down.');
                $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);
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
        // setting the global variable, this
        // is used in deleteItem()
        rowToRemove = id;

        // get the name and the description of the item that we are removing.

        var shiftName = $('*[data-row="' + rowToRemove + '"]').children().filter(':nth-child(1)').html();

        $('#delete-dialog-field-name').html(shiftName);

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

function initEditShiftModal(updateURL) {


    var HTML = "\
    <div id='shift-edit-form' class='jqUI-modal' title='Edit Shift'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form id='shift_edit_modal'>\
            <fieldset>\
                <div class=\"row\">\
                    <div class=\"col-md-12\">\
                        <label class=\"control-label\">Shift Name</label>\
                        <input type=\"text\" pattern=\".{2,}\" title=\"2 characters minimum\" class=\"form-control\" id=\"modal_shift_name\" \
                               name=\"name\" oninput=\"setCustomValidity('');\" oninvalid=\"setCustomValidity('Name should contain at least 2 characters.');\" required />\
                    </div>\
                </div>\
                <div class=\"row\">\
                    <div class=\"col-md-12\">\
                        <label class=\"control-label\">Early Time</label>\
                        <input type=\"text\" class=\"form-control modal_timepicker\" name=\"early_time\" id=\"modal_early_time\" required>\
                    </div>\
                </div>\
                <div class=\"row\">\
                    <div class=\"col-md-12\">\
                        <label class=\"control-label\">Start Time</label>\
                        <input type=\"text\" class=\"form-control modal_timepicker\" name=\"start_time\" id=\"modal_start_time\" required>\
                    </div>\
                </div>\
                <div class=\"row\">\
                    <div class=\"col-md-12\">\
                        <label class=\"control-label\">Late Time</label>\
                        <input type=\"text\" class=\"form-control modal_timepicker\" name=\"late_time\" id=\"modal_late_time\" required>\
                    </div>\
                </div>\
                <div class=\"row\">\
                    <div class=\"col-md-12\">\
                        <label class=\"control-label\">Half Day</label>\
                        <input type=\"text\" class=\"form-control modal_timepicker\" name=\"half_day\" id=\"modal_half_day\" required>\
                    </div>\
                </div>\
                <div class=\"row\">\
                    <div class=\"col-md-12\">\
                        <label class=\"control-label\">Shift End</label>\
                        <input type=\"text\" class=\"form-control modal_timepicker\" name=\"shift_end\" id=\"modal_shift_end\" required>\
                    </div>\
                </div>\
                <div class=\"row\">\
                    <div class=\"col-md-12\">\
                        <label class=\"control-label\">Day End</label>\
                        <input type=\"text\" class=\"form-control modal_timepicker\" name=\"day_end\" id=\"modal_day_end\" required>\
                    </div>\
                </div>\
                <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
    </div>\
    ";

    $('#modals').append(HTML);
    $('.modal_timepicker').timepicki();

    // the modal is found in #employeeModals

    // first name and last name should not have extra space characters.
    var fieldShiftName = $('#modal_shift_name');

    var fieldEarlyTime = $('#modal_early_time');

    var fieldStartTime = $('#modal_start_time');

    var fieldLateTime = $('#modal_late_time');

    var fieldHalfDay = $('#modal_half_day');

    var fieldShiftEnd = $('#modal_shift_end');

    var fieldDayEnd = $('#modal_day_end');



    // all the fields that need to be verified.
    var allFields = $([])
    .add(fieldShiftName)
    .add(fieldEarlyTime)
    .add(fieldStartTime)
    .add(fieldLateTime)
    .add(fieldHalfDay)
    .add(fieldShiftEnd)
    .add(fieldDayEnd);




    function updateItem() {
        allFields.removeClass("ui-state-error");


        // TODO time validations
        var valid = true;
        valid = valid && checkLength(fieldShiftName, "Shift Name", 2, 25);
        //valid = valid && checkLength(fieldDescription, "description", 0, 100);

        valid = valid && checkShift('#shift_edit_modal');

        if (valid) {





            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    id: rowBeingEdited,
                    name: fieldShiftName.val(),
                    early_time: fieldEarlyTime.val(),
                    start_time: fieldStartTime.val(),
                    late_time: fieldLateTime.val(),
                    half_day: fieldHalfDay.val(),
                    shift_end: fieldShiftEnd.val(),
                    day_end: fieldDayEnd.val()
                },
                success: function (data) {

                    $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(1)')
                        .html(fieldShiftName.val());

                    $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(2)')
                        .html(fieldEarlyTime.val());

                    $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(3)')
                        .html(fieldStartTime.val());


                    $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(4)')
                        .html(fieldLateTime.val());

                    $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(5)')
                        .html(fieldHalfDay.val());


                    $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(6)')
                        .html(fieldShiftEnd.val());


                    $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(7)')
                        .html(fieldDayEnd.val());









                    editDialog.dialog("close");
                    $('#toast').html('Record updated successfully');
                    $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);


                },
                error: function (event) {

                    editDialog.dialog("close");
                    $('#toast').html('There was an error. The database server might be down.');
                    $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);
                },
                complete: function (data) {


                },
                dataType: 'text json'
            });

        }
        return valid;
    }

    editDialog = $("#shift-edit-form").dialog({
        autoOpen: false,
        height: 600,
        width: 300,
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

    toReturn = function (id) {
        // set the rowBeingEdited global.
        rowBeingEdited = id;

        // get all the fields of the modal
        // first name and last name should not have extra space characters.
        var fieldShiftName = $('#modal_shift_name');

        var fieldEarlyTime = $('#modal_early_time');

        var fieldStartTime = $('#modal_start_time');

        var fieldLateTime = $('#modal_late_time');

        var fieldHalfDay = $('#modal_half_day');

        var fieldShiftEnd = $('#modal_shift_end');

        var fieldDayEnd = $('#modal_day_end');


        fieldShiftName.val(
            $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(1)').html()
            );

        fieldEarlyTime.val(
            $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(2)').html()
            );

        fieldStartTime.val(
            $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(3)').html()
            );

        fieldLateTime.val(
            $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(4)').html()
            );

        fieldHalfDay.val(
            $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(5)').html()
            );

        fieldShiftEnd.val(
            $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(6)').html()
            );

        fieldDayEnd.val(
            $('*[data-row="' + rowBeingEdited + '"]')
                        .children().filter(':nth-child(7)').html()
            );

        // open the dialog
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