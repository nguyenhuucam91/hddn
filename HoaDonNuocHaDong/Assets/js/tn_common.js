$(document).ready(function () {
    // select2 items
    $(".select2").select2();
    $(".datepicker").datepicker({
        format: 'mm/dd/yyyy',
        onSelect: function () {
            $(this).change();
        }
    });

    $(".data-table").dataTable({
        fixedHeader: {
            header: true,
            footer: true
        },
        "autoWidth": false,
        responsive: false,
        "language": {
            "emptyTable": "Không có dữ liệu trong bảng"
        },
        //"bSort": false,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bInfo": false,
        "bAutoWidth": false
    });
    $('body').tooltip({
        selector: '[data-toggle="tooltip"]'
    });
});

/**
 * load list quanHuyen with selected quanHuyenID
 * #modifies $slQuanHuyen
 * #effects
 *  clear slQuanHuyen
 *  ajax load quanHuyen
 *      update $slQuanHuyen with loaded values
 *  if quanHuyenID not specified
 *      first quanHuyen will be selected
 * #returns selected quanHuyenID
 */
function loadQuanHuyen($slQuanHuyen, quanHuyenID) {
    BUtils.slRemoveAllOptions($slQuanHuyen);

    $.ajax({
        url: "/Services/QuanHuyen/GetAll"
    }).done(function (data) {
        if (data.IsSuccess) {
            var items = [];
            $.each(data.Data, function (index, item) {
                items.push({ "id": item.QuanhuyenID, "text": item.QuanhuyenID + " - " + item.Ten });
            });

            $slQuanHuyen.select2({ data: items });
            //if (quanHuyenID != "")
            $slQuanHuyen.val(quanHuyenID);
            $slQuanHuyen.change();
        }
    });
}

/**
 * #see: loadQuanHuyen()
 */
function loadTo($slTo, quanHuyenID, toID) {
    BUtils.slRemoveAllOptions($slTo);

    if (quanHuyenID == null) {
        $slTo.change();
        return false;
    }

    $.ajax({
        url: "/Services/To/GetByQuanHuyenID",
        data: { quanHuyenID: quanHuyenID }
    }).done(function (data) {
        if (data.IsSuccess) {
            var items = [];
            $.each(data.Data, function (index, item) {
                items.push({ "id": item.ToQuanHuyenID, "text": item.ToQuanHuyenID + " - " + item.Ma });
            });

            $slTo.select2({ data: items });
            //if (toID != "")
            $slTo.val(toID);
            $slTo.change();
        }
    });
}

/**
 * #see: loadQuanHuyen()
 */
function loadNhanVien($slNhanVien, toID, nhanvienID) {
    BUtils.slRemoveAllOptions($slNhanVien);

    if (toID == null) {
        $slNhanVien.change();
        return false;
    }

    $.ajax({
        url: "/Services/NhanVien/GetByToID",
        data: { toID: toID }
    }).done(function (data) {
        if (data.IsSuccess) {
            var items = [];
            $.each(data.Data, function (index, item) {
                items.push({ "id": item.NhanvienID, "text": item.MaNhanVien + " - " + item.Ten });
            });

            $slNhanVien.select2({ data: items });
            //if (nhanvienID != "")
            $slNhanVien.val(nhanvienID);
            $slNhanVien.change();
        }
    });
}

/**
 * #see: loadQuanHuyen()
 */
function loadTuyen($slTuyen, nhanVienID, tuyenID) {
    BUtils.slRemoveAllOptions($slTuyen);

    if (nhanVienID == null) {
        $slTuyen.change();
        return false;
    }

    $.ajax({
        url: "/Services/Tuyen/GetByNhanVienID",
        data: { nhanVienID: nhanVienID }
    }).done(function (data) {
        if (data.IsSuccess) {
            var items = [];
            $.each(data.Data, function (index, item) {
                items.push({ "id": item.TuyenKHID, "text": item.MaTuyen + " - " + item.Ten });
            });

            $slTuyen.select2({ data: items });
            //if (tuyenID != "")
            $slTuyen.val(tuyenID);
            $slTuyen.change();
        }
    });
}

/***** Thanh Toan HD *****/
/**
 * update HoaDon status against database & get back updated model
 * update GUI trangThaiThu, ngayThu, soTienDaNop
 */
