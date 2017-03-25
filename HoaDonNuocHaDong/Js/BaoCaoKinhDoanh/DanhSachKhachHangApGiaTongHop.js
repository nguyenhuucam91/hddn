
$(document).ready(function () {
    
    var table = $('.processedInServer').DataTable({        
        dom: 'Bfrtip',
        ajax: {
            url: '/Baocaokinhdoanh/jsonTransformKhachHangApGiaTongHop',
            type: 'POST',
            dataSrc: "",            
        },
        processing:true,
        serverSide: true,
        columns: [
            { data: "STT" },
            { data: "MaKH" },
            { data: "HoTen" },
            { data: "DiaChi" },
            { data: "Tuyen" },
            { data: "TTDoc" },
            { data: "CachTinh" },
            { data: "SinhHoat" },
            { data: "SanXuat" },
            { data: "HanhChinh" },
            { data: "CongCong" },
            { data: "KinhDoanh" },
        ],
        bFilter: false,       
        bSort: false,
        bInfo: false,
        paging:false,
        language: {
            "emptyTable": "Không có dữ liệu"
        },
        buttons: [
            'excelHtml5',
        ],
    });

    //Đổi chữ trong tất cả các báo cáo từ Excel => xuất Excel
    $(".buttons-excel").text("Xuất Excel");
 
});