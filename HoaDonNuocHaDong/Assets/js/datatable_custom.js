$(document).ready(function () {

    $('.reset-default-datatable').DataTable({
        /*"dom": 'lBfrtip',
        "buttons": [
            {
                extend: 'collection',
                text: 'Export',
                buttons: [
                    'copy',
                    'excel',
                    'csv',
                    'pdf',
                    'print'
                ]
            }
        ],   */
        "language": {
            
            "info": "Hiển thị trang _PAGE_ / _PAGES_",
            "infoEmpty": "Không có bản ghi",
            "search": "Tìm kiếm",            
            "emptyTable": "Không có dữ liệu trong bảng"
        },        

    });

    $('.example').DataTable({
        /*"dom": 'lBfrtip',
        "buttons": [
            {
                extend: 'collection',
                text: 'Export',
                buttons: [
                    'copy',
                    'excel',
                    'csv',
                    'pdf',
                    'print'
                ]
            }
        ],   */     
        "language": {
            "lengthMenu": "Hiển thị _MENU_ bản ghi một trang",
            "info": "Hiển thị trang _PAGE_ / _PAGES_",
            "infoEmpty": "Không có bản ghi",
            "search": "Tìm kiếm",
            "paginate": {
                "previous": "Trang trước",
                "next": "Trang sau"
            },
            "emptyTable": "Không có dữ liệu trong bảng"
        },
        "pageLength": 50,
       
    });

    //Không giới hạn số trang trong bảng hóa đơn
    var table = $('.hoaDonNuoc').DataTable({       
        fixedHeader: {
            header: true,
            footer: true
        },
        "autoWidth": false,
        responsive:false,
        "language": {
            "lengthMenu": "Hiển thị _MENU_ bản ghi một trang",
            "info": "Hiển thị trang _PAGE_ / _PAGES_",
            "infoEmpty": "Không có bản ghi",
            "search": "Tìm kiếm",
            "paginate": {
                "previous": "Trang trước",
                "next": "Trang sau"
            },
            "emptyTable": "Không có dữ liệu trong bảng"
        },
        "bSort": false,
        "pageLength": 100000000000
    });

    //để các bảng datatable có class là unlimited k phân trang, có sorting, có searching
    $('.unlimited').DataTable({
       
        "language": {
            "lengthMenu": "Hiển thị _MENU_ bản ghi một trang",
            "info": "Hiển thị trang _PAGE_ / _PAGES_",
            "infoEmpty": "Không có bản ghi",
            "search": "Tìm kiếm",
            "paginate": {
                "previous": "Trang trước",
                "next": "Trang sau"
            },
            "emptyTable": "Không có dữ liệu trong bảng"
        },
        "pageLength": 1000000000000,
        dom: 'Bfrtip',
        buttons: ['excel']
    });

   /* ------ Xem chi tiết chỉ số tuyến ---------------*/
    $('a.toggle-vis').on('click', function (e) {
        e.preventDefault();
        if ($(this).hasClass('clicked')) {
            $(this).removeClass('clicked');
        } else {
            $(this).addClass('clicked');
        }
        // Get the column API object
        var column = table.column($(this).attr('data-column'));

        // Toggle the visibility
        column.visible(!column.visible());
    });   
});
