using HDNHD.Core.Controllers;
using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Models.KhachHang;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Base
{
    public class BaseController : SecuredController
    {
        protected HDNHDUnitOfWork uow;
        protected INhanVienRepository nhanVienRepository;
        protected IPhongBanRepository phongBanRepository;
        protected HDNHD.Models.DataContexts.Nhanvien nhanVien;
        protected HDNHD.Models.DataContexts.Phongban phongBan;

        // view data
        protected string title;

        public BaseController()
        {
            uow = new HDNHDUnitOfWork();
            nhanVienRepository = uow.Repository<NhanVienRepository>();
            phongBanRepository = uow.Repository<PhongBanRepository>();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var role = EUserRole.Admin;
            // load nhanVien for LoggedInUser
            nhanVien = nhanVienRepository.GetByID(LoggedInUser.NhanvienID ?? 0);

            if (nhanVien != null)
            {
                phongBan = phongBanRepository.GetSingle(m => m.PhongbanID == nhanVien.PhongbanID);

                if (phongBan != null)
                {
                    var tenPhongBan = phongBan.Ten.ToLower();

                    if (tenPhongBan.Contains("kinh"))
                    {
                        role = EUserRole.KinhDoanh;
                    }
                    else if (tenPhongBan.Contains("in"))
                    {
                        role = EUserRole.InHoaDon;
                    }
                    else // add more role here
                    {
                        role = EUserRole.ThuNgan;
                    }
                }
            }
            // cache role
            RequestScope.UserRole = role;
            //kiểm tra ngày tháng hiện tại, nếu ngày tháng hiện tại mà lớn hơn ngày hết áp định thì cho số hộ(số định mức) = 1
            var currentDate = DateTime.Now;
            ModelKhachHang modelKH = new ModelKhachHang();
            modelKH.updateKHHetHanApGia(currentDate);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            // view data
            ViewBag.Title = title;
        }

        protected override ActionResult RedirectToLoginPage(string prevUrl)
        {
            return RedirectToAction("Login", "Secure", new { area = "", prevUrl = prevUrl });
        }

        protected override void Dispose(bool disposing)
        {
            uow.Dispose();
            base.Dispose(disposing);
        }
    }
}