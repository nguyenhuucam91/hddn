using System;
using System.Linq;
using HDNHD.Core.Repositories;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Repositories
{
    public class GiaoDichRepository : LinqRepository<HDNHD.Models.DataContexts.GiaoDich>, IGiaoDichRepository
    {
        private HDNHDDataContext dc;

        public GiaoDichRepository(HDNHDDataContext context) : base(context)
        {
            dc = context;
        }

        public IQueryable<GiaoDichSumModel> GetAllByMonthYear(int month, int year)
        {
            return from gd in dc.GiaoDiches
                   where gd.NgayGiaoDich.Value.Month == month && gd.NgayGiaoDich.Value.Year == year
                   join stntt in dc.SoTienNopTheoThangs on gd.TienNopTheoThangID equals stntt.ID
                   join hd in dc.Hoadonnuocs on stntt.HoaDonNuocID equals hd.HoadonnuocID
                   join kh in dc.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                   select new GiaoDichSumModel()
                   {
                       GiaoDich = gd,
                       SoTien = gd.SoTien,
                       IsChuyenKhoan = kh.HinhthucttID == (int) HDNHD.Models.Constants.EHinhThucThanhToan.ChuyenKhoan
                   };
        }

        public IQueryable<GiaoDichModel> GetAllGiaoDichModel()
        {
            return from gd in dc.GiaoDiches
                   join stntt in dc.SoTienNopTheoThangs on gd.TienNopTheoThangID equals stntt.ID
                   join hd in dc.Hoadonnuocs on stntt.HoaDonNuocID equals hd.HoadonnuocID
                   join d in dc.DuCos on stntt.ID equals d.TienNopTheoThangID into gj
                   from dco in gj.DefaultIfEmpty()
                   orderby gd.GiaoDichID descending

                   select new GiaoDichModel()
                   {
                       GiaoDich = gd,
                       HoaDon = hd,
                       SoTienNopTheoThang = stntt,
                       DuCo = dco
                   };
        }

        public IQueryable<GiaoDichModel> GetAllGiaoDichModelByKHID(int khachHangID)
        {
            return GetAllGiaoDichModel().Where(m => m.HoaDon.KhachhangID == khachHangID);
        }

        public HDNHD.Models.DataContexts.GiaoDich GetGDThanhToanByHDID(int hoaDonID)
        {
            return (from gd in dc.GiaoDiches
                    join stntt in dc.SoTienNopTheoThangs on gd.TienNopTheoThangID equals stntt.ID
                    where stntt.HoaDonNuocID == hoaDonID && gd.SoDu == 0
                    orderby gd.GiaoDichID // nếu có nhiều gd sau thanh toán, lấy gd đầu tiên làm thay đổi TrangThaiThu
                    select gd
                   ).FirstOrDefault();
        }

        public GiaoDichModel GetGiaoDichModelByID(int id)
        {
            return GetAllGiaoDichModel().FirstOrDefault(m => m.GiaoDich.GiaoDichID == id);
        }

        public GiaoDichModel GetLastGiaoDichByKHID(int khachHangID)
        {
            return GetAllGiaoDichModelByKHID(khachHangID).FirstOrDefault();
        }
    }
}