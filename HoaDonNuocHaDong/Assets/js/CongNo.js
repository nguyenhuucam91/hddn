
$(document).ready(function () {
  
    //đẩy nhân viên id vào form trong trang reportlist để gửi đến server 
    $("input[type='submit']").click(function () {
        var formAction = $(this).parent().attr("action");
        var nhanvien = $("#nhanvien").val();
        $("input[name='nhanvienid']").val(nhanvien);

    });


    var view = $("#viewname").html().trim();
    var actioUrl = "/congno/ChangeTrangthai";
    //kiểm tra action name, nếu action = index thì load index
    if (view == "Index")
        actioUrl = "/congno/ChangeTrangthai";
    else if (view == "congNoDoanhNghiep")
        actioUrl = "ChangeTrangthai";
    else if (view == "congNoChuyenKhoan")
        actioUrl = "ChangeTrangthai";
    //alert(view);
    //if (view == "baoCaoCongNo" || view == "BaoCaoDuNo" || view=="DanhSachKhachHangInHoaDon") {
    //    $(".footer").append("<script src='/Assets/BaoCaoJs/dataTables.buttons.min.js'></script>");
    //    $(".footer").append("<script src='/Assets/BaoCaoJs/buttons.html5.min.js'></script>");
    //    $(".footer").append("<script src='/Assets/BaoCaoJs/jszip.min.js");
    //}
   // $(".dataTables_length label").remove();

    var count = $('#checkAll').closest('form').find(':checkbox').not(":checked").not($('#checkAll')).length;

    if (count > 0) {
        $('#checkAll').prop('checked', false);
    }
    else {
        $('#checkAll').prop('checked', true);
    }

    


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

    //nếu nhấn check all, tất cả các checkbox đều dc check
    //lập tức thay đổi số tiền đã tuh bằng số tiền phải nộp
    $('#checkAll').click(function () {
        alert("vui lòng đợi giây lát đến khi yêu cầu được xử lý hoàn tất");
        var checkboxes = $(this).closest('form').find(':checkbox').not($('#checkAll')).not(':checked');
        var link;
        if (view == "Index")
            link = "Congno/NopTien";
        else
            link = "NopTien";
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        var day = dd + "/" + mm + "/" + yyyy;
        if ($(this).is(':checked')) {
            checkboxes.prop('checked', true);
            $.each(checkboxes, function () {
                var classValue = $(this).prop("class");
                $.ajax({
                    url: actioUrl,
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'text',
                    data: JSON.stringify({ HoadonnuocID: classValue, printStatus: 'true' }),
                    //result là 1 chuỗi bao gồm tiennoptheothangid và sotienphainop ngăn cách bằng ký tự space
                    success: function (result) {//tự động chuyển số tiền đã thu = số tiền phải nộp
                        var arr = result.split(" ");
                        var a = $("input[data-id=" + arr[1] + "]").val(arr[0]);
                        noptien(arr[1], arr[0], link);
                        //thay đổi ngày thu -> ngày hiện tại
                        $("#" + classValue).val(day);
                    }
                });

            });
        } else {
            var ck = $(this).closest('form').find(':checkbox').not($('#checkAll'));
            ck.prop('checked', false);
            $.each(ck, function () {
                var classValue = $(this).prop("class");

                $.ajax({
                    url: actioUrl,
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'text',
                    data: JSON.stringify({ HoadonnuocID: classValue, printStatus: 'false' }),
                    success: function (result) {//tự động cho số tiền đã thu về 0
                        var a = $("input[data-id]").val(0);
                        var arr = result.split(" ");
                        $("#" + classValue).val("");
                        noptien(arr[1], 0, link);

                    }
                });
            });
        }
    });

    //hàm thay đổi trạng thái thu, sau khi thay đổi checkbox, số tiền đã thu cũng dc tự động update
    $("input[type='checkbox']").not($('#checkAll')).change(function () {

        var link;
        if (view == "Index")
            link = "Congno/NopTien";
        else
            link = "NopTien";
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        var day = dd + "/" + mm + "/" + yyyy;
        //nếu checkbox trong công nợ được checked
        if (view != "XemChiTiet") {
            if ($(this).is(':checked')) {
                var classValue = $(this).prop("class");
                $.ajax({
                    url: actioUrl,
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'text',
                    data: JSON.stringify({ HoadonnuocID: classValue, printStatus: 'true' }),
                    //result là 1 chuỗi bao gồm tiennoptheothangid và sotienphainop ngăn cách bằng ký tự space
                    success: function (result) { //sau khi hoàn thành thay tự động update số tiền đã thu = số tiền phải nộp
                        var arr = result.split(" ");
                        var a = $("input[data-id='" + arr[1] + "']").val(arr[0]);
                        noptien(arr[1], arr[0], link);
                        //alert(day);
                        $("#" + classValue).val(day);
                        //thay đổi số tiền đã thu thành số tiền phải thu
                        $("." + classValue).html(arr[0]);

                        //alert(classValue +" "+ dutruoc);
                    },

                });
            }
                //nếu hủy check sẽ tiến hành gửi 1 POST request về congno/index 
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
                        var a = $("input[data-id='" + arr[1] + "']").val("0");
                        noptien(arr[1], 0, link);
                        $("#" + classValue).val("");
                        $("." + classValue).html("0");
                        var tien = parseInt(arr[0]);
                    },

                })
            }
        }
    });


    //lưu số tiền đã thu vào db
    //sau khi ng dùng nhập số tiền đã thu vào form, đẩy dữ liệu bằng ajax đén controller
    $(document).on("focusout", "input[name='SoTienThu']", function () {
        var link;
        if (view == "Index")
            link = "Congno/NopTien";
        else
            link = "NopTien";

        var id = $(this).attr("data-id");

        var tienNop = $(this).val();
        noptien(id, tienNop, link);

    });

    //lưu ghi chú cho hóa đơn
    //sau khi ng dùng nhập ghi chú và chuyển sang ô khác, đưa dữ liệu đến controller bằng ajax 
    $("input[name='GhiChu']").focusout(function () {
        var id = $(this).attr("data-hid");
        var ghiChu = $(this).val();
        var link;
        if (view == "Index")
            link = "Congno/GhiChu";
        else
            link = "GhiChu";
        $.ajax({
            url: link,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ HoaDonID: id, ghiChu: ghiChu }),
            //success: function (result) {
            //},
            //error: function (xhr, ajaxOptions, err) {
            //    alert(xhr.status);
            //    alert(xhr.responseText);
            //    alert(err.toString());

        });
    });


    //thay đổi ngày nộp tiền khi ng dùng thay đổi ngày nộp 
   // $(".datepicker").datepicker('setDate', new Date());
    $(".ngaynoptien").datepicker({
        dateFormat: "dd/MM/yyyy",
        onSelect: function () {
            
            $(this).change(function () {
            });
        }
    }).on("change", function () {
        var ngaynop = ($(this).val());
        var from = ngaynop.split("/");
        id = $(this).attr("id");
        var date = new Date(ngaynop);

        //alert(ngaynop);
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
        //else
        //    alert("Bạn chưa chọn ngày nộp");
    });
    
    //đánh dấu tất că khách hàng trong quận là đã nộp tiền, gửi requset đến server, hàm CheckAll
    $("#chooseAll").click(function () {
        if ($(this).attr("data-status") == 0) {
            alert("Tất cả khách hàng đã được đánh dấu đã chọn");
        }
        else {
            alert("Bạn đã chọn đánh dấu tất cả khách hàng trong tháng là đã nộp tiền");
            var conf;
            conf = confirm("Bạn có chắc chắn muốn đánh dấu tất cả là đã nộp");
            if (conf == true) {
                $.ajax({
                    url: "/congno/CheckAll",
                    type: 'POST',
                    success: function (result) {
                        alert("đã đánh dấu tất cả khách hàng trong chi nhánh là đã nộp tiền ");
                    },
                    error: function (xhr, ajaxOptions, err) {
                        alert(xhr.status);
                        alert(xhr.responseText);
                        alert(err.toString());
                    }
                });
            }
        }
    });

    //hiển thị hộp thoại chứa form thêm giao dịch khi click vào nút có hình dấu cộng
    // khi click vào nút hình dấu cộng ở ngoài màn hình chính gửi ajax requet để nhận về list giao dịch của khách hàng
    //thông tin về khách hàng lưu ở các attribute data-
    $(".passGiaoDich").click(function () {
        var rr = "";
        var khID = $(this).attr("data-khachhang");
        var TienNopTheoThangID = $(this).attr("data-tiennoptheothang");
        var hdid = $(this).attr("data-hdID");
        $("#tiennoptheothangID").val(TienNopTheoThangID);
        $.ajax({
            url: "/congno/getGiaoDich",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ khID: khID }),
            success: function (result) {
                var obj = JSON.parse(result);
                $.each(obj, function (key, value) {
                    var du = "0";
                    if (value.du != null) {
                        du = value.du;
                    }
                    if (key == 0) {
                        rr = rr + "<h4 class='span2'>" + value.ngay + "</h4>" + "Nộp:  <input type='text' name='changegiaodich',' value='"+value.sotien+"' size=8 class='span1' id='"+TienNopTheoThangID+"' data-id='"+hdid+"' /> <div class='span2' style='float:right'>Số Dư:" + du + "</div> <br>";
                    }
                    else {
                        rr = rr + "<h4 class='span2'>" + value.ngay + "</h4>" + "Nộp:" + value.sotien + "<div class='span2' style='float:right'>Số Dư:" + du + "</div> <br>";
                    }
                });
                $("#ls").html(rr);
               
            },
            error: function (xhr, ajaxOptions, err) {
                alert(xhr.status);
                alert(xhr.responseText);
                alert(err.toString());
            }
        });
        
    });
   
    //gửi request đến controller khi click vào nút gửi ở form thêm giao dịch
    $(".modal-body").find("button").click(function () {
        var check = 1;
        $(".modal-body").find("input").each(function () {
            if ($(this).val() === "")
                check = 0;
        });
        if (check == 0) {
            alert("Không được bỏ trống các ô nhập");
        }
        else {
            alert("khách hàng đóng thêm số tiền:" + $("input[name='sotien']").val());
            var sotien = $("input[name='sotien']").val();
            var tiennoptheothangID = $("input[name='tiennoptheothangID']").val();
            var ngaynop = $("input[name='ngaynop']").val();
            $.ajax({
                url: "/congno/addGiaoDich",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                dataType: 'text',
                data: JSON.stringify({ tiennoptheothangID: tiennoptheothangID, sotien: sotien, ngaynop: ngaynop }),
                success: function (result) {
                    location.reload();
                },
                error: function (xhr, ajaxOptions, err) {
                    //alert(xhr.status);
                    //alert(xhr.responseText);
                    //alert(err.toString());
                }
            });
            
        }
    });

    //thay đổi ô tiền đã nộp trong dialog thêm giao dịch
    //khi focusout, lấy số tiền đã nhập và tiền nộp theo tháng id
    $(document).on("focusout", "input[name='changegiaodich']", function () {
        var link;
        if (view == "Index")
            link = "Congno/NopTien";
        else
            link = "NopTien";

        var id = $("input[name='changegiaodich']").attr("id");

        var tienNop = $("input[name='changegiaodich']").val();
        $.ajax({
            url: "/congno/changeGiaDich",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: 'text',
            data: JSON.stringify({ tiennoptheothangID : id, sotien : tienNop }),
            success: function (result) {
                location.reload();
            },
            error: function (xhr, ajaxOptions, err) {
                alert(xhr.status);
                alert(xhr.responseText);
                alert(err.toString());
            }
        });
    });
    
});



