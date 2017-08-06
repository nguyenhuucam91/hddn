using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HoaDonNuocHaDong;
using System.Web.Routing;
using System.Data.Entity.Validation;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HDNHD.Models.Constants;
using System.Diagnostics;
using PagedList;
using HoaDonNuocHaDong.Helper;

namespace HoaDonNuocHaDong.Controllers
{
    public class TuyenController : BaseController
    {
        HoaDonNuocHaDong.Helper.Tuyen tuyenHelper = new HoaDonNuocHaDong.Helper.Tuyen();
        NguoidungHelper ngDungHelper = new NguoidungHelper();
        // GET: /Tuyen/
        public ActionResult Index(int? nhanvien, int page = 1)
        {
            int quanHuyenId = getQuanHuyenOfLoggedInUser();
            int phongBanId = getPhongBanNguoiDung();
            bool isOnlyTruongPhong = ngDungHelper.isNguoiDungLaTruongPhong(LoggedInUser.NhanvienID);

            var nhanViens = new List<Nhanvien>();
            if (!LoggedInUser.Isadmin.Value)
            {
                nhanViens = (from i in db.Nhanviens
                             join r in db.ToQuanHuyens on i.ToQuanHuyenID equals r.ToQuanHuyenID
                             join t in db.Quanhuyens on r.QuanHuyenID equals quanHuyenId
                             where i.PhongbanID == phongBanId && i.ChucvuID == (int)EChucVu.NHAN_VIEN && t.QuanhuyenID == quanHuyenId
                             select new
                             {
                                 nhanvien = i
                             }).Select(p => p.nhanvien).ToList();
            }
            else
            {
                nhanViens = db.Nhanviens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            }

            int pageSize = (int)EPaginator.PAGESIZE;
            int pageNumber = page != 0 ? page : 0;
            IEnumerable<Tuyenkhachhang> tuyensKhachHang = tuyenHelper.getDanhSachTuyensByNhanVien(nhanvien).OrderByDescending(p => p.TuyenKHID).Where(p => p.IsDelete == false).ToPagedList(page, pageSize);

            #region ViewBag
            ViewData["nhanVien"] = nhanViens;
            ViewBag.pageSize = pageSize;
            ViewBag.selectedNhanvien = nhanvien;
            ViewBag.isOnlyTruongPhong = isOnlyTruongPhong;
            ViewBag.phongBanId = phongBanId;
            #endregion
            return View(tuyensKhachHang);
        }


        [HttpPost]
        public ActionResult Index(FormCollection form, int? nhanvien, int page = 1)
        {
            String tuKhoaTimKiem = form["tukhoa"].ToLower();

            IEnumerable<Tuyenkhachhang> tuyenkhachhangs = null;
            var nhanVienFilter = new List<Nhanvien>();
            int phongBanId = getPhongBanNguoiDung();
            bool isOnlyTruongPhong = ngDungHelper.isNguoiDungLaTruongPhong(LoggedInUser.NhanvienID);

            //nếu trống thì chọn tất cả
            if (LoggedInUser.Isadmin.Value)
            {
                nhanVienFilter = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
                ViewData["nhanVien"] = nhanVienFilter;
            }
            else
            {
                nhanVienFilter = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId).ToList();
                ViewData["nhanVien"] = nhanVienFilter;
            }

            tuyenkhachhangs = tuyenHelper.getDanhSachTuyensByNhanVien(nhanvien);

            if (!String.IsNullOrEmpty(tuKhoaTimKiem))
            {
                tuyenkhachhangs = tuyenkhachhangs.Where(p => p.Matuyen == tuKhoaTimKiem || p.Ten.Contains(tuKhoaTimKiem));
            }
            int pageSize = (int)EPaginator.PAGESIZE;
            int pageNumber = page != 0 ? page : 0;

            #region ViewBag
            ViewBag.pageSize = pageSize;
            ViewBag.selectedNhanvien = nhanvien;
            ViewBag.isOnlyTruongPhong = isOnlyTruongPhong;
            ViewBag.phongBanId = phongBanId;
            #endregion

            return View(tuyenkhachhangs.OrderByDescending(p => p.TuyenKHID).ToPagedList(pageNumber, pageSize));
        }

        // GET: /Tuyen/Details/5
        public ActionResult Details(int? id)
        {
            Tuyenkhachhang tuyenkhachhang = db.Tuyenkhachhangs.Find(id);
            int loggedInUserDepartment = getPhongBanNguoiDung();
            if (tuyenkhachhang == null || id == null)
            {
                return HttpNotFound();
            }

            var tuyenTheoNhanVien = (from i in db.Tuyentheonhanviens
                                     join r in db.Nhanviens on i.NhanVienID equals r.NhanvienID
                                     where i.TuyenKHID == id && r.PhongbanID == loggedInUserDepartment
                                     select new
                                     {
                                         NhanVienID = i.NhanVienID,
                                     }).FirstOrDefault();
            if (tuyenTheoNhanVien != null)
            {
                ViewBag.nhanVien = db.Nhanviens.Where(p => p.NhanvienID == tuyenTheoNhanVien.NhanVienID).First();
            }
            //nếu ko có thì load ds nhân viên có trong hệ thống ra
            else
            {
                ViewBag.nhanVien = null;
            }

            return View(tuyenkhachhang);
        }

