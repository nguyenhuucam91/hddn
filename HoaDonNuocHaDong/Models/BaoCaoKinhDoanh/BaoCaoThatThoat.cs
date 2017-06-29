using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class BaoCaoThatThoat:ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public int KyHieuDongHo { get { return GetINT(1); } set { SetINT(1, value); } }
        public string Ten { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public int SoKH { get { return GetINT(3); } set { SetINT(3, value); } }
        public int SanLuongThangBaoCao { get { return GetINT(4); } set { SetINT(4, value); } }
        public int SanLuongThangTruoc { get { return GetINT(5); } set { SetINT(5, value); } }
        public int SanLuongPhat { get { return GetINT(6); } set { SetINT(6, value); } }
        public int LuongThatThoat { get { return GetINT(7); } set { SetINT(7, value); } }
        public int TiLeThatThoat { get { return GetINT(8); } set { SetINT(8, value); } }
        public string GhiChu{ get { return GetSTR(9); } set { SetSTR(9, value); } }
        public BaoCaoThatThoat()
        {
            MaxPosModelField = 9;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}