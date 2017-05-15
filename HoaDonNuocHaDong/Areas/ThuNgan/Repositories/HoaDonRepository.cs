using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HDNHD.Models.DataContexts;
using System.Linq;
using System;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class HoaDonRepository : LinqRepository<HDNHD.Models.DataContexts.Hoadonnuoc>, IHoaDonRepository
    {
        private HDNHDDataContext dc;

        public HoaDonRepository(HDNHDDataContext context) : base(context)
        {
            dc = context;
        }

        public IQueryable<HoaDonModel> GetAllHoaDonModel()
        {

            var items = from hd in dc.Hoadonnuocs
                        where hd.Trangthaiin == true && (hd.Trangthaixoa == false || hd.Trangthaixoa == null)
                        join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                        join stntt in dc.SoTienNopTheoThangs on hd.SoTienNopTheoThangID equals stntt.ID
                        join d in dc.DuCos on stntt.ID equals d.TienNopTheoThangID into gj
                        from dco in gj.DefaultIfEmpty()
                        join chitietHd in dc.Chitiethoadonnuocs on hd.HoadonnuocID equals chitietHd.HoadonnuocID
                        let cnt = (from _hd in dc.Hoadonnuocs
                                   where _hd.KhachhangID == hd.KhachhangID && _hd.HoadonnuocID < hd.HoadonnuocID
                                       && (_hd.Trangthaixoa == false || _hd.Trangthaixoa == null)
                                       && (_hd.Trangthaithu == false || _hd.Trangthaithu == null)
                                   select dc).Count()
                        orderby kh.TuyenKHID
                        orderby kh.TTDoc
                        select new HoaDonModel()
                        {
                            HoaDon = hd,
                            KhachHang = kh,
                            SoTienNopTheoThang = stntt,
                            DuCo = dco,
                            ChiTietHoaDon = chitietHd,
                            CoDuNoQuaHan = cnt > 0
                        };
            
            return items;
        }

        /// <summary>
        /// trả về ds hóa đơn chưa thanh toán cho tới thời điểm month/ year
        /// </summary>
        public IQueryable<DuNoModel> GetAllDuNoModel(int month, int year)
        {
            return from hd in dc.Hoadonnuocs
                   where hd.Trangthaiin == true && (hd.Trangthaixoa == false || hd.Trangthaixoa == null)
                   && ((hd.Trangthaithu == false || hd.Trangthaithu == null)) || // chưa thanh toán HOẶC đã thanh toán nhưng sau thời điểm month/ year
                        (hd.Trangthaithu == true &&
                          (hd.NgayNopTien.Value.Year > year || (hd.NgayNopTien.Value.Year == year && hd.NgayNopTien.Value.Month > month)))
                           && hd.NamHoaDon < year || (hd.NamHoaDon == year && hd.ThangHoaDon <= month) // hóa đơn trong tháng hoặc trước đó
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join stntt in dc.SoTienNopTheoThangs on hd.HoadonnuocID equals stntt.HoaDonNuocID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   let lgd = (from gd in dc.GiaoDiches
                              where gd.TienNopTheoThangID == stntt.ID
                              where gd.NgayGiaoDich.Value.Year < year || (gd.NgayGiaoDich.Value.Year == year && gd.NgayGiaoDich.Value.Month <= month)
                              select gd).FirstOrDefault()
                   orderby kh.TuyenKHID
                   orderby kh.TTDoc
                   select new DuNoModel()
                   {
                       HoaDon = hd,
                       KhachHang = kh,
                       SoTienNopTheoThang = stntt,
                       TuyenKhachHang = t,
                       LastGiaoDich = lgd,
                       SoTienDaNop = lgd != null ? (long)(stntt.SoTienPhaiNop - lgd.SoDu) : 0,
                       SoTienNo = (long)(lgd != null ? lgd.SoDu : stntt.SoTienPhaiNop),
                   };
        }

        public HoaDonModel GetHoaDonModelByID(int hoaDonID)
        {
            return GetAllHoaDonModel().FirstOrDefault(m => m.HoaDon.HoadonnuocID == hoaDonID);
        }

        public IQueryable<HoaDonModel> GetAllHoaDonModelByKHID(int khachHangID)
        {
            return GetAllHoaDonModel().Where(m => m.KhachHang.KhachhangID == khachHangID);
        }
    }
}