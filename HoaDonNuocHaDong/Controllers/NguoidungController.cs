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
using System.Data.SqlClient;
using HDNHD.Models.Constants;

namespace HoaDonNuocHaDong.Controllers
{
    public class NguoidungController : BaseController
    {
        public ActionResult Index()
        {
            int phongBanId = getPhongBanNguoiDung();
            int loggedInUserQuanHuyenId = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0);
            var nguoidungs = new List<Nguoidung>();
            String isAdminVaTruongPhong = isLoggedUserAdminVaTruongPhong();
            if (phongBanId != 0)
            {
                nguoidungs = (from i in db.Nguoidungs
                              join r in db.Nhanviens on i.NhanvienID equals r.NhanvienID
                              join s in db.ToQuanHuyens on r.ToQuanHuyenID equals s.ToQuanHuyenID
                              join t in db.Quanhuyens on s.QuanHuyenID equals t.QuanhuyenID
                              where r.PhongbanID == phongBanId && i.Isadmin == false && t.QuanhuyenID == loggedInUserQuanHuyenId
                              select new
                              {
                                  nguoiDung = i,
                                  nhanvien = r
                              }).Select(p => p.nguoiDung).ToList();
                ViewBag.phongBan = db.Phongbans.Where(p => p.PhongbanID == phongBanId).ToList();
            }
            else
            {
                nguoidungs = db.Nguoidungs.ToList();
                ViewBag.phongBan = db.Phongbans.ToList();
            }

            #region ViewBag
            ViewBag.isAdminVaTruongPhong = isAdminVaTruongPhong;
            ViewBag.isAdmin = LoggedInUser.Isadmin == true ? "1" : "0";
            ViewData["toQuanHuyens"] = db.ToQuanHuyens.Where(p => p.IsDelete == false).ToList();
            ViewBag.loggedInUserQuanHuyenId = loggedInUserQuanHuyenId;
            ViewBag.currentlyLoggedInUser = LoggedInUser.NguoidungID;
            ViewBag.chinhanh = db.Quanhuyens.Where(p => p.IsDelete == false).ToList();
            #endregion
            return View(nguoidungs.ToList());
        }

        [HttpPost, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult Index(FormCollection form)
        {
            String chiNhanh = form["chinhanh"];
            String phongBan = form["phongBan"];
            int toQH = String.IsNullOrEmpty(form["toQuanHuyen"]) ? 0 : Convert.ToInt32(form["toQuanHuyen"]);
            String isAdmin = form["isAdmin"];
            IEnumerable<Nguoidung> nguoiDung = db.Nguoidungs;
            int phongBanNguoiDung = getPhongBanNguoiDung();
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
                                             nguoiDung = i,
                                             phongBanID = r.PhongbanID
                                         });
                if (phongBanNguoiDung == 0)
                {
                    nguoiDung = nguoiDungChiNhanh.Select(p => p.nguoiDung).Distinct().ToList();
                }
                else
                {
                    nguoiDung = nguoiDungChiNhanh.Where(p => p.phongBanID == phongBanNguoiDung).Select(p => p.nguoiDung).Distinct().ToList();
                }
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
            int phongbanId = getPhongBanNguoiDung();
            if (phongbanId == 0)
            {
                ViewBag.phongBan = db.Phongbans.ToList();
            }
            else
            {
                ViewBag.phongBan = db.Phongbans.Where(p => p.PhongbanID == phongbanId).ToList();
            }

