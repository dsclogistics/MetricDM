$(document).ready(function () {

    $('.btn-toolbar').on('click', '.btn', function (e) {
        var $target = $(this).parentsUntil('btn-toolbar').next('.collapse');
        //alert($target.attr("aria-expanded"));
        $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();

        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });

    $('.panel-heading').on('click', '.glyph-link', function (e) {
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });


    //-------------------------------------------------------------------------------------------------
    //Building Modal
    //-------------------------------------------------------------------------------------------------
    $("#divBldgBtns").on('click', '#btnManageUserBuildings', function () {
        var userId = $("#hdnAppUserId").val();

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserBldgAssign',     // the url where we want to direct our Ajax Call
            method: "GET",
            cache: false,
            data: { app_user_id: userId },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlBldgAsgnBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            $('#mdlBldgAsgnBody').html(d);
            $('#mdlBldgAsgn').modal('show');
        });
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlBldgAsgnBody").on('click', '#btnAsgnBldg', function () {
        //moveSelectListItem('unasgndBldgList', 'asgndBldgList', false);
        moveListItems('unasgndBldgList', 'asgndBldgList', false);
    });
    $("#mdlBldgAsgnBody").on('click', '#btnUnasgnBldg', function () {
        //moveSelectListItem('asgndBldgList', 'unasgndBldgList', false);
        moveListItems('asgndBldgList', 'unasgndBldgList', false);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlBldgAsgnBody").on('dblclick', '#unasgndBldgList', function () {
        moveListItems('unasgndBldgList', 'asgndBldgList', false);
    });
    $("#mdlBldgAsgnBody").on('dblclick', '#asgndBldgList', function () {
        moveListItems('asgndBldgList', 'unasgndBldgList', false);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlBldgAsgn").on('click', '#btnSaveBldgAsgn', function () {
        var userId = $("#hdnAppUserId").val();

        var selElem = document.getElementById('asgndBldgList');
        var tmpArr = [];
        for (var i = 0; i < selElem.options.length; i++) {
            tmpArr[i] = parseInt(selElem.options[i].value);
        }

        if (userId == null) {
            userId = 0;
        }

        data = {
            app_user_id: userId,
            asgndBldgList: tmpArr
        }

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserBldgAssign',     // the url where we want to direct our Ajax Call
            method: "POST",
            cache: false,
            data: { raw_json: JSON.stringify(data) },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlBldgAsgnBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            //$('#mdlBldgAsgnBody').html(d);
            //$('#mdlBldgAsgn').modal('show');
            showAlert('Success!', '', 'Y');
        });

    });
    //-------------------------------------------------------------------------------------------------


    //-------------------------------------------------------------------------------------------------
    //Metric Modal
    //-------------------------------------------------------------------------------------------------
    $(".divMtrcBtns").on('click', '#btnManageRoleMetrics', function () {
        var userRoleId = $(this).siblings('.hdnMuarId').val();

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserRoleMtrcAssign',     // the url where we want to direct our Ajax Call
            method: "GET",
            cache: false,
            data: { app_user_role_id: userRoleId },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlMtrcAsgnBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            $('#mdlMtrcAsgnBody').html(d);
            $('#mdlMtrcAsgn').modal('show');
        });
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlMtrcAsgnBody").on('click', '#btnAsgnMtrc', function () {
        //moveSelectListItem('unasgndMtrcList', 'asgndMtrcList', true);
        moveListItems('unasgndMtrcList', 'asgndMtrcList', true);
    });
    $("#mdlMtrcAsgnBody").on('click', '#btnUnasgnMtrc', function () {
        //moveSelectListItem('asgndMtrcList', 'unasgndMtrcList', true);
        moveListItems('asgndMtrcList', 'unasgndMtrcList', true);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlMtrcAsgnBody").on('dblclick', '#unasgndMtrcList', function () {
        moveListItems('unasgndMtrcList', 'asgndMtrcList', true);
    });
    $("#mdlMtrcAsgnBody").on('dblclick', '#asgndMtrcList', function () {
        moveListItems('asgndMtrcList', 'unasgndMtrcList', true);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlMtrcAsgn").on('click', '#btnSaveMtrcAsgn', function () {
        var userId = $("#hdnAppUserId").val();
        var userRoleId = $('#hdnAppUserRoleId').val();

        var selElem = document.getElementById('asgndMtrcList');
        var tmpArr = [];
        for (var i = 0; i < selElem.options.length; i++) {
            tmpArr[i] = parseInt(selElem.options[i].value);
        }

        if (userId == null) {
            userId = 0;
        }

        data = {
            app_user_id: userId,
            app_user_role_id: userRoleId, 
            asgndMtrcList: tmpArr
        }

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserRoleMtrcAssign',     // the url where we want to direct our Ajax Call
            method: "POST",
            cache: false,
            data: { raw_json: JSON.stringify(data) },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlMtrcAsgnBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            showAlert('Success!', '', 'Y');
        });

    });
    //-------------------------------------------------------------------------------------------------


    //-------------------------------------------------------------------------------------------------
    //Role Modal
    //-------------------------------------------------------------------------------------------------
    $("#divRoleBtns").on('click', '#btnManageUserRoles', function () {
        var userId = $("#hdnAppUserId").val();

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserRoleAssign',     // the url where we want to direct our Ajax Call
            method: "GET",
            cache: false,
            data: { app_user_id: userId },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlRoleAsgnBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            $('#mdlRoleAsgnBody').html(d);
            $('#mdlRoleAsgn').modal('show');
        });
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlRoleAsgnBody").on('change', '#prod_sel_list', function () {
        var userId = $("#hdnAppUserId").val();
        var prodId = $("#prod_sel_list").val();

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserRoleAssign',     // the url where we want to direct our Ajax Call
            method: "GET",
            cache: false,
            data: { app_user_id: userId, prod_id: prodId },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlRoleAsgnBody").html(xhr.responseText);
                $('#mdlRoleAsgn').modal('show');
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            $('#mdlRoleAsgnBody').html(d);
            $('#mdlRoleAsgn').modal('show');
        });
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlRoleAsgnBody").on('change', '#role_sel_list', function () {
        var userId = $("#hdnAppUserId").val();
        var prodId = $("#prod_sel_list").val();
        var marId = $("#role_sel_list").val();

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserRoleAssign',     // the url where we want to direct our Ajax Call
            method: "GET",
            cache: false,
            data: { app_user_id: userId, prod_id: prodId, mar_id: marId },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlRoleAsgnBody").html(xhr.responseText);
                $('#mdlRoleAsgn').modal('show');
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            $('#mdlRoleAsgnBody').html(d);
            $('#mdlRoleAsgn').modal('show');
        });
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlRoleAsgnBody").on('click', '#btnAsgnRoleBldg', function () {
        //moveSelectListItem('unasgndRoleBldgList', 'asgndRoleBldgList', false);
        moveListItems('unasgndRoleBldgList', 'asgndRoleBldgList', false);
    });
    $("#mdlRoleAsgnBody").on('click', '#btnUnasgnRoleBldg', function () {
        //moveSelectListItem('asgndRoleBldgList', 'unasgndRoleBldgList', false);
        moveListItems('asgndRoleBldgList', 'unasgndRoleBldgList', false);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlRoleAsgnBody").on('dblclick', '#unasgndRoleBldgList', function () {
        moveListItems('unasgndRoleBldgList', 'asgndRoleBldgList', false);
    });
    $("#mdlRoleAsgnBody").on('dblclick', '#asgndRoleBldgList', function () {
        moveListItems('asgndRoleBldgList', 'unasgndRoleBldgList', false);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlRoleAsgnBody").on('click', '#btnAsgnRoleMtrc', function () {
        //moveSelectListItem('unasgndRoleMtrcList', 'asgndRoleMtrcList', true);
        moveListItems('unasgndRoleMtrcList', 'asgndRoleMtrcList', true);
    });
    $("#mdlRoleAsgnBody").on('click', '#btnUnasgnRoleMtrc', function () {
        //moveSelectListItem('asgndRoleMtrcList', 'unasgndRoleMtrcList', true);
        moveListItems('asgndRoleMtrcList', 'unasgndRoleMtrcList', true);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlRoleAsgnBody").on('dblclick', '#unasgndRoleMtrcList', function () {
        moveListItems('unasgndRoleMtrcList', 'asgndRoleMtrcList', true);
    });
    $("#mdlRoleAsgnBody").on('dblclick', '#asgndRoleMtrcList', function () {
        moveListItems('asgndRoleMtrcList', 'unasgndRoleMtrcList', true);
    });
    //-------------------------------------------------------------------------------------------------
    $("#mdlRoleAsgn").on('click', '#btnSaveRoleAsgn', function () {
        var userId = $("#hdnAppUserId").val();
        var marId = $("#role_sel_list").val();
        var bldgArr = [];
        var mtrcArr = [];

        if ($("#hdnReqMtrcAuth").val() == 'Y') {
            var selElem = document.getElementById('asgndRoleMtrcList');
            for (var i = 0; i < selElem.options.length; i++) {
                mtrcArr[i] = parseInt(selElem.options[i].value);
            }
        }

        if ($("#hdnReqBldgAuth").val() == 'Y') {
            var selElem = document.getElementById('asgndRoleBldgList');
            for (var i = 0; i < selElem.options.length; i++) {
                bldgArr[i] = parseInt(selElem.options[i].value);
            }
        }

        if (userId == null) {
            userId = 0;
        }

        data = {
            app_user_id: userId,
            mar_id: marId, 
            asgndMtrcList: mtrcArr,
            asgndBldgList: bldgArr
        }

        // ------------ Make the Ajax Call --------------------------------------------------------------
        $.ajax({
            url: '/UserMgmt/_UserRoleAssign',     // the url where we want to direct our Ajax Call
            method: "POST",
            cache: false,
            data: { raw_json: JSON.stringify(data) },     //<---- Data Parameters (if not already passed in the Url)
            //--- On error, execute this function ------
            error: function (xhr, status, error) {
                //var err = eval("(" + xhr.responseText + ")");
                $("#mdlMtrcAsgnBody").html(xhr.responseText);
                //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
            if (d == "Success") {
                showAlert('Success!', '', 'Y');
            }
            else {
                showAlert('An error occurred.', '', 'Y');
            }
        });

    });
    //-------------------------------------------------------------------------------------------------
    $(document).on('click', '.btnUserRoleRemove', function () {
        var userId = $("#hdnAppUserId").val();
        var muarId = $(this).siblings('.hdnDelMuarId').val();

        if (userId == null) { userId = 0; }
        if (muarId == null) { muarId = 0; }

        data = {
            app_user_id: userId,
            muar_id: muarId
        }

        var next = confirm("This action will remove the user role and all associated metric associations. Are you sure you want to continue?");

        if (next) {
            // ------------ Make the Ajax Call --------------------------------------------------------------
            $.ajax({
                url: '/UserMgmt/_RemoveUserRole',     // the url where we want to direct our Ajax Call
                method: "POST",
                cache: false,
                data: { raw_json: JSON.stringify(data) },     //<---- Data Parameters (if not already passed in the Url)
                //--- On error, execute this function ------
                error: function (xhr, status, error) {
                    //var err = eval("(" + xhr.responseText + ")");
                    $("#mdlMtrcAsgnBody").html(xhr.responseText);
                    //alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
                }
            }).done(function (d) {
                //Execute this code After the Ajax call completes successfully
                //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
                if (d == "Success") {
                    showAlert('Success!', '', 'Y');
                }
                else {
                    showAlert('An error occurred.', '', 'Y');
                }
            });
        }

    });
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Disable/Enable Buttons
    //-------------------------------------------------------------------------------------------------
    $(document).on('click', '#btnDisableUser', function () {
        var userId = $("#hdnAppUserId").val();

        if (userId == null) { userId = 0; }

        data = {
            app_user_id: userId
        }

        var next = confirm("This action will disable all login access for the user. Are you sure you want to continue?");

        if (next) {
            // ------------ Make the Ajax Call --------------------------------------------------------------
            $.ajax({
                url: '/UserMgmt/_DisableUser',     // the url where we want to direct our Ajax Call
                method: "POST",
                cache: false,
                data: { raw_json: JSON.stringify(data) },     //<---- Data Parameters (if not already passed in the Url)
                //--- On error, execute this function ------
                error: function (xhr, status, error) {
                    //var err = eval("(" + xhr.responseText + ")");
                    //$("#mdlMtrcAsgnBody").html(xhr.responseText);
                    alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
                }
            }).done(function (d) {
                //Execute this code After the Ajax call completes successfully
                //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
                if (d == "Success") {
                    showAlert('Success!', '', 'Y');
                }
                else {
                    showAlert('An error occurred.', '', 'Y');
                }
            });
        }

    });

    $(document).on('click', '#btnEnableUser', function () {
        var userId = $("#hdnAppUserId").val();

        if (userId == null) { userId = 0; }

        data = {
            app_user_id: userId
        }

        var next = confirm("This action will reenable all login access previous held by the user. Are you sure you want to continue?");

        if (next) {
            // ------------ Make the Ajax Call --------------------------------------------------------------
            $.ajax({
                url: '/UserMgmt/_EnableUser',     // the url where we want to direct our Ajax Call
                method: "POST",
                cache: false,
                data: { raw_json: JSON.stringify(data) },     //<---- Data Parameters (if not already passed in the Url)
                //--- On error, execute this function ------
                error: function (xhr, status, error) {
                    //var err = eval("(" + xhr.responseText + ")");
                    //$("#mdlMtrcAsgnBody").html(xhr.responseText);
                    alert("An Error has Occurred.");  //<-- Trap and alert of any errors if they occurred
                }
            }).done(function (d) {
                //Execute this code After the Ajax call completes successfully
                //Insert the partial view retrieved into the output 'mdlBldgAsgnBody' section of modal
                if (d == "Success") {
                    showAlert('Success!', '', 'Y');
                }
                else {
                    showAlert('An error occurred.', '', 'Y');
                }
            });
        }

    });
});


