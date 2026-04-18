
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




function initDeleteModal(parentDivIdentifier, deleteURL, capital) {

  
    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital + "'>\
        <p>Are you sure you want to delete the following "+ capital + "?</p>\
        <form>\
            <fieldset>\
            <label>"+ capital + " Name: <span id='delete-dialog-field-name'></span></label>\
            <label>"+ capital + " Description: <span id='delete-dialog-field-description'></span></label>\
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

        // get the name and the description of the item that we are removing.
        var name = $('*[data-row="' + rowToRemove + '"] > td:eq(0)').html();
        var description = $('*[data-row="' + rowToRemove + '"] > td:eq(1)').html();

        $('#delete-dialog-field-name').html(name);
        $('#delete-dialog-field-description').html(description);


        //  deleteDialog.dialog("open");
      

        swal({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
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

function initEditModal(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <label class='control-label'>" + capital + " Name</label>\
            <input type='hidden' name='fn_id' id='modal_"+ capital + "_id'>\
            <input type='text' name='name' id='modal_" + capital + "_name' class='form-control'>\
            <label class='control-label'>"+ capital + " Description</label>\
            <textarea name='description' id='modal_"+ capital + "_description' class='form-control'></textarea>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);



    var fieldID = $("#modal_" + capital + "_id");
    var fieldName = $("#modal_" + capital + "_name");
    var fieldDescription = $("#modal_" + capital + "_description");
    var allFields = $([]).add(fieldName).add(fieldDescription);





    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        valid = valid && checkLength(fieldName, capital + " name", 2, 50);
        valid = valid && checkLength(fieldDescription, "description", 0, 100);

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    id: parseInt(fieldID.val()),
                    name: fieldName.val(),
                    description: fieldDescription.val()
                },
                success: function (data) {
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html(fieldName.val());
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(fieldDescription.val());
                    editDialog.dialog("close");
                    window.location.reload();
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
        height: 400,
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

    toReturn = function (id, name, description) {
        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_name').val(name);
        $('#modal_' + capital + '_description').val(description);

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


