using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Models
{
    public class SoTienNopTheoThangModel 
    {
        public HDNHD.Models.DataContexts.SoTienNopTheoThang SoTienNopTheoThang { get; set; }
        public long? SoTienTrenHoaDon { get; set; }
        public long? SoTienPhaiNop { get; set; }
    }
}