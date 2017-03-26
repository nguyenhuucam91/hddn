
/*sửa select2.js sau khi chọn bị mất focus*/
$('.dropdown').on("select2:selecting", function (e) {
    // what you would like to happen
    $(this).next(".select2-container").addClass('select2-container--focus');
});

/* load combo box select2 trong phần lọc khách hàng*/

/* ------------ Load danh sách quận trong phần lọc --------------*/

$(".dropdown.quan").select2({
    placeholder: "Chọn quận",
});

$(".dropdown.quanAllowClear").select2({
    placeholder: "Chọn quận",
    allowClear: true,
});

//phường
$(".dropdown.phuong").select2({
    placeholder: "Chọn phường",
});
$(".dropdown.phuongAllowClear").select2({
    placeholder: "Chọn phường",
    allowClear: true,
});
//phòng ban
$(".dropdown.phongBan").select2({
    placeholder: "Lọc tất cả phòng ban",
});

$(".dropdown.phongbanAllowClear").select2({
    placeholder: "Lọc tất cả phòng ban",
    allowClear: true
});

//cụm dân cư
$(".dropdown.cumdancu").select2({
    placeholder: "Chọn cụm dân cư",

});

$(".dropdown.chinhanh").select2({
    placeholder: "Chọn chi nhánh",
    allowClear: true,
});

//tổ quận huyện
$(".dropdown.to").select2({
    placeholder: "Chọn tổ",
});
$(".dropdown.toAllowClear").select2({
    placeholder: "Chọn tổ",
    allowClear: true
});

$(".dropdown.toes").select2({
    placeholder: "Chọn tổ",
});
$(".dropdown.nhanvien").select2({
    placeholder: "Chọn nhân viên",

});

$(".dropdown.tuyen").select2({
    placeholder: "Chọn tuyến",
});

$(".dropdown.tuyens").select2({
    placeholder: "Chọn tuyến",
});

//chức vụ
$(".dropdown.chucvu").select2({
    placeholder: "Chọn chức vụ",
    allowClear: true,

});

$(".dropdown.tuyenong").select2({
    placeholder: "Chọn tuyến ống",

});

$(".dropdown.admin").select2({
    placeholder: "Là admin",
    allowClear: true,

});

$(".dropdown.dsKhachHang").select2({
    placeholder: "Tìm khách hàng",
});

$(".dropdown.category").select2({
    placeholder: "Tìm theo module",
    allowClear: true,
});

$(".dropdown.quanTuyen").select2({
    placeholder: "Tìm theo quận huyện",
});

$(".dropdown.tuyenQuan").select2({
    placeholder: "Tìm theo tuyến",
});

//Load phòng ban theo select2
$(".dropdown.phongBan").select2({
    placeholder: "Lọc tất cả phòng ban",
    allowClear: true
});


$(".quan,.quanAllowClear").change(function () {
    //clear result trước khi thay đổi
    $(".phuong,.phuongAllowClear").find("option").remove().end();
    var selectedVal = $(this).val();
    $.ajax({
        url: '/khachhang/FillPhuong',
        method: "GET",
        data: { QuanhuyenID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {
                $(".phuong").append("<option value=" + value.PhuongXaID + ">" + (value.PhuongXaID + " - " + value.Ten) + "</option>");
                $(".phuongAllowClear").append("<option value=" + value.PhuongXaID + ">" + (value.PhuongXaID + " - " + value.Ten) + "</option>");
            });
        }
    });
});

//khi chi nhánh thay đổi thì Tổ cũng thay đổi theo
$(".quan").change(function () {
    //clear result trước khi thay đổi
    $(".to").find("option").remove().end();
    var selectedVal = $(this).val();
    $.ajax({
        url: '/khachhang/filltobyquan',
        method: "GET",
        data: { ChiNhanhID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {

                $(".to").append("<option value=" + value.ToID + ">" + (value.ToID + " - " + value.Ten) + "</option>");
            });
        }
    });
});

