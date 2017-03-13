using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models.Hoadonnuocbihuy
{
    public class Hoadonnuocbihuy
    {
        public int id { get; set; }

        public String SoHoaDon { get; set; }

        public String maKH { get; set; }

        public String tenKH { get; set; }

        public double? SoTien { get; set; }

        public String Tuyen { get; set; }

        public String ngayHuy { get; set; }

        public int nguoiHuy { get; set; }

        public String NguoiYeuCauHuy { get; set; }

        public String lidohuy { get; set; }

        public String diachi { get; set; }
    }
}