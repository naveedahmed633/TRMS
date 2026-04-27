



function initDeleteModalSkill(parentDivIdentifier, deleteURL, capital) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital + "'>\
        <p>Are you sure you want to delete the "+ capital + "?</p>\
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
                id: rowToRemove

            },
            success: function (data) {

                $('*[data-row="' + rowToRemove + '"]').parent().parent().remove();

                //deleteDialog.dialog("close");
                //$('#toast').html('Record deleted successfully');
                //$('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);
                location.reload();
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
            <input type='hidden' name='tid' id='tid'>\
            <label class='control-label'>Terminal Name</label>\
            <input type='text' name='tterminal_name' id='tterminal_name' class='form-control'>\
            <label class='control-label'>Location</label>\
            <input type='text' name='tterminal_id' id='tterminal_id' class='form-control'>\
            <label class='control-label'>Branch Code</label>\
            <input type='text' name='bbranch_code' id='bbranch_code' class='form-control'>\
            <label class='control-label'>Branch Name</label>\
            <input type='text' name='bbranch_name' id='bbranch_name' class='form-control'>\
            <label class='control-label'>Terminal Type</label>\
            <select id='tt_type' class='form-control' name='tt_type' required>\
                <option value='None'>None</option>\
                <option value='In' selected>In</option>\
                <option value='Out'>Out</option>\
            </select>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);



    var Id = $("#tid");
    var fieldName = $("#tterminal_name");
    var fieldLocation = $("#tterminal_id");
    var fieldBCode = $("#bbranch_code");
    var fieldBName = $("#bbranch_name");
    var fieldType = $("#tt_type");
    var allFields = $([]).add(fieldName).add(fieldLocation).add(fieldBCode).add(fieldBName).add(fieldType);





    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        //valid = valid && checkLength(fieldName, capital + " name", 2, 50);


        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    id: parseInt(Id.val()),
                    terminal_name: fieldName.val(),
                    terminal_id: fieldLocation.val(),
                    branch_code: fieldBCode.val(),
                    branch_name: fieldBName.val(),
                    t_type: fieldType.val()
                },
                success: function (data) {

                    //alert('done');
                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html(fieldName.val());
                    //editDialog.dialog("close");
                    //$('#toast').html('Record updated successfully');
                    //$('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);
                    window.location.href = "/SuperAdmin/ManageEmployees/AddNewTerminal";
                    //window.location.reload();
                    //console.log(data);
                },
                error: function (event) {

                    window.location.href = "/SuperAdmin/ManageEmployees/AddNewTerminal";


                    //alert('error'+JSON.stringify(event));
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

    toReturn = function (id, t_name, t_location, b_code, b_name, ttype) {
        $('#tid').val(id);
        $('#tterminal_name').val(t_name);
        $('#tterminal_id').val(t_location);
        $('#bbranch_code').val(b_code);
        $('#bbranch_name').val(b_name);
        $('#tt_type').val(ttype);

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


