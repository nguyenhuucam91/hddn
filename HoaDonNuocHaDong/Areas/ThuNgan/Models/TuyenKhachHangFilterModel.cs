using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class TuyenKhachHangFilterModel : BaseFilterModel<TuyenKhachHangModel>
    {
        public int? Month { get; set; }
        public int? Year { get; set; }

        public TuyenKhachHangFilterModel()
        {
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
        }

        public override void ApplyFilter(ref IQueryable<TuyenKhachHangModel> items)
        {
            HDNHDDataContext context = (HDNHDDataContext)GetDataContext(items);

            items = from item in items
                    join tdc in context.TuyenDuocChots on item.TuyenKhachHang.TuyenKHID equals tdc.TuyenKHID
                    where tdc.Nam == Year && tdc.Thang == Month
                    join kh in context.Khachhangs on item.TuyenKhachHang.TuyenKHID equals kh.TuyenKHID
                    join hd in context.Hoadonnuocs on kh.KhachhangID equals hd.KhachhangID
                    where hd.NamHoaDon == Year && hd.ThangHoaDon == Month
                    group new { item, tdc, hd } by tdc.TuyenKHID into g
                    select new TuyenKhachHangModel()
                    {
                        TuyenKhachHang = g.Select(m => m.item.TuyenKhachHang).FirstOrDefault(),
                        TuyenDuocChot = g.Select(m => m.tdc).FirstOrDefault(),
                        SoHoaDon = g.Select(m => m.hd).Count(),
                        SoHoaDonDaIn = g.Select(m => m.hd.Trangthaiin == true).Count()
                    };
        }
    }
}