

function initDeleteModal(parentDivIdentifier, deleteURL, capital) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital + "'>\
        <p>Are you sure you want to delete the record with ID: <span id='delete-dialog-field-id'></span>?</p>\
        <form>\
            <fieldset>\
            <!-- <label>"+ capital + " Name: <span id='delete-dialog-field-name'></span></label>\
            <label>" + capital + " Description: <span id='delete-dialog-field-description'></span></label> -->\
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

                //console.log(data);
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
        width: 400,
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
        //var name = $('*[data-row="' + rowToRemove + '"] > td:eq(0)').html();
        //var description = $('*[data-row="' + rowToRemove + '"] > td:eq(1)').html();

        $('#delete-dialog-field-id').html(rowToRemove);

        //$('#delete-dialog-field-name').html(name);
        //$('#delete-dialog-field-description').html(description);


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

function initEditLeaveReason(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form2' class='jqUI-modal' title='Edit " + capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form  id='myform'>\
            <fieldset>\
            <input type='hidden' name='fn_id' id='id'>\
            <label class='control-label'>Reason</label>\
            <input type='text' name='lR_reason' id='lR_reason' class='form-control'>\
            <div style='margin-top: 18px;margin-bottom: 9px;'>\
            <input type='checkbox' class='control-label' id='isforactive' name='isforactive'>\
            <label class='control-label' for='customCheck2'>Is Active?</label>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);


    var id = $('#id');
    var reason = $('#lR_reason');
    var is_active = $("#isforactive");
    
    var allFields = $([]).add(id).add(reason).add(is_active);


    function updateItem() {
        allFields.removeClass("ui-state-error");

      
        var valid = true;

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    Id: parseInt(id.val()),
                    LeaveReasonText: reason.val(),
                    IsActive: is_active.is(":checked"),
           

                },
                success: function (data) {


                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html(reason.val());
                    if ($("#isforactive").is(":checked") == true) {
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html("Active");
                    }
                    else {
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html("InActive");
                    }
               
                  
                   


                    editDialog.dialog("close");
                    //window.location.reload();

                    $('#toast2').html('Updated successfully.');
                    $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);

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
        height: 350,
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


    toReturn = function (id,leave_reson,active_status) {

        $('#id').val(id);
        $('#lR_reason').val(leave_reson);

        if (active_status == "True") {
            $('input[name=isforactive]').attr('checked', true);
        }
        else $('input[name=isforactive]').attr('checked', false);


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



