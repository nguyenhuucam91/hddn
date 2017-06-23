using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.SoLieuTieuThu
{
    public class HoaDonNuoc
    {
        public int HoaDonNuocID { get; set; }
        public int KhachHangID { get; set; }
        public String MaKhachHang { get; set; }
        public String TenKhachHang { get; set; }
        public int? SoHo { get; set; }
        public int? SoKhau { get; set; }
        public int? ChiSoCu { get; set; }
        public int? ChiSoMoi { get; set; }
        public int? SanLuong { get; set; }
        public int? ThuTuDoc { get; set; }
        //xem chi tiết
        public String LoaiApGia { get; set; }
        public int? LoaiApGiaID { get; set; }
        public int? SoKhoan { get; set; }
        public String SH1 { get; set; }
        public String SH2 { get; set; }
        public String SH3 { get; set; }
        public String SH4 { get; set; }
        public String SXXD { get; set; }
        public String HC { get; set; }
        public String CC { get; set; }
        public String KDDV { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public String ThangNamHoaDon { get; set; }
        public int KHID { get; set; }
        public String SoHoaDon { get; set; }

        public DateTime? NgayKiHopDong { get; set; }
        public bool TrangThaiChot { get; set; }

        public DateTime? NgayNgungCapNuoc { get; set; }
        public DateTime? NgayCapNuocLai { get; set; }

        public double TongCong { get; set; }
    }
}