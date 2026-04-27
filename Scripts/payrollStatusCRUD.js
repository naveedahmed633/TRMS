
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
    var default_string = "<option value='1'>Unpaid</option><option value='2'>Paid</option>";
    var managed_string = "";
    var strComma = "", strColon = "";

    options = "1:Unpaid,2:Paid";

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
        <div id='dialog-form' class='jqUI-modal' title='Update "+ capital.replace("_", " ") + " Status'>\
        <p class='validationTips'>Select status and update below:</p>\
        <form>\
            <fieldset>\
            <label class='control-label'>Employee Code:&nbsp;</label><span id='modal_" + capital + "_emp_code'></span><br>\
            <label class='control-label'>Full Name:&nbsp;</label><span id='modal_" + capital + "_emp_name'></span><br>\
            <label class='control-label'>Salary Month:&nbsp;</label><span id='modal_" + capital + "_salary_month'></span>\
            <br><br>\
            <input type='hidden' name='Id' id='modal_" + capital + "_id'>\
            <label class='control-label'>Salary Status:</label>\
            <select id='modal_" + capital + "_status' name='StatusId' class='form-control'>" + generateApprovalOptions('1:Unpaid,2:Paid') + "</select>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var fieldID = $("#modal_" + capital + "_id");
    var fieldEmpCode = $("#modal_" + capital + "_emp_code");
    var fieldEmpName = $("#modal_" + capital + "_emp_name");
    var fieldSalaryMonth = $("#modal_" + capital + "_salary_month");
    var fieldStatus = $("#modal_" + capital + "_status");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldEmpCode).add(fieldEmpName).add(fieldSalaryMonth).add(fieldStatus);

    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        //valid = valid && checkLength(fieldFromDate, capital + " id", 10, 20);
        //valid = valid && checkLength(fieldToDate, "status", 10, 20);

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    Id: parseInt(fieldID.val()),
                    StatusId: fieldStatus.val()
                },
                success: function (data) {
                    if (fieldStatus.val() == "1")
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(42)').html("Unpaid");
                    else
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(42)').html("Paid");

                    editDialog.dialog("close");
                    $('#toast').html('Record updated successfully');
                    $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);

                    console.log(data);
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

    toReturn = function (id, emp_code, emp_name, salary_name, status) {
        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_emp_code').text(emp_code);
        $('#modal_' + capital + '_emp_name').text(emp_name);
        $('#modal_' + capital + '_salary_month').text(salary_name);
        $('#modal_' + capital + '_status').val(status);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_status').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, status) {
        event.preventDefault();
        updateItem();
    });


    return toReturn;
}