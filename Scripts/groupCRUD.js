





function initDeleteGroupModal(deleteURL) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete Group'>\
        <p>Delete the following group?</p>\
        <form>\
            <fieldset>\
            <label>Line Manager ID: <span id='delete-dialog-field-name'></span></label>\
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
                $('#toast2').html('Group deleted successfully');
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

function initEditGroupModal(updateURL) {


    


    function updateItem() {

        var groupEmployees = new Array();
        $('#modal-group-employees option:selected').each(function () {
            groupEmployees[groupEmployees.length] = $(this).val()+"";
        });


        var groupShifts = new Array();
        $('#modal-group-shifts option:selected').each(function () {
            groupShifts[groupShifts.length] = $(this).val()+"";
        });



        // do an ajax call
        $.ajax({
            contentType: 'application/json',
            type: "POST",
            url: updateURL,
            data: JSON.stringify({
                group_id:groupID,
                group_name: 'grp', // un used
                follows_general_calendar: ($('#modal-group-follows-gc').is(':checked') == true)?"true":null, // null if unckecked.
                line_manager: $('#modal-line-manager option:selected').val()+'',
                group_employees: groupEmployees,
                group_shifts: groupShifts
            }),
            success: function (data) {
                if (data.error == "") {
                    // set line_manager code
                    $('*[data-id="' + groupID + '"]')
                        .parent().parent().children().filter(':nth-child(2)')
                        .html(data.line_manager_code);

                    // set line_manager name
                    $('*[data-id="' + groupID + '"]')
                        .parent().parent().children().filter(':nth-child(3)')
                        .html(data.line_manager_name);

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
    

    editDialog = $("#group-edit-form").dialog({
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
        groupID = id;


        // do an ajax call
        $.ajax({
            type: "POST",
            url: '/HR/RosterManagement/getGroupData',
            data: {
                id: groupID
            },
            success: function (data) {


                if (data.success) {

                    // I made functions for conciseness.
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

                    console.log(data);
                    //console.log(data.group_employees);
                    //console.log(data.group_shifts);
                    if(data.line_manager_id != 0)
                        setDropDownVal($('#modal-line-manager'), data.line_manager_id, data.line_manager_code);
                    else {

                        $('#modal-line-manager').prop('disabled', true).trigger("chosen:updated");
                        $('#modal-line-manager').empty();
                        $('#modal-line-manager').append("<option value='0'>none</option>");
                        $('#modal-line-manager').val(0);
                        $('#modal-line-manager').prop('disabled', false).trigger("chosen:updated");

                    }
                    setMultiDropDown($('#modal-group-employees'), data.group_employees);
                    setMultiDropDown($('#modal-group-shifts'), data.group_shifts);

                    if (data.follows_general_calendar) {
                        $('#modal-group-follows-gc').attr('checked', true);
                    } else {
                        $('#modal-group-follows-gc').attr('checked', false);
                    }
                    
                    


                    // open the dialog
                    editDialog.dialog("open");

                } else {


                    editDialog.dialog("close");
                    $('#toast2').html('Error in fetching data for this group.');
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