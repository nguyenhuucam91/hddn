using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHangCoSanLuongDotBien:ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public string Tuyen { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public int ThuTuDoc { get { return GetINT(2); } set { SetINT(2, value); } }
        public string MaKhachHang { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public string Ten { get { return GetSTR(4); } set { SetSTR(4, value); } }
        public string Diachi { get { return GetSTR(5); } set { SetSTR(5, value); } }
        public int SanLuongBinhQuan3ThangTruoc { get { return GetINT(6); } set { SetINT(6, value); } }
        public int SanLuongThangTruoc { get { return GetINT(7); } set { SetINT(7, value); } }
        public int ChiSoDauKy { get { return GetINT(8); } set { SetINT(8, value); } }
        public int ChiSoCuoiKy { get { return GetINT(9); } set { SetINT(9, value); } }
        public int TongSanLuong { get { return GetINT(10); } set { SetINT(10, value); } }

        public DanhSachKhachHangCoSanLuongDotBien()
        {
            MaxPosModelField = 10;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}