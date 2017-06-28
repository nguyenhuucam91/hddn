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
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;

namespace HoaDonNuocHaDong.Controllers
{
    public class NhanvienController : BaseController
    {
        NhanVienHelper nhanVienHelper = new NhanVienHelper();
        NguoidungHelper ngDungHelper = new NguoidungHelper();
        // GET: /Nhanvien/
        public ActionResult Index()
        {
            int quanHuyenIdLoggedInUser = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0);
            int phongBanId = getPhongBanNguoiDung();
            String isAdminVaTruongPhong = isLoggedUserAdminVaTruongPhong();
            bool isOnlyTruongPhong = ngDungHelper.isNguoiDungLaTruongPhong(LoggedInUser.NhanvienID);

            List<Nhanvien> nhanviens = new List<Nhanvien>();
            if (phongBanId == 0)
            {
                nhanviens = (from i in db.Nhanviens
                             where i.IsDelete == false
                             select new
                             {
                                 nhanvien = i,
                             }).Select(p => p.nhanvien).ToList();
                ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
                ViewBag.chiNhanh = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            }
            else
            {
                nhanviens = (from i in db.Nhanviens
                             join r in db.ToQuanHuyens on i.ToQuanHuyenID equals r.ToQuanHuyenID
                             join s in db.Quanhuyens on r.QuanHuyenID equals s.QuanhuyenID
                             where i.IsDelete == false && i.PhongbanID == phongBanId && s.QuanhuyenID == quanHuyenIdLoggedInUser
                             select new
                             {
                                 nhanvien = i,
                             }).Select(p => p.nhanvien).ToList();
                ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId && p.QuanHuyenID == quanHuyenIdLoggedInUser);
                ViewBag.chiNhanh = db.Quanhuyens.Where(p => p.QuanhuyenID == quanHuyenIdLoggedInUser);
            }

            var userLoggedInRole = getLoggedInUserRole();

            #region ViewBag
            ViewBag.isAdminVaTruongPhong = isAdminVaTruongPhong;
            ViewBag.isAdmin = LoggedInUser.Isadmin == true ? "1" : "0";
            ViewBag.chucVu = db.Chucvus;
            ViewBag.selectedQuan = quanHuyenIdLoggedInUser;
            ViewBag.selectedTo = 0;
            ViewBag.phongBan = db.Phongbans.ToList();
            ViewBag.selectedPhongban = 0;
            ViewBag.isOnlyTruongPhong = isOnlyTruongPhong;

