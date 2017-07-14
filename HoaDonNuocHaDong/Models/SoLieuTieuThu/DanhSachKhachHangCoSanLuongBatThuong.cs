using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.SoLieuTieuThu
{
    public class DanhSachKhachHangCoSanLuongBatThuong : HoaDonNuoc
    {
        public new String SoHoaDon { get { return GetSTR(10); } set { SetSTR(10, value); } }
        public new int SoKhoan { get { return GetINT(11); } set { SetINT(11, value); } }
        public new int SanLuongThangTruoc { get { return GetINT(12); } set { SetINT(12, value); } }
    }
}