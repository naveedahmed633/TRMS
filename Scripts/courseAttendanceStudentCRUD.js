

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

function initEditCourseAttendanceStudent(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form2' class='jqUI-modal' title='Edit " + capital.replace("_", " ") + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='id' id='modal_id'>\
            <input type='hidden' name='student_id' id='modal_student_id'>\
            <input type='hidden' name='course_id' id='modal_course_id'>\
            <label class='control-label m-t-10'>Schedule Date</label>\
            <input type='text' class='form-control datepicker' name='schedule_date' id='modal_schedule_date' readOnly />\
            <label class='control-label m-t-10'>Student Code</label>\
            <input type='text' class='form-control' name='student_code' id='modal_student_code' readOnly />\
            <label class='control-label m-t-10'>Course Code</label>\
            <input type='text' class='form-control' name='course_code' id='modal_course_code' readOnly />\
            <label class='control-label m-t-10'>Attendance Status</label>\
            <select class='form-control' name='ddl_remarks' id='modal_ddl_remarks' required>\
                <option value='PO' selected='selected'>Present On Time</option>\
                <option value='POE'>Present On Time - Early Out</option>\
                <option value='PLO'>Present Late - On Time Out</option>\
                <option value='PLE'>Present Late - Early Out</option>\
                <option value='PLM'>Present Late - Miss Punch</option>\
                <option value='POM'>Present On Time - Miss Punch</option>\
                <option value='AB'>Absent</option>\
                <option value='OFF'>Off</option>\
            </select>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var id = $('#modal_id');
    var student_id = $('#modal_student_id');
    var course_id = $("#modal_course_id");
    var schedule_date = $("#modal_schedule_date");
    var student_code = $("#modal_student_code");
    var course_code = $("#modal_course_code");
    var ddl_remarks = $("#modal_ddl_remarks");

    var allFields = $([]).add(student_id).add(course_id).add(schedule_date).add(student_code).add(course_code).add(ddl_remarks);


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
                    student_id: parseInt(student_id.val()),
                    course_id: parseInt(course_id.val()),
                    schedule_date: schedule_date.val(),
                    student_code: student_code.val(),
                    course_code: course_code.val(),
                    ddl_remarks: ddl_remarks.val()
                },
                success: function (data) {
                    if (data.status == "already") {
                        editDialog.dialog("close");
                        $('#toast2').html('Record already exists.');
                        $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                    } else {
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html(ddl_remarks.val()+"*");

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


    toReturn = function (id, student_id, course_id, schedule_date, student_code, course_code, ddl_remarks) {

        $('#modal_id').val(id);
        $('#modal_student_id').val(student_id);
        $('#modal_course_id').val(course_id);
        $('#modal_schedule_date').val(schedule_date);
        $("#modal_student_code").val(student_code);
        $("#modal_course_code").val(course_code);
        $('#modal_ddl_remarks').val(ddl_remarks);
        
        rowBeingEdited = id;


        editDialog.dialog("open");

        $('#modal_ddl_remarks').focus();


    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        updateItem();


    });


    return toReturn;
}
