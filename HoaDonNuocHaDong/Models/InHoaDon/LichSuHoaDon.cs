using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.InHoaDon
{
    public class LichSuHoaDon
    {
        #region ThongTinCoBan
        public int HoaDonID { get; set; }
        public int ThangHoaDon { get; set; }
        public int NamHoaDon { get; set; }
        public String MaKH { get; set; }
        public String TenKH { get; set; }
        public String DiaChi { get; set; }
        public String MST { get; set; }
        public String SoHopDong { get; set; }
        public int SanLuongTieuThu { get; set; }
        public int ChiSoCu { get; set; }
        public int ChiSoMoi { get; set; }
        public String NgayBatDau { get; set; }
        public String NgayKetThuc { get; set; }
        #endregion

        #region SinhHoat
        public double SH1 { get; set; }
        public double SH2 { get; set; }
        public double SH3 { get; set; }
        public double SH4 { get; set; }

        public double SH1Price { get; set; }
        public double SH2Price { get; set; }
        public double SH3Price { get; set; }
        public double SH4Price { get; set; }
        #endregion

        #region KhongPhaiSinhHoat
        public double HC { get; set; }
        public double CC { get; set; }
        public double SX { get; set; }
        public double KD { get; set; }
        public double HCPrice { get; set; }
        public double CCPrice { get; set; }
        public double SXPrice { get; set; }
        public double KDPrice { get; set; }
        #endregion
        public double PhiVAT { get; set; }
        public double TileBVMT { get; set; }
        public double PhiBVMT { get; set; }
        public String BangChu { get; set; }
        public String TTVoOng { get; set; }
        public int TTDoc { get; set; }
        public int SanLuong { get; set; }
        public double TongCong { get; set; }
        public String TTThungan { get; set; }
        public int TuyenKHID { get; set; }

        public double ChiSoCongDon { get; set; }
    }
}