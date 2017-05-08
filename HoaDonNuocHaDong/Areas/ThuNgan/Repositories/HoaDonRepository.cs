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
                        let cnt = (from _hd in dc.Hoadonnuocs where _hd.KhachhangID == hd.KhachhangID && _hd.HoadonnuocID < _hd.HoadonnuocID select dc).Count()
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

        public IQueryable<DuNoModel> GetAllDuNoModel()
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