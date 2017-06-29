using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class ModelNhanVien
    {
        public int NhanvienID { get; set; }
        public String Ten { get; set; }
        public String MaNhanVien { get; set; }
        public int? ChucvuID { get; set; }
    }
}