using HDNHD.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HDNHD.Models.DataContexts;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class GiaoDichFilterModel : BaseFilterModel<GiaoDichSumModel>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? TuyenKHID { get; set; }

        public override void ApplyFilter(ref IQueryable<GiaoDichSumModel> items)
        {
            HDNHDDataContext context = (HDNHDDataContext)GetDataContext(items);

            var query = from item in items
                        join stntt in context.SoTienNopTheoThangs on item.GiaoDich.TienNopTheoThangID equals stntt.ID
                        join hd in context.Hoadonnuocs on stntt.HoaDonNuocID equals hd.HoadonnuocID
                        join kh in context.Khachhangs on hd.KhachhangID equals kh.KhachhangID
                        select new
                        {
                            GiaoDich = item,
                            KhachHang = kh
                        };

            if (TuyenKHID != null)
            {
                query = query.Where(m => m.KhachHang.TuyenKHID == TuyenKHID);
            }
            else if (NhanVienID != null) // nhan vien
            {
                query = from hdkh in query
                        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                        where ttnv.NhanVienID == NhanVienID
                        select hdkh;
            } // to
            else if (ToID != null)
            {
                query = from item in query
                        join to in context.ToQuanHuyens on item.KhachHang.QuanhuyenID equals to.QuanHuyenID
                        where to.ToQuanHuyenID == ToID
                        select item;
            } // quan huyen
            else if (QuanHuyenID != null)
            {
                query = query.Where(m => m.KhachHang.QuanhuyenID == QuanHuyenID);
            }

            items = from item in query
                    select item.GiaoDich;
        }
    }
}