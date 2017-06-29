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

namespace HoaDonNuocHaDong.Controllers
{
    public class PhuongxaController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        // GET: /Phuongxa/
        public ActionResult Index()
        {
            var phuongxas = db.Phuongxas.Include(p => p.Quanhuyen);
            ViewBag.quanHuyen = db.Quanhuyens.Where(p => p.IsDelete == null || p.IsDelete == false).ToList();
            return View(phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList());
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            String quanHuyen = form["quanhuyen"];
            var phuongXa = db.Phuongxas.Include(p => p.Quanhuyen).Where(p => p.IsDelete == false || p.IsDelete == null);
            //nếu quận huyện để trống
            if (!String.IsNullOrEmpty(quanHuyen))
            {
                int quanHuyenID = Convert.ToInt32(quanHuyen);
                phuongXa = phuongXa.Where(p => p.QuanhuyenID == quanHuyenID);
            }

            ViewBag.quanHuyen = db.Quanhuyens.Where(p => p.IsDelete == null || p.IsDelete == false).ToList();
            return View(phuongXa.ToList());
        }
        // GET: /Phuongxa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phuongxa phuongxa = db.Phuongxas.Find(id);
            if (phuongxa == null)
            {
                return HttpNotFound();
            }
            return View(phuongxa);
        }

        // GET: /Phuongxa/Create
        public ActionResult Create()
        {
            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            return View();
        }

        // POST: /Phuongxa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PhuongxaID,QuanhuyenID,Ten,IsDelete")] Phuongxa phuongxa)
        {
            phuongxa.IsDelete = false;
            if (ModelState.IsValid)
            {
                db.Phuongxas.Add(phuongxa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            return View(phuongxa);
        }

        // GET: /Phuongxa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phuongxa phuongxa = db.Phuongxas.Find(id);
            if (phuongxa == null)
            {
                return HttpNotFound();
            }

            ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten", phuongxa.QuanhuyenID);
            return View(phuongxa);
        }

        // POST: /Phuongxa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PhuongxaID,QuanhuyenID,Ten,IsDelete")] Phuongxa phuongxa)
        {
            phuongxa.IsDelete = false;
            if (ModelState.IsValid)
            {
                db.Entry(phuongxa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten", phuongxa.QuanhuyenID);
            return View(phuongxa);
        }

        // GET: /Phuongxa/Delete/5
        public ActionResult Delete(int? id)
        {
            Phuongxa phuongxa = db.Phuongxas.Find(id);
            phuongxa.IsDelete = true;
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
