using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces
{
    public interface ISoTienNopTheoThangRepository : IRepository<HDNHD.Models.DataContexts.SoTienNopTheoThang>
    {
        IQueryable<HDNHD.Models.DataContexts.SoTienNopTheoThang> GetAllByMonthYear(int month, int year);
    }
}
