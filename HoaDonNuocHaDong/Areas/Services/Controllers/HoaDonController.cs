using HDNHD.Core.Models;
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
        public AjaxResult CapNhatThanhToan(int hoaDonID, bool trangThaiThu = false, string ngayThu = null)
        {
            IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();

            var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);
            if (model != null)
            {
                //DateTime dt;
                //if (!DateTime.TryParseExact(ngayThu,
                //        "dd/MM/yyyy",
                //        CultureInfo.InvariantCulture,
                //        DateTimeStyles.None, out dt))
                //{
                //    dt = DateTime.Now;
                //}
                //model.HoaDon.NgayNopTien = dt;

                var result = HoaDonHelpers.CapNhatThanhToan(model, trangThaiThu, uow);

                return new AjaxResult()
                {
                    IsSuccess = result,
                    Data = new
                    {
                        HoaDon = new
                        {
                            ID = model.HoaDon.HoadonnuocID,
                            TrangThaiThu = model.HoaDon.Trangthaithu,
                            NgayNopTien = model.HoaDon.NgayNopTien != null ? model.HoaDon.NgayNopTien.Value.ToString("dd/MM/yyyy") : ""
                        },
                        SoTienNopTheoThang = new
                        {
                            SoTienDaThu = HoaDonNuocHaDong.Helper.HoaDonNuoc.formatCurrency(model.SoTienNopTheoThang.SoTienDaThu ?? 0)
                        },
                        DuCo = new
                        {
                            SoTienDu = model.DuCo != null ? model.DuCo.SoTienDu : -1
                        }
                    }
                };
            }

            return AjaxResult.Fail("Hóa đơn đã bị hủy, vui lòng tải lại trang.");
        }

        public AjaxResult CapNhatNgayThu(int hoaDonID, DateTime? ngayThu = null)
        {
            return null;
        }
    }
}