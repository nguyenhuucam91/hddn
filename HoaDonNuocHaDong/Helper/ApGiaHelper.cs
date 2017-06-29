using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class ApGiaHelper
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        public int isApGiaDacBiet(int? hoaDonID)
        {
            return db.ApGiaDacBiets.Count(p => p.HoaDonNuocID == hoaDonID);
        }

        public String getChiSoApGiaTongHop(int khID,int col)
        {
            List<Apgiatonghop> _apGia = db.Apgiatonghops.Where(p => p.KhachhangID == khID).ToList();
            if (_apGia != null)
            {
                foreach (var item in _apGia)
                {
                    if (item.IDLoaiApGia == col)
                    {
                        double? SL = item.SanLuong;
                        if (SL == null)
                        {
                            return "";
                        }
                        else
                        {
                            return SL.Value.ToString();
                        }
                    }
                }
            }
            return "";
        }
    }
}