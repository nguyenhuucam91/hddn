namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{    
    public class HoaDonModel
    {
        public HDNHD.Models.DataContexts.Hoadonnuoc HoaDon { get; set; }
        public HDNHD.Models.DataContexts.Khachhang KhachHang { get; set; }
        public HDNHD.Models.DataContexts.SoTienNopTheoThang SoTienNopTheoThang { get; set; } // not nullable

        public HDNHD.Models.DataContexts.DuCo DuCo { get; set; } // nullable
    }

    public class DuCoModel
    {
        public HDNHD.Models.DataContexts.Hoadonnuoc HoaDon { get; set; }
        public HDNHD.Models.DataContexts.Khachhang KhachHang { get; set; }
        public HDNHD.Models.DataContexts.Nhanvien NhanVien { get; set; }

        public HDNHD.Models.DataContexts.DuCo DuCo { get; set; }
        public HDNHD.Models.DataContexts.Tuyenkhachhang TuyenKH { get; set; }
    }

    public class DuNoModel
    {
        public HDNHD.Models.DataContexts.Hoadonnuoc HoaDon { get; set; }
        public HDNHD.Models.DataContexts.Khachhang KhachHang { get; set; }
        public HDNHD.Models.DataContexts.Nhanvien NhanVien { get; set; }

        public HDNHD.Models.DataContexts.SoTienNopTheoThang SoTienNopTheoThang { get; set; }
        public HDNHD.Models.DataContexts.Tuyenkhachhang TuyenKhachHang { get; set; }
    }
}