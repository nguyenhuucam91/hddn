using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class GiaoDichRepository : LinqRepository<HDNHD.Models.DataContexts.GiaoDich>, IGiaoDichRepository
    {
        public GiaoDichRepository(HDNHDDataContext context) : base(context) { }
    }
}