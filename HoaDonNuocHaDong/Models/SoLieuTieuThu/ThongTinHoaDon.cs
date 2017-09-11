using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.SoLieuTieuThu
{
    public class ThongTinHoaDon : HoaDonNuoc
    {
        public int TongSoTieuThu { get { return GetINT(0); } set { SetINT(0, value); } }

        public new int LoaiApGiaID { get { return GetINT(1); } set { SetINT(1, value); } }

        public new int SoHo { get { return GetINT(2); } set { SetINT(2, value); } }

        public new int SoKhau { get { return GetINT(3); } set { SetINT(3, value); } }
    }
}