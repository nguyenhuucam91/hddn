using HoaDonNuocHaDong.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class CumdancuController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();        

        // GET: Cumdancu
        public ActionResult Index()
        {
            List<Cumdancu> cumDanCu = db.Cumdancus.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.quanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.phuongXa = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            return View(cumDanCu);
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int quanHuyenID = form["quanhuyen"] == null ? 0 : Convert.ToInt32(form["quanhuyen"]);
            int phuongXaID = form["phuongxa"] == null ? 0 : Convert.ToInt32(form["phuongxa"]);
            List<Cumdancu> cumDanCu = new List<Cumdancu>();
            if (phuongXaID != 0)
            {
                cumDanCu = db.Cumdancus.Where(p => p.IsDelete == false || p.IsDelete == null).Where(p => p.PhuongxaID == phuongXaID).ToList();
            }
            else
            {
                cumDanCu = db.Cumdancus.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            }
            ViewBag.quanHuyen = db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            ViewBag.phuongXa = db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null).ToList();
            return View(cumDanCu);
        }

        // GET: Cumdancu/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Cumdancu/Create
        public ActionResult Create()
        {
            ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            ViewBag.PhuongXaID = new SelectList(db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null), "PhuongXaID", "Ten");
            return View();
        }

        // POST: Cumdancu/Create
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            try
            {
                int quanHuyenID = form["QuanHuyenID"] == null ? 0 : Convert.ToInt32(form["QuanHuyenID"]);
                int phuongXaID = form["PhuongXaID"] == null ? 0 : Convert.ToInt32(form["PhuongXaID"]);
                String name = form["Ten"] == null ? "" : form["Ten"].ToString();
                // TODO: Add insert logic here
                Cumdancu cumdancu = new Cumdancu();
                cumdancu.Ten = name;
                cumdancu.PhuongxaID = phuongXaID;
                cumdancu.IsDelete = false;
                db.Cumdancus.Add(cumdancu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
                ViewBag.PhuongXaID = new SelectList(db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null), "PhuongXaID", "Ten");
                return View();
            }
        }

        // GET: Cumdancu/Edit/5
        public ActionResult Edit(int id)
        {
            Cumdancu cumdancu = db.Cumdancus.Find(id);
            ViewBag._QuanhuyenID = new SelectList(db.Quanhuyens.Where(p => p.IsDelete == false || p.IsDelete == null), "QuanhuyenID", "Ten");
            ViewBag._PhuongXaID = new SelectList(db.Phuongxas.Where(p => p.IsDelete == false || p.IsDelete == null), "PhuongXaID", "Ten",cumdancu.PhuongxaID);
            return View(cumdancu);
        }

        // POST: Cumdancu/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection form)
        {
            try
            {
                // TODO: Add update logic here
                int quanHuyenID = form["QuanHuyenID"] == null ? 0 : Convert.ToInt32(form["QuanHuyenID"]);
                int phuongXaID = form["PhuongXaID"] == null ? 0 : Convert.ToInt32(form["PhuongXaID"]);
                String name = form["Ten"] == null ? "" : form["Ten"].ToString();
                // TODO: Add insert logic here
                Cumdancu cumdancu = db.Cumdancus.Find(id);
                cumdancu.Ten = name;
                cumdancu.PhuongxaID = phuongXaID;
                cumdancu.IsDelete = false;
                db.Entry(cumdancu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Cumdancu/Delete/5
        public ActionResult Delete(int id)
        {
            Cumdancu cumDanCu = db.Cumdancus.Find(id);
            cumDanCu.IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Cumdancu/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
