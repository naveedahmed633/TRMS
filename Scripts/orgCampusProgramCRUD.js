
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

    //debugger;

    //options = "1:Building-001,5:DMC Building 002";

    if (options.length > 0) {

        if (options.indexOf(',') > 0) {
            strComma = options.split(',');
        } else {
            options = options + ",";
            strComma = options.split(',');
        }

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


function initEditModal(parentDivIdentifier, updateURL, capital, list_campus, list_program) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>all fields are required</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='Id' id='modal_" + capital + "_id'>\
            <label class='control-label'>Campus</label>\
            <select id='modal_" + capital + "_campus_id' name='CampusId' class='form-control'>" + generateDDLOptions(list_campus) + "</select>\
            <label class='control-label'>Program</label>\
            <select id='modal_" + capital + "_program_id' name='ProgramId' class='form-control'>" + generateDDLOptions(list_program) + "</select>\
         <label class='control-label'>Is Active Program?</label>\
           <select id='modal_" + capital + "_is_active_program' name='IsActiveProgram' class='form-control'>\
                <option value='true'>Yes</option>\
                <option value='false'>No</option>\
           </select>            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var fieldID = $("#modal_" + capital + "_id");
    var fieldCampusID = $("#modal_" + capital + "_campus_id");
    var fieldProgramID = $("#modal_" + capital + "_program_id");
    var fieldIsActiveProgram = $("#modal_" + capital + "_is_active_program");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldID).add(fieldCampusID).add(fieldProgramID).add(fieldIsActiveProgram);

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
                    CampusId: parseInt(fieldCampusID.val()),
                    ProgramId: parseInt(fieldProgramID.val()),
                    IsActiveProgram: (fieldIsActiveProgram.val() == "true")
                },
                success: function (data) {

                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html("*");//fieldID.val()
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html("*");
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html("*");


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

    toReturn = function (id, campus_id, program_id, is_active_program) {

        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_campus_id').val(campus_id);
        $('#modal_' + capital + '_program_id').val(program_id);
        $('#modal_' + capital + '_is_active_program').val(is_active_program);


        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_campus_id').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, campus_id, program_id, is_active_program) {
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



