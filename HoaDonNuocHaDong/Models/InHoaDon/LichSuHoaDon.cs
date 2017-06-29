using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.InHoaDon
{
    public class LichSuHoaDon : ModelBase
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
        public int HoaDonID { get { return GetINT(0); } set { SetINT(0, value); } }
        public int ThangHoaDon { get { return GetINT(1); } set { SetINT(1, value); } }
        public int NamHoaDon { get { return GetINT(2); } set { SetINT(2, value); } }
        public String MaKH { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public String TenKH { get { return GetSTR(4); } set { SetSTR(4, value); } }
        public String DiaChi { get { return GetSTR(5); } set { SetSTR(5, value); } }
        public String MST { get { return GetSTR(6); } set { SetSTR(6, value); } }
        public String SoHopDong { get { return GetSTR(7); } set { SetSTR(7, value); } }
        public int SanLuongTieuThu { get { return GetINT(8); } set { SetINT(8, value); } }
        public int ChiSoCu { get { return GetINT(9); } set { SetINT(9, value); } }
        public int ChiSoMoi { get { return GetINT(10); } set { SetINT(10, value); } }
        public String NgayBatDau { get { return GetSTR(11); } set { SetSTR(11, value); } }
        public String NgayKetThuc { get { return GetSTR(12); } set { SetSTR(12, value); } }
        #endregion

        #region SinhHoat
        public double SH1 { get { return GetD(13); } set { SetD(13, value); } }
        public double SH1Price { get { return GetD(14); } set { SetD(14, value); } }
        public double SH2 { get { return GetD(15); } set { SetD(15, value); } }
        public double SH2Price { get { return GetD(16); } set { SetD(16, value); } }
        public double SH3 { get { return GetD(17); } set { SetD(17, value); } }
        public double SH3Price { get { return GetD(18); } set { SetD(18, value); } }
        public double SH4 { get { return GetD(19); } set { SetD(19, value); } }
        public double SH4Price { get { return GetD(20); } set { SetD(20, value); } }
        #endregion

        #region KhongPhaiSinhHoat
        public double CC { get { return GetD(21); } set { SetD(21, value); } }
        public double CCPrice { get { return GetD(22); } set { SetD(22, value); } }
        public double HC { get { return GetD(23); } set { SetD(23, value); } }
        public double HCPrice { get { return GetD(24); } set { SetD(24, value); } }
        public double SX { get { return GetD(25); } set { SetD(25, value); } }
        public double SXPrice { get { return GetD(26); } set { SetD(26, value); } }
        public double KD { get { return GetD(27); } set { SetD(27, value); } }
        public double KDPrice { get { return GetD(28); } set { SetD(28, value); } }

        #endregion
        public double TruocThue { get { return GetD(29); } set { SetD(29, value); } }
        public double ThueSuatPrice { get { return GetD(30); } set { SetD(30, value); } }
        public double TileBVMT { get { return GetD(31); } set { SetD(31, value); } }
        public double PhiBVMT { get { return GetD(32); } set { SetD(32, value); } }
        public String BangChu { get { return GetSTR(33); } set { SetSTR(33, value); } }
        public String TTVoOng { get { return GetSTR(34); } set { SetSTR(34, value); } }
        public int TTDoc { get { return GetINT(35); } set { SetINT(35, value); } }
        public int SanLuong { get { return GetINT(36); } set { SetINT(36, value); } }
        public double TongCong { get { return GetD(37); } set { SetD(37, value); } }
        public String TTThungan { get { return GetSTR(38); } set { SetSTR(38, value); } }
        public int TuyenKHID { get { return GetINT(39); } set { SetINT(39, value); } }
        public double ChiSoCongDon { get { return GetD(40); } set { SetD(40, value); } }

        public static String[] sortLichSuHoaDonByTTDoc(String[] hoadons)
        {           
            HoaDonHaDongEntities db = new HoaDonHaDongEntities();
            List<Lichsuhoadon> lichSuHoaDons = new List<Lichsuhoadon>();
            String[] lichSuHoaDonsAsArray = new String[hoadons.Length];
            foreach(var hoadon in hoadons)
            {
                int hoaDonId = Convert.ToInt32(hoadon);
                Lichsuhoadon lichSuHoaDon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hoaDonId);
                lichSuHoaDons.Add(lichSuHoaDon);
            }
            List<Lichsuhoadon> sortedLichSuHoaDonsByTTDoc = lichSuHoaDons.OrderBy(p => p.TTDoc).ToList();
            int i = 0;
            foreach (var sortedLichSuHoaDonByTTDoc in sortedLichSuHoaDonsByTTDoc)
            {
                lichSuHoaDonsAsArray[i] = sortedLichSuHoaDonByTTDoc.HoaDonID.ToString();
                i++;
            }
            return lichSuHoaDonsAsArray;
        }
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}