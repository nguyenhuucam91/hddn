using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HDNHD.Models.Constants
{
    public enum ELoaiKhachHang : int
    {
        CoQuanToChuc = 0,
        HoGiaDinh = 1,
        HoKinhDoanhDichVu = 2,
        CoQuanDoanhNghiep = 4,
        KhuTapThe = 6 // hợp đồng tổng
    }

    public enum EHinhThucThanhToan : int
    {
        TienMat = 1,
        ChuyenKhoan = 2,
        ThuTrucTiep = 3
    }

    public enum EApGia : int
    {
        SinhHoat = 1,
        CoQuanHanhChinh = 2,
        DonViSuNghiep = 3,
        KinhDoanhDichVu = 4,
        SanXuat = 5,

        TongHop = 7,
        DacBiet = 8,
        SH1 = 9,
        SH2 = 10,
        SH3 = 11,
        SH4 = 12
    }

    public enum EUserRole
    {
        Admin = 1,
        InHoaDon = 2,
        KinhDoanh = 3,
        ThuNgan = 4
    }

    public enum ECachTinhGia
    {
        DACBIET = -1,
        TONGHOPSOKHOAN = 0,
        TONGHOPPHANTRAM = 1
    }
}