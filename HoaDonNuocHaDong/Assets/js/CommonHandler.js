//hàm để lấy danh sách các item được check và bỏ check
//check item và push vào arr
//ẩn các nút yêu cầu lựa chọn như gán nhân viên, gán tuyến
var triggeredShownElement = $(".assignEmployee").add($(".printSelected").add($(".khachHangSelected")));
triggeredShownElement.hide();
var checkedElement = $("input[name='check']");
var data = [];

checkedElement.click(function () {
    if ($(this).is(":checked")) {
        $.each($(this), function (index, value) {
            //nếu không chứa dữ liệu
            if (data.indexOf(value) === -1) {
                data.push($.trim(value.value));
            }
        });
    } else {
        $.each($(this), function (index, value) {
            var indexOfValue = data.indexOf(value.value);           
            //nếu không chứa dữ liệu            
            data.splice(indexOfValue, 1);
        });
    }

    //hiện nút [xóa lựa chọn, gán nhân viên] cho tuyến khi số lượng item trong array data[] > 0, nếu không thì ẩn
    if (data.length > 0) {
        triggeredShownElement.show();
    } else {
        triggeredShownElement.hide();
    }
    $(".selectedTuyen").add($(".printSelectedHidden")).add($(".khachHangSelectedHidden")).val(data.join(","));
    
});

//sửa input type date băng datepicker
$("input[type='date']").prop('type', 'text').addClass('datepicker');

//chèn vào trang để căn ra giữa
$(".row").css("margin-left", "0%");
$(".span12").css("margin-left", "0%");


/*---------------kiểm tra chuỗi rỗng ở trường bắt buộc (kể cả ấn dấu cách)-------------------------*/

var checkReq = function checkEmptySpaceOnField(buttonClicked, itemRequiredChecked, itemShowMessage) {
    buttonClicked.click(function (e) {
        var itemValLength = $.trim(itemRequiredChecked.val()).length;
        if (itemValLength == 0) {
            e.preventDefault();
            itemShowMessage.show();            
        } else {
            this.form.submit();
        }
    });
}

var checkNegativeNumber = function checkNegativeNum(buttonClicked, itemCheckNegative, itemShowMessage) {
    buttonClicked.click(function (e) {
        var checkNegNumber = parseInt(itemCheckNegative);
        if (checkNegNumber < 0) {
            e.preventDefault();
            itemShowMessage.show();
        }
    });
}

//chặn không cho nhập số âm

var blockNegativeNumber = function blockNegativeNumber(item) {
    item.keypress(function (event) {
        if (event.which == 45) {
            event.preventDefault();
        }
    });
}



checkReq($(".createNew"), $("#Ten").add($("#Tieude")), $("span.hide"));
checkNegativeNumber($(".createNew"), $("#Denmuc"), $("span.hide"));
blockNegativeNumber($(".negNumber"));