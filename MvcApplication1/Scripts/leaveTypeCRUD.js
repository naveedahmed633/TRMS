

function initDeleteModal(parentDivIdentifier, deleteURL, capital) {


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

                //alert(data.status);
                if (data.status == "already") {
                    deleteDialog.dialog("close");
                    $('#toast2').html('Cant Delete! Leaves already applied unde this Leave Type');
                    $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);
                }
                else {
                    $('*[data-row="' + rowToRemove + '"]').parent().parent().remove();

                    deleteDialog.dialog("close");
                    $('#toast2').html('Record deleted successfully');
                    $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);
                }

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

function initEditLeaveType(parentDivIdentifier, updateURL, capital) {

    var HTML = "\
        <div id='dialog-form2' class='jqUI-modal' title='Edit " + capital + "'>\
        <p class='validationTips'>All form fields are required.</p>\
        <form>\
            <fieldset>\
            <input type='hidden' name='id' id='id'>\
            <label class='control-label m-t-10'>Name</label>\
            <input type='text' class='form-control' name='lt_name' id='lt_name' />\
            <label class='control-label m-t-10'>Default Count</label>\
            <select class='form-control' name='leaves_default_count' id='ddlLeaveDefaultCount' required>\
                <option value='0' selected='selected'>00</option>\
                <option value='1'>01</option><option value='2'>02</option><option value='3'>03</option><option value='4'>04</option><option value='5'>05</option>\
                <option value='6'>06</option><option value='7'>07</option><option value='8'>08</option><option value='9'>09</option><option value='10'>10</option>\
                <option value='11'>11</option><option value='12'>12</option><option value='13'>13</option><option value='14'>14</option><option value='15'>15</option>\
                <option value='16'>16</option><option value='17'>17</option><option value='18'>18</option><option value='19'>19</option><option value='20'>20</option>\
                <option value='21'>21</option><option value='22'>22</option><option value='23'>23</option><option value='24'>24</option><option value='25'>25</option>\
                <option value='26'>26</option><option value='27'>27</option><option value='28'>28</option><option value='29'>29</option><option value='30'>30</option>\
                <option value='31'>31</option><option value='32'>32</option><option value='33'>33</option><option value='34'>34</option><option value='35'>35</option>\
                <option value='36'>36</option><option value='37'>37</option><option value='38'>38</option><option value='39'>39</option><option value='40'>40</option>\
                <option value='41'>41</option><option value='42'>42</option><option value='43'>43</option><option value='44'>44</option><option value='45'>45</option>\
                <option value='46'>46</option><option value='47'>47</option><option value='48'>48</option><option value='49'>49</option><option value='50'>50</option>\
                <option value='51'>51</option><option value='52'>52</option><option value='53'>53</option><option value='54'>54</option><option value='55'>55</option>\
                <option value='56'>56</option><option value='57'>57</option><option value='58'>58</option><option value='59'>59</option><option value='60'>60</option>\
                <option value='61'>61</option><option value='62'>62</option><option value='63'>63</option><option value='64'>64</option><option value='65'>65</option>\
                <option value='66'>66</option><option value='67'>67</option><option value='68'>68</option><option value='69'>69</option><option value='70'>70</option>\
                <option value='71'>71</option><option value='72'>72</option><option value='73'>73</option><option value='74'>74</option><option value='75'>75</option>\
                <option value='76'>76</option><option value='77'>77</option><option value='78'>78</option><option value='79'>79</option><option value='80'>80</option>\
                <option value='81'>81</option><option value='82'>82</option><option value='83'>83</option><option value='84'>84</option><option value='85'>85</option>\
                <option value='86'>86</option><option value='87'>87</option><option value='88'>88</option><option value='89'>89</option><option value='90'>90</option>\
                <option value='91'>91</option><option value='92'>92</option><option value='93'>93</option><option value='94'>94</option><option value='95'>95</option>\
                <option value='96'>96</option><option value='97'>97</option><option value='98'>98</option><option value='99'>99</option>\
            </select>\
            <label class='control-label m-t-10'>Max. Count</label>\
            <select class='form-control' name='leaves_max_count' id='ddlLeaveMaxCount' required>\
                <option value='0' selected='selected'>00</option>\
                <option value='1'>01</option><option value='2'>02</option><option value='3'>03</option><option value='4'>04</option><option value='5'>05</option>\
                <option value='6'>06</option><option value='7'>07</option><option value='8'>08</option><option value='9'>09</option><option value='10'>10</option>\
                <option value='11'>11</option><option value='12'>12</option><option value='13'>13</option><option value='14'>14</option><option value='15'>15</option>\
                <option value='16'>16</option><option value='17'>17</option><option value='18'>18</option><option value='19'>19</option><option value='20'>20</option>\
                <option value='21'>21</option><option value='22'>22</option><option value='23'>23</option><option value='24'>24</option><option value='25'>25</option>\
                <option value='26'>26</option><option value='27'>27</option><option value='28'>28</option><option value='29'>29</option><option value='30'>30</option>\
                <option value='31'>31</option><option value='32'>32</option><option value='33'>33</option><option value='34'>34</option><option value='35'>35</option>\
                <option value='36'>36</option><option value='37'>37</option><option value='38'>38</option><option value='39'>39</option><option value='40'>40</option>\
                <option value='41'>41</option><option value='42'>42</option><option value='43'>43</option><option value='44'>44</option><option value='45'>45</option>\
                <option value='46'>46</option><option value='47'>47</option><option value='48'>48</option><option value='49'>49</option><option value='50'>50</option>\
                <option value='51'>51</option><option value='52'>52</option><option value='53'>53</option><option value='54'>54</option><option value='55'>55</option>\
                <option value='56'>56</option><option value='57'>57</option><option value='58'>58</option><option value='59'>59</option><option value='60'>60</option>\
                <option value='61'>61</option><option value='62'>62</option><option value='63'>63</option><option value='64'>64</option><option value='65'>65</option>\
                <option value='66'>66</option><option value='67'>67</option><option value='68'>68</option><option value='69'>69</option><option value='70'>70</option>\
                <option value='71'>71</option><option value='72'>72</option><option value='73'>73</option><option value='74'>74</option><option value='75'>75</option>\
                <option value='76'>76</option><option value='77'>77</option><option value='78'>78</option><option value='79'>79</option><option value='80'>80</option>\
                <option value='81'>81</option><option value='82'>82</option><option value='83'>83</option><option value='84'>84</option><option value='85'>85</option>\
                <option value='86'>86</option><option value='87'>87</option><option value='88'>88</option><option value='89'>89</option><option value='90'>90</option>\
                <option value='91'>91</option><option value='92'>92</option><option value='93'>93</option><option value='94'>94</option><option value='95'>95</option>\
                <option value='96'>96</option><option value='97'>97</option><option value='98'>98</option><option value='99'>99</option>\
            </select>\
            \
            <input type='submit' tabindex='-1' style='position:absolute; top:-1000px'>\
            </fieldset>\
        </form>\
        </div>";

    $(parentDivIdentifier).append(HTML);

    var id = $('#id');
    var lt_name = $("#lt_name");
    var dcount = $("#ddlLeaveDefaultCount");
    var mcount = $("#ddlLeaveMaxCount");
    //var is_active = $("#isforactive");

    var allFields = $([]).add(lt_name).add(dcount).add(mcount);


    function updateItem() {
        allFields.removeClass("ui-state-error");

        var valid = true;
        //valid = valid && checkLength(fieldName, capital + " name", 2, 50);
        // valid = valid && checkLength(fieldDescription, "description", 0, 100);
        //var check2 = $("#isforacademiccalendar2").is(":checked");
        if (valid) {
            // do an ajax call
            $.ajax({
                type: "POST",
                url: updateURL,
                data: {
                    Id: parseInt(id.val()),
                    LeaveTypeText: lt_name.val(),
                    LeaveDefaultCount: dcount.val(),
                    LeaveMaxCount: mcount.val()
                    //IsActive: is_active.is(":checked"),
                },
                success: function (data) {
                    if (data.status == "already") {
                        editDialog.dialog("close");
                        $('#toast2').html('Record already exists.');
                        $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);
                    }
                    else if (data.status == "already-name") {
                        editDialog.dialog("close");
                        $('#toast2').html('Leave Type Name already exists.');
                        $('#toast2').stop().fadeIn(400).delay(9000).fadeOut(400);
                    } else {
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(1)').html(lt_name.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(2)').html(dcount.val());
                        $('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(3)').html(mcount.val());
                        //$('*[data-row="' + rowBeingEdited + '"]').parent().parent().find('td:eq(4)').html(is_active.val());

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

    editDialog = $("#dialog-form2").dialog({
        autoOpen: false,
        height: 420,
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


    toReturn = function (id, lt_name, dcount, mcount) {

        $('#id').val(id);
        $('#lt_name').val(lt_name);
        $('#ddlLeaveDefaultCount').val(dcount);
        $("#ddlLeaveMaxCount").val(mcount);
        //$('#is_active').val(is_active);

        //if (is_active == "True") {
        //    $('input[name=is_active]').attr('checked', true);
        //}
        //else {
        //    $('input[name=is_active]').attr('checked', false);
        //}

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
