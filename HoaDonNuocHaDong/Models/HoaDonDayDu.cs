using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class HoaDonDayDu
    {
        public Hoadonnuoc h { get; set; }
        public DuCo d { get; set; }

        public HoaDonDayDu(Hoadonnuoc hoadon, DuCo dc)
        {
            h = hoadon;
            d = dc;
        }

        public HoaDonDayDu()
        {
            
        }
    }
}