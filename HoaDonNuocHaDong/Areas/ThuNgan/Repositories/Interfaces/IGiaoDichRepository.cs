using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces
{
    public interface IGiaoDichRepository : IRepository<HDNHD.Models.DataContexts.GiaoDich>
    {
        IQueryable<GiaoDichModel> GetAllGiaoDichModel(); 
        IQueryable<GiaoDichModel> GetAllGiaoDichModelByKHID(int khachHangID);
        GiaoDichModel GetGiaoDichModelByID(int id);
        GiaoDichModel GetLastGiaoDichByKHID(int khachHangID);
        HDNHD.Models.DataContexts.GiaoDich GetGDThanhToanByHDID(int hoaDonID);

        IQueryable<GiaoDichSumModel> GetAllByMonthYear(int month, int year);
    }
}
