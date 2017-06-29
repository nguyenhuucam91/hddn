using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.Tuyenong
{
    public class ChiSoCap
    {
        public int TuyenOngID { get; set; }
        public String MaTuyenOng { get; set; }
        public String TenTuyenOng { get; set; }
        public int CapTuyenOng { get; set; }
        public int ChiSoSanLuongTuyenOng { get; set; }
    }
}