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
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Models;

namespace HoaDonNuocHaDong.Controllers
{
    public class TuyenongController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        // GET: /Tuyenong/
        public ActionResult Index()
        {
            ViewBag.quanHuyen = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            ViewBag.phuongXa = new SelectList(db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null), "PhuongxaID", "Ten");
            var tuyenongs = db.Tuyenongs.Where(p => p.IsDelete == false || p.IsDelete == null).Include(t => t.Captuyen);
            return View(tuyenongs.ToList());
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int quanHuyenID = String.IsNullOrEmpty(form["QuanHuyenID"]) ? 0 : Convert.ToInt32(form["QuanHuyenID"]);
            int phuongXaID = String.IsNullOrEmpty(form["PhuongXaID"]) ? 0 : Convert.ToInt32(form["PhuongXaID"]);

            ViewBag.quanHuyen = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            ViewBag.phuongXa = new SelectList(db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null), "PhuongxaID", "Ten");
            var tuyenongs = db.Tuyenongs.Where(p => p.IsDelete == false || p.IsDelete == null).Where(p => p.QuanHuyenID == quanHuyenID && p.PhuongxaID == phuongXaID).Include(t => t.Captuyen);
            return View(tuyenongs.ToList());
        }

        public ActionResult ViewAsTree()
        {
            var tuyenongs = db.Tuyenongs.Where(p => p.IsDelete == false).Include(t => t.Captuyen);
            return View(tuyenongs.ToList());
        }

        public ActionResult NhapChiSo()
        {
            populateTableChiSoCap(DateTime.Now.Month, DateTime.Now.Year);          
            var tuyenongs = (from i in db.Tuyenongs
                             join r in db.Chisocaps on i.TuyenongID equals r.TuyenongID                             
                             where (i.IsDelete == false || i.IsDelete == null) && (r.Thang.Value == DateTime.Now.Month) && (r.Nam.Value == DateTime.Now.Year)
                             select new HoaDonNuocHaDong.Models.Tuyenong.ChiSoCap
                             {
                                 TuyenOngID = i.TuyenongID,
                                 MaTuyenOng = i.Matuyen,
                                 TenTuyenOng = i.Tentuyen,
                                 CapTuyenOng = i.CaptuyenID == null ? 0 : i.CaptuyenID.Value ,
                                 ChiSoSanLuongTuyenOng = r.Chiso.Value,
                             }).ToList();

            ViewData["quanHuyens"] = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewData["phuongXas"] = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewData["danhsachTuyenOng"] = tuyenongs;
            ViewBag.selectedMonth = "";
            ViewBag.selectedYear = "";
            ViewBag.selectedQuanHuyen = 0;
            ViewBag.selectedPhuongXa = 0;
            return View();
        }

        private void populateTableChiSoCap(int month, int year)
        {
            var tuyenongs = db.Tuyenongs.Where(p=>p.IsDelete == false || p.IsDelete == null).ToList();
            foreach(var item in tuyenongs){
                var chiSoCap = db.Chisocaps.FirstOrDefault(p=>p.TuyenongID == item.TuyenongID && p.Thang == month && p.Nam == year);
                if (chiSoCap == null)
                {
                    Chisocap chiSoCapTheoThangNam = new Chisocap();
                    chiSoCapTheoThangNam.Thang = month;
                    chiSoCapTheoThangNam.Nam = year;
                    chiSoCapTheoThangNam.TuyenongID = item.TuyenongID;
                    chiSoCapTheoThangNam.Chiso = 0;
                    db.Chisocaps.Add(chiSoCapTheoThangNam);                   
                }
                db.SaveChanges();
            }            
        }

        [HttpPost]
        public ActionResult NhapChiSo(FormCollection form)
        {            
            int quanHuyenID = String.IsNullOrEmpty(form["QuanHuyenID"]) ? 0 : Convert.ToInt32(form["QuanHuyenID"]);
            int phuongXaID = String.IsNullOrEmpty(form["PhuongXaID"]) ? 0 : Convert.ToInt32(form["PhuongXaID"]);
            int thangCoChiSo = String.IsNullOrEmpty(form["thangcs"]) ? DateTime.Now.Month : Convert.ToInt32(form["thangcs"]);
            int namCoChiSo = String.IsNullOrEmpty(form["namcs"]) ? DateTime.Now.Month : Convert.ToInt32(form["namcs"]);            
            populateTableChiSoCap(thangCoChiSo, namCoChiSo);

            #region getAllTuyenOngs
            var tuyenongs = (from i in db.Tuyenongs
                             join r in db.Chisocaps on i.TuyenongID equals r.TuyenongID
                             where (i.IsDelete == false || i.IsDelete == null) && (r.Thang == thangCoChiSo) && (r.Nam == namCoChiSo)
                             select new HoaDonNuocHaDong.Models.Tuyenong.ChiSoCap
                             {
                                 TuyenOngID = i.TuyenongID,
                                 MaTuyenOng = i.Matuyen,
                                 TenTuyenOng = i.Tentuyen,
                                 CapTuyenOng = i.CaptuyenID == null ? 0 : i.CaptuyenID.Value,
                                 ChiSoSanLuongTuyenOng = r.Chiso == null ? 0 : r.Chiso.Value,
                             }).ToList();
            #endregion

            #region !quanHuyenID && !phuongXaID
            if (quanHuyenID != 0 && phuongXaID != 0)
            {
                tuyenongs = (from i in db.Tuyenongs
                                 join r in db.Chisocaps on i.TuyenongID equals r.TuyenongID
                                 where (i.IsDelete == false || i.IsDelete == null) && (r.Thang == thangCoChiSo) && (r.Nam == namCoChiSo) && (i.QuanHuyenID == quanHuyenID && i.PhuongxaID == phuongXaID)
                                 select new HoaDonNuocHaDong.Models.Tuyenong.ChiSoCap
                                 {
                                     TuyenOngID = i.TuyenongID,
                                     MaTuyenOng = i.Matuyen,
                                     TenTuyenOng = i.Tentuyen,
                                     CapTuyenOng = i.CaptuyenID == null ? 0 : i.CaptuyenID.Value,
                                     ChiSoSanLuongTuyenOng = r.Chiso == null ? 0 : r.Chiso.Value,
                                 }).ToList();
            }
            #endregion

            #region ViewBag-ViewData
            ViewData["quanHuyens"] = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewData["phuongXas"] = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.selectedQuanHuyen = quanHuyenID;
            ViewBag.selectedPhuongXa = phuongXaID;
            ViewBag.selectedMonth = thangCoChiSo;
            ViewBag.selectedYear = namCoChiSo;
            ViewData["danhsachTuyenOng"] = tuyenongs;
            #endregion

            return View();
        }


        public void NhapSanLuong(String TuyenOngID, String SanLuong, String thang, String nam)
        {
            int tuyenongIdToInt = Convert.ToInt32(TuyenOngID);
            if (!String.IsNullOrEmpty(TuyenOngID))
            {
                var Cthang = DateTime.Now.Month;
                var Cnam = DateTime.Now.Year;
                if (!String.IsNullOrEmpty(thang))
                {
                    Cthang = Convert.ToInt32(thang);
                }
                if (!String.IsNullOrEmpty(nam))
                {
                    Cnam = Convert.ToInt32(nam);
                }

                Chisocap checkChiSoCapTheoThangNam = db.Chisocaps.FirstOrDefault(p => p.Thang == Cthang && p.Nam == Cnam && p.TuyenongID == tuyenongIdToInt);
                if (checkChiSoCapTheoThangNam != null)
                {
                    checkChiSoCapTheoThangNam.Chiso = Convert.ToInt32(SanLuong);
                    db.Entry(checkChiSoCapTheoThangNam).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    Chisocap chiso = new Chisocap();
                    chiso.TuyenongID = tuyenongIdToInt;
                    chiso.Thang = Cthang;
                    chiso.Nam = Cnam;
                    chiso.Chiso = Convert.ToInt32(SanLuong);
                    db.Chisocaps.Add(chiso);
                    db.SaveChanges();
                }
            }
        }

        // GET: /Tuyenong/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuyenong tuyenong = db.Tuyenongs.Find(id);
            if (tuyenong == null)
            {
                return HttpNotFound();
            }
            return View(tuyenong);
        }

        // GET: /Tuyenong/Create
        public ActionResult Create()
        {
            ViewBag.capTuyenOng = db.Captuyens.ToList();
            ViewBag.QuanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.Phuongxa = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.parentID = db.Tuyenongs.Where(p => p.IsDelete == false).ToList();
            ViewBag.CaptuyenID = new SelectList(db.Captuyens, "CaptuyenID", "Ten");
            return View();
        }



        // POST: /Tuyenong/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TuyenongID,TuyenongPID,CaptuyenID,Matuyen,Tentuyen,QuanHuyenID,PhuongxaID,IsDelete")] Tuyenong tuyenong)
        {

            if (ModelState.IsValid)
            {
                Tuyenong firstPlumber = db.Tuyenongs.FirstOrDefault(p => p.Tentuyen == tuyenong.Tentuyen && p.Matuyen == tuyenong.Matuyen);
                if (firstPlumber == null)
                {
                    db.Tuyenongs.Add(tuyenong);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.duplicate = true;
                }
            }
            ViewBag.QuanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.Phuongxa = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.parentID = db.Tuyenongs.Where(p => p.IsDelete == false).ToList();
            ViewBag.CaptuyenID = new SelectList(db.Captuyens, "CaptuyenID", "Ten", tuyenong.CaptuyenID);
            return View(tuyenong);
        }

        // GET: /Tuyenong/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuyenong tuyenong = db.Tuyenongs.Find(id);
            if (tuyenong == null)
            {
                return HttpNotFound();
            }
            ViewBag._QuanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag._selectedQuanHuyen = tuyenong.QuanHuyenID;
            ViewBag._Phuongxa = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag._selectedPhuongXa = tuyenong.PhuongxaID;
            ViewBag._parentID = db.Tuyenongs.Where(p => p.IsDelete == false).ToList();
            ViewBag.selectedPID = tuyenong.TuyenongPID;
            ViewBag.CaptuyenID = new SelectList(db.Captuyens, "CaptuyenID", "Ten", tuyenong.CaptuyenID);

            return View(tuyenong);
        }

        // POST: /Tuyenong/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TuyenongID,TuyenongPID,CaptuyenID,Matuyen,Tentuyen,QuanHuyenID,PhuongxaID,IsDelete")] Tuyenong tuyenong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tuyenong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag._QuanHuyen = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten", tuyenong.QuanHuyenID);
            ViewBag._Phuongxa = new SelectList(db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null), "PhuongxaID", "Ten", tuyenong.PhuongxaID);
            ViewBag.selectedPID = tuyenong.TuyenongPID;
            ViewBag._parentID = db.Tuyenongs.Where(p => p.IsDelete == false).ToList();
            ViewBag._CaptuyenID = new SelectList(db.Captuyens, "CaptuyenID", "Ten", tuyenong.CaptuyenID);
            return View(tuyenong);
        }

        // GET: /Tuyenong/Delete/5
        public ActionResult Delete(int? id)
        {

            Tuyenong tuyenOng = db.Tuyenongs.Find(id);
            tuyenOng.IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static void recursive(int id)
        {

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
