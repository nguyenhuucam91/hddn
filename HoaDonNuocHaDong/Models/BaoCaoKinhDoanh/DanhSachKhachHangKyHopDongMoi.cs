using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHangKyHopDongMoi:ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public string MaTuyen { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public int ThuTuDoc { get { return GetINT(2); } set { SetINT(2, value); } }
        public string MaKhachHang { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public string Ten { get { return GetSTR(4); } set { SetSTR(4, value); } }
        public string Diachi { get { return GetSTR(5); } set { SetSTR(5, value); } }
        public DateTime Ngaykyhopdong { get { return GetDT(6); } set { SetDT(6, value); } }
        public String Sohopdong { get { return GetSTR(7); } set { SetSTR(7, value); } }
        public DateTime Ngaylapdat { get { return GetDT(8); } set { SetDT(8, value); } }
        public int Chisolapdat { get { return GetINT(9); } set { SetINT(9, value); } }
        public DanhSachKhachHangKyHopDongMoi()
        {
            MaxPosModelField = 9;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }

    }
}