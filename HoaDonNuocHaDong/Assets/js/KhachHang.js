/* Hàm để đánh địa chỉ thu tiền trùng với địa chỉ*/
    $(function () {
        $("#Diachi").keyup(function () {
            $("#Diachithutien").val($(this).val());
        });
    })
