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
    public class PhongbanController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            throw new HttpException(403, "Bạn không được quyền truy cập vào module này");
        }

        // GET: /Phongban/
        public ActionResult Index()
        {
            var phongbans = db.Phongbans.Include(p => p.Chinhanh);            
            return View(phongbans.ToList());
        }

        // GET: /Phongban/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phongban phongban = db.Phongbans.Find(id);
            if (phongban == null)
            {
                return HttpNotFound();
            }
            return View(phongban);
        }

        // GET: /Phongban/Create
        public ActionResult Create()
        {            
            ViewBag.ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten");
            return View();
        }

        // POST: /Phongban/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PhongbanID,ChinhanhID,Ten")] Phongban phongban)
        {
            if (ModelState.IsValid)
            {
                db.Phongbans.Add(phongban);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten", phongban.ChinhanhID);
            return View(phongban);
        }

        // GET: /Phongban/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phongban phongban = db.Phongbans.Find(id);
            if (phongban == null)
            {
                return HttpNotFound();
            }
            ViewBag._ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten", phongban.ChinhanhID);
            return View(phongban);
        }

        // POST: /Phongban/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PhongbanID,ChinhanhID,Ten")] Phongban phongban)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phongban).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChinhanhID = new SelectList(db.Chinhanhs, "ChinhanhID", "Ten", phongban.ChinhanhID);
            return View(phongban);
        }

        // GET: /Phongban/Delete/5
        public ActionResult Delete(int? id)
        {
            Phongban phongban = db.Phongbans.Find(id);
            db.Phongbans.Remove(phongban);
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
