using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HDNHD.Models.DataContexts;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class HoaDonRepository : LinqRepository<HDNHD.Models.DataContexts.Hoadonnuoc>, IHoaDonRepository
    {
        public HoaDonRepository(DataContext context) : base(context) { }

        public IQueryable<HoaDonModel> GetAllModel()
        {
            HDNHDDataContext dc = (HDNHDDataContext)context;

            // join with KhachHang
            var items = from hd in dc.Hoadonnuocs
                    join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                    join stntt in dc.SoTienNopTheoThangs
                    on hd.SoTienNopTheoThangID equals stntt.ID
                    where hd.Trangthaiin == true && hd.Trangthaixoa != true
                    select new HoaDonModel()
                    {
                        HoaDon = hd,
                        KhachHang = kh,
                        SoTienNopTheoThang = stntt
                    };
            
            return items;
        }
    }
}