using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HDNHD.Models.Constants
{
    public enum ELoaiKhachHang : int
    {
        HoGiaDinh = 1,
        CoQuanToChuc = 2
    }

    public enum EHinhThucThanhToan : int
    {
        TrucTiep = 1,
        ChuyenKhoan = 2
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