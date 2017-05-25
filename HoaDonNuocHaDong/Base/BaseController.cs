using HDNHD.Core.Constants;
using HDNHD.Core.Controllers;
using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Helper;
using HoaDonNuocHaDong.Models;
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
        protected HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        protected HDNHD.Models.DataContexts.Nhanvien nhanVien;
        protected HDNHD.Models.DataContexts.Phongban phongBan;

        // view data
        protected string title = "hdn";

        public BaseController()
        {
            uow = new HDNHDUnitOfWork();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            INhanVienRepository nhanVienRepository = uow.Repository<NhanVienRepository>();
            IPhongBanRepository phongBanRepository = uow.Repository<PhongBanRepository>();

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
                    else if (tenPhongBan.ToLower().Contains("in") || tenPhongBan.ToLower().Contains("kế toán"))
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

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                int chucNangID = getChucNangIDFromUrl(controllerName, actionName);
                appendToLogTable(chucNangID);
            }
            catch (Exception)
            {
                // ignore this
            }
        }

        private void appendToLogTable(int chucNangID)
        {
            Lichsusudungct lichSu = new Lichsusudungct();
            lichSu.ChucnangID = chucNangID;
            lichSu.NguoidungID = LoggedInUser.NguoidungID;
            lichSu.Thoigian = DateTime.Now;
            db.Lichsusudungcts.Add(lichSu);
            db.SaveChanges();
        }

        private int getChucNangIDFromUrl(string controllerName, string actionName)
        {
            if (String.IsNullOrEmpty(actionName))
            {
                actionName = "Index";
            }

            int nhomChucNangID = getNhomChucNangIDFromUrl(controllerName);

            Chucnangchuongtrinh chucNang = db.Chucnangchuongtrinhs.FirstOrDefault(p => p.NhomchucnangID == nhomChucNangID && p.TenAction == actionName);
            if (chucNang != null)
            {
                return chucNang.ChucnangID;
            }
            return 0;
        }

        private int getNhomChucNangIDFromUrl(string controllerName)
        {
            String controllerNameToLower = controllerName + "Controller".ToLower();
            Nhomchucnang nhomChucNang = db.Nhomchucnangs.FirstOrDefault(p => p.TenController.ToLower() == controllerNameToLower);
            if (nhomChucNang != null)
            {
                return nhomChucNang.NhomchucnangID;
            }
            return 0;
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

        public ViewResult ExcelResult(String view, object data)
        {
            Response.Clear();
            //Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Charset = "utf-8";
            Response.AddHeader("Content-Disposition",
                String.Format(@"attachment; filename={0}.xls", HDNHD.Core.Helpers.StringHelpers.GenerateSlug(title)));

            return View(view, data);
        }

        public int getUserRole(int? NhanVienID)
        {
            if (NhanVienID != null)
            {
                int nhanvienLoggedInId = NhanVienID.Value;
                Nhanvien nhanVien = db.Nhanviens.Find(nhanvienLoggedInId);
                if (nhanVien != null)
                {
                    return nhanVien.ChucvuID.Value;
                }
            }
            return 0;
        }

        public List<ModelNhanVien> getNhanViensByTo(int? ToID)
        {
            int phongBanID = getPhongBanNguoiDung();
            var nhanViens = (from i in db.Nhanviens
                             where i.ToQuanHuyenID == ToID.Value && i.IsDelete == false && i.PhongbanID == phongBanID
                             select new ModelNhanVien
                             {
                                 NhanvienID = i.NhanvienID,
                                 Ten = i.Ten,
                                 MaNhanVien = i.MaNhanVien,
                                 ChucvuID = i.ChucvuID
                             }).Distinct().ToList();
            return nhanViens;
        }

        public List<ToQuanHuyen> getToes(int quanHuyenID, int phongBanID)
        {
            return db.ToQuanHuyens.Where(p => p.IsDelete == false && p.QuanHuyenID == quanHuyenID && p.PhongbanID == phongBanID).ToList();
        }

        public int getQuanHuyenOfLoggedInUser()
        {
            return Convert.ToInt32(NguoidungHelper.getChiNhanhCuaNguoiDung(LoggedInUser.NguoidungID, 0));
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

        public List<ModelTuyen> getTuyensThuocNhanVien(int? NhanVienID)
        {
            var tuyens = (from i in db.Tuyentheonhanviens
                          join r in db.Tuyenkhachhangs on i.TuyenKHID equals r.TuyenKHID
                          where i.NhanVienID == NhanVienID && r.IsDelete == false
                          select new ModelTuyen
                          {
                              TuyenID = r.TuyenKHID,
                              Ten = r.Ten,
                              Matuyen = r.Matuyen,
                          }).Distinct().ToList();
            return tuyens;
        }

        protected String isLoggedUserAdminVaTruongPhong()
        {
            String isAdminVaTruongPhong = "0";
            Nguoidung nguoiDungEntity = db.Nguoidungs.FirstOrDefault(p => p.NguoidungID == LoggedInUser.NguoidungID);
            if (nguoiDungEntity != null)
            {
                Nhanvien nhanVien = nguoiDungEntity.Nhanvien;
                if (nhanVien != null)
                {
                    int? chucVuId = nhanVien.ChucvuID;
                    if (chucVuId != null)
                    {
                        isAdminVaTruongPhong = chucVuId == (int)EChucVu.TRUONG_PHONG ? "1" : "0";
                    }
                }
                else
                {
                    isAdminVaTruongPhong = LoggedInUser.Isadmin.Value == true ? "1" : "0";
                }
            }
            return isAdminVaTruongPhong;
        }
    }
}