function capNhatThanhToan(hoaDonID, $trangThaiThu, $ngayThu, $soTienDaNop, $soTienConThieu) {
    var trangThaiThu = $trangThaiThu.is(":checked"),
        ngayThu = $ngayThu.val();

    // waiting
    $trangThaiThu.attr("disabled", true);
    $trangThaiThu.after('<div class="progress progress-striped active"><div class="bar" style="width: 100%;"></div></div>');

    // processing
    if (trangThaiThu == true) {
        var url = "/Services/HoaDon/ThanhToan",
            data = { hoaDonID: hoaDonID, ngayThu: ngayThu };
    } else {
        var url = "/Services/HoaDon/HuyThanhToan",
            data = { hoaDonID: hoaDonID };
    }

    BUtils.ajax(url, data,
        function (data) { // success
            var model = data.Data;
            $trangThaiThu.prop("checked", model.HoaDon.TrangThaiThu);
            $ngayThu.val(model.HoaDon.NgayNopTien);
            $soTienDaNop.html(model.SoTienNopTheoThang.SoTienDaThu);
            $soTienConThieu.html(model.SoTienNopTheoThang.DuNo);
            // set màu cho duNo/ duCo
            if (!model.HoaDon.TrangThaiThu) {
                $soTienConThieu.removeClass("text-success");
                $soTienConThieu.addClass("text-error");
                $soTienDaNop.removeClass("text-success");
            } else {
                $soTienConThieu.removeClass("text-error");
                $soTienConThieu.addClass("text-success");
                $soTienDaNop.addClass("text-success");
            }

            $trangThaiThu.after('<div class="progress progress-success"><div class="bar" style="width: 100%;"></div></div>');
        },
        function (data) { // error
            $trangThaiThu.prop("checked", !trangThaiThu); // back state
            $trangThaiThu.after('<div><a href="#" class="text-error" data-toggle="tooltip" title="' + data.Message + '"><i class="icon-info-sign"></i></a></div>');
        });

    // done
    $trangThaiThu.attr("disabled", false);
    $trangThaiThu.siblings(".progress-striped").remove();
}

function capNhatNgayThu($ngayThu, hoaDonID, ngayThu) {
    // waiting
    $ngayThu.attr("disabled", true);
    $ngayThu.after('<div class="progress progress-striped active"><div class="bar" style="width: 100%;"></div></div>');

    // processing
    var url = "/Services/HoaDon/CapNhatNgayThu",
        data = { hoaDonID: hoaDonID, ngayThu: ngayThu};

    BUtils.ajax(url, data,
        function (data) { // success
            $ngayThu.after('<div class="progress progress-success"><div class="bar" style="width: 100%;"></div></div>');
        },
        function (data) { // error
            var model = data.Data;
            $ngayThu.val(model.HoaDon.NgayNopTien); // back state
            $ngayThu.after('<div><a href="#" class="text-error" data-toggle="tooltip" title="' + data.Message + '"><i class="icon-info-sign"></i></a></div>');
        });

    // done
    $ngayThu.attr("disabled", false);
    $ngayThu.siblings(".progress-striped").remove();
}

function huyGiaoDich($lnkHuyGiaoDich, khachHangID, giaoDichID) {
    // waiting
    $lnkHuyGiaoDich.hide();
    $lnkHuyGiaoDich.after('<div class="progress progress-striped active"><div class="bar" style="width: 100%;"></div></div>');

    // processing
    var url = "/Services/GiaoDich/HuyGiaoDich",
        data = { khachHangID: khachHangID, giaoDichID: giaoDichID};

    BUtils.ajax(url, data,
        function (data) { // success
            $lnkHuyGiaoDich.after('<div class="progress progress-success"><div class="bar" style="width: 100%;"></div></div>');
        },
        function (data) { // error
            var model = data.Data;
            $lnkHuyGiaoDich.show(); // back state
            $lnkHuyGiaoDich.after('<div><a href="#" class="text-error" data-toggle="tooltip" title="' + data.Message + '"><i class="icon-info-sign"></i></a></div>');
        });

    // done
    $lnkHuyGiaoDich.siblings(".progress-striped").remove();
}
/***** END Thanh Toan HD *****/