//khi chi nhánh thay đổi thì Tổ cũng thay đổi theo
$(".quan").change(function () {
    //clear result trước khi thay đổi
    $(".toes").find("option").remove().end();
    var selectedVal = $(this).val();
    $.ajax({
        url: '/khachhang/FillToByQuan',
        method: "GET",
        data: { ChiNhanhID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {
                $(".toes").append("<option value=" + value.ToID + ">" + (value.ToID + " - " + value.Ten) + "</option>");
            });
        }
    });
});

//chọn quận thì sẽ load danh sách tuyến ra (chỉ dành cho in hóa đơn)
$(".quanTuyen").change(function () {
    $(".tuyenQuan").find("option").remove().end();
    var selectedVal = $(this).val();

    $.ajax({
        url: "/khachhang/FillTuyenByQuan",
        method: "GET",
        data: { QuanID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {
                $(".tuyenQuan").append("<option value=" + value.TuyenKHID + ">" + (value.Ten) + "</option>");
            });
        }, error: function () {
            console.log('false');
        }
    });
});

//khi chi nhánh thay đổi thì Tổ cũng thay đổi theo
$(".chinhanh").change(function () {
    //clear result trước khi thay đổi
    $(".to").find("option").remove().end();
    $(".to").append("<option value='-1'>--Chọn một--</option>")
    var selectedVal = $(this).val();
    $.ajax({
        url: '/khachhang/FillTo',
        method: "GET",
        data: { ChiNhanhID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {
                var tenKey = value.Ten;
                $(".to").append("<option value=" + value.ToID + ">" + (value.ToID + " - " + value.Ten) + "</option>");
            });
        }
    });
});

//gửi request ajax dạng GET
//khi tổ thay đổi thì nhân viên thay đổi theo
$(".to").change(function () {
    $(".nhanvien").find("option").remove().end();
    $(".tuyen").find("option").remove().end();
    var selectedVal = $(this).val();
    var nhanVienFirst = 0;
    $.ajax({
        url: '/khachhang/FillNhanVienByTo',
        method: "GET",
        data: { ToID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {
                if (key == 0) {
                    nhanVienFirst = value.NhanvienID;
                    $.ajax({
                        url: '/khachhang/FillTuyen',
                        method: "GET",
                        data: { NhanVienID: nhanVienFirst },
                        success: function (result) {
                            $.each(result, function (key, value) {
                                $(".tuyen").append("<option value=" + value.TuyenID + ">" + (value.Matuyen + "-" + value.Ten) + "</option>");
                            });
                        }
                    });
                }
                $(".nhanvien").append("<option value=" + value.NhanvienID + ">" + (value.MaNhanVien + " - " + value.Ten) + "</option>");
                //ajax thứ 2 để load danh sách tuyến dựa theo người đầu tiên

            });
        }
    });

});

//Khi nhân viên thay đổi thì tuyến thay đổi theo
$(".nhanvien").change(function () {
    $(".tuyen").find("option").remove().end();
    var selectedVal = $(this).val();
    $.ajax({
        url: '/khachhang/FillTuyen',
        method: "GET",
        data: { NhanVienID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {
                $(".tuyen").append("<option value=" + value.TuyenID + ">" + (value.Matuyen + "-" + value.Ten) + "</option>");
            });
        }
    });
});

$(".nhanvien").change(function () {
    $(".hasDefaultVal").find("option").remove().end();
    $(".hasDefaultVal").append("<option value='-1'>--Chọn một--</option>")
});

//khi tuyến thay đổi thì danh sách khách hàng cũng được cập nhật theo
$(".tuyen").change(function () {
    $(".dsKhachHang").find("option").remove().end();
    var selectedVal = $(this).val();
    $.ajax({
        url: '/khachhang/FillNguoiDung',
        method: "GET",
        data: { TuyenID: selectedVal },
        success: function (result) {
            $.each(result, function (key, value) {
                $(".dsKhachHang").append("<option value=" + value.KhachhangID + ">" + (value.MaKhachHang + " - " + value.Ten) + "</option>");
            });
        }
    });
});

