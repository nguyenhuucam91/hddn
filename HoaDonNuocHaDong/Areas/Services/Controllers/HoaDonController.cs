using HDNHD.Core.Models;
using HDNHD.Core.Helpers;
using HoaDonNuocHaDong.Areas.ThuNgan.Helpers;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using System;
using System.Globalization;

namespace HoaDonNuocHaDong.Areas.Services.Controllers
{
    public class HoaDonController : BaseController
    {
        private IHoaDonRepository hoaDonRepository;
        private DateTime current;

        public HoaDonController()
        {
            hoaDonRepository = uow.Repository<HoaDonRepository>();
            current = DateTime.Now.AddMonths(-1);
        }


        /// <summary>
        /// thanh toán hóa đơn vs ID đã cho
        /// nếu ngày nộp không đúng > ngày nộp = ngày hiện tại
        /// </summary>
        /// <effects>
        ///     get HoaDonModel <tt>model</tt> with specified <tt>hoaDonID</tt>
        ///     if model == null || model.HoaDon.TrangThaiThu == true
        ///         throw NotPossibleException: Dữ liệu bất đồng bộ, vui lòng refresh lại trang
        ///     invoke @{link HoaDonHelpers#ThemGiaoDich(HoaDonModel)}: thanh toan HoaDonModel
        /// </effects>
        public AjaxResult ThanhToan(int hoaDonID, string ngayThu)
        {
            var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);
            if (model == null || model.HoaDon.Trangthaithu == true)
                return AjaxResult.Fail("Dữ liệu bất đồng bộ, vui lòng refresh lại trang.", true);

            try
            {
                DateTime dt;
                if (!DateTime.TryParseExact(ngayThu, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dt = DateTime.Now;

                if (!HoaDonHelpers.ThanhToan(model, dt, uow))
                    return AjaxResult.Fail("Lỗi cập nhật dữ liệu. Vui lòng thử lại.");
            }
            catch (Exception e)
            {
                return AjaxResult.Fail(e.Message);
            }

            int duNo = (int)(model.SoTienNopTheoThang.SoTienPhaiNop - model.SoTienNopTheoThang.SoTienDaThu);
            return AjaxResult.Success("Thanh toán thành công.",
                (model.HoaDon.ThangHoaDon != current.Month && model.HoaDon.NamHoaDon != current.Year),
                 new
                 {
                     HoaDon = new
                     {
                         ID = model.HoaDon.HoadonnuocID,
                         TrangThaiThu = model.HoaDon.Trangthaithu,
                         NgayNopTien = model.HoaDon.NgayNopTien != null ? model.HoaDon.NgayNopTien.Value.ToString("dd/MM/yyyy") : ""
                     },
                     SoTienNopTheoThang = new
                     {
                         SoTienDaThu = CurrencyHelpers.FormatVN(model.SoTienNopTheoThang.SoTienDaThu ?? 0),
                         DuNo = CurrencyHelpers.FormatVN(duNo > 0 ? duNo : -duNo)
                     }
                 });
        }

        /// <summary>
        /// Hủy thanh toán hóa đơn vs ID đã cho
        /// </summary>
        /// <effects>
        ///     get HoaDonModel <tt>model</tt> with specified <tt>hoaDonID</tt>
        ///     if model == null || model.TrangThaiThu == false
        ///         throw NotPossibleException: Dữ liệu bất đồng bộ, vui lòng refresh lại trang
        ///     if model.SoTienPhaiNop.SoTienDaThu == 0
        ///         throw NotPossibleException: Khách hàng thanh toán trừ dư có
        ///     if model.KhachHang.HinhThucThanhToan == ChuyenKhoan
        ///         throw NotPossibleException: Khách hàng thanh toán qua chuyển khoản
        ///     if lastGiaoDich.NgayThu == model.HoaDon.NgayThu && lastGiaoDich.SoTien == model.SoTienNopTheoThang.SoTienDaThu
        ///         invoke @{link #HuyGiaoDich()}: huy giao dich gan nhat
        ///     throw NotPossibleException: Không thể hủy thanh toán! Vui lòng hủy giao dịch tại Xem lịch sử giao dịch.
        /// </effects>
        public AjaxResult HuyThanhToan(int hoaDonID)
        {
            var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);
            if (model == null || model.HoaDon.Trangthaithu == false || model.HoaDon.Trangthaithu == null)
                return AjaxResult.Fail("Dữ liệu bất đồng bộ, vui lòng refresh lại trang.", true);

            try
            {
                if (!HoaDonHelpers.HuyThanhToan(model, uow))
                    return AjaxResult.Fail("Lỗi cập nhật dữ liệu. Vui lòng thử lại.");
            }
            catch (Exception e)
            {
                return AjaxResult.Fail(e.Message);
            }

            int duNo = (int)(model.SoTienNopTheoThang.SoTienPhaiNop - model.SoTienNopTheoThang.SoTienDaThu);
            return AjaxResult.Success("Hủy thanh toán thành công.", false,
                new
                {
                    HoaDon = new
                    {
                        ID = model.HoaDon.HoadonnuocID,
                        TrangThaiThu = model.HoaDon.Trangthaithu,
                        NgayNopTien = model.HoaDon.NgayNopTien != null ? model.HoaDon.NgayNopTien.Value.ToString("dd/MM/yyyy") : ""
                    },
                    SoTienNopTheoThang = new
                    {
                        SoTienDaThu = CurrencyHelpers.FormatVN(model.SoTienNopTheoThang.SoTienDaThu ?? 0),
                        DuNo = CurrencyHelpers.FormatVN(duNo > 0 ? duNo : -duNo)
                    }
                });
        }

        /// <summary>
        /// cập nhật ngày thu cho HoaDon với ID đã cho
        /// </summary>
        public AjaxResult CapNhatNgayThu(int hoaDonID, string ngayThu)
        {
            var hoaDon = hoaDonRepository.GetByID(hoaDonID);
            string bakNgayNop = null;

            if (hoaDon != null && hoaDon.Trangthaithu == true)
            {
                IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
                var giaoDich = giaoDichRepository.GetGDThanhToanByHDID(hoaDonID);
                if (giaoDich == null)
                    return AjaxResult.Fail("Không tìm thấy giao dịch thanh toán cho hóa đơn đã chọn. Vui lòng tải lại trang.", true, new
                    {
                        HoaDon = new { NgayNopTien = bakNgayNop } // send back prev ngayNop
                    });

                bakNgayNop = hoaDon.NgayNopTien.Value.ToString("dd/MM/yyyy"); // backup ngayNop
                DateTime dt;
                if (DateTime.TryParseExact(ngayThu, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    if ((hoaDon.ThangHoaDon < dt.Month && hoaDon.NamHoaDon == dt.Year) || hoaDon.NamHoaDon < dt.Year)
                    {
                        hoaDon.NgayNopTien = dt;
                        giaoDich.NgayGiaoDich = dt;
                        uow.SubmitChanges();
                        return AjaxResult.Success("Ngày thu đã được cập nhật.");
                    }
                }
            }

            return AjaxResult.Fail("Ngày thu không đúng hoặc hóa đơn chưa thanh toán.", false, new
            {
                HoaDon = new { NgayNopTien = bakNgayNop } // send back prev ngayNop
            });
        }
    }
}