        // GET: /Tuyen/Create
        public ActionResult Create()
        {
            ViewBag.CumdancuID = new SelectList(db.Cumdancus.Where(p => p.IsDelete == false), "CumdancuID", "Ten");
            ViewData["nhanVien"] = db.Nhanviens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.ToID = new SelectList(db.Toes, "ToID", "Ten");
            return View();
        }

        // POST: /Tuyen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Matuyen,TuyenKHID,ToID,CumdancuID,Ten,Diachi,IsDelete")] Tuyenkhachhang tuyenkhachhang)
        {
            tuyenkhachhang.IsDelete = false;
            if (ModelState.IsValid)
            {
                //kiểm tra trong tuyến trước
                Tuyenkhachhang existingTuyen = db.Tuyenkhachhangs.FirstOrDefault(p => p.Matuyen == tuyenkhachhang.Matuyen && p.IsDelete == false);
                if (existingTuyen == null)
                {
                    db.Tuyenkhachhangs.Add(tuyenkhachhang);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.hasTuyen = true;
                }
            }
            ViewData["nhanVien"] = db.Nhanviens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.CumdancuID = new SelectList(db.Cumdancus.Where(p => p.IsDelete == false), "CumdancuID", "Ten");
            ViewBag.ToID = new SelectList(db.Toes, "ToID", "Ten", tuyenkhachhang.ToID);
            return View(tuyenkhachhang);
        }

        // GET: /Tuyen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuyenkhachhang tuyenkhachhang = db.Tuyenkhachhangs.Find(id);
            Tuyentheonhanvien tuyenTheoNhanVien = db.Tuyentheonhanviens.FirstOrDefault(p => p.TuyenKHID == id);
            if (tuyenTheoNhanVien != null)
            {
                ViewBag.selectedNhanVien = tuyenTheoNhanVien.NhanVienID;
                ViewBag._nhanVien = db.Nhanviens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            }
            //nếu ko có thì load tất cả ra
            else
            {
                ViewBag._nhanVien = db.Nhanviens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            }
            if (tuyenkhachhang == null)
            {
                return HttpNotFound();
            }
            return View(tuyenkhachhang);
        }

        // POST: /Tuyen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Matuyen,TuyenKHID,ToID,CumdancuID,Ten,Diachi,IsDelete")] Tuyenkhachhang tuyenkhachhang)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            Tuyenkhachhang tuyen = _db.Tuyenkhachhangs.Find(tuyenkhachhang.TuyenKHID);
            if (ModelState.IsValid)
            {

                Tuyenkhachhang existingTuyen = db.Tuyenkhachhangs.AsNoTracking().FirstOrDefault(p => p.Matuyen == tuyenkhachhang.Matuyen && p.IsDelete == false);
                if (existingTuyen == null || tuyen.Matuyen == tuyenkhachhang.Matuyen)
                {
                    db.Tuyenkhachhangs.AsNoTracking();
                    db.Tuyenkhachhangs.Attach(tuyenkhachhang);
                    db.Entry(tuyenkhachhang).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.hasTuyen = "Mã tuyến đã tồn tại trong cơ sở dữ liệu";
                }

            }

            return View(tuyenkhachhang);
        }

        // GET: /Tuyen/Delete/5
        public ActionResult Delete(int? id)
        {
            db.Tuyenkhachhangs.Find(id.Value).IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AssignEmployee(FormCollection form)
        {
            String selectedTuyen = form["selectedTuyen"];
            String _selectedNhanVien = form["nhanvien"];

            if (!String.IsNullOrEmpty(_selectedNhanVien))
            {
                int selectedNhanVien = Convert.ToInt32(_selectedNhanVien);
                string[] selectedTuyenArray = selectedTuyen.Split(',');

                foreach (var item in selectedTuyenArray)
                {
                    int checkedTuyen = Convert.ToInt32(item);
                    List<Tuyentheonhanvien> tuyenTheoNhanVien = db.Tuyentheonhanviens.Where(p => p.TuyenKHID == checkedTuyen).ToList();
                    db.Tuyentheonhanviens.RemoveRange(tuyenTheoNhanVien);
                    db.SaveChanges();
                    //re-add lại tuyến theo nhân viên
                    Tuyentheonhanvien tuyenTheoNhanVienMoi = new Tuyentheonhanvien();
                    tuyenTheoNhanVienMoi.NhanVienID = selectedNhanVien;
                    tuyenTheoNhanVienMoi.TuyenKHID = checkedTuyen;
                    db.Tuyentheonhanviens.Add(tuyenTheoNhanVienMoi);
                    db.SaveChanges();

                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
