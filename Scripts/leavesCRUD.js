
function updateValidationTips(tipText) {
    $(".validationTips").text(tipText);

}

function checkLength(field, fieldName, minLength, maxLength) {
    if (field.val().length > maxLength || field.val().length < minLength) {
        field.addClass("ui-state-error");
        updateValidationTips("Length of " + fieldName + " must be between " +
            minLength + " and " + maxLength + ".");
        return false;
    } else {
        return true;
    }
}

function checkRegexp(field, regexp, onInvalidTip) {
    if (!(regexp.test(field.val()))) {
        field.addClass("ui-state-error");
        updateValidationTips(onInvalidTip);
        return false;
    } else {
        return true;
    }
}

//////////////////////////////////////// EDIT SECTION ////////////////////////////////////////////////


function generateApprovalOptions(options) {
    var default_string = "<option value='1'>Pending</option><option value='2'>Approved</option><option value='3'>Rejected</option>";
    var managed_string = "";
    var strComma = "", strColon = "";

    //options = "1:Pending,2:Approved,3:Rejected";

    if (options.length > 0) {

        strComma = options.split(',');

        for (var i = 0; i < strComma.length; i++) {

            strColon = strComma[i].split(':');
            //console.log("<option value='" + strColon[0] + "'>" + strColon[1] + "</option>");
            managed_string += "<option value='" + strColon[0] + "'>" + strColon[1] + "</option>";
        }
    }

    if (options.length == 0) {
        return default_string;
    }

    return managed_string;
}

function initEditModal(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <label class='control-label'>From Date</label>\
            <input type='hidden' name='Id' id='modal_"+ capital + "_id'>\
            <input type='hidden' name='EmployeeId' id='modal_" + capital + "_employee_id'>\
            <input type='text' class='form-control datepicker' name='FromDate' id='modal_" + capital + "_from_date' readonly />\
            <label class='control-label'>To Date</label>\
            <input type='text' name='ToDate' id='modal_" + capital + "_to_date' class='form-control datepicker' readonly />\
            <label class='control-label'>Days Count</label>\
            <input type='text' name='DaysCount' id='modal_" + capital + "_days_count' class='form-control' />\
            <label class='control-label'>Approval Status</label>\
            <select id='modal_" + capital + "_status' name='LeaveStatusId' class='form-control'>" + generateApprovalOptions('1:Pending,2:Approved') + "</select>\
            <label class='control-label'>Approver Response</label>\
            <input type='text' name='ApproverDetail' id='modal_" + capital + "_approver_detail' class='form-control' />\
            <label class='control-label' style='display: none;'>Is Active? <input type='checkbox' CHECKED name='IsActive' id='modal_" + capital + "_is_active' class='form-control' /></label>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);



    var fieldID = $("#modal_" + capital + "_id");
    var fieldEmployeeID = $("#modal_" + capital + "_employee_id");
    var fieldFromDate = $("#modal_" + capital + "_from_date");
    var fieldToDate = $("#modal_" + capital + "_to_date");
    var fieldDaysCount = $("#modal_" + capital + "_days_count");
    var fieldApproverDetail = $("#modal_" + capital + "_approver_detail");
    var fieldStatus = $("#modal_" + capital + "_status");
    var fieldIsActive = $("#modal_" + capital + "_is_active");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldFromDate).add(fieldToDate).add(fieldDaysCount).add(fieldStatus).add(fieldIsActive);

    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        valid = valid && checkLength(fieldFromDate, capital + " name", 10, 20);
        valid = valid && checkLength(fieldToDate, "description", 10, 20);

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    Id: parseInt(fieldID.val()),
                    EmployeeId: parseInt(fieldEmployeeID.val()),
                    FromDate: fieldFromDate.val(),
                    ToDate: fieldToDate.val(),
                    DaysCount: fieldDaysCount.val(),
                    ApproverDetail: fieldApproverDetail.val(),
                    LeaveStatusId: fieldStatus.val(),
                    IsActive: fieldIsActive.is(":checked")
                },
                success: function (data) {
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(fieldFromDate.val());
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(fieldToDate.val());

                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldDaysCount.val());

                    if (fieldStatus.val() == "1")
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("Pending");
                    else if (fieldStatus.val() == "2")
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("Approved");
                    else
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("Rejected");

                    editDialog.dialog("close");
                    $('#toast2').html('Record updated successfully');
                    $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);

                    console.log(data);
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

    editDialog = $("#dialog-form").dialog({
        autoOpen: false,
        height: 450,
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

    toReturn = function (id, emp_id, from_date, to_date, days_count, approver_detail, status, is_active) {
        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_employee_id').val(emp_id);
        $('#modal_' + capital + '_from_date').val(from_date);
        $('#modal_' + capital + '_to_date').val(to_date);
        $('#modal_' + capital + '_days_count').val(days_count);
        $('#modal_' + capital + '_approver_detail').val(approver_detail);
        $('#modal_' + capital + '_status').val(status);

        if (is_active == 'True')
            $('#modal_' + capital + '_is_active').attr('checked', true);
        else
            $('#modal_' + capital + '_is_active').attr('checked', false);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_status').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, emp_id, from_date, to_date, days_count, approver_detail, status, is_active) {
        event.preventDefault();
        updateItem();


    });


    return toReturn;
}


/////////////////////////////////////// DELETE SECTION //////////////////////////////////////////////

function initDeleteModal(parentDivIdentifier, deleteURL, capital) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital.replace("_", " ") + "'>\
        <p>Are you sure you want to delete the record with ID: <span id='delete-dialog-field-id'></span>?</p>\
        <form>\
            <fieldset>\
            <!-- <label>"+ capital + " From Date: <span id='delete-dialog-field-name'></span></label> -->\
            <!-- <label>" + capital + " To Date: <span id='delete-dialog-field-description'></span></label> -->\
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
                from_date: "",
                to_date: ""
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
        width: 475,
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

        // get the name and the description of the item that we are removing.
        ////var name = $('*[data-row="' + rowToRemove + '"] > td:eq(0)').html();
        ////var description = $('*[data-row="' + rowToRemove + '"] > td:eq(1)').html();

        $('#delete-dialog-field-id').html(rowToRemove);

        ////$('#delete-dialog-field-name').html(rowToRemove);
        ////$('#delete-dialog-field-description').html(description);


        deleteDialog.dialog("open");
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    deleteDialog.find("form").on("submit", function (id, from_date, to_date, days_count, status) {
        event.preventDefault();
        deleteItem();


    });


    return toReturn;


}



