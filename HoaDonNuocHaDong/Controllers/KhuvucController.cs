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
    public class KhuvucController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        // GET: /Khuvuc/
        

        public ActionResult Index()
        {
            var toes = db.Toes.Include(t => t.Chinhanh).Include(t => t.Phuongxa);
            return View(toes.ToList());
        }

        // GET: /Khuvuc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            To to = db.Toes.Find(id);
            if (to == null)
            {
                return HttpNotFound();
            }
            return View(to);
        }

        // GET: /Khuvuc/Create
        public ActionResult Create()
        {
            ViewBag.ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten");
            ViewBag.PhuongxaID = new SelectList(db.Phuongxas, "PhuongxaID", "Ten");
            return View();
        }

        // POST: /Khuvuc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ToID,ChinhanhID,PhuongxaID,Ten,Diachi")] To to)
        {
            if (ModelState.IsValid)
            {
                db.Toes.Add(to);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten", to.ChinhanhID);
            ViewBag.PhuongxaID = new SelectList(db.Phuongxas, "PhuongxaID", "Ten", to.PhuongxaID);
            return View(to);
        }

        // GET: /Khuvuc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            To to = db.Toes.Find(id);
            if (to == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten", to.ChinhanhID);
            ViewBag.PhuongxaID = new SelectList(db.Phuongxas, "PhuongxaID", "Ten", to.PhuongxaID);
            return View(to);
        }

        // POST: /Khuvuc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ToID,ChinhanhID,PhuongxaID,Ten,Diachi")] To to)
        {
            if (ModelState.IsValid)
            {
                db.Entry(to).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten", to.ChinhanhID);
            ViewBag.PhuongxaID = new SelectList(db.Phuongxas, "PhuongxaID", "Ten", to.PhuongxaID);
            return View(to);
        }

        // GET: /Khuvuc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            To to = db.Toes.Find(id);
            if (to == null)
            {
                return HttpNotFound();
            }
            return View(to);
        }

        // POST: /Khuvuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            To to = db.Toes.Find(id);
            db.Toes.Remove(to);
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
