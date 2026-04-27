
var availableShifts =
    [
        /*{
            id:0,
            name: "Normal Shift",
            startTime: "9:00 am",
            endTime: "5:00 pm"
        }*/
    ];

var calendarData =
{
    year: "",
    general_shift: null,
    shift_days: [null, null, null, null, null, null, null],
    general_overrides: []
};

Helpers =
{
    applyPopOver: function (element, popOverTitle, popOverDescription) {
        var content = '<div class="event-tooltip-content">'
            + '<div class="event-name" style="color:#F0F">' + popOverTitle + '</div>'
            + '<div class="event-location">' + popOverDescription + '</div>'
            + '</div>';

        $(element).popover('destroy');
        $(element).popover({
            trigger: 'manual',
            container: 'body',
            html: true,
            content: content
        });

        $(element)
            .mouseenter(function () {
                $(element).popover('show');
            })
            .mouseleave(function () {
                $(element).popover('hide');
            })

    
    },
    removePopOver: function (element) {
        $(element).popover('destroy');
    },
    removeAllClasses: function(elementSelector) {

    $(elementSelector).each(function (i) {
        $(this).removeClass('holiday');
        $(this).removeClass('gazetted-holiday');
        $(this).removeClass('other-holiday');
        $(this).removeClass('custom-shift');
    });
    },
    dayApply: function(day, title, text) {
    $('.month > tbody > tr > td:nth-child(' + day + ') > .day-content').each(function (i) {
        Helpers.applyPopOver(this, title, text);
    });
    },
    makeDayHoliday: function(day) {
    $('.month > tbody > tr > td:nth-child(' + day + ') > .day-content').each(function (i) {
        $(this).addClass("holiday");
        Helpers.removePopOver(this);
    });
    },
    assignOverride: function (calendarOverrideObject) {

        date = calendarOverrideObject.date.split("/");
        month = date[0];
        day = date[1];

        monthContainer = $('.months-container > div:nth-child(' + month + ')');


        var dayElement;
        monthContainer.find('.day-content').each(function (e) {
            if (parseInt($(this).html()+"") == parseInt(day+"")) {
                dayElement = this;
            }
        });

        switch(calendarOverrideObject.type) {
            case "holiday":
                $(dayElement).addClass("other-holiday");
                Helpers.applyPopOver(dayElement, "Holiday", calendarOverrideObject.reason);

                break;
            case "gazetted":
                $(dayElement).addClass("gazetted-holiday");
                Helpers.applyPopOver(dayElement, "Gazetted Holiday", calendarOverrideObject.reason);
                break;

            case "shift":
                $(dayElement).addClass("custom-shift");


                shift = null;
                for (var i = 0; i < availableShifts.length; i++) {
                    if (availableShifts[i].id == parseInt(calendarOverrideObject.shift)) {
                        shift = availableShifts[i];
                        break;
                    }
                }

                Helpers.applyPopOver(dayElement, shift.name, shift.startTime+"-"+shift.endTime);

                break;
        }
        


    }
}


RenderingFunctions = {
    repaintCalendar: function () {

        function getShiftIndex(shiftId) {
            for (var i = 0; i < availableShifts.length; i++) {
                if (availableShifts[i].id == shiftId)
                    return i;
            }
        }

        // remove all styling from all day elements
        Helpers.removeAllClasses('.day-content');

        /**************************** 1) Render general shifts ************************************/
        if (calendarData.general_shift != null) {

            i = getShiftIndex(calendarData.general_shift);

            Helpers.dayApply(1, availableShifts[i].name,
                availableShifts[i].startTime + ' - ' + availableShifts[i].endTime);
            Helpers.dayApply(2, availableShifts[i].name,
                availableShifts[i].startTime + ' - ' + availableShifts[i].endTime);
            Helpers.dayApply(3, availableShifts[i].name,
                availableShifts[i].startTime + ' - ' + availableShifts[i].endTime);
            Helpers.dayApply(4, availableShifts[i].name,
                availableShifts[i].startTime + ' - ' + availableShifts[i].endTime);
            Helpers.dayApply(5, availableShifts[i].name,
                availableShifts[i].startTime + ' - ' + availableShifts[i].endTime);
            Helpers.dayApply(6, availableShifts[i].name,
                availableShifts[i].startTime + ' - ' + availableShifts[i].endTime);
            Helpers.dayApply(7, availableShifts[i].name,
                availableShifts[i].startTime + ' - ' + availableShifts[i].endTime);
        }
        /******************************************************************************************/
    
    
        /**************************** 1) Render days **********************************************/
        for (var i = 0; i < 7; i++) {
            if (calendarData.shift_days[i] != null) {
                if (calendarData.shift_days[i] == -1) {
                    Helpers.makeDayHoliday(i+1);
                } else {

                    j = getShiftIndex(calendarData.shift_days[i]);

                    Helpers.dayApply(i+1,
                        availableShifts[j].name,
                        availableShifts[j].startTime + ' ' + availableShifts[j].endTime);
                }
            }
        }
        /********************************************************************************/


        /********************************** Render Dates ******************************************/

        var generalOverrides = calendarData.general_overrides;
        // loop over all the overrides and assign colors
        // and pop overs. TODO
        for (var i = 0; i < generalOverrides.length; i++) {

            

            
            Helpers.assignOverride(generalOverrides[i]);

        }

        /******************************************************************************************/
    


    },

}


