
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


function initEditModal(parentDivIdentifier, updateURL, capital, list_program, list_types) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>all fields are required</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='Id' id='modal_" + capital + "_id'>\
            <label class='control-label'>Program</label>\
            <select id='modal_" + capital + "_program_id' name='ProgramId' class='form-control'>" + generateDDLOptions(list_program) + "</select>\
            <label class='control-label'>Course Code</label>\
            <input type='text' id='modal_" + capital + "_course_code' name='CourseCode' class='form-control' />\
            <label class='control-label'>Course Title</label>\
            <input type='text' id='modal_" + capital + "_course_title' name='CourseTitle' class='form-control' />\
            <label class='control-label'>Book Name</label>\
            <input type='text' id='modal_" + capital + "_book_name' name='BookName' class='form-control' />\
            <label class='control-label'>Book Author</label>\
            <input type='text' id='modal_" + capital + "_book_author' name='BookAuthor' class='form-control' />\
            <label class='control-label'>Default Program Type</label>\
            <select id='modal_" + capital + "_default_program_type_id' name='DefaultProgramTypeId' class='form-control'>" + generateDDLOptions(list_types) + "</select>\
            <label class='control-label'>Default Program Type Number</label>\
            <select id='modal_" + capital + "_default_program_type_number' name='DefaultProgramTypeNumber' class='form-control'>\
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
            <label class='control-label'>Credit Hours</label>\
            <select id='modal_" + capital + "_credit_hours' name='CreditHours' class='form-control'>\
                <option value='1'>01</option>\
                <option value='2'>02</option>\
                <option value='3'>03</option>\
                <option value='6'>06</option>\
           </select>            \
            <label class='control-label'>Passing Marks</label>\
            <input type='number' id='modal_" + capital + "_passing_marks' name='PassingMarks' class='form-control' />\
            <label class='control-label'>Total Marks</label>\
            <input type='number' id='modal_" + capital + "_total_marks' name='TotalMarks' class='form-control' />\
            <label class='control-label'>Is Active Course?</label>\
           <select id='modal_" + capital + "_is_active_course' name='IsActiveCourse' class='form-control'>\
                <option value='true'>Yes</option>\
                <option value='false'>No</option>\
           </select>            \
             <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var fieldID = $("#modal_" + capital + "_id");
    var fieldProgramID = $("#modal_" + capital + "_program_id");
    var fieldCourseCode = $("#modal_" + capital + "_course_code");
    var fieldCourseTitle = $("#modal_" + capital + "_course_title");
    var fieldBookName = $("#modal_" + capital + "_book_name");
    var fieldBookAuthor = $("#modal_" + capital + "_book_author");
    var fieldDefaultProgramTypeID = $("#modal_" + capital + "_default_program_type_id");
    var fieldDefaultProgramTypeNumber = $("#modal_" + capital + "_default_program_type_number");
    var fieldCreditHours = $("#modal_" + capital + "_credit_hours");
    var fieldPassingMarks = $("#modal_" + capital + "_passing_marks");
    var fieldTotalMarks = $("#modal_" + capital + "_total_marks");
    var fieldIsActiveCourse = $("#modal_" + capital + "_is_active_course");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldID).add(fieldProgramID).add(fieldCourseCode).add(fieldCourseTitle).add(fieldBookName).add(fieldBookAuthor).add(fieldDefaultProgramTypeID).add(fieldDefaultProgramTypeNumber).add(fieldCreditHours).add(fieldPassingMarks).add(fieldTotalMarks).add(fieldIsActiveCourse);

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
                    ProgramId: parseInt(fieldProgramID.val()),
                    CourseCode: fieldCourseCode.val(),
                    CourseTitle: fieldCourseTitle.val(),
                    BookName: fieldBookName.val(),
                    BookAuthor: fieldBookAuthor.val(),
                    DefaultProgramTypeId: parseInt(fieldDefaultProgramTypeID.val()),
                    DefaultProgramTypeNumber: parseInt(fieldDefaultProgramTypeNumber.val()),
                    CreditHours: parseInt(fieldCreditHours.val()),
                    PassingMarks: parseInt(fieldPassingMarks.val()),
                    TotalMarks: parseInt(fieldTotalMarks.val()),
                    IsActiveCourse: (fieldIsActiveCourse.val() == "true")
                },
                success: function (data) {

                    var str_message = "";

                    if (data.status == "success") {

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html("*");//fieldID.val()
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(fieldCourseCode.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(fieldCourseTitle.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldBookName.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html(fieldBookAuthor.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(6)').html(fieldCreditHours.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html(fieldPassingMarks.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(8)').html(fieldTotalMarks.val());

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(8)').html("-");
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

    toReturn = function (id, program_id, course_code, course_title, book_name, book_author, default_program_type_id, default_program_type_number, credit_hours, passing_marks, total_marks, is_active_course) {

        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_program_id').val(program_id);
        $('#modal_' + capital + '_course_code').val(course_code);
        $('#modal_' + capital + '_course_title').val(course_title);
        $('#modal_' + capital + '_book_name').val(book_name);
        $('#modal_' + capital + '_book_author').val(book_author);
        $('#modal_' + capital + '_default_program_type_id').val(default_program_type_id);
        $('#modal_' + capital + '_default_program_type_number').val(default_program_type_number);
        $('#modal_' + capital + '_credit_hours').val(credit_hours);
        $('#modal_' + capital + '_passing_marks').val(passing_marks);
        $('#modal_' + capital + '_total_marks').val(total_marks);
        $('#modal_' + capital + '_is_active_course').val(is_active_course);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_program_id').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, program_id, course_code, course_title, book_name, book_author, default_program_type_id, default_program_type_number, credit_hours, passing_marks, total_marks, is_active_course) {
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
                $('#toast').html('Record deleted successfully');
                $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);

                console.log(data);
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



