using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class DuCoModel
    {
        public HDNHD.Models.DataContexts.Hoadonnuoc HoaDon { get; set; }
        public HDNHD.Models.DataContexts.Khachhang KhachHang { get; set; }
        public HDNHD.Models.DataContexts.Nhanvien NhanVien { get; set; }

        public HDNHD.Models.DataContexts.DuCo DuCo { get; set; }
        public HDNHD.Models.DataContexts.Tuyenkhachhang TuyenKH { get; set; }
    }
}