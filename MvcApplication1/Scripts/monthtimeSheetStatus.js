


function initEditCourse(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form2' class='jqUI-modal' title='Edit " + capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form  id='myform' method='POST'>\
            <fieldset>\
             <input type='hidden' name='id' id='id'>\
            <label class='control-label'>Status<span class='text-danger font-14'>*</span></label>\
            <select id='overtime_status' class='form-control' name='overtime_status' required>\
            <option value='1'>Unapproved</option>\
            <option value='2'>Approved</option>\
            <option value='3'>Discard</option>\
            </select>\
            <div class='row m-t-10'>\
            </div>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var statusId = $("#overtime_status");
    var id = $("#id");



    var allFields = $([]).add(statusId);

    


    function updateItem() {
        allFields.removeClass("ui-state-error");
    

        
        var valid = true;

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    id: parseInt(id.val()),
                    overtime_status: statusId.val()
                    
                },
                success: function (data) {


                  
                    $('*[data-id="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html($("#overtime_status :selected").text());
                  
                    
                    editDialog.dialog("close");
                   // window.location.reload();

                
                    $('#toast').html('Record updated successfully');
                    $('#toast').stop().fadeIn(400).delay(3000).fadeOut(400);

                },
                error: function (event) {

                    editDialog.dialog("close");
                    $('#modals').html('There was an error. The database server might be down.');
                    $('#modals').stop().fadeIn(400).delay(3000).fadeOut(400);
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
        height: 300,
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


    toReturn = function (id, cc_status) {

        $('#id').val(id);
        $("#overtime_status").val(cc_status);



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



