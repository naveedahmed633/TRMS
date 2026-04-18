
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

    options = "1:Pending,2:Approved,3:Rejected";

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
            <input type='hidden' name='Id' id='modal_"+ capital + "_id'>\
            <input type='hidden' name='DesignationId' id='modal_" + capital + "_designation_id'>\
            <input type='hidden' name='GradeId' id='modal_" + capital + "_grade_id'>\
            <label class='control-label'>Designation</label>\
            <input type='text' class='form-control' name='DesignationText' id='modal_" + capital + "_designation_text' readonly />\
            <label class='control-label'>Grade</label>\
            <input type='text' name='GradeText' id='modal_" + capital + "_grade_text' class='form-control' readonly />\
            <label class='control-label'>Basic Pay (Rs.)</label>\
            <input type='text' name='BasicPay' id='modal_" + capital + "_basic_pay' class='form-control' />\
            <label class='control-label'>Increment (Rs.)</label>\
            <input type='text' name='Increment' id='modal_" + capital + "_increment' class='form-control' />\
            <label class='control-label'>Transport (Rs.)</label>\
            <input type='text' name='Transport' id='modal_" + capital + "_transport' class='form-control' />\
            <label class='control-label'>Mobile (Rs.)</label>\
            <input type='text' name='Mobile' id='modal_" + capital + "_mobile' class='form-control' />\
            <label class='control-label'>Medical (Rs.)</label>\
            <input type='text' name='Medical' id='modal_" + capital + "_medical' class='form-control' />\
            <label class='control-label'>Cash Allowance (Rs.)</label>\
            <input type='text' name='Cash Allowance' id='modal_" + capital + "_cash_allowance' class='form-control' />\
            <label class='control-label'>Commission (Rs.)</label>\
            <input type='text' name='Commission' id='modal_" + capital + "_commission' class='form-control' />\
            <label class='control-label'>Food (Rs.)</label>\
            <input type='text' name='Food' id='modal_" + capital + "_food' class='form-control' />\
            <label class='control-label'>Night (Rs.)</label>\
            <input type='text' name='Night' id='modal_" + capital + "_night' class='form-control' />\
            <label class='control-label'>Rent (Rs.)</label>\
            <input type='text' name='Rent' id='modal_" + capital + "_rent' class='form-control' />\
            <label class='control-label'>Group Allowance (Rs.)</label>\
            <input type='text' name='GroupAllowance' id='modal_" + capital + "_group_allowance' class='form-control' />\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);



    //var fieldID = $("#modal_" + capital + "_id");
    //var fieldEmployeeID = $("#modal_" + capital + "_employee_id");
    //var fieldFromDate = $("#modal_" + capital + "_from_date");
    //var fieldToDate = $("#modal_" + capital + "_to_date");
    //var fieldDaysCount = $("#modal_" + capital + "_days_count");
    //var fieldApproverDetail = $("#modal_" + capital + "_approver_detail");
    //var fieldStatus = $("#modal_" + capital + "_status");
    //var fieldIsActive = $("#modal_" + capital + "_is_active");

    var fieldID = $('#modal_' + capital + '_id');
    var fieldDesgId = $('#modal_' + capital + '_designation_id');
    var fieldGrdId = $('#modal_' + capital + '_grade_id');
    var fieldBasicPay = $('#modal_' + capital + '_basic_pay');
    var fieldIncrement = $('#modal_' + capital + '_increment');
    var fieldTransport = $('#modal_' + capital + '_transport');
    var fieldMobile = $('#modal_' + capital + '_mobile');
    var fieldMedical = $('#modal_' + capital + '_medical');
    var fieldCashAllowance = $('#modal_' + capital + '_cash_allowance');
    var fieldCommission = $('#modal_' + capital + '_commission');
    var fieldFood = $('#modal_' + capital + '_food');
    var fieldNight = $('#modal_' + capital + '_night');
    var fieldRent = $('#modal_' + capital + '_rent');
    var fieldGroupAllowance = $('#modal_' + capital + '_group_allowance');

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldDesgId).add(fieldGrdId).add(fieldBasicPay).add(fieldIncrement).add(fieldTransport).add(fieldMobile).add(fieldMedical)
        .add(fieldCashAllowance).add(fieldCommission).add(fieldFood).add(fieldNight).add(fieldRent).add(fieldGroupAllowance);

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
                    DesignationId: parseInt(fieldDesgId.val()),
                    GradeId: fieldGrdId.val(),
                    BasicPay: fieldBasicPay.val(),
                    Increment: fieldIncrement.val(),
                    Transport: fieldTransport.val(),
                    Mobile: fieldMobile.val(),
                    Medical: fieldMedical.val(),
                    CashAllowance: fieldCashAllowance.val(),
                    Commission: fieldCommission.val(),
                    Food: fieldFood.val(),
                    Night: fieldNight.val(),
                    Rent: fieldRent.val(),
                    GroupAllowance: fieldGroupAllowance.val()
                },
                success: function (data) {

                    console.log('edit done...' + rowBeingEdited);

                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(fieldBasicPay.val());
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldIncrement.val());
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(5)').html(fieldTransport.val());
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(6)').html(fieldMobile.val());
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html(fieldMedical.val());
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(8)').html(fieldCashAllowance.val());
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(9)').html(fieldCommission.val());
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(10)').html(fieldFood.val());
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(11)').html(fieldNight.val());
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(12)').html(fieldRent.val());
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(13)').html(fieldGroupAllowance.val());

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

    toReturn = function (id, desg_id, desg_text, grd_id, grd_text, basic_pay, increment, transport, mobile, medical, cash_allowance, commission, food, night, rent, group_allowance) {
        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_designation_id').val(desg_id);
        $('#modal_' + capital + '_designation_text').val(desg_text);
        $('#modal_' + capital + '_grade_id').val(grd_id);
        $('#modal_' + capital + '_grade_text').val(grd_text);
        $('#modal_' + capital + '_basic_pay').val(basic_pay);
        $('#modal_' + capital + '_increment').val(increment);
        $('#modal_' + capital + '_transport').val(transport);
        $('#modal_' + capital + '_mobile').val(mobile);
        $('#modal_' + capital + '_medical').val(medical);
        $('#modal_' + capital + '_cash_allowance').val(cash_allowance);
        $('#modal_' + capital + '_commission').val(commission);
        $('#modal_' + capital + '_food').val(food);
        $('#modal_' + capital + '_night').val(night);
        $('#modal_' + capital + '_rent').val(rent);
        $('#modal_' + capital + '_group_allowance').val(group_allowance);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_basic_pay').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, desg_id, desg_text, grd_id, grd_text, basic_pay, increment, transport, mobile, medical, cash_allowance, commission, food, night, rent, group_allowance) {
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