CalendarUtils = {
    submitGeneralCalendar: function (calendarData, calendarURL) {

        var toSend = JSON.stringify(calendarData);

        $.ajax({
            type: 'POST',
            url: calendarURL,
            data: "calendarData=" + toSend,
            success: function (data) {
                console.log(data);
                if (data.success == true) {
                    $('#toast2').html('Calendar Saved');
                    $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                } else {
                    $('#toast2').html('There was an error. the database server might be down.');
                    $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);
                }

            },
            error: function (event) {

            },
            complete: function (data) {
            },
            dataType: 'text json'
        });


    },

    // this function takes in the calendar year,
    // and the calendar url, to query the server,
    // for a calendar object of the year. It then 
    // converts it to a calendarData object and
    // returns it through a callback, Example:
    /*getGeneralCalendar('2014', 'http://localhost:57588/HR/ShiftManagement/getCalendarForYear', function (calendar) {
        console.log(calendar);
    });*/
    getGeneralCalendar: function (yearForCalendar, calendarURL, callback) {

        $.ajax({
            type: 'POST',
            url: calendarURL,
            data: {
                year: yearForCalendar,
            },
            success: function (data) {

                calendarObject = JSON.parse(data.data);

                // if the server returned null, this means
                // that the calendar for this year did not 
                // already exist, so use default calendar Data
                // object.
                if (calendarObject == null) {

                    callback({
                        year: yearForCalendar,
                        general_shift: null,
                        shift_days: [
                          null,
                          null,
                          null,
                          null,
                          null,
                          null,
                          null
                        ],
                        general_overrides: [
                        ]
                    });

                } else {


                    var obj =
                    {
                        year: yearForCalendar,
                        general_shift: (calendarObject.generalShift == null) ? null : calendarObject.generalShift.id,
                        shift_days: [
                          (calendarObject.Shift == null) ? null : calendarObject.Shift.id,
                          (calendarObject.Shift1 == null) ? null : calendarObject.Shift1.id,
                          (calendarObject.Shift2 == null) ? null : calendarObject.Shift2.id,
                          (calendarObject.Shift3 == null) ? null : calendarObject.Shift3.id,
                          (calendarObject.Shift4 == null) ? null : calendarObject.Shift4.id,
                          (calendarObject.Shift5 == null) ? null : calendarObject.Shift5.id,
                          (calendarObject.Shift6 == null) ? null : calendarObject.Shift6.id
                        ],
                        general_overrides: [
                        ]
                    };

                    for (var i = 0; i < calendarObject.generalOverrides.length; i++) {
                        obj.general_overrides[i] = calendarObject.generalOverrides[i];
                    }





                    callback(obj);
                }


            },
            error: function (event) {

            },
            complete: function (data) {
            },
            dataType: 'text json'
        });
    },



    // This function takes in the shifts url,
    // it queries the server for the available shifts
    // and returns the data through a callback, Example:
    /*
    getGeneralShifts("http://localhost:57588/HR/ShiftManagement/getShiftsForGeneralCalendar",function (shifts) {
      console.log(shifts);
    });
    */
    getGeneralShifts: function (shiftsURL, callback) {
        $.ajax({
            type: 'POST',
            url: shiftsURL,
            data: null,
            success: function (data) {
                callback(JSON.parse(data.data));


            },
            error: function (event) {

            },
            complete: function (data) {
            },
            dataType: 'text json'
        });
    }
}








