using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HDNHD.Models.Constants
{
    public enum EUserRole
    {
        Admin = 1,
        InHoaDon = 2,
        KinhDoanh = 3,
        ThuNgan = 4
    }

    public enum ELoaiKhachHang
    {
        CoQuanToChuc = 0,
        [Description("Hộ gia đình")]
        HoGiaDinh = 1,
        [Description("Hộ kinh doanh dịch vụ")]
        HoKinhDoanhDichVu = 2,
        [Description("Cơ quan doanh nghiệp")]
        CoQuanDoanhNghiep = 4,
        [Description("Khu tập thể (hợp đồng tổng)")]
        KhuTapThe = 6 // hợp đồng tổng
    }

    public enum EHinhThucThanhToan
    {
        [Description("Tiền mặt")]
        TienMat = 1,
        [Description("Chuyển khoản")]
        ChuyenKhoan = 2
    }

    public enum ETrangThaiThu
    {
        DaNopTien = 1,
        ChuaNopTien = 2,
        DaQuaHan = 3
    }

    public enum EApGia : int
    {
        [Description("Sinh hoạt")]
        SinhHoat = 1,
        [Description("Cơ quan hành chính sự nghiệp")]
        CoQuanHanhChinh = 2,
        [Description("Đơn vị sự nghiệp")]
        DonViSuNghiep = 3,
        [Description("Kinh doanh dịch vụ")]
        KinhDoanhDichVu = 4,
        [Description("Sản xuất")]
        SanXuat = 5,

        [Description("Tổng hợp")]
        TongHop = 7,
        [Description("Giá đặc biệt")]
        DacBiet = 8,
        SH1 = 9,
        SH2 = 10,
        SH3 = 11,
        SH4 = 12
    }

    public enum ECachTinhGia
    {
        DACBIET = -1,
        TONGHOPSOKHOAN = 0,
        TONGHOPPHANTRAM = 1
    }

    public enum PrintModeEnum
    {
        PRINT_ALL = 1,
        PRINT_SELECTED = 2,
        PRINT_FROM_RECEIPT_TO_RECEIPT = 3
    }
}