function initDeleteSLMModal(deleteURL) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete SLM'>\
        <p>Delete the following SLM?</p>\
        <form>\
            <fieldset>\
            <label>Super Line Manager ID: <span id='delete-dialog-field-name'></span></label>\
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $('#modals').append(HTML);

    function deleteItem() {

        $.ajax({
            type: "POST",
            url: deleteURL,
            data: {
                id: rowToRemove,
            },
            success: function (data) {
                // remove the tr element
                $('*[data-id="' + rowToRemove + '"]').parent().parent().remove();


                deleteDialog.dialog("close");
                $('#toast2').html('SLM Group deleted successfully');
                $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
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
        height: 230,
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
        var groupName = $('*[data-id="' + rowToRemove + '"]').parent().parent()
            .children().filter(':nth-child(2)').html();

        $('#delete-dialog-field-name').html(groupName);

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

function initEditSLMModal(updateURL) {


    


    function updateItem() {

        var groupEmployees = new Array();
        $('#modal-employees option:selected').each(function () {
            groupEmployees[groupEmployees.length] = $(this).val()+"";
        });

        // do an ajax call
        $.ajax({
            contentType: 'application/json',
            type: "POST",
            url: updateURL,
            data: JSON.stringify({
                slm_id: SLMID,
                slm_employees: groupEmployees
            }),
            success: function (data) {
                if (data.error == "") {
                   
                    editDialog.dialog("close");
                    $('#toast2').html('Record updated successfully');
                } else {
                    editDialog.dialog("close");
                    $('#toast2').html(error);
                }
                
                



                
                $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);

                //console.log(data);
            },
            error: function (event) {

                editDialog.dialog("close");
                $('#toast2').html('There was an error. The database server might be down.');
                $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
            },
            complete: function (data) {


            },
            dataType: 'json'
        });

    }
    

    editDialog = $("#edit-slm-form").dialog({
        autoOpen: false,
        height: 600,
        width: 300,
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
        }
    });

    toReturn = function (id) {
        // set the rowBeingEdited global.
        SLMID = id;


        // do an ajax call
        $.ajax({
            type: "POST",
            url: '/HR/SuperLineManagers/getSLM',
            data: {
                id: SLMID
            },
            success: function (data) {


                if (data.success) {

                    function setMultiDropDown(multiDropDown, array) {

                        if (array == null) {
                            return;
                        }

                        multiDropDown.prop('disabled', true).trigger("chosen:updated");
                        multiDropDown.empty();
                        //<option value="0">none</option>

                        for (var i = 0; i < array.length; i++) {
                            multiDropDown.append("<option selected='selected' value='" + array[i].id + "'>" + array[i].text + "</option>");
                        }
                        

                        multiDropDown.prop('disabled', false).trigger("chosen:updated");

                    }

                    setMultiDropDown($('#modal-employees'), data.slm_employees);

                    


                    // open the dialog
                    editDialog.dialog("open");

                } else {


                    editDialog.dialog("close");
                    $('#toast2').html('Error in fetching data for this SLM.');
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





    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        updateItem();


    });


    return toReturn;
}