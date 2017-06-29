using HDNHD.Core.Constants;
using HDNHD.Core.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Helpers;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Controllers
{
    public class HoaDonController : BaseController
    {
        private IHoaDonRepository hoaDonRepository;
        private IToRepository toRepository;

        public HoaDonController()
        {
            hoaDonRepository = uow.Repository<HoaDonRepository>();
            toRepository = uow.Repository<ToRepository>();
        }

        /// <summary>
        /// view list of HoaDon with filter
        /// </summary>
        public ActionResult Index(HoaDonFilterModel filter, Pager pager, String todo, ViewMode viewMode = ViewMode.Default)
        {
            title = "Quản lý công nợ khách hàng";
            
            // default values
            if (filter.Mode == null || filter.Mode == HoaDonFilterModel.FilterByUserInfo) { // not in filter
                if ((filter.Month == null) || filter.TrangThaiThu == HDNHD.Models.Constants.ETrangThaiThu.DaQuaHan)
                {
                    filter.Month = DateTime.Now.Month;
                    filter.Year = DateTime.Now.Year;

                    if (filter.TrangThaiThu == null)
                        filter.TrangThaiThu = HDNHD.Models.Constants.ETrangThaiThu.ChuaNopTien;
                    if (filter.HinhThucThanhToan == null)
                        filter.HinhThucThanhToan = HDNHD.Models.Constants.EHinhThucThanhToan.TienMat;
                }

                // set selected to, quan huyen = nhanVien's to, quan huyen
                if (nhanVien != null && filter.QuanHuyenID == null)
                {
                    filter.NhanVienID = nhanVien.NhanvienID;
                    filter.ToID = nhanVien.ToQuanHuyenID;

                    var to = toRepository.GetByID(nhanVien.ToQuanHuyenID ?? 0);
                    if (to != null)
                    {
                        filter.QuanHuyenID = to.QuanHuyenID;
                    }
                }
            }
            
            // query items
            var items = hoaDonRepository.GetAllHoaDonModel();

            items = filter.ApplyFilter(items);

            ViewBag.TongSoTienTrenHoaDon = items.Sum(m => m.SoTienTrenHoaDon) ?? 0;
            ViewBag.TongSoTienPhaiNop = items.Sum(m => m.SoTienNopTheoThang.SoTienPhaiNop) ?? 0;
            ViewBag.TongSoTienDaNop = items.Sum(m => m.SoTienNopTheoThang.SoTienDaThu) ?? 0;
            ViewBag.TongDuCo = items.Where(m => m.DuCo != null).Sum(m => m.DuCo.SoTienDu) ?? 0;
            ViewBag.TongDuNo = items.Sum(m => m.DuNo) ?? 0;

            if (viewMode == ViewMode.Excel)
                return ExcelResult("IndexExport", items.ToList());

            items = pager.ApplyPager(items);

            #region view data
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            ViewBag.ToDo = todo; // actions
            #endregion
            return View(items.ToList());
        }

        public ActionResult ThemGiaoDich(int hoaDonID, Pager pager)
        {
            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
            var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);

            if (model == null)
                return HttpNotFound("Dữ liệu bất đồng bộ. Vui lòng refresh lại trang!");

            var giaoDichs = giaoDichRepository.GetAllGiaoDichModelByKHID(model.KhachHang.KhachhangID);
            giaoDichs = pager.ApplyPager(giaoDichs);
            #region view data
            ViewBag.HoaDonModel = model;
            ViewBag.Pager = pager;
            ViewBag.KhachHang = model.KhachHang;
            #endregion
            return View(giaoDichs.ToList());
        }
    }
}