function UpdateTime(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form2' class='jqUI-modal' title='Edit " + capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form  id='myform' method='POST'>\
            <fieldset>\
            <input type='hidden' name='id' id='id'>\
            <input type='hidden' name='empCode' id='empCode'>\
            <input type='hidden' name='date' id='date'>\
            <label class='control-label'>Time In<span class='text-danger font-14'>*</span></label>\
            <input type='text' name='timeIn' id='timeIn' class='form-control' />\
            <label class='control-label'>Time out<span class='text-danger font-14'>*</span></label>\
            <input type='text' name='timeOut' id='timeOut' class='form-control' />\
            <div class='row m-t-10'>\
            </div>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var u_empCode = $("#empCode");
    var u_id = $("#id");
    var u_date = $("#date");

    var u_timeIn = $("#timeIn");
    var u_timeOut = $("#timeOut");




    var allFields = $([]).add(u_id);

    
    function testTime(time) {
        var regex = /^([0-1][0-9])\:[0-5][0-9]\s*[ap]m$/i;
        var match = time.match(regex);
        if (match) {
            var hour = parseInt(match[1]);
            if (!isNaN(hour) && hour <= 11) {
                return true;
            }
        }
        return false;
    }

    function updateItem() {
        allFields.removeClass("ui-state-error");



        var time_In = testTime(u_timeIn.val());
        var time_Out = testTime(u_timeOut.val());

        //console.log(test1);
        //console.log(test2);

        var valid = true;
        if (time_In == true && time_Out == true) {

            if (valid) {
                // do an ajax call
                $.ajax({
                    type: "POST",
                    url: updateURL,
                    data: {
                        id: parseInt(u_id.val()),
                        employee_code: u_empCode.val(),
                        date: u_date.val(),
                        time_in: u_timeIn.val(),
                        time_out: u_timeOut.val()

                    },
                    success: function (data) {
                        editDialog.dialog("close");
                        $('#attendance_table').DataTable().draw();


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
        } else {
            alert('Invalid Time Formate.')
        }
        return valid;
    }

    editDialog = $("#dialog-form2").dialog({
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


    toReturn = function (_id, _empCode,_date ,_timeIn,_timeOut) {

        $('#id').val(_id);
        $("#empCode").val(_empCode);
        $("#date").val(_date);

        $("#timeIn").val(_timeIn);
        $("#timeOut").val(_timeOut);



        rowBeingEdited = _id;


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



