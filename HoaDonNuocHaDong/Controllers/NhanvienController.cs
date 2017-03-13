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

namespace HoaDonNuocHaDong.Controllers
{
    public class NhanvienController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        // GET: /Nhanvien/
        public ActionResult Index()
        {
            ViewBag.chiNhanh = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            ViewBag.Phongban = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            ViewBag.chucVu = db.Chucvus;
            ViewData["Tuyen"] = db.Tuyenkhachhangs.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            var nhanviens = db.Nhanviens.Where(p => p.IsDelete == false).Include(n => n.Chucvu).Include(n => n.Phongban);
            return View(nhanviens.ToList());
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
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
            ViewBag.ChinhanhID = new SelectList(db.Quanhuyens.Where(p=>p.IsDelete == false||p.IsDelete==null), "QuanhuyenID", "Ten");
            ViewBag.To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            ViewBag.ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten");
            ViewBag._PhongbanID = new SelectList(db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "ToQuanHuyenID", "Ma");
            ViewBag.PhongBanQuanHuyen = new SelectList(db.Phongbans, "PhongbanID", "Ten");
            return View();
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
            ViewBag.ChinhanhID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            ViewBag.selectedTuyenKHID = tuyenKHIDList;
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
            ViewBag._ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten", nhanvien.ChucvuID);
            ViewBag._PhongbanID = new SelectList(db.Phongbans, "PhongbanID", "Ten", nhanvien.PhongbanID);
            ViewBag.selectedTo = nhanvien.ToQuanHuyenID;
            ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);

            return View(nhanvien);
        }

        // POST: /Nhanvien/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NhanvienID,PhongbanID,ChucvuID,Ten,SDT,Diachi,ToQuanHuyenID,IsDelete,MaNhanVien")] Nhanvien nhanvien, FormCollection form,int? id)
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

            ViewBag.ChinhanhID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            ViewBag.selectedTuyenKHID = tuyenKHIDList;
            ViewBag._TuyenKHID = db.Tuyenkhachhangs.ToList();
            ViewBag._ChucvuID = new SelectList(db.Chucvus, "ChucvuID", "Ten", nhanvien.ChucvuID);
            ViewBag._PhongbanID = new SelectList(db.Phongbans, "PhongbanID", "Ten", nhanvien.PhongbanID);
            ViewBag.selectedTo = nhanvien.ToQuanHuyenID;
            ViewBag._To = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
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
            int to = String.IsNullOrEmpty(form["to"]) ? 0 : Convert.ToInt32(form["to"]);

            List<Nhanvien> nhanVien = db.Nhanviens.Include(n => n.Chucvu).Include(n => n.Phongban).Where(p=>p.IsDelete==false).ToList();
            //nếu chi nhánh = "" thì lấy tất
            if (to != 0)
            {
                nhanVien = nhanVien.Where(p => p.ToQuanHuyenID == to).ToList();
            }
            String chucVu = form["chucVu"];
            if (!String.IsNullOrEmpty(chucVu))
            {
                nhanVien = nhanVien.Where(p => p.ChucvuID == Convert.ToInt32(chucVu)).ToList();
            }
           
            ViewBag.chiNhanh = db.Quanhuyens.Where(p=>p.IsDelete==false||p.IsDelete==null);
            ViewBag.Phongban = db.ToQuanHuyens.Where(p => p.IsDelete == false);
            ViewBag.chucVu = db.Chucvus;
            ViewData["Tuyen"] = db.Tuyenkhachhangs.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            return View("Index", nhanVien);
        }
    }
}
