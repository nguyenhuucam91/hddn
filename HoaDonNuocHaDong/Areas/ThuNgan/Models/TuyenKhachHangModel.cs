namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class TuyenKhachHangModel
    {
        public HDNHD.Models.DataContexts.Tuyenkhachhang TuyenKhachHang { get; set; }
        public HDNHD.Models.DataContexts.TuyenDuocChot TuyenDuocChot { get; set; }
        
        public int SoHoaDon { get; set; }
        public int SoHoaDonDaIn { get; set; }
    }
}