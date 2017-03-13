using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class BaoCaoSanLuongDoanhThu
    {
        public int STT { get; set; }
        public string CacMuc { get; set; }
        public Nullable<double> SanLuong { get; set; }
        public Nullable<double> DoanhThuTruocThue { get; set; }
        public Nullable<double> VAT { get; set; }
        public Nullable<double> PhiNuocThai { get; set; }
        public Nullable<double> TongCong { get; set; }
    }
}