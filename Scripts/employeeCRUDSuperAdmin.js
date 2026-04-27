
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




function initDeleteEmployeeModal(parentDivIdentifier, deleteURL) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete Employee'>\
        <p>Are you sure you want to delete the following Employee?</p>\
        <form>\
            <fieldset>\
            <label>Employee Name: <span id='delete-dialog-field-name'></span></label>\
            <label>Employee Code: <span id='delete-dialog-field-code'></span></label>\
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
            },
            success: function (data) {
                // remove the row from the data table
                $('*[data-id="' + rowToRemove + '"]')
                    .parent()
                    .parent()
                    .remove();


                deleteDialog.dialog("close");
                $('#toast').html('Employee deleted successfully');
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

        var firstName = $('*[data-id="' + rowToRemove + '"]').parent().parent().children().filter(':nth-child(1)').html();
        var lastName = $('*[data-id="' + rowToRemove + '"]').parent().parent().children().filter(':nth-child(2)').html();

        var employeeCode = $('*[data-id="' + rowToRemove + '"]').parent().parent().children().filter(':nth-child(3)').html();

        var name = firstName + " " + lastName;


        $('#delete-dialog-field-name').html(name);
        $('#delete-dialog-field-code').html(employeeCode);


        // deleteDialog.dialog("open");

        swal({
            title: 'Delete the following Employee?',
            text: 'Employee Name: '+name+' Employee Code: ' + employeeCode + '',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            confirmButtonClass: 'btn btn-success mt-2',
            cancelButtonClass: 'btn btn-danger ml-2 mt-2',
            buttonsStyling: false
        }).then(function () {
            swal({
                title: 'Deleted !',
                text: "Your file has been deleted",
                type: 'success',
                confirmButtonClass: 'btn btn-confirm mt-2'
            }
            )
            deleteItem();
        }, function (dismiss) {
            // dismiss can be 'cancel', 'overlay',
            // 'close', and 'timer'
            if (dismiss === 'cancel') {
                swal({
                    title: 'Cancelled',
                    text: "Your imaginary file is safe :)",
                    type: 'error',
                    confirmButtonClass: 'btn btn-confirm mt-2'
                }
                )
            }
        })
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    deleteDialog.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        deleteItem();


    });


    return toReturn;


}

