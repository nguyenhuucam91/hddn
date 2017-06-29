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
    public class ThongbaoController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        // GET: /Thongbao/
        public ActionResult Index()
        {
            return View(db.Thongbaos.OrderByDescending(p=>p.Ngaychinhsua).ToList());
        }

        // GET: /Thongbao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thongbao thongbao = db.Thongbaos.Find(id);
            if (thongbao == null)
            {
                return HttpNotFound();
            }
            return View(thongbao);
        }

        // GET: /Thongbao/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Thongbao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Tieude,Noidung,Nguoitao,Ngaytao,Nguoichinhsua,Ngaychinhsua")] Thongbao thongbao)
        {
            if (ModelState.IsValid)
            {
                int currentLoggedInNguoiDungId = LoggedInUser.NguoidungID;
                thongbao.Nguoitao = currentLoggedInNguoiDungId;
                thongbao.Ngaytao = DateTime.Now;
                thongbao.Nguoichinhsua = currentLoggedInNguoiDungId;
                thongbao.Ngaychinhsua = DateTime.Now;
                db.Thongbaos.Add(thongbao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thongbao);
        }

        // GET: /Thongbao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thongbao thongbao = db.Thongbaos.Find(id);
            if (thongbao == null)
            {
                return HttpNotFound();
            }
            return View(thongbao);
        }

        [HttpPost]
        public ActionResult FilterThongbao(FormCollection form)
        {
            String month = form["month"];
            String year = form["year"];
            int _month = 0, _year = 0;
            if (!String.IsNullOrEmpty(month))
            {
                _month = Convert.ToInt32(month);
            }
            if (!String.IsNullOrEmpty(year))
            {
                _year = Convert.ToInt32(year);
            }
            List<Thongbao> thongBao = db.Thongbaos.Where(p => p.Ngaychinhsua.Value.Month == _month && p.Ngaychinhsua.Value.Year == _year).ToList();
            return View("Index",thongBao);
        }

        // POST: /Thongbao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Tieude,Noidung,Nguoitao,Ngaytao,Nguoichinhsua,Ngaychinhsua")] Thongbao thongbao)
        {
            if (ModelState.IsValid)
            {
                int currentLoggedInNguoiDungId = LoggedInUser.NguoidungID;
                thongbao.Nguoichinhsua = currentLoggedInNguoiDungId;
                thongbao.Ngaychinhsua = DateTime.Now;
                db.Entry(thongbao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thongbao);
        }

        // GET: /Thongbao/Delete/5
        public ActionResult Delete(int? id)
        {            
            Thongbao thongbao = db.Thongbaos.Find(id);
            db.Thongbaos.Remove(thongbao);
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
