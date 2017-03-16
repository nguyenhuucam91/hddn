using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using HoaDonNuocHaDong;
using System.Web.Routing;
using iTextSharp.text.html.simpleparser;
using System.Data.Entity;
using System.Net;
using HoaDonHaDong.Helper;
using HoaDonNuocHaDong.Reports;
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Base;

namespace HoaDonNuocHaDong.Controllers
{
    public class PrintController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        private ChiSo cS = new ChiSo();
        private NguoidungHelper ngHelper = new NguoidungHelper();
        private HoaDonNuocHaDong.Helper.Tuyen _tuyen = new HoaDonNuocHaDong.Helper.Tuyen();
        //
        // GET: /Print/
        public ActionResult Index()
        {
            return RedirectToAction("ChiSoTuyen");
        }


        /// <summary>
        /// Tính tiền theo tuyến
        /// </summary>
        /// <param name="tuyenID"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="chinhanhID"></param>
        /// <param name="toID"></param>
        /// <param name="nhanvien"></param>
        public void tinhTienTheoTuyen(String tuyenID, String month, String year, String chinhanhID = null, String toID = null, String nhanvien = null)
        {
            int _tuyenID = Convert.ToInt32(tuyenID);
            int _chinhanhID = Convert.ToInt32(chinhanhID);
            int _toID = Convert.ToInt32(toID);
            int _nhanVienID = Convert.ToInt32(nhanvien);

            int thangIn = String.IsNullOrEmpty(month) ? DateTime.Now.Month : Convert.ToInt32(month);
            int namIn = String.IsNullOrEmpty(year) ? DateTime.Now.Year : Convert.ToInt32(year);

            List<Models.InHoaDon.TuyenTinhTien> danhSach = (from i in db.Lichsuhoadons
                                                            where i.TuyenKHID == _tuyenID && i.ThangHoaDon == thangIn 
                                                                  && i.NamHoaDon == namIn && i.SanLuongTieuThu > 0
                                                            select new Models.InHoaDon.TuyenTinhTien                                                            
                                                            {
                                                                HoaDonNuoc = i.HoaDonID,
                                                                MaKH = i.MaKH,
                                                                TenKH = i.TenKH,
                                                                DiaChi = i.Diachi,
                                                                NgayBatDau = i.NgayBatDau,
                                                                NgayKetThuc = i.NgayKetThuc,
                                                                SH1 = i.SH1,
                                                                SH2 = i.SH2,
                                                                SH3 = i.SH3,
                                                                SH4 = i.SH4,
                                                                HC = i.HC,
                                                                CC = i.CC,
                                                                SX = i.SX,
                                                                KD = i.KD,
                                                                PhiVAT = i.ThueSuatPrice,
                                                                PhiBVMT = i.PhiBVMT,
                                                                TTDoc = i.TTDoc.Value,
                                                                SanLuong = i.ChiSoMoi - i.ChiSoCu,
                                                                TongCong = i.TongCong
                                                            }).ToList();

            ViewBag.beforeFilter = false;
            ViewBag.dsachKH = danhSach.OrderBy(p => p.TTDoc).ToList();
            ViewBag.chiNhanh = db.Quanhuyens.ToList();
            ViewBag.to = db.ToQuanHuyens.ToList();
            ViewBag.nhanVien = db.Nhanviens.ToList();
            ViewBag.tuyen = db.Tuyenkhachhangs.ToList();

            ViewBag.selectedChinhanhID = _chinhanhID;
            ViewBag.selectedTuyen = _tuyenID;
            ViewBag.selectedNhanVien = _nhanVienID;
            ViewBag.selectedTo = _toID;

            ViewBag.selectedMonth = thangIn;
            ViewBag.selectedYear = namIn;
        }

