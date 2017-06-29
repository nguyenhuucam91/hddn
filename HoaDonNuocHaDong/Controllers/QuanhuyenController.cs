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
    public class QuanhuyenController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();


        // GET: /Quanhuyen/
        public ActionResult Index()
        {
            return View(db.Quanhuyens.ToList().Where(p=>p.IsDelete==null||p.IsDelete==false));
        }

        // GET: /Quanhuyen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quanhuyen quanhuyen = db.Quanhuyens.Find(id);
            if (quanhuyen == null)
            {
                return HttpNotFound();
            }
            return View(quanhuyen);
        }

        // GET: /Quanhuyen/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Quanhuyen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="QuanhuyenID,Ten,DienThoai,DienThoai2,DienThoai3,IsDelete")] Quanhuyen quanhuyen)
        {
            quanhuyen.IsDelete = false;
            if (ModelState.IsValid)
            {
                quanhuyen.Ten = quanhuyen.Ten.Trim();
                db.Quanhuyens.Add(quanhuyen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quanhuyen);
        }

        // GET: /Quanhuyen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quanhuyen quanhuyen = db.Quanhuyens.Find(id);
            if (quanhuyen == null)
            {
                return HttpNotFound();
            }
            return View(quanhuyen);
        }

        // POST: /Quanhuyen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuanhuyenID,Ten,DienThoai,DienThoai2,DienThoai3,IsDelete")] Quanhuyen quanhuyen)
        {
            quanhuyen.IsDelete = false;
            if (ModelState.IsValid)
            {
                db.Entry(quanhuyen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quanhuyen);
        }

        // GET: /Quanhuyen/Delete/5
        public ActionResult Delete(int? id)
        {
            Quanhuyen quanhuyen = db.Quanhuyens.Find(id);
            quanhuyen.IsDelete = true;
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
