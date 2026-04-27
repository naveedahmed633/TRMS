
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


function generateDDLOptions(options) {
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

function initEditModal(parentDivIdentifier, updateURL, capital, list_campuses) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>all fields are required</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='Id' id='modal_" + capital + "_id'>\
            <input type='hidden' name='OrganizationId' id='modal_" + capital + "_organization_id'>\
            <label class='control-label'>Campus Type</label>\
            <select id='modal_" + capital + "_campus_type_id' name='CampusTypeId' class='form-control'>" + generateDDLOptions(list_campuses) + "</select>\
            <label class='control-label'>Campus Code</label>\
            <input type='text' class='form-control' name='CampusCode' id='modal_" + capital + "_campus_code' />\
            <label class='control-label'>Campus Title</label>\
            <input type='text' name='CampusTitle' id='modal_" + capital + "_campus_title' class='form-control' />\
            <label class='control-label'>Email Address</label>\
            <input type='text' name='EmailAddress' id='modal_" + capital + "_email_address' class='form-control' />\
            <label class='control-label'>Address</label>\
            <input type='text' name='Address' id='modal_" + capital + "_address' class='form-control' />\
            <label class='control-label'>Zip Code</label>\
            <input type='text' name='ZipCode' id='modal_" + capital + "_zip_code' class='form-control' />\
            <label class='control-label'>Phone 01</label>\
            <input type='text' name='Phone01' id='modal_" + capital + "_phone_01' class='form-control' />\
            <label class='control-label'>Phone 02</label>\
            <input type='text' name='Phone02' id='modal_" + capital + "_phone_02' class='form-control' />\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var fieldID = $("#modal_" + capital + "_id");
    var fieldOrgID = $("#modal_" + capital + "_organization_id");
    var fieldCampusTypeID = $("#modal_" + capital + "_campus_type_id");
    var fieldCampusCode = $("#modal_" + capital + "_campus_code");
    var fieldCampusTitle = $("#modal_" + capital + "_campus_title");
    var fieldEmailAddress = $("#modal_" + capital + "_email_address");
    var fieldAddress = $("#modal_" + capital + "_address");
    var fieldZipCode = $("#modal_" + capital + "_zip_code");
    var fieldPhone01 = $("#modal_" + capital + "_phone_01");
    var fieldPhone02 = $("#modal_" + capital + "_phone_02");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldID).add(fieldOrgID).add(fieldCampusTypeID).add(fieldCampusCode).add(fieldCampusTitle).add(fieldEmailAddress).add(fieldAddress).add(fieldZipCode).add(fieldPhone01).add(fieldPhone02);

    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        //valid = valid && checkLength(fieldFromDate, capital + " name", 10, 20);
        //valid = valid && checkLength(fieldToDate, "description", 10, 20);

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    Id: parseInt(fieldID.val()),
                    OrganizationId: parseInt(fieldOrgID.val()),
                    CampusTypeId: parseInt(fieldCampusTypeID.val()),
                    CampusCode: fieldCampusCode.val(),
                    CampusTitle: fieldCampusTitle.val(),
                    EmailAddress: fieldEmailAddress.val(),
                    Address: fieldAddress.val(),
                    ZipCode: fieldZipCode.val(),
                    Phone01: fieldPhone01.val(),
                    Phone02: fieldPhone02.val()
                },
                success: function (data) {

                    var str_message = "";

                    if (data.status == "success") {
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html("*");
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(fieldCampusCode.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(fieldCampusTitle.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldEmailAddress.val());

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html("-");

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(6)').html(fieldAddress.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html(fieldZipCode.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(8)').html(fieldPhone01.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(9)').html(fieldPhone02.val());

                        str_message = "data is updated successfully";
                    }
                    else if (data.status == "already") {
                        str_message = "same code already exists";
                    }
                    else {
                        str_message = "error occurred";
                    }

                    editDialog.dialog("close");
                    $('#toast2').html(str_message);
                    $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);

                    console.log(data);
                },
                error: function (event) {

                    editDialog.dialog("close");
                    $('#toast2').html('There was an error. The database server might be down.');
                    $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);
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

    toReturn = function (id, org_id, campus_type_id, campus_code, campus_title, email_address, address, zip_code, phone_01, phone_02) {
        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_organization_id').val(org_id);
        $('#modal_' + capital + '_campus_type_id').val(campus_type_id);
        $('#modal_' + capital + '_campus_code').val(campus_code);
        $('#modal_' + capital + '_campus_title').val(campus_title);
        $('#modal_' + capital + '_email_address').val(email_address);
        $('#modal_' + capital + '_address').val(address);
        $('#modal_' + capital + '_zip_code').val(zip_code);
        $('#modal_' + capital + '_phone_01').val(phone_01);
        $('#modal_' + capital + '_phone_02').val(phone_02);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_campus_type_id').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, org_id, campus_type_id, campus_code, campus_title, email_address, address, zip_code, phone_01, phone_02) {
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
                $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);

                console.log(data);
            },
            error: function (event) {

                deleteDialog.dialog("close");
                $('#toast2').html('There was an error. The database server might be down.');
                $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);
            },
            complete: function (data) {


            },
            dataType: 'text json'
        });
    }

    deleteDialog = $("#delete-dialog-form").dialog({
        autoOpen: false,
        height: 200,
        width: 450,
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



