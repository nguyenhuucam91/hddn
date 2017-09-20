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
        public new double  SH1 { get { return GetD(4); } set { SetD(4, value); } }
        public new double  SH2 { get { return GetD(5); } set { SetD(5, value); } }
        public new double  SH3 { get { return GetD(6); } set { SetD(6, value); } }
        public new double  SH4 { get { return GetD(7); } set { SetD(7, value); } }
        public new double  HC { get { return GetD(8); } set { SetD(8, value); } }
        public new double  CC { get { return GetD(9); } set { SetD(9, value); } }
        public new double  SXXD { get { return GetD(10); } set { SetD(10, value); } }
        public new double  KDDV { get { return GetD(11); } set { SetD(11, value); } }
        public new String MaTuyen { get { return GetSTR(12); } set { SetD(12, value); } }

        #region ThongTinKhachHang
        public new String TenKhachHang { get { return GetSTR(13); } set { SetSTR(13, value); } }
        public new String DiaChi { get { return GetSTR(14); } set { SetSTR(14, value); } }
        public new String MaSoThue { get { return GetSTR(15); } set { SetSTR(15, value); } }
        public new String MaKhachHang { get { return GetSTR(16); } set { SetSTR(16, value); } }
        public new int TuyenKHID { get { return GetINT(17); } set { SetINT(17, value); } }
        public new String SoHopDong { get { return GetSTR(18); } set { SetSTR(18, value); } }
        public new double TiLePhiMoiTruong { get { return GetD(19); } set { SetD(19, value); } }
        public new int QuanHuyenID { get { return GetINT(20); } set { SetINT(20, value); } }
        public new int TTDoc { get { return GetINT(21); } set { SetINT(21, value); } }
        public int? ChiSoTruocKiemDinh { get { return GetINT(22); } set { SetINT(22, value); } }
        public int? ChiSoSauKiemDinh { get { return GetINT(23); } set { SetINT(23, value); } }

        #endregion
    }
}