
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
    var default_string = "<option value='1'>000000</option>"; //<option value='2'>Approved</option><option value='3'>Rejected</option>";
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


function initEditModal(parentDivIdentifier, updateURL, capital, list_campus, list_rooms, list_program, list_shift, list_lecture_groups, list_courses, list_teachers) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>all fields are required</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='Id' id='modal_" + capital + "_id'>\
            <label class='control-label'>Campus</label>\
            <select id='modal_" + capital + "_campus_id' name='CampusId' class='form-control'>" + generateDDLOptions(list_campus) + "</select>\
            <label class='control-label'>Room</label>\
            <select id='modal_" + capital + "_room_id' name='RoomId' class='form-control'>" + generateDDLOptions(list_rooms) + "</select>\
            <label class='control-label'>Program</label>\
            <select id='modal_" + capital + "_program_id' name='ProgramId' class='form-control'>" + generateDDLOptions(list_program) + "</select>\
            <label class='control-label'>Shift</label>\
            <select id='modal_" + capital + "_shift_id' name='ShiftId' class='form-control'>" + generateDDLOptions(list_shift) + "</select>\
            <label class='control-label'>Start Time (mm/dd/yyyy)</label>\
            <input type='text' name='StartTime' id='modal_" + capital + "_start_time' class='form-control' />\
            <label class='control-label'>End Time (mm/dd/yyyy)</label>\
            <input type='text' name='EndTime' id='modal_" + capital + "_end_time' class='form-control' />\
            <label class='control-label'>Lecture Group</label>\
            <select id='modal_" + capital + "_lecture_group_id' name='LectureGroupId' class='form-control'><option value='0'>0</option>" + generateDDLOptions(list_lecture_groups) + "</select>\
            <label class='control-label'>Course (Code/OFF/Break)</label>\
            <select id='modal_" + capital + "_course_id' name='CourseId' class='form-control'>" + generateDDLOptions(list_courses) + "</select>\
            <label class='control-label'>Study Title</label>\
            <input type='text' class='form-control' name='StudyTitle' id='modal_" + capital + "_study_title' />\
            <label class='control-label'>Lecturer</label>\
            <select id='modal_" + capital + "_employee_teacher_id' name='EmployeeTeacherId' class='form-control'>" + generateDDLOptions(list_teachers) + "</select>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var fieldID = $("#modal_" + capital + "_id");
    var fieldRoomID = $("#modal_" + capital + "_room_id");
    var fieldCourseID = $("#modal_" + capital + "_course_id");
    var fieldLectureGroupID = $("#modal_" + capital + "_lecture_group_id");
    var fieldStudyTitle = $("#modal_" + capital + "_study_title");
    var fieldStartTime = $("#modal_" + capital + "_start_time");
    var fieldEndTime = $("#modal_" + capital + "_end_time");
    var fieldEmployeeTeacherID = $("#modal_" + capital + "_employee_teacher_id");
    var fieldShiftID = $("#modal_" + capital + "_shift_id");
    var fieldProgramID = $("#modal_" + capital + "_program_id");
    var fieldCampusID = $("#modal_" + capital + "_campus_id");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldID).add(fieldRoomID).add(fieldCourseID).add(fieldLectureGroupID).add(fieldStudyTitle).add(fieldStartTime)
        .add(fieldEndTime).add(fieldEmployeeTeacherID).add(fieldShiftID).add(fieldProgramID).add(fieldCampusID);

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
                    RoomId: parseInt(fieldRoomID.val()),
                    ProgramId: parseInt(fieldProgramID.val()),
                    ShiftId: parseInt(fieldShiftID.val()),
                    LectureGroupId: parseInt(fieldLectureGroupID.val()),
                    CourseId: parseInt(fieldCourseID.val()),
                    StudyTitle: fieldStudyTitle.val(),
                    StartTime: fieldStartTime.val(),
                    EndTime: fieldEndTime.val(),
                    EmployeeTeacherId: parseInt(fieldEmployeeTeacherID.val())
                },
                success: function (data) {

                    var str_message = "";

                    if (data.status == "success") {

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html(fieldRoomID.val() + "*");
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(fieldCourseID.val() + "*");
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(fieldLectureGroupID.val() + "*");

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldStudyTitle.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html(fieldStartTime.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(6)').html(fieldStartTime.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("-");

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

    toReturn = function (id, campus_id, room_id, program_id, shift_id, lecture_group_id, course_id, study_title, start_time, end_time, emp_teacher_id) {

        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_campus_id').val(campus_id);
        $('#modal_' + capital + '_room_id').val(room_id);
        $('#modal_' + capital + '_program_id').val(program_id);
        $('#modal_' + capital + '_shift_id').val(shift_id);
        $('#modal_' + capital + '_lecture_group_id').val(lecture_group_id);
        $('#modal_' + capital + '_course_id').val(course_id);
        $('#modal_' + capital + '_study_title').val(study_title);
        $('#modal_' + capital + '_start_time').val(start_time);
        $('#modal_' + capital + '_end_time').val(end_time);
        $('#modal_' + capital + '_employee_teacher_id').val(emp_teacher_id);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_room_id').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, campus_id, room_id, program_id, shift_id, lecture_group_id, course_id, study_title, start_time, end_time, emp_teacher_id) {
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



