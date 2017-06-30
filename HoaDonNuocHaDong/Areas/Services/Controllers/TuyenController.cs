using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.Services.Controllers
{
    public class TuyenController : BaseController
    {
        private ITuyenKHRepository tuyenKHRepository;

        public TuyenController()
        {
            tuyenKHRepository = uow.Repository<TuyenKHRepository>();
        }

        public AjaxResult GetByNhanVienID(int nhanVienID)
        {
            IQueryable<HDNHD.Models.DataContexts.Tuyenkhachhang> items;
            INhanVienRepository nhanVienRepository = uow.Repository<NhanVienRepository>();

            var nhanVien = nhanVienRepository.GetByID(nhanVienID);

            if (nhanVien != null && nhanVien.ChucvuID == (int)EChucVu.TRUONG_PHONG)
                items = tuyenKHRepository.GetByToID(nhanVien.ToQuanHuyenID.Value);
            else
                items = tuyenKHRepository.GetByNhanVienID(nhanVienID);

            var models = items.Select(m => new
            {
                TuyenKHID = m.TuyenKHID,
                MaTuyen = m.Matuyen,
                Ten = m.Ten
            });

            return new AjaxResult()
            {
                Data = models.ToList()
            };
        }
    }
}