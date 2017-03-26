using HDNHD.Core.Controllers;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Controllers
{
    public class HoaDonController : BaseController
    {
        private IHoaDonRepository hoaDonRepository;
        private IToRepository toRepository;
        private IQuanHuyenRepository quanHuyenRepository;

        public HoaDonController()
        {
            hoaDonRepository = uow.Repository<HoaDonRepository>();
            toRepository = uow.Repository<ToRepository>();
            quanHuyenRepository = uow.Repository<QuanHuyenRepository>();
        }

        /// <summary>
        /// load hiển thị dữ liệu hóa đơn
        /// </summary>
        public ActionResult Index(HoaDonFilterModel filter, HDNHD.Core.Models.Pager pager)
        {
            // default values
            if (filter.Mode == null) // not in filter
            {
                if (filter.From == null && filter.To == null)
                    filter.From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                // set selected to, quan huyen = nhanVien's to, quan huyen
                if (nhanVien != null)
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

            IQueryable<HoaDonModel> items =
                hoaDonRepository.GetAll(m => m.Trangthaiin == true && m.Trangthaixoa == false).Select(m => new HoaDonModel()
                {
                    HoaDon = m
                });
            filter.ApplyFilter(ref items, ref pager);

            #region view data
            title = "Quản lý công nợ";
            var sectionTitle = "Quản lý công nợ khách hàng ";

            if (filter.HinhThucThanhToan == EHinhThucThanhToan.ChuyenKhoan)
                sectionTitle += "Chuyển khoản";
            else if (filter.LoaiKhachHang == ELoaiKhachHang.HoGiaDinh)
                sectionTitle += "Hộ gia đình";
            else
                sectionTitle += "Cơ quan, tổ chức";

            // load ds to thuoc filter.quanhuyen
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            ViewBag.SectionTitle = sectionTitle;
            #endregion
            return View(items.ToList());
        }
    }
}