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
        public static void themMoiHoaDonThangSau(int KHID, int HoaDonID, int ChiSoCuoi, int nhanvienID, int? _month, int? _year, DateTime ngayBatDau)
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
                SH1 = chiTiet.SH1.GetValueOrDefault();
                SH2 = chiTiet.SH2.GetValueOrDefault();
                SH3 = chiTiet.SH3.GetValueOrDefault();
                SH4 = chiTiet.SH4.GetValueOrDefault();
                SXXD = chiTiet.SXXD.GetValueOrDefault();
                HC = chiTiet.HC.GetValueOrDefault();
                CC = chiTiet.CC.GetValueOrDefault();
                KDDV = chiTiet.KDDV.GetValueOrDefault();
            }

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
            double tongPhiBVMT = Convert.ToInt32(tongTienDinhMuc * tileBVMT);

            /* congnv 170515 */
            var stntt = db.SoTienNopTheoThangs.FirstOrDefault(p => p.HoaDonNuocID == HoaDonID);
            var ducoTruoc = db.DuCoes.FirstOrDefault(m => m.KhachhangID == hoadon.KhachhangID // dư có trước đó chưa trừ hết
                && (m.TrangThaiTruHet == false || // chưa trừ hoặc đã trừ hết cho hóa đơn này trước đó (trong TH cập nhật)
                (m.TrangThaiTruHet == true && m.NgayTruHet.Value.Month == hoadon.ThangHoaDon && m.NgayTruHet.Value.Year == hoadon.NamHoaDon)));
            DuCo duco = null; // dư có tháng này (trong TH cập nhật)

            if (stntt == null)
            {
                stntt = new SoTienNopTheoThang()
                {
                    HoaDonNuocID = hoadon.HoadonnuocID,
                    SoTienDaThu = 0,
                    SoTienPhaiNop = 0
                };

                db.SoTienNopTheoThangs.Add(stntt);
            } else {
                db.Entry(stntt).State = System.Data.Entity.EntityState.Modified;
                duco = db.DuCoes.FirstOrDefault(m => m.TienNopTheoThangID == stntt.ID);
            }

            stntt.SoTienTrenHoaDon = (int) (tongTienDinhMuc + tongVAT + tongPhiBVMT);
            stntt.SoTienPhaiNop = stntt.SoTienTrenHoaDon;

            if (ducoTruoc != null) // trừ dư có (nếu có)
            {
                ducoTruoc.TrangThaiTruHet = true;
                ducoTruoc.NgayTruHet = DateTime.Now;

                if (ducoTruoc.SoTienDu < stntt.SoTienTrenHoaDon)
                {
                    stntt.SoTienPhaiNop -= ducoTruoc.SoTienDu;
                } else
                {
                    stntt.SoTienPhaiNop = 0;
                    // update hoadon
                    hoadon.Trangthaithu = true;
                    hoadon.NgayNopTien = DateTime.Now;

                    // save db cập nhật stntt.ID
                    db.SaveChanges();

                    if (ducoTruoc.SoTienDu > stntt.SoTienTrenHoaDon)
                    {
                       if (duco == null)
                        {
                            duco = new DuCo()
                            {
                                KhachhangID = hoadon.KhachhangID,
                                TienNopTheoThangID = stntt.ID,
                            };
                            db.DuCoes.Add(duco);
                        } else
                        {
                            db.Entry(duco).State = System.Data.Entity.EntityState.Modified;
                        }

                        duco.SoTienDu = ducoTruoc.SoTienDu - stntt.SoTienTrenHoaDon;
                    }
                }
            }
            db.SaveChanges();
            /* END congnv 170515 */
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