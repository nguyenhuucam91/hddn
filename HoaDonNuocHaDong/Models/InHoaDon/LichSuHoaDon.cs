using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.InHoaDon
{
    public class LichSuHoaDon
    {        
        public void Lichsuhoadon(int HoaDonID, int ThangHoaDon, int NamHoaDon, String MaKH, String TenKH, String diaChi, String MST, String SoHopDong, 
            int SanLuongTieuThu, int ChiSoCu, int ChiSoMoi, String NgayBatDau, String NgayKetThuc, double SH1, double SH2, double SH3, double SH4,
            double SH1Price, double SH2Price, double SH3Price, double SH4Price, double HC, double CC, double SX, double KD, double HCPrice,
                double CCPrice, double SXPrice, double KDPrice, double PhiVAT, double TileBVMT, double PhiBVMT, String BangChu, String TTVoOng, int TTDoc,
            int SanLuong, double TongCong, String TTTNgan, int TuyenKHID, double ChiSoCongDon) 
        {
            this.HoaDonID = HoaDonID;
            this.ThangHoaDon = ThangHoaDon;
            this.NamHoaDon = NamHoaDon;
            this.MaKH = MaKH;
            this.TenKH = TenKH;
            this.DiaChi = diaChi;
            this.MST = MST;
            this.SoHopDong = SoHopDong;
            this.SanLuongTieuThu = SanLuongTieuThu;
            this.ChiSoCu = ChiSoCu;
            this.ChiSoMoi = ChiSoMoi;
            this.NgayBatDau = NgayBatDau;
            this.NgayKetThuc = NgayKetThuc;
            this.SH1 = SH1;
            this.SH2 = SH2;
            this.SH3 = SH3;
            this.SH4 = SH4;
            this.SH1Price = SH1Price;
            this.SH2Price = SH2Price;
            this.SH3Price = SH3Price;
            this.SH4Price = SH4Price;
            this.HC = HC;
            this.CC = CC;
            this.SX = SX;
            this.KD = KD;
            this.HCPrice = HCPrice;
            this.CCPrice = CCPrice;
            this.SXPrice = SXPrice;
            this.KDPrice = KDPrice;
            this.ThueSuatPrice = PhiVAT;
            this.TileBVMT = TileBVMT;
            this.PhiBVMT = PhiBVMT;
            this.BangChu = BangChu;
            this.TTVoOng = TTVoOng;
            this.TTDoc = TTDoc;
            this.SanLuong = SanLuong;
            this.TongCong = TongCong;
            this.TTThungan = TTThungan;
            this.TuyenKHID = TuyenKHID;
            this.ChiSoCongDon = ChiSoCongDon;
        }
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
        public double ThueSuatPrice { get; set; }
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