        [HttpPost]
        public ActionResult PrintSelected(FormCollection form, int? TuyenID, int? month, int? year)
        {
            Report report = new Report();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Report.rpt")));
            String[] selectedForm = form["printSelectedHidden"].Split(',');
            List<dynamic> ls = new List<dynamic>();
            foreach (var item in selectedForm)
            {
                int HoaDonID = Convert.ToInt32(item);
                var source = (from p in db.Lichsuhoadons
                              where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year && p.HoaDonID == HoaDonID
                              select new
                              {
                                  ThangHoaDon = p.ThangHoaDon,
                                  NamHoaDon = p.NamHoaDon,
                                  TenKH = p.TenKH,
                                  DiaChi = p.Diachi,
                                  MST = p.MST,
                                  MaKH = p.MaKH,
                                  SoHopDong = p.SoHopDong,
                                  SanLuongTieuThu = p.SanLuongTieuThu,
                                  ChiSoCu = p.ChiSoCu,
                                  ChiSoMoi = p.ChiSoMoi,
                                  SH1 = p.SH1,
                                  SH2 = p.SH2,
                                  SH3 = p.SH3,
                                  SH4 = p.SH4,
                                  SH1Price = p.SH1Price,
                                  SH2Price = p.SH2Price,
                                  SH3Price = p.SH3Price,
                                  SH4Price = p.SH4Price,
                                  HC = p.HC,
                                  CC = p.CC,
                                  SX = p.SX,
                                  KD = p.KD,
                                  HCPrice = p.HCPrice,
                                  CCPrice = p.CCPrice,
                                  SXPrice = p.SXPrice,
                                  KDPrice = p.KDPrice,
                                  TileBVMT = p.TileBVMT,
                                  PhiBVMT = p.PhiBVMT,
                                  TongCong = p.TongCong,
                                  BangChu = p.BangChu,
                                  TTVoOng = p.TTVoOng,
                                  TTThungan = p.TTThungan,
                                  NgayBatDau = p.NgayBatDau,
                                  NgayKetThuc = p.NgayKetThuc,
                                  ChiSoCongDon = p.ChiSoCongDon,
                              }).FirstOrDefault();
                ls.Add(source);
                //cập nhật trạng thái in vào tất cả các hóa đơn được đánh dấu tick
                CapNhatTrangThaiIn(HoaDonID);
            }

            report.SetDataSource(ls);

            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Cập nhật trạng thái in cho các hóa đơn
        /// </summary>
        /// <param name="HoaDonID"></param>
        public void CapNhatTrangThaiIn(int HoaDonID)
        {
            Hoadonnuoc hoaDonObj = db.Hoadonnuocs.Find(HoaDonID);
            if (hoaDonObj != null)
            {
                hoaDonObj.Trangthaiin = true;
                //thêm ngày in vào hóa đơn
                hoaDonObj.NgayIn = DateTime.Now;
                db.Entry(hoaDonObj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// In tất cả danh sách
        /// </summary>
        /// <param name="TuyenID"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrintFrom(FormCollection form, int TuyenID, int month, int year)
        {
            Report report = new Report();
            int count = db.Lichsuhoadons.Count(p => p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year);
            int from = String.IsNullOrEmpty(form["from"]) ? 1 : Convert.ToInt16(form["from"]);
            int to = String.IsNullOrEmpty(form["to"]) ? count : Convert.ToInt16(form["to"]);
            List<dynamic> ls = new List<dynamic>();
            for (int i = from; i <= to; i++)
            {
                var source = (from p in db.Lichsuhoadons
                              where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year && p.TTThungan.Contains(" - " + i)                              
                              select new
                              {
                                  HoaDonID = p.HoaDonID,
                                  ThangHoaDon = p.ThangHoaDon,
                                  NamHoaDon = p.NamHoaDon,
                                  TenKH = p.TenKH,
                                  DiaChi = p.Diachi,
                                  MST = p.MST,
                                  MaKH = p.MaKH,
                                  SoHopDong = p.SoHopDong,
                                  SanLuongTieuThu = p.SanLuongTieuThu,
                                  ChiSoCu = p.ChiSoCu,
                                  ChiSoMoi = p.ChiSoMoi,
                                  SH1 = p.SH1,
                                  SH2 = p.SH2,
                                  SH3 = p.SH3,
                                  SH4 = p.SH4,
                                  SH1Price = p.SH1Price,
                                  SH2Price = p.SH2Price,
                                  SH3Price = p.SH3Price,
                                  SH4Price = p.SH4Price,
                                  HC = p.HC,
                                  CC = p.CC,
                                  SX = p.SX,
                                  KD = p.KD,
                                  HCPrice = p.HCPrice,
                                  CCPrice = p.CCPrice,
                                  SXPrice = p.SXPrice,
                                  KDPrice = p.KDPrice,
                                  TileBVMT = p.TileBVMT,
                                  PhiBVMT = p.PhiBVMT,
                                  TongCong = p.TongCong,
                                  BangChu = p.BangChu,
                                  TTVoOng = p.TTVoOng,
                                  TTThungan = p.TTThungan,
                                  NgayBatDau = p.NgayBatDau,
                                  NgayKetThuc = p.NgayKetThuc,
                                  ChiSoCongDon = p.ChiSoCongDon,
                              }).FirstOrDefault();
                ls.Add(source);

                //cập nhật trạng thái in cho hóa đơn được in từ số thứ tự (số hóa đơn) từ xx->yy
                int hoaDonID = source.HoaDonID;
                //cập nhật trạng thái in vào tất cả các hóa đơn
                CapNhatTrangThaiIn(hoaDonID);
            }

            report.SetDataSource(ls);

            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public ActionResult PrintCrystalReport(int TuyenID, int month, int year)
        {
            Report report = new Report();
            report.Load(Path.Combine(Server.MapPath("~/Reports/Report.rpt")));
            var source = (from p in db.Lichsuhoadons
                          where p.TuyenKHID == TuyenID && p.ThangHoaDon == month && p.NamHoaDon == year && p.SanLuongTieuThu > 0
                          orderby p.TTDoc
                          select new
                          {
                              HoaDonID = p.HoaDonID,
                              ThangHoaDon = p.ThangHoaDon,
                              NamHoaDon = p.NamHoaDon,
                              TenKH = p.TenKH,
                              DiaChi = p.Diachi,
                              MST = p.MST,
                              MaKH = p.MaKH,
                              SoHopDong = p.SoHopDong,
                              SanLuongTieuThu = p.SanLuongTieuThu,
                              ChiSoCu = p.ChiSoCu,
                              ChiSoMoi = p.ChiSoMoi,
                              SH1 = p.SH1,
                              SH2 = p.SH2,
                              SH3 = p.SH3,
                              SH4 = p.SH4,
                              SH1Price = p.SH1Price,
                              SH2Price = p.SH2Price,
                              SH3Price = p.SH3Price,
                              SH4Price = p.SH4Price,
                              HC = p.HC,
                              CC = p.CC,
                              SX = p.SX,
                              KD = p.KD,
                              HCPrice = p.HCPrice,
                              CCPrice = p.CCPrice,
                              SXPrice = p.SXPrice,
                              KDPrice = p.KDPrice,
                              TileBVMT = p.TileBVMT,
                              PhiBVMT = p.PhiBVMT,
                              TongCong = p.TongCong,
                              BangChu = p.BangChu,
                              TTVoOng = p.TTVoOng,
                              TTThungan = p.TTThungan,
                              NgayBatDau = p.NgayBatDau,
                              NgayKetThuc = p.NgayKetThuc,
                              ChiSoCongDon = p.ChiSoCongDon,
                          }).ToList();
            //cập nhật trạng thái in cho tất cả các hóa đơn
            foreach (var item in source)
            {
                int hoaDonID = item.HoaDonID;
                CapNhatTrangThaiIn(hoaDonID);
            }
            //đặt datasource để đẩy vào crystal report
            report.SetDataSource(source);
            try
            {
                Stream str = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Danh sách tuyến đã có chỉ số
        /// </summary>
        /// <returns></returns>
        public ActionResult ChiSoTuyen()
        {
            ViewBag.beforeFiltered = true;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";
            ViewData["tuyen"] = new List<Tuyenkhachhang>();
            ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            return View();
        }

        /// <summary>
        /// Lọc danh sách tuyến đã có chỉ số bằng cách ấn nút Lọc
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChiSoTuyen(FormCollection form)
        {
            //lấy danh sách tổ 
            ViewData["to"] = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            //một tuyến được nhập xong chỉ số tức là tất cả hóa đơn trong đó đã nhập xong
            int month = String.IsNullOrEmpty(form["thang"]) ? DateTime.Now.Month : Convert.ToInt32(form["thang"]);
            int year = String.IsNullOrEmpty(form["year"]) ? DateTime.Now.Year : Convert.ToInt32(form["year"]);
            int to = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            List<Tuyenkhachhang> newLs = new List<Tuyenkhachhang>();
            //nếu tổ ko đc chọn
            if (to == 0)
            {
                //lấy toàn bộ danh sách tuyến trong hệ thống không lọc                
                List<TuyenDuocChot> tuyen = db.TuyenDuocChots.Where(p => p.Nam == year && p.Thang == month).ToList();
                foreach (var item in tuyen)
                {
                    Tuyenkhachhang tuyenKH = db.Tuyenkhachhangs.Find(item.TuyenKHID);
                    newLs.Add(tuyenKH);
                }
            }
            else
            {
                //Lấy danh sách tuyến trong hệ thống lọc theo tổ
                List<int> danhSachTuyenThuocTo = _tuyen.getTuyenByTo(to).Select(p=>p.TuyenCuaKH).Distinct().ToList();
                List<int> danhSachTuyenDaChot = db.TuyenDuocChots.Where(p => p.Nam == year && p.Thang == month).Select(p => p.TuyenKHID.Value).ToList();
                foreach (var r in danhSachTuyenThuocTo.Intersect(danhSachTuyenDaChot))
                {                    
                    Tuyenkhachhang tuyen = db.Tuyenkhachhangs.Find(r);
                    newLs.Add(tuyen);
                }
            }

            ViewBag.beforeFiltered = false;
            ViewBag.hasNumber = "Danh sách tuyến đã có chỉ số";
            ViewBag.selectedMonth = month;
            ViewBag.selectedYear = year;
            ViewData["tuyen"] = newLs;
            return View();
        }

        /// <summary>
        /// Xem chi tiết thông tin tính tiền của tuyến trong tháng và năm
        /// </summary>
        /// <param name="tuyen"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public ActionResult XemChiTiet(String tuyen, String month, String year)
        {
            //Cập nhật trạng thái tính tiền
            int tuyenInt = Convert.ToInt32(tuyen);
            int monthInt = Convert.ToInt32(month);
            int yearInt = Convert.ToInt32(year);
            TuyenDuocChot chotTuyen = db.TuyenDuocChots.FirstOrDefault(p => p.TuyenKHID == tuyenInt && p.Thang == monthInt && p.Nam == yearInt);
            if (chotTuyen != null)
            {
                chotTuyen.TrangThaiTinhTien = true;
                db.Entry(chotTuyen).State = EntityState.Modified;
                db.SaveChanges();
            }
            String tuyenID = Request.QueryString["tuyen"];
            tinhTienTheoTuyen(tuyenID, month, year);
            return View();
        }

        /// <summary>
        /// Hủy hóa đơn cho khách hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult HuyHoaDon()
        {
            ViewBag.hasMaKhachHang = false;
            return View();
        }

        /// <summary>
        /// Hàm lọc khách hàng hủy hóa đơn dựa trên mã khách hàng
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HuyHoaDon(FormCollection form)
        {
            String maKH = String.IsNullOrEmpty(form["maKH"]) ? "" : form["maKH"];
            int ThangHoaDon = String.IsNullOrEmpty(form["month"]) ? DateTime.Now.Month : Convert.ToInt32(form["month"]);
            int namHoaDon = String.IsNullOrEmpty(form["year"]) ? DateTime.Now.Year : Convert.ToInt32(form["year"]);
            Khachhang khHang = db.Khachhangs.FirstOrDefault(p => p.MaKhachHang == maKH);

            if (khHang != null)
            {
                Hoadonnuoc hD = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khHang.KhachhangID && p.ThangHoaDon == ThangHoaDon && p.NamHoaDon == namHoaDon);
                if (hD != null)
                {
                    SoTienNopTheoThang soTienObj = db.SoTienNopTheoThangs.FirstOrDefault(p => p.HoaDonNuocID == hD.HoadonnuocID);
                    if (soTienObj != null)
                    {
                        ViewData["soTienPhaiNop"] = soTienObj;
                    }
                    ViewData["hoadon"] = hD;
                    ViewData["khachHang"] = khHang;
                    ViewBag.hasMaKhachHang = true;
                    ViewBag.maKH = maKH;
                }
            }
            return View();
        }

        /// <summary>
        /// Hiển thị danh sách hủy hóa đơn trong hệ thống
        /// </summary>
        /// <returns></returns>
        public ActionResult DsHuyHoaDon()
        {
            var huyhd = (from i in db.Hoadonnuocbihuys
                         join b in db.Hoadonnuocs on i.HoadonnuocID equals b.HoadonnuocID
                         join t in db.SoTienNopTheoThangs on i.HoadonnuocID equals t.HoaDonNuocID
                         join s in db.Khachhangs on b.KhachhangID equals s.KhachhangID
                         join r in db.Tuyenkhachhangs on s.TuyenKHID equals r.TuyenKHID
                         select new Models.Hoadonnuocbihuy.Hoadonnuocbihuy
                         {
                             id = i.Id,
                             SoHoaDon = i.Sohieuhoadon,
                             maKH = s.MaKhachHang,
                             tenKH = s.Ten,
                             SoTien = t.SoTienPhaiNop,
                             Tuyen = r.Matuyen,
                             ngayHuy = i.Ngayhuyhoadon.ToString(),
                             NguoiYeuCauHuy = i.Nguoiyeucauhuy,
                             lidohuy = i.Lidohuyhoadon,
                             nguoiHuy = i.Nguoihuyhoadon.Value,
                         }).ToList();
            ViewData["huyHoaDon"] = huyhd;
            return View();
        }

        /// <summary>
        /// Xác nhận hủy hóa đơn trong hệ thống
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Xacnhanhuy(FormCollection form)
        {
            int hoadonID = String.IsNullOrEmpty(form["hoadonid"]) ? 0 : Convert.ToInt32(form["hoadonid"]);
            DateTime ngayhuyhoadon = String.IsNullOrEmpty(form["ngayhuyhoadon"]) ? DateTime.Now : Convert.ToDateTime(form["ngayhuyhoadon"]);
            String lidoHuy = String.IsNullOrEmpty(form["lidohuy"]) ? "" : form["lidohuy"];
            String ngYeuCauHuy = String.IsNullOrEmpty(form["ngyeucauhuy"]) ? "" : form["ngyeucauhuy"];
            String soHieuHoaDon = String.IsNullOrEmpty(form["soHoaDon"]) ? "" :form["soHoaDon"];
            //thêm mới record hủy hóa đơn
            int ngDungID = Convert.ToInt32(Session["nguoiDungID"]);
            Hoadonnuocbihuy huyhd = new Hoadonnuocbihuy();
            huyhd.Ngayhuyhoadon = ngayhuyhoadon;
            huyhd.Lidohuyhoadon = lidoHuy;
            huyhd.Nguoiyeucauhuy = ngYeuCauHuy;
            huyhd.HoadonnuocID = hoadonID;
            huyhd.Nguoihuyhoadon = ngHelper.getNhanVienIDFromNguoiDungID(ngDungID);
            huyhd.Sohieuhoadon = soHieuHoaDon;
            db.Hoadonnuocbihuys.Add(huyhd);
            db.SaveChanges();

            //có hóa đơn ID, lấy hóa đơn tương ứng với bảng HoaDon và tiến hành cập nhật trạng thái chốt = false
            Hoadonnuoc hoaDon = db.Hoadonnuocs.Find(hoadonID);
            if (hoaDon != null)
            {
                hoaDon.Trangthaichot = false;
                db.Entry(hoaDon).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("DsHuyHoaDon");
        }
    }
}