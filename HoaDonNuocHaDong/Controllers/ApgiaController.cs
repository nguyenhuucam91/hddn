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
    public class ApgiaController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        
        // GET: /Apgia/
        public ActionResult Index()
        {
            var apgias = db.Apgias.Include(a => a.Loaiapgia);
            return View(apgias.ToList());
        }

        // GET: /Apgia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apgia apgia = db.Apgias.Find(id);
            if (apgia == null)
            {
                return HttpNotFound();
            }
            return View(apgia);
        }

        // GET: /Apgia/Create
        public ActionResult Create()
        {
            ViewBag.LoaiapgiaID = new SelectList(db.Loaiapgias, "LoaiapgiaID", "Ten");
            return View();
        }

        // POST: /Apgia/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApgiaID,Ten,LoaiapgiaID,Denmuc,Gia")] Apgia apgia)
        {
            if (ModelState.IsValid)
            {
                db.Apgias.Add(apgia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LoaiapgiaID = new SelectList(db.Loaiapgias, "LoaiapgiaID", "Ten", apgia.LoaiapgiaID);
            return View(apgia);
        }

        // GET: /Apgia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apgia apgia = db.Apgias.Find(id);
            if (apgia == null)
            {
                return HttpNotFound();
            }
            ViewBag._LoaiapgiaID = new SelectList(db.Loaiapgias, "LoaiapgiaID", "Ten", apgia.LoaiapgiaID);
            return View(apgia);
        }

        // POST: /Apgia/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApgiaID,Ten,LoaiapgiaID,Denmuc,Gia")] Apgia apgia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(apgia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LoaiapgiaID = new SelectList(db.Loaiapgias, "LoaiapgiaID", "Ten", apgia.LoaiapgiaID);
            return View(apgia);
        }


        /// <summary>
        /// Xóa áp giá dựa theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            Apgia apgia = db.Apgias.Find(id);
            db.Apgias.Remove(apgia);
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
