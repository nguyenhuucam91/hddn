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

        // override lọc bỏ bản ghi đã xóa
        public override IQueryable<HDNHD.Models.DataContexts.Hoadonnuoc> GetAll()
        {
            return base.GetAll().Where(m => m.Trangthaixoa == null || m.Trangthaixoa == false);
        }

        public IQueryable<HoaDonModel> GetAllHoaDonModel(bool? trangThaiIn = true)
        {
            var hds = GetAll();

            var items = from hd in hds
                        join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                        join lshd in dc.Lichsuhoadons on hd.HoadonnuocID equals lshd.HoaDonID
                        join stntt in dc.SoTienNopTheoThangs on hd.HoadonnuocID equals stntt.HoaDonNuocID
                        join d in dc.DuCos on stntt.ID equals d.TienNopTheoThangID into gj
                        from dco in gj.DefaultIfEmpty()
                        let cnt = (from _hd in GetAll()
                                   where _hd.KhachhangID == hd.KhachhangID && _hd.HoadonnuocID < hd.HoadonnuocID && _hd.Trangthaithu == false
                                   select _hd).Count()
                        let hdTiepTheo = (from _hd in GetAll()
                                          where _hd.KhachhangID == hd.KhachhangID && _hd.HoadonnuocID > hd.HoadonnuocID
                                          select _hd).FirstOrDefault()
                        orderby kh.TuyenKHID, kh.TTDoc, hd.HoadonnuocID descending
                        select new HoaDonModel()
                        {
                            HoaDon = hd,
                            KhachHang = kh,
                            LichSuHoaDon = lshd,
                            SoTienNopTheoThang = stntt,
                            DuCo = dco,
                            CoDuNoQuaHan = cnt > 0,
                            HoaDonTiepTheo = hdTiepTheo
                        };

            if (trangThaiIn != null)
            {
                items = items.Where(m => m.HoaDon.Trangthaiin == trangThaiIn); // mặc định: đã in
            }

            return items;
        }

        /// <summary>
        /// trả về ds hóa đơn chưa thanh toán cho tới thời điểm month/ year
        /// </summary>
        public IQueryable<DuNoModel> GetAllDuNoModel(int month, int year)
        {
            var hds = GetAll();

            var dtHoaDon = new DateTime(year, month, 1).AddMonths(-1);

            return from hd in hds
                   where hd.Trangthaiin == true
                   && hd.NamHoaDon < dtHoaDon.Year || (hd.NamHoaDon == dtHoaDon.Year && hd.ThangHoaDon <= dtHoaDon.Month) // hóa đơn trong tháng hoặc trước đó
                   && ((hd.Trangthaithu == false || hd.Trangthaithu == null) || // chưa thanh toán HOẶC đã thanh toán nhưng sau thời điểm month/ year
                        (hd.Trangthaithu == true &&
                          (hd.NgayNopTien.Value.Year > year || (hd.NgayNopTien.Value.Year == year && hd.NgayNopTien.Value.Month > month))))
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join stntt in dc.SoTienNopTheoThangs on hd.HoadonnuocID equals stntt.HoaDonNuocID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   let lgd = (from gd in dc.GiaoDiches
                              where gd.TienNopTheoThangID == stntt.ID
                              where gd.NgayGiaoDich.Value.Year < year || (gd.NgayGiaoDich.Value.Year == year && gd.NgayGiaoDich.Value.Month <= month)
                              select gd).FirstOrDefault()
                   orderby kh.TuyenKHID, kh.TTDoc, hd.HoadonnuocID descending
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

        public HoaDonModel GetHoaDonModelByID(int hoaDonID, bool? trangThaiIn = true)
        {
            return GetAllHoaDonModel(trangThaiIn).FirstOrDefault(m => m.HoaDon.HoadonnuocID == hoaDonID);
        }

        public IQueryable<HoaDonModel> GetAllHoaDonModelByKHID(int khachHangID)
        {
            return GetAllHoaDonModel().Where(m => m.KhachHang.KhachhangID == khachHangID);
        }
    }
}