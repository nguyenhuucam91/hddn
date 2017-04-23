using HoaDonHaDong.Helper;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Models.KhachHang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class KiemdinhController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        KiemDinh kiemDinhHelper = new KiemDinh();
        NguoidungHelper ngHelper = new NguoidungHelper();
        // GET: /Kiemdinh/
        public ActionResult Create()
        {
            ViewBag.result = false;
            return View();
        }

        /// <summary>
        /// Tìm kiếm khách hàng có mã khách hàng nhập từ form cho sẵn
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            #region FormRequest            
            String maKhachHang = String.IsNullOrEmpty(form["maKhachHang"]) ? "0" : form["maKhachHang"];
            String thangKiemDinh = String.IsNullOrEmpty(form["thang"]) ? DateTime.Now.Month.ToString() : form["thang"];
            String namKiemDinh = String.IsNullOrEmpty(form["nam"]) ? DateTime.Now.Year.ToString() : form["nam"];
            int quanHuyenID = Convert.ToInt32(form["quan"]);
            int hoaDonNuocID = 0;

            ViewBag.Thang = thangKiemDinh;
            ViewBag.Nam = namKiemDinh;
            #endregion

            Kiemdinh kD = new Kiemdinh();
            var khachHang = (from i in db.Khachhangs
                             join r in db.Hoadonnuocs on i.KhachhangID equals r.KhachhangID
                             where i.MaKhachHang == maKhachHang && r.ThangHoaDon.ToString() == thangKiemDinh && r.NamHoaDon.ToString() == namKiemDinh
                             select new KhachHangModel
                             {
                                 MaKH = i.MaKhachHang,
                                 KHID = i.KhachhangID,
                                 tenKhachHang = i.Ten,
                                 QuanhuyenID = i.QuanhuyenID.Value,
                                 PhuongxaID = i.PhuongxaID.Value,
                                 CumdancuID = i.CumdancuID.Value,
                                 TuyenKHID = i.TuyenKHID.Value,
                             }).FirstOrDefault();            
            if (khachHang != null)
            {
                maKhachHang = khachHang.MaKH;
                var hoaDonNuoc = db.Hoadonnuocs.FirstOrDefault(p => p.KhachhangID == khachHang.KHID && p.ThangHoaDon.ToString() == thangKiemDinh && p.NamHoaDon.ToString() == namKiemDinh);

                if (hoaDonNuoc != null)
                {
                    hoaDonNuocID = hoaDonNuoc.HoadonnuocID;
                }

                #region ViewBag
                ViewBag.message = null;
                ViewBag.khachHang = khachHang;
                ViewBag.maKH = maKhachHang;
                ViewBag.khachHangID = khachHang.KHID;
                ViewBag.chiSoThangTruoc = ChiSo.getChiSoThang(thangKiemDinh, namKiemDinh, khachHang.KHID);                
                ViewBag.result = true;                                
                ViewBag.HoaDonID = hoaDonNuocID;
                #endregion
                return View(kD);
            }
            else
            {
                ViewBag.message = "Không tìm thấy hóa đơn có mã khách hàng ";
                ViewBag.result = false;
                ViewBag.maKH = maKhachHang;
            }

            return View();
        }

        /// <summary>
        /// Xem danh sách kiểm định trong tháng
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.month = DateTime.Now.Month;
            ViewBag.year = DateTime.Now.Year;
            ViewData["kiemDinh"] = new List<HoaDonNuocHaDong.Models.KhachHang.KiemDinhModel>();
            return View();
        }

        /// <summary>
        /// Lấy danh sách kiểm định của tháng nhất định
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int month = String.IsNullOrEmpty(form["month"].ToString()) ? DateTime.Now.Month : Convert.ToInt32(form["month"]);
            int year = String.IsNullOrEmpty(form["year"].ToString()) ? DateTime.Now.Year : Convert.ToInt32(form["year"]);
            int tuyen = String.IsNullOrEmpty(form["tuyen"]) ? 0 : Convert.ToInt32(form["tuyen"]);
            List<HoaDonNuocHaDong.Models.KhachHang.KiemDinhModel> kiemDinh = kiemDinhHelper.getDanhSachKiemDinh(month, year, tuyen);
            #region ViewData - ViewBag
            ViewBag.month = month;
            ViewBag.year = year;
            #endregion
            ViewData["kiemDinh"] = kiemDinh;
            return View();
        }

        /// <summary>
        /// Cập nhật kiểm định
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            Kiemdinh kiemDinh = db.Kiemdinhs.FirstOrDefault(p => p.KiemdinhID == id);
            Hoadonnuoc hoaDonNuoc = db.Hoadonnuocs.Find(kiemDinh.HoaDonId);
            int kiemDinhKHID = kiemDinh.KhachhangID.Value;         

            ViewData["kiemDinh"] = kiemDinh;
            ViewData["khachHang"] = db.Khachhangs.Find(kiemDinh.KhachhangID);
            ViewBag.chiSoThangTruoc = ChiSo.getChiSoThang(hoaDonNuoc.ThangHoaDon.Value.ToString(), hoaDonNuoc.NamHoaDon.Value.ToString(), hoaDonNuoc.KhachhangID.Value);
            ViewBag.id = id;
            return View(kiemDinh);
        }

        [HttpPost]
        public ActionResult Edit(int? id, FormCollection form)
        {
            Kiemdinh kD = db.Kiemdinhs.Find(id);
            DateTime ngayKiemDinh = Convert.ToDateTime(form["ngayKiemDinh"]);
            if (ngayKiemDinh != null)
            {
                //lấy danh sách attributes của khách hàng
                int khachHangID = int.Parse(form["khachHangID"].ToString());
                String ghiChu = form["ghiChu"];
                int chiSoLucKiemDinh = Convert.ToInt32(form["chiSoTruocKiemDinh"]);
                int chiSoSauKhiKiemDinh = Convert.ToInt32(form["chiSoSauKhiKiemDinh"]);
                //thêm object kiểm định mới

                kD.KhachhangID = khachHangID;
                kD.Ngaykiemdinh = ngayKiemDinh;
                kD.Ghichu = ghiChu;
                kD.Chisoluckiemdinh = chiSoLucKiemDinh;
                kD.Chisosaukiemdinh = chiSoSauKhiKiemDinh;
                db.Entry(kD).State = System.Data.Entity.EntityState.Modified;
                //lưu vào db
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Edit", kD);
            }
        }

        [HttpPost]
        /// <summary>
        /// Thêm mới kiểm định cho khách hàng nào đó.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public ActionResult AddKiemDinh([Bind()] Kiemdinh kD, FormCollection form)
        {
            #region formRequest
                DateTime ngayKiemDinh = Convert.ToDateTime(form["ngayKiemDinh"]);
                String ghiChu = form["ghiChu"];
                int chiSoLucKiemDinh = Convert.ToInt32(form["chiSoTruocKiemDinh"]);
                int chiSoSauKhiKiemDinh = Convert.ToInt32(form["chiSoSauKhiKiemDinh"]);
                int khachHangID = int.Parse(form["khachHangID"].ToString());
                int hoaDonID = int.Parse(form["hoaDonID"]);
            #endregion
            if (ngayKiemDinh != null)
            {                
                //thêm object kiểm định mới
                kD = new Kiemdinh();
                kD.KhachhangID = khachHangID;
                kD.Ngaykiemdinh = ngayKiemDinh;
                kD.Ghichu = ghiChu;
                kD.Chisoluckiemdinh = chiSoLucKiemDinh;
                kD.Chisosaukiemdinh = chiSoSauKhiKiemDinh;
                kD.HoaDonId = hoaDonID;
                db.Kiemdinhs.Add(kD);
                //lưu vào db
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Create", kD);
            }

        }

        /// <summary>
        /// Xóa kiểm định ra khỏi hệ thống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var kiemDinh = db.Kiemdinhs.FirstOrDefault(p => p.KiemdinhID == id);
            if (kiemDinh != null)
            {
                db.Kiemdinhs.Remove(kiemDinh);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


    }
}