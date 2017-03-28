using System;
using HvitFramework;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoaDonNuocHaDong.Models.BaoCaoInHoaDon;
using System.Data.SqlClient;
using System.Web.Routing;
using HoaDonNuocHaDong.Helper;
using System.Data;
using System.Data.Entity;
using System.Net;
using HoaDonNuocHaDong;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data.Entity.Validation;
using HoaDonNuocHaDong.Base;

namespace HoaDonNuocHaDong.Controllers
{
    public class BaoCaoInHoaDonController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        NguoidungHelper ngDungHelper = new NguoidungHelper();
        // GET: BaoCaoInHoaDon
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BaoCaoSanLuongDoanhThu()
        {
            ViewBag.selectedMonth = DateTime.Now.Month;
            ViewBag.selectedYear = DateTime.Now.Year;
            ViewData["quan"] = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();
            ViewData["tuyen"] = db.Tuyenkhachhangs.Where(p => p.IsDelete == false).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult XuliBaoCaoSanLuongDoanhThu(FormCollection form, int type)
        {
            int month = String.IsNullOrEmpty(form["m1"]) ? DateTime.Now.Month : Convert.ToInt32(form["m1"]);
            int year = String.IsNullOrEmpty(form["y1"]) ? DateTime.Now.Year : Convert.ToInt32(form["y1"]);
            int quanHuyenID = String.IsNullOrEmpty(form["quan"]) ? 0 : Convert.ToInt32(form["quan"]);
            ControllerBase<BaoCaoSanLuongDoanhThu> cB = new ControllerBase<BaoCaoSanLuongDoanhThu>();
            //type = 0 => quận
            if (type == 0)
            {
                if (quanHuyenID == 0)
                {
                    BaoCaoSanLuongDoanhThu bc = cB.Query("BaoCaoSanLuongKinhDoanhTaiVu",
                        new SqlParameter("@thang", month),
                        new SqlParameter("@nam", year)).First();
                    ViewData["baoCaoSanLuongDoanhThu"] = bc;
                }
                else
                {
                    BaoCaoSanLuongDoanhThu bc = cB.Query("BaoCaoSanLuongKinhDoanhTaiVuTheoQuan",
                       new SqlParameter("@thang", month),
                       new SqlParameter("@nam", year),
                       new SqlParameter("@quan", quanHuyenID),
                       new SqlParameter("@d2", 0.05)).First();
                    ViewData["baoCaoSanLuongDoanhThu"] = bc;
                }
            }
            //type = 1 => tuyến
            if (type == 1)
            {
                int tuyenID = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
                BaoCaoSanLuongDoanhThu bc = cB.Query("BaoCaoSanLuongKinhDoanhTaiVuTheoTuyen",
                       new SqlParameter("@thang", month),
                       new SqlParameter("@nam", year),
                       new SqlParameter("@tuyen", tuyenID),
                       new SqlParameter("@d2", 0.05)).First();
                ViewData["baoCaoSanLuongDoanhThu"] = bc;
            }

            ViewBag.selectedMonth = month;
            ViewBag.selectedYear = year;
            return View();
        }

        /// <summary>
        /// Vào view báo cáo nước thải
        /// </summary>
        /// <returns></returns>
        public ActionResult BaoCaoNuocThai()
        {
            ViewData["loaiKhachHang"] = db.LoaiKHs.ToList();
            ViewData["loaiApGia"] = db.Loaiapgias.Where(p => p.LoaiapgiaID != KhachHang.DACBIET).ToList();
            return View();
        }

        /// <summary>
        /// Xử lí báo cáo nước thải
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult XuliBaoCaoNuocThai(FormCollection form)
        {
            int phiNuocThai = String.IsNullOrEmpty(form["phinuocthai"]) ? 0 : Convert.ToInt32(form["phinuocthai"]);
            int month = String.IsNullOrEmpty(form["month"]) ? DateTime.Now.Month : Convert.ToInt16(form["month"]);
            int year = String.IsNullOrEmpty(form["year"]) ? DateTime.Now.Year : Convert.ToInt16(form["year"]);
            int loaiKH = Convert.ToInt32(form["customerType"]);
            int loaiApGia = Convert.ToInt32(form["apgiaType"]);

            ControllerBase<BaoCaoPhiNuocThai> cB = new ControllerBase<BaoCaoPhiNuocThai>();
            List<BaoCaoPhiNuocThai> bc = cB.Query("BaoCaoNuocThai",
                new SqlParameter("@tile", phiNuocThai),
                new SqlParameter("@month", month),
                new SqlParameter("@year", year),
                new SqlParameter("@customerType", loaiKH),
                new SqlParameter("@apgia", loaiApGia)
                ).ToList();
            ViewData["bc"] = bc;
            return View();
        }

        /// <summary>
        /// Danh sách hóa đơn nhận
        /// </summary>
        /// <returns></returns>
        public ActionResult BaoCaoHoaDonNhan()
        {
            List<Tuyenkhachhang> ls = db.Tuyenkhachhangs.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            List<LoaiKH> loaiKH = db.LoaiKHs.ToList();
            ViewData["tuyen"] = ls;
            ViewData["loaiKH"] = loaiKH;
            return View();
        }

        [HttpPost]
        public ActionResult XuliBaoCaoHoaDonNhan(FormCollection form)
        {
            int tuyen = form["tuyen"] == "" ? 0 : Convert.ToInt32(form["tuyen"]);
            int dayStart = form["fromDay"] == "" ? DateTime.Now.Day : Convert.ToInt32(form["fromDay"]);
            int dayEnd = form["toDay"] == "" ? DateTime.Now.Day : Convert.ToInt32(form["toDay"]);
            int monthStart = form["fromMonth"] == "" ? DateTime.Now.Month : Convert.ToInt32(form["fromMonth"]);
            int monthEnd = form["toMonth"] == "" ? DateTime.Now.Month : Convert.ToInt32(form["toMonth"]);
            int yearStart = form["fromYear"] == "" ? DateTime.Now.Year : Convert.ToInt32(form["fromYear"]);
            int yearEnd = form["toYear"] == "" ? DateTime.Now.Year : Convert.ToInt32(form["toYear"]);
            int loaiKH = Convert.ToInt32(form["customerType"]);

            DateTime createdDate = new DateTime(yearStart, monthStart, dayStart);
            DateTime endDate = new DateTime(yearEnd, monthEnd, dayEnd);

            var lsHoaDon = (from i in db.Hoadonnuocs
                            join r in db.Lichsuhoadons on i.HoadonnuocID equals r.HoaDonID
                            join t in db.Khachhangs on i.KhachhangID equals t.KhachhangID
                            where i.Ngayhoadon >= createdDate && i.Ngayhoadon <= endDate && t.TuyenKHID == tuyen
                            select new BaoCaoHoaDonNhan
                            {
                                ID = i.HoadonnuocID,
                                MaKH = r.MaKH,
                                LoaiKH = t.LoaiKHID,
                                NgayHoaDon = i.Ngayhoadon,
                                TenKH = r.TenKH,
                                TongTien = r.TongCong
                            }).ToList();

            //nếu chọn loại khách hàng
            if (loaiKH != 0)
            {
                if (loaiKH == 9) // Nhóm cơ quan, tổ chức
                    lsHoaDon = lsHoaDon.Where(p => p.LoaiKH != 1).ToList();
                else 
                    lsHoaDon = lsHoaDon.Where(p => p.LoaiKH == loaiKH).ToList();
            }

            ViewBag.tenTuyen = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == tuyen).Ten;
            ViewBag.monthStart = monthStart;
            ViewBag.monthEnd = monthEnd;
            ViewBag.yearStart = yearStart;
            ViewBag.yearEnd = yearEnd;
            ViewData["hoadon"] = lsHoaDon;
            return View();
        }

