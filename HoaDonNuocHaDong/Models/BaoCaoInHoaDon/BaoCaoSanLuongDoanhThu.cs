using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoInHoaDon
{
    public class BaoCaoSanLuongDoanhThu : ModelBase
    {
        public double SanLuongSH1 { get { return GetD(0); } set { SetD(0, value); } }
        public double SanLuongSH2 { get { return GetD(1); } set { SetD(1, value); } }
        public double SanLuongSH3 { get { return GetD(2); } set { SetD(2, value); } }
        public double SanLuongSH4 { get { return GetD(3); } set { SetD(3, value); } }
        public double SanLuongSX { get { return GetD(4); } set { SetD(4, value); } }
        public double SanLuongCC { get { return GetD(5); } set { SetD(5, value); } }
        public double SanLuongHC { get { return GetD(6); } set { SetD(6, value); } }
        public double SanLuongKD { get { return GetD(7); } set { SetD(7, value); } }
        public double TongCongSanLuong { get { return GetD(8); } set { SetD(8, value); } }

        //doanh thu
        public double SanLuongSH1TruocThue { get { return GetD(9); } set { SetD(9, value); } }
        public double SanLuongSH2TruocThue { get { return GetD(10); } set { SetD(10, value); } }
        public double SanLuongSH3TruocThue { get { return GetD(11); } set { SetD(11, value); } }
        public double SanLuongSH4TruocThue { get { return GetD(12); } set { SetD(12, value); } }
        public double SanLuongSXTruocThue { get { return GetD(13); } set { SetD(13, value); } }
        public double SanLuongCCTruocThue { get { return GetD(14); } set { SetD(14, value); } }
        public double SanLuongHCTruocThue { get { return GetD(15); } set { SetD(15, value); } }
        public double SanLuongKDTruocThue { get { return GetD(16); } set { SetD(16, value); } }
        public double TongGiaTruocThue { get { return GetD(17); } set { SetD(17, value); } }

        //VAT
        public double SanLuongSH1VAT { get { return GetD(18); } set { SetD(18, value); } }
        public double SanLuongSH2VAT { get { return GetD(19); } set { SetD(19, value); } }
        public double SanLuongSH3VAT { get { return GetD(20); } set { SetD(20, value); } }
        public double SanLuongSH4VAT { get { return GetD(21); } set { SetD(21, value); } }
        public double SanLuongSXVAT { get { return GetD(22); } set { SetD(22, value); } }
        public double SanLuongCCVAT { get { return GetD(23); } set { SetD(23, value); } }
        public double SanLuongHCVAT { get { return GetD(24); } set { SetD(24, value); } }
        public double SanLuongKDVAT { get { return GetD(25); } set { SetD(25, value); } }
        public double TongVAT { get { return GetD(26); } set { SetD(26, value); } }

        //Phí nước thải
        public double PhiNuocThaiSH1 { get { return GetD(27); } set { SetD(27, value); } }
        public double PhiNuocThaiSH2 { get { return GetD(28); } set { SetD(28, value); } }
        public double PhiNuocThaiSH3 { get { return GetD(29); } set { SetD(29, value); } }
        public double PhiNuocThaiSH4 { get { return GetD(30); } set { SetD(30, value); } }
        public double PhiNuocThaiSX { get { return GetD(31); } set { SetD(31, value); } }
        public double PhiNuocThaiCC { get { return GetD(32); } set { SetD(32, value); } }
        public double PhiNuocThaiHC { get { return GetD(33); } set { SetD(33, value); } }
        public double PhiNuocThaiKD { get { return GetD(34); } set { SetD(34, value); } }
        public double PhiNuocThaiTongCong { get { return GetD(35); } set { SetD(35, value); } }

        //TOngCong total

        public double TongCongSH1 { get { return GetD(36); } set { SetD(36, value); } }
        public double TongCongSH2 { get { return GetD(37); } set { SetD(37, value); } }
        public double TongCongSH3 { get { return GetD(38); } set { SetD(38, value); } }
        public double TongCongSH4 { get { return GetD(39); } set { SetD(39, value); } }
        public double TongCongSX { get { return GetD(40); } set { SetD(40, value); } }
        public double TongCongCC { get { return GetD(41); } set { SetD(41, value); } }
        public double TongCongHC { get { return GetD(42); } set { SetD(42, value); } }
        public double TongCongKD { get { return GetD(43); } set { SetD(43, value); } }
        public double TongCong { get { return GetD(44); } set { SetD(44, value); } }

        public double tuyenOngID { get { return GetD(45); } set { SetD(45, value); } }
        public double tuyenKHID { get { return GetD(46); } set { SetD(46, value); } }
        public double nhanvienID { get { return GetD(47); } set { SetD(47, value); } }
       
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}