$(document).ready(function () {

    $('#accordion').accordion();
    
});

function initDataTable(identifier) {
    var table = $(identifier).DataTable({
        "scrollY": "200px"
    });

    table.on("draw", function () {
        var keyword = $(identifier + '_filter > label:eq(0) > input').val();

        $(identifier).unmark();
        $(identifier).mark(keyword, {});
    });
}

function followsPasswordPolicy(password) {

    ////debugger;

    // minimum length must be 6.
    var minLengthMet = password.length >= 6;

    // Must contain a capital letter.
    hasUpper = (/[A-Z]+/).test(password);

    // Must have a lower case letter.
    hasLower = (/[a-z]+/).test(password);

    // Must have a number
    hasNumber = (/[0-9]+/).test(password);

    // Must have a special character.
    hasSpecial = (/[!@@#$%^&*()_-]+/).test(password);


    return minLengthMet && hasUpper && hasLower && hasNumber && hasSpecial;
}


function setActiveLink(name) {
    $('#accordion a').filter(function (index) { return $(this).text() === name; }).css("color", "#008962");
}


function initServerSideTable(identifier,url,columns) {
    var oTable = $(identifier).DataTable({
        "searchDelay": 2000,
        "scrollY": "300px",
        "serverSide": true,
        "ajax": {
            "type": "POST",
            "url": url,
            "contentType": 'application/json; charset=utf-8',
            'data': function (data) { return data = JSON.stringify(data); }, error: function (request, error) {
               // alert("Session Timeout");
                location.reload();
            }
        },
        "scrollX": true,
        "scrollCollapse": true,
        "processing": true,
        "paging": true,
        "deferRender": true,
        "columns": columns,
        "order": [0, "asc"],
        "iDisplayLength": 500,
        "aLengthMenu": [[10, 25, 50, 100, 500, 1000, 1500], [10, 25, 50, 100, 500, 1000, 1500]]


    });

    

    oTable.on("draw", function () {
        var keyword = $(identifier + '_filter > label:eq(0) > input').val();
        $(identifier).unmark();
        $(identifier).mark(keyword, {});



        $(identifier + ' tbody tr').click(function (e) {
            $('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        });
    });





    // set the z-index of the 'processing...' overlay
    // to a great number, so that it may always display
    // on top of the table. Also set the text color
    // to black as it is white by default.
    $(identifier + '_processing').css({
        "z-index": "1000",
        "color": "rgb(0, 0, 0)"
    });


    // initiate search one second after the user
    // stops typing.
    function setSearchDelayOnDataTable(idSelector) {
        // remove all events from the search box
        $(idSelector + '_filter input').off();


        function customDataTableSearch() {

            // get the entered string
            var toSearch = $(idSelector + '_filter input').val();

            // do nothing if the string is null.
            // I've not tested this, it is just
            // in case.
            if (toSearch == null) {
                return;
            }

            // initiate search programatically.
            $(idSelector).DataTable().search(toSearch).draw();
        }

        // this variable holds the timer
        var timeout;

        // register a keypress on the search box
        $(idSelector + '_filter input').keypress(function () {
            if (timeout != null) {
                clearTimeout(timeout);
                timeout = null;
            }

            // timeout set to 1 second.
            timeout = setTimeout(customDataTableSearch, 1000);
        });

    }

    setSearchDelayOnDataTable(identifier);    
}





function initServerSideTable(identifier, url, columns, count) {
    var oTable = $(identifier).DataTable({
        "searchDelay": 2000,
        "scrollY": "500px",
        "serverSide": true,
        "ajax": {
            "type": "POST",
            "url": url,
            "contentType": 'application/json; charset=utf-8',
            'data': function (data) { return data = JSON.stringify(data); }, error: function (request, error) {
                // alert("Session Timeout");
                location.reload();
            }
        },
        "scrollX": true,
        "scrollCollapse": true,
        "processing": true,
        "paging": true,
        "deferRender": true,
        "columns": columns,
        "order": [0, "asc"],
        "iDisplayLength": count,
        "aLengthMenu": [[10, 25, 50, 100, 500, 1000, 1500], [10, 25, 50, 100, 500, 1000, 1500]]


    });



    oTable.on("draw", function () {
        var keyword = $(identifier + '_filter > label:eq(0) > input').val();
        $(identifier).unmark();
        $(identifier).mark(keyword, {});



        $(identifier + ' tbody tr').click(function (e) {
            $('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        });
    });





    // set the z-index of the 'processing...' overlay
    // to a great number, so that it may always display
    // on top of the table. Also set the text color
    // to black as it is white by default.
    $(identifier + '_processing').css({
        "z-index": "1000",
        "color": "rgb(0, 0, 0)"
    });


    // initiate search one second after the user
    // stops typing.
    function setSearchDelayOnDataTable(idSelector) {
        // remove all events from the search box
        $(idSelector + '_filter input').off();


        function customDataTableSearch() {

            // get the entered string
            var toSearch = $(idSelector + '_filter input').val();

            // do nothing if the string is null.
            // I've not tested this, it is just
            // in case.
            if (toSearch == null) {
                return;
            }

            // initiate search programatically.
            $(idSelector).DataTable().search(toSearch).draw();
        }

        // this variable holds the timer
        var timeout;

        // register a keypress on the search box
        $(idSelector + '_filter input').keypress(function () {
            if (timeout != null) {
                clearTimeout(timeout);
                timeout = null;
            }

            // timeout set to 1 second.
            timeout = setTimeout(customDataTableSearch, 1000);
        });

    }

    setSearchDelayOnDataTable(identifier);
}

function setupTableExportButton(tableId,fileName) {

  
        $(tableId).tableExport({ type: 'csv', escape: 'false', name: fileName });
    
    

}