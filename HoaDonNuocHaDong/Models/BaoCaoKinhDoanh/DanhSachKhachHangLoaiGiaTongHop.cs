using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class DanhSachKhachHangLoaiGiaTongHop:ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public string MaKhachHang { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public string Ten { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public string Diachi { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public int ThuTuDoc { get { return GetINT(4); } set { SetINT(4, value); } }
        public string Tuyen { get { return GetSTR(5); } set { SetSTR(5, value); } }
        public string CachTinh { get { return GetSTR(6); } set { SetSTR(6, value); } }
        public double SinhHoat { get { return GetD(7); } set { SetD(7, value); } }
        public double SXXD { get { return GetD(8); } set { SetD(8, value); } }
        public double HanhChinh { get { return GetD(9); } set { SetD(9, value); } }
        public double CongCong { get { return GetD(10); } set { SetD(10, value); } }
        public double KinhDoanh { get { return GetD(11); } set { SetD(11, value); } }
        public DanhSachKhachHangLoaiGiaTongHop()
        {
            MaxPosModelField = 11;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}