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
    public class HoadonController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        
        public ActionResult Index()
        {
            var hoadonnuocs = db.Hoadonnuocs.Include(h => h.Khachhang).Include(h => h.Nhanvien);
            return View(hoadonnuocs.ToList());
        }

        // GET: /Hoadon/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            if (hoadonnuoc == null)
            {
                return HttpNotFound();
            }
            return View(hoadonnuoc);
        }

        // GET: /Hoadon/Create
        public ActionResult Create()
        {
            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan");
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten");
            return View();
        }

        // POST: /Hoadon/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="HoadonnuocID,Ngayhoadown,KhachhangID,NhanvienID,Sohoadon,Kyhieu,Tongsotieuthu,Trangthaithu,Trangthaiin")] Hoadonnuoc hoadonnuoc)
        {
            if (ModelState.IsValid)
            {
                db.Hoadonnuocs.Add(hoadonnuoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan", hoadonnuoc.KhachhangID);
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten", hoadonnuoc.NhanvienID);
            return View(hoadonnuoc);
        }

        // GET: /Hoadon/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            if (hoadonnuoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan", hoadonnuoc.KhachhangID);
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten", hoadonnuoc.NhanvienID);
            return View(hoadonnuoc);
        }

        // POST: /Hoadon/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="HoadonnuocID,Ngayhoadown,KhachhangID,NhanvienID,Sohoadon,Kyhieu,Tongsotieuthu,Trangthaithu,Trangthaiin")] Hoadonnuoc hoadonnuoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoadonnuoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KhachhangID = new SelectList(db.Khachhangs, "KhachhangID", "Sotaikhoan", hoadonnuoc.KhachhangID);
            ViewBag.NhanvienID = new SelectList(db.Nhanviens, "NhanvienID", "Ten", hoadonnuoc.NhanvienID);
            return View(hoadonnuoc);
        }

        // GET: /Hoadon/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            if (hoadonnuoc == null)
            {
                return HttpNotFound();
            }
            return View(hoadonnuoc);
        }

        // POST: /Hoadon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hoadonnuoc hoadonnuoc = db.Hoadonnuocs.Find(id);
            db.Hoadonnuocs.Remove(hoadonnuoc);
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
