using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using System.Data.Linq;
using System.Linq;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HDNHD.Models.DataContexts;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class HoaDonRepository : LinqRepository<HDNHD.Models.DataContexts.Hoadonnuoc>, IHoaDonRepository
    {
        private HDNHDDataContext dc;

        public HoaDonRepository(DataContext context) : base(context)
        {
            dc = (HDNHDDataContext)context;
        }

        public IQueryable<HoaDonModel> GetAllAsHoaDonModel()
        {
            return from hd in dc.Hoadonnuocs
                   where hd.Trangthaiin == true && (hd.Trangthaixoa == false || hd.Trangthaixoa == null)
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join stntt in dc.SoTienNopTheoThangs on hd.SoTienNopTheoThangID equals stntt.ID
                   select new HoaDonModel()
                   {
                       HoaDon = hd,
                       KhachHang = kh,
                       SoTienNopTheoThang = stntt
                   };

        }
        
        public IQueryable<DuNoModel> GetAllAsDuNoModel()
        {
            return from hd in dc.Hoadonnuocs
                   where hd.Trangthaithu == false || hd.Trangthaithu == null
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join nv in dc.Nhanviens on hd.NhanvienID equals nv.NhanvienID
                   join stntt in dc.SoTienNopTheoThangs on hd.HoadonnuocID equals stntt.HoaDonNuocID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   select new DuNoModel()
                   {
                       HoaDon = hd,
                       KhachHang = kh,
                       NhanVien = nv,
                       SoTienNopTheoThang = stntt,
                       TuyenKhachHang = t
                   };
        }
    }
}