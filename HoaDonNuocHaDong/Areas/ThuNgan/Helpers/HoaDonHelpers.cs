using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Helpers
{
    public class HoaDonHelpers
    {
        /// <summary>
        /// Thực hiện thanh toán với HoaDonModel
        /// </summary>
        /// <requires>
        /// model != null /\ model.SoTienNopTheoThang != null
        /// </requires>
        /// <effects>
        /// if model.SoTienNopTheoThang.DuNo > 0
        ///     throw NotPossibleException: khách hàng còn dư nợ quá hạn cần thanh toán trước
        /// invoke @{link #ThemGiaoDich()}: add GiaoDich with soTien = model.SoTienNopTheoThang.SoTienPhaiNop
        /// </effects>
        public static bool ThanhToan(HoaDonModel model, DateTime ngayThu, HDNHDUnitOfWork uow = null)
        {
            if (uow == null) uow = new HDNHDUnitOfWork();

            if (model.CoDuNoQuaHan)
                throw new Exception("Khách hàng còn dư nợ quá hạn cần thanh toán trước.");

            int duNo = (int)(model.SoTienNopTheoThang.SoTienPhaiNop - model.SoTienNopTheoThang.SoTienDaThu);

            return GiaoDichHelpers.ThemGiaoDich(model, duNo, ngayThu, uow);
        }

        /// <summary>
        /// Hủy thanh toán HoaDonModel
        /// </summary>
        /// <requires>
        /// model != null /\ model.HoaDon.TrangThaiThu eq true
        /// </requires>
        /// <effects>
        /// if model.SoTienPhaiNop.SoTienDaThu == 0
        ///     throw NotPossibleException: Khách hàng thanh toán trừ dư có
        /// if model.KhachHang.HinhThucThanhToan == ChuyenKhoan
        ///     throw NotPossibleException: Khách hàng thanh toán qua chuyển khoản
        /// if lastGiaoDich.NgayThu == model.HoaDon.NgayThu && lastGiaoDich.SoTien == model.SoTienNopTheoThang.SoTienDaThu
        ///     invoke @{link GiaoDichHelpers#HuyGiaoDich()}: huy giao dich gan nhat
        ///     
        /// throw NotPossibleException: Không thể hủy thanh toán tại đây! Vui lòng hủy giao dịch tại Xem lịch sử giao dịch.
        public static bool HuyThanhToan(HoaDonModel model, HDNHDUnitOfWork uow = null)
        {
            if (uow == null) uow = new HDNHDUnitOfWork();

            if (model.SoTienNopTheoThang.SoTienDaThu == 0)
                throw new Exception("Khách hàng thanh toán trừ dư có.");

            if (model.KhachHang.HinhthucttID == (int)EHinhThucThanhToan.ChuyenKhoan)
                throw new Exception("Khách hàng thanh toán qua chuyển khoản.");

            var current = DateTime.Now.AddMonths(-1);
            if (model.HoaDonTiepTheo != null && model.HoaDonTiepTheo.Trangthaithu == true
                && (model.HoaDon.ThangHoaDon != current.Month || model.HoaDon.NamHoaDon != current.Year))
            {
                throw new Exception("Khách hàng đã thanh toán hóa đơn tiếp theo.");
            }

            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
            var lastGiaoDich = giaoDichRepository.GetLastGiaoDichByKHID(model.KhachHang.KhachhangID);

            if (lastGiaoDich == null)
                throw new Exception("Lỗi, không có dữ liệu giao dịch cho hóa đơn này.");

            if (lastGiaoDich.SoTienNopTheoThang.ID != model.SoTienNopTheoThang.ID ||
                lastGiaoDich.GiaoDich.SoTien != model.SoTienNopTheoThang.SoTienDaThu)
            {
                throw new Exception("Không thể hủy thanh toán tại đây! Vui lòng hủy tại trang 'Lịch sử giao dịch'.");
            }

            return GiaoDichHelpers.HuyGiaoDich(lastGiaoDich, uow);
        }
    }
}