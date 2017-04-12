using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoInHoaDon
{
    public class BaoCaoHoaDonNhan
    {
        public int ID { get; set; }
        public String MaKH { get; set; }
        public String TenKH { get; set; }
        public int? LoaiKH { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public double TongTien { get; set; }
        public int? TuyenKHID { get; set; }
    }
}