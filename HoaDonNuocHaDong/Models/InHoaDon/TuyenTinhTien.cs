using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.InHoaDon
{
    public class TuyenTinhTien : ModelBase
    {
        public int HoaDonNuoc { get { return GetINT(0); } set { SetINT(0, value); } }
        public String MaKH { get { return GetSTR(1); } set { SetSTR(1, value); } }
        public String TenKH { get { return GetSTR(2); } set { SetSTR(2, value); } }
        public String DiaChi { get { return GetSTR(3); } set { SetSTR(3, value); } }
        public DateTime NgayBatDau { get { return GetDT(4); } set { SetDT(4, value); } }
        public DateTime NgayKetThuc { get { return GetDT(5); } set { SetDT(5, value); } }
        public double SH1 { get { return GetD(6); } set { SetD(6, value); } }
        public double SH1Price { get { return GetD(7); } set { SetD(7, value); } }
        public double SH2 { get { return GetD(8); } set { SetD(8, value); } }
        public double SH2Price { get { return GetD(9); } set { SetD(9, value); } }
        public double SH3 { get { return GetD(10); } set { SetD(10, value); } }
        public double SH3Price { get { return GetD(11); } set { SetD(11, value); } }
        public double SH4 { get { return GetD(12); } set { SetD(12, value); } }
        public double SH4Price { get { return GetD(13); } set { SetD(13, value); } }
        public double HC { get { return GetD(14); } set { SetD(14, value); } }
        public double HCPrice { get { return GetD(15); } set { SetD(15, value); } }
        public double CC { get { return GetD(16); } set { SetD(16, value); } }
        public double CCPrice { get { return GetD(17); } set { SetD(17, value); } }
        public double SX { get { return GetD(18); } set { SetD(18, value); } }
        public double SXPrice { get { return GetD(19); } set { SetD(19, value); } }
        public double KD { get { return GetD(20); } set { SetD(20, value); } }
        public double KDPrice { get { return GetD(21); } set { SetD(21, value); } }
        public double TruocThue { get { return GetD(22); } set { SetD(22, value); } }
        public double PhiVAT { get { return GetD(23); } set { SetD(23, value); } }
        public double TileBVMT { get { return GetD(24); } set { SetD(24, value); } }
        public double PhiBVMT { get { return GetD(25); } set { SetD(25, value); } }
        public int TTDoc { get { return GetINT(26); } set { SetINT(26, value); } }
        public int SanLuong { get { return GetINT(27); } set { SetINT(27, value); } }
        public double TongCong { get { return GetD(28); } set { SetD(28, value); } }
        public String TTThuNgan { get { return GetSTR(29); } set { SetSTR(29, value); } }
        public int TuyenKHID { get { return GetINT(30); } set { SetINT(30, value); } }

        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}