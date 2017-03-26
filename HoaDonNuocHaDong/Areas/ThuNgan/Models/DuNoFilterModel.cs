using HDNHD.Core.Models;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class DuNoFilterModel : BaseFilterModel<DuNoModel>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override void ApplyFilter(ref IQueryable<DuNoModel> items)
        {
            var context = (HDNHDDataContext)GetDataContext(items);

            items = items.Where(m => m.HoaDon.Trangthaithu != true);
            // year
            if (Year != null)
            {
                // month
                if (Month != null)
                {
                    items = items.Where(m => m.HoaDon.NamHoaDon < Year || (m.HoaDon.NamHoaDon == Year && m.HoaDon.ThangHoaDon <= Month));
                }
                else
                {
                    items = items.Where(m => m.HoaDon.NamHoaDon <= Year);
                }
            }

            // join all
            items = from hdm in items
                    join kh in context.Khachhangs on hdm.HoaDon.KhachhangID equals kh.KhachhangID
                    join nv in context.Nhanviens on hdm.HoaDon.NhanvienID equals nv.NhanvienID
                    join stntt in context.SoTienNopTheoThangs on hdm.HoaDon.HoadonnuocID equals stntt.HoaDonNuocID
                    join t in context.Tuyenkhachhangs on kh.TuyenKHID equals t.TuyenKHID
                    select new DuNoModel()
                    {
                        HoaDon = hdm.HoaDon,
                        KhachHang = kh,
                        NhanVien = nv,
                        SoTienNopTheoThang = stntt,
                        TuyenKhachHang = t
                    };

            // nhan vien
            if (NhanVienID != null)
            {
                items = items.Where(m => m.NhanVien.NhanvienID == NhanVienID);
            } // to
            else if (ToID != null)
            {
                items = from item in items
                        join to in context.ToQuanHuyens on item.KhachHang.QuanhuyenID equals to.QuanHuyenID
                        where to.ToQuanHuyenID == ToID
                        select item;
            } // quan huyen
            else if (QuanHuyenID != null) // find by QuanHuyenID
            {
                items = items.Where(m => m.KhachHang.QuanhuyenID == QuanHuyenID);
            }
        }
    }
}