using HDNHD.Core.Repositories;
using HDNHD.Core.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HDNHD.Models.DataContexts;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class SoTienNopTheoThangRepository : LinqRepository<HDNHD.Models.DataContexts.SoTienNopTheoThang>, ISoTienNopTheoThangRepository
    {
        private HDNHDDataContext dc;

        public SoTienNopTheoThangRepository(HDNHDDataContext context) : base(context)
        {
            dc = context;
        }

        public IQueryable<SoTienNopTheoThangModel> GetAllByMonthYear(int month, int year)
        {
            var dtHoaDon = new DateTime(year, month, 1).AddMonths(-1);

            return from item in dc.SoTienNopTheoThangs
                   join hd in dc.Hoadonnuocs on item.HoaDonNuocID equals hd.HoadonnuocID
                   where hd.ThangHoaDon == dtHoaDon.Month && hd.NamHoaDon == dtHoaDon.Year &&
                   hd.Trangthaiin == true && (hd.Trangthaixoa == false || hd.Trangthaixoa == null)
                   select new SoTienNopTheoThangModel()
                   {
                       SoTienNopTheoThang = item,
                       SoTienPhaiNop = (long) item.SoTienPhaiNop,
                       SoTienTrenHoaDon = item.SoTienTrenHoaDon
                   };
        }
    }
}