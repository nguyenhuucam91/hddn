using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class GiaoDichModel
    {
        public HDNHD.Models.DataContexts.GiaoDich GiaoDich { get; set; }
        public HDNHD.Models.DataContexts.SoTienNopTheoThang SoTienNopTheoThang { get; set; }
        public HDNHD.Models.DataContexts.Hoadonnuoc HoaDon { get; set; }
        public HDNHD.Models.DataContexts.DuCo DuCo { get; set; }
    }

    public class GiaoDichSumModel
    {
        public HDNHD.Models.DataContexts.GiaoDich GiaoDich { get; set; }
        public long? SoTien { get; set; }
        public bool IsChuyenKhoan { get; set; }
    }
}