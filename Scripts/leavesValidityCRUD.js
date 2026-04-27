
function updateValidationTips(tipText) {
    $(".validationTips").text(tipText);

}

function checkLength(field, fieldName, minLength, maxLength) {
    if (field.val().length > maxLength || field.val().length < minLength) {
        field.addClass("ui-state-error");
        updateValidationTips("Length of " + fieldName + " must be between " +
            minLength + " and " + maxLength + ".");
        return false;
    } else {
        return true;
    }
}

function checkRegexp(field, regexp, onInvalidTip) {
    if (!(regexp.test(field.val()))) {
        field.addClass("ui-state-error");
        updateValidationTips(onInvalidTip);
        return false;
    } else {
        return true;
    }
}

//////////////////////////////////////// EDIT SECTION ////////////////////////////////////////////////


function generateApprovalOptions(options) {
    var default_string = "<option value='1'>Pending</option><option value='2'>Approved</option><option value='3'>Rejected</option>";
    var managed_string = "";
    var strComma = "", strColon = "";

    //options = "1:Pending,2:Approved,3:Rejected";

    if (options.length > 0) {

        strComma = options.split(',');

        for (var i = 0; i < strComma.length; i++) {

            strColon = strComma[i].split(':');
            //console.log("<option value='" + strColon[0] + "'>" + strColon[1] + "</option>");
            managed_string += "<option value='" + strColon[0] + "'>" + strColon[1] + "</option>";
        }
    }

    if (options.length == 0) {
        return default_string;
    }

    return managed_string;
}