/*------------------- Function ajax để sử dụng để append dòng-------*/
function ajaxApGiaDacBietTongHopFn(khachHangID, month, year, item) {

    $.ajax({
        url: '/solieutieuthu/loadApGiaInfo',
        method: "GET",
        contentType: "application/json",
        dataType: "json",
        data: { KhachHangID: khachHangID, thang: month, nam: year },
        success: function (result) {
            var str = "";
            $.each(result, function (index, value) {
                var appendedTr = "<span>"
                  + (value.LoaiApGia == 1 ? "Sinh hoạt" : value.LoaiApGia == 2 ? "Hành chính" : value.LoaiApGia == 3 ? "Sự nghiệp" : value.LoaiApGia == 4 ? "Kinh doanh" : value.LoaiApGia == 5 ? "Sản xuất" : value.LoaiApGia == 9 ? "SH1" : value.LoaiApGia == 10 ? "SH2" : value.LoaiApGia == 11 ? "SH3" : "SH4")
                  + "  - " + value.SanLuong
                  + (value.CachTinh == 0 ? "" : value.CachTinh == 1 ? "%" : "")
                  + "</span> || ";
                str += appendedTr;
            });
            item.html(str);
        }
    });
}

/*------------------- Số liệu tiêu thụ -----------------------------*/
//nếu ấn chuột vào ô nào đó thì nhân focus vào ô đó, đặt autofocus = true
$("table tbody tr td input").click(function () {
    $('.detail-focused').removeClass('detail-focused');
    $(this).addClass('detail-focused');
    $("table tbody tr td input").attr("autofocus", false);
    $(this).attr("autofocus", true);
});

//nếu như ngày bắt đầu và ngày kết thúc để trống thì tải lại trang để cập nhật vào ô ngày bắt đầu và ngày kết thúc tương ứng của khách hàng
$(document).ready(function () {
    //xóa localStorage cho ngày bắt đầu và ngày kết thúc nếu có
    localStorage.removeItem("startDate");
    localStorage.removeItem("endDate");
    $("#endDateFixedTop").datepicker().on('changeDate', function (ev) {
        localStorage.setItem("endDate", $(this).val());
    });
});

//khi thay đổi chỉ số (lose focus) thì cập nhật trường ngày bắt đầu, ngày kết thúc, sản lượng...
$(document).ready(function () {
    $("input[name='chiSoMoi']").change(function () {
        //nếu hóa đơn đã được nhập từ tháng trước thì không cần nhập lại ngày bắt đầu nữa
        var endFixedTop = localStorage.getItem("endDate");

        var dateStart = $(this).parent('td').siblings(".startDate").find('input[name="startDate"]').val();
        var endDateValue = $(this).parent('td').siblings(".endDate").find('input[name="endDate"]').val();
        if (endDateValue == "") {
            //đặt endDate cho ngày kết thúc dựa theo ngày kết thúc trên fixed navbar
            if (endFixedTop != null) {
                var dateEndInput = $(this).parent('td').siblings(".endDate").find('input');
                var dateEndFixedTop = endFixedTop;
                dateEndInput.val(dateEndFixedTop);
                var dateEnd = dateEndInput.val();
            } else {
                alert("Ngày kết thúc không để trống");
                $(this).val("");
                $("#endDateFixedTop").focus();
                return;
            }
        } else {
            var dateEndInput = $(this).parent('td').siblings(".endDate").find('input');
            var dateEnd = dateEndInput.val();
        }

        var hoaDonID = $(this).data("hoadonid");
        var KHID = $(this).data("khid");
        var _soHoaDon = $(this).data("sohoadon");
        var dateInput = new Date();
        var chiSoMoiValue = $(this).val();
        var chiSoCuValue = $(this).parent().prev("td").find("input").val();
        //tháng và năm
        var selectedMonth = $("input[name='thang']").val();
        var selectedYear = $("input[name='nam']").val();

        var hieuSo = chiSoMoiValue - chiSoCuValue;
        //nếu dòng đó là dòng kiểm định
        var isToggledCheckbox = $(this).closest('tr').last('td').find('input[type="checkbox"]');
        if (isToggledCheckbox.is(":checked")) {
            var truocKD = isToggledCheckbox.data("truockd");
            var sauKD = isToggledCheckbox.data("saukd");
            //lấy hiệu số sau khi kiểm định
            hieuSo = (truocKD - chiSoCuValue) + (chiSoMoiValue - sauKD);
            var sanLuongValue = $(this).parent().next("td").find("input").val(hieuSo);

        } else {
            var sanLuongValue = $(this).parent().next("td").find("input").val(hieuSo);
        }

        var SoKhoan = 0;
        //console.log(JSON.stringify(sanLuongValue.));
        //gửi yêu cầu ajax: thay đổi cột số khoán, mặc định = số mới - số cũ, đẩy vào db kiêm tách số, tách số phần DB làm
        $.ajax({
            url: "/SoLieuTieuThu/NhapChiSoMoi",
            datatype: "json",
            method: "POST",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify({
                HoaDonID: hoaDonID, ChiSoDau: chiSoCuValue, ChiSoCuoi: chiSoMoiValue, TongSoTieuThu: hieuSo, SoKhoan: "0", KHID: KHID,
                SoHoaDon: _soHoaDon, dateStart: dateStart, dateEnd: dateEnd, dateInput: dateInput, thang: selectedMonth, nam: selectedYear
            }),
        });
    });
});
//khi gán số khoán

