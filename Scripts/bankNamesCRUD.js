function initDeleteModalSkill(parentDivIdentifier, deleteURL, capital) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital + "'>\
        <p>Are you sure you want to delete the following "+ capital + "?</p>\
        <form>\
            <fieldset>\
            <label>"+ capital + " Name: <span id='delete-dialog-field-name'></span></label>\
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
                skillname: "",
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
        var name = $('*[data-row="' + rowToRemove + '"] > td:eq(1)').html();
      

        $('#delete-dialog-field-name').html(name);

        deleteDialog.dialog("open");
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    deleteDialog.find("form").on("submit", function (id, name) {
        event.preventDefault();
        deleteItem();


    });


    return toReturn;


}

function initEditModalSkill(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <label class='control-label'>" + capital + " Name</label>\
            <input type='hidden' name='id' id='modal_"+ capital + "_id'>\
            <input type='text' name='skillname' id='modal_" + capital + "_name' class='form-control'>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);



    var fieldID = $("#modal_" + capital + "_id");
    var fieldName = $("#modal_" + capital + "_name");
    var allFields = $([]).add(fieldName);





    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        valid = valid && checkLength(fieldName, capital + " name", 2, 50);
        

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    id: parseInt(fieldID.val()),
                    BankNameText: fieldName.val(),
                   
                },
                success: function (data) {
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html(fieldName.val());
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

    toReturn = function (id, name) {
        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_name').val(name);

        rowBeingEdited = id;

        editDialog.dialog("open");
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, name) {
        event.preventDefault();
        updateItem();


    });


    return toReturn;
}


