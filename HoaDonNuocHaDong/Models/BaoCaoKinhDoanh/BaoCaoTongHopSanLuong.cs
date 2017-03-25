using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoKinhDoanh
{
    public class BaoCaoTongHopSanLuong : ModelBase
    {
        public String TenTuyen { get { return GetSTR(0); } set { SetSTR(0, value); } }
        public double SH1Sum { get { return GetD(1); } set { SetD(1, value); } }
        public double SH2Sum { get { return GetD(2); } set { SetD(2, value); } }
        public double SH3Sum { get { return GetD(3); } set { SetD(3, value); } }
        public double SH4Sum { get { return GetD(4); } set { SetD(4, value); } }
        public double SXSum { get { return GetD(5); } set { SetD(5, value); } }
        public double HCSum { get { return GetD(6); } set { SetD(6, value); } }
        public double CCSum { get { return GetD(7); } set { SetD(7, value); } }
        public double KDSum { get { return GetD(8); } set { SetD(8, value); } }
        public double TongSL { get { return GetD(9); } set { SetD(9, value); } }
        public double SLTruocThue { get { return GetD(10); } set { SetD(10, value); } }
        public double TongVAT { get { return GetD(11); } set { SetD(11, value); } }
        public double TongBVMT { get { return GetD(12); } set { SetD(12, value); } }
        public double TongCong { get { return GetD(13); } set { SetD(13, value); } }


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