function populateDropDowns() {
    // fetch shifts first
    CalendarUtils.getGeneralShifts("/HR/ShiftManagement/getShiftsForGeneralCalendar", function (shifts) {

        if (shifts.length == 0) {
            alert("You cannot add general calendars without adding shifts. Press ok to redirect to shift management.");


            window.location.replace("ManageShift");
        }

        // set global to empty
        availableShifts = [];

        // populate it
        for (var i = 0; i < shifts.length; i++) {
            var obj = {
                id: shifts[i].id,
                name: shifts[i].name,
                startTime: shifts[i].start_time,
                endTime: shifts[i].shift_end
            };
            availableShifts[i] = obj;
        }

        if(allShiftsLoaded != null)
            allShiftsLoaded();






        // There are two drop downs that
        // need population. the '#general-shift'
        // drop down and the '#day-shift' drop down.
        // As the names hint, the '#general-shift' dd
        // is used to asign an overall shift to the calendar
        // and the '#day-shift' drop down is used in combination
        // with another drop down - '#day' - to assign shifts
        // to a week day, for example you could mark
        // sunday as a holiday using '#day' and '#day-shift'



        // append an option 'none'
        // users can select this option if they want to not
        // select any shift for a day specially.
        $('#day-shift').append("<option value='null' title='none'>none</option>");

        // fetch the shifts for the calendar
        // fetchShifts()


        // loop through all the shifts and append them
        // as options in the shift drop downs.
        for (var i = 0; i < availableShifts.length; i++) {

            var HTML = "<option \
            value='" + availableShifts[i].id + "' \
            title='" + availableShifts[i].startTime + '-' + availableShifts[i].endTime + "'>" + availableShifts[i].name + "</option>";

            $('#general-shift').append(HTML);
            $('#day-shift').append(HTML);


        }

        // append an option for holiday,
        // note the value is '-1', it is important.
        $('#day-shift').append("<option value='-1' title='off' style='color:#ff0000'>Holiday</option>");



    });
}


function initializeTimeTuneGeneralCalendar() {

    // override the render function for the calendar,
    // I have basically copied the default code
    // for this function from bootstrap-year-calendar.js.
    // and at the end of it I have added a line so that
    // whenever the user changes the year, we always have 
    // it in our calendarData. I've also called repaint
    // so that popups are assigned to individual days
    // according to the shifts assigned.
    // UPDATE: I have also added code for fetching
    // the calendar for the new year, if the calendar
    // exists that is.
    $.fn.calendar().__proto__._render = function () {
        this.element.empty();
        this._renderHeader();
        this._renderBody();
        this._renderDataSource();
        this._applyEvents();
        this.element.find(".months-container").fadeIn(500);
        calendarData.year = this.options.startYear; // set the year in the calendarData
        this._triggerEvent("renderEnd", { currentYear: this.options.startYear });

        $('#toast').html('Please Wait!<br><br>Loading calendar for ' + calendarData.year + '....');
        $('#toast').fadeIn(400);
        // fetch the calendar for the current year.
        CalendarUtils.getGeneralCalendar(calendarData.year, '/HR/ShiftManagement/getGeneralCalendarForYear', function (calendar) {
            // set calendar data and render calendar
            calendarData = calendar;
            RenderingFunctions.repaintCalendar();

            $("#calendar_edit_form")[0].reset();

            if (calendarData.general_shift != null)
                $('#general-shift').val(calendarData.general_shift);

            if (calendarLoaded != null)
                calendarLoaded();



            // oh! and also open edit day modal on clicking a 
            // day element.
            $(".day-content").on("click", function (e) {
                //alert("I'm here");
                //console.log();
                openEditDayModal(this);
            });

            $('#toast').fadeOut(200);
        });
    }

    // global variable, this will represent the calendar instance
    // this instance is the calendar provided by the calendar library
    currentCalendar = $('#calendar').calendar();

}