function initEditModal(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form' class='jqUI-modal' title='Edit "+ capital.replace("_", " ") + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <label class='control-label'>From Date</label>\
            <input type='hidden' name='Id' id='modal_"+ capital + "_id'>\
            <input type='hidden' name='EmployeeId' id='modal_" + capital + "_employee_id'>\
            <input type='hidden' name='AccessId' id='modal_" + capital + "_access_id'>\
            <input type='hidden' name='GradeId' id='modal_" + capital + "_grade_id'>\
            <input type='hidden' name='SiteId' id='modal_" + capital + "_site_id'>\
            <input type='hidden' name='LeaveTypeId' id='modal_" + capital + "_leave_type_id'>\
            <input type='hidden' name='HODSt' id='modal_" + capital + "_hod_st'>\
            <input type='hidden' name='PRNSt' id='modal_" + capital + "_prn_st'>\
            <input type='hidden' name='HRSt' id='modal_" + capital + "_hr_st'>\
            <input type='hidden' name='VCSt' id='modal_" + capital + "_vc_st'>\
            <input type='text' class='form-control datepicker' name='FromDate' id='modal_" + capital + "_from_date' readonly />\
            <label class='control-label'>To Date</label>\
            <input type='text' name='ToDate' id='modal_" + capital + "_to_date' class='form-control datepicker' readonly />\
            <label class='control-label'>Days Count</label>\
            <input type='text' name='DaysCount' id='modal_" + capital + "_days_count' class='form-control' readonly />\
            <label class='control-label'>Validate Status</label>\
            <select id='modal_" + capital + "_val_id' name='ValId' class='form-control'>" + generateApprovalOptions('1:Pending,2:Approved') + "</select>\
            <label type='hidden' class='control-label'></label>\
            <input type='hidden'  name='ValRemarks' id='modal_" + capital + "_val_rem' class='form-control' />\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var fieldID = $("#modal_" + capital + "_id");
    var fieldEmployeeID = $("#modal_" + capital + "_employee_id");
    var fieldLeaveTypeID = $("#modal_" + capital + "_leave_type_id");
    var fieldGradeID = $("#modal_" + capital + "_grade_id");
    //var fieldFromDate = $("#modal_" + capital + "_from_date");
    //var fieldToDate = $("#modal_" + capital + "_to_date");
    //var fieldDaysCount = $("#modal_" + capital + "_days_count");
    var fieldValID = $("#modal_" + capital + "_val_id");
    var fieldValRem = $("#modal_" + capital + "_val_rem");
    //var fieldIsActive = $("#modal_" + capital + "_is_active");

    //console.log("fieldStatus = " + fieldStatus.val());
    var allFields = $([]).add(fieldID).add(fieldEmployeeID).add(fieldLeaveTypeID).add(fieldGradeID).add(fieldValID).add(fieldValRem);//.add(fieldIsActive);

    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        //valid = valid && checkLength(fieldFromDate, capital + " name", 10, 20);
        //valid = valid && checkLength(fieldToDate, "description", 10, 20);

        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    Id: parseInt(fieldID.val()),
                    EmployeeId: parseInt(fieldEmployeeID.val()),
                    LeaveTypeId: parseInt(fieldLeaveTypeID.val()),
                    LeaveReasonId: parseInt(fieldGradeID.val()),
                    LeaveStatusId: fieldValID.val(),
                    //FromDate: fieldFromDate.val(),
                    //ToDate: fieldToDate.val(),
                    //DaysCount: fieldDaysCount.val(),
                    LeaveValidityId: fieldValID.val(),
                    LeaveValidityRemarks: fieldValRem.val(),
                    //IsActive: fieldIsActive.is(":checked")
                },
                success: function (data) {

                    ////alert('done');
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("*");
                    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(8)').html(fieldValRem.val());

                    //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(fieldDaysCount.val());

                    //if (fieldStatus.val() == "1")
                    //    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("Pending");
                    //else if (fieldStatus.val() == "2")
                    //    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("Approved");
                    //else
                    //    $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(7)').html("Rejected");

                    editDialog.dialog("close");
                    $('#toast2').html('Record updated successfully');
                    $('#toast2').stop().fadeIn(400).delay(3000).fadeOut(400);

                    console.log(data);
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

    editDialog = $("#dialog-form").dialog({
        autoOpen: false,
        height: 450,
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

    ////toReturn = function (id, emp_id, from_date, to_date, days_count, val_id, val_rem) {
    toReturn = function (id, emp_id, access_id,usite_id, grd_id, site_id, ltype_id, hod_st, prn_st, hr_st, vc_st, from_date, to_date, days_count, val_id, val_rem, status) {

        //////// [START] - IR added Conditions /////////////////////////

        // //alert("site_id=" + site_id + " and ltype_id=" + ltype_id);

        //if (access_id == "1") {
        //    site_id = "2";//HR site id set to 2 temp
        //}
        //else
        if (usite_id == 4) {
            if (access_id == "2") {
                site_id = "4";//LM site id set to 4 temp
            }
            if (site_id == 1) {
                if (val_rem == 1) {
                    alert("Super - HR Cannot Approved this leave");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
                else {
                    alert('The Request for this Leave is Approved');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                }

            }
            //alert("site_id=" + site_id + " and ltype_id=" + ltype_id);

            if (ltype_id == 1) {//Medical

                if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                    alert("The Request for this Leave Is Approved");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
                 if (site_id == 5) {//Principal

                    //alert('4-555');
                     //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                     if (prn_st == 0 || prn_st==1) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    }
                    //else if (prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    }
                }
                else if (site_id == 2) {//HR=Admin
                    //alert('4-222');

                     //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                     if((hr_st == 1 || hr_st==0) && prn_st == 2 && hod_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                     }
                     else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st==0) && hod_st == 2) {
                        alert('Approval By Principle required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD and Principle Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by HR');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
            }
            else if (ltype_id == 2) {// Casual
                //do nothing
                if (site_id == 5) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principle");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
                else {
                    alert('Casual Leave For HOD Approved Only By Principle');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
            }
            else if (ltype_id == 3) {//Earned

                //$("#modal_Leave_Application_val_id option[value='2']").val("-5");

                if (grd_id < 20) {
                    if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2) {
                        alert("The Request for this Leave Is Approved");
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    //if (site_id == 4) {//HOD=LM

                    //    //alert('4-444');

                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    //    if (hod_st == 1) {

                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    //    }
                    //    else {
                    //        alert('Already Approved by HOD');
                    //        //added by M
                    //        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    //    }
                    //}
                     if (site_id == 5) {//Principal

                        //alert('4-555');
                         //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                         if ((prn_st == 1 || prn_st==0) && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                        }
                        //else if (prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by Principle');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                        }
                    }
                    else if (site_id == 2) {//HR=Admin
                        //alert('4-222');

                         //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                         if ((hr_st == 1 || hr_st==0) && prn_st == 2 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                            $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                         }
                         else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st==0) && hod_st == 2) {
                            alert('Approval By Principle required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD and Principle Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by HR');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }

                    //if (site_id == 4) {//HOD=LM

                    //    //alert('3-444');
                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    //    if (hod_st == 1) {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    //    }
                    //    else {
                    //        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                    //        //$("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step01 Approval
                    //    }
                    //}
                    //else if (site_id == 5) {//Principal   

                    //    //alert('3-555');

                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                    //    if (prn_st == 1 && hod_st == 2) {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    //    } else {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    //    }

                    //}
                    //else if (site_id == 2) {//HR=Admin

                    //    //alert('3-222');

                    //    if (hr_st == 1 && prn_st == 2 && hod_st == 2) {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    //    } else {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Principal Approval is req.
                    //    }
                    //}
                }
                else {//greater than and = to Grade 20

                    if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                        alert("The Request for this Leave Is Approved");
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }

                    //if (site_id == 4) {//HOD=LM

                    //    //alert('4-444');

                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    //    if (hod_st == 1) {

                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    //    }
                    //    else {
                    //        alert('Already Approved by HOD');
                    //        //added by M
                    //        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    //    }
                    //}
                     if (site_id == 5) {//Principal

                        //alert('4-555');
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                        if (prn_st == 1 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                        }
                        //else if (prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by Principle');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                        }
                    }
                    else if (site_id == 2) {//HR=Admin
                        //alert('4-222');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                        if (hr_st == 1 && prn_st == 2 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-3");//FINAL Approval
                        }
                        else if (hr_st == 1 && prn_st == 1 && hod_st == 2) {
                            alert('Approval By Principle required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD and Principle Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by HR');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }
                    else if (site_id == 6) {//6 VC
                        //alert('4-666');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        if (vc_st == 1 && hr_st == 2 && prn_st == 2 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                            $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        }
                        else if (vc_st == 1 && hr_st == 1 && prn_st == 2 && hod_st == 2) {
                            alert('Approval By HR Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else if (vc_st == 1 && hr_st == 1 && prn_st == 1 && hod_st == 1) {
                            alert('Approval By Principle and HR Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else {
                            alert('Already Approved By VC');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }

                }
            }
            else if (ltype_id == 4) {//exPak for ALL
                if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                    alert("The Request for this Leave Is Approved");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }

                //if (site_id == 4) {//HOD=LM

                //    //alert('4-444');

                //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                //    if (hod_st == 1) {

                //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                //    }
                //    else {
                //        alert('Already Approved by HOD');
                //        //added by M
                //        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                //        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                //        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                //    }
                //}
                 if (site_id == 5) {//Principal

                    //alert('4-555');
                    //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                    if (prn_st ==1||prn_st==0) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    }
                    //else if (prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    }
                }
                else if (site_id == 2) {//HR=Admin
                    //alert('4-222');

                     //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                     if ((hr_st == 1 || hr_st==0) && prn_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-3");//FINAL Approval
                     }
                     else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st==0)) {
                        alert('Approval By Principle required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD and Principle Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by HR');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
                else if (site_id == 6) {//6 VC
                    //alert('4-666');

                     //$("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                     if ((vc_st == 1 || vc_st==0) && hr_st == 2 && prn_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                     }
                     else if ((vc_st == 1 || vc_st == 0) && (hr_st == 1 || hr_st==0) && prn_st == 2 && hod_st == 2) {
                        alert('Approval By HR Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                     }
                     else if ((vc_st == 1 || vc_st == 0) && (hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st==0) && hod_st == 2) {
                        alert('Approval By  and Principle and HR Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else {
                        alert('Already Approved By VC');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
            }
            else if (ltype_id >= 5 && ltype_id <= 15) {// Extra Type of leaves
                //do nothing
                if (site_id == 5) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principle");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
            }
            else if (ltype_id >= 16) {// Maunal Attendance Options to Apply
                //do nothing
                ////alert('manual attendance');
                if (site_id == 5) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principle");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
                else {
                    alert('Allocation Leave For HOD Approved Only By Principle');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
            }
        }
        else if (usite_id == 5)
        {
            if (site_id == 1) {
                if (val_rem == 1) {
                    alert("Super - HR Cannot Approved this leave");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
                else {
                    alert('The Request for this Leave is Approved');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                }

            }
            if (ltype_id == 1) {//Medical


                if (site_id == 2) {//Principal

                    //alert('4-555');
                    //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                    if (hr_st == 1) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    }
                    //else if (prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by HR');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    }
                }
                else if (site_id == 6) {//HR=Admin
                    //alert('4-222');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                    if (hr_st == 2 && vc_st==1) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    }
                    else if (hr_st == 1) {
                        alert('Approval By HR required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD and Principle Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by VC');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
            }
            else if (ltype_id == 2) {// Casual
                //do nothing
                if (site_id == 2) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by HR');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
                else {
                    alert('Casual Leave For PRN Approved Only By HR');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
            }
            else if (ltype_id == 3) {//Earned

                //$("#modal_Leave_Application_val_id option[value='2']").val("-5");

                if (grd_id < 20) {
                    //if (site_id == 4) {//HOD=LM

                    //    //alert('4-444');

                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    //    if (hod_st == 1) {

                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    //    }
                    //    else {
                    //        alert('Already Approved by HOD');
                    //        //added by M
                    //        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    //    }
                    //}
                    if (site_id == 5) {//Principal

                        //alert('4-555');
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                        if (prn_st == 1 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                        }
                        //else if (prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by Principle');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                        }
                    }
                    else if (site_id == 2) {//HR=Admin
                        //alert('4-222');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                        if (hr_st == 1 && prn_st == 2 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-5");//FINAL Approval
                        }
                      
                        else {
                            alert('Already Approved by HR');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }
                    else if (site_id == 6) {//HR=Admin
                        //alert('4-222');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                        if (hr_st == 2 && vc_st == 1) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                            $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        }
                        else if (hr_st == 1) {
                            alert('Approval By HR required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD and Principle Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by HR');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }

                   
                }
                else {//greater than and = to Grade 20

                    if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                        alert("The Request for this Leave Is Approved");
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }

                    //if (site_id == 4) {//HOD=LM

                    //    //alert('4-444');

                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    //    if (hod_st == 1) {

                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    //    }
                    //    else {
                    //        alert('Already Approved by HOD');
                    //        //added by M
                    //        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    //    }
                    //}
                    if (site_id == 5) {//Principal

                        //alert('4-555');
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                        if (prn_st == 1 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                        }
                        //else if (prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by Principle');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                        }
                    }
                    else if (site_id == 2) {//HR=Admin
                        //alert('4-222');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                        if (hr_st == 1) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-3");//FINAL Approval
                        }
                       
                        //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                        //    alert('Approval By HOD and Principle Required');
                        //    //added by M
                        //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //}
                        else {
                            alert('Already Approved by HR');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }
                    else if (site_id == 6) {//6 VC
                        //alert('4-666');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        if (vc_st == 1 && hr_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                            $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        }
                        else if (vc_st == 1 && hr_st == 1 ) {
                            alert('Approval By HR Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                      
                        else {
                            alert('Already Approved By VC');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }

                }
            }
            else if (ltype_id == 4) {//exPak for ALL
                if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                    alert("The Request for this Leave Is Approved");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }

                //if (site_id == 4) {//HOD=LM

                //    //alert('4-444');

                //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                //    if (hod_st == 1) {

                //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                //    }
                //    else {
                //        alert('Already Approved by HOD');
                //        //added by M
                //        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                //        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                //        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                //    }
                //}
                if (site_id == 5) {//Principal

                    //alert('4-555');
                    //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                    if (prn_st == 1) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    }
                    //else if (prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    }
                }
                else if (site_id == 2) {//HR=Admin
                    //alert('4-222');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                    if (hr_st == 1) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-3");//FINAL Approval
                    }
                    
                    //else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                    //    alert('Approval By HOD and Principle Required');
                    //    //added by M
                    //    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    //    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    //}
                    else {
                        alert('Already Approved by HR');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
                else if (site_id == 6) {//6 VC
                    //alert('4-666');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    if (vc_st == 1 && hr_st == 2 ) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    }
                    else if (vc_st == 1 && hr_st == 1 ) {
                        alert('Approval By HR Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                  
                    else {
                        alert('Already Approved By VC');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
            }
            else if (ltype_id >= 5 && ltype_id <= 15) {// Extra Type of leaves
                //do nothing
                if (site_id == 5) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principle");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
            }
            else if (ltype_id >= 16) {// Maunal Attendance Options to Apply
                //do nothing
                ////alert('manual attendance');

            }
        }
        else if (usite_id == 2 || usite_id == 6)
        {
            if (site_id == 1)
            {
                if (val_rem == 1) {

                    $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR-Super");
                    $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                }
                else {
                    alert('The Request for this Leave is Approved');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                }

            }
        }
        else {
            if (access_id == "2") {
                site_id = "4";//LM site id set to 4 temp
            }

            //alert("site_id=" + site_id + " and ltype_id=" + ltype_id);
            if (site_id == 1) {
                if (val_rem == 1) {
                    alert("Super - HR Cannot Approved this leave");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
                else {
                    alert('The Request for this Leave is Approved');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                }

            }
            if (ltype_id == 1) {//Medical
                if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                    alert("The Request for this Leave Is Approved");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }

                if (site_id == 4) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (hod_st == 1 || hod_st==0) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by HOD');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
                else if (site_id == 5) {//Principal

                    //alert('4-555');
                    //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                    if ((prn_st == 1 || prn_st==0) && hod_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    }
                    else if ((prn_st == 1 || prn_st == 0) && (hod_st == 1 || hod_st==0)) {
                        alert('Approval By HOD Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    }
                }
                else if (site_id == 2) {//HR=Admin
                    //alert('4-222');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                    if ((hr_st == 1 || hr_st==0) && prn_st == 2 && hod_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    }
                    else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st==0) && hod_st == 2) {
                        alert('Approval By Principle required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st == 0) && (hod_st == 1 || hod_st==0)) {
                        alert('Approval By HOD and Principle Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else {
                        alert('Already Approved by HR');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
            }
            else if (ltype_id == 2) {// Casual
                //do nothing
                if (site_id == 4) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by HOD');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
                else {
                    alert('Casual Leave Approved Only By HOD');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }
            }
            else if (ltype_id == 3) {//Earned

                //$("#modal_Leave_Application_val_id option[value='2']").val("-5");

                if (grd_id < 20) {
                    if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                        alert("The Request for this Leave Is Approved");
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    if (site_id == 4) {//HOD=LM

                        //alert('4-444');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                        if (hod_st == 1 || hod_st == 0) {

                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                        }
                        else {
                            alert('Already Approved by HOD');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                        }
                    }
                    else if (site_id == 5) {//Principal

                        //alert('4-555');
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                        if ((prn_st == 1 || prn_st == 0) && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                        }
                        else if ((prn_st == 1 || prn_st == 0) && (hod_st == 1 || hod_st == 0)) {
                            alert('Approval By HOD Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else {
                            alert('Already Approved by Principle');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                        }
                    }
                    else if (site_id == 2) {//HR=Admin
                        //alert('4-222');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                        if ((hr_st == 1 || hr_st == 0) && prn_st == 2 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                            $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        }
                        else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st == 0) && hod_st == 2) {
                            alert('Approval By Principle required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st == 0) && (hod_st == 1 || hod_st == 0)) {
                            alert('Approval By HOD and Principle Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else {
                            alert('Already Approved by HR');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }
                    else if (site_id == 6) {
                        alert('This leave can Approved By HR ');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");}
                    //if (site_id == 4) {//HOD=LM

                    //    //alert('3-444');
                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    //    if (hod_st == 1) {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    //    }
                    //    else {
                    //        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                    //        //$("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step01 Approval
                    //    }
                    //}
                    //else if (site_id == 5) {//Principal   

                    //    //alert('3-555');

                    //    //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                    //    if (prn_st == 1 && hod_st == 2) {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    //    } else {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    //    }

                    //}
                    //else if (site_id == 2) {//HR=Admin

                    //    //alert('3-222');

                    //    if (hr_st == 1 && prn_st == 2 && hod_st == 2) {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    //    } else {
                    //        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                    //        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Principal Approval is req.
                    //    }
                    //}
                }
                else {//greater than and = to Grade 20

                    if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                        alert("The Request for this Leave Is Approved");
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }

                    if (site_id == 4) {//HOD=LM

                        //alert('4-444');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                        if (hod_st == 1) {

                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                        }
                        else {
                            alert('Already Approved by HOD');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                        }
                    }
                    else if (site_id == 5) {//Principal

                        //alert('4-555');
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                        if (prn_st == 1 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                        }
                        else if (prn_st == 1 && hod_st == 1) {
                            alert('Approval By HOD Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else {
                            alert('Already Approved by Principle');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                            //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                            //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                        }
                    }
                    else if (site_id == 2) {//HR=Admin
                        //alert('4-222');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                        if (hr_st == 1 && prn_st == 2 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                            $("#modal_Leave_Application_val_id option[value='2']").val("-3");//FINAL Approval
                        }
                        else if (hr_st == 1 && prn_st == 1 && hod_st == 2) {
                            alert('Approval By Principle required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else if (hr_st == 1 && prn_st == 1 && hod_st == 1) {
                            alert('Approval By HOD and Principle Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else {
                            alert('Already Approved by HR');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }
                    else if (site_id == 6) {//6 VC
                        //alert('4-666');

                        //$("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        if (vc_st == 1 && hr_st == 2 && prn_st == 2 && hod_st == 2) {
                            $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                            $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                        }
                        else if (vc_st == 1 && hr_st == 1 && prn_st == 2 && hod_st == 2) {
                            alert('Approval By HR Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else if (vc_st == 1 && hr_st == 1 && prn_st == 1 && hod_st == 1) {
                            alert('Approval By HOD and Principle and HR Required');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                        else {
                            alert('Already Approved By VC');
                            //added by M
                            $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                            $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        }
                    }

                }
            }
            else if (ltype_id == 4) {//exPak for ALL
                if (val_rem == 2 && hod_st == 2 && hr_st == 2 && prn_st == 2 && vc_st == 2) {
                    alert("The Request for this Leave Is Approved");
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }

                if (site_id == 4) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (hod_st == 1 || hod_st==0) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by HOD');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
                else if (site_id == 5) {//Principal

                    //alert('4-555');
                    //$("#modal_Leave_Application_val_id option[value='2']").val("-5");
                    if ((prn_st == 1 || prn_st==0) && hod_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by Principal");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-5");//Step02 Approval
                    }
                    else if ((prn_st == 1 || prn_st == 0) && (hod_st == 1 || hod_st==0)) {
                        alert('Approval By HOD Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else {
                        alert('Already Approved by Principle');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        //$("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        //$("#modal_Leave_Application_val_id option[value='2']").val("-4");//Step02 HOD Approval req.
                    }
                }
                else if (site_id == 2) {//HR=Admin
                    //alert('4-222');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-3");
                    if ((hr_st == 1 || hr_st==0) && prn_st == 2 && hod_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HR");
                        $("#modal_Leave_Application_val_id option[value='2']").val("-3");//FINAL Approval
                    }
                    else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st==0) && hod_st == 2) {
                        alert('Approval By Principle required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else if ((hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st == 0) && (hod_st == 1 || hod_st==0)) {
                        alert('Approval By HOD and Principle Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else {
                        alert('Already Approved by HR');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
                else if (site_id == 6) {//6 VC
                    //alert('4-666');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    if ((vc_st == 1 || vc_st==0) && hr_st == 2 && prn_st == 2 && hod_st == 2) {
                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by VC");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//FINAL Approval
                    }
                    else if ((vc_st == 1 || vc_st == 0) && (hr_st == 1 || hr_st==0) && prn_st == 2 && hod_st == 2) {
                        alert('Approval By HR Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else if ((vc_st == 1 || vc_st == 0) && (hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st==0) && hod_st == 2) {
                        alert('Approval By and Principle and HR Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else if ((vc_st == 1 || vc_st == 0) && (hr_st == 1 || hr_st == 0) && (prn_st == 1 || prn_st == 0) && (hod_st == 1 || hod_st==0)) {
                        alert('Approval By HOD and Principle and HR Required');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                    else {
                        alert('Already Approved By VC');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                    }
                }
            }
            else if (ltype_id >= 5 && ltype_id <= 15) {// Extra Type of leaves
                //do nothing
                if (site_id == 4) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by HOD');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
            }
            else if (ltype_id >= 16) {// Maunal Attendance Options to Apply
                //do nothing
                ////alert('manual attendance');
                if (site_id == 4) {//HOD=LM

                    //alert('4-444');

                    //$("#modal_Leave_Application_val_id option[value='2']").val("-4");
                    if (val_rem == 1) {

                        $("#modal_Leave_Application_val_id option[value='2']").text("Approval by HOD");
                        $("#modal_Leave_Application_val_id option[value='2']").val("2");//Step01 Approval
                    }
                    else {
                        alert('Already Approved by HOD');
                        //added by M
                        $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                        $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                        /* $("#modal_Leave_Application_val_id option[value='2']").val("-4");*///Step01 Approval
                    }
                }
                else {
                    alert('Allocation Leave Approved Only By HOD');
                    //added by M
                    $("#modal_Leave_Application_val_id option[value='1']").change.text(" ");
                    $("#modal_Leave_Application_val_id option[value='2']").change.text(" ");
                }

            }
        }
        ////////////// [END] /////////////////////////////////////

        $('#modal_' + capital + '_id').val(id);
        $('#modal_' + capital + '_employee_id').val(emp_id);
        $('#modal_' + capital + '_access_id').val(access_id);
        $('#modal_' + capital + '_grade_id').val(grd_id);
        $('#modal_' + capital + '_site_id').val(site_id);
        $('#modal_' + capital + '_leave_type_id').val(ltype_id);
        $('#modal_' + capital + '_hod_st').val(hod_st);
        $('#modal_' + capital + '_prn_st').val(prn_st);
        $('#modal_' + capital + '_hr_st').val(hr_st);
        $('#modal_' + capital + '_vc_st').val(vc_st);
        ////
        $('#modal_' + capital + '_from_date').val(from_date);
        $('#modal_' + capital + '_to_date').val(to_date);
        $('#modal_' + capital + '_days_count').val(days_count);
        $('#modal_' + capital + '_val_id').val(val_id);
        $('#modal_' + capital + '_val_rem').val(val_rem);

        rowBeingEdited = id;

        editDialog.dialog("open");

        $('#modal_' + capital + '_val_id').focus();
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    editDialog.find("form").on("submit", function (id, emp_id, access_id,usite_id, grd_id, site_id, ltype_id, hod_st, prn_st, hr_st, vc_st, from_date, to_date, days_count, val_id, val_rem, status) {
        event.preventDefault();
        updateItem();


    });

    return toReturn;
}


/////////////////////////////////////// DELETE SECTION //////////////////////////////////////////////

function initDeleteModal(parentDivIdentifier, deleteURL, capital) {


    var HTML = "\
        <div id='delete-dialog-form' class='jqUI-modal' title='Delete "+ capital.replace("_", " ") + "'>\
        <p>Are you sure you want to delete the record with ID: <span id='delete-dialog-field-id'></span>?</p>\
        <form>\
            <fieldset>\
            <!-- <label>"+ capital + " From Date: <span id='delete-dialog-field-name'></span></label> -->\
            <!-- <label>" + capital + " To Date: <span id='delete-dialog-field-description'></span></label> -->\
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
                from_date: "",
                to_date: ""
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
        width: 475,
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
        ////var name = $('*[data-row="' + rowToRemove + '"] > td:eq(0)').html();
        ////var description = $('*[data-row="' + rowToRemove + '"] > td:eq(1)').html();

        $('#delete-dialog-field-id').html(rowToRemove);

        ////$('#delete-dialog-field-name').html(rowToRemove);
        ////$('#delete-dialog-field-description').html(description);


        deleteDialog.dialog("open");
    };

    // When the enter key is pressed
    // prevent the form from being submitted.
    deleteDialog.find("form").on("submit", function (id, from_date, to_date, days_count, status) {
        event.preventDefault();
        deleteItem();


    });


    return toReturn;


}



