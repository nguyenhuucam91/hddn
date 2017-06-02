using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System.Linq;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Controllers
{
    public class KhachHangController : BaseController
    {
        private IKhachHangRepository khachHangRepository;

        public KhachHangController()
        {
            khachHangRepository = uow.Repository<KhachHangRepository>();
        }

        /// <summary>
        ///     view list KhachHang with filter 
        /// </summary>
        public ActionResult Index(KhachHangFilterModel filter, Pager pager)
        {
            title = "Quản lý Khách hàng";

            // default values
            if (filter.Mode == KhachHangFilterModel.FilterByManagementInfo) // not in filter
            {
                // set selected to, quan huyen = nhanVien's to, quan huyen
                if (nhanVien != null && filter.QuanHuyenID == null)
                {
                    filter.NhanVienID = nhanVien.NhanvienID;
                    filter.ToID = nhanVien.ToQuanHuyenID;
                    IToRepository toRepository = uow.Repository<ToRepository>();
                    var to = toRepository.GetByID(nhanVien.ToQuanHuyenID ?? 0);
                    if (to != null)
                    {
                        filter.QuanHuyenID = to.QuanHuyenID;
                    }
                }
            }

            var items = khachHangRepository.GetAllKhachHangModel();

            items = filter.ApplyFilter(items);
            items = pager.ApplyPager(items);
            
            #region view data
            ViewBag.Filter = filter;
            ViewBag.Pager = pager;
            #endregion
            return View(items.ToList());
        }

        /// <summary>
        ///     view KhachHang details
        /// </summary>
        public ActionResult ChiTiet(int id)
        {
            title = "Chi tiết thông tin khách hàng";

            var model = khachHangRepository.GetKhachHangDetailsModel(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            
            return View(model);
        }
        
        public ActionResult CapNhat(int id)
        {
            title = "Cập nhật thông tin khách hàng";

            var khachHangModel = khachHangRepository.GetKhachHangDetailsModel(id);
            if (khachHangModel == null)
            {
                return RedirectToAction("Index");
            }
            
            return View(khachHangModel);
        }

        [HttpPost]
        public ActionResult CapNhat(int id, EHinhThucThanhToan hinhThucThanhToan, ELoaiKhachHang loaiKhachHang)
        {
            var khachHangModel = khachHangRepository.GetKhachHangDetailsModel(id);
            if (khachHangModel != null)
            {
                khachHangModel.KhachHang.HinhthucttID = (int)hinhThucThanhToan;
                khachHangModel.KhachHang.LoaiKHID = (int)loaiKhachHang;

                uow.SubmitChanges();
            }

            return RedirectToAction("ChiTiet", new { id = id });
        }

        /// <summary>
        ///     view list HoaDon of KhachHang with specified <tt>id</tt>
        /// </summary>
        /// <effects>
        ///     get KhachHang khachHang with specified <tt>id</tt>
        ///     if khachHang not exists
        ///         redirect user to ds KhachHang
        ///     else  
        ///         load all HoaDon of this KhachHang and display
        /// </effects>
        public ActionResult LichSuDungNuoc(int id, Pager pager)
        {
            title = "Chi tiết lịch sử dùng nước";

            var khachHangModel = khachHangRepository.GetKhachHangDetailsModel(id);

            if (khachHangModel == null)
            {
                return RedirectToAction("Index");
            }

            IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();
            var items = hoaDonRepository.GetAllHoaDonModelByKHID(id);

            pager.PageSize = 12; // hien thi 12 hoa don gan nhat
            items = pager.ApplyPager(items);

            #region view data
            ViewBag.Pager = pager;
            ViewBag.KhachHangModel = khachHangModel;
            #endregion

            return View(items.ToList());
        }

        /// <summary>
        ///     Xem lịch sử giao dịch của khách hàng
        /// </summary>
        public ActionResult LichSuGiaoDich(int id, Pager pager)
        {
            title = "Chi tiết lịch sử giao dịch";

            var khachHangModel = khachHangRepository.GetKhachHangDetailsModel(id);

            if (khachHangModel == null)
            {
                return RedirectToAction("Index");
            }

            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
            var items = giaoDichRepository.GetAllGiaoDichModelByKHID(id);
            items = pager.ApplyPager(items);
            
            #region view data
            ViewBag.Pager = pager;
            ViewBag.KhachHangModel = khachHangModel;
            #endregion

            return View(items.ToList());
        }
    }
}