using HDNHD.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HDNHD.Models.DataContexts;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class KhachHangFilterModel : BaseFilterModel<KhachHangModel>
    {
        public const string FilterByManagementInfo = "FilterByManagementInfo";
        public const string FilterByUserInfo = "FilterByUserInfo";

        // filter by management info
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? TuyenKHID { get; set; }

        // filter by customer info
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string DiaChiKH { get; set; }
        
        public KhachHangFilterModel()
        {
            Mode = FilterByManagementInfo;
        }
        
        public override void ApplyFilter(ref IQueryable<KhachHangModel> items)
        {
            if (Mode == FilterByManagementInfo) // default search mode
            {
                var context = (HDNHDDataContext)GetDataContext(items);

                if (TuyenKHID != null) // find by TuyenKHID
                {
                    items = items.Where(m => m.KhachHang.TuyenKHID == TuyenKHID);
                }
                else if (NhanVienID != null) // find by NhanVienID
                {
                    items = from item in items
                            join ttnv in context.Tuyentheonhanviens on item.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                            where ttnv.NhanVienID == NhanVienID
                            select item;
                }
                else if (ToID != null) // find by ToID
                {
                    items = from item in items
                            join ttnv in context.Tuyentheonhanviens on item.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                            join nv in context.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                            where nv.ToQuanHuyenID == ToID
                            select item;
                }
                else if (QuanHuyenID != null) // find by QuanHuyenID
                {
                    items = items.Where(m => m.KhachHang.QuanhuyenID == QuanHuyenID);
                }
            } 
            
            if (Mode == FilterByUserInfo)
            {
                if (MaKH != null)
                    items = items.Where(m => m.KhachHang.MaKhachHang == MaKH);
                if (TenKH != null)
                    items = items.Where(m => m.KhachHang.Ten.Contains(TenKH));
                if (DiaChiKH != null)
                    items = items.Where(m => m.KhachHang.Diachi.Contains(DiaChiKH));
            }
        }
    }
}