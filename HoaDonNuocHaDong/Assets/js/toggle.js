$(function () {
    $('.mainnav a').filter(function () { return this.href == location.href }).parent().addClass('active').siblings().removeClass('active')
    $('.mainnav a').click(function () {
        $(this).parent().addClass('active').siblings().removeClass('active')
    });
})
//Ẩn hiện cột nhân viên
$(function () {
    var isAdmin = $("input[name='isAdmin']:checked").val();
    if (isAdmin == "1") {
        $(".nhanVienRadio").addClass('hide');
    } else {
        $(".nhanVienRadio").removeClass('hide');
    }

    if ($("#LoaiapgiaID").val() == "7") {
        $(".tonghop").removeClass('hide');
    } else {
        $(".tonghop").addClass('hide');
    }

    //nếu admin được check thì ẩn danh sách nhân viên đi (addClass hide), nếu nhân viên được check sẽ hiện danh sách nhân viên lên (removeClass hide)
    $("input[name='isAdmin']").click(function () {
        var isAdminValue = $(this).val();
        if (isAdminValue == 1) {
            $(".nhanVienRadio").addClass('hide');
        } else {
            $(".nhanVienRadio").removeClass('hide');
        }
    });   
});

//Unlock account
$(".unlockAcc").click(function () {
    var item = $(this);
    var rowID = item.data('rowid');
    $.ajax({
        url: "/Nguoidung/unlockAccount",
        method: "GET",
        contentType: "application/json; charset=utf-8",
        data: { id: rowID },
        success: function (result) {
            item.parents('tr').find(".lockStatus").text("");
            item.next().remove();
            item.remove();
        },
        error: function (result) {
            console.log(result);
        }
    })


});
