using HDNHD.Core.Models;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.Services.Controllers
{
    public class NhanVienController : BaseController
    {
        private INhanVienRepository nhanVienRepository;

        public NhanVienController()
        {
            nhanVienRepository = uow.Repository<NhanVienRepository>();
        }

        public AjaxResult GetByToID(int toID)
        {
            var models = nhanVienRepository.GetByToID(toID).Select(m => new { 
                NhanvienID = m.NhanvienID,
                MaNhanVien = m.MaNhanVien,
                Ten = m.Ten
            });

            return new AjaxResult()
            {
                Data = models.ToList()
            };
        }
	}
}