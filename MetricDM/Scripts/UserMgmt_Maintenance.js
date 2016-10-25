$(document).ready(function () {

    $('.btn-toolbar').on('click', '.btn', function (e) {
        var $target = $(this).parentsUntil('btn-toolbar').next('.collapse');
        //alert($target.attr("aria-expanded"));
        $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });

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

    $("#divRoleBtns").on('click', '#btnManageUserRoles', function () {
        var userId = $("#hdnAppUserId").val();
        alert(userId);
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

    $("#mdlBldgAsgnBody").on('click', '#btnAsgnBldg', function () {
        moveSelectListItem('unasgndBldgList', 'asgndBldgList');
    });

    $("#mdlBldgAsgnBody").on('click', '#btnUnasgnBldg', function () {
        moveSelectListItem('asgndBldgList', 'unasgndBldgList');
    });

    $("#mdlMtrcAsgnBody").on('click', '#btnAsgnMtrc', function () {
        moveSelectListItem('unasgndMtrcList', 'asgndMtrcList');
    });

    $("#mdlMtrcAsgnBody").on('click', '#btnUnasgnMtrc', function () {
        moveSelectListItem('asgndMtrcList', 'unasgndMtrcList');
    });

    $("#mdlRoleAsgnBody").on('click', '#btnAsgnRoleBldg', function () {
        moveSelectListItem('unasgndRoleBldgList', 'asgndRoleBldgList');
    });

    $("#mdlRoleAsgnBody").on('click', '#btnUnasgnRoleBldg', function () {
        moveSelectListItem('asgndRoleBldgList', 'unasgndRoleBldgList');
    });

    $("#mdlRoleAsgnBody").on('click', '#btnAsgnRoleMtrc', function () {
        moveSelectListItem('unasgndRoleMtrcList', 'asgndRoleMtrcList');
    });

    $("#mdlRoleAsgnBody").on('click', '#btnUnasgnRoleMtrc', function () {
        moveSelectListItem('asgndRoleMtrcList', 'unasgndRoleMtrcList');
    });

});


//---------------------------------------------------
//---------------------Functions---------------------
//---------------------------------------------------
function moveSelectListItem(fromListId, toListId) {
    var fromList = document.getElementById(fromListId);
    var selItem = fromList.selectedIndex;

    if (selItem == -1) {

    } else {
        var toList = document.getElementById(toListId);
        var newOption = fromList[selItem].cloneNode(true);

        fromList.removeChild(fromList[selItem]);
        toList.appendChild(newOption);

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