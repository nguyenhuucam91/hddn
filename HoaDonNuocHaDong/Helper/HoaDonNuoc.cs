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
        HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
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
        public static void themMoiHoaDonThangSau(int KHID, int ChiSoCuoi, int nhanvienID, int? _month, int? _year, DateTime ngayBatDau)
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
            var hoaDonNuocThangSau = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == KHID && p.ThangHoaDon == month && p.NamHoaDon == year);
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

                //tạo new record chi tiết hóa đơn nước vào tháng sau
                Chitiethoadonnuoc chiTietThangSau = new Chitiethoadonnuoc();
                chiTietThangSau.HoadonnuocID = hoaDonThangSau.HoadonnuocID;
                chiTietThangSau.Chisocu = ChiSoCuoi;
                db.Chitiethoadonnuocs.Add(chiTietThangSau);
                db.SaveChanges();

            }

            //nếu có record trong tháng sau thì cập nhật lại ngày bắt đầu và các thông số liên quan
            else
            {
                hoaDonNuocThangSau.Ngaybatdausudung = ngayBatDau;
                db.SaveChanges();
                var chiTietHoaDonNuocExist = hoaDonNuocThangSau.Chitiethoadonnuocs.FirstOrDefault();
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
        /// Hàm để format chuỗi tiền tệ
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static String formatCurrency(double money)
        {
            System.Globalization.CultureInfo danishCulture = new System.Globalization.CultureInfo("vi-VN");
            return money.ToString("#,###.##", danishCulture);
        }

        public String getCssClassTinhTrangHoaDonBiHuy(int hoaDonNuocID)
        {
            Hoadonnuocbihuy hoaDonBiHuy = _db.Hoadonnuocbihuys.FirstOrDefault(p => p.HoadonnuocID == hoaDonNuocID);
            String cssClass = "";
            if (hoaDonBiHuy != null)
            {
                if (hoaDonBiHuy.Trangthaicapnhathuy == false || hoaDonBiHuy.Trangthaicapnhathuy == null)
                {
                    cssClass = "hoadonbihuy";
                }
                else
                {
                    cssClass = "hoadonduoccapnhat";
                }
            }
            return cssClass;
        }

        public DateTime compareNgayCapNuocLaiVoiNgayBatDauHoaDon(DateTime ngayCapNuocLai, DateTime ngayBatDauHoaDon)
        {
            if (DateTime.Compare(ngayCapNuocLai, ngayBatDauHoaDon) > 0)
            {
                return ngayCapNuocLai;
            }
            return ngayBatDauHoaDon;
        }
    }
}