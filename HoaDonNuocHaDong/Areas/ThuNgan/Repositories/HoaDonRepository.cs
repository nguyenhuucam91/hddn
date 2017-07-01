<<<<<<< e91a899d106a244de0128abde9b6fef1e49ec248
﻿using HDNHD.Core.Repositories;
=======
using HDNHD.Core.Repositories;
>>>>>>> c7252917acf8a10dc488323e7a680149d908d8d5
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
<<<<<<< e91a899d106a244de0128abde9b6fef1e49ec248
            return base.GetAll().Where(m => m.Trangthaichot == true).Where(m => m.Trangthaixoa == null || m.Trangthaixoa == false);
        }

        public IQueryable<HoaDonModel> GetAllHoaDonModel(bool? trangThaiIn = null)
        {
            var hds = GetAll();

            var items = from hd in hds
                        join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                        join lshd in dc.Lichsuhoadons on hd.HoadonnuocID equals lshd.HoaDonID
                        join stntt in dc.SoTienNopTheoThangs on hd.HoadonnuocID equals stntt.HoaDonNuocID
                        join d in dc.DuCos on stntt.ID equals d.TienNopTheoThangID into gj
                        from dco in gj.DefaultIfEmpty()
                        let cnt = (from _hd in GetAll()
                                   where _hd.KhachhangID == hd.KhachhangID && _hd.HoadonnuocID < hd.HoadonnuocID && (_hd.Trangthaithu == false || _hd.Trangthaithu == null)
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
                            HoaDonTiepTheo = hdTiepTheo,
                            SoTienTrenHoaDon = stntt.SoTienTrenHoaDon,
                            DuNo = (long)((hd.Trangthaithu == true) ? 0 : (stntt.SoTienPhaiNop - stntt.SoTienDaThu))
                        };

            if (trangThaiIn != null)
            {
                items = items.Where(m => m.HoaDon.Trangthaiin == trangThaiIn);
            }

            return items;
        }

        public HoaDonModel GetHoaDonModelByID(int hoaDonID)
        {
            return GetAllHoaDonModel().FirstOrDefault(m => m.HoaDon.HoadonnuocID == hoaDonID);
        }

        public IQueryable<HoaDonModel> GetAllHoaDonModelByKHID(int khachHangID, bool? trangThaiIn = null)
        {
            return GetAllHoaDonModel(trangThaiIn).Where(m => m.KhachHang.KhachhangID == khachHangID);
        }

        /// <summary>
        /// trả về ds hóa đơn chưa thanh toán cho tới thời điểm month/ year
        /// </summary>
        public IQueryable<DuNoModel> GetAllDuNoModel(int month, int year)
        {
            var hds = GetAll().Where(m => m.Trangthaiin == true);

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
                   let ngayHoaDon = new DateTime(hd.NamHoaDon.Value, hd.ThangHoaDon.Value, 1)
                   orderby ngayHoaDon, kh.TuyenKHID, kh.TTDoc
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

        public IQueryable<KhongSanLuongModel> GetAllKhongSanLuongModel(int month, int year)
=======
            return base.GetAll().Where(m => m.Trangthaixoa == null || m.Trangthaixoa == false);
        }

        public IQueryable<HoaDonModel> GetAllHoaDonModel(bool? trangThaiIn = true)
>>>>>>> c7252917acf8a10dc488323e7a680149d908d8d5
        {
            var hds = GetAll();

            return from hd in hds
                   where hd.NamHoaDon == year && hd.ThangHoaDon == month && hd.Tongsotieuthu == 0 // không sl
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   select new KhongSanLuongModel()
                   {
                       HoaDon = hd,
                       KhachHang = kh,
                       TuyenKH = t
                   };
        }

        public IQueryable<LoaiGiaModel> GetAllLoaiGiaModel(int month, int year)
        {
            var hds = GetAll();

            return from hd in hds
                   where hd.NamHoaDon == year && hd.ThangHoaDon == month && hd.Trangthaiin == true
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join lshd in dc.Lichsuhoadons on hd.HoadonnuocID equals lshd.HoaDonID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   select new LoaiGiaModel()
                   {
                       HoaDon = hd,
                       KhachHang = kh,
                       LichSuHoaDon = lshd,
                       TuyenKH = t,
                       SoTien = (long?) lshd.TongCong
                   };
        }
    }
<<<<<<< e91a899d106a244de0128abde9b6fef1e49ec248
=======

>>>>>>> c7252917acf8a10dc488323e7a680149d908d8d5
}