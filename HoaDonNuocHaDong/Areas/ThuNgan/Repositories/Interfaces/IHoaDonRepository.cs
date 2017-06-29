using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces
{
    public interface IHoaDonRepository : IRepository<HDNHD.Models.DataContexts.Hoadonnuoc>
    {
        IQueryable<HoaDonModel> GetAllHoaDonModel(bool? trangThaiThu = true);
        IQueryable<HoaDonModel> GetAllHoaDonModelByKHID(int khachHangID);
        
        HoaDonModel GetHoaDonModelByID(int hoaDonID, bool? trangThaiThu = true);
        
        IQueryable<DuNoModel> GetAllDuNoModel(int month, int year);
    }
}
