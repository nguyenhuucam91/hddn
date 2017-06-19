using HDNHD.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class DoanhThuFilterModel
    {
        public string Mode { get; set; }
        public int? QuanHuyenID { get; set; }
        public int? ToID { get; set; }
        public int? NhanVienID { get; set; }
        public int? TuyenKHID { get; set; }
        public ELoaiKhachHang? LoaiKhachHang { get; set; }
        public EHinhThucThanhToan? HinhThucThanhToan { get; set; }
    }
}