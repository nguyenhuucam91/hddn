using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces
{
    public interface IHoaDonRepository : IRepository<HDNHD.Models.DataContexts.Hoadonnuoc>
    {
        IQueryable<HoaDonModel> GetAllHoaDonModel();
        IQueryable<HoaDonModel> GetAllHoaDonModelByKHID(int khachHangID);
        /// <requires>
        /// model != null /\ model.HoaDon != null /\ model.KhachHang != null
        /// </requires>
        HoaDonModel GetPrevUnPaidHoaDonModel(HoaDonModel model);
        HoaDonModel GetPrevPaidHoaDonModel(HoaDonModel model);
        

        HoaDonModel GetHoaDonModelByID(int hoaDonID);
        
        IQueryable<DuNoModel> GetAllDuNoModel();
    }
}