function initEditEmployeeModal(updateURL) {

    // the modal is found in #employeeModals

    // first name and last name should not have extra space characters.
    var fieldFirstname = $('#modal_first_name');

    var fieldLastname = $('#modal_last_name');

    // employee code should not have any non numeric values
    var fieldEmployeeCode = $('#modal_employee_code');

    var fieldEmailAddress = $('#modal_email');

    var fieldAddress = $('#modal_address');

    var fieldMobileNo = $('#modal_mobile_no');

    var fieldDateOfJoining = $('#modal_date_of_joining');

    var fieldDateOfLeaving = $('#modal_date_of_leaving');

    var selectFunction = $('#modal_function');

    var selectDesignation = $('#modal_designation');

    var selectDepartment = $('#modal_department');

    var selectTypeOfEmployment = $('#modal_type_of_employment');

    var selectGrade = $('#modal_grade');

    var selectAccessGroup = $('#modal_access_group');

    var selectTimeTuneStatus = $('#modal_time_tune_status');

    var selectRegion = $('#modal_region');

    var selectLocation = $('#modal_location');

    var ddlSickLeaves = $('#modal_sick_leaves');
    var ddlCasualLeaves = $('#modal_casual_leaves');
    var ddlAnnualLeaves = $('#modal_annual_leaves');
    var upload_photograph = $("#modal_photograph");
    var upload_file_01 = $("#modal_file_01");
    var upload_file_02 = $("#modal_file_02");
    var upload_file_03 = $("#modal_file_03");
    var upload_file_04 = $("#modal_file_04");
    var fieldEmergencyContact01 = $("#modal_emergency_contact_01");
    var fieldEmergencyContact02 = $("#modal_emergency_contact_02");
    var fieldDescription = $("#modal_description");
    var fieldDateOfBirth = $("#modal_date_of_birth");
    var fieldSkillSet = $("#skills_set");



    // all the fields that need to be verified.
    //debugger;
    var allFields = $([])
    .add(fieldFirstname)
    .add(fieldLastname)
    .add(fieldEmployeeCode)
    .add(fieldEmailAddress)
    .add(fieldAddress)
    .add(fieldMobileNo)
    .add(fieldDateOfJoining)
    .add(fieldDateOfLeaving)
    .add(selectFunction)
    .add(selectDesignation)
    .add(selectDepartment)
    .add(selectTypeOfEmployment)
    .add(selectGrade)
    .add(selectAccessGroup)
    .add(selectTimeTuneStatus)
    .add(selectRegion)
    .add(selectLocation)
    .add(ddlSickLeaves)
    .add(ddlCasualLeaves)
    .add(ddlAnnualLeaves)
    .add(upload_photograph)
    .add(upload_file_01)
    .add(upload_file_02)
    .add(upload_file_03)
    .add(upload_file_04)
    .add(fieldEmergencyContact01) //newly added
    .add(fieldEmergencyContact02)
    .add(fieldDescription)
    .add(fieldDateOfBirth)
   .add(fieldSkillSet);
    
    function refreshPage() {
        updateItem();
        window.location.href = "ManageEmployee";
    }


    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        valid = valid && checkLength(fieldEmployeeCode, "Employee Code", 1, 10);
        //valid = valid && checkLength(fieldDescription, "description", 0, 100);
        valid = valid && checkSubmit('#employee-modal-form');

        if (upload_photograph.val() != null && upload_photograph.val() != "") {
            updateFileForUserDocs("upload_photograph".replace("upload", "modal"));
        }

        if (upload_file_01.val() != null && upload_file_01.val() != "") {
            updateFileForUserDocs("upload_file_01".replace("upload", "modal"));
        }

        if (upload_file_02.val() != null && upload_file_02.val() != "") {
            updateFileForUserDocs("upload_file_02".replace("upload", "modal"));
        }

        if (upload_file_03.val() != null && upload_file_03.val() != "") {
            updateFileForUserDocs("upload_file_03".replace("upload", "modal"));
        }

        if (upload_file_04.val() != null && upload_file_04.val() != "") {
            updateFileForUserDocs("upload_file_04".replace("upload", "modal"));
        }

        if (valid) {

            var text = $('#skills_set').val();
            if (text != null && text != "") {

                text = text.join();
            }
            //var text = $('#skills_set').map(function () {
            //    return $(this).text();
            //}).get().join(',');
            console.log(text);
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    id: rowBeingEdited,
                    first_name: fieldFirstname.val(),
                    last_name: fieldLastname.val(),
                    employee_code: fieldEmployeeCode.val(),
                    email: fieldEmailAddress.val(),
                    address: fieldAddress.val(),
                    mobile_no: fieldMobileNo.val(),
                    date_of_joining: fieldDateOfJoining.val(),
                    date_of_leaving: fieldDateOfLeaving.val(),
                    date_of_birth: fieldDateOfBirth.val(),
                    function_id: selectFunction.val(),
                    designation_id: selectDesignation.val(),
                    department_id: selectDepartment.val(),
                    type_of_employment_id: selectTypeOfEmployment.val(),
                    access_group_id: selectAccessGroup.val(),
                    time_tune_status: selectTimeTuneStatus.val(),
                    region_id: selectRegion.val(),
                    grade_id: selectGrade.val(),
                    location_id: selectLocation.val(),
                    sick_leaves: ddlSickLeaves.val(),
                    casual_leaves: ddlCasualLeaves.val(),
                    annual_leaves: ddlAnnualLeaves.val(),
                    photograph: upload_photograph.val(),
                    file_01: upload_file_01.val(),
                    file_02: upload_file_02.val(),
                    file_03: upload_file_03.val(),
                    file_04: upload_file_04.val(),
                    emergency_contact_01: fieldEmergencyContact01.val(),//new
                    emergency_contact_02: fieldEmergencyContact02.val(),
                    description: fieldDescription.val(),
                    modal_skills_set: text
                },
                success: function (data) {

                    // set first_name
                    $('*[data-id="' + rowBeingEdited + '"]')
                        .parent().parent().children().filter(':nth-child(1)')
                        .html(fieldFirstname.val());

                    // set last_name
                    $('*[data-id="' + rowBeingEdited + '"]')
                        .parent().parent().children()
                        .filter(':nth-child(2)')
                        .html(fieldLastname.val());

                    // set employee_code
                    $('*[data-id="' + rowBeingEdited + '"]')
                        .parent().parent().children()
                        .filter(':nth-child(3)')
                        .html(fieldEmployeeCode.val());

                    // set function
                    $('*[data-id="' + rowBeingEdited + '"]')
                        .parent().parent().children().filter(':nth-child(4)')
                        .html($("#modal_function option:selected").text());

                    // set designation
                    $('*[data-id="' + rowBeingEdited + '"]')
                        .parent().parent().children().filter(':nth-child(5)')
                        .html($("#modal_designation option:selected").text());

                    location.reload(true);

                    //window.location.href = '/HR/EmployeeManagement/ManageEmployee';

                    //editDialog.dialog("close");
                    //$('#toast').html('Record updated successfully');
                    //$('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);

                    //console.log(data);
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

    editDialog = $("#employee-edit-form").dialog({
        autoOpen: false,
        height: 600,
        width: 300,
        modal: true,
        buttons: {
            "Update": refreshPage,
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

        var employeeCode = rowBeingEdited;
        /**

            Do an ajax call to get all the parameters for the employee specified by employeeCode.
            
            if the status is false do not open the dialog.
            else load all the drop downs with the recieved function,designation, etc
            options. set first_name last_name etc and open the form.

            // open the dialog
            editDialog.dialog("open");


                {
                    id: ''
                    first_name: ''
                    last_name: ''
                    employee_code: ''
                    email: ''
                    address: ''
                    mobile_no: ''
                    date_of_joining: ''
                    date_of_leaving: ''
                    function_id: ''
                    designation_id: ''
                    department_id: ''
                    type_of_employment_id: ''
                    access_group_id: ''
                    region_id: ''
                    grade_id: ''
                    location_id: ''
                }

        **/


        // do an ajax call, to get the employee data
        $.ajax({
            type: "POST",
            url: '/SuperAdmin/SU/GetEmployee',
            data: {
                employee_id: employeeCode
            },
            success: function (data) {


                if (data.success) {

                    fieldFirstname.val(data.first_name);
                    fieldLastname.val(data.last_name);
                    fieldEmployeeCode.val(data.employee_code);
                    fieldEmailAddress.val(data.email);
                    fieldAddress.val(data.address);
                    fieldMobileNo.val(data.mobile_no);
                    fieldDateOfJoining.val(data.date_of_joining);
                    fieldDateOfLeaving.val(data.date_of_leaving);

                    //var values1 = data.skills_set;
                    //console.log(values1);
                    //$("#skills_set_chosen input").val('');
                    //$.each(values1.split(","), function (i, e) {
                       
            
                    //    $("#skills_set_chosen ul.chosen-choices").append("<li class='search-choice'><span>" + e + '<a class="search-choice-close"></a></span></li>');
                    //});

                    //fieldSkillSet.val(data.g_skills_set);
                    fieldDateOfBirth.val(data.date_of_birth);
                    ddlSickLeaves.val(data.sick_leaves);
                    ddlCasualLeaves.val(data.casual_leaves);
                    ddlAnnualLeaves.val(data.annual_leaves);

                    // I made a function for conciseness.
                    function setDropDownVal(dropDown, id, name) {
                        if (name == null || id == null) {
                            return;
                        }
                        dropDown.prop('disabled', true).trigger("chosen:updated");
                        dropDown.empty();
                        //<option value="0">none</option>
                        dropDown.append("<option value='" + id + "'>" + name + "</option>");
                        dropDown.append("<option value='0'>none</option>");
                        dropDown.val(id);
                        dropDown.prop('disabled', false).trigger("chosen:updated");

                    }


                    function setDropDownVal2(dropDown, id, name) {
                        if (name == null || id == null) {
                            return;
                        }
                        dropDown.empty();
                        dropDown.append("<option value='" + id + "'>" + name + "</option>");
                        if (name == "Active") {

                            dropDown.append("<option value='0'>Deactive</option>");
                        }
                        else {
                            dropDown.append("<option value='1'>Active</option>");
                        }
                        //<option value="0">none</option>

                        var key = "'" + id + "'";


                        dropDown.val(id);
                        //console.log("The key is: ");
                        //console.log(key);
                    }

                    setDropDownVal(selectFunction, data.function_id, data.function_name);
                    setDropDownVal(selectDesignation, data.designation_id, data.designation_name);
                    setDropDownVal(selectDepartment, data.department_id, data.department_name);

                    selectAccessGroup.val(data.access_group_id);
                    //selectTimeTuneStatus.val(data.time_tune_status);
                    setDropDownVal(selectGrade, data.grade_id, data.grade_name);
                    setDropDownVal(selectLocation, data.location_id, data.location_name);
                    setDropDownVal(selectRegion, data.region_id, data.region_name);
                    setDropDownVal(selectTypeOfEmployment, data.type_of_employment_id, data.type_of_employment_name);


                    setDropDownVal2(selectTimeTuneStatus, data.time_tune_status_id, data.time_tune_status_val);

                    fieldEmergencyContact01.val(data.emergency_contact_01);
                    fieldEmergencyContact02.val(data.emergency_contact_02);
                    fieldDescription.val(data.description);

                    // open the dialog
                    editDialog.dialog("open");

                } else {


                    editDialog.dialog("close");
                    $('#toast').html('Error in fetching data for this employee code.');
                    $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);

                }




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









    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        updateItem();


    });


    return toReturn;
}

function updateFileForUserDocs(strFileName) {
    var files = $("#" + strFileName).get(0).files;

    var fileData = new FormData();

    //debugger;

    for (var i = 0; i < files.length; i++) {
        fileData.append(strFileName, files[i]);
    }

    $.ajax({
        type: "POST",
        url: "/EmployeeManagement/UploadFilesForUsers",
        dataType: "json",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: fileData,
        success: function (result, status, xhr) {

        },
        error: function (xhr, status, error) {

        }
    });
}