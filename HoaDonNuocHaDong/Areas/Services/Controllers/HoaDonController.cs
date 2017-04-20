using HDNHD.Core.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Helpers;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Base;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.Services.Controllers
{
    public class HoaDonController : BaseController
    {
        private IHoaDonRepository hoaDonRepository;

        public HoaDonController()
        {
            hoaDonRepository = uow.Repository<HoaDonRepository>();
        }

        [HttpPost]
        public AjaxResult GiaoDich(int hoaDonID, int soTien, string ngayNop)
        {
            uow.BeginTransaction();

            try
            {
                // số tiền không hợp lệ
                if (soTien <= 0)
                    return AjaxResult.Fail("Số tiền không hợp lệ.");

                DateTime dt;
                if (!DateTime.TryParseExact(ngayNop,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out dt))
                {
                    dt = DateTime.Now;
                }

                IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();
                IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
                var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);

                // id không tồn tại
                if (model == null)
                    return AjaxResult.Fail("ID hóa đơn không tồn tại.");

                // tạo giao dịch
                HDNHD.Models.DataContexts.GiaoDich giaoDich = new HDNHD.Models.DataContexts.GiaoDich()
                {
                    TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                    NgayGiaoDich = dt,
                    SoTien = soTien,
                    SoDu = 0
                };
                giaoDichRepository.Insert(giaoDich);

                // cập nhật SoTienNopTheoThang
                model.SoTienNopTheoThang.SoTienDaThu += soTien;
                if (model.SoTienNopTheoThang.SoTienDaThu >= model.SoTienNopTheoThang.SoTienPhaiNop)
                {
                    // cập nhật trạng thái thu
                    if (model.HoaDon.Trangthaithu == null || model.HoaDon.Trangthaithu == false)
                    {
                        model.HoaDon.Trangthaithu = true;
                        model.HoaDon.NgayNopTien = dt;
                    }
                }

                // thêm vào DuCo
                if (model.SoTienNopTheoThang.SoTienDaThu > model.SoTienNopTheoThang.SoTienPhaiNop)
                {
                    if (model.DuCo == null)
                    {
                        model.DuCo = new HDNHD.Models.DataContexts.DuCo()
                        {
                            KhachhangID = model.KhachHang.KhachhangID,
                            TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                            SoTienDu = 0
                        };
                        duCoRepository.Insert(model.DuCo);
                    }

                    model.DuCo.SoTienDu = (int)(model.SoTienNopTheoThang.SoTienDaThu - model.SoTienNopTheoThang.SoTienPhaiNop);
                }

                uow.SubmitChanges();
                uow.Commit();
                return AjaxResult.Success("Thành công.");
            }
            catch (Exception)
            {
                uow.RollBack();

                return AjaxResult.Success("Lỗi, vui lòng thử lại.");
            }
        }

        public AjaxResult CapNhatThanhToan(int hoaDonID, bool trangThaiThu = false, string ngayThu = null)
        {
            IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();

            var model = hoaDonRepository.GetHoaDonModelByID(hoaDonID);
            if (model != null)
            {
                DateTime dt;
                if (!DateTime.TryParseExact(ngayThu,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out dt))
                {
                    dt = DateTime.Now;
                }
                model.HoaDon.NgayNopTien = dt;

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

        public AjaxResult CapNhatNgayThu(int hoaDonID, string ngayThu)
        {
            var hoaDon = hoaDonRepository.GetByID(hoaDonID);
            if (hoaDon != null && hoaDon.Trangthaithu == true)
            {
                DateTime dt;
                if (DateTime.TryParseExact(ngayThu,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out dt))
                {
                    hoaDon.NgayNopTien = dt;

                    uow.SubmitChanges();
                    return AjaxResult.Success("Ngày thu đã được cập nhật.");
                }
            }

            return AjaxResult.Fail("Ngày thu chưa đúng định dạng hoặc hóa đơn chưa thanh toán.");
        }
    }
}