$(function () {
    var TRUONG_PHONG = 2;
    var chucVuID = $("#ChucvuID");
    var tuyenContainerDiv = $("div.tuyenKhachHangs");
    var tuyensSelectOption = $(".tuyen");
    if (chucVuID.val() == TRUONG_PHONG) {
        tuyenContainerDiv.hide();
    }

    var isTruongPhongAndAnTuyen = $(function () {
        chucVuID.change(function () {
            var chucVuID = $(this).val();            
            if (chucVuID == TRUONG_PHONG) {
                tuyensSelectOption.val(null).trigger("change");;
                tuyenContainerDiv.hide();
            }
            else {
                tuyenContainerDiv.show();
            }
        });
    });
});