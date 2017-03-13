using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class BaoCaoTongHopSanLuong : ModelBase
    {
        public int TT { get { return GetINT(0); } set { SetINT(0, value); } }
        public double SH1 { get { return GetD(1); } set { SetD(1, value); } }
        public double SH2 { get { return GetD(2); } set { SetD(2, value); } }
        public double SH3 { get { return GetD(3); } set { SetD(3, value); } }
        public double SH4 { get { return GetD(4); } set { SetD(4, value); } }
        public double SXXD { get { return GetD(5); } set { SetD(5, value); } }
        public double HanhChinh { get { return GetD(6); } set { SetD(6, value); } }
        public double CongCong { get { return GetD(7); } set { SetD(7, value); } }
        public double KinhDoanh { get { return GetD(8); } set { SetD(8, value); } }
        public double TruocThue { get { return GetD(9); } set { SetD(9, value); } }
        public double ThueVAT { get { return GetD(10); } set { SetD(10, value); } }
        public double PhiNT { get { return GetD(11); } set { SetD(11, value); } }
        public double Tong { get { return GetD(12); } set { SetD(12, value); } }
        public double TongSanLuong { get { return GetD(13); } set { SetD(13, value); } }
      
        public BaoCaoTongHopSanLuong()
        {
            MaxPosModelField = 13;
        }
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}