//---------------------------------------------------
//---------------------Functions---------------------
//---------------------------------------------------
function moveSelectListItem(fromListId, toListId, byId) {
    var fromList = document.getElementById(fromListId);
    var selItem = fromList.selectedIndex;

    if (selItem == -1) {

    } else {
        var toList = document.getElementById(toListId);
        var newOption = fromList[selItem].cloneNode(true);

        fromList.removeChild(fromList[selItem]);
        toList.appendChild(newOption);

        if (byId == true) {
            sortSelectById(fromList);
            sortSelectById(toList);
        }
        else {
            sortSelect(fromList);
            sortSelect(toList);
        }
    }
}

function moveListItems(fromListId, toListId, byId) {
    var fromList = document.getElementById(fromListId);
    var toList = document.getElementById(toListId);

    $(fromList).find(':selected').appendTo(toList);

    if (byId == true) {
        sortSelectById(fromList);
        sortSelectById(toList);
    }
    else {
        sortSelect(fromList);
        sortSelect(toList);
    }
}

function sortSelect(selElem) {
    //http://stackoverflow.com/questions/278089/javascript-to-sort-contents-of-select-element
    var tmpArr = new Array();
    for (var i = 0; i < selElem.options.length; i++) {
        tmpArr[i] = new Array();
        tmpArr[i][0] = selElem.options[i].text;
        tmpArr[i][1] = selElem.options[i].value;
    }
    tmpArr.sort();

    while(selElem.options.length > 0){
        selElem.options[0] = null;
    }
    for (var i = 0; i < tmpArr.length; i++){
        var op = new Option(tmpArr[i][0], tmpArr[i][1]);
        selElem.options[i] = op;
    }
    return;
}

function sortSelectById(selElem) {
    var tmpArr = new Array();
    for (var i = 0; i < selElem.options.length; i++) {
        tmpArr[i] = new Array();
        tmpArr[i][0] = selElem.options[i].text;
        tmpArr[i][1] = parseInt(selElem.options[i].value, 10);
    }
    tmpArr.sort(function (a, b) {
        return a[1] - b[1];
    });

    while (selElem.options.length > 0) {
        selElem.options[0] = null;
    }
    for (var i = 0; i < tmpArr.length; i++) {
        var op = new Option(tmpArr[i][0], tmpArr[i][1]);
        selElem.options[i] = op;
    }
    return;
}