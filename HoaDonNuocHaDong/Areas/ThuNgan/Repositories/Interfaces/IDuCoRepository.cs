using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces
{
    public interface IDuCoRepository : IRepository<HDNHD.Models.DataContexts.DuCo>
    {
        IQueryable<DuCoModel> GetAllDuCoModel(int month, int year);
    }
}
