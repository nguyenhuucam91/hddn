using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.KhachHang
{
    public class KiemDinhModel
    {
        public int KhachHangID { get; set; }
        public String MaKhachHang { get; set; }
        public String TenKhachHang { get; set; }
        public DateTime NgayKiemDinh { get; set; }
        public int ChiSoCu { get; set; }
        public int ChiSoMoi { get; set; }
        public int ChiSoLucKiemDinh { get; set; }
        public int ChiSoSauKiemDinh { get; set; }
        public int KiemDinhID { get; set; }
        public String GhiChu { get; set; }

    }
}