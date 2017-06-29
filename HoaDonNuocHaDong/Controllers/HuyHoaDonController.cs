using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Controllers
{
    public class HuyHoaDonController : BaseController
    {
        private NguoidungHelper ngHelper = new NguoidungHelper();
        //
        // GET: /HuyHoaDon/
        public ActionResult Index()
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


        [HttpPost]
        public ActionResult Xacnhanhuy(FormCollection form)
        {
            int hoadonID = String.IsNullOrEmpty(form["hoadonid"]) ? 0 : Convert.ToInt32(form["hoadonid"]);
            DateTime ngayhuyhoadon = String.IsNullOrEmpty(form["ngayhuyhoadon"]) ? DateTime.Now : Convert.ToDateTime(form["ngayhuyhoadon"]);
            String lidoHuy = String.IsNullOrEmpty(form["lidohuy"]) ? "" : form["lidohuy"];
            String ngYeuCauHuy = String.IsNullOrEmpty(form["ngyeucauhuy"]) ? "" : form["ngyeucauhuy"];
            String soHieuHoaDon = String.IsNullOrEmpty(form["soHoaDon"]) ? "" : form["soHoaDon"];
            //thêm mới record hủy hóa đơn
            int ngDungID = Convert.ToInt32(Session["nguoiDungID"]);

            Hoadonnuocbihuy hoaDonBiHuy = db.Hoadonnuocbihuys.FirstOrDefault(p => p.HoadonnuocID == hoadonID);
            if (hoaDonBiHuy != null)
            {
                hoaDonBiHuy.Trangthaicapnhathuy = false;
            }
            else
            {
                Hoadonnuocbihuy huyhd = new Hoadonnuocbihuy();
                huyhd.Ngayhuyhoadon = ngayhuyhoadon;
                huyhd.Lidohuyhoadon = lidoHuy;
                huyhd.Nguoiyeucauhuy = ngYeuCauHuy;
                huyhd.HoadonnuocID = hoadonID;
                huyhd.Nguoihuyhoadon = ngHelper.getNhanVienIDFromNguoiDungID(ngDungID);
                huyhd.Sohieuhoadon = soHieuHoaDon;
                huyhd.Trangthaicapnhathuy = false;
                db.Hoadonnuocbihuys.Add(huyhd);
                db.SaveChanges();
            }

            //có hóa đơn ID, lấy hóa đơn tương ứng với bảng HoaDon và tiến hành cập nhật trạng thái chốt = false
            Hoadonnuoc hoaDon = db.Hoadonnuocs.Find(hoadonID);
            if (hoaDon != null)
            {
                hoaDon.Trangthaichot = false;
                hoaDon.Trangthaiin = false;
                hoaDon.Tongsotieuthu = 0;
                db.Entry(hoaDon).State = EntityState.Modified;
                db.SaveChanges();
            }

            Lichsuhoadon lichSuHoaDon = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hoadonID);
            if (lichSuHoaDon != null)
            {
                lichSuHoaDon.SanLuongTieuThu = 0;
                lichSuHoaDon.ChiSoMoi = 0;
                lichSuHoaDon.SH1 = 0;
                lichSuHoaDon.SH2 = 0;
                lichSuHoaDon.SH3 = 0;
                lichSuHoaDon.SH4 = 0;
                lichSuHoaDon.CC = 0;
                lichSuHoaDon.HC = 0;
                lichSuHoaDon.SX = 0;
                lichSuHoaDon.KD = 0;
                lichSuHoaDon.PhiBVMT = 0;
                lichSuHoaDon.TongCong = 0;
                db.Entry(lichSuHoaDon).State = EntityState.Modified;
                db.SaveChanges();
            }

            // TODO: áp dụng lại dư có cho những tháng sau (nếu có)
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            ViewBag.hasMaKhachHang = false;
            ViewBag.maKhachHang = "";
            ViewBag.selectedMonth = "";
            ViewBag.selectedYear = "";
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection form)
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
                    Lichsuhoadon soTienObj = db.Lichsuhoadons.FirstOrDefault(p => p.HoaDonID == hD.HoadonnuocID);
                    if (soTienObj != null)
                    {
                        ViewData["soTienPhaiNop"] = soTienObj;
                    }
                    ViewData["hoadon"] = hD;
                    ViewData["khachHang"] = khHang;
                    ViewBag.hasMaKhachHang = true;
                }
            }
            ViewBag.maKhachHang = maKH;
            ViewBag.selectedMonth = ThangHoaDon;
            ViewBag.selectedYear = namHoaDon;
            return View();
        }


	}
}