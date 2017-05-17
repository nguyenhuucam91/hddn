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
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        // GET: /Nhanvien/
        public ActionResult Index()
        {
            ViewBag.chiNhanh = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            int phongBanId = getPhongBanNguoiDung();
            List<Nhanvien> nhanviens = new List<Nhanvien>();
            if (phongBanId == 0)
            {
                ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
                nhanviens = db.Nhanviens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            }
            else
            {
                ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId);
                nhanviens = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId).ToList();
            }


            var userLoggedInRole = getLoggedInUserRole();

            #region ViewBag
            ViewBag.userLoggedInRole = userLoggedInRole;
            ViewBag.chucVu = db.Chucvus;
            ViewData["Tuyen"] = db.Tuyenkhachhangs.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedQuan = 0;
            ViewBag.selectedTo = 0;
            ViewBag.phongBan = db.Phongbans.ToList();
            ViewBag.selectedPhongban = 0;
            #endregion
            return View(nhanviens);
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

        public int getPhongBanNguoiDung()
        {
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            if (nhanVien != null)
            {
                var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
                int phongBanID = phongBan.PhongbanID;
                return phongBanID;
            }
            return 0;
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
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.Where(p=>p.IsDelete==false).ToList();
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

            ViewBag._PhongbanID = new SelectList(db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "ToQuanHuyenID", "Ma");
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

            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
            ViewBag.ChinhanhID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            ViewBag.To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            ViewBag.ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten");
            ViewBag._PhongbanID = new SelectList(db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "ToQuanHuyenID", "Ma");
            ViewBag.PhongBanQuanHuyen = new SelectList(db.Phongbans, "PhongbanID", "Ten");
            return View(nhanvien);
        }

        // GET: /Nhanvien/Edit/5
        public ActionResult Edit(int? id)
        {
            String tuyenKHIDList = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nhanvien nhanvien = db.Nhanviens.Find(id);
            if (nhanvien == null)
            {
                return HttpNotFound();
            }
            //lấy tuyến khách hàng của nhân viên
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
            ViewBag.ChinhanhID = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedTuyenKHID = tuyenKHIDList;
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
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
            }
            else
            {
                ViewBag._PhongbanID = new SelectList(db.Phongbans.Where(p => p.PhongbanID == phongBanId), "PhongbanID", "Ten", nhanvien.PhongbanID);
            }

            ViewBag.selectedTo = nhanvien.ToQuanHuyenID;
            ViewBag.selectedQuanHuyen = getQuanHuyenIDFromToID(nhanvien.ToQuanHuyenID);
            int phongbanId = getPhongBanNguoiDung();
            if (phongbanId == 0)
            {
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false);
            }
            else
            {
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongbanId);
            }

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

            ViewBag.ChinhanhID = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedTuyenKHID = tuyenKHIDList;
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
            ViewBag._ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten", nhanvien.ChucvuID);
            ViewBag._PhongbanID = new SelectList(db.Phongbans, "PhongbanID", "Ten", nhanvien.PhongbanID);
            ViewBag.selectedTo = nhanvien.ToQuanHuyenID;
            ViewBag.selectedQuanHuyen = getQuanHuyenIDFromToID(nhanvien.ToQuanHuyenID.Value);
            int phongbanId = getPhongBanNguoiDung();
            if (phongbanId == 0)
            {
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            }
            else
            {
                ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null && p.PhongbanID == phongbanId);
            }
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult FilterNhanVien(FormCollection form)
        {
            int phongBanId = getPhongBanNguoiDung();
            int quan = String.IsNullOrEmpty(form["chinhanh"]) ? 0 : Convert.ToInt32(form["chinhanh"]);
            int to = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);
            int phongBan = String.IsNullOrEmpty(form["phongban"]) ? 0 : Convert.ToInt32(form["phongban"]);
            List<Nhanvien> nhanViens = db.Nhanviens.ToList();
            if (phongBanId == 0)
            {
                if (phongBan != 0)
                {
                    nhanViens = db.Nhanviens.Where(p => p.PhongbanID == phongBan && (p.IsDelete == false || p.IsDelete == null)).ToList();
                }
                else
                {
                    nhanViens = db.Nhanviens.Include(n => n.Chucvu).Include(n => n.Phongban).Where(p => p.IsDelete == false).ToList();
                }
                ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            }
            else
            {
                nhanViens = db.Nhanviens.Where(p => p.PhongbanID == phongBanId).Include(n => n.Chucvu).Include(n => n.Phongban).Where(p => p.IsDelete == false).ToList();
                ViewBag.ToQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanId);
            }

            if (to != 0)
            {
                nhanViens = nhanViens.Where(p => p.ToQuanHuyenID == to).ToList();
            }
            String chucVu = form["chucVu"];
            if (!String.IsNullOrEmpty(chucVu))
            {
                nhanViens = nhanViens.Where(p => p.ChucvuID == Convert.ToInt32(chucVu)).ToList();
            }

            var userLoggedInRole = getLoggedInUserRole();

            #region ViewBag
            ViewBag.userLoggedInRole = userLoggedInRole;
            ViewBag.chiNhanh = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            ViewBag.chucVu = db.Chucvus;
            ViewData["Tuyen"] = db.Tuyenkhachhangs.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedQuan = quan;
            ViewBag.selectedTo = to;
            ViewBag.phongBan = db.Phongbans.ToList();
            ViewBag.selectedPhongBan = phongBan;
            #endregion

            return View("Index", nhanViens);
        }
    }
}
