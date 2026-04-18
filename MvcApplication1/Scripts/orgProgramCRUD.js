
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


function initEditModal(parentDivIdentifier, updateURL, capital, list_category, list_types) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>all fields are required</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='Id' id='modal_" + capital + "_id'>\
            <label class='control-label'>Category</label>\
            <select id='modal_" + capital + "_category_id' name='CategoryId' class='form-control'>" + generateDDLOptions(list_category) + "</select>\
            <label class='control-label'>Program Code</label>\
            <input type='text' id='modal_" + capital + "_program_code' name='ProgramCode' class='form-control' />\
            <label class='control-label'>Program Title</label>\
            <input type='text' id='modal_" + capital + "_program_title' name='ProgramTitle' class='form-control' />\
            <label class='control-label'>Discipline Name</label>\
            <input type='text' id='modal_" + capital + "_discipline_name' name='DisciplineName' class='form-control' />\
            <label class='control-label'>Credit Hours</label>\
            <input type='number' id='modal_" + capital + "_credit_hours' name='CreditHours' class='form-control' />\
            <label class='control-label'>Whole Program Type</label>\
            <select id='modal_" + capital + "_whole_program_type_id' name='WholeProgramTypeId' class='form-control'>" + generateDDLOptions(list_types) + "</select>\
            <label class='control-label'>Whole Program Type Number</label>\
            <select id='modal_" + capital + "_whole_program_type_number' name='WholeProgramTypeNumber' class='form-control'>\
                <option value='1'>01</option>\
                <option value='2'>02</option>\
                <option value='3'>03</option>\
                <option value='4'>04</option>\
                <option value='5'>05</option>\
                <option value='6'>06</option>\
                <option value='7'>07</option>\
                <option value='8'>08</option>\
                <option value='9'>09</option>\
                <option value='10'>10</option>\
                <option value='11'>11</option>\
                <option value='12'>12</option>\
           </select>            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var fieldID = $("#modal_" + capital + "_id");
    var fieldCategoryID = $("#modal_" + capital + "_category_id");
    var fieldProgramCode = $("#modal_" + capital + "_program_code");
    var fieldProgramTitle = $("#modal_" + capital + "_program_title");
    var fieldDisciplineName = $("#modal_" + capital + "_discipline_name");
    var fieldCreditHours = $("#modal_" + capital + "_credit_hours");
    var fieldWholeProgramTypeID = $("#modal_" + capital + "_whole_program_type_id");
    var fieldWholeProgramTypeNumber = $("#modal_" + capital + "_whole_program_type_number");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldID).add(fieldCategoryID).add(fieldProgramCode).add(fieldProgramTitle).add(fieldDisciplineName).add(fieldCreditHours).add(fieldWholeProgramTypeID).add(fieldWholeProgramTypeNumber);

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
                    CategoryId: parseInt(fieldCategoryID.val()),
                    ProgramCode: fieldProgramCode.val(),
                    ProgramTitle: fieldProgramTitle.val(),
                    DisciplineName: fieldDisciplineName.val(),
                    CreditHours: fieldCreditHours.val(),
                    WholeProgramTypeID: parseInt(fieldWholeProgramTypeID.val()),
                    WholeProgramTypeNumber: parseInt(fieldWholeProgramTypeNumber.val())
                },
                success: function (data) {

                    var str_message = "";

                    if (data.status == "success") {

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html("*");//fieldID.val()
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(fieldProgramCode.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(fieldProgramTitle.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldDisciplineName.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html(fieldCreditHours.val());

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(6)').html("-");
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("*");

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

    toReturn = function (id, category_id, program_code, program_title, discipline_name, credit_hours, whole_program_type_id, whole_program_type_number) {

        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_category_id').val(category_id);
        $('#modal_' + capital + '_program_code').val(program_code);
        $('#modal_' + capital + '_program_title').val(program_title);
        $('#modal_' + capital + '_discipline_name').val(discipline_name);
        $('#modal_' + capital + '_credit_hours').val(credit_hours);
        $('#modal_' + capital + '_whole_program_type_id').val(whole_program_type_id);
        $('#modal_' + capital + '_whole_program_type_number').val(whole_program_type_number);


        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_category_id').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, category_id, program_code, program_title, discipline_name, credit_hours, whole_program_type_id, whole_program_type_number) {
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



