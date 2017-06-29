using HDNHD.Core.Models;
using HDNHD.Models.Constants;
using HDNHD.Models.DataContexts;
using System.Linq;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class DuCoFilterModel : BaseFilterModel<DuCoModel>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? TuyenKHID { get; set; }

        public ELoaiKhachHang? LoaiKhachHang { get; set; }
        public EHinhThucThanhToan? HinhThucThanhToan { get; set; }

        public override void ApplyFilter(ref IQueryable<DuCoModel> items)
        {
            HDNHDDataContext context = (HDNHDDataContext)GetDataContext(items);

            if (TuyenKHID != null)
            {
                items = items.Where(m => m.KhachHang.TuyenKHID == TuyenKHID);
            } else if (NhanVienID != null) // nhan vien
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
                //items = from hdkh in items
                //        join ttnv in context.Tuyentheonhanviens on hdkh.KhachHang.TuyenKHID equals ttnv.TuyenKHID
                //        join nv in context.Nhanviens on ttnv.NhanVienID equals nv.NhanvienID
                //        where nv.ToQuanHuyenID == ToID
                //        group hdkh by hdkh.HoaDon.HoadonnuocID into g
                //        select g.First();
            } // quan huyen
            else if (QuanHuyenID != null)
            {
                items = items.Where(m => m.KhachHang.QuanhuyenID == QuanHuyenID);
            }

            // loai kh
            if (LoaiKhachHang != null)
            {
                if (LoaiKhachHang == ELoaiKhachHang.CoQuanToChuc)
                {
                    items = items.Where(m => m.KhachHang.LoaiKHID != (int)ELoaiKhachHang.HoGiaDinh);
                }
                else
                {
                    items = items.Where(m => m.KhachHang.LoaiKHID == (int)LoaiKhachHang.Value);
                }
            }

            if (HinhThucThanhToan != null)
            {
                items = items.Where(m => m.KhachHang.HinhthucttID == (int)HinhThucThanhToan.Value);
            }
        }
    }
}