using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.InHoaDon
{
    public class TuyenTinhTien
    {
        public int HoaDonNuoc { get; set; }
        public String MaKH { get; set; }
        public String TenKH { get; set; }
        public String DiaChi { get; set; }
        public String NgayBatDau { get; set; }
        public String NgayKetThuc { get; set; }
        public double SH1 { get; set; }
        public double SH1Price { get; set; }
        public double SH2 { get; set; }
        public double SH2Price { get; set; }
        public double SH3 { get; set; }
        public double SH3Price { get; set; }
        public double SH4 { get; set; }
        public double SH4Price { get; set; }
        public double HC { get; set; }
        public double HCPrice { get; set; }
        public double CC { get; set; }
        public double CCPrice { get; set; }
        public double SX { get; set; }
        public double SXPrice { get; set; }
        public double KD { get; set; }
        public double KDPrice { get; set; }
        public double TruocThue { get; set; }
        public double PhiVAT { get; set; }
        public double TileBVMT { get; set; }
        public double PhiBVMT { get; set; }
        public int TTDoc { get; set; }
        public int SanLuong { get; set; }
        public double TongCong { get; set; }
        public String TTThuNgan { get; set; }
        public int TuyenKHID { get; set; }
    }
}