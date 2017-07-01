using HoaDonNuocHaDong.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Repositories
{
    public class LichSuHoaDonRepository
    {
        protected HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        public void updateLichSuHoaDon(int HoaDonID, int thangHoaDon, int namHoaDon, String tenKH, String diaChi, String MST, String maKH, int TuyenKHID, String soHD,
            int chiSoCu, int ChiSoMoi, int TongTieuThu, double SH1, double SH1Price, double SH2, double SH2Price, double SH3, double SH3Price, double SH4, double SH4Price, double HC, double HCPrice,
            double CC, double CCPrice, double SX, double SXPrice, double KD, double KDPrice, double DinhMuc, double Thue, double TienThueVAT, double TileBVMT,
            double BVMTPrice, double TongCong, String bangChu, String TTVoOng, String ThuNgan, int tuyen, int TTDoc, double chiSoCongDon, string ngayBatDau, string ngayKetThuc)
        {
            Lichsuhoadon lichSuHoaDon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == HoaDonID);
            if (lichSuHoaDon != null)
            {
                lichSuHoaDon.HoaDonID = HoaDonID;
                lichSuHoaDon.ThangHoaDon = thangHoaDon;
                lichSuHoaDon.NamHoaDon = namHoaDon;
                lichSuHoaDon.TenKH = tenKH;
                lichSuHoaDon.Diachi = diaChi;
                lichSuHoaDon.MST = MST == null ? "" : MST;
                lichSuHoaDon.MaKH = maKH;
                lichSuHoaDon.TuyenKHID = TuyenKHID;
                lichSuHoaDon.SoHopDong = soHD;
                lichSuHoaDon.ChiSoCu = chiSoCu;
                lichSuHoaDon.ChiSoMoi = ChiSoMoi;
                lichSuHoaDon.SanLuongTieuThu = TongTieuThu;
                lichSuHoaDon.SH1 = SH1;
                lichSuHoaDon.SH1Price = SH1Price;
                lichSuHoaDon.SH2 = SH2;
                lichSuHoaDon.SH2Price = SH2Price;
                lichSuHoaDon.SH3 = SH3;
                lichSuHoaDon.SH3Price = SH3Price;
                lichSuHoaDon.SH4 = SH4;
                lichSuHoaDon.SH4Price = SH4Price;
                lichSuHoaDon.HC = HC;
                lichSuHoaDon.HCPrice = HCPrice;
                lichSuHoaDon.CC = CC;
                lichSuHoaDon.CCPrice = CCPrice;
                lichSuHoaDon.SX = SX;
                lichSuHoaDon.SXPrice = SXPrice;
                lichSuHoaDon.KD = KD;
                lichSuHoaDon.KDPrice = KDPrice;
                lichSuHoaDon.TruocThue = DinhMuc;
                lichSuHoaDon.ThueSuat = Thue;
                lichSuHoaDon.ThueSuatPrice = TienThueVAT;
                lichSuHoaDon.TileBVMT = TileBVMT;
                lichSuHoaDon.PhiBVMT = BVMTPrice;
                lichSuHoaDon.TongCong = TongCong;
                lichSuHoaDon.BangChu = ConvertMoney.So_chu(TongCong);
                lichSuHoaDon.TTVoOng = TTVoOng;
                lichSuHoaDon.TTThungan = ThuNgan;
                lichSuHoaDon.TuyenKHID = tuyen;
                lichSuHoaDon.TTDoc = TTDoc;
                lichSuHoaDon.ChiSoCongDon = chiSoCongDon;
                lichSuHoaDon.NgayBatDau = ngayBatDau;
                lichSuHoaDon.NgayKetThuc = ngayKetThuc;
                db.Entry(lichSuHoaDon).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                addLichSuHoaDon(HoaDonID, thangHoaDon, namHoaDon, tenKH, diaChi, MST, maKH, TuyenKHID, soHD,
              chiSoCu, ChiSoMoi, TongTieuThu, SH1, SH1Price, SH2, SH2Price, SH3, SH3Price, SH4, SH4Price, HC, HCPrice,
              CC, CCPrice, SX, SXPrice, KD, KDPrice, DinhMuc, Thue, TienThueVAT, TileBVMT,
              BVMTPrice, TongCong, bangChu, TTVoOng, ThuNgan, tuyen, TTDoc, chiSoCongDon, ngayBatDau, ngayKetThuc);
            }

            
        }

        public void addLichSuHoaDon(int HoaDonID, int thangHoaDon, int namHoaDon, String tenKH, String diaChi, String MST, String maKH, int TuyenKHID, String soHD,
            int chiSoCu, int ChiSoMoi, int TongTieuThu, double SH1, double SH1Price, double SH2, double SH2Price, double SH3, double SH3Price, double SH4, double SH4Price, double HC, double doubleHCPrice,
            double CC, double CCPrice, double SX, double SXPrice, double KD, double KDPrice, double DinhMuc, double Thue, double TienThueVAT, double TileBVMT,
            double BVMTPrice, double TongCong, String bangChu, String TTVoOng, String ThuNgan, int tuyen, int TTDoc, double chiSoCongDon, string ngayBatDau, string ngayKetThuc)
        {
            Lichsuhoadon history = new Lichsuhoadon();
            history.HoaDonID = HoaDonID;
            history.ThangHoaDon = thangHoaDon;
            history.NamHoaDon = namHoaDon;
            history.TenKH = tenKH;
            history.Diachi = diaChi;
            history.MST = MST == null ? "" : MST;
            history.TuyenKHID = TuyenKHID;
            history.MaKH = maKH;
            history.SoHopDong = soHD;
            history.ChiSoCu = chiSoCu;
            history.ChiSoMoi = ChiSoMoi;
            history.SanLuongTieuThu = TongTieuThu;
            history.SH1 = SH1;
            history.SH1Price = SH1Price;
            history.SH2 = SH2;
            history.SH2Price = SH2Price;
            history.SH3 = SH3;
            history.SH3Price = SH3Price;
            history.SH4 = SH4;
            history.SH4Price = SH4Price;
            history.HC = HC;
            history.HCPrice = doubleHCPrice;
            history.CC = CC;
            history.CCPrice = CCPrice;
            history.SX = SX;
            history.SXPrice = SXPrice;
            history.KD = KD;
            history.KDPrice = KDPrice;
            history.TruocThue = DinhMuc;
            history.ThueSuat = Thue;
            history.ThueSuatPrice = TienThueVAT;
            history.TileBVMT = TileBVMT;
            history.PhiBVMT = BVMTPrice;
            history.TongCong = TongCong;
            history.BangChu = ConvertMoney.So_chu(TongCong);
            history.TTVoOng = TTVoOng;
            history.TTThungan = ThuNgan;
            history.TuyenKHID = tuyen;
            history.TTDoc = TTDoc;
            history.ChiSoCongDon = chiSoCongDon;
            history.NgayBatDau = ngayBatDau;
            history.NgayKetThuc = ngayKetThuc;
            db.Lichsuhoadons.Add(history);
            SaveChanges();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public bool hasLichSuHoaDon(int hoaDonID)
        {
            int countLichSuHoaDon = db.Lichsuhoadons.Count(p => p.HoaDonID == hoaDonID);
            if (countLichSuHoaDon > 0)
            {
                return true;
            }
            return false;
        }

        public Lichsuhoadon findLichSuHoaDonByHoaDonId(int hoaDonID)
        {
            Lichsuhoadon lichSuHoaDon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hoaDonID);
            if (lichSuHoaDon != null)
            {
                return lichSuHoaDon;
            }
            return null;
        }
    }
}