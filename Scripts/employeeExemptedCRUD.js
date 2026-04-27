

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

function initEditEmployeeExempted(parentDivIdentifier, updateURL, capital) {

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

    var allFields = $([]).add(name).add(start_date).add(end_date).add(ddl_sleaves).add(ddl_cleaves).add(ddl_aleaves).add(ddl_oleaves);


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
                    OtherLeaves: ddl_oleaves.val()
            },
                success: function (data) {
                    if (data.status == "already") {
                        editDialog.dialog("close");
                        $('#toast2').html('Record already exists.');
                        $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                    } else {
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(start_date.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(end_date.val());

                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html(ddl_sleaves.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(6)').html(ddl_cleaves.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html(ddl_aleaves.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(8)').html(ddl_oleaves.val());

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


    toReturn = function (id, userid, name, start_date, end_date, sick_leaves, casual_leaves, annual_leaves, other_leaves, leave_type_01, leave_type_02, leave_type_03, leave_type_04) {

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
