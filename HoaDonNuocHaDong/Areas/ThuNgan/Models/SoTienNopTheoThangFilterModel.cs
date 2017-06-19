using HDNHD.Core.Models;
using System.Linq;
using HDNHD.Models.DataContexts;
using HDNHD.Models.Constants;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class SoTienNopTheoThangFilterModel : BaseFilterModel<SoTienNopTheoThangModel>
    {
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? TuyenKHID { get; set; }

        public ELoaiKhachHang? LoaiKhachHang { get; set; }
        public EHinhThucThanhToan? HinhThucThanhToan { get; set; }

        public override void ApplyFilter(ref IQueryable<SoTienNopTheoThangModel> items)
        {
            HDNHDDataContext context = (HDNHDDataContext)GetDataContext(items);

            var query = from item in items
                        join hd in context.Hoadonnuocs on item.SoTienNopTheoThang.HoaDonNuocID equals hd.HoadonnuocID
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

            // loai kh
            if (LoaiKhachHang != null)
            {
                if (LoaiKhachHang == ELoaiKhachHang.CoQuanToChuc)
                {
                    query = query.Where(m => m.KhachHang.LoaiKHID != (int)ELoaiKhachHang.HoGiaDinh);
                }
                else
                {
                    query = query.Where(m => m.KhachHang.LoaiKHID == (int)LoaiKhachHang.Value);
                }
            }

            if (HinhThucThanhToan != null)
            {
                query = query.Where(m => m.KhachHang.HinhthucttID == (int)HinhThucThanhToan.Value);
            }

            items = from item in query
                    select item.GiaoDich;
        }
    }
}