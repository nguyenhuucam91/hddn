using HDNHD.Core.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using System.Data.Linq;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using System.Linq;
using HDNHD.Models.DataContexts;
using System;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class DuCoRepository : LinqRepository<HDNHD.Models.DataContexts.DuCo>, IDuCoRepository
    {
        private HDNHDDataContext dc;

        public DuCoRepository(DataContext context) : base(context)
        {
            dc = (HDNHDDataContext)context;
        }

        public IQueryable<DuCoModel> GetAllDuCoModel(int month, int year)
        {
            var dtHoaDon = new DateTime(year, month, 1).AddMonths(-1); 

            return from d in dc.DuCos
                   join stntt in dc.SoTienNopTheoThangs on d.TienNopTheoThangID equals stntt.ID
                   join hd in dc.Hoadonnuocs on stntt.HoaDonNuocID equals hd.HoadonnuocID
                   where (hd.NamHoaDon < dtHoaDon.Year || (hd.NamHoaDon == dtHoaDon.Year && hd.ThangHoaDon <= dtHoaDon.Month)) &&
                    (d.TrangThaiTruHet == false ||
                    (d.TrangThaiTruHet == true && (d.NgayTruHet.Value.Year > year ||
                    (d.NgayTruHet.Value.Year == year && d.NgayTruHet.Value.Month > month)))) // chưa trừ HOẶC đã trừ nhưng sau thời điểm month/ year
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   join t in dc.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                   let lgd = (from gd in dc.GiaoDiches
                              where gd.TienNopTheoThangID == stntt.ID
                              where gd.NgayGiaoDich.Value.Year < year || (gd.NgayGiaoDich.Value.Year == year && gd.NgayGiaoDich.Value.Month <= month)
                              select gd).FirstOrDefault() // nullable
                   orderby kh.TuyenKHID, kh.TTDoc, hd.HoadonnuocID descending
                   //where kh.LoaiKHID != (int)EApGia.SinhHoat
                   select new DuCoModel()
                   {
                       DuCo = d,
                       SoTienNopTheoThang = stntt,
                       HoaDon = hd,
                       KhachHang = kh,
                       TuyenKH = t,
                       SoTien = lgd != null ? d.SoTienDu - lgd.SoDu : 0 // sau giao dịch phát sinh dư có, GiaoDich.SoDu = dư có trước đó
                   };
        }
    }
}