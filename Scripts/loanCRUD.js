
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

function initEditModal(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='Id' id='modal_"+ capital + "_id'>\
            <input type='hidden' name='EmployeeId' id='modal_" + capital + "_employee_id'>\
            <input type='hidden' name='LastDeductableAmount' id='modal_" + capital + "_ldeductable_amount'>\
            <label class='control-label'>Employee Code</label>\
            <input type='text' name='EmployeeCode' id='modal_" + capital + "_employee_code' class='form-control' readonly />\
            <label class='control-label'>Employee Name</label>\
            <input type='text' name='EmployeeName' id='modal_" + capital + "_employee_name' class='form-control' readonly />\
            <label class='control-label'>Loan Allocated Date</label>\
            <input type='text' name='LoanADate' id='modal_" + capital + "_loan_adate' class='form-control' readonly />\
            <label class='control-label'>Loan Amount (Rs.)</label>\
            <input type='text' name='LoanAmount' id='modal_" + capital + "_loan_amount' class='form-control' readonly />\
            <label class='control-label'>Installment Numbers</label>\
            <input type='text' name='InstallmentNumbers' id='modal_" + capital + "_installment_numbers' class='form-control' readonly />\
            <label class='control-label'>Installment Amount (Rs.)</label>\
            <input type='text' name='InstallmentAmount' id='modal_" + capital + "_installment_amount' class='form-control' readonly />\
            <label class='control-label'>Deductable Amount (Rs.) (-1 to update Balance)</label>\
            <input type='text' name='DeductableAmount' id='modal_" + capital + "_deductable_amount' class='form-control' />\
            <label class='control-label'>Balance Amount (Rs.)</label>\
            <input type='text' name='BalanceAmount' id='modal_" + capital + "_balance_amount' class='form-control' style='background-color: #eee;' />\
            <label class='control-label'>Status:</label>\
            <select id='modal_" + capital + "_loan_status' name='LoanStatusId' class='form-control'>" + generateApprovalOptions('1:Open,2:Closed') + "</select>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);



    var fieldID = $("#modal_" + capital + "_id");
    var fieldEmployeeID = $("#modal_" + capital + "_employee_id");
    var fieldLoanAmount = $("#modal_" + capital + "_loan_amount");
    var fieldInstallmentNumbers = $("#modal_" + capital + "_installment_numbers");
    var fieldLastDeductableAmount = $("#modal_" + capital + "_ldeductable_amount");
    var fieldInstallmentAmount = $("#modal_" + capital + "_installment_amount");
    var fieldDeductableAmount = $("#modal_" + capital + "_deductable_amount");
    var fieldBalanceAmount = $("#modal_" + capital + "_balance_amount");
    var fieldLoanStatus = $("#modal_" + capital + "_loan_status");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldEmployeeID).add(fieldLoanAmount).add(fieldInstallmentNumbers).add(fieldLastDeductableAmount).add(fieldInstallmentAmount).add(fieldDeductableAmount).add(fieldBalanceAmount).add(fieldLoanStatus);

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
                    EmployeeId: parseInt(fieldEmployeeID.val()),
                    LoanAmount: fieldLoanAmount.val(),
                    InstallmentNumbers: fieldInstallmentNumbers.val(),
                    InstallmentAmount: fieldInstallmentAmount.val(),
                    LoanTypeId: fieldLastDeductableAmount.val(),//actually last deduction
                    DeductableAmount: fieldDeductableAmount.val(),
                    BalanceAmount: fieldBalanceAmount.val(),
                    LoanStatusId: fieldLoanStatus.val()
                },
                success: function (data) {
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(fieldInstallmentAmount.val());
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(fieldDeductableAmount.val());
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldBalanceAmount.val());

                    window.location.href = "/HR/LoansManagement/LoanApplication";

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

    toReturn = function (id, emp_id, employee_code, employee_name, loan_adate, loan_amount, installment_numbers, installment_amount, deductable_amount, balance_amount, loan_status) {
        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_employee_id').val(emp_id);
        $('#modal_' + capital + '_ldeductable_amount').val(deductable_amount);
        $('#modal_' + capital + '_employee_code').val(employee_code);
        $('#modal_' + capital + '_employee_name').val(employee_name);
        $('#modal_' + capital + '_loan_adate').val(loan_adate);
        $('#modal_' + capital + '_loan_amount').val(loan_amount);
        $('#modal_' + capital + '_installment_numbers').val(installment_numbers);
        $('#modal_' + capital + '_installment_amount').val(installment_amount);
        $('#modal_' + capital + '_deductable_amount').val(deductable_amount);
        $('#modal_' + capital + '_balance_amount').val(balance_amount);
        $('#modal_' + capital + '_loan_status').val(loan_status);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_deductable_amount').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, emp_id, employee_code, employee_name, loan_adate, loan_amount, installment_numbers, installment_amount, deduction, balance_amount, loan_status) {
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



