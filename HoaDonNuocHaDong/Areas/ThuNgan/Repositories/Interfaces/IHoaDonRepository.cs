using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces
{
    public interface IHoaDonRepository : IRepository<HDNHD.Models.DataContexts.Hoadonnuoc>
    {
        IQueryable<HoaDonModel> GetAllHoaDonModel(bool? trangThaiIn = null);
        IQueryable<HoaDonModel> GetAllHoaDonModelByKHID(int khachHangID, bool? trangThaiIn = null);
        
        HoaDonModel GetHoaDonModelByID(int hoaDonID);
        
        IQueryable<DuNoModel> GetAllDuNoModel(int month, int year);
        IQueryable<KhongSanLuongModel> GetAllKhongSanLuongModel(int month, int year);
        IQueryable<LoaiGiaModel> GetAllLoaiGiaModel(int month, int year);
    }
}
