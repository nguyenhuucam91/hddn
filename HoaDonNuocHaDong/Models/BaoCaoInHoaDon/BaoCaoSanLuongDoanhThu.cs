using HvitFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.BaoCaoInHoaDon
{
    public class BaoCaoSanLuongDoanhThu : ModelBase
    {
        public int SanLuongSH1 { get { return GetINT(0); } set { SetINT(0, value); } }
        public int SanLuongSH2 { get { return GetINT(1); } set { SetINT(1, value); } }
        public int SanLuongSH3 { get { return GetINT(2); } set { SetINT(2, value); } }
        public int SanLuongSH4 { get { return GetINT(3); } set { SetINT(3, value); } }
        public int SanLuongSX { get { return GetINT(4); } set { SetINT(4, value); } }
        public int SanLuongCC { get { return GetINT(5); } set { SetINT(5, value); } }
        public int SanLuongHC { get { return GetINT(6); } set { SetINT(6, value); } }
        public int SanLuongKD { get { return GetINT(7); } set { SetINT(7, value); } }
        public int TongCongSanLuong { get { return GetINT(8); } set { SetINT(8, value); } }

        //doanh thu
        public int SanLuongSH1TruocThue { get { return GetINT(9); } set { SetINT(9, value); } }
        public int SanLuongSH2TruocThue { get { return GetINT(10); } set { SetINT(10, value); } }
        public int SanLuongSH3TruocThue { get { return GetINT(11); } set { SetINT(11, value); } }
        public int SanLuongSH4TruocThue { get { return GetINT(12); } set { SetINT(12, value); } }
        public int SanLuongSXTruocThue { get { return GetINT(13); } set { SetINT(13, value); } }
        public int SanLuongCCTruocThue { get { return GetINT(14); } set { SetINT(14, value); } }
        public int SanLuongHCTruocThue { get { return GetINT(15); } set { SetINT(15, value); } }
        public int SanLuongKDTruocThue { get { return GetINT(16); } set { SetINT(16, value); } }
        public int TongGiaTruocThue { get { return GetINT(17); } set { SetINT(17, value); } }

        //VAT
        public double SanLuongSH1VAT { get { return GetINT(18); } set { SetINT(18, value); } }
        public double SanLuongSH2VAT { get { return GetINT(19); } set { SetINT(19, value); } }
        public double SanLuongSH3VAT { get { return GetINT(20); } set { SetINT(20, value); } }
        public double SanLuongSH4VAT { get { return GetINT(21); } set { SetINT(21, value); } }
        public double SanLuongSXVAT { get { return GetINT(22); } set { SetINT(22, value); } }
        public double SanLuongCCVAT { get { return GetINT(23); } set { SetINT(23, value); } }
        public double SanLuongHCVAT { get { return GetINT(24); } set { SetINT(24, value); } }
        public double SanLuongKDVAT { get { return GetINT(25); } set { SetINT(25, value); } }
        public double TongVAT { get { return GetINT(26); } set { SetINT(26, value); } }

        //Phí nước thải
        public int PhiNuocThaiSH1 { get { return GetINT(27); } set { SetINT(27, value); } }
        public int PhiNuocThaiSH2 { get { return GetINT(28); } set { SetINT(28, value); } }
        public int PhiNuocThaiSH3 { get { return GetINT(29); } set { SetINT(29, value); } }
        public int PhiNuocThaiSH4 { get { return GetINT(30); } set { SetINT(30, value); } }
        public int PhiNuocThaiSX { get { return GetINT(31); } set { SetINT(31, value); } }
        public int PhiNuocThaiCC { get { return GetINT(32); } set { SetINT(32, value); } }
        public int PhiNuocThaiHC { get { return GetINT(33); } set { SetINT(33, value); } }
        public int PhiNuocThaiKD { get { return GetINT(34); } set { SetINT(34, value); } }
        public int PhiNuocThaiTongCong { get { return GetINT(35); } set { SetINT(35, value); } }

        //TOngCong total

        public int TongCongSH1 { get { return GetINT(36); } set { SetINT(36, value); } }
        public int TongCongSH2 { get { return GetINT(37); } set { SetINT(37, value); } }
        public int TongCongSH3 { get { return GetINT(38); } set { SetINT(38, value); } }
        public int TongCongSH4 { get { return GetINT(39); } set { SetINT(39, value); } }
        public int TongCongSX { get { return GetINT(40); } set { SetINT(40, value); } }
        public int TongCongCC { get { return GetINT(41); } set { SetINT(41, value); } }
        public int TongCongHC { get { return GetINT(42); } set { SetINT(42, value); } }
        public int TongCongKD { get { return GetINT(43); } set { SetINT(43, value); } }
        public int TongCong { get { return GetINT(44); } set { SetINT(44, value); } }

        public int tuyenOngID { get { return GetINT(45); } set { SetINT(45, value); } }
        public int tuyenKHID { get { return GetINT(46); } set { SetINT(46, value); } }
        public int nhanvienID { get { return GetINT(47); } set { SetINT(47, value); } }

       
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
}