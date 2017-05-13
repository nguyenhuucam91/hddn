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

        IQueryable<DuCoModel> IDuCoRepository.GetAllDuCoModel(int month, int year)
        {
            return from d in dc.DuCos
                   join stntt in dc.SoTienNopTheoThangs on d.TienNopTheoThangID equals stntt.ID
                   join hd in dc.Hoadonnuocs on stntt.HoaDonNuocID equals hd.HoadonnuocID
                   where (hd.NamHoaDon < year || (hd.NamHoaDon == year && hd.ThangHoaDon <= month)) &&
                    (d.TrangThaiTruHet == false ||
                    (d.TrangThaiTruHet == true && (d.NgayTruHet.Value.Year > year ||
                    (d.NgayTruHet.Value.Year == year && d.NgayTruHet.Value.Month > month)))) // chưa trừ HOẶC đã trừ nhưng sau thời điểm month/ year
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   let lgd = (from gd in dc.GiaoDiches
                              where gd.TienNopTheoThangID == stntt.ID
                              where gd.NgayGiaoDich.Value.Year < year || (gd.NgayGiaoDich.Value.Year == year && gd.NgayGiaoDich.Value.Month <= month)
                              select gd).FirstOrDefault()
                   orderby kh.TuyenKHID
                   orderby kh.TTDoc
                   //where kh.LoaiKHID != (int)EApGia.SinhHoat
                   select new DuCoModel()
                   {
                       DuCo = d,
                       HoaDon = hd,
                       KhachHang = kh,
                       TuyenKH = t,
                       SoTien = (d.SoTienDu - lgd.SoDu) ?? 0
                   };
        }
    }
}