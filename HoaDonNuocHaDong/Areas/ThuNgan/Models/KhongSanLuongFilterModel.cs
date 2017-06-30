using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HDNHD.Models.DataContexts;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class KhongSanLuongFilterModel : BaseFilterModel<KhongSanLuongModel>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? TuyenKHID { get; set; }
       
        public override void ApplyFilter(ref IQueryable<KhongSanLuongModel> items)
        {
            var context = (HDNHDDataContext)GetDataContext(items);

            //* filter by management info
            if (TuyenKHID != null)
            {
                items = items.Where(m => m.KhachHang.TuyenKHID == TuyenKHID);
            }
            else if (NhanVienID != null) // nhan vien
            {
                items = from hdkh in items
                        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                        where ttnv.NhanVienID == NhanVienID
                        select hdkh;
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