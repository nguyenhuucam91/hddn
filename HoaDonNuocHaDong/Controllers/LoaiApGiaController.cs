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
    public class LoaiApGiaController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();       
       
        // GET: /LoaiApGia/
        public ActionResult Index()
        {
            return View(db.Loaiapgias.ToList());
        }

        // GET: /LoaiApGia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loaiapgia loaiapgia = db.Loaiapgias.Find(id);
            if (loaiapgia == null)
            {
                return HttpNotFound();
            }
            return View(loaiapgia);
        }

        // GET: /LoaiApGia/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /LoaiApGia/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="LoaiapgiaID,Ten")] Loaiapgia loaiapgia)
        {
            if (ModelState.IsValid)
            {
                db.Loaiapgias.Add(loaiapgia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(loaiapgia);
        }

        // GET: /LoaiApGia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loaiapgia loaiapgia = db.Loaiapgias.Find(id);
            if (loaiapgia == null)
            {
                return HttpNotFound();
            }
            return View(loaiapgia);
        }

        // POST: /LoaiApGia/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="LoaiapgiaID,Ten")] Loaiapgia loaiapgia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loaiapgia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loaiapgia);
        }

        // GET: /LoaiApGia/Delete/5
        public ActionResult Delete(int? id)
        {
            Loaiapgia loaiapgia = db.Loaiapgias.Find(id);
            db.Loaiapgias.Remove(loaiapgia);
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
