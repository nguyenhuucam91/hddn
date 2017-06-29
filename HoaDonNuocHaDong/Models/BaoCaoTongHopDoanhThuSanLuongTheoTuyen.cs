using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class BaoCaoTongHopDoanhThuSanLuongTheoTuyen
    {
        public int Ma { get; set; }
        public string Ten { get; set; }
        public Nullable<int> SoLuongHoaDon { get; set; }
        public Nullable<int> TongSanLuong { get; set; }
        public Nullable<double> SH1 { get; set; }
        public Nullable<double> SH2 { get; set; }
        public Nullable<double> SH3 { get; set; }
        public Nullable<double> SH4 { get; set; }
        public Nullable<double> SX { get; set; }
        public Nullable<double> CC { get; set; }
        public Nullable<double> HCSN { get; set; }
        public Nullable<double> KDDV { get; set; }
        public Nullable<double> DTTT { get; set; }
        public Nullable<double> VAT { get; set; }
        public Nullable<double> PhiNuocThai { get; set; }
        public Nullable<double> TongCong { get; set; }
    }
}