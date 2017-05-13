using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using System.Data.Linq;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;
using HDNHD.Models.DataContexts;
using System;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class SoTienNopTheoThangRepository : LinqRepository<HDNHD.Models.DataContexts.SoTienNopTheoThang>, ISoTienNopTheoThangRepository
    {
        private HDNHDDataContext dc;

        public SoTienNopTheoThangRepository(DataContext context) : base(context)
        {
            dc = (HDNHDDataContext)context;
        }

        public IQueryable<HDNHD.Models.DataContexts.SoTienNopTheoThang> GetAllByMonthYear(int month, int year)
        {
            return null;
        }
    }
}