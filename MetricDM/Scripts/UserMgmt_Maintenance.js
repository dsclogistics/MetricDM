$(document).ready(function () {

    $('.btn-toolbar').on('click', '.btn', function (e) {
        var $target = $(this).parentsUntil('btn-toolbar').next('.collapse');
        //alert($target.attr("aria-expanded"));
        $target.attr("aria-expanded") ? $target.collapse('toggle') : $target.collapse();
        $(this).children('.glyphicon').toggleClass('glyphicon-chevron-right glyphicon-chevron-down');
    });

    $("#divBldgBtns").on('click', '#btnManageUserBuildings', function () {
        $('#mdlBldgAsgn').modal('show');
    });

    $("#mdlBldgAsgnBody").on('click', '#btnAsgnBldg', function () {
        moveSelectListItem('unasgndBldgList', 'asgndBldgList');
    });

    $("#mdlBldgAsgnBody").on('click', '#btnUnasgnBldg', function () {
        moveSelectListItem('asgndBldgList', 'unasgndBldgList');
    });

});



//Functions
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