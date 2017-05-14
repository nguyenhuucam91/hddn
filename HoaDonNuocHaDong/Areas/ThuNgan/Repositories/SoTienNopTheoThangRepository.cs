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

        public IQueryable<SoTienNopTheoThangModel> GetAllByMonthYear(int month, int year)
        {
            return from item in dc.SoTienNopTheoThangs
                   join hd in dc.Hoadonnuocs on item.HoaDonNuocID equals hd.HoadonnuocID
                   where hd.ThangHoaDon == month && hd.NamHoaDon == year
                   select new SoTienNopTheoThangModel()
                   {
                       SoTienNopTheoThang = item,
                       SoTienPhaiNop = (long?) item.SoTienPhaiNop,
                       SoTienTrenHoaDon = item.SoTienTrenHoaDon
                   };
        }
    }
}