$("input[name='soKhoan']").change(function () {
    var dateStart = $(this).parent('td').siblings(".startDate").find('input').val();
    var dateEnd = $(this).parent('td').siblings(".endDate").find('input').val();
    var soKhoanInputValue = $(this).val();
    var hoaDonID = $(this).data("hoadonid");
    var KHID = $(this).data("khid");
    var soHoaDon = $(this).data("sohoadon");

    var chiSoMoiValue = $(this).parent('td').prev('td').prev('td').find('input').val();
    var chiSoCuValue = $(this).parent('td').prev('td').prev('td').prev('td').find('input').val();
    var sanLuong = 0;
    var checkboxKiemDinh = $(this).parent('td').next('td').find('input[type="checkbox"]');

    if (checkboxKiemDinh.is(':checked')) {
        var truocKiemDinh = $("input[name='kiemdinh']").data('truockd');
        var sauKiemDinh = $("input[name='kiemdinh']").data('saukd');
        var chiSoSau = $(this).parent('td').prev('td').prev('td').find('input').val();
        var chiSoTruoc = $(this).parent('td').prev('td').prev('td').prev('td').find('input').val();
        sanLuong = (truocKiemDinh - chiSoTruoc) + (chiSoSau - sauKiemDinh);
    }
    else {
        sanLuong = $(this).parent('td').prev('td').prev('td').find('input').val() - $(this).parent('td').prev('td').prev('td').prev('td').find('input').val();
    }
    var hieuSo = parseInt(sanLuong) + parseInt(soKhoanInputValue);

    //gửi yêu cầu ajax: thay đổi cột số khoán, mặc định = số mới - số cũ, đẩy vào db kiêm tách số, tách số phần DB làm
    $.ajax({
        url: "/SoLieuTieuThu/ChinhSuaSoKhoan",
        datatype: "json",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ HoaDonID: hoaDonID, ChiSoDau: chiSoCuValue, ChiSoCuoi: chiSoMoiValue, TongSoTieuThu: hieuSo, SoKhoan: soKhoanInputValue, KHID: KHID, dateStart: dateStart, dateEnd: dateEnd, sohoadon: soHoaDon }),
    });
});

