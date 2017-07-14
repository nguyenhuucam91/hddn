using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.SoLieuTieuThu
{
    public class DanhSachHoaDonKhongSanLuong : HoaDonNuoc
    {
        public new int ChiSoCu { get { return GetINT(2); } set { SetINT(2, value); } }

        public new DateTime? NgayBatDauSuDung { get { return GetDT(3); } set { SetDT(3, value); } }
    }
}