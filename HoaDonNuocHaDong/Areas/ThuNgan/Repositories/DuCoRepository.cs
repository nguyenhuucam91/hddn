using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using System.Data.Linq;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;
using HDNHD.Models.DataContexts;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class DuCoRepository : LinqRepository<HDNHD.Models.DataContexts.DuCo>, IDuCoRepository
    {
        private HDNHDDataContext dc;

        public DuCoRepository(DataContext context) : base(context)
        {
            dc = (HDNHDDataContext)context;
        }

        IQueryable<DuCoModel> IDuCoRepository.GetAllDuCoModel()
        {
            return from d in dc.DuCos
                   join stntt in dc.SoTienNopTheoThangs on d.TienNopTheoThangID equals stntt.ID
                   join hd in dc.Hoadonnuocs on stntt.HoaDonNuocID equals hd.HoadonnuocID
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join nv in dc.Nhanviens on hd.NhanvienID equals nv.NhanvienID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   //where kh.LoaiKHID != (int)EApGia.SinhHoat
                   select new DuCoModel()
                   {
                       DuCo = d,
                       HoaDon = hd,
                       KhachHang = kh,
                       NhanVien = nv,
                       TuyenKH = t
                   };
        }
    }
}