//chỉnh sửa lên xuống phải trái, bằng cách ấn enter (focus vào ô đó)
$(document).on('keyup', function (e) {
    var cachTinhGia = $(".cachTinhGia");
    cachTinhGia.text("");
    var focusedElement = $("input[autofocus='autofocus']");
    if (focusedElement.length > 1) {
        focusedElement = $(".detail-focused");
    }
    //lấy vị trí cột dựa theo index
    var index = focusedElement.closest("td").index();
    //nếu nút ấn là nút Enter
    var ENTERKEYCODE = 13;
    var UPARROW = 38;
    var DOWNARROW = 40;

    if (e.which == ENTERKEYCODE || e.which == DOWNARROW) {

        if (focusedElement.hasClass('sanLuong')) {
            var nextParent = focusedElement.parent().parent().next("tr.traverse");
            var nextElement = nextParent.find("td:eq(" + index + ")").find("input");
            focusedElement.blur();
            nextElement.attr("autofocus", true).focus();
            nextElement.addClass('detail-focused');
            focusedElement.removeClass('detail-focused');
            //nếu là áp giá tổng hợp / áp giá đặc biệt thì load 2 dòng cách tính ra, nếu không thì không load và chuyển tới element tiếp theo
            if (nextParent.hasClass('7') || nextParent.hasClass('8')) {
                var khachHangID = $(".detail-focused").data('khachhangid');
                var month = $(".detail-focused").data('month');
                var year = $(".detail-focused").data('year');
                ajaxApGiaDacBietTongHopFn(khachHangID, month, year, cachTinhGia);
            }
        }
        else {
            var nextParent = focusedElement.parents("tr").next("tr");
            var nextElement = nextParent.find("td:eq(" + index + ")").find("input");
            focusedElement.blur();
            nextElement.attr("autofocus", true).focus();
            nextElement.addClass('detail-focused');
            focusedElement.removeClass('detail-focused');
        }
    }

    else if (e.which === UPARROW) {
        if (focusedElement.hasClass('sanLuong')) {

            //traverse up
            var prevParent = focusedElement.parent().parent().prev("tr.traverse");
            var prevElement = prevParent.find("td:eq(" + index + ")").find("input");

            focusedElement.blur();
            prevElement.attr("autofocus", true).focus();
            focusedElement.removeClass('detail-focused');
            prevElement.addClass('detail-focused');
            //nếu là áp giá tổng hợp / áp giá đặc biệt thì load 2 dòng cách tính ra, nếu không thì không load và chuyển tới element tiếp theo
            if (prevParent.hasClass('7') || prevParent.hasClass('8')) {
                var khachHangID = $(".detail-focused").data('khachhangid');
                var month = $(".detail-focused").data('month');
                var year = $(".detail-focused").data('year');
                ajaxApGiaDacBietTongHopFn(khachHangID, month, year, cachTinhGia);
            }
        } else {
            var prevParent = focusedElement.parents("tr").prev("tr");
            var prevElement = prevParent.find("td:eq(" + index + ")").find("input");
            focusedElement.blur();
            prevElement.attr("autofocus", true).focus();
            focusedElement.removeClass('detail-focused');
            prevElement.addClass('detail-focused');
        }
    }
});

//chọn 1 ô và cho class focus vào đó
$(".sanLuong").on('click', function () {
    var cachTinhGia = $(".cachTinhGia");
    //clear class detail-focused đi
    var focusedElement = $(".detail-focused");
    focusedElement.removeClass("detail-focused");
    $(this).addClass("detail-focused");
    var khachHangID = $(this).data('khachhangid');
    var month = $(this).data('month');
    var year = $(this).data('year');
    //loadAjax
    ajaxApGiaDacBietTongHopFn(khachHangID, month, year, cachTinhGia);
});

//thay đổi thứ tự đọc và đẩy thẳng vào 
$("input[name='thuTuDoc']").change(function () {
    var thuTuDoc = $(this);
    var thuTuDocValue = $(this).val();
    var hoaDonID = $(this).data('hoadonid');
    $.ajax({
        url: "/SoLieuTieuThu/ThayDoiThuTu",
        contentType: "application/json",
        datatype: "json",
        method: "POST",
        data: JSON.stringify({ HoaDonID: hoaDonID, ThuTuDoc: thuTuDocValue }),
        success: function (result) {
            $.each(result, function (key, value) {
                $("input[name='thuTuDoc'][data-hoadonid=" + value._HoaDonID + "]").val(value.ThutuDoc)
            });

        }
    });
});

//Nếu startDate & endDate thay đổi thì cập nhật vào db tương ứng
$(document).ready(function () {
    $("input[name='startDate']").on('change', function () {
        var hoaDonID = $(this).data('hoadonid');
        var newVal = $(this).val();
        $.ajax({
            url: "/SoLieuTieuThu/capnhatbatdau",
            dataType: "json",
            method: "POST",
            contentType: "application/json",
            data: { HoaDonID: hoaDonID, ngayBatDau: newVal }
        });
    });
});

