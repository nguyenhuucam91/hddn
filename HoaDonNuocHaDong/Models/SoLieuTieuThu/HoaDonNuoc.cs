using HoaDonNuocHaDong.Config;
using HvitFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.SoLieuTieuThu
{
    public class HoaDonNuoc : ModelBase
    {
        public int HoaDonNuocID { get { return GetINT(0); } set { SetINT(0, value); } }
        public int KhachHangID { get { return GetINT(1); } set { SetINT(1, value); } }
        public String MaKhachHang { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public String TenKhachHang { get { return GetSTR(3); } set { SetINT(3, value); } }
        public int? SoHo { get { return GetINT(4); } set { SetINT(4, value); } }
        public int? SoKhau { get { return GetINT(5); } set { SetINT(5, value); } }
        public int? ChiSoCu { get { return GetINT(6); } set { SetINT(6, value); } }
        public int? ChiSoMoi { get { return GetINT(7); } set { SetINT(7, value); } }
        public int? SanLuong { get { return GetINT(8); } set { SetINT(8, value); } }
        public int? ThuTuDoc { get { return GetINT(9); } set { SetINT(9, value); } }
        public DateTime? NgayNgungCapNuoc { get { return GetDT(10); } set { SetDT(10, value); } }
        public DateTime? NgayCapNuocLai { get { return GetDT(11); } set { SetDT(11, value); } }
        public int? SoKhoan { get { return GetINT(12); } set { SetINT(12, value); } }
        public DateTime? NgayKiHopDong { get { return GetDT(13); } set { SetDT(13, value); } }
        public DateTime? NgayBatDauSuDung { get { return GetDT(14); } set { SetDT(14, value); } }
        public DateTime? NgayKetThucSuDung { get { return GetDT(15); } set { SetDT(15, value); } }
        public String SH1 { get { return GetSTR(16); } set { SetSTR(16, value); } }
        public String SH2 { get { return GetSTR(17); } set { SetSTR(17, value); } }
        public String SH3 { get { return GetSTR(18); } set { SetSTR(18, value); } }
        public String SH4 { get { return GetSTR(19); } set { SetSTR(19, value); } }
        public String HC { get { return GetSTR(20); } set { SetSTR(20, value); } }
        public String CC { get { return GetSTR(21); } set { SetSTR(21, value); } }
        public String SXXD { get { return GetSTR(22); } set { SetSTR(22, value); } }
        public String KDDV { get { return GetSTR(23); } set { SetSTR(23, value); } }
        //xem chi tiết
        public String LoaiApGia { get { return GetSTR(24); } set { SetSTR(24, value); } }
        public int? LoaiApGiaID { get { return GetINT(25); } set { SetINT(25, value); } }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public String ThangNamHoaDon { get; set; }
        public String SoHoaDon { get; set; }
        public DateTime ThangNamHoaDonToDateTime { get; set; }
        public int nhanVienId { get; set; }
        public String TrangThaiChot { get { return GetSTR(26); } set { SetSTR(26, value); } }
        public bool TrangThaiIn { get; set; }
        public DateTime ngayIn { get; set; }
        public DateTime ngayNhapHoaDon { get; set; }
        public double TongCong { get; set; }

        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}