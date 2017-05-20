using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Models
{
    public class ModelQuanHuyen
    {
        private HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        public String getTenQuanHuyenFromQuanHuyenId(int quanHuyenID)
        {
            if (quanHuyenID == 0)
            {
                return "";
            }
            else
            {
                Quanhuyen quanHuyen = db.Quanhuyens.Find(quanHuyenID);
                if (quanHuyen != null)
                {
                    return quanHuyen.Ten;
                }                
            }
            return "";
        }
    }
}