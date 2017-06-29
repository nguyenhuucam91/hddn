$(function () {
    $('.friendlyName').change(function () {
        var idRow = $(this).prop("id");
        var idVal = $(this).val();

        //ajax goes here
        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/Log/saveControllerName",
            data: JSON.stringify({ RowID: idRow, RowValue: idVal }),
            success: function (result) {

            },
            error: function (result) {

            }
        });
    });

    //ajax call for friendly name in action
    $(".actionFriendlyName").change(function () {
        var friendlyValue = $(this).val();
        var idRow = $(this).data("rowid");
        $.ajax({
            method:"POST",
            url: "/Log/SaveActionName",
            contentType: "application/json;charset:utf-8",
            dataType: "json",
            data: JSON.stringify({RowID:idRow,RowValue:friendlyValue}),            
        });
    });
});