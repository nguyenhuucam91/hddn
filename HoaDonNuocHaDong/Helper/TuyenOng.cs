using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class TuyenOng
    {
        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();

        public static String getTenTuyenOng(int? tuyenOngID)
        {
           Tuyenong tuyenOngCha = db.Tuyenongs.Where(p => p.TuyenongID == tuyenOngID).FirstOrDefault();
           if (tuyenOngCha != null)
           {
               return tuyenOngCha.Tentuyen;
           }
           return "";
        }

        /// <summary>
        /// Lấy tên cấp tuyến ống
        /// </summary>
        /// <param name="tuyenOngID"></param>
        /// <returns></returns>
        public static int getCapTuyenOng(int? tuyenOngID)
        {
            Tuyenong tuyenOngCha = db.Tuyenongs.Where(p => p.TuyenongID == tuyenOngID).FirstOrDefault();
            if (tuyenOngCha != null)
            {
                return tuyenOngCha.CaptuyenID.Value;
            }
            return 0;
        }
    }
}