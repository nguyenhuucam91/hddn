using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HoaDonNuocHaDong;
using HoaDonNuocHaDong.Helper;
using System.Web.Routing;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;

namespace HoaDonNuocHaDong.Controllers
{
    public class NguoidungController : BaseController
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        public ActionResult Index()
        {                        
            ViewBag.chinhanh = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();
            ViewBag.phongBan = db.Phongbans.ToList();
            ViewData["toQuanHuyens"] = db.ToQuanHuyens.Where(p => p.IsDelete == false).ToList();
            int phongBanId = getPhongBanNguoiDung();
            var nguoidungs = new List<Nguoidung>();
            if (phongBanId != 0)
            {
                nguoidungs = (from i in db.Nguoidungs
                                  join r in db.Nhanviens on i.NhanvienID equals r.NhanvienID
                                  where r.PhongbanID == phongBanId
                                  select new
                                  {
                                      nguoiDung = i
                                  }).Select(p => p.nguoiDung).ToList();
            }
            else
            {
                nguoidungs = db.Nguoidungs.ToList();
            }
            ViewBag.isAdmin = LoggedInUser.Isadmin.Value == true ? "" : null;
            return View(nguoidungs.ToList());
        }

        public int getPhongBanNguoiDung()
        {
            var phongBanRepository = uow.Repository<PhongBanRepository>();
            if (nhanVien != null)
            {
                var phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);
                int phongBanID = phongBan.PhongbanID;
                return phongBanID;
            }
            return 0;
        }

        [HttpPost, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult Index(FormCollection form)
        {
            String chiNhanh = form["chinhanh"];
            String phongBan = form["phongBan"];
            int toQH = String.IsNullOrEmpty(form["toQuanHuyen"]) ? 0 : Convert.ToInt32(form["toQuanHuyen"]);
            String isAdmin = form["isAdmin"];
            IEnumerable<Nguoidung> nguoiDung = db.Nguoidungs;
            if (!String.IsNullOrEmpty(chiNhanh))
            {
                int chiNhanhID = Convert.ToInt32(chiNhanh);
                var nguoiDungChiNhanh = (from i in db.Nguoidungs
                                         join r in db.Nhanviens on i.NhanvienID equals r.NhanvienID
                                         join s in db.ToQuanHuyens on r.ToQuanHuyenID equals s.ToQuanHuyenID
                                         join t in db.Quanhuyens on s.QuanHuyenID equals t.QuanhuyenID
                                         where t.QuanhuyenID == chiNhanhID
                                         select new
                                         {
                                             nguoiDung = i
                                         });
                nguoiDung = nguoiDungChiNhanh.Select(p => p.nguoiDung).Distinct().ToList();

            }
            //nếu phòng ban không để trông thì lọc theo phòng ban của ng dùng
            if (!String.IsNullOrEmpty(phongBan))
            {
                int phongBanID = Convert.ToInt32(phongBan);
                var nguoiDungPhongBan = (from i in nguoiDung
                                         join r in db.Nhanviens on i.NhanvienID equals r.NhanvienID
                                         join s in db.ToQuanHuyens on r.PhongbanID equals s.PhongbanID
                                         where r.PhongbanID == phongBanID
                                         select new
                                         {
                                             nguoiDung = i
                                         });
                nguoiDung = nguoiDungPhongBan.Select(p => p.nguoiDung).Distinct().ToList();
            }

            //nếu là admin
            if (!String.IsNullOrEmpty(isAdmin))
            {
                bool isAdminValue = Convert.ToInt32(isAdmin) == 1 ? true : false;
                nguoiDung = nguoiDung.Where(p => p.Isadmin == isAdminValue).Distinct().ToList();
            }

            ViewBag.chinhanh = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();
            ViewBag.phongBan = db.Phongbans.ToList();
            ViewData["toQuanHuyens"] = db.ToQuanHuyens.Where(p => p.IsDelete == false).ToList();
            return View(nguoiDung.OrderByDescending(p => p.NguoidungID).ToList());
        }

        // GET: /Nguoidung/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            return View(nguoidung);
        }

        // GET: /Nguoidung/Create
        public ActionResult Create()
        {
            ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            return View();
        }

        // POST: /Nguoidung/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NguoidungID,NhanvienID,Taikhoan,Matkhau,Isadmin")] Nguoidung nguoidung, FormCollection form)
        {
            String repeatMK = form["RepeatMatKhau"];
            //lấy giá trị checkbox
            String isAdmin = form["isAdmin"];

            //nếu trùng mật khâu nhập đi vs mật khẩu nhập lại thì mới add record, nếu sai thì đưa ra thông báo trùng
            if (repeatMK.Equals(nguoidung.Matkhau))
            {
                //kiểm tra xem trong CSDL đã có người nào trùng tên hay chưa

                String matKhau = nguoidung.Matkhau;
                String firstHash = String.Concat(UserInfo.CreateMD5(matKhau).ToLower(), matKhau);
                String md5MatKhau = UserInfo.CreateMD5(firstHash).ToLower();
                //lưu record người dùng vào CSDL
                nguoidung.Matkhau = md5MatKhau;
                if (Convert.ToInt32(isAdmin) == 1)
                {
                    nguoidung.Isadmin = true;
                    nguoidung.NhanvienID = null;
                }
                else
                {
                    nguoidung.Isadmin = false;
                }

                Nguoidung ngDung = db.Nguoidungs.FirstOrDefault(p => p.Taikhoan == nguoidung.Taikhoan && p.Matkhau == nguoidung.Matkhau);
                if (ngDung == null)
                {
                    db.Nguoidungs.Add(nguoidung);
                    db.SaveChanges();
                    //lưu vào thông tin đăng nhập
                    DateTime? nullDateTime = null;
                    Dangnhap dangNhap = new Dangnhap();
                    dangNhap.NguoidungID = nguoidung.NguoidungID;
                    dangNhap.Solandangnhapsai = 0;
                    dangNhap.Thoigiandangnhap = nullDateTime;
                    dangNhap.Trangthaikhoa = false;
                    dangNhap.Thoigianhethankhoa = nullDateTime;
                    db.Dangnhaps.Add(dangNhap);
                    db.SaveChanges();
                    //điều hướng về trang chủ
                    return RedirectToAction("Index");
                }
                //nếu có ng dùng trong CSDL
                else
                {
                    ViewBag.isDuplicate = true;
                }

            }
            else
            {
                ViewBag.passwordMesg = "Mật khẩu cũ và mật khẩu mới phải trùng nhau";
            }

            ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            return View(nguoidung);
        }

        // GET: /Nguoidung/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            ViewBag.selectedNhanVien = nguoidung.NhanvienID;
            ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            ViewBag.isAdmin = LoggedInUser.Isadmin.Value == true ? "" : null;

            return View(nguoidung);
        }

        // POST: /Nguoidung/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NguoidungID,NhanvienID,Taikhoan,Matkhau")] Nguoidung nguoidung, FormCollection form)
        {

            String repeatMK = form["RepeatMatKhau"];
            //lấy giá trị checkbox
            String isAdmin = form["isAdmin"];

            //nếu trùng mật khâu nhập đi vs mật khẩu nhập lại thì mới add record, nếu sai thì đưa ra thông báo trùng
            if (repeatMK.Equals(nguoidung.Matkhau))
            {
                if (ModelState.IsValid)
                {
                    String matKhau = nguoidung.Matkhau;
                    String firstHash = String.Concat(UserInfo.CreateMD5(matKhau).ToLower(), matKhau);
                    nguoidung.Matkhau = UserInfo.CreateMD5(firstHash).ToLower();
                    //lưu record người dùng vào CSDL                
                    if (Convert.ToInt32(isAdmin) == 1)
                    {
                        nguoidung.Isadmin = true;
                    }
                    else
                    {
                        nguoidung.Isadmin = false;
                    }
                    db.Entry(nguoidung).State = EntityState.Modified;
                    //lưu thay đổi vào hệ thống
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.passwordMesg = "Mật khẩu cũ và mật khẩu mới phải trùng nhau";
            }

            ViewBag.selectedNhanVien = nguoidung.NhanvienID;
            ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            return View(nguoidung);
        }

        // GET: /Nguoidung/Delete/5
        public ActionResult Delete(int? id)
        {
            //xóa bảng đăng nhập 
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            int ngDungID = nguoidung.NguoidungID;
            Dangnhap dN = db.Dangnhaps.FirstOrDefault(p => p.NguoidungID == ngDungID);
            //xóa record đăng nhập
            db.Dangnhaps.Remove(dN);
            db.Nguoidungs.Remove(nguoidung);
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

        /// <summary>
        /// Đặt người dùng làm admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetAsAdmin(int id)
        {
            Nguoidung ngDung = db.Nguoidungs.Find(id);
            ngDung.Isadmin = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Đặt người dùng làm admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveAdmin(int id)
        {
            Nguoidung ngDung = db.Nguoidungs.Find(id);
            ngDung.Isadmin = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Mở khóa tài khoản ra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public void unlockAccount(int id)
        {
            Dangnhap _login = db.Dangnhaps.FirstOrDefault(p => p.NguoidungID == id);
            if (_login != null)
            {
                _login.Trangthaikhoa = false;
                _login.Solandangnhapsai = 0;
                _login.Thoigianhethankhoa = null;
                db.Entry(_login).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