            #region ViewBag
            ViewData["toQuanHuyens"] = db.ToQuanHuyens.Where(p => p.IsDelete == false).ToList();
            int loggedInUserQuanHuyenId = (int)NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0);
            ViewBag.loggedInUserQuanHuyenId = loggedInUserQuanHuyenId;
            ViewBag.isAdminVaTruongPhong = isLoggedUserAdminVaTruongPhong();
            ViewBag.currentlyLoggedInUser = LoggedInUser.NguoidungID;
            ViewBag.isAdmin = LoggedInUser.Isadmin == true ? "1" : "0";
            #endregion
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
            int isAdmin = LoggedInUser.Isadmin.Value == true ? 1 : 0;
            int phongBanID = getPhongBanNguoiDung();
            if (phongBanID == 0)
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            }
            else
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanID).ToList();
            }
            ViewBag.isAdmin = isAdmin;
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
            String matKhau = form["MatKhau"];
            //lấy giá trị checkbox
            String isAdmin = form["isAdmin"];
           
            if (matKhau == "")
            {
                ViewBag.passwordMesg = "Mật khẩu không được để trống";
            }
            else
            {
                if (repeatMK == matKhau)
                {
                    String firstHash = String.Concat(UserInfo.CreateMD5(matKhau).ToLower(), matKhau);
                    String md5MatKhau = UserInfo.CreateMD5(firstHash).ToLower();
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

                    Nguoidung ngDung = db.Nguoidungs.FirstOrDefault(p => p.Taikhoan == nguoidung.Taikhoan);
                    if (ngDung == null)
                    {
                        db.Nguoidungs.Add(nguoidung);
                        db.SaveChanges();
                        DateTime? nullDateTime = null;
                        Dangnhap dangNhap = new Dangnhap();
                        dangNhap.NguoidungID = nguoidung.NguoidungID;
                        dangNhap.Solandangnhapsai = 0;
                        dangNhap.Thoigiandangnhap = nullDateTime;
                        dangNhap.Trangthaikhoa = false;
                        dangNhap.Thoigianhethankhoa = nullDateTime;
                        db.Dangnhaps.Add(dangNhap);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.isDuplicate = "Người dùng này đã có trong cơ sở dữ liệu";
                    }

                }
                else
                {
                    ViewBag.passwordMesg = "Mật khẩu cũ và mật khẩu mới phải trùng nhau";
                }

            }           

            int phongBanID = getPhongBanNguoiDung();
            if (phongBanID == 0)
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            }
            else
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanID).ToList();
            }
            ViewBag.isAdmin = LoggedInUser.Isadmin.Value == true ? 1 : 0;
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
            int phongBanID = getPhongBanNguoiDung();
            if (phongBanID == 0)
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            }
            else
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanID).ToList();
            }
            ViewBag.isAdmin = LoggedInUser.Isadmin.Value == true ? "" : null;

            return View(nguoidung);
        }

        // POST: /Nguoidung/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NguoidungID,NhanvienID,Taikhoan,Matkhau")] Nguoidung nguoidung, int id, FormCollection form)
        {
            int nhanvienID = int.Parse(form["NhanvienID"]);
            String repeatMK = form["RepeatMatKhau"];
            String matKhau = form["Matkhau"];           
            String isAdmin = form["isAdmin"];

            Nguoidung nguoiDung = db.Nguoidungs.Find(id);
            if (repeatMK.Equals(matKhau))
            {
                if (matKhau != "")
                {
                    if (ModelState.IsValid)
                    {
                        String firstHash = String.Concat(UserInfo.CreateMD5(matKhau).ToLower(), matKhau);
                        nguoiDung.Matkhau = UserInfo.CreateMD5(firstHash).ToLower();
                        if (Convert.ToInt32(isAdmin) == 1)
                        {
                            nguoiDung.Isadmin = true;
                        }
                        else
                        {
                            nguoiDung.Isadmin = false;
                        }
                        db.Entry(nguoiDung).State = EntityState.Modified;
                        db.SaveChanges();
                        
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.passwordMesg = "Mật khẩu cũ và mật khẩu mới phải trùng nhau";
            }

            ViewBag.selectedNhanVien = nguoidung.NhanvienID;
            int phongBanID = getPhongBanNguoiDung();
            if (phongBanID == 0)
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false).ToList();
            }
            else
            {
                ViewBag.NhanvienID = db.Nhanviens.Where(p => p.IsDelete == false && p.PhongbanID == phongBanID).ToList();
            }
            ViewBag.isAdmin = LoggedInUser.Isadmin.Value == true ? "" : null;
            return View(nguoiDung);
        }

        // GET: /Nguoidung/Delete/5
        public ActionResult Delete(int? id)
        {
            //xóa bảng đăng nhập 
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            int ngDungID = nguoidung.NguoidungID;
            Dangnhap dN = db.Dangnhaps.FirstOrDefault(p => p.NguoidungID == ngDungID);
            String sqlCommand = "DELETE FROM [HoaDonHaDong].[dbo].[Lichsusudungct] WHERE NguoidungID = @user";
            db.Database.ExecuteSqlCommand(sqlCommand, new SqlParameter("@user", ngDungID));
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
