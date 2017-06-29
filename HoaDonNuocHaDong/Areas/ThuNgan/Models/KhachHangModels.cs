namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class KhachHangModel
    {
        public HDNHD.Models.DataContexts.Khachhang KhachHang { get; set; }
    }

    public class KhachHangDetailsModel
    {
        public HDNHD.Models.DataContexts.Khachhang KhachHang { get; set; }
        public HDNHD.Models.DataContexts.Tuyenkhachhang TuyenKH { get; set; }
        public HDNHD.Models.DataContexts.Quanhuyen QuanHuyen { get; set; }
        public HDNHD.Models.DataContexts.Phuongxa PhuongXa { get; set; }
        public HDNHD.Models.DataContexts.Cumdancu CumDanCu { get; set; }
        public HDNHD.Models.DataContexts.Tuyenong TuyenOng { get; set; }
    }
}