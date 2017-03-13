using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class BaoCaoTongHopKhachHangQuanLy:ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public int DongHoThongKeDauKy { get { return GetINT(1); } set { SetINT(1, value); } }
        public int KhoanThongKeDauKy { get { return GetINT(2); } set { SetINT(2, value); } }
        public int TongThongKeDauKy { get { return GetINT(3); } set { SetINT(3, value); } }
        public int  DongHoTangTrongKy{ get { return GetINT(4); } set { SetINT(4, value); } }
        public int  KhoanTangTrongKy{ get { return GetINT(5); } set { SetINT(5, value); } }
        public int TongTangTrongKy { get { return GetINT(6); } set { SetINT(6, value); } }
        public int DongHoGiamTrongKy { get { return GetINT(7); } set { SetINT(7, value); } }
        public int  KhoanGiamTrongKy{ get { return GetINT(8); } set { SetINT(8, value); } }
        public int TongGiamTrongKy { get { return GetINT(9); } set { SetINT(9, value); } }
        public int DongHoThongKeCuoiKy { get { return GetINT(10); } set { SetINT(10, value); } }
        public int KhoanThongKeCuoiKy { get { return GetINT(11); } set { SetINT(11, value); } }
        public int TongThongKeCuoiKy { get { return GetINT(12); } set { SetINT(12, value); } }
        
        public BaoCaoTongHopKhachHangQuanLy()
        {
            MaxPosModelField = 12;
        }
        protected override Type TransferType()
        {
            return this.GetType();    
        }
    }
}