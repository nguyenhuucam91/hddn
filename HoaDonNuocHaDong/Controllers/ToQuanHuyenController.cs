using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class ToQuanHuyenController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        

        // GET: /Chinhanh/
        public ActionResult Index()
        {
            ViewBag.quanHuyen = db.Quanhuyens.Where(p=>p.IsDelete==false||p.IsDelete==null);
            var chinhanhs = db.ToQuanHuyens.Where(p=>p.IsDelete==false||p.IsDelete==null);
            ViewData["phongBan"] = db.Phongbans.ToList();
            return View(chinhanhs.ToList());
        }

        /// <summary>
        /// Lọc chi nhánh
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            String quanHuyen = form["quanhuyen"];
            String phongBan = form["phongBan"];
            var toQuanHuyen = db.ToQuanHuyens.Where(p => p.IsDelete == false || p.IsDelete == null);
            //nếu quận huyện để trống
            if (!String.IsNullOrEmpty(quanHuyen))
            {
                int quanHuyenID = Convert.ToInt32(quanHuyen);
                toQuanHuyen = toQuanHuyen.Where(p => p.QuanHuyenID == quanHuyenID);
            }
            if(!String.IsNullOrEmpty(phongBan)){
                int phongBanInt = Convert.ToInt32(phongBan);
                toQuanHuyen = toQuanHuyen.Where(p=>p.PhongbanID == phongBanInt);
            }
            
            ViewBag.quanHuyen = db.Quanhuyens.Where(p=>p.IsDelete==false||p.IsDelete==null).ToList();
            ViewData["phongBan"] = db.Phongbans.ToList();
            return View(toQuanHuyen.ToList());
        }
        // GET: /Chinhanh/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chinhanh chinhanh = db.Chinhanhs.Find(id);
            if (chinhanh == null)
            {
                return HttpNotFound();
            }
            return View(chinhanh);
        }

        // GET: /Chinhanh/Create
        public ActionResult Create()
        {
            ViewData["phongBan"] = new SelectList(db.Phongbans.ToList(), "PhongbanID", "Ten");
            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            return View();
        }

        // POST: /Chinhanh/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ToQuanHuyenID,Ma,SoCN,QuanHuyenID,IsDelete,PhongbanID")] ToQuanHuyen chinhanh)
        {
            if (ModelState.IsValid)
            {
                db.ToQuanHuyens.Add(chinhanh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["phongBan"] = new SelectList(db.Phongbans.ToList(),"PhongbanID","Ten");

            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten", chinhanh.QuanHuyenID);
            return View(chinhanh);
        }

        // GET: /Chinhanh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToQuanHuyen chinhanh = db.ToQuanHuyens.Find(id);
            if (chinhanh == null)
            {
                return HttpNotFound();
            }
            ViewData["phongBan"] = new SelectList(db.Phongbans.ToList(), "PhongbanID", "Ten",chinhanh.PhongbanID);
            ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten", chinhanh.QuanHuyenID);
            return View(chinhanh);
        }

        // POST: /Chinhanh/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ToQuanHuyenID,Ma,SoCN,QuanHuyenID,IsDelete,PhongbanID")] ToQuanHuyen chinhanh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chinhanh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["phongBan"] = new SelectList(db.Phongbans.ToList(), "PhongbanID", "Ten",chinhanh.PhongbanID);            
            ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten", chinhanh.QuanHuyenID);
            return View(chinhanh);
        }

        // GET: /Chinhanh/Delete/5
        public ActionResult Delete(int? id)
        {
            ToQuanHuyen to = db.ToQuanHuyens.Find(id);
            to.IsDelete = true;
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
    }
}
