using HDNHD.Core.Models;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.Services.Controllers
{
    public class ToController : BaseController
    {
        private IToRepository toRepository;

        public ToController()
        {
            toRepository = uow.Repository<ToRepository>();
        }
        
        /// <summary>
        /// returns list of ToQuanHuyen of the specified <tt>quanHuyenID</tt> 
        ///     customized by <tt>this.phongBan</tt> if exist
        /// </summary>
        public AjaxResult GetByQuanHuyenID(int quanHuyenID, bool byNhanVien = false) {
            var tos = toRepository.GetByQuanHuyenID(quanHuyenID);

            if (byNhanVien && phongBan != null)
            {
                tos = tos.Where(m => m.PhongbanID == phongBan.PhongbanID);
            }

            return new AjaxResult() { 
                Data = tos.ToList()
            };
        }
	}
}