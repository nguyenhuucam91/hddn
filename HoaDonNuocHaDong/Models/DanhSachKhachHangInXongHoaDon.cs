using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class DanhSachKhachHangInXongHoaDon
    {
        public string MaKhachHang { get; set; }
        public string Ten { get; set; }
        public string Diachi { get; set; }
        public string Dienthoai { get; set; }
        public string MucApGia { get; set; }
        public Nullable<int> Tongsotieuthu { get; set; }
        public Nullable<int> Tongsotien { get; set; }
    }
}