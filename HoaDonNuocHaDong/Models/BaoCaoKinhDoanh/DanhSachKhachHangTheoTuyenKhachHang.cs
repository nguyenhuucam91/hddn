using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHangTheoTuyenKhachHang:ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public string ThuTuDoc { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public string MaKhachHang { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public string Ten { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public string Diachi { get { return GetSTR(4); } set { SetSTR(4, value); } }
        public string Ghichu { get { return GetSTR(5); } set { SetSTR(5, value); } }
        public string ChiSoMoi { get { return GetSTR(6); } set { SetSTR(6, value); } }
        public DanhSachKhachHangTheoTuyenKhachHang()
        {
            MaxPosModelField = 5;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}