$(document).ready(function () {
    $("input[name='endDate']").change(function () {
        var hoaDonID = $(this).data('hoadonid');
        var khID = $(this).data("khid");
        var newVal = $(this).val();
        var month = parseInt($("input[name='thang']").val());
        var year = parseInt($("input[name='nam']").val());
        var nextMonth = parseInt(month) + 1 > 12 ? 1 : parseInt(month) + 1;
        var nextYear = parseInt(month) + 1 > 12 ? parseInt(year) + 1 : parseInt(year);

        $.ajax({
            url: "/SoLieuTieuThu/capnhatketthuc",
            dataType: "json",
            method: "GET",
            contentType: "application/json",
            data: { HoaDonID: hoaDonID, KhachHangID: khID, ngayKetThuc: newVal, thangNay: month, thangSau: nextMonth, namNay: year, namSau: nextYear },
            success: function (result) {
                console.log('123');
            },
            error: function (result) {
                console.log('222');
            }
        });
    });
});
/*-----------------------------------Số liệu tiêu thụ------------------------------------*/

$("button.xemChiTietKD").click(function () {
    var khID = $(this).data("khid");
    var month = $(this).data("month");
    var year = $(this).data("year");
    var trRow = $(this).closest('tr');

    //gửi request ajax về    
    if (!$(this).hasClass('toggled')) {
        $(this).addClass('toggled');
        $.ajax({
            url: '/SoLieuTieuThu/showDetail',
            method: "GET",
            data: { KhachHangID: khID, month: month, year: year },
            dataType: 'json',
            contentType: 'application/json',
            success: function (result) {
                $.each(result, function (key, value) {
                    //  console.log(value.NgayKiemDinh);
                    var appendedTr = "<tr><td colspan='18'><ul><li style='line-height:20px'><strong>Ngày kiểm định</strong>: "
                   + (value.NgayKiemDinh + "/" + value.ThangKiemDinh + "/" + value.NamKiemDinh)
                   + "<li><strong>Chỉ số trước khi kiểm định: </strong>" + value.ChiSoLucKiemDinh
                   + "<li><strong>Chỉ số sau khi kiểm định: </strong>" + value.ChiSoSauKiemDinh
                   + "</ul></td>";
                    + "</tr>";
                    //append sau trRow
                    trRow.after(appendedTr);
                });
            },
        });
    }
        //nếu đã có class toggled thì tìm đến <tr> gần nhât, tìm <tr> tiếp theo và xóa class đó đi
    else {
        $(this).closest('tr').next().remove();
        $(this).removeClass('toggled');
    }


});

/*-----------Datepicker class --------------*/

$(function () {
    $(".datepicker").datepicker({
        todayBtn: "linked",
        format: 'dd/mm/yyyy',
    });
    //chọn datepicker lớn hơn ngày cho sẵn   
    $(".apdinh").change(function () {
        $(".hetdinh").datepicker('setStartDate', $(".apdinh").val());
    });

});
/*----------------------Hiển thị datepicker cho ngày bắt đầu và ngày kết thúc (bên cạnh thông tin tuyên-người dùng) trong phần nhập số liệu tiêu thụ----------------*/

$("#startDateFixedTop").prop('disabled', true);

$("#endDateFixedTop").datepicker({
    todayBtn: "linked",
    format: 'dd/mm/yyyy',
});

$("input[name='startDate']").prop('disabled', true);

$("input[name='endDate']").datepicker({
    todayBtn: "linked",
    format: 'dd/mm/yyyy',
});

$(".disabledEndDate").prop('disabled', true);


/*----Xóa phần chi tiết chia áp giá khi click vào body--------*/
$(function () {
    $("body").click(function () {
        $(".cachTinhGia").html("");
    });
});

/*--- Nhập chỉ số tuyến ống ----- */
$("input[name='sanLuongTuyenOng']").blur(function () {
    var selectedTuyenOng = $(this);
    var tuyenongId = $(this).data('tuyenongid');
    var chiSoSanLuong = selectedTuyenOng.val();
    var thangCoChiSo = $("input[name='thangcs']").val();
    var namCoChiSo = $("input[name='namcs']").val();
    $.ajax({
        url: '/tuyenong/nhapsanluong',
        method: 'POST',
        data: JSON.stringify({ TuyenOngID: tuyenongId, SanLuong: chiSoSanLuong, thang: thangCoChiSo, nam: namCoChiSo }),
        contentType:'application/json',
    })
});