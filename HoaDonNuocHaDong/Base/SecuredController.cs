using HDNHD.Core.Helpers;
using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace HoaDonNuocHaDong.Base
{
    public abstract class SecuredController : Controller
    {
        protected AdminUnitOfWork adminUow;
        protected INguoiDungRepository nguoiDungRepository;
        protected IDangNhapRepository dangNhapRepository;
        protected HDNHD.Models.DataContexts.Nguoidung LoggedInUser;

        public SecuredController()
        {
            adminUow = new AdminUnitOfWork(new HDNHD.Models.DataContexts.AdminDataContext());
            nguoiDungRepository = adminUow.Repository<NguoiDungRepository>();
            dangNhapRepository = adminUow.Repository<DangNhapRepository>();
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            var cookies = filterContext.HttpContext.Request.Cookies;
            if (cookies[Cookies.B_ADMIN_LOGIN_TOKEN] == null)
            {
                filterContext.Result = RedirectToLoginPage(Request.Url.ToString());
                return;
            }

            // expired session
            var dangNhap = dangNhapRepository.CheckLogin(cookies[Cookies.B_ADMIN_LOGIN_TOKEN].Value, new TimeSpan(31, 0, 0, 0));
            if (dangNhap != null)
            {
                LoggedInUser = nguoiDungRepository.GetByID(dangNhap.NguoidungID);
                // update last_login
                dangNhap.Thoigiandangnhap = DateTime.Now;
                dangNhap.Solandangnhapsai = 0;
                adminUow.SubmitChanges();
            }

            if (LoggedInUser == null)
            {
                filterContext.Result = RedirectToLoginPage(Request.Url.ToString());
                return;
            }

            // cache
            RequestScope.LoggedInUser = LoggedInUser;
            
            base.OnAuthentication(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected abstract ActionResult RedirectToLoginPage(string prevUrl);
    }
}