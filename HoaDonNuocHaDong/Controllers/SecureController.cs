using HDNHD.Core.Helpers;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Controllers
{
    public class SecureController : Controller
    {
        protected AdminUnitOfWork adminUow;
        protected INguoiDungRepository nguoiDungRepository;
        protected IDangNhapRepository dangNhapRepository;
        protected BackupRepository backupRepository = new BackupRepository();

        public SecureController()
        {
            adminUow = new AdminUnitOfWork(new HDNHD.Models.DataContexts.AdminDataContext());
            nguoiDungRepository = adminUow.Repository<NguoiDungRepository>();
            dangNhapRepository = adminUow.Repository<DangNhapRepository>();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string prevUrl)
        {
            var passwordHash = AuthHelpers.CreatePassword(password);
            var count = nguoiDungRepository.GetAll().Count();

            if (count > 0)
            {
                // get NguoiDung by username
                var nguoiDung = nguoiDungRepository.GetSingle(m => m.Taikhoan == username);

                if (nguoiDung != null)
                {
                    var dangNhap = dangNhapRepository.GetByNguoiDungID(nguoiDung.NguoidungID);
                    // create DangNhap if not exists
                    if (dangNhap == null)
                    {
                        dangNhap = new HDNHD.Models.DataContexts.Dangnhap()
                        {
                            NguoidungID = nguoiDung.NguoidungID,
                            Solandangnhapsai = 0
                        };
                        dangNhapRepository.Insert(dangNhap);
                    }

                    var encryptedPassword = AuthHelpers.CreatePassword(password);
                    if (nguoiDung.Matkhau == encryptedPassword)
                    { // password matched
                        // check if locked
                        if (dangNhap.Trangthaikhoa == true && dangNhap.Thoigianhethankhoa < DateTime.Now)
                        {
                            // release lock
                            dangNhap.Trangthaikhoa = false;
                            dangNhap.Solandangnhapsai = 0;
                        }

                        if (dangNhap.Trangthaikhoa != true)
                        {
                            // update Thoigiandangnhap
                            dangNhap.Thoigiandangnhap = DateTime.Now;
                            adminUow.SubmitChanges();

                            // set cookie
                            var cookie = new HttpCookie(Cookies.B_ADMIN_LOGIN_TOKEN, AuthHelpers.MD5(nguoiDung.NguoidungID.ToString()));
                            cookie.Expires = DateTime.Now.AddDays(31);
                            cookie.Path = "/"; // make cookie available across applications
                            Response.Cookies.Add(cookie);

                            backupRepository.applyBackupProcess(nguoiDung.NguoidungID);

                            // redirect
                            if (prevUrl != null)
                                Response.Redirect(prevUrl);
                            else
                                return RedirectToAction("Index", "Default", new { area = "" });
                        }
                        else
                        {
                            ViewBag.Message = "Tài khoản của bạn đã bị khóa (tới sau ngày " + dangNhap.Thoigianhethankhoa + ") do nhập sai password quá 5 lần. Xin hãy quay lại sau.";
                        }
                    }
                    else
                    { // sai mật khẩu => đếm số lần và khóa
                        dangNhap.Solandangnhapsai++;

                        if (dangNhap.Solandangnhapsai == HDNHDConstants.LOGIN_MAX_FAILS)
                        {
                            dangNhap.Trangthaikhoa = true;
                            dangNhap.Thoigianhethankhoa = DateTime.Now.AddDays(HDNHDConstants.LOGIN_LOCK_DAYS);
                            ViewBag.Message = "Tài khoản của bạn đã bị khóa " + HDNHDConstants.LOGIN_LOCK_DAYS + " ngày do nhập sai password quá 5 lần. Xin hãy quay lại sau.";
                        }
                        else
                        {
                            ViewBag.Message = "Tài khoản hoặc mật khẩu chưa đúng. Vui lòng kiểm tra lại";
                        }

                        adminUow.SubmitChanges();
                    }



                }
                else
                {
                    ViewBag.Message = "Tài khoản hoặc mật khẩu chưa đúng. Vui lòng kiểm tra lại."; ;
                }
            }
            else
            {
                ViewBag.Message = "Không có thông tin người dùng trong hệ thống";
            }

            return View();
        }

        public ActionResult Logout()
        {
            if (Request.Cookies[Cookies.B_ADMIN_LOGIN_TOKEN] != null)
            {
                Response.Cookies[Cookies.B_ADMIN_LOGIN_TOKEN].Expires = DateTime.Now.AddDays(-1);
            }

            return RedirectToAction("Login");
        }

        /// <summary>
        /// create admin account if no account exists
        /// </summary>
        public ActionResult Seed()
        {
            int count = nguoiDungRepository.GetAll().Count();
            if (count == 0)
            {
                var password = AuthHelpers.CreatePassword("123456");

                adminUow.BeginTransaction();
                try
                {
                    var nguoiDung = new HDNHD.Models.DataContexts.Nguoidung()
                    {
                        Taikhoan = "admin",
                        Isadmin = true,
                        Matkhau = password
                    };
                    nguoiDungRepository.Insert(nguoiDung);
                    adminUow.SubmitChanges();

                    var dangNhap = new HDNHD.Models.DataContexts.Dangnhap()
                    {
                        NguoidungID = nguoiDung.NguoidungID,
                        Solandangnhapsai = 0
                    };
                    dangNhapRepository.Insert(dangNhap);

                    adminUow.SubmitChanges();
                    adminUow.Commit();

                    return Content("Created user: 'admin' successfully!");
                }
                catch (Exception e)
                {
                    adminUow.RollBack();
                    return Content("Fail to create seeding user: 'admin'.");
                }
            }

            return Content("Users exist. Seeding aborted!");
        }
    }
}