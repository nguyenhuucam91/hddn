using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHangTheoNhanVien:ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public string TuyenKHID { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public int ThuTuDoc { get { return GetINT(2); } set { SetINT(2, value); } }
        public int TuyenOng { get { return GetINT(3); } set { SetINT(3, value);}  }
        public string MaKhachHang { get { return GetSTR(4); } set { SetSTR(4, value); } }
        public string Ten { get { return GetSTR(5); } set { SetSTR(5, value); } }
        public string Diachi { get { return GetSTR(6); } set { SetSTR(6, value); } }
        public string Ghichu { get { return GetSTR(7); } set { SetSTR(7, value); } }
        public DanhSachKhachHangTheoNhanVien()
        {
            MaxPosModelField = 6;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}