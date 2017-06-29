//thay đổi trạng thái thu và số tiền đã nộp khi người dùng chính sửa số tiền đã thu
// ở trang lịch sử sử dụng nước của khách hàng  
$("input[name='tien']").focusout(function () {
    actioUrl = "/congno/ChangeTrangthai";
    var tien = $(this).val();
    var status;
    if (parseInt(tien) == 0) {
        status = 'false';
    }
    else {
        status = "true";
    }
    var tienoptheothangID = $(this).attr("id");
    var hoadonID = $(this).attr("data-hoadon");
   // alert(tien + " id:" + tienoptheothangID + " hoadon" + hoadonID);
    if (tienoptheothangID != "undefined" && tienoptheothangID!=null) {
        link = "/Congno/NopTien";
        $.ajax({
            url: actioUrl,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ HoadonnuocID: hoadonID, printStatus: status }),
            success: function (result) { //sau khi hoàn thành thay tự động update số tiền đã thu = số tiền phải nộp
                $("." + hoadonID).prop('checked', true);
            },
            error: function (result) {
                alert("fail");
            }
        });

        $.ajax({
            url: link,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ TienNopTheoThangID: tienoptheothangID, soTien: tien }),
            success: function (result) {
                //alert("success");             
            }
        });
    }
    else {
        $(this).attr("placeholder", "Hóa dơn tháng này chưa được in");
    }
    
});

//thay đổi ngày nộp tiền khi ng dùng thay đổi ngày nộ trong
//trang lịch sử sử dụng nước của khách hàng
$("input[name='ngaynop']").change(function () {
    var ngaynop = ($(this).val());
    var date = new Date(ngaynop);
    id = $(this).attr("id");
   
    if (ngaynop.length > 0) {
        month = date.getMonth() + 1;
        
        if (month < 10) {
            thang = "0" + month.toString();
        }
        else {
            thang = month.toString();
        }
        day = date.getDate();
        var ngay = day.toString();
        if (day < 10) {
            ngay = "0" + day.toString();
        }
        ngaynop = ngay + "/" + thang + '/' + date.getFullYear();
        $.ajax({
            url: "/congno/ChangeNgayNop",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ HoadonID: id, ngaynop: ngaynop }),
            success: function (result) {
                //alert("success");             
            },
            error: function (result) {
                alert("fail");
            }
        });
    }
    else
        alert("Bạn chưa chọn ngày nộp");
});

$("#excel").click(function () {
    KHID = $(this).attr("data-KHID");
    name = $(this).attr("data-name");
    nam = $("#nam").html();
    alert("vui lòng đợi giây lát file excel sẽ được mở");
    $.ajax({
        url: "/BaoCao/Report",
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: 'text',
        data: JSON.stringify({ KHID: KHID, name:name, nam:nam }),
        success: function (result) {
           // alert("success");             
        },
        error: function (result) {
           // alert("fail");
        }
    });
});

var  actioUrl = "/congno/ChangeTrangthai";

//hàm thay đổi trạng thái thu, sau khi thay đổi checkbox, số tiền đã thu cũng dc tự động update
$("input[type='checkbox']").not($('#checkAll')).change(function () {

    var link = "/Congno/NopTien";
    var today = new Date();
    var dd = today.getDate();
    if (dd < 10) {
        dd = "0" + dd.toString();
    }
    var mm = today.getMonth()+1; //January is 0!
    var yyyy = today.getFullYear();
    var day = yyyy + "-" + mm + "-" + dd;
    
    //alert(today);
    //nếu checkbox trong công nợ được checked
    if ($(this).is(':checked')) {
        var classValue = $(this).prop("class");
        //alert(classValue);
        $.ajax({
            url: actioUrl,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ HoadonnuocID: classValue, printStatus: 'true' }),
            success: function (result) { //sau khi hoàn thành thay tự động update số tiền đã thu = số tiền phải nộp
                var arr = result.split(" ");
                var a = $("input[id=" + arr[1] + "]").val(arr[0]);
                noptien(arr[1], arr[0], link);
                $("#" + classValue).val( day);

            },
            error: function (result) {
                console.log('123');
            }
        });
    }
        //nếu hủy check sẽ tiến hành gửi 1 POST request về congno/ChangeTrangthai 
    else {
        var classValue = $(this).prop("class");
        $.ajax({
            url: actioUrl,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ HoadonnuocID: classValue, printStatus: 'false' }),
            success: function (result) {//sau khi hoàn thành lập tức cho số tiền đã nộp về 0
                var arr = result.split(" ");
                var a = $("input[id='" + arr[1] + "']").val("0");
                noptien(arr[1], 0, link);
                $("#" + classValue).val(null);
            },
            error: function () {
                console.log("aaaaa");
            }
        })
    }
});

//function nhận tham số để thực hiện tính năng thay đổi số tiền đã thu
function noptien(pid, ptien, link) {

    var id = pid;
    var tienNop = ptien;
    $.ajax({
        url: link,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: 'text',
        data: JSON.stringify({ TienNopTheoThangID: pid, soTien: ptien }),
        success: function (result) {
            //alert("success");             
        }
    });
}
