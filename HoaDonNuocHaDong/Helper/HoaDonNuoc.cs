using HoaDonNuocHaDong;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class HoaDonNuoc
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        HoaDonHaDong.Helper.ChiSo cS = new HoaDonHaDong.Helper.ChiSo();
        /// <summary>
        /// Lấy thứ tự đọc của hóa đơn dựa theo hóa đơn ID
        /// </summary>
        /// <param name="hoaDonID"></param>
        /// <returns></returns>
        public int getThuTuDoc(int KHID)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            var khachHang = _db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            if (khachHang != null)
            {
                return khachHang.TTDoc.Value;
            }
            return 0;
        }

        /// <summary>
        /// Thêm mới hóa đơn cho tháng sau
        /// </summary>
        /// <param name="KHID"></param>
        /// <param name="HoaDonID"></param>
        /// <param name="ChiSoCuoi"></param>
        public static void themMoiHoaDonThangSau(int KHID, int HoaDonID, int ChiSoCuoi, int nhanvienID, int? _month, int? _year,DateTime ngayBatDau)
        {
            int month = _month.Value; int year = _year.Value;
            //thêm 1 record vào tháng sau trong trường hợp chưa có
            //nếu tháng sau = 12 thì chuyển năm
            if (month + 1 > 12)
            {
                //ngayHoaDon = new DateTime(DateTime.Now.Year + 1, 1,15);
                month = 1;
                year = _year.Value + 1;
            }
            //nếu không thì vẫn chuyển năm như bình thường
            else
            {
                month = month + 1;
            }
            var hoaDonNuocThangSau = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == KHID && p.ThangHoaDon == month
                && p.NamHoaDon == year);
            //nếu k có trong db mới tiến hành add record
            if (hoaDonNuocThangSau == null)
            {
                Hoadonnuoc hoaDonThangSau = new Hoadonnuoc();
                hoaDonThangSau.Ngayhoadon = null;
                hoaDonThangSau.KhachhangID = KHID;

                //lấy hóa đơn tháng sau dựa trên thứ tự đọc của hóa đơn tháng trước              
                hoaDonThangSau.ThangHoaDon = month;
                hoaDonThangSau.NamHoaDon = year;
                hoaDonThangSau.NhanvienID = nhanvienID;
                hoaDonThangSau.Ngaybatdausudung = ngayBatDau;
                db.Hoadonnuocs.Add(hoaDonThangSau);
                db.SaveChanges();

                var chiTietHoaDonNuocExist = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == hoaDonThangSau.HoadonnuocID);
                //nếu k có trong db thì add
                if (chiTietHoaDonNuocExist == null)
                {
                    //tạo new record chi tiết hóa đơn nước vào tháng sau
                    Chitiethoadonnuoc chiTietThangSau = new Chitiethoadonnuoc();
                    chiTietThangSau.HoadonnuocID = hoaDonThangSau.HoadonnuocID;
                    chiTietThangSau.Chisocu = ChiSoCuoi;
                    db.Chitiethoadonnuocs.Add(chiTietThangSau);
                    db.SaveChanges();
                }
                else
                {
                    chiTietHoaDonNuocExist.Chisocu = ChiSoCuoi;
                    db.SaveChanges();
                }
            }

            //nếu có record trong tháng sau thì cập nhật lại ngày bắt đầu và các thông số liên quan
            else
            {
                hoaDonNuocThangSau.Ngaybatdausudung = ngayBatDau;
                db.SaveChanges();
                var chiTietHoaDonNuocExist = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == hoaDonNuocThangSau.HoadonnuocID);
                //nếu k có trong db thì add
                if (chiTietHoaDonNuocExist == null)
                {
                    //tạo new record chi tiết hóa đơn nước vào tháng sau
                    Chitiethoadonnuoc chiTietThangSau = new Chitiethoadonnuoc();
                    chiTietThangSau.HoadonnuocID = hoaDonNuocThangSau.HoadonnuocID;
                    chiTietThangSau.Chisocu = ChiSoCuoi;
                    db.Chitiethoadonnuocs.Add(chiTietThangSau);
                    db.SaveChanges();
                }
                else
                {
                    chiTietHoaDonNuocExist.Chisocu = ChiSoCuoi;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Thêm mới số tiền phải nộp trong tháng đó
        /// </summary>
        /// <param name="HoaDonID"></param>

        public void themMoiSoTienPhaiNop(int HoaDonID)
        {
            Chitiethoadonnuoc chiTiet = db.Chitiethoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            Hoadonnuoc hoadon = db.Hoadonnuocs.FirstOrDefault(p => p.HoadonnuocID == HoaDonID);
            //khu vực SH
            double SH1 = 0; double SXXD = 0;
            double SH2 = 0; double HC = 0;
            double SH3 = 0; double CC = 0;
            double SH4 = 0; double KDDV = 0;

            if (chiTiet != null)
            {
                 SH1 = chiTiet.SH1.Value;
                 SH2 = chiTiet.SH2.Value;
                 SH3 = chiTiet.SH3.Value;
                 SH4 = chiTiet.SH4.Value;
                 SXXD = chiTiet.SXXD.Value;
                 HC = chiTiet.HC.Value;
                 CC = chiTiet.CC.Value;
                 KDDV = chiTiet.KDDV.Value;
            }
            var soTienPhaiNopTheoThang = db.SoTienNopTheoThangs.FirstOrDefault(p => p.HoaDonNuocID == HoaDonID);
            int thangHoaDon = hoadon.ThangHoaDon.Value;
            int namHoaDon = hoadon.NamHoaDon.Value;
            int PhiBVMT = hoadon.Khachhang.Tilephimoitruong.Value;
            //tỉnh tổng định mức
            double tongTienDinhMuc = cS.tinhTongTienTheoDinhMuc(HoaDonID, SH1, SH2, SH3, SH4, HC, CC, KDDV, SXXD);
            double tongVAT = Convert.ToInt32(tongTienDinhMuc * 0.05);
            var phiBVMT = (from i in db.Hoadonnuocs
                           join r in db.Khachhangs on i.KhachhangID equals r.KhachhangID
                           select new
                           {
                               TilePhiMoiTruong = r.Tilephimoitruong
                           }).FirstOrDefault();
            double soBVMT = Convert.ToDouble(phiBVMT.TilePhiMoiTruong.Value);
            double tileBVMT = soBVMT / 100; //ra 0
            double tongPhiBVMT = Convert.ToInt32(tongTienDinhMuc*tileBVMT);
            if (soTienPhaiNopTheoThang != null)
            {
                soTienPhaiNopTheoThang.SoTienPhaiNop = tongTienDinhMuc+tongVAT+tongPhiBVMT;
                db.Entry(soTienPhaiNopTheoThang).State = System.Data.Entity.EntityState.Modified;
                hoadon.SoTienNopTheoThangID = soTienPhaiNopTheoThang.ID;
                hoadon.SoTienNopTheoThang = soTienPhaiNopTheoThang;
                db.SaveChanges();
            }
            else
            {
                SoTienNopTheoThang sotien = new SoTienNopTheoThang();
                sotien.HoaDonNuocID = HoaDonID;
                sotien.SoTienPhaiNop = tongTienDinhMuc + tongPhiBVMT + tongVAT;
                sotien.SoTienDaThu = 0;
                hoadon.SoTienNopTheoThangID = sotien.ID;
                hoadon.SoTienNopTheoThang = sotien;
                db.SoTienNopTheoThangs.Add(sotien);
                db.SaveChanges();
            }

            var KhID = hoadon.KhachhangID;
            var thang = hoadon.ThangHoaDon - 1;
            var nam = hoadon.NamHoaDon;
            if (DateTime.Now.Month == 1)
            {
                thang = 12;
                nam = nam - 1;
            }
            DuCo duCoTruoc = db.DuCoes.FirstOrDefault(d => d.KhachhangID == KhID && d.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == thang && d.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == nam);
            if (duCoTruoc != null)
            {
                var DuCo = new DuCo();
                DuCo.KhachhangID = KhID;

                DuCo.TienNopTheoThangID = soTienPhaiNopTheoThang.ID;
                DuCo.SoTienDu = duCoTruoc.SoTienDu;
                db.DuCoes.Add(DuCo);
                db.SaveChanges();
                int id = DuCo.DuCoID;
            }
        }

        /// <summary>
        /// Hàm để format chuỗi tiền tệ
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static String formatCurrency(double money)
        {
            System.Globalization.CultureInfo danishCulture = new System.Globalization.CultureInfo("da");            
            return money.ToString("#,###.##", danishCulture);
        }
    }
}