$(document).ready(function () {
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

        if(val==0){
            $('.btwnVal').hide();
            $('.singleVal').hide();
        }
        else if(val == 'Between') {
            $('.btwnVal').show();
            $('.singleVal').hide();
        }else{
            $('.btwnVal').hide();
            $('.singleVal').show();
        }
    });

    // Set New Goal Button
    $("#mdlGoalMgmt").on('click', '#btnEditGoal', function () {
        $('#divEditGoal').show()
    });

    // Effective date datepicker
    //$("#mdlGoalMgmt").on('change', '#ruleOptions', function () {

    //});

});