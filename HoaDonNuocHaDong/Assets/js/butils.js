function BUtils() {

}
/* PUBLIC STATIC METHODS */
// return <select> selected text
BUtils.slGetSelectedText = function ($select) {
    return $select.find(":selected").text();
}
// clear <select> options
BUtils.slRemoveAllOptions = function ($select) {
    $select.html("");
}
// check if n is numeric
BUtils.isNumeric = function (n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

// ajax with AjaxResult
BUtils.ajax = function (url, data, fnSuccess, fnError, type="get") {
    $.ajax({
        type: type,
        url: url,
        data: data
    }).done(function (data) {
        if (data.IsSuccess) {
            if (fnSuccess !== undefined)
                fnSuccess(data);
            if (data.ForceRefresh) {
                location.reload();
            }
        } else {
            if (fnError !== undefined) {
                fnError(data);
            }
            if (data.ForceRefresh) {
                if (confirm(data.Message + " Xác nhận?")) {
                    location.reload();
                }
            }
        }
    }).fail(function () {
        alert("Lỗi kết nối, vui lòng kiểm tra lại.");
    });
}