            #endregion
            return View(nhanviens);
        }

        [HttpPost]
        public ActionResult FilterNhanVien(FormCollection form)
        {
            int phongBanId = getPhongBanNguoiDung();
            int quanHuyenIdLoggedInUser = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0);
            String isAdminVaTruongPhong = isLoggedUserAdminVaTruongPhong();
            var isAdmin = LoggedInUser.Isadmin == true ? "1" : "0";

            #region FormRequest
            int selectedQuan = String.IsNullOrEmpty(form["chinhanh"]) ? 0 : Convert.ToInt32(form["chinhanh"]);
            int selectedTo = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            int selectedPhongBan = String.IsNullOrEmpty(form["phongban"]) ? 0 : Convert.ToInt32(form["phongban"]);
            String chucVu = form["chucVu"];
            #endregion

            List<Nhanvien> nhanViens = new List<Nhanvien>();
            if (selectedQuan != 0)
            {
                nhanViens = (from i in db.Nhanviens
                             join r in db.ToQuanHuyens on i.ToQuanHuyenID equals r.ToQuanHuyenID
                             join s in db.Quanhuyens on r.QuanHuyenID equals s.QuanhuyenID
                             where i.IsDelete == false && s.QuanhuyenID == selectedQuan
                             select new
                             {
                                 nhanvien = i,
                             }).Select(p => p.nhanvien).ToList();
            }
            else
            {
                nhanViens = (from i in db.Nhanviens
                             join r in db.ToQuanHuyens on i.ToQuanHuyenID equals r.ToQuanHuyenID
                             join s in db.Quanhuyens on r.QuanHuyenID equals s.QuanhuyenID
                             where i.IsDelete == false
                             select new
                             {
                                 nhanvien = i,
                             }).Select(p => p.nhanvien).ToList();
            }

            if (selectedTo != 0)
            {
                nhanViens = nhanViens.Where(p => p.ToQuanHuyenID == selectedTo && (p.IsDelete == false || p.IsDelete == null)).ToList();
            }

            if (selectedPhongBan != 0)
            {
                nhanViens = nhanViens.Where(p => p.PhongbanID == selectedPhongBan && (p.IsDelete == false || p.IsDelete == null)).ToList();
            }

            if (isAdmin == "1")
            {
                if (selectedQuan == 0)
                {
                    ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false);
                }
                else
                {
                    ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == selectedQuan);
                }
                ViewBag.chiNhanh = db.Quanhuyens.Where(p => p.IsDelete == false);
            }
            else
            {
                ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId && p.QuanHuyenID == quanHuyenIdLoggedInUser);
                ViewBag.chiNhanh = db.Quanhuyens.Where(p => p.QuanhuyenID == quanHuyenIdLoggedInUser);
                nhanViens = nhanViens.Where(p => p.PhongbanID == phongBanId).ToList();
            }

            if (!String.IsNullOrEmpty(chucVu))
            {
                nhanViens = nhanViens.Where(p => p.ChucvuID == Convert.ToInt32(chucVu)).ToList();
            }

            #region ViewBag
            ViewBag.isAdminVaTruongPhong = isAdminVaTruongPhong;
            ViewBag.isAdmin = isAdmin;
            ViewBag.chucVu = db.Chucvus;
            ViewBag.selectedQuan = selectedQuan;
            ViewBag.selectedTo = selectedTo;
            ViewBag.phongBan = db.Phongbans.ToList();
            ViewBag.selectedPhongBan = selectedPhongBan;
            #endregion

            return View("Index", nhanViens);
        }

        public IQueryable<Nhanvien> filterEmployeesByDepartment()
        {
            IQueryable<Nhanvien> nhanviens = db.Nhanviens;
            if (LoggedInUser.Isadmin.Value)
            {
                nhanviens = db.Nhanviens.Where(p => p.IsDelete == false).Include(n => n.Chucvu).Include(n => n.Phongban);
            }
            else
            {
                int phongBanId = getPhongBanNguoiDung();
                switch (phongBanId)
                {
                    case PhongbanHelper.KINHDOANH:
                        nhanviens = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.KINHDOANH).Include(n => n.Chucvu).Include(n => n.Phongban);
                        break;
                    case PhongbanHelper.THUNGAN:
                        nhanviens = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.THUNGAN).Include(n => n.Chucvu).Include(n => n.Phongban);
                        break;
                    default:
                        nhanviens = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == PhongbanHelper.INHOADON).Include(n => n.Chucvu).Include(n => n.Phongban);
                        break;
                }
            }
            return nhanviens;
        }

        /// <summary>
        /// Gán tuyến cho nhân viên được chọn
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AssignTuyen(FormCollection form)
        {
            String selectedNhanVien = form["selectedTuyen"];
            String _selectedTuyen = form["nhanvien"];

            if (_selectedTuyen != null)
            {
                int selectedTuyen = Convert.ToInt32(_selectedTuyen);
                string[] selectedNhanVienArray = selectedNhanVien.Split(',');

                foreach (var item in selectedNhanVienArray)
                {
                    int checkedNhanVien = Convert.ToInt32(item);
                    List<Tuyentheonhanvien> tuyenTheoNhanVien = db.Tuyentheonhanviens.Where(p => p.NhanVienID == checkedNhanVien).ToList();
                    db.Tuyentheonhanviens.RemoveRange(tuyenTheoNhanVien);
                    db.SaveChanges();
                    //re-add lại tuyến theo nhân viên
                    Tuyentheonhanvien tuyenTheoNhanVienMoi = new Tuyentheonhanvien();
                    tuyenTheoNhanVienMoi.NhanVienID = checkedNhanVien;
                    tuyenTheoNhanVienMoi.TuyenKHID = selectedTuyen;
                    db.Tuyentheonhanviens.Add(tuyenTheoNhanVienMoi);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        // GET: /Nhanvien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nhanvien nhanvien = db.Nhanviens.Find(id);
            if (nhanvien == null)
            {
                return HttpNotFound();
            }
            return View(nhanvien);
        }

        // GET: /Nhanvien/Create
        public ActionResult Create()
        {

            ViewBag.ChinhanhID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            int loggedInUserRole = getLoggedInUserRole();
            if (loggedInUserRole == 0 || loggedInUserRole == ChucVuHelper.TRUONGPHONG)
            {
                ViewBag.ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten");
            }
            else
            {
                ViewBag.ChucvuID = new SelectList(db.Chucvus.Where(p => p.ChucvuID == loggedInUserRole), "ChucvuID", "Ten");
            }
            
            int phongBanId = getPhongBanNguoiDung();
            if (phongBanId != 0)
            {
                ViewBag.PhongBanQuanHuyen = new SelectList(db.Phongbans.Where(p => p.PhongbanID == phongBanId), "PhongbanID", "Ten");
                ViewBag.To = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId);
            }
            else
            {
                ViewBag.PhongBanQuanHuyen = new SelectList(db.Phongbans, "PhongbanID", "Ten");
                ViewBag.To = db.ToQuanHuyens.Where(p => p.IsDelete == false);
            }
            ViewBag._TuyenKHID = nhanVienHelper.loadTuyenChuaCoNhanVien();
            ViewBag._PhongbanID = new SelectList(db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "ToQuanHuyenID", "Ma");
            ViewBag.phongBanLoggedInUser = phongBanId;
            return View();
        }

        private int getLoggedInUserRole()
        {
            if (LoggedInUser.NhanvienID != null)
            {
                int nhanvienLoggedInId = LoggedInUser.NhanvienID.Value;
                Nhanvien loggedInUserRoleRecord = db.Nhanviens.Find(nhanvienLoggedInId);
                if (loggedInUserRoleRecord != null)
                {
                    return loggedInUserRoleRecord.ChucvuID.Value;
                }
            }
            return 0;
        }

        // POST: /Nhanvien/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PhongbanID,ChucvuID,Ten,SDT,Diachi,ToQuanHuyenID,IsDelete,MaNhanVien")] Nhanvien nhanvien, FormCollection form)
        {
            String tuyenID = form["TuyenID"];
            if (ModelState.IsValid)
            {
                db.Nhanviens.Add(nhanvien);
                db.SaveChanges();

                int tuyenIDValue = 0;
                if (!String.IsNullOrEmpty(tuyenID))
                {
                    String[] tuyenIDArray = tuyenID.Split(',');
                    foreach (var item in tuyenIDArray)
                    {
                        //nếu là trưởng phòng thì ko cần tuyến ( tuyến ID = 0 || null)
                        tuyenIDValue = Convert.ToInt32(item);
                        Tuyentheonhanvien tt = new Tuyentheonhanvien();
                        tt.TuyenKHID = tuyenIDValue;
                        tt.NhanVienID = nhanvien.NhanvienID;
                        db.Tuyentheonhanviens.Add(tt);
                        db.SaveChanges();
                    }
                }
                //trưởng phòng
                else
                {
                    Tuyentheonhanvien tt = new Tuyentheonhanvien();
                    tt.TuyenKHID = tuyenIDValue;
                    tt.NhanVienID = nhanvien.NhanvienID;
                    db.Tuyentheonhanviens.Add(tt);
                    db.SaveChanges();
                }

                //kiểm tra xem tuyến với nhân viên đó đã có chưa, nếu có thì update
                return RedirectToAction("Index");
            }

            int phongBanId = getPhongBanNguoiDung();
            if (phongBanId != 0)
            {
                ViewBag.PhongBanQuanHuyen = new SelectList(db.Phongbans.Where(p => p.PhongbanID == phongBanId), "PhongbanID", "Ten");
                ViewBag.To = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId);
            }
            else
            {
                ViewBag.PhongBanQuanHuyen = new SelectList(db.Phongbans, "PhongbanID", "Ten");
                ViewBag.To = db.ToQuanHuyens.Where(p => p.IsDelete == false);
            }
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
            ViewBag.ChinhanhID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");           
            ViewBag.ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten");
            ViewBag._PhongbanID = new SelectList(db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "ToQuanHuyenID", "Ma");
            ViewBag.phongBanLoggedInUser = phongBanId;
            return View(nhanvien);
        }

        // GET: /Nhanvien/Edit/5
        public ActionResult Edit(int? id)
        {
            String tuyenKHIDList = "";
            String[] tuyenKH = new String[0];            

            Nhanvien nhanvien = db.Nhanviens.Find(id);
            if (nhanvien == null)
            {
                return HttpNotFound();
            }

            //lấy tuyến khách hàng của nhân viên
            List<Tuyentheonhanvien> kH = db.Tuyentheonhanviens.Where(p => p.NhanVienID == id).ToList();
            if (kH.Count > 0)
            {
                foreach (var item in kH)
                {
                    tuyenKHIDList = tuyenKHIDList + item.TuyenKHID + ",";
                }

                //Bỏ dấu phẩy ở cuối string
                if (!String.IsNullOrEmpty(tuyenKHIDList))
                {
                    tuyenKHIDList = tuyenKHIDList.Remove(tuyenKHIDList.Length - 1);
                    tuyenKH = tuyenKHIDList.Split(',');
                }
            }


            int loggedInRole = getLoggedInUserRole();
            if (loggedInRole == 0 || loggedInRole == ChucVuHelper.TRUONGPHONG)
            {
                ViewBag._ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten", nhanvien.ChucvuID);
            }
            else
            {
                ViewBag._ChucvuID = new SelectList(db.Chucvus.Where(p => p.ChucvuID == loggedInRole), "ChucvuID", "Ten", nhanvien.ChucvuID);
            }

            int phongBanId = getPhongBanNguoiDung();
            if (phongBanId == 0)
            {
                ViewBag._PhongbanID = new SelectList(db.Phongbans, "PhongbanID", "Ten", nhanvien.PhongbanID);
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false);
            }
            else
            {
                ViewBag._PhongbanID = new SelectList(db.Phongbans.Where(p => p.PhongbanID == phongBanId), "PhongbanID", "Ten", nhanvien.PhongbanID);
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId);

            }


            List<Models.TuyenKhachHang.TuyenKhachHang> tuyensChuaCoNhanVien = nhanVienHelper.loadTuyenChuaCoNhanVien();
            List<Models.TuyenKhachHang.TuyenKhachHang> dsTuyenDuocLoad = new List<Models.TuyenKhachHang.TuyenKhachHang>();           

            ViewBag.selectedTo = nhanvien.ToQuanHuyenID;
            ViewBag.selectedQuanHuyen = getQuanHuyenIDFromToID(nhanvien.ToQuanHuyenID);
            ViewBag.ChinhanhID = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedTuyenKHID = tuyenKHIDList;
            ViewBag._TuyenKHID = tuyensChuaCoNhanVien.OrderBy(p => p.MaTuyenKH).ToList();
            ViewBag.phongBanId = phongBanId;
            return View(nhanvien);
        }

        private int? getQuanHuyenIDFromToID(int? toID)
        {
            ToQuanHuyen toQuanHuyen = db.ToQuanHuyens.Find(toID);
            if (toQuanHuyen != null)
            {
                return toQuanHuyen.QuanHuyenID == null ? 0 : toQuanHuyen.QuanHuyenID;
            }
            return 0;
        }

        // POST: /Nhanvien/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NhanvienID,PhongbanID,ChucvuID,Ten,SDT,Diachi,ToQuanHuyenID,IsDelete,MaNhanVien")] Nhanvien nhanvien, FormCollection form, int? id)
        {
            String tuyenKHIDList = "";
            String tuyenID = form["TuyenID"];
            if (ModelState.IsValid)
            {
                db.Entry(nhanvien).State = EntityState.Modified;
                db.SaveChanges();
                //xóa hết record tương ứng với nhân viên đó trong bảng tuyến theo nhân viên
                List<Tuyentheonhanvien> tuyenTheoNV = db.Tuyentheonhanviens.Where(p => p.NhanVienID == nhanvien.NhanvienID).ToList();
                foreach (var item in tuyenTheoNV)
                {
                    db.Tuyentheonhanviens.Remove(item);
                    db.SaveChanges();
                }
                //thêm mới record tương ứng với nhân viên đó
                int tuyenIDValue = 0;
                if (!String.IsNullOrEmpty(tuyenID))
                {
                    String[] tuyenIDArray = tuyenID.Split(',');
                    foreach (var item in tuyenIDArray)
                    {
                        //nếu là trưởng phòng thì ko cần tuyến ( tuyến ID = 0 || null)
                        tuyenIDValue = Convert.ToInt32(item);
                        Tuyentheonhanvien tt = new Tuyentheonhanvien();
                        tt.TuyenKHID = tuyenIDValue;
                        tt.NhanVienID = nhanvien.NhanvienID;
                        db.Tuyentheonhanviens.Add(tt);
                        db.SaveChanges();
                    }
                }
                //trưởng phòng
                else
                {
                    Tuyentheonhanvien tt = new Tuyentheonhanvien();
                    tt.TuyenKHID = tuyenIDValue;
                    tt.NhanVienID = nhanvien.NhanvienID;
                    db.Tuyentheonhanviens.Add(tt);
                    db.SaveChanges();
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //trong trường hợp nhập sai hoặc có validation fail
            List<Tuyentheonhanvien> kH = db.Tuyentheonhanviens.Where(p => p.NhanVienID == id).ToList();
            foreach (var item in kH)
            {
                tuyenKHIDList = tuyenKHIDList + item.TuyenKHID + ",";
            }
            //Bỏ dấu phẩy ở cuối string
            if (!String.IsNullOrEmpty(tuyenKHIDList))
            {
                tuyenKHIDList = tuyenKHIDList.Remove(tuyenKHIDList.Length - 1);
            }

            int phongbanId = getPhongBanNguoiDung();
            if (phongbanId == 0)
            {
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            }
            else
            {
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null && p.PhongbanID == phongbanId);
            }

            ViewBag.ChinhanhID = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedTuyenKHID = tuyenKHIDList;
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
            ViewBag._ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten", nhanvien.ChucvuID);
            ViewBag._PhongbanID = new SelectList(db.Phongbans, "PhongbanID", "Ten", nhanvien.PhongbanID);
            ViewBag.selectedTo = nhanvien.ToQuanHuyenID;
            ViewBag.selectedQuanHuyen = getQuanHuyenIDFromToID(nhanvien.ToQuanHuyenID.Value);
            ViewBag.phongBanId = phongbanId;
            return View(nhanvien);
        }

        // GET: /Nhanvien/Delete/5
        public ActionResult Delete(int? id)
        {
            Nhanvien nhanvien = db.Nhanviens.Find(id);
            nhanvien.IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult quickAssignNhanvienChoTuyen()
        {
            return View("QuickAssign");
        }

        [HttpPost]
        public ActionResult quickAssign(FormCollection form)
        {
            int nhanVienId = String.IsNullOrEmpty(form["nhanvienid"]) ? 0 : Convert.ToInt32(form["nhanvienid"]);
            String[] tuyens = form["tuyen"].Split(',');
            List<Tuyentheonhanvien> tuyenTheoNhanViens = db.Tuyentheonhanviens.Where(p => p.NhanVienID == nhanVienId).ToList();
            db.Tuyentheonhanviens.RemoveRange(tuyenTheoNhanViens);
            foreach (var tuyen in tuyens)
            {
                int tuyenKHID = Convert.ToInt32(tuyen);
                Tuyentheonhanvien tuyenNhanVien = new Tuyentheonhanvien();
                tuyenNhanVien.TuyenKHID = tuyenKHID;
                tuyenNhanVien.NhanVienID = nhanVienId;
                db.Tuyentheonhanviens.Add(tuyenNhanVien);
                db.SaveChanges();
            }
            return RedirectToAction("quickAssignNhanvienChoTuyen");
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
