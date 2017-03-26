$(document).ready(function () {
    // select2 items
    $(".select2").select2();

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

function getQuanHuyenByID($slQuanHuyen, quanHuyenID) {
    BUtils.slRemoveAllOptions($slQuanHuyen);

    $.ajax({
        url: "/Services/QuanHuyen/GetByID"
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
function queryTo($slTo, quanHuyenID, phongBanId, toID) {
    BUtils.slRemoveAllOptions($slTo);

    if (quanHuyenID == null) {
        $slTo.change();
        return false;
    }

    $.ajax({
        url: "/Services/To/Query",
        data: { quanHuyenID: quanHuyenID, phongBanID: phongBanId }
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
            console.log(items);
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