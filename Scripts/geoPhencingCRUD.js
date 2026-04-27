

function initDeleteGeoPhencingModal(parentDivIdentifier, deleteURL, capital) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital + "'>\
       <p>Are you sure you want to delete the record with ID: <span id='delete-dialog-field-id'></span>?</p>\
        <form>\
            <fieldset>\
           <!-- <label>"+ capital + " Name: <span id='delete-dialog-field-name'></span></label>\
            <label>"+ capital + " Description: <span id='delete-dialog-field-description'></span></label> -->\
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

                console.log(data);
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
        rowToRemove = id;

        //// get the name and the description of the item that we are removing.
        //var name = $('*[data-row="' + rowToRemove + '"] > td:eq(0)').html();
        //var description = $('*[data-row="' + rowToRemove + '"] > td:eq(1)').html();

        //$('#delete-dialog-field-name').html(name);
        //$('#delete-dialog-field-description').html(description);
        $('#delete-dialog-field-id').html(rowToRemove);

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



function generateULOptions(options) {

    options = options.slice(1,-1);
    //alert(options);

    var default_string = "";
    var managed_string = "";
    var strComma = "", strColon = "";

    //debugger;

    //options = "1:Building-001,5:DMC Building 002";

    if (options.length > 0) {

        if (options.indexOf('^') > 0) {
            strComma = options.split('^');
        } else {
            options = options + "^";
            strComma = options.split('^');
        }

        for (var i = 0; i < strComma.length; i++) {

            strColon = strComma[i].split('|');
            //console.log("<option value='" + strColon[0] + "'>" + strColon[1] + "</option>");
            managed_string += "<li>" + strColon[0] + " : "  + strColon[1] + "</li>";
        }
    }

    if (options.length == 0) {
        return default_string;
    }

    return managed_string;
}

function initEditGeoPhencingModal(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form2' class='jqUI-modal' title='Edit " + capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='id1' id='id1'>\
            <input type='hidden' name='employeeid1' id='employeeid1'>\
            <label class='control-label m-t-10'>Employee Code</label>\
            <input type='text' class='form-control' name='emp_code1' id='emp_code1' readOnly />\
            <label class='control-label m-t-10'>Employee Name</label>\
            <input type='text' class='form-control' name='emp_name1' id='emp_name1' readOnly />\
            <label class='control-label m-t-10'>Branches List</label>\
            <ul id='ul_branches1'></ul>\
            <input type='text' class='form-control' name='lst_branches1' id='modal-branches1' />\
            <label class='control-label m-t-10'>Terminals List</label>\
            <ul id='ul_terminals1'></ul>\
            <input type='text' class='form-control d-none' name='lst_terminals1' id='lst_terminals1' readOnly />\
            <label class='control-label m-t-10'>Created Date</label>\
            <input type='text' class='form-control' name='crt_date1' id='crt_date1' readOnly />\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    //$(parentDivIdentifier).append(HTML);

                //+  + <input type='text' class='form-control' name='lst_branches' id='lst_branches' readOnly    />\

    var id = $('#id');
    var employeeid = $('#employeeid');
    //var emp_code = $("#emp_code");
    //var emp_name = $("#emp_name");
    var lst_branches = $("#modal-branches");
    //var lst_terminals = $("#modal-terminals");


   
    var allFields = $([]).add(employeeid).add(lst_branches);//.add(lst_terminals);


    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        //valid = valid && checkLength(fieldName, capital + " name", 2, 50);
        // valid = valid && checkLength(fieldDescription, "description", 0, 100);
        //var check2 = $("#isforacademiccalendar2").is(":checked");
        if (valid) {

            //alert(lst_branches.val());

            var listData = {
                id: parseInt(id.val()),
                EmployeeId: parseInt(employeeid.val()),
                lstBranches: lst_branches.val() + ""
            };

            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                cache: false,
                traditional: true,
                //data: JSON.stringify(listData),
                data: {
                    id: parseInt(id.val()),
                    EmployeeId: parseInt(employeeid.val()),
                    lstBranches: lst_branches.val()
                    //,TerminalsList: lst_terminals.val()
                },
                success: function (data) {
                    if (data.status == "already") {
                        editDialog.dialog("close");
                        $('#toast2').html('Record already exists.');
                        $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                    } else {
                        //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(start_date.val());
                        //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(end_date.val());

                        editDialog.dialog("close");
                        $('#toast2').html('Updated successfully.');
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

        }
        return valid;
    }

    editDialog = $("#geo-phenced-form").dialog({
        autoOpen: false,
        height: 500,
        width: 660,
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


    toReturn = function (id, employeeid, emp_code, emp_name, lst_branches, lst_terminals, crt_date) {

        $('#id').val(id);
        $('#employeeid').val(employeeid);
        $('#emp_code').val(emp_code);
        $('#emp_name').val(emp_name);

        var ul_branches = generateULOptions(lst_branches)
        $('#ul_branches').html(ul_branches);
        $('#modal-branches').val(lst_branches);

        var ul_terminals = generateULOptions(lst_terminals)
        $('#ul_terminals').html(ul_terminals);
        //$('#lst_terminals').val(lst_terminals);

        $('#crt_date').val(crt_date);

        rowBeingEdited = id;
        editDialog.dialog("open");

        $('#modal-branches').focus();
        $('#emp_name').focus();
        $('#modal-branches').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        updateItem();


    });


    return toReturn;
}
