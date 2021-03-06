﻿$(document).ready(function () {
    // Product Dropdown
    $("#prod_sel_list").on("change", function () {
        var selId = $(this).val();
        location.href = "/GoalMgmt/?prodid=" + selId;
        //alert("Valueselected is id: " + selId);
    });

    // Metric Dropdown
    $("#mp_sel_list").on("change", function () {
        var prodId = $('#prod_sel_list').val();
        var selId = $(this).val();
        location.href = "/GoalMgmt/?prodid=" + prodId + "&mpid=" + selId;
        //alert("Valueselected is id: " + selId);
    });

    // Enterprise Summary Link (Top Right)
    $('#lnkEnterpriseSummary').on('click', function () {
        $('#mdlEnterpriseGoals').modal('show');
    });

    // Manage Enterprise Goals Popup
    $(document).on('click', '#lnkManageEntGoal', function () {
        var prodId = $("#hdnProdId").val();
        var mpId = $("#hdnMpId").val();

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/GoalMgmt/_Detail',     // the url where we want to direct our Ajax Call
            method: "GET",
            cache: false,
            data: { prodId: prodId, mpId: mpId },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlGoalMgmtBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            $('#mdlGoalMgmtBody').html(d);
            $('#mdlGoalMgmt').modal('show');
        });
    });

    // Manage Building Goals Popup
    $(document).on('click', '#lnkManageBldgGoal', function () {
        var prodId = $("#hdnProdId").val();
        var mpId = $("#hdnMpId").val();
        var bldgId = $(this).siblings('#hdnBldgId').val();

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/GoalMgmt/_Detail',     // the url where we want to direct our Ajax Call
            method: "GET",
            cache: false,
            data: { prodId: prodId, mpId: mpId, bldgId: bldgId },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlGoalMgmtBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            $('#mdlGoalMgmtBody').html(d);
            $('#mdlGoalMgmt').modal('show');
        });
    });

    // Manage Goal Rule Dropdown
    $("#mdlGoalMgmt").on('change', '#ruleOptions', function () {
        var val = $(this).val();

        if (val == 0) {
            $('.btwnVal').hide();
            $('.singleVal').hide();
        }
        else if (val == 'Between') {
            $('.btwnVal').show();
            $('.singleVal').hide();
        } else {
            $('.btwnVal').hide();
            $('.singleVal').show();
        }
    });

    // Set New Goal Button
    $("#mdlGoalMgmt").on('click', '#btnEditGoal', function () {
        $('#divEditGoal').show()
    });

    // Cancel Adding New Goal
    $("#mdlGoalMgmt").on('click', '#btnCancelEdit', function () {
        $('#ruleOptions').val(0);  //Reset Dropdown Selection
        $('.singleVal').hide();
        $('.btwnVal').hide();
        $('#divEditGoal').hide()
    });

    $("#mdlGoalMgmt").on('change', '.goalField', function () {
        $('#divErrorMsg').html("");
    });


    // Add/Save New Goal
    $("#mdlGoalMgmt").on('click', '#btnSaveGoal', function () {
        var prodId = $("#hdnProdId").val();
        var mpId = $("#hdnMpId").val();
        var bldgId = $("#hdnBldgId").val();
        var mpGoal = "";
        var date1 = "";
        var date2 = "";
        //Perform Validation
        if (prodId == '' || mpId == '' || bldgId == '') {
            $('#divErrorMsg').html("Invalid Metric Information was found. Please refresh the form and resubmit again");
            return;
        }
        if ($('#ruleOptions').val() == 0 || $('#ruleOptions').val() == null) {
            $('#divErrorMsg').html("A Rule must be specified.");
            return;
        }

        if ($('#ruleOptions').val() == 'Between') {
            var message = "";
            if ($('#btwnRuleVal1').val() == '') {
                message = "A Starting Value Range must be specified";
            }
            if ($('#btwnRuleVal2').val() == '') {
                if (message != "") { message = message + "<br>"; }
                message = message + "An Ending Value Range must be specified";
            }

            if (message != "") {
                message = message + "<br/>For Selection '" + $('#ruleOptions').val() + "'";
                $('#divErrorMsg').html(message);
                return;
            }
            mpGoal = "Between," + $('#btwnRuleVal1').val() + "," + $('#btwnRuleVal2').val() + "," + $('#chkVal1').prop('checked') + "," + $('#chkVal2').prop('checked');
        }
        else {
            if ($('#singleRuleVal').val() == '') {
                $('#divErrorMsg').html("A Value must be specified for selection '" + $('#ruleOptions').val() + "'");
                return;
            }
            mpGoal = $('#ruleOptions').val() + "," + $('#singleRuleVal').val();
        }

        if ($('#start').val() == '' || $('#end').val() == '') {
            $('#divErrorMsg').html("Valid Start / End Dates must be Entered");
            return;
        }

        date1 = $('#start').val();
        date2 = $('#end').val();

        //Compile the Form Values to pass to the Ajax Call
        //Mpbg_less_val
        //Mpbg_less_eq_val
        //Mpbg_greater_val
        //Mpbg_greater_eq_val
        //Mpbg_equal_val

        //Validation PAssed. Submit request via Ajax to Save/Add the Building Metric Period Goals
        // ------------ Make the Ajax Call To Save the Current Building Goal ---------------------------------------
        $.ajax({
            url: '/GoalMgmt/_addSaveBuildingGoal',
            method: "POST",
            cache: false,
            data: { prodId: prodId, mpId: mpId, bldgId: bldgId, mpGoal: mpGoal, startDate: date1, endDate: date2},     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlGoalMgmtBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            var wasSuccess = false;
            if (d.substring(0, 8) == "SUCCESS:") {
                alert(d.substring(8));
            }
            else { $('#divErrorMsg').html(d); }
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal

            

        });

    });



    // Effective date datepicker
    //$("#mdlGoalMgmt").on('change', '#ruleOptions', function () {

    //});

});