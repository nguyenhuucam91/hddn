$(document).ready(function () {
    $(".chiso").click(function () {
        var sl = $("."+$(this).attr("id")).val();
        if (sl.match(/^\d+$/)) {
            var id = $(this).attr("id");
            var thang = $("input[name='thangcs']").val();
            var nam = $("input[name='namcs']").val();
            $.ajax({
                url: "/Tuyenong/NhapSanLuong",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                dataType: 'text',
                data: JSON.stringify({ TuyenOngID: id, SanLuong: sl, thang: thang, nam: nam }),
                success: function (result) {
                    alert("done");
                },
                error: function (xhr, ajaxOptions, err) {
                    alert(xhr.status);
                    alert(xhr.responseText);
                    alert(err.toString());
                }
            });
        }
        else {
            alert("char");
        }
    });
});