function setDropDownBehaviour() {

    // When the value in the general-shift
    // drop down changes.
    $('#general-shift').change(function (a) {
        var value = $('#general-shift option:selected').val();

        calendarData.general_shift = parseInt(value);

        RenderingFunctions.repaintCalendar();
    });





    // select appropriate shifts in the day-shift drop down
    // against every day.
    $('#day').change(function (c) {
        weekDay = $('#day option:selected').val();
        if (calendarData.shift_days[weekDay] != null) {
            $('#day-shift').val(calendarData.shift_days[weekDay]);
        } else {
            $('#day-shift').val(-2);
        }

    });

    function applyShiftToDay(dayDropDownSelector, shiftDropDownSelector) {

        var dayValue = $(dayDropDownSelector + ' option:selected').val();
        var dayText = $(dayDropDownSelector + ' option:selected').text();

        var shiftValue = $(shiftDropDownSelector + ' option:selected').val();
        var shiftText = $(shiftDropDownSelector + ' option:selected').text();


        calendarData.shift_days[dayValue] = (shiftValue == "null") ? null : parseInt(shiftValue);

        RenderingFunctions.repaintCalendar();

        
    }

    // assign a shift to all the dates that fall on
    // a certain day.
    $('#day-shift').change(function (d) {

        applyShiftToDay('#day', '#day-shift');

    });
}
function initEditDayModal(parentDivIdentifier) {

    // concatenate all the shifts available into a 
    // string

    var allTheShifts = '';
    for (var i = 0; i < availableShifts.length; i++) {

        var str = "<option value='" + availableShifts[i].id + "' title='" + availableShifts[i].startTime + '-' + availableShifts[i].endTime + "'>" + availableShifts[i].name + "</option>";

        allTheShifts = allTheShifts + str;
    }


    var HTML =
        "<div id='edit-day-form' class='jqUI-modal' title='Edit Day'>\
            <div class='container-fluid'>\
                <div class='row'>\
                    <div class='col-md-12'>\
                        <h4 id='edit-day-date-title'></h4>\
                        <form>\
                            <fieldset>\
                            <label>Type:</label><br>\
                            <select id='edit-day-type'>\
                                <option value='-1' selected='selected' disabled='disabled'>Select Type</option>\
                                <option value='1'>None</option>\
                                <option value='2'>Holiday</option>\
                                <option value='3'>Gazetted Holiday</option>\
                                <option value='4'>Shift</option>\
                            </select><br>\
                            \
                            \
                            <span id='edit-day-holiday-span'>\
                            <label>Reason:</label><br>\
                            <textarea id='edit-day-reason' class='text ui-widget-content ui-corner-all'></textarea>\
                            </span>\
                            \
                            <span id='edit-day-shift-span'>\
                            <label>Shift:</label><br>\
                            <select id='edit-day-shift'>\
                                <option value='-1' selected='selected' disabled='disabled'>Select Shift</option>\
                                "+ allTheShifts + "\
                            </select>\
                            </span>\
                            \
                            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
                            </fieldset>\
                        </form>\
                    </div>\
                </div>\
            </div>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    //edit-day-form

    //edit-day-holiday-span
    //edit-day-shift-span

    //edit-day-type
    //edit-day-reason
    //edit-day-shift











    $('#edit-day-type').change(function (e) {
        value = parseInt($('#edit-day-type option:selected').val());

        switch (value) {
            case 1: // None
                editDayFormType = "none";
                $('#edit-day-holiday-span').hide();
                $('#edit-day-shift-span').hide();
                break;
            case 2: // Holiday
                editDayFormType = "holiday";
                $('#edit-day-holiday-span').show();
                $('#edit-day-shift-span').hide();
                break;
            case 3: // Gazetted
                editDayFormType = "gazetted";
                $('#edit-day-holiday-span').show();
                $('#edit-day-shift-span').hide();
                break;
            case 4: // Shift
                editDayFormType = "shift";
                $('#edit-day-holiday-span').hide();
                $('#edit-day-shift-span').show();
        }
    });



    function onClickDone() {


        // holiday

        var obj = {
            date: editDayFormDate,
            type: "none",
            reason: "",
            shift: -1
        };

        switch (editDayFormType) {
            case "holiday":
                obj = {
                    date: editDayFormDate,
                    type: "holiday",
                    reason: $('#edit-day-reason').val(),
                    shift: -1
                };
                break;
            case "gazetted":
                obj = {
                    date: editDayFormDate,
                    type: "gazetted",
                    reason: $('#edit-day-reason').val(),
                    shift: -1
                };
                break;
            case "shift":
                obj = {
                    date: editDayFormDate,
                    type: "shift",
                    reason: "",
                    shift: parseInt($('#edit-day-shift option:selected').val())
                };
                break;
        }

        // gazetted

        // shift




        // if the date already exists in the array do not add a new.
        // edit the existing object.
        for (var i = 0; i < calendarData.general_overrides.length; i++) {
            if (calendarData.general_overrides[i].date == editDayFormDate) {
                calendarData.general_overrides[i] = obj;
                RenderingFunctions.repaintCalendar();
                editDayForm.dialog("close");
                return;
            }
        }

        // else return, after adding a new override.
        calendarData.general_overrides[calendarData.general_overrides.length] = obj;
        RenderingFunctions.repaintCalendar();
        editDayForm.dialog("close");


    }

    editDayForm = $("#edit-day-form").dialog({
        autoOpen: false,
        height: 300,
        width: 300,
        modal: true,
        buttons: {
            "Done": function () {
                onClickDone();
            },
            Cancel: function () {
                editDayForm.dialog("close");
            }
        },
        close: function () {

        }
    });

    toReturn = function (dayElement) {
        var year = calendarData.year;
        var month = $(dayElement)
            .parent().parent().parent().parent().children()
            .find(".month-title").html();
        var day = $(dayElement).html();



        function convertMonthNameToNumber(name) {
            name = name.trim();
            switch (name) {
                case "January":
                    return "01";
                case "February":
                    return "02";
                case "March":
                    return "03";
                case "April":
                    return "04";
                case "May":
                    return "05";
                case "June":
                    return "06";
                case "July":
                    return "07";
                case "August":
                    return "08";
                case "September":
                    return "09";
                case "October":
                    return "10";
                case "November":
                    return "11";
                case "December":
                    return "12";
            }
        }

        function convertDayNumber(name) {
            name = name.trim();
            switch (name) {
                case "1":
                    return "01";
                case "2":
                    return "02";
                case "3":
                    return "03";
                case "4":
                    return "04";
                case "5":
                    return "05";
                case "6":
                    return "06";
                case "7":
                    return "07";
                case "8":
                    return "08";
                case "9":
                    return "09";
                default:
                    return name;
            }
        }

        function loadDataIfAlreadyOverriden(date) {

            var generalOverrides = calendarData.general_overrides;

            // check if the selected date already has a general override.
            var selectedOverride = null;
            for (var i = 0; i < generalOverrides.length; i++) {

                // if the date matches break from loop
                if (generalOverrides[i].date == date) {
                    selectedOverride = generalOverrides[i];
                    break;
                }
            }


            // if it has one, load the form with the values.
            if (selectedOverride != null) {

                if (selectedOverride.type == "holiday") {

                    editDayFormType = "holiday";
                    $('#edit-day-type').val(2);
                    $('#edit-day-reason').val(selectedOverride.reason);
                    $('#edit-day-shift').val(-1);

                    $('#edit-day-holiday-span').show();
                    $('#edit-day-shift-span').hide();


                } else if (selectedOverride.type == "gazetted") {

                    editDayFormType = "gazetted";
                    $('#edit-day-type').val(3);
                    $('#edit-day-reason').val(selectedOverride.reason);
                    $('#edit-day-shift').val(-1);

                    $('#edit-day-holiday-span').show();
                    $('#edit-day-shift-span').hide();

                } else if (selectedOverride.type == "shift") {
                    

                    editDayFormType = "shift";

                    $('#edit-day-type').val(4);
                    $('#edit-day-reason').val("");
                    $('#edit-day-shift').val(selectedOverride.shift);


                    $('#edit-day-holiday-span').hide();
                    $('#edit-day-shift-span').show();


                }
                
            }


        }


        // global variables used by dialog
        editDayFormType = "none";
        editDayFormDate = convertMonthNameToNumber(month) + "/" + convertDayNumber(day) + "/" + year;
        editDayFormElement = dayElement;

        $("#edit-day-date-title").html(editDayFormDate);


        // reset form elements
        $('#edit-day-type').val(-1);
        $('#edit-day-reason').val("");
        $('#edit-day-shift').val(-1);
        $('#edit-day-holiday-span').hide();
        $('#edit-day-shift-span').hide();



        //////////////////////////////////////////
        loadDataIfAlreadyOverriden(editDayFormDate);
        //////////////////////////////////////////


        editDayForm.dialog("open");
    };



    
    // When the enter key is pressed
    // prevent the form from being submitted.
    editDayForm.find("form").on("submit", function (id, name, description) {
        event.preventDefault();
        onClickDone();
    });


    return toReturn;
}
// These are the actions that happen when the shifts are loaded, 
// its like a call back in populateDropDowns(); which does
// the actual shifts loading.
function allShiftsLoaded() {
    // make rainbows.
    setDropDownBehaviour();


    
    // define global function, which takes place
    // when a day is clicked.
    openEditDayModal = initEditDayModal('#modals');
}


// this is a call back which executes 
// when the calendar is loaded, its a callback
// in initializeTimeTuneGeneralCalendar();
// which does the calendar loading
function setSubmitButtonAction() {
    
    $('#submit_calendar_button').on("click", function (e) {

        CalendarUtils.submitGeneralCalendar(calendarData, "/HR/ShiftManagement/addOrUpdateCalendar");

        e.preventDefault();
    });

}

function calendarLoaded() {
    // define actions to take when submit calendar is pressed.
    
}



$(document).ready(function () {

    // fill the drop downs with the different shifts.
    populateDropDowns();

    // get the calendar ready for display.
    initializeTimeTuneGeneralCalendar();

    setSubmitButtonAction();

    

});


