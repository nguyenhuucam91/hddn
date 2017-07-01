using HoaDonNuocHaDong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class QuanHuyenHelper
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        public String getTTVoOng(int? quanHuyenID)
        {            
            Quanhuyen qH = db.Quanhuyens.Find(quanHuyenID);
            if (qH != null)
            {
                String ttVoOng = qH.DienThoai + "<br/>" + qH.DienThoai2 + "<br/>" + qH.DienThoai3;
                return ttVoOng;
            }
            return "";
        }

        public int? getQuanIDIfQuanParamIsNull(int tuyenID)
        {
            HoaDonNuocHaDong.Helper.Tuyen tuyen = new HoaDonNuocHaDong.Helper.Tuyen();
            HoaDonNuocHaDong.Helper.NhanVienHelper nhanVien = new NhanVienHelper();
            int nhanvienID = tuyen.getNhanVienIDTuTuyen(tuyenID);
            int qH = nhanVien.getQuanHuyen(nhanvienID);           
            return qH;
        }
    }
}