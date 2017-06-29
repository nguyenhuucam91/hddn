using HDNHD.Core.Models;
using HDNHD.Core.Helpers;
using HoaDonNuocHaDong.Areas.ThuNgan.Helpers;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.Services.Controllers
{
    public class GiaoDichController : BaseController
    {
        /// <summary>
        /// Thêm giao dịch cho khách hàng với ID đã cho
        /// Nếu ngày nộp không đúng -> ngày nộp = ngày hiện tại
        /// </summary>
        /// <effects>
        /// if KhachHang with specified ID not exists
        ///     throw NotFoundException: Khách hàng không tồn tại. Vui lòng tải lại trang.
        /// invoke @{link #ThemGiaoDich()}: add GiaoDich for specified khachHang
        /// </effects>
        [HttpPost]
        public AjaxResult ThemGiaoDich(int hoaDonID, int soTien, string ngayGiaoDich)
        {
            IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();
            var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);

            if (model == null)
                return AjaxResult.Fail("Hóa đơn không tồn tại. Vui lòng tải lại trang.", true);

            if (model.CoDuNoQuaHan)
                return AjaxResult.Fail("Khách hàng có dư nợ quá hạn cần thanh toán trước.", true);

            var current = DateTime.Now.AddMonths(-1);
            if (!(!model.CoDuNoQuaHan && // không có dư nợ & (tháng hiện tại || (< tháng hiện tại & chưa thu))
                ((model.HoaDon.ThangHoaDon == current.Month && model.HoaDon.NamHoaDon == current.Year) ||
                ((model.HoaDon.NamHoaDon < current.Year || (model.HoaDon.NamHoaDon == current.Year && model.HoaDon.ThangHoaDon < current.Month))
                && (model.HoaDon.Trangthaithu == false || model.HoaDon.Trangthaithu == null)))))
            {
                return AjaxResult.Fail("Không thể thêm giao dịch cho hóa đơn này. Dữ liệu bất đồng bộ, vui lòng load lại trang.", true);
            }

            DateTime dt;
            if (!DateTime.TryParseExact(ngayGiaoDich, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                dt = DateTime.Now;

            if (!GiaoDichHelpers.ThemGiaoDich(model, soTien, dt, uow))
            {
                return AjaxResult.Fail("Lỗi thêm giao dịch. Vui lòng thử lại.");
            }

            int duNo = (int)(model.SoTienNopTheoThang.SoTienPhaiNop - model.SoTienNopTheoThang.SoTienDaThu);
            return AjaxResult.Success("Giao dịch thành công.", false,
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
        /// Hủy giao dịch với ID đã cho
        /// </summary>
        /// <effects>
        /// if giaoDich with specified giaoDichID not exist
        ///     throw NotFoundException: Dữ liệu bất đồng bộ. Vui lòng tải lại trang.
        /// if lastGiaoDich.ID == giaoDichID
        ///     invoke @{link #HuyGiaoDich}: roll back lastGiaoDich
        /// </effects>
        public AjaxResult HuyGiaoDich(int khachHangID, int giaoDichID)
        {
            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();

            var model = giaoDichRepository.GetLastGiaoDichByKHID(khachHangID);
            if (model == null || model.GiaoDich.GiaoDichID != giaoDichID)
                return AjaxResult.Fail("Dữ liệu bất đồng bộ. Vui lòng tải lại trang.", true);

            if (!GiaoDichHelpers.HuyGiaoDich(model, uow))
                return AjaxResult.Fail("Lỗi hủy giao dịch. Vui lòng thử lại.");

            return AjaxResult.Success("Hủy giao dịch thành công.", true);
        }
    }
}