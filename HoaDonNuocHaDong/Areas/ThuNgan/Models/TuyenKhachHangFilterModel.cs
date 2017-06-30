using HDNHD.Core.Models;
using HDNHD.Models.DataContexts;
using System;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class TuyenKhachHangFilterModel : BaseFilterModel<HDNHD.Models.DataContexts.Tuyenkhachhang>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        
        public override void ApplyFilter(ref IQueryable<HDNHD.Models.DataContexts.Tuyenkhachhang> items)
        {
            HDNHDDataContext context = (HDNHDDataContext)GetDataContext(items);
            
            if (NhanVienID != null) // nhan vien
            {
                items = from item in items
                        join ttnv in context.Tuyentheonhanviens on item.TuyenKHID equals ttnv.TuyenKHID
                        where ttnv.NhanVienID == NhanVienID
                        select item;
            } // to
            else if (ToID != null)
            {
                items = from item in items
                        join ttnv in context.Tuyentheonhanviens on item.TuyenKHID equals ttnv.TuyenKHID
                        join nv in context.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                        where nv.ToQuanHuyenID == ToID
                        select item;
            } // quan huyen
            else if (QuanHuyenID != null)
            {
                items = from item in items
                        join ttnv in context.Tuyentheonhanviens on item.TuyenKHID equals ttnv.TuyenKHID
                        join nv in context.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                        join to in context.ToQuanHuyens on nv.ToQuanHuyenID equals to.ToQuanHuyenID
                        where to.QuanHuyenID == QuanHuyenID
                        select item;
            }

            // select distinct result if duplicate on TuyenTheoNhanVien
            items = items.Distinct();
        }
    }
}