        /// <summary>
        /// Báo cáo dư nợ
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public ActionResult duno(int month, int year)
        {
            int chinhanh = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);

            var selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedChiNhanh = selectedChiNhanh;
            var selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["phongBan"]), chinhanh, 0); ;
            ViewBag.selectedPhongBan = selectedPhongBan;
            var nhanvien = 0;
            if (Request.QueryString["nhanvienid"] != null && Request.QueryString["nhanvienid"].ToString().Length > 0)
            {
                nhanvien = Convert.ToInt32(Request.QueryString["nhanvienid"]);
            }
            if (Request.QueryString["year"] != null && Request.QueryString["year"].ToString() != "")
            {
                year = Convert.ToInt32(Request.QueryString["year"]);
            }
            if (Request.QueryString["month"] != null && Request.QueryString["month"].ToString() != "")
            {
                month = Convert.ToInt32(Request.QueryString["month"]);
            }

            var hoadonnuocs = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.NamHoaDon == year && (h.Trangthaithu == null || h.Trangthaithu == false) && h.Khachhang.QuanhuyenID == (int)selectedChiNhanh);
            var d = hoadonnuocs.Count();
            if (nhanvien != 0)
            {
                hoadonnuocs = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien).Where(h => h.NamHoaDon == year && (h.Trangthaithu == null || h.Trangthaithu == false) && h.NhanvienID == nhanvien);
            }
            return View(hoadonnuocs.ToList());
        }

        /// <summary>
        /// Báo cáo dư có
        /// </summary>
        /// <returns></returns>
        public ActionResult duco()
        {
            int selectedQuanHuyenID = Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0));
            int quanHuyenID = selectedQuanHuyenID;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var selectedChiNhanh = NguoidungHelper.getChiNhanhCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), 0);
            ViewBag.selectedChiNhanh = selectedChiNhanh;
            var selectedPhongBan = ngDungHelper.getPhongBanCuaNguoiDung(Convert.ToInt32(Session["nguoiDungID"]), quanHuyenID, 0);
            ViewBag.selectedPhongBan = selectedPhongBan;
            var nhanvien = 0;
            if (Request.QueryString["nhanvienid"] != null && Request.QueryString["nhanvienid"].ToString().Length > 0)
            {
                nhanvien = Convert.ToInt32(Request.QueryString["nhanvienid"]);
            }
            if (Request.QueryString["year"] != null && Request.QueryString["year"].ToString() != "")
            {
                year = Convert.ToInt32(Request.QueryString["year"]);
            }
            if (Request.QueryString["month"] != null && Request.QueryString["year"].ToString() != "")
            {
                month = Convert.ToInt32(Request.QueryString["month"]);
            }
            var Duco = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year && h.Khachhang.QuanhuyenID == (int)selectedChiNhanh);
            if (nhanvien != 0)
            {
                Duco = db.DuCoes.Where(h => h.SoTienNopTheoThang.Hoadonnuoc.ThangHoaDon == month && h.SoTienNopTheoThang.Hoadonnuoc.NamHoaDon == year && h.SoTienNopTheoThang.Hoadonnuoc.NhanvienID == nhanvien);
            }
            return View(Duco.